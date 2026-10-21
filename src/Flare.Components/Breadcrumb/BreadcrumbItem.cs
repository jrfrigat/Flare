namespace Flare.Components;

/// <summary>A single entry in a <c>FlareBreadcrumb</c> trail.</summary>
/// <param name="Text">Visible label.</param>
/// <param name="Href">Optional navigation target; when null the item renders as plain text (e.g. the current page).</param>
/// <param name="Disabled">Whether the item is non-interactive.</param>
/// <param name="Icon">Optional icon shown before the label - a home glyph on the first crumb, for example.</param>
public sealed record BreadcrumbItem(string Text, string? Href = null, bool Disabled = false, FlareIcon? Icon = null);
