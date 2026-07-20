namespace Querio;

/// <summary>
/// Everything a query is allowed to touch: the entities a user may draw from and the relations that
/// connect them. The consumer owns this description - it is the only place the query model learns
/// anything about the data, which is what keeps the model independent of any store or ORM.
/// <para>
/// This is a class rather than a record on purpose. It caches lookup dictionaries, and a record's
/// synthesized equality compares every field, so those caches would make two structurally identical
/// schemas compare as unequal.
/// </para>
/// </summary>
public sealed class QuerySchema
{
    private readonly Dictionary<string, QueryEntity> _entities;
    private readonly Dictionary<string, QueryRelation> _relations;
    private readonly Dictionary<string, QueryFunction> _functions;

    /// <summary>Builds a schema from its entities, the relations between them, and callable functions.</summary>
    /// <param name="entities">The queryable entities. Keys are matched case-insensitively.</param>
    /// <param name="relations">Declared paths between entities. Null means the entities stand alone.</param>
    /// <param name="functions">Functions queries may call. Null means none are offered.</param>
    public QuerySchema(
        IReadOnlyList<QueryEntity> entities,
        IReadOnlyList<QueryRelation>? relations = null,
        IReadOnlyList<QueryFunction>? functions = null)
    {
        Entities = entities ?? throw new ArgumentNullException(nameof(entities));
        Relations = relations ?? [];
        Functions = functions ?? [];

        // Duplicate keys are a schema mistake, but throwing from a constructor would deny the caller
        // the readable diagnostic QueryValidator produces, so the later declaration simply wins here.
        _entities = new Dictionary<string, QueryEntity>(Entities.Count, StringComparer.OrdinalIgnoreCase);
        foreach (var entity in Entities) _entities[entity.Key] = entity;

        _relations = new Dictionary<string, QueryRelation>(Relations.Count, StringComparer.OrdinalIgnoreCase);
        foreach (var relation in Relations) _relations[relation.Key] = relation;

        _functions = new Dictionary<string, QueryFunction>(Functions.Count, StringComparer.OrdinalIgnoreCase);
        foreach (var function in Functions) _functions[function.Key] = function;
    }

    /// <summary>The queryable entities, in declaration order.</summary>
    public IReadOnlyList<QueryEntity> Entities { get; }

    /// <summary>The declared relations between entities, in declaration order.</summary>
    public IReadOnlyList<QueryRelation> Relations { get; }

    /// <summary>The functions queries may call, in declaration order.</summary>
    public IReadOnlyList<QueryFunction> Functions { get; }

    /// <summary>Finds an entity by key, case-insensitively. Null when the schema has no such entity.</summary>
    /// <param name="key">The logical entity name.</param>
    public QueryEntity? FindEntity(string key)
        => !string.IsNullOrEmpty(key) && _entities.TryGetValue(key, out var entity) ? entity : null;

    /// <summary>Finds a relation by key, case-insensitively. Null when the schema has no such relation.</summary>
    /// <param name="key">The relation name.</param>
    public QueryRelation? FindRelation(string key)
        => !string.IsNullOrEmpty(key) && _relations.TryGetValue(key, out var relation) ? relation : null;

    /// <summary>Finds a function by key, case-insensitively. Null when the schema offers no such function.</summary>
    /// <param name="key">The logical function name.</param>
    public QueryFunction? FindFunction(string key)
        => !string.IsNullOrEmpty(key) && _functions.TryGetValue(key, out var function) ? function : null;

    /// <summary>Finds a field on an entity. Null when either the entity or the field is unknown.</summary>
    /// <param name="entityKey">The logical entity name.</param>
    /// <param name="fieldKey">The logical field name.</param>
    public QueryField? FindField(string entityKey, string fieldKey)
        => FindEntity(entityKey)?.FindField(fieldKey);

    /// <summary>
    /// Every relation touching the entity, from either side. Used to offer the joins reachable from
    /// what a user has already picked.
    /// </summary>
    /// <param name="entityKey">The logical entity name.</param>
    public IEnumerable<QueryRelation> RelationsOf(string entityKey)
    {
        foreach (var relation in Relations)
        {
            if (string.Equals(relation.From, entityKey, StringComparison.OrdinalIgnoreCase)
                || string.Equals(relation.To, entityKey, StringComparison.OrdinalIgnoreCase))
            {
                yield return relation;
            }
        }
    }
}
