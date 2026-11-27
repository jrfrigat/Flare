namespace Flare.Components.IDE;

/// <summary>One navigation node in a settings document.</summary>
/// <param name="Id">A stable navigation identifier, unique within the document.</param>
/// <param name="Label">The displayed and accessible title, localized by the host.</param>
public sealed record FlareSettingsCategory(string Id, string Label)
{
    /// <summary>Optional text displayed beneath the page title and included in search.</summary>
    public string? Description { get; init; }

    /// <summary>The child categories and pages in display order.</summary>
    public IReadOnlyList<FlareSettingsCategory> Children { get; init; } = [];

    /// <summary>Optional keywords and setting labels that let search find this category.</summary>
    public IReadOnlyList<string> SearchTerms { get; init; } = [];

    /// <summary>Whether selecting the node displays its page. Set to false for a group heading.</summary>
    public bool Selectable { get; init; } = true;
}
