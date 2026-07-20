using System.Collections;
using System.Collections.Generic;

namespace Querio.Linq;

/// <summary>One column of a result: the name the query gave it and the type its values are.</summary>
/// <param name="Name">The output name, as the query's alias set it.</param>
/// <param name="ClrType">The type every value in this column has.</param>
public sealed record QueryResultColumn(string Name, Type ClrType);

/// <summary>
/// What running a query produced. Rows are plain arrays rather than a generated type, because the
/// shape of a result is decided by the query at run time and there is no compile-time type to have.
/// </summary>
public sealed class QueryResult
{
    private readonly Dictionary<string, int> _index;

    internal QueryResult(IReadOnlyList<QueryResultColumn> columns, IReadOnlyList<object?[]> rows)
    {
        Columns = columns;
        Rows = rows;
        _index = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < columns.Count; i++) _index[columns[i].Name] = i;
    }

    /// <summary>The columns, in the order the query selected them.</summary>
    public IReadOnlyList<QueryResultColumn> Columns { get; }

    /// <summary>The rows, each holding one value per column.</summary>
    public IReadOnlyList<object?[]> Rows { get; }

    /// <summary>Reads one value by column name.</summary>
    /// <param name="row">Zero-based row position.</param>
    /// <param name="column">The output name of the column.</param>
    /// <exception cref="KeyNotFoundException">The result has no such column.</exception>
    public object? this[int row, string column] => Rows[row][IndexOf(column)];

    /// <summary>The position of a column by name.</summary>
    /// <param name="column">The output name of the column.</param>
    /// <exception cref="KeyNotFoundException">The result has no such column.</exception>
    public int IndexOf(string column)
        => _index.TryGetValue(column, out var found)
            ? found
            : throw new KeyNotFoundException($"The result has no column named '{column}'.");

    /// <summary>
    /// Reads one column across every row, converted to the type asked for. Handy for asserting on a
    /// result, and for feeding one column into something that expects a plain sequence.
    /// </summary>
    /// <typeparam name="T">The type to read the values as.</typeparam>
    /// <param name="column">The output name of the column.</param>
    public IReadOnlyList<T> Column<T>(string column)
    {
        var position = IndexOf(column);
        var values = new List<T>(Rows.Count);
        foreach (var row in Rows)
        {
            values.Add((T)QueryClrValue.ChangeType(row[position], QueryClrValue.NonNullable(typeof(T)))!);
        }
        return values;
    }

    /// <summary>Presents the rows as dictionaries, for serialising or binding to a generic grid.</summary>
    public IEnumerable<IReadOnlyDictionary<string, object?>> AsDictionaries()
    {
        foreach (var row in Rows)
        {
            var map = new Dictionary<string, object?>(Columns.Count, StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < Columns.Count; i++) map[Columns[i].Name] = row[i];
            yield return map;
        }
    }
}

/// <summary>
/// The objects a query runs over: one sequence per entity the schema declares. Binding by entity
/// rather than by alias is deliberate, since the same entity joined to itself is still one source.
/// </summary>
public sealed class QuerySources
{
    private readonly Dictionary<string, (Type Type, IEnumerable Rows)> _byEntity =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Binds a sequence to an entity.</summary>
    /// <typeparam name="T">The type of one row.</typeparam>
    /// <param name="entityKey">The entity key as the schema declares it.</param>
    /// <param name="rows">The objects standing for that entity.</param>
    public QuerySources Add<T>(string entityKey, IEnumerable<T> rows)
    {
        if (rows is null) throw new ArgumentNullException(nameof(rows));
        _byEntity[entityKey] = (typeof(T), rows);
        return this;
    }

    internal bool TryGet(string entityKey, out Type type, out IEnumerable rows)
    {
        if (_byEntity.TryGetValue(entityKey, out var found))
        {
            type = found.Type;
            rows = found.Rows;
            return true;
        }
        type = typeof(object);
        rows = Array.Empty<object>();
        return false;
    }
}
