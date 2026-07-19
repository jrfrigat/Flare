using System.Collections.Generic;
using System.Linq;

namespace Querio;

/// <summary>
/// Checks a query against the schema it was built for. The checks are entirely dialect-free - they
/// ask whether the query is coherent, never whether some particular store could run it - which is
/// why they belong here rather than in a renderer, and why a visual designer can reuse them to mark
/// the offending row while a query is still being edited.
/// </summary>
public static class QueryValidator
{
    /// <summary>Validates a query against a schema and returns every problem found.</summary>
    /// <param name="spec">The query to check.</param>
    /// <param name="schema">The schema the query is built against.</param>
    public static QueryValidationResult Validate(this QuerySpec spec, QuerySchema schema)
    {
        if (spec is null) throw new ArgumentNullException(nameof(spec));
        if (schema is null) throw new ArgumentNullException(nameof(schema));

        var context = new Context(spec, schema);
        context.Run();
        return new QueryValidationResult(context.Errors);
    }

    private sealed class Context(QuerySpec spec, QuerySchema schema)
    {
        // Null value means the entity itself was unknown; the problem is reported once, at the
        // participant, and field lookups against it then stay quiet instead of cascading.
        private readonly Dictionary<string, QueryEntity?> _participants = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _outputAliases = new(StringComparer.OrdinalIgnoreCase);

        public List<QueryError> Errors { get; } = [];

        public void Run()
        {
            CollectParticipants();
            ValidateJoins();
            CollectOutputAliases();
            ValidateSelect();
            ValidateGroupBy();
            ValidateFilter(spec.Where, "Where", allowSelectTarget: false);
            ValidateFilter(spec.Having, "Having", allowSelectTarget: true);
            ValidateOrderBy();
            ValidatePaging();
        }

        private void CollectParticipants()
        {
            Register(spec.From.Entity, spec.From.Alias, "From");
            for (var i = 0; i < spec.Joins.Count; i++)
            {
                Register(spec.Joins[i].Entity, spec.Joins[i].Alias, $"Joins[{i}]");
            }
        }

        private void Register(string entity, string alias, string path)
        {
            QueryEntity? resolved = null;
            if (string.IsNullOrEmpty(entity))
            {
                Add(QueryErrorCode.UnknownEntity, "The participant names no entity.", path);
            }
            else
            {
                resolved = schema.FindEntity(entity);
                if (resolved is null) Add(QueryErrorCode.UnknownEntity, $"The schema has no entity '{entity}'.", path);
            }

            if (string.IsNullOrEmpty(alias))
            {
                Add(QueryErrorCode.MissingAlias, "The participant has no alias, so its fields cannot be referenced.", path);
                return;
            }
            if (_participants.ContainsKey(alias))
            {
                Add(QueryErrorCode.DuplicateAlias, $"The alias '{alias}' is already used by another participant.", path);
                return;
            }
            _participants[alias] = resolved;
        }

        private void ValidateJoins()
        {
            // Grown one join at a time on purpose: a join may only attach to a participant declared
            // before it. Checking against every participant at once would let a self-relation vouch
            // for itself, so "from requests join users on user_manager" would pass while reaching
            // nothing the query already had.
            var reachable = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(spec.From.Entity)) reachable.Add(spec.From.Entity);

            var attached = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(spec.From.Alias)) attached.Add(spec.From.Alias);

            for (var i = 0; i < spec.Joins.Count; i++)
            {
                var join = spec.Joins[i];
                var path = $"Joins[{i}]";

                if (!string.IsNullOrEmpty(join.From) && !attached.Contains(join.From!))
                {
                    Add(QueryErrorCode.UnknownAlias,
                        $"The join attaches to '{join.From}', which is not a participant declared before it.",
                        path);
                }

                // A cross join pairs everything with everything, so it deliberately has no condition.
                if (join.Kind != QueryJoinKind.Cross)
                {
                    if (!string.IsNullOrEmpty(join.Relation))
                    {
                        var relation = schema.FindRelation(join.Relation!);
                        if (relation is null)
                        {
                            Add(QueryErrorCode.UnknownRelation, $"The schema has no relation '{join.Relation}'.", path);
                        }
                        else if (!Connects(relation, join.Entity, reachable))
                        {
                            Add(QueryErrorCode.RelationNotConnected,
                                $"The relation '{relation.Key}' does not connect '{join.Entity}' to anything already in the query.",
                                path);
                        }
                        else
                        {
                            ValidateRelationFields(relation, path);
                        }
                    }
                    else if (join.On is { Count: > 0 })
                    {
                        for (var j = 0; j < join.On.Count; j++)
                        {
                            ResolveField(join.On[j].Left, $"{path}.On[{j}].Left");
                            ResolveField(join.On[j].Right, $"{path}.On[{j}].Right");
                        }
                    }
                    else
                    {
                        Add(QueryErrorCode.MissingJoinCondition,
                            "The join names neither a relation nor explicit match conditions.", path);
                    }
                }

                if (!string.IsNullOrEmpty(join.Entity)) reachable.Add(join.Entity);
                if (!string.IsNullOrEmpty(join.Alias)) attached.Add(join.Alias);
            }
        }

        // One end of the relation has to be the entity being joined, and the other has to be an
        // entity the query already reached. A self-relation satisfies both with the same entity,
        // which is exactly what a self-join needs - provided that entity was already there.
        private static bool Connects(QueryRelation relation, string entity, HashSet<string> reachable)
        {
            var fromMatches = string.Equals(relation.From, entity, StringComparison.OrdinalIgnoreCase);
            var toMatches = string.Equals(relation.To, entity, StringComparison.OrdinalIgnoreCase);
            if (!fromMatches && !toMatches) return false;
            var other = fromMatches ? relation.To : relation.From;
            return reachable.Contains(other);
        }

        private void ValidateRelationFields(QueryRelation relation, string path)
        {
            var from = schema.FindEntity(relation.From);
            var to = schema.FindEntity(relation.To);
            for (var i = 0; i < relation.On.Count; i++)
            {
                var pair = relation.On[i];
                if (from is not null && from.FindField(pair.FromField) is null)
                {
                    Add(QueryErrorCode.UnknownField,
                        $"The relation '{relation.Key}' matches on '{pair.FromField}', which '{relation.From}' does not declare.",
                        path);
                }
                if (to is not null && to.FindField(pair.ToField) is null)
                {
                    Add(QueryErrorCode.UnknownField,
                        $"The relation '{relation.Key}' matches on '{pair.ToField}', which '{relation.To}' does not declare.",
                        path);
                }
            }
        }

        private void CollectOutputAliases()
        {
            foreach (var select in spec.Select)
            {
                if (!string.IsNullOrEmpty(select.Alias)) _outputAliases.Add(select.Alias!);
            }
            foreach (var group in spec.GroupBy)
            {
                if (!string.IsNullOrEmpty(group.Alias)) _outputAliases.Add(group.Alias!);
            }
        }

        private void ValidateSelect()
        {
            for (var i = 0; i < spec.Select.Count; i++)
            {
                var select = spec.Select[i];
                var path = $"Select[{i}]";
                var field = select.Field is null ? null : ResolveField(select.Field, path);

                if (select.Aggregate is null)
                {
                    if (select.Field is null)
                    {
                        Add(QueryErrorCode.EmptySelectItem,
                            "The selected item names neither a field nor an aggregate.", path);
                        continue;
                    }
                    if (select.Truncate is not null && field is not null && field.Type != QueryFieldType.DateTime)
                    {
                        Add(QueryErrorCode.TruncationNotApplicable,
                            $"'{select.Field}' does not hold a timestamp, so it cannot be truncated to a period.",
                            path);
                    }
                    // Once the query groups rows, a plain field only has one value per group if it is
                    // one of the grouping keys - truncation included, since a raw timestamp and the
                    // day it falls in are different keys.
                    if (spec.GroupBy.Count > 0 && !IsGrouped(select.Field, select.Truncate))
                    {
                        Add(QueryErrorCode.MissingGroupBy,
                            $"'{select.Field}' is returned as-is but the query groups rows, so it must also be grouped by, with the same truncation.",
                            path);
                    }
                    continue;
                }

                var aggregate = select.Aggregate.Value;
                if (select.Field is null && aggregate != QueryAggregate.Count)
                {
                    Add(QueryErrorCode.AggregateWithoutField,
                        $"The {aggregate} aggregate needs a field; only a row count may omit one.", path);
                }
                if (field is not null && !field.AllowedAggregates.Contains(aggregate))
                {
                    Add(QueryErrorCode.AggregateNotAllowed,
                        $"The schema does not permit the {aggregate} aggregate on '{select.Field}'.", path);
                }
                if (aggregate == QueryAggregate.Percentile)
                {
                    if (select.Percentile is null)
                    {
                        Add(QueryErrorCode.MissingPercentileRank,
                            "A percentile was requested without saying which one.", path);
                    }
                    else if (select.Percentile.Value < 0 || select.Percentile.Value > 1)
                    {
                        Add(QueryErrorCode.InvalidPercentileRank,
                            $"The percentile rank {select.Percentile.Value} falls outside the range 0 to 1.", path);
                    }
                }
            }
        }

        private bool IsGrouped(QueryFieldRef reference, QueryDateTruncation? truncate)
        {
            foreach (var group in spec.GroupBy)
            {
                if (string.Equals(group.Field.Alias, reference.Alias, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(group.Field.Field, reference.Field, StringComparison.OrdinalIgnoreCase)
                    && group.Truncate == truncate)
                {
                    return true;
                }
            }
            return false;
        }

        private void ValidateGroupBy()
        {
            for (var i = 0; i < spec.GroupBy.Count; i++)
            {
                var group = spec.GroupBy[i];
                var path = $"GroupBy[{i}]";
                var field = ResolveField(group.Field, path);
                if (field is null) continue;

                if (!field.Groupable)
                {
                    Add(QueryErrorCode.FieldNotGroupable,
                        $"The schema marks '{group.Field}' as not groupable.", path);
                }
                if (group.Truncate is not null && field.Type != QueryFieldType.DateTime)
                {
                    Add(QueryErrorCode.TruncationNotApplicable,
                        $"'{group.Field}' does not hold a timestamp, so it cannot be truncated to a period.", path);
                }
            }
        }

        private void ValidateFilter(QueryFilterGroup? group, string path, bool allowSelectTarget)
        {
            if (group is null) return;
            for (var i = 0; i < group.Conditions.Count; i++)
            {
                ValidateCondition(group.Conditions[i], $"{path}.Conditions[{i}]", allowSelectTarget);
            }
            for (var i = 0; i < group.Groups.Count; i++)
            {
                ValidateFilter(group.Groups[i], $"{path}.Groups[{i}]", allowSelectTarget);
            }
        }

        private void ValidateCondition(QueryCondition condition, string path, bool allowSelectTarget)
        {
            var hasField = condition.Field is not null;
            var hasSelect = !string.IsNullOrEmpty(condition.Select);

            if (hasField && hasSelect)
            {
                Add(QueryErrorCode.AmbiguousConditionTarget,
                    "The condition names both a field and a select output alias.", path);
                return;
            }
            if (!hasField && !hasSelect)
            {
                Add(QueryErrorCode.MissingConditionTarget,
                    "The condition names neither a field nor a select output alias.", path);
                return;
            }

            QueryField? field = null;
            if (hasSelect)
            {
                if (!allowSelectTarget)
                {
                    Add(QueryErrorCode.SelectConditionOutsideHaving,
                        $"'{condition.Select}' is a computed aggregate, which does not exist yet where this clause is applied. Move the condition to Having.",
                        path);
                }
                else if (!_outputAliases.Contains(condition.Select!))
                {
                    Add(QueryErrorCode.UnknownSelectAlias,
                        $"Nothing in the query is selected as '{condition.Select}'.", path);
                }
            }
            else
            {
                field = ResolveField(condition.Field!, path);
                if (field is not null)
                {
                    if (!field.Filterable)
                    {
                        Add(QueryErrorCode.FieldNotFilterable,
                            $"The schema marks '{condition.Field}' as not filterable.", path);
                    }
                    else if (!field.AllowedOperators.Contains(condition.Operator))
                    {
                        Add(QueryErrorCode.OperatorNotAllowed,
                            $"The schema does not permit the {condition.Operator} operator on '{condition.Field}'.", path);
                    }
                }
            }

            ValidateOperandCount(condition, path);
            ValidateOperand(condition.Value, $"{path}.Value", field);
            ValidateOperand(condition.Value2, $"{path}.Value2", field);
        }

        private void ValidateOperandCount(QueryCondition condition, string path)
        {
            if (QueryDefaults.TakesNoValue(condition.Operator))
            {
                if (condition.Value is not null || condition.Value2 is not null)
                {
                    Add(QueryErrorCode.UnexpectedOperand,
                        $"The {condition.Operator} operator takes no operand.", path);
                }
                return;
            }
            if (QueryDefaults.TakesTwoValues(condition.Operator))
            {
                if (condition.Value is null || condition.Value2 is null)
                {
                    Add(QueryErrorCode.MissingOperand,
                        $"The {condition.Operator} operator needs both a lower and an upper bound.", path);
                }
                return;
            }
            if (QueryDefaults.TakesValueList(condition.Operator))
            {
                if (condition.Value is null || condition.Value.Kind != QueryOperandKind.List)
                {
                    Add(QueryErrorCode.MissingOperand,
                        $"The {condition.Operator} operator needs a set of values.", path);
                }
                return;
            }
            if (condition.Value is null)
            {
                Add(QueryErrorCode.MissingOperand,
                    $"The {condition.Operator} operator needs a value to compare against.", path);
            }
        }

        private void ValidateOperand(QueryOperand? operand, string path, QueryField? field)
        {
            if (operand is null) return;
            switch (operand.Kind)
            {
                case QueryOperandKind.Field:
                    if (operand.Field is null)
                    {
                        Add(QueryErrorCode.MissingOperand, "The operand names no field.", path);
                    }
                    else
                    {
                        ResolveField(operand.Field, path);
                    }
                    break;

                case QueryOperandKind.List:
                    if (operand.Values is null || operand.Values.Count == 0)
                    {
                        Add(QueryErrorCode.MissingOperand, "The operand holds an empty set of values.", path);
                    }
                    break;

                case QueryOperandKind.Relative:
                    if (operand.Relative is null)
                    {
                        Add(QueryErrorCode.MissingOperand, "The operand holds no time offset.", path);
                    }
                    else if (field is not null && field.Type != QueryFieldType.DateTime)
                    {
                        Add(QueryErrorCode.RelativeValueNotApplicable,
                            "A relative time offset only compares against a field that holds a timestamp.", path);
                    }
                    break;

                case QueryOperandKind.Literal:
                default:
                    // A literal needs no further checking: a null value is a legitimate comparison.
                    break;
            }
        }

        private void ValidateOrderBy()
        {
            for (var i = 0; i < spec.OrderBy.Count; i++)
            {
                var sort = spec.OrderBy[i];
                var path = $"OrderBy[{i}]";
                var hasField = sort.Field is not null;
                var hasSelect = !string.IsNullOrEmpty(sort.Select);

                if (hasField && hasSelect)
                {
                    Add(QueryErrorCode.AmbiguousConditionTarget,
                        "The ordering names both a field and a select output alias.", path);
                    continue;
                }
                if (!hasField && !hasSelect)
                {
                    Add(QueryErrorCode.MissingConditionTarget,
                        "The ordering names neither a field nor a select output alias.", path);
                    continue;
                }

                if (hasSelect)
                {
                    if (!_outputAliases.Contains(sort.Select!))
                    {
                        Add(QueryErrorCode.UnknownSelectAlias,
                            $"Nothing in the query is selected as '{sort.Select}'.", path);
                    }
                }
                else
                {
                    ResolveField(sort.Field!, path);
                }
            }
        }

        private void ValidatePaging()
        {
            if (spec.Limit is < 0)
            {
                Add(QueryErrorCode.InvalidLimit, $"The row limit {spec.Limit} is negative.", "Limit");
            }
            if (spec.Offset is < 0)
            {
                Add(QueryErrorCode.InvalidOffset, $"The row offset {spec.Offset} is negative.", "Offset");
            }
        }

        private QueryField? ResolveField(QueryFieldRef reference, string path)
        {
            if (string.IsNullOrEmpty(reference.Alias)
                || !_participants.TryGetValue(reference.Alias, out var entity))
            {
                Add(QueryErrorCode.UnknownAlias,
                    $"'{reference.Alias}' is not the alias of any participant in this query.", path);
                return null;
            }

            // The entity was already reported as unknown; saying so again per field would only bury it.
            if (entity is null) return null;

            var field = entity.FindField(reference.Field);
            if (field is null)
            {
                Add(QueryErrorCode.UnknownField,
                    $"The entity '{entity.Key}' declares no field '{reference.Field}'.", path);
            }
            return field;
        }

        private void Add(QueryErrorCode code, string message, string path)
            => Errors.Add(new QueryError(code, message, path));
    }
}
