using Flare.Abstractions.Tokens;

namespace Flare.Components;

/// <summary>
/// The theme a provider or scope cascades to the Flare components under it. The holder itself is cascaded as a
/// fixed value and the snapshot's version as a plain number beside it: Blazor treats a number as unchanged when it
/// is equal, but treats any other object as changed - so cascading the snapshot directly made every Flare component
/// on the page render again whenever the provider's parent rendered, theme change or not.
/// </summary>
internal sealed class ThemeState
{
    /// <summary>Name of the cascaded version number that tells the components the theme changed.</summary>
    public const string VersionName = "FlareThemeVersion";

    /// <summary>The theme as it is now.</summary>
    public ThemeSnapshot? Snapshot { get; set; }
}
