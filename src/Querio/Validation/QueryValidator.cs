using System.Collections.Generic;
using System.Linq;

namespace Querio;

/// <summary>
/// Checks a query against the schema it was built for. The checks are entirely dialect-free - they
/// ask whether the query is coherent, never whether some particular store could run it - which is
/// why they belong here rather than in a renderer, and why a visual designer can reuse them to mark
/// the offending row while a query is still being edited.
/// </summary>
public static partial class QueryValidator
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

    /// <summary>
    /// One thing a query can draw fields from. An entity and a table function are interchangeable
    /// here: both contribute an alias and a set of fields, and everything downstream only cares
    /// about those two.
    /// </summary>
    private sealed record Participant(string Label, IReadOnlyList<QueryField> Fields, string? EntityKey)
    {
        public QueryField? FindField(string key)
        {
            for (var i = 0; i < Fields.Count; i++)
            {
                if (string.Equals(Fields[i].Key, key, StringComparison.OrdinalIgnoreCase)) return Fields[i];
            }
            return null;
        }
    }

    private sealed partial class Context(QuerySpec spec, QuerySchema schema)
    {
        // Null value means the participant itself could not be resolved; the problem is reported
        // once, where it occurs, and lookups against it then stay quiet instead of cascading.
        private readonly Dictionary<string, Participant?> _participants = new(StringComparer.OrdinalIgnoreCase);
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
            Register(ResolveParticipant(spec.From.Entity, spec.From.Call, "From"), spec.From.Alias, "From");
            for (var i = 0; i < spec.Joins.Count; i++)
            {
                var join = spec.Joins[i];
                var path = $"Joins[{i}]";
                Register(ResolveParticipant(join.Entity, join.Call, path), join.Alias, path);
            }
        }

        // A participant is either an entity or a table function call. Both end up as a label and a
        // set of fields, so the rest of validation need not care which it was.
        private Participant? ResolveParticipant(string? entityKey, QueryFunctionCall? call, string path)
        {
            if (call is not null)
            {
                if (!string.IsNullOrEmpty(entityKey))
                {
                    Add(QueryErrorCode.AmbiguousValueSource,
                        "The participant names both an entity and a table function.", path);
                    return null;
                }
                var function = ValidateCall(call, QueryFunctionKind.Table, path);
                return function is null ? null : new Participant(function.Key, function.Columns, null);
            }

            if (string.IsNullOrEmpty(entityKey))
            {
                Add(QueryErrorCode.MissingValueSource,
                    "The participant names neither an entity nor a table function.", path);
                return null;
            }

            var entity = schema.FindEntity(entityKey!);
            if (entity is null)
            {
                Add(QueryErrorCode.UnknownEntity, $"The schema has no entity '{entityKey}'.", path);
                return null;
            }
            return new Participant(entity.Key, entity.Fields, entity.Key);
        }

        private void Register(Participant? participant, string alias, string path)
        {
            if (string.IsNullOrEmpty(alias))
            {
                Add(QueryErrorCode.MissingAlias,
                    "The participant has no alias, so its fields cannot be referenced.", path);
                return;
            }
            if (_participants.ContainsKey(alias))
            {
                Add(QueryErrorCode.DuplicateAlias,
                    $"The alias '{alias}' is already used by another participant.", path);
                return;
            }
            _participants[alias] = participant;
        }

        private void ValidateJoins()
        {
            // Grown one join at a time on purpose: a join may only attach to a participant declared
            // before it. Checking against every participant at once would let a self-relation vouch
            // for itself, so "from requests join users on user_manager" would pass while reaching
            // nothing the query already had.
            var reachable = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(spec.From.Entity)) reachable.Add(spec.From.Entity!);

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
                if (join.Kind != QueryJoinKind.Cross) ValidateJoinMatch(join, path, reachable);

                if (!string.IsNullOrEmpty(join.Entity)) reachable.Add(join.Entity!);
                if (!string.IsNullOrEmpty(join.Alias)) attached.Add(join.Alias);
            }
        }

        private void ValidateJoinMatch(QueryJoin join, string path, HashSet<string> reachable)
        {
            if (join.On is { Count: > 0 })
            {
                for (var j = 0; j < join.On.Count; j++)
                {
                    ResolveField(join.On[j].Left, $"{path}.On[{j}].Left");
                    ResolveField(join.On[j].Right, $"{path}.On[{j}].Right");
                }
                return;
            }

            if (!string.IsNullOrEmpty(join.Relation))
            {
                // A table function has no declared relations, so there is nothing to traverse to it.
                if (join.Call is not null)
                {
                    Add(QueryErrorCode.MissingJoinCondition,
                        "A table function has no declared relations, so this join needs explicit match conditions.",
                        path);
                    return;
                }

                var relation = schema.FindRelation(join.Relation!);
                if (relation is null)
                {
                    Add(QueryErrorCode.UnknownRelation, $"The schema has no relation '{join.Relation}'.", path);
                    return;
                }
                if (!Connects(relation, join.Entity, reachable))
                {
                    Add(QueryErrorCode.RelationNotConnected,
                        $"The relation '{relation.Key}' does not connect '{join.Entity}' to anything already in the query.",
                        path);
                    return;
                }
                ValidateRelationFields(relation, path);
                return;
            }

            Add(QueryErrorCode.MissingJoinCondition,
                "The join names neither a relation nor explicit match conditions.", path);
        }

        // One end of the relation has to be the entity being joined, and the other has to be an
        // entity the query already reached. A self-relation satisfies both with the same entity,
        // which is exactly what a self-join needs - provided that entity was already there.
        private static bool Connects(QueryRelation relation, string? entity, HashSet<string> reachable)
        {
            if (string.IsNullOrEmpty(entity)) return false;
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
                var counting = select.Aggregate == QueryAggregate.Count;
                var empty = select.Field is null && select.Call is null;

                if (empty && select.Aggregate is null)
                {
                    Add(QueryErrorCode.EmptySelectItem,
                        "The selected item names neither a field nor an aggregate.", path);
                    continue;
                }
                if (empty && !counting)
                {
                    Add(QueryErrorCode.AggregateWithoutField,
                        $"The {select.Aggregate} aggregate needs a value; only a row count may omit one.", path);
                    continue;
                }

                var type = empty ? null : ValueType(select.Field, select.Call, path);

                if (select.Aggregate is null)
                {
                    if (select.Truncate is not null && type is not null && type != QueryFieldType.DateTime)
                    {
                        Add(QueryErrorCode.TruncationNotApplicable,
                            "The selected value does not hold a timestamp, so it cannot be truncated to a period.",
                            path);
                    }
                    // Once the query groups rows, a plain value only has one result per group if it
                    // is one of the grouping keys - truncation included, since a raw timestamp and
                    // the day it falls in are different keys.
                    if (spec.GroupBy.Count > 0 && !IsGrouped(select.Field, select.Call, select.Truncate))
                    {
                        Add(QueryErrorCode.MissingGroupBy,
                            "The item is returned as-is but the query groups rows, so it must also be grouped by, with the same truncation.",
                            path);
                    }
                    continue;
                }

                var aggregate = select.Aggregate.Value;
                if (type is not null && !AllowedAggregates(select.Field, type.Value).Contains(aggregate))
                {
                    Add(QueryErrorCode.AggregateNotAllowed,
                        $"The schema does not permit the {aggregate} aggregate here.", path);
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

        // A field's permitted aggregates may be narrowed by the schema; a function call falls back to
        // whatever its return type ordinarily allows.
        private IReadOnlyList<QueryAggregate> AllowedAggregates(QueryFieldRef? field, QueryFieldType type)
        {
            if (field is null) return QueryDefaults.AggregatesFor(type);
            var resolved = FindField(field);
            return resolved?.AllowedAggregates ?? QueryDefaults.AggregatesFor(type);
        }

        private bool IsGrouped(QueryFieldRef? field, QueryFunctionCall? call, QueryDateTruncation? truncate)
        {
            foreach (var group in spec.GroupBy)
            {
                if (group.Truncate != truncate) continue;
                if (field is not null && group.Field is not null
                    && string.Equals(group.Field.Alias, field.Alias, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(group.Field.Field, field.Field, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                if (call is not null && group.Call is not null && SameCall(group.Call, call)) return true;
            }
            return false;
        }

        private void ValidateGroupBy()
        {
            for (var i = 0; i < spec.GroupBy.Count; i++)
            {
                var group = spec.GroupBy[i];
                var path = $"GroupBy[{i}]";
                if (group.Field is null && group.Call is null)
                {
                    Add(QueryErrorCode.MissingValueSource,
                        "The grouping names neither a field nor a function call.", path);
                    continue;
                }

                var type = ValueType(group.Field, group.Call, path);
                if (group.Field is not null && FindField(group.Field) is { Groupable: false })
                {
                    Add(QueryErrorCode.FieldNotGroupable,
                        $"The schema marks '{group.Field}' as not groupable.", path);
                }
                if (group.Truncate is not null && type is not null && type != QueryFieldType.DateTime)
                {
                    Add(QueryErrorCode.TruncationNotApplicable,
                        "The grouping key does not hold a timestamp, so it cannot be truncated to a period.", path);
                }
            }
        }

        private void ValidateOrderBy()
        {
            for (var i = 0; i < spec.OrderBy.Count; i++)
            {
                var sort = spec.OrderBy[i];
                var path = $"OrderBy[{i}]";
                var targets = 0;
                if (sort.Field is not null) targets++;
                if (sort.Call is not null) targets++;
                if (!string.IsNullOrEmpty(sort.Select)) targets++;

                if (targets > 1)
                {
                    Add(QueryErrorCode.AmbiguousConditionTarget,
                        "The ordering names more than one thing to order by.", path);
                    continue;
                }
                if (targets == 0)
                {
                    Add(QueryErrorCode.MissingConditionTarget,
                        "The ordering names neither a field, a function call nor a select output alias.", path);
                    continue;
                }

                if (!string.IsNullOrEmpty(sort.Select))
                {
                    if (!_outputAliases.Contains(sort.Select!))
                    {
                        Add(QueryErrorCode.UnknownSelectAlias,
                            $"Nothing in the query is selected as '{sort.Select}'.", path);
                    }
                    continue;
                }
                ValueType(sort.Field, sort.Call, path);
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

        private QueryField? FindField(QueryFieldRef reference)
            => _participants.TryGetValue(reference.Alias, out var participant)
                ? participant?.FindField(reference.Field)
                : null;

        private QueryField? ResolveField(QueryFieldRef reference, string path)
        {
            if (string.IsNullOrEmpty(reference.Alias)
                || !_participants.TryGetValue(reference.Alias, out var participant))
            {
                Add(QueryErrorCode.UnknownAlias,
                    $"'{reference.Alias}' is not the alias of any participant in this query.", path);
                return null;
            }

            // The participant was already reported as unresolvable; saying so per field would bury it.
            if (participant is null) return null;

            var field = participant.FindField(reference.Field);
            if (field is null)
            {
                Add(QueryErrorCode.UnknownField,
                    $"'{participant.Label}' declares no field '{reference.Field}'.", path);
            }
            return field;
        }

        private void Add(QueryErrorCode code, string message, string path)
            => Errors.Add(new QueryError(code, message, path));
    }
}
