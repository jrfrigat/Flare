using Microsoft.AspNetCore.Components;

namespace Flare.Components.IDE;

/// <summary>A named command group in a toolbar.</summary>
/// <param name="Key">Stable key, unique within the toolbar.</param>
/// <param name="Label">Accessible name of the group.</param>
/// <param name="Content">Buttons and other command controls in the group.</param>
public sealed record IdeToolbarGroup(string Key, string Label, RenderFragment Content);
