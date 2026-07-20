using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Querio.Linq;

/// <summary>
/// A table function's rows, together with the type its rows are, so fields can be read off them.
/// </summary>
/// <param name="ElementType">The CLR type of one row.</param>
/// <param name="Rows">The rows the call produced.</param>
public sealed record QueryTableResult(Type ElementType, IEnumerable Rows);

/// <summary>
/// Supplies the .NET behind the functions a schema declares.
/// <para>
/// A schema says a function exists and what it takes; it deliberately does not say what it does.
/// A SQL renderer resolves that to a routine name, and this resolves it to actual code. Registering
/// nothing is a safe default: calling an unregistered function fails loudly rather than guessing.
/// </para>
/// </summary>
public sealed class QueryFunctionLibrary
{
    private readonly Dictionary<string, Func<IReadOnlyList<Expression>, Expression>> _values =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<string, Func<IReadOnlyList<object?>, QueryTableResult>> _tables =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>A library with nothing registered, for queries that call no functions.</summary>
    public static QueryFunctionLibrary Empty { get; } = new();

    /// <summary>
    /// Registers a value function as an expression the renderer inlines into the query it builds.
    /// A LINQ provider therefore sees the body itself rather than an opaque call.
    /// </summary>
    /// <param name="key">The function key as the schema declares it.</param>
    /// <param name="body">The lambda to inline. Its parameter count must match how the query calls it.</param>
    public QueryFunctionLibrary Register(string key, LambdaExpression body)
    {
        if (body is null) throw new ArgumentNullException(nameof(body));
        _values[key] = arguments =>
        {
            if (arguments.Count != body.Parameters.Count)
            {
                throw new QueryRenderException(
                    $"'{key}' is registered with {body.Parameters.Count} parameter(s) but the query calls it with {arguments.Count}.");
            }
            var bound = new List<Expression>(arguments.Count);
            for (var i = 0; i < arguments.Count; i++)
            {
                bound.Add(QueryClrValue.Coerce(arguments[i], body.Parameters[i].Type));
            }
            return Expression.Invoke(body, bound);
        };
        return this;
    }

    /// <summary>Registers a one-argument value function.</summary>
    /// <typeparam name="TArg">The argument type.</typeparam>
    /// <typeparam name="TResult">What the function yields.</typeparam>
    /// <param name="key">The function key as the schema declares it.</param>
    /// <param name="body">The lambda to inline.</param>
    public QueryFunctionLibrary Register<TArg, TResult>(string key, Expression<Func<TArg, TResult>> body)
        => Register(key, (LambdaExpression)body);

    /// <summary>Registers a two-argument value function.</summary>
    /// <typeparam name="TFirst">The first argument type.</typeparam>
    /// <typeparam name="TSecond">The second argument type.</typeparam>
    /// <typeparam name="TResult">What the function yields.</typeparam>
    /// <param name="key">The function key as the schema declares it.</param>
    /// <param name="body">The lambda to inline.</param>
    public QueryFunctionLibrary Register<TFirst, TSecond, TResult>(
        string key, Expression<Func<TFirst, TSecond, TResult>> body)
        => Register(key, (LambdaExpression)body);

    /// <summary>
    /// Registers a value function as a factory over the already-built argument expressions. Use this
    /// when the shape of the result depends on how many arguments the query supplied, which is what
    /// an optional parameter means.
    /// </summary>
    /// <param name="key">The function key as the schema declares it.</param>
    /// <param name="factory">Builds the expression for one call.</param>
    public QueryFunctionLibrary Register(string key, Func<IReadOnlyList<Expression>, Expression> factory)
    {
        _values[key] = factory ?? throw new ArgumentNullException(nameof(factory));
        return this;
    }

    /// <summary>
    /// Registers a table function: given the call's arguments, it produces the rows that stand in
    /// the query wherever an entity otherwise would.
    /// </summary>
    /// <typeparam name="TRow">The type of one row.</typeparam>
    /// <param name="key">The function key as the schema declares it.</param>
    /// <param name="rows">Produces the rows for one call. Arguments arrive already read into .NET values.</param>
    public QueryFunctionLibrary RegisterTable<TRow>(string key, Func<IReadOnlyList<object?>, IEnumerable<TRow>> rows)
    {
        if (rows is null) throw new ArgumentNullException(nameof(rows));
        _tables[key] = arguments => new QueryTableResult(typeof(TRow), rows(arguments));
        return this;
    }

    /// <summary>Builds the expression for a call to a value function.</summary>
    /// <param name="key">The function key as the schema declares it.</param>
    /// <param name="arguments">The already-built argument expressions.</param>
    /// <exception cref="QueryRenderException">Nothing is registered under that key.</exception>
    public Expression Invoke(string key, IReadOnlyList<Expression> arguments)
    {
        if (!_values.TryGetValue(key, out var factory))
        {
            throw new QueryRenderException(
                $"No .NET implementation is registered for the function '{key}'. " +
                $"Register one on {nameof(QueryFunctionLibrary)} before running the query.",
                QueryFeature.ValueFunctions);
        }
        return factory(arguments);
    }

    /// <summary>Produces the rows of a table function.</summary>
    /// <param name="key">The function key as the schema declares it.</param>
    /// <param name="arguments">The call's arguments, already read into .NET values.</param>
    /// <exception cref="QueryRenderException">Nothing is registered under that key.</exception>
    public QueryTableResult Table(string key, IReadOnlyList<object?> arguments)
    {
        if (!_tables.TryGetValue(key, out var factory))
        {
            throw new QueryRenderException(
                $"No .NET implementation is registered for the table function '{key}'. " +
                $"Register one on {nameof(QueryFunctionLibrary)} before running the query.",
                QueryFeature.TableFunctions);
        }
        return factory(arguments);
    }
}
