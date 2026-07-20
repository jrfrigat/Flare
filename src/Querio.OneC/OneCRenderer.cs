using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Querio.OneC;

/// <summary>
/// Turns a query into 1C:Enterprise query text plus the parameters it refers to.
/// <para>
/// Two things differ from a SQL target and shape this renderer. 1C has no way to quote an
/// identifier, so names are validated instead of escaped; and its query language has no function for
/// the current moment, so a relative time window is resolved into a parameter value as the query is
/// rendered. That keeps the window relative where it matters - the spec still says "the last 30
/// days", and every render resolves it afresh.
/// </para>
/// <para>
/// Keywords are emitted in Russian, which is what 1C developers read and what its documentation
/// uses. They are output data, not source identifiers.
/// </para>
/// </summary>
public sealed class OneCRenderer
{
    private readonly QuerySpec _spec;
    private readonly QuerySchema _schema;
    private readonly DateTime _now;
    private readonly List<OneCQueryParameter> _parameters = [];
    private readonly Dictionary<string, IReadOnlyList<QueryField>> _fields = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<KeyValuePair<string, string>> _ordered = [];
    private readonly Dictionary<string, string> _outputExpressions = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, QueryFieldType> _outputTypes = new(StringComparer.OrdinalIgnoreCase);

    private OneCRenderer(QuerySpec spec, QuerySchema schema, DateTime now)
    {
        _spec = spec;
        _schema = schema;
        _now = now;
    }

    /// <summary>
    /// What 1C can do. It has no percentile, no row offset and no cross join, so a query asking for
    /// one of those is refused rather than approximated.
    /// </summary>
    public static IQueryCapabilities Capabilities { get; } = QueryCapabilities.All.Without(
        QueryFeature.Percentile,
        QueryFeature.Offset,
        QueryFeature.CrossJoin,
        // A value function maps onto whatever 1C spells the same job, given a physical name. Its
        // table-valued equivalent - a virtual table - has a wholly different syntax, so it is not
        // something this renderer can honestly produce.
        QueryFeature.TableFunctions);

    /// <summary>Renders a query into 1C query text.</summary>
    /// <param name="spec">The query to render.</param>
    /// <param name="schema">The schema it was built against, which supplies types and physical names.</param>
    /// <param name="now">
    /// The moment a relative time window is measured from. Defaults to the current UTC time; pass one
    /// explicitly to render deterministically.
    /// </param>
    /// <exception cref="QueryValidationException">The query is not coherent.</exception>
    /// <exception cref="QueryRenderException">1C cannot express the query.</exception>
    public static OneCRenderResult Render(QuerySpec spec, QuerySchema schema, DateTime? now = null)
    {
        if (spec is null) throw new ArgumentNullException(nameof(spec));
        if (schema is null) throw new ArgumentNullException(nameof(schema));

        return new OneCRenderer(spec, schema, now ?? DateTime.UtcNow).Run();
    }

    private OneCRenderResult Run()
    {
        _spec.Validate(_schema).ThrowIfInvalid();
        CollectParticipants();
        RequireCapabilities();

        var query = new StringBuilder();
        AppendSelect(query);
        AppendFrom(query);
        AppendJoins(query);
        AppendClause(query, " ГДЕ ", _spec.Where);
        AppendGroupBy(query);
        AppendClause(query, " ИМЕЮЩИЕ ", _spec.Having);
        AppendOrderBy(query);

        return new OneCRenderResult(query.ToString(), _parameters);
    }

    private void CollectParticipants()
    {
        Register(_spec.From.Entity, _spec.From.Call, _spec.From.Alias);
        foreach (var join in _spec.Joins) Register(join.Entity, join.Call, join.Alias);
    }

    private void Register(string? entityKey, QueryFunctionCall? call, string alias)
    {
        EnsureIdentifier(alias, "alias");
        if (call is not null)
        {
            _fields[alias] = _schema.FindFunction(call.Function)!.Columns;
            _ordered.Add(new KeyValuePair<string, string>(alias, string.Empty));
            return;
        }
        _fields[alias] = _schema.FindEntity(entityKey!)!.Fields;
        _ordered.Add(new KeyValuePair<string, string>(alias, entityKey!));
    }

    private void RequireCapabilities()
    {
        foreach (var join in _spec.Joins)
        {
            switch (join.Kind)
            {
                case QueryJoinKind.Left: Require(QueryFeature.LeftJoin); break;
                case QueryJoinKind.Right: Require(QueryFeature.RightJoin); break;
                case QueryJoinKind.Full: Require(QueryFeature.FullJoin); break;
                case QueryJoinKind.Cross: Require(QueryFeature.CrossJoin); break;
            }
            if (join.Call is not null) Require(QueryFeature.TableFunctions);
        }
        if (_spec.From.Call is not null) Require(QueryFeature.TableFunctions);

        foreach (var item in _spec.Select)
        {
            if (item.Aggregate == QueryAggregate.Percentile) Require(QueryFeature.Percentile);
        }

        if (_spec.Offset is not null) Require(QueryFeature.Offset);
    }

    private static void Require(QueryFeature feature)
    {
        if (Capabilities.Supports(feature)) return;
        throw new QueryRenderException($"The 1C query language does not support {feature}.", feature);
    }

    private void AppendSelect(StringBuilder query)
    {
        query.Append("ВЫБРАТЬ ");
        if (_spec.Distinct) query.Append("РАЗЛИЧНЫЕ ");
        if (_spec.Limit is not null)
        {
            query.Append("ПЕРВЫЕ ")
                 .Append(_spec.Limit.Value.ToString(CultureInfo.InvariantCulture))
                 .Append(' ');
        }

        if (_spec.Select.Count == 0)
        {
            query.Append('*');
            return;
        }

        var items = new List<string>(_spec.Select.Count);
        foreach (var item in _spec.Select)
        {
            var expression = SelectExpression(item);
            if (string.IsNullOrEmpty(item.Alias))
            {
                items.Add(expression);
                continue;
            }
            EnsureIdentifier(item.Alias!, "output alias");
            _outputExpressions[item.Alias!] = expression;
            _outputTypes[item.Alias!] = OutputType(item);
            items.Add(expression + " КАК " + item.Alias);
        }
        query.Append(string.Join(", ", items));
    }

    private string SelectExpression(QuerySelect item)
    {
        if (item.Aggregate is null)
        {
            var plain = Value(item.Field, item.Call);
            return item.Truncate is null ? plain : Truncate(plain, item.Truncate.Value);
        }

        if (item.Aggregate == QueryAggregate.Count && item.Field is null && item.Call is null) return "КОЛИЧЕСТВО(*)";

        var inner = Value(item.Field, item.Call);
        var function = item.Aggregate.Value switch
        {
            QueryAggregate.Count => "КОЛИЧЕСТВО",
            QueryAggregate.Sum => "СУММА",
            QueryAggregate.Avg => "СРЕДНЕЕ",
            QueryAggregate.Min => "МИНИМУМ",
            QueryAggregate.Max => "МАКСИМУМ",
            _ => "КОЛИЧЕСТВО",
        };
        return $"{function}({(item.Distinct ? "РАЗЛИЧНЫЕ " : string.Empty)}{inner})";
    }

    private QueryFieldType OutputType(QuerySelect item)
    {
        if (item.Aggregate == QueryAggregate.Count) return QueryFieldType.Number;
        if (item.Field is null && item.Call is null) return QueryFieldType.Number;
        return ValueType(item.Field, item.Call);
    }

    private static string Truncate(string expression, QueryDateTruncation truncation)
    {
        var period = truncation switch
        {
            QueryDateTruncation.Minute => "МИНУТА",
            QueryDateTruncation.Hour => "ЧАС",
            QueryDateTruncation.Day => "ДЕНЬ",
            QueryDateTruncation.Week => "НЕДЕЛЯ",
            QueryDateTruncation.Month => "МЕСЯЦ",
            QueryDateTruncation.Quarter => "КВАРТАЛ",
            QueryDateTruncation.Year => "ГОД",
            _ => "ДЕНЬ",
        };
        return $"НАЧАЛОПЕРИОДА({expression}, {period})";
    }

    private void AppendFrom(StringBuilder query)
    {
        query.Append(" ИЗ ")
             .Append(SourceExpression(_spec.From.Entity, _spec.From.Call))
             .Append(" КАК ")
             .Append(_spec.From.Alias);
    }

    private string SourceExpression(string? entityKey, QueryFunctionCall? call)
        => call is not null ? CallExpression(call) : QualifiedName(_schema.FindEntity(entityKey!)!.PhysicalName);

    private void AppendJoins(StringBuilder query)
    {
        for (var i = 0; i < _spec.Joins.Count; i++)
        {
            var join = _spec.Joins[i];
            var keyword = join.Kind switch
            {
                QueryJoinKind.Left => "ЛЕВОЕ СОЕДИНЕНИЕ",
                QueryJoinKind.Right => "ПРАВОЕ СОЕДИНЕНИЕ",
                QueryJoinKind.Full => "ПОЛНОЕ СОЕДИНЕНИЕ",
                _ => "ВНУТРЕННЕЕ СОЕДИНЕНИЕ",
            };

            query.Append(' ').Append(keyword).Append(' ')
                 .Append(SourceExpression(join.Entity, join.Call))
                 .Append(" КАК ")
                 .Append(join.Alias)
                 .Append(" ПО ")
                 .Append(JoinCondition(join, i));
        }
    }

    private string JoinCondition(QueryJoin join, int index)
    {
        if (join.On is { Count: > 0 })
        {
            return string.Join(" И ", join.On.Select(pair =>
                $"{FieldExpression(pair.Left)} = {FieldExpression(pair.Right)}"));
        }

        var relation = _schema.FindRelation(join.Relation!)!;
        var newIsTarget = string.Equals(relation.To, join.Entity, StringComparison.OrdinalIgnoreCase);
        var otherEntity = newIsTarget ? relation.From : relation.To;
        var otherAlias = join.From ?? ResolveOtherAlias(otherEntity, index, join.Alias);

        return string.Join(" И ", relation.On.Select(pair =>
        {
            var source = newIsTarget ? Member(otherAlias, pair.FromField) : Member(join.Alias, pair.FromField);
            var target = newIsTarget ? Member(join.Alias, pair.ToField) : Member(otherAlias, pair.ToField);
            return $"{source} = {target}";
        }));
    }

    private string ResolveOtherAlias(string entityKey, int joinIndex, string joinAlias)
    {
        var candidates = _ordered
            .Take(joinIndex + 1)
            .Where(participant => !string.Equals(participant.Key, joinAlias, StringComparison.OrdinalIgnoreCase)
                && string.Equals(participant.Value, entityKey, StringComparison.OrdinalIgnoreCase))
            .Select(participant => participant.Key)
            .ToList();

        if (candidates.Count == 1) return candidates[0];
        if (candidates.Count == 0)
        {
            throw new QueryRenderException(
                $"The join onto '{joinAlias}' traverses a relation reaching '{entityKey}', which no earlier participant uses.");
        }
        throw new QueryRenderException(
            $"'{entityKey}' appears more than once ({string.Join(", ", candidates)}), so the join onto " +
            $"'{joinAlias}' is ambiguous. Set QueryJoin.From to the alias it attaches to.");
    }

    private void AppendClause(StringBuilder query, string keyword, QueryFilterGroup? group)
    {
        if (group is null || group.IsEmpty) return;
        query.Append(keyword).Append(RenderGroup(group));
    }

    private string RenderGroup(QueryFilterGroup group)
    {
        var parts = new List<string>(group.Conditions.Count + group.Groups.Count);
        foreach (var condition in group.Conditions) parts.Add(RenderCondition(condition));
        foreach (var nested in group.Groups)
        {
            if (nested.IsEmpty) continue;
            parts.Add("(" + RenderGroup(nested) + ")");
        }
        return string.Join(group.Or ? " ИЛИ " : " И ", parts);
    }

    private string RenderCondition(QueryCondition condition)
    {
        string left;
        QueryFieldType type;
        if (!string.IsNullOrEmpty(condition.Select))
        {
            left = _outputExpressions[condition.Select!];
            type = _outputTypes.TryGetValue(condition.Select!, out var known) ? known : QueryFieldType.Number;
        }
        else
        {
            left = Value(condition.Field, condition.Call);
            type = ValueType(condition.Field, condition.Call);
        }

        switch (condition.Operator)
        {
            case QueryOperator.IsNull:
                return left + " ЕСТЬ NULL";
            case QueryOperator.IsNotNull:
                return left + " ЕСТЬ НЕ NULL";
            case QueryOperator.Contains:
                return Like(left, condition.Value!, "%{0}%");
            case QueryOperator.StartsWith:
                return Like(left, condition.Value!, "{0}%");
            case QueryOperator.EndsWith:
                return Like(left, condition.Value!, "%{0}");
            case QueryOperator.In:
                return $"{left} В ({ValueList(condition.Value!, type)})";
            case QueryOperator.NotIn:
                return $"{left} НЕ В ({ValueList(condition.Value!, type)})";
            case QueryOperator.Between:
                return $"{left} МЕЖДУ {Operand(condition.Value!, type)} И {Operand(condition.Value2!, type)}";
            case QueryOperator.NotBetween:
                return $"НЕ ({left} МЕЖДУ {Operand(condition.Value!, type)} И {Operand(condition.Value2!, type)})";
            default:
                return $"{left} {Symbol(condition.Operator)} {Operand(condition.Value!, type)}";
        }
    }

    private string Like(string left, QueryOperand operand, string shape)
    {
        if (operand.Kind != QueryOperandKind.Literal)
        {
            return $"{left} ПОДОБНО {Operand(operand, QueryFieldType.Text)}";
        }
        var pattern = string.Format(shape, EscapePattern(operand.Value ?? string.Empty));
        return $"{left} ПОДОБНО {AddParameter(pattern)} СПЕЦСИМВОЛ \"\\\"";
    }

    private static string EscapePattern(string value)
    {
        var builder = new StringBuilder(value.Length);
        foreach (var character in value)
        {
            if (character is '\\' or '%' or '_' or '[') builder.Append('\\');
            builder.Append(character);
        }
        return builder.ToString();
    }

    private string ValueList(QueryOperand operand, QueryFieldType type)
        => string.Join(", ", (operand.Values ?? []).Select(value => AddParameter(QueryValue.Parse(value, type))));

    private string Operand(QueryOperand operand, QueryFieldType type) => operand.Kind switch
    {
        QueryOperandKind.Field => FieldExpression(operand.Field!),
        // 1C queries have no current-moment function, so the window is resolved here and travels as a
        // value. The spec keeps the offset, so the next render resolves it again.
        QueryOperandKind.Relative => AddParameter(Shift(_now, operand.Relative!)),
        QueryOperandKind.List => ValueList(operand, type),
        QueryOperandKind.Function => CallExpression(operand.Call!),
        _ => AddParameter(QueryValue.Parse(operand.Value, type)),
    };

    // A value function maps onto whatever the target spells the same job, which the schema supplies
    // as the physical name - so a call renders here exactly as it would in SQL, just unquoted.
    private string CallExpression(QueryFunctionCall call)
    {
        var function = _schema.FindFunction(call.Function)!;
        var arguments = new List<string>(call.Arguments.Count);
        for (var i = 0; i < call.Arguments.Count; i++)
        {
            var type = i < function.Parameters.Count ? function.Parameters[i].Type : QueryFieldType.Text;
            arguments.Add(Operand(call.Arguments[i], type));
        }
        return QualifiedName(function.PhysicalName) + "(" + string.Join(", ", arguments) + ")";
    }

    private static DateTime Shift(DateTime now, QueryRelativeValue relative) => relative.Unit switch
    {
        QueryTimeUnit.Minute => now.AddMinutes(relative.Amount),
        QueryTimeUnit.Hour => now.AddHours(relative.Amount),
        QueryTimeUnit.Day => now.AddDays(relative.Amount),
        QueryTimeUnit.Week => now.AddDays(relative.Amount * 7),
        QueryTimeUnit.Month => now.AddMonths(relative.Amount),
        QueryTimeUnit.Quarter => now.AddMonths(relative.Amount * 3),
        QueryTimeUnit.Year => now.AddYears(relative.Amount),
        _ => now.AddDays(relative.Amount),
    };

    private static string Symbol(QueryOperator op) => op switch
    {
        QueryOperator.Equals => "=",
        QueryOperator.NotEquals => "<>",
        QueryOperator.GreaterThan => ">",
        QueryOperator.GreaterThanOrEqual => ">=",
        QueryOperator.LessThan => "<",
        QueryOperator.LessThanOrEqual => "<=",
        _ => "=",
    };

    private void AppendGroupBy(StringBuilder query)
    {
        if (_spec.GroupBy.Count == 0) return;

        var items = new List<string>(_spec.GroupBy.Count);
        foreach (var group in _spec.GroupBy)
        {
            var expression = Value(group.Field, group.Call);
            if (group.Truncate is not null) expression = Truncate(expression, group.Truncate.Value);
            if (!string.IsNullOrEmpty(group.Alias) && !_outputExpressions.ContainsKey(group.Alias!))
            {
                _outputExpressions[group.Alias!] = expression;
            }
            items.Add(expression);
        }
        query.Append(" СГРУППИРОВАТЬ ПО ").Append(string.Join(", ", items));
    }

    private void AppendOrderBy(StringBuilder query)
    {
        if (_spec.OrderBy.Count == 0) return;

        var items = new List<string>(_spec.OrderBy.Count);
        foreach (var sort in _spec.OrderBy)
        {
            var expression = sort.Field is not null || sort.Call is not null
                ? Value(sort.Field, sort.Call)
                : sort.Select!;
            items.Add(expression + (sort.Direction == QuerySortDirection.Descending ? " УБЫВ" : " ВОЗР"));
        }
        query.Append(" УПОРЯДОЧИТЬ ПО ").Append(string.Join(", ", items));
    }

    private string QualifiedName(string name)
    {
        foreach (var part in name.Split('.')) EnsureIdentifier(part, "object name");
        return name;
    }

    private string Value(QueryFieldRef? field, QueryFunctionCall? call)
        => field is not null ? FieldExpression(field) : CallExpression(call!);

    private QueryFieldType ValueType(QueryFieldRef? field, QueryFunctionCall? call)
        => field is not null
            ? FieldType(field)
            : _schema.FindFunction(call!.Function)?.ReturnType ?? QueryFieldType.Text;

    private string FieldExpression(QueryFieldRef reference) => Member(reference.Alias, reference.Field);

    private string Member(string alias, string fieldKey)
    {
        var field = Find(alias, fieldKey)!;
        EnsureIdentifier(field.PhysicalName, "field name");
        return alias + "." + field.PhysicalName;
    }

    private QueryFieldType FieldType(QueryFieldRef reference)
        => Find(reference.Alias, reference.Field)!.Type;

    private QueryField? Find(string alias, string fieldKey)
    {
        if (!_fields.TryGetValue(alias, out var fields)) return null;
        for (var i = 0; i < fields.Count; i++)
        {
            if (string.Equals(fields[i].Key, fieldKey, StringComparison.OrdinalIgnoreCase)) return fields[i];
        }
        return null;
    }

    // 1C offers no way to quote a name, so an unusable name has to be rejected rather than escaped.
    // This is also what keeps a user-chosen alias from being able to alter the query.
    private static void EnsureIdentifier(string identifier, string what)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new QueryRenderException($"An empty {what} cannot be written into a 1C query.");
        }
        for (var i = 0; i < identifier.Length; i++)
        {
            var character = identifier[i];
            var allowed = char.IsLetter(character) || character == '_' || (i > 0 && char.IsDigit(character));
            if (!allowed)
            {
                throw new QueryRenderException(
                    $"The {what} '{identifier}' is not a valid 1C identifier. 1C cannot quote a name, so " +
                    "only letters, digits and underscores are usable.");
            }
        }
    }

    private string AddParameter(object? value)
    {
        var name = "p" + _parameters.Count.ToString(CultureInfo.InvariantCulture);
        _parameters.Add(new OneCQueryParameter(name, value));
        return "&" + name;
    }
}
