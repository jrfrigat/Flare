using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace Querio.Linq;

/// <summary>
/// Bridges the query model's semantic types and the CLR types a .NET object actually has.
/// <para>
/// A query says a field is a number; the object holding it may use <c>int</c>, <c>long</c>,
/// <c>decimal</c> or a nullable of any of them. Everything that reconciles the two lives here, so
/// the renderer can stay about meaning rather than about conversions.
/// </para>
/// </summary>
public static class QueryClrValue
{
    /// <summary>Finds the property or field a query field reads, trying its physical name first.</summary>
    /// <param name="type">The type holding the member.</param>
    /// <param name="field">The query field to resolve.</param>
    /// <returns>The member, or null when the type has neither name.</returns>
    public static MemberInfo? FindMember(Type type, QueryField field)
        => FindMember(type, field.PhysicalName) ?? FindMember(type, field.Key);

    /// <summary>Finds a property or field by name, ignoring case.</summary>
    /// <param name="type">The type holding the member.</param>
    /// <param name="name">The member name to look for.</param>
    /// <returns>The member, or null when the type has no such name.</returns>
    public static MemberInfo? FindMember(Type type, string name)
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;
        return (MemberInfo?)type.GetProperty(name, flags) ?? type.GetField(name, flags);
    }

    /// <summary>What a member yields.</summary>
    /// <param name="member">The property or field.</param>
    public static Type MemberType(MemberInfo member)
        => member is PropertyInfo property ? property.PropertyType : ((FieldInfo)member).FieldType;

    /// <summary>The type behind a nullable, or the type itself when it is not one.</summary>
    /// <param name="type">The type to unwrap.</param>
    public static Type NonNullable(Type type) => Nullable.GetUnderlyingType(type) ?? type;

    /// <summary>Whether a value of this type can be null.</summary>
    /// <param name="type">The type to test.</param>
    public static bool AcceptsNull(Type type) => !type.IsValueType || Nullable.GetUnderlyingType(type) is not null;

    /// <summary>
    /// Brings an expression to the type it is being used as. A fixed value is converted while the
    /// query is being built, so the tree carries a constant of the right type rather than a
    /// conversion a LINQ provider would then have to translate.
    /// </summary>
    /// <param name="value">The expression to bring across.</param>
    /// <param name="target">The type it has to be.</param>
    public static Expression Coerce(Expression value, Type target)
    {
        if (value.Type == target) return value;

        if (value is ConstantExpression constant)
        {
            if (constant.Value is null)
            {
                return Expression.Constant(null, AcceptsNull(target) ? target : typeof(object));
            }
            return Expression.Constant(ChangeType(constant.Value, NonNullable(target)), target);
        }

        // Widening a value to its nullable form, or narrowing back, is what a lifted comparison needs.
        return Expression.Convert(value, target);
    }

    /// <summary>Brings both sides of a comparison to one type, preferring the left side's.</summary>
    /// <param name="left">The left side, whose type usually wins.</param>
    /// <param name="right">The right side.</param>
    public static (Expression Left, Expression Right) Align(Expression left, Expression right)
    {
        if (left.Type == right.Type) return (left, right);

        // A nullable side decides, since comparing a nullable against a plain value has to lift.
        if (NonNullable(left.Type) == NonNullable(right.Type))
        {
            return AcceptsNull(left.Type)
                ? (left, Coerce(right, left.Type))
                : (Coerce(left, right.Type), right);
        }

        return (left, Coerce(right, left.Type));
    }

    /// <summary>
    /// Reads a value as another type. Handles the cases a query produces that
    /// <see cref="Convert.ChangeType(object, Type)"/> will not: identifiers and enum members arrive
    /// as text, because that is how the query model stores every value.
    /// </summary>
    /// <param name="value">The value to read.</param>
    /// <param name="target">The type to read it as, already stripped of nullability.</param>
    public static object? ChangeType(object? value, Type target)
    {
        if (value is null) return null;
        if (target.IsInstanceOfType(value)) return value;

        if (target == typeof(Guid)) return value is Guid id ? id : Guid.Parse(Convert.ToString(value, CultureInfo.InvariantCulture)!);
        if (target.IsEnum)
        {
            return value is string name
                ? Enum.Parse(target, name, ignoreCase: true)
                : Enum.ToObject(target, value);
        }
        if (target == typeof(DateTimeOffset))
        {
            return value is DateTime moment
                ? new DateTimeOffset(moment)
                : DateTimeOffset.Parse(Convert.ToString(value, CultureInfo.InvariantCulture)!, CultureInfo.InvariantCulture);
        }
        if (target == typeof(string)) return Convert.ToString(value, CultureInfo.InvariantCulture);

        return Convert.ChangeType(value, target, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Collapses a moment to the start of the period containing it. This is what turns a stream of
    /// events into a series: the same truncation has to be applied on both sides of a grouping.
    /// </summary>
    /// <param name="moment">The moment to collapse.</param>
    /// <param name="truncation">The period to collapse it to.</param>
    public static DateTime Truncate(DateTime moment, QueryDateTruncation truncation) => truncation switch
    {
        QueryDateTruncation.Minute => new DateTime(moment.Year, moment.Month, moment.Day, moment.Hour, moment.Minute, 0, moment.Kind),
        QueryDateTruncation.Hour => new DateTime(moment.Year, moment.Month, moment.Day, moment.Hour, 0, 0, moment.Kind),
        QueryDateTruncation.Day => moment.Date,
        // Weeks start on Monday, matching ISO 8601 rather than the calling thread's culture.
        QueryDateTruncation.Week => moment.Date.AddDays(-(((int)moment.DayOfWeek + 6) % 7)),
        QueryDateTruncation.Month => new DateTime(moment.Year, moment.Month, 1, 0, 0, 0, moment.Kind),
        QueryDateTruncation.Quarter => new DateTime(moment.Year, (((moment.Month - 1) / 3) * 3) + 1, 1, 0, 0, 0, moment.Kind),
        QueryDateTruncation.Year => new DateTime(moment.Year, 1, 1, 0, 0, 0, moment.Kind),
        _ => moment,
    };

    /// <summary>Moves a moment by a signed offset, which is how a relative window is pinned down.</summary>
    /// <param name="moment">The moment to move.</param>
    /// <param name="offset">The signed offset to apply.</param>
    public static DateTime Shift(DateTime moment, QueryRelativeValue offset) => offset.Unit switch
    {
        QueryTimeUnit.Minute => moment.AddMinutes(offset.Amount),
        QueryTimeUnit.Hour => moment.AddHours(offset.Amount),
        QueryTimeUnit.Day => moment.AddDays(offset.Amount),
        QueryTimeUnit.Week => moment.AddDays(offset.Amount * 7),
        QueryTimeUnit.Month => moment.AddMonths(offset.Amount),
        QueryTimeUnit.Quarter => moment.AddMonths(offset.Amount * 3),
        QueryTimeUnit.Year => moment.AddYears(offset.Amount),
        _ => moment,
    };

    /// <summary>
    /// Orders two values the way a result set does, putting nothing before everything. Used for
    /// sorting, and for the smallest and largest aggregates.
    /// </summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static int Compare(object? left, object? right)
    {
        if (left is null) return right is null ? 0 : -1;
        if (right is null) return 1;
        if (left is IComparable comparable && left.GetType() == right.GetType()) return comparable.CompareTo(right);

        // Different numeric types still order against each other once both are read as one.
        if (IsNumeric(left) && IsNumeric(right))
        {
            return Convert.ToDouble(left, CultureInfo.InvariantCulture)
                .CompareTo(Convert.ToDouble(right, CultureInfo.InvariantCulture));
        }
        return Comparer<object>.Default.Compare(left, right);
    }

    private static bool IsNumeric(object value)
        => value is byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal;
}
