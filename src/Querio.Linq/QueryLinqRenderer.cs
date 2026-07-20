using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Querio.Linq;

/// <summary>
/// Renders a query as .NET expression trees. What each node means here is a real operator rather
/// than a piece of text, which is the point: the same query that renders to SQL also compiles to
/// code, and nothing about the model had to bend to allow it.
/// </summary>
internal abstract class QueryLinqRenderer : QueryRenderer<Expression>
{
    private static readonly MethodInfo StringContains =
        typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;

    private static readonly MethodInfo StringStartsWith =
        typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string)])!;

    private static readonly MethodInfo StringEndsWith =
        typeof(string).GetMethod(nameof(string.EndsWith), [typeof(string)])!;

    private static readonly MethodInfo TruncateMethod =
        typeof(QueryClrValue).GetMethod(nameof(QueryClrValue.Truncate))!;

    protected QueryLinqRenderer(QuerySpec spec, QuerySchema schema, QueryFunctionLibrary library, DateTime now)
        : base(spec, schema)
    {
        Library = library ?? QueryFunctionLibrary.Empty;
        Now = now;
    }

    /// <summary>
    /// Everything a LINQ provider can do, except the outer joins that keep unmatched rows from the
    /// side being added. Those have no natural shape over a sequence of objects, and approximating
    /// them would answer a different question.
    /// </summary>
    public static IQueryCapabilities Capabilities { get; } =
        QueryCapabilities.All.Without(QueryFeature.RightJoin, QueryFeature.FullJoin);

    /// <summary>The .NET behind the schema's declared functions.</summary>
    protected QueryFunctionLibrary Library { get; }

    /// <summary>
    /// The moment a relative window is measured from. Pinned once per run so every condition in one
    /// query agrees on when "now" was, and so a predicate handed to a provider carries a fixed value.
    /// </summary>
    protected DateTime Now { get; }

    /// <inheritdoc/>
    protected override string TargetName => "The LINQ renderer";

    /// <summary>The expression yielding the object a participant's fields are read from.</summary>
    /// <param name="alias">Alias of the participant.</param>
    protected abstract Expression Participant(string alias);

    /// <inheritdoc/>
    protected override Expression Field(string alias, QueryField field)
    {
        var instance = Participant(alias);
        var member = QueryClrValue.FindMember(instance.Type, field)
            ?? throw new QueryRenderException(
                $"'{instance.Type.Name}' has no property or field named '{field.PhysicalName}', " +
                $"which '{alias}.{field.Key}' needs.");
        return MemberAccess(alias, instance, member);
    }

    /// <summary>
    /// Reads one member off a participant. Overridable because a participant is not always there:
    /// an outer join leaves it absent, and reading through an absent one has to yield nothing rather
    /// than fail.
    /// </summary>
    /// <param name="alias">Alias of the participant being read.</param>
    /// <param name="instance">The expression yielding the participant.</param>
    /// <param name="member">The property or field to read.</param>
    protected virtual Expression MemberAccess(string alias, Expression instance, MemberInfo member)
        => Expression.MakeMemberAccess(instance, member);

    /// <inheritdoc/>
    protected override Expression Literal(object? value, QueryFieldType type)
        => value is null
            ? Expression.Constant(null, typeof(object))
            : Expression.Constant(value, value.GetType());

    /// <inheritdoc/>
    protected override Expression Relative(QueryRelativeValue offset)
        => Expression.Constant(QueryClrValue.Shift(Now, offset), typeof(DateTime));

    /// <inheritdoc/>
    protected override Expression Call(QueryFunction function, IReadOnlyList<Expression> arguments)
        => Library.Invoke(function.Key, arguments);

    /// <inheritdoc/>
    protected override Expression Comparison(
        Expression left, QueryOperator op, QueryFieldType type, Expression? right, Expression? upper)
    {
        switch (op)
        {
            case QueryOperator.IsNull:
            case QueryOperator.IsNotNull:
                // A field the CLR cannot leave empty is never null, whatever the schema declares.
                if (!QueryClrValue.AcceptsNull(left.Type)) return Expression.Constant(op == QueryOperator.IsNotNull);
                return op == QueryOperator.IsNull
                    ? Expression.Equal(left, Expression.Constant(null, left.Type))
                    : Expression.NotEqual(left, Expression.Constant(null, left.Type));

            case QueryOperator.Contains:
                return Text(left, right!, StringContains);
            case QueryOperator.StartsWith:
                return Text(left, right!, StringStartsWith);
            case QueryOperator.EndsWith:
                return Text(left, right!, StringEndsWith);

            case QueryOperator.Between:
                return Expression.AndAlso(
                    Binary(left, right!, Expression.GreaterThanOrEqual),
                    Binary(left, upper!, Expression.LessThanOrEqual));
            case QueryOperator.NotBetween:
                return Expression.Not(Expression.AndAlso(
                    Binary(left, right!, Expression.GreaterThanOrEqual),
                    Binary(left, upper!, Expression.LessThanOrEqual)));

            case QueryOperator.NotEquals:
                return Binary(left, right!, Expression.NotEqual);
            case QueryOperator.GreaterThan:
                return Binary(left, right!, Expression.GreaterThan);
            case QueryOperator.GreaterThanOrEqual:
                return Binary(left, right!, Expression.GreaterThanOrEqual);
            case QueryOperator.LessThan:
                return Binary(left, right!, Expression.LessThan);
            case QueryOperator.LessThanOrEqual:
                return Binary(left, right!, Expression.LessThanOrEqual);

            default:
                return Binary(left, right!, Expression.Equal);
        }
    }

    /// <inheritdoc/>
    protected override Expression Membership(Expression left, QueryOperator op, IReadOnlyList<Expression> values)
    {
        // A typed list rather than a chain of equality tests: that is the shape a LINQ provider
        // recognises as set membership, and it stays one node however long the set gets.
        var elementType = left.Type;
        var listType = typeof(List<>).MakeGenericType(elementType);
        var list = (System.Collections.IList)Activator.CreateInstance(listType)!;
        foreach (var value in values)
        {
            var constant = (ConstantExpression)QueryClrValue.Coerce(value, elementType);
            list.Add(constant.Value);
        }

        var contains = listType.GetMethod(nameof(List<int>.Contains), [elementType])!;
        var test = Expression.Call(Expression.Constant(list, listType), contains, left);
        return op == QueryOperator.NotIn ? Expression.Not(test) : test;
    }

    /// <inheritdoc/>
    protected override Expression Combine(bool or, IReadOnlyList<Expression> parts)
    {
        var combined = parts[0];
        for (var i = 1; i < parts.Count; i++)
        {
            combined = or ? Expression.OrElse(combined, parts[i]) : Expression.AndAlso(combined, parts[i]);
        }
        return combined;
    }

    /// <summary>
    /// Renders a value, collapsing a moment to the start of its period when asked. Grouping and
    /// selecting both go through here so the two cannot disagree about the buckets.
    /// </summary>
    /// <param name="field">The field, when the value is one.</param>
    /// <param name="call">The call, when the value is one.</param>
    /// <param name="truncate">The period to collapse a moment to, or null to keep it exact.</param>
    protected Expression ValueOf(QueryFieldRef? field, QueryFunctionCall? call, QueryDateTruncation? truncate)
    {
        var value = Value(field, call);
        if (truncate is null) return value;

        var moment = QueryClrValue.NonNullable(value.Type) == typeof(DateTime)
            ? QueryClrValue.Coerce(value, typeof(DateTime))
            : throw new QueryRenderException(
                $"A period can only be taken from a moment, and this value is {value.Type.Name}.",
                QueryFeature.DateTruncation);

        return Expression.Call(TruncateMethod, moment, Expression.Constant(truncate.Value));
    }

    private static Expression Binary(
        Expression left, Expression right, Func<Expression, Expression, BinaryExpression> build)
    {
        var (alignedLeft, alignedRight) = QueryClrValue.Align(left, right);
        return build(alignedLeft, alignedRight);
    }

    private static Expression Text(Expression left, Expression right, MethodInfo method)
    {
        var subject = left.Type == typeof(string)
            ? left
            : Expression.Call(QueryClrValue.AcceptsNull(left.Type) ? left : Expression.Convert(left, typeof(object)),
                typeof(object).GetMethod(nameof(ToString))!);
        var pattern = QueryClrValue.Coerce(right, typeof(string));

        // Nothing contains anything, so an absent value fails the test rather than throwing.
        return Expression.AndAlso(
            Expression.NotEqual(subject, Expression.Constant(null, typeof(string))),
            Expression.Call(subject, method, pattern));
    }
}
