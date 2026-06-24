using System.Collections;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace Flare.Components;

/// <summary>
/// Translates a <see cref="DataGridRequest"/> (filters, sorts, paging) into LINQ expression trees and
/// applies them to an <see cref="IQueryable{T}"/>. Because the predicates and key selectors are built
/// from standard expression nodes, an Entity Framework Core source runs the sort/filter/page entirely
/// in the database - no hand-written query code. Works equally on an in-memory
/// <c>list.AsQueryable()</c>. Column keys are matched to public properties/fields by name.
/// </summary>
public static class DataGridQuery
{
    private static readonly MethodInfo StringToLower = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;

    /// <summary>Applies the request's filters then sorts to <paramref name="source"/> (no paging).</summary>
    public static IQueryable<T> ApplyFiltersAndSorts<T>(IQueryable<T> source, DataGridRequest request)
        => ApplySorts(ApplyFilters(source, request.FilterModel), request.Sorts);

    /// <summary>Runs the full request against <paramref name="source"/>: filter, sort, count (before
    /// paging) and page, materialising the page synchronously. For EF Core this executes as SQL.</summary>
    public static DataGridResult<T> Execute<T>(IQueryable<T> source, DataGridRequest request)
    {
        var filteredSorted = ApplyFiltersAndSorts(source, request);
        var total = filteredSorted.Count();
        // Honor a windowed (StartIndex/Count) request from the virtualization path; otherwise page.
        var skip = request.StartIndex ?? Math.Max(0, request.Page) * Math.Max(1, request.PageSize);
        var take = request.Count ?? Math.Max(1, request.PageSize);
        var page = filteredSorted.Skip(Math.Max(0, skip)).Take(Math.Max(1, take)).ToList();
        return new DataGridResult<T>(page, total);
    }

    /// <summary>Folds each filter into a <c>Where</c> (AND). Filters whose column or operator cannot be
    /// translated are skipped rather than throwing.</summary>
    public static IQueryable<T> ApplyFilters<T>(IQueryable<T> source, IEnumerable<DataGridFilter>? filters)
    {
        if (filters is null) return source;
        foreach (var filter in filters)
        {
            var predicate = BuildPredicate<T>(filter);
            if (predicate is not null) source = source.Where(predicate);
        }
        return source;
    }

    /// <summary>Applies multi-column ordering by property name (first sort is OrderBy, rest ThenBy).</summary>
    public static IQueryable<T> ApplySorts<T>(IQueryable<T> source, IReadOnlyList<DataGridSort>? sorts)
    {
        if (sorts is null || sorts.Count == 0) return source;
        IOrderedQueryable<T>? ordered = null;
        foreach (var sort in sorts)
        {
            if (string.IsNullOrEmpty(sort.Key)) continue;
            var param = Expression.Parameter(typeof(T), "x");
            var member = ResolveMember(param, sort.Key);
            if (member is null) continue;
            var keySelector = Expression.Lambda(member, param);
            var asc = sort.Direction == SortDirection.Ascending;
            var method = ordered is null
                ? (asc ? "OrderBy" : "OrderByDescending")
                : (asc ? "ThenBy" : "ThenByDescending");
            var call = Expression.Call(typeof(Queryable), method, [typeof(T), member.Type],
                (ordered ?? source).Expression, Expression.Quote(keySelector));
            ordered = (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);
        }
        return ordered ?? source;
    }

    private static Expression<Func<T, bool>>? BuildPredicate<T>(DataGridFilter filter)
    {
        if (string.IsNullOrEmpty(filter.Key)) return null;
        var param = Expression.Parameter(typeof(T), "x");
        var member = ResolveMember(param, filter.Key);
        if (member is null) return null;

        var type = member.Type;
        var underlying = Nullable.GetUnderlyingType(type) ?? type;

        var body = filter.Operator switch
        {
            FilterOperator.Contains => StringOp(member, nameof(string.Contains), filter.Value),
            FilterOperator.StartsWith => StringOp(member, nameof(string.StartsWith), filter.Value),
            FilterOperator.EndsWith => StringOp(member, nameof(string.EndsWith), filter.Value),
            FilterOperator.Equals => Equality(member, type, underlying, filter.Value, negate: false),
            FilterOperator.NotEquals => Equality(member, type, underlying, filter.Value, negate: true),
            FilterOperator.GreaterThan => Comparison(member, type, underlying, filter.Value, ExpressionType.GreaterThan),
            FilterOperator.GreaterThanOrEqual => Comparison(member, type, underlying, filter.Value, ExpressionType.GreaterThanOrEqual),
            FilterOperator.LessThan => Comparison(member, type, underlying, filter.Value, ExpressionType.LessThan),
            FilterOperator.LessThanOrEqual => Comparison(member, type, underlying, filter.Value, ExpressionType.LessThanOrEqual),
            FilterOperator.Between => Between(member, type, underlying, filter.Value, filter.Value2, negate: false),
            FilterOperator.NotBetween => Between(member, type, underlying, filter.Value, filter.Value2, negate: true),
            FilterOperator.In => InList(member, type, underlying, filter.Values),
            FilterOperator.NotIn => Negate(InList(member, type, underlying, filter.Values)),
            FilterOperator.IsNull => NullCheck(member, type, underlying, wantNull: true),
            FilterOperator.IsNotNull => NullCheck(member, type, underlying, wantNull: false),
            _ => null,
        };

        return body is null ? null : Expression.Lambda<Func<T, bool>>(body, param);
    }

    // Case-insensitive string Contains/StartsWith/EndsWith, null-guarded.
    private static Expression? StringOp(Expression member, string method, string? value)
    {
        if (member.Type != typeof(string) || value is null) return null;
        var call = Expression.Call(Expression.Call(member, StringToLower),
            typeof(string).GetMethod(method, [typeof(string)])!,
            Expression.Constant(value.ToLowerInvariant()));
        return Expression.AndAlso(Expression.NotEqual(member, Expression.Constant(null, typeof(string))), call);
    }

    private static Expression? Equality(Expression member, Type type, Type underlying, string? value, bool negate)
    {
        Expression body;
        if (underlying == typeof(string))
        {
            if (value is null) return null;
            var eq = Expression.Equal(Expression.Call(member, StringToLower),
                Expression.Constant(value.ToLowerInvariant(), typeof(string)));
            body = Expression.AndAlso(Expression.NotEqual(member, Expression.Constant(null, typeof(string))), eq);
        }
        else
        {
            var parsed = ParseValue(underlying, value);
            if (parsed is null) return null;
            body = Expression.Equal(member, Expression.Constant(parsed, type));
        }
        return negate ? Expression.Not(body) : body;
    }

    private static Expression? Comparison(Expression member, Type type, Type underlying, string? value, ExpressionType op)
    {
        var parsed = ParseValue(underlying, value);
        if (parsed is null) return null;
        var constant = Expression.Constant(parsed, type);
        return Expression.MakeBinary(op, member, constant);
    }

    private static Expression? Between(Expression member, Type type, Type underlying, string? lo, string? hi, bool negate)
    {
        var low = Comparison(member, type, underlying, lo, ExpressionType.GreaterThanOrEqual);
        var high = Comparison(member, type, underlying, hi, ExpressionType.LessThanOrEqual);
        if (low is null || high is null) return null;
        var inRange = Expression.AndAlso(low, high);
        return negate ? Expression.Not(inRange) : inRange;
    }

    private static Expression? InList(Expression member, Type type, Type underlying, IReadOnlyList<string>? values)
    {
        if (values is null || values.Count == 0) return null;
        var listType = typeof(List<>).MakeGenericType(type);
        var list = (IList)Activator.CreateInstance(listType)!;
        foreach (var v in values)
        {
            var parsed = underlying == typeof(string) ? v : ParseValue(underlying, v);
            if (parsed is not null || underlying == typeof(string)) list.Add(parsed);
        }
        var contains = listType.GetMethod(nameof(List<object>.Contains), [type])!;
        return Expression.Call(Expression.Constant(list, listType), contains, member);
    }

    private static Expression? NullCheck(Expression member, Type type, Type underlying, bool wantNull)
    {
        Expression body;
        if (underlying == typeof(string))
        {
            var isNullOrEmpty = typeof(string).GetMethod(nameof(string.IsNullOrEmpty), [typeof(string)])!;
            body = Expression.Call(isNullOrEmpty, member);
        }
        else if (Nullable.GetUnderlyingType(type) is not null || !type.IsValueType)
        {
            body = Expression.Equal(member, Expression.Constant(null, type));
        }
        else
        {
            body = Expression.Constant(false); // a non-nullable value type is never null
        }
        return wantNull ? body : Expression.Not(body);
    }

    private static Expression? Negate(Expression? e) => e is null ? null : Expression.Not(e);

    // Resolves a column key to a public property or field on T; null when there is no such member.
    private static Expression? ResolveMember(Expression param, string key)
    {
        try { return Expression.PropertyOrField(param, key); }
        catch { return null; }
    }

    private static object? ParseValue(Type t, string? raw)
    {
        if (raw is null) return null;
        if (t == typeof(string)) return raw;
        if (raw.Length == 0) return null;
        try
        {
            if (t.IsEnum) return Enum.Parse(t, raw, ignoreCase: true);
            if (t == typeof(Guid)) return Guid.Parse(raw);
            if (t == typeof(DateTime)) return DateTime.Parse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None);
            if (t == typeof(DateTimeOffset)) return DateTimeOffset.Parse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None);
            if (t == typeof(DateOnly)) return DateOnly.Parse(raw, CultureInfo.InvariantCulture);
            if (t == typeof(TimeOnly)) return TimeOnly.Parse(raw, CultureInfo.InvariantCulture);
            if (t == typeof(bool)) return bool.Parse(raw);
            return Convert.ChangeType(raw, t, CultureInfo.InvariantCulture);
        }
        catch { return null; }
    }
}
