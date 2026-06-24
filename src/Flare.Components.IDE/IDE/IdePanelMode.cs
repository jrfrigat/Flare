namespace Flare.Components.IDE;

/// <summary>
/// Controls how a docked tool panel in <see cref="FlareIdeLayout"/> behaves when expanded.
/// </summary>
public enum IdePanelMode
{
    /// <summary>The panel occupies layout space when expanded (it pushes the document area); when
    /// collapsed it shrinks to a thin rail. This is the default.</summary>
    Docked,

    /// <summary>The panel never permanently occupies space: collapsed it shows only a thin rail, and
    /// when expanded it flies out as an overlay above the document area (auto-hide).</summary>
    AutoHide,
}
