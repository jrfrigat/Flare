namespace Querio;

/// <summary>
/// Points at one field of one participant in a query. The alias, not the entity name, identifies the
/// participant - that is what lets the same entity appear twice (a self-join, or two foreign keys
/// reaching the same target) without the two occurrences becoming ambiguous.
/// </summary>
/// <param name="Alias">Alias of the source or join this field belongs to.</param>
/// <param name="Field">Logical field name within that participant's entity.</param>
public sealed record QueryFieldRef(string Alias, string Field)
{
    /// <summary>Renders the reference as <c>alias.field</c>, for diagnostics and debugging.</summary>
    public override string ToString() => $"{Alias}.{Field}";
}
