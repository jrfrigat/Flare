using Flare.Components;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.IDE;

/// <summary>
/// A collapsible tool panel for the IDE sidebar. Wraps content with a header bar
/// containing title, optional toolbar, and collapse button.
/// Composed from base Flare components - no custom tokens.
/// When placed inside a <see cref="FlareIdeLayout"/> region the layout owns the collapsed state and
/// the thin rail / auto-hide flyout, so collapsing frees the docked space.
/// </summary>
public partial class FlareToolPanel : FlareLocalizedComponentBase
{
    private Microsoft.Extensions.Localization.IStringLocalizer<IdeStrings> IdeText => Localizer<IdeStrings>();
    /// <summary>The enclosing IDE layout, when this panel is used in one of its regions.</summary>
    [CascadingParameter] internal FlareIdeLayout? Layout { get; set; }

    /// <summary>The enclosing tabbed group, when this panel is one of its tabs.</summary>
    [CascadingParameter] internal FlareToolPanelGroup? Group { get; set; }

    /// <summary>Panel title displayed in the header (and on the collapsed rail inside a layout).</summary>
    [Parameter] public string? Title { get; set; }

    /// <summary>Icon that represents the tool. Inside a <see cref="FlareToolPanelGroup"/> with
    /// <see cref="FlareToolPanelGroup.TabStripPlacement"/> set to
    /// <see cref="Flare.Components.IDE.TabStripPlacement.Bottom"/> it is shown beside the active tool's
    /// title in the header; inside a <see cref="FlareIdeLayout"/> it identifies the tool on the per-tool
    /// auto-hide rail. Null renders no icon.</summary>
    [Parameter] public FlareIcon? Icon { get; set; }

    /// <summary>Stable key that <see cref="FlareToolPanelGroup.ActiveKey"/> uses to select this panel.
    /// When unset the panel is addressed by <see cref="Title"/>, which is enough for a fixed, untranslated
    /// title; set an explicit key when titles are localized, repeat or change at run time.</summary>
    [Parameter] public string? Key { get; set; }

    /// <summary>Content rendered inside the panel body.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Optional toolbar content in the header (e.g. filter, settings buttons).</summary>
    [Parameter] public RenderFragment? ToolbarContent { get; set; }

    /// <summary>Whether to show the toolbar area.</summary>
    [Parameter] public bool ShowToolbar { get; set; }

    /// <summary>Position of the panel in the IDE layout.</summary>
    [Parameter] public ToolPanelPosition Position { get; set; } = ToolPanelPosition.Left;

    /// <summary>How the panel occupies space inside a <see cref="FlareIdeLayout"/>: <see cref="IdePanelMode.Docked"/>
    /// (takes space when expanded) or <see cref="IdePanelMode.AutoHide"/> (flies out as an overlay).
    /// Ignored when the panel is used standalone (outside a layout).</summary>
    [Parameter] public IdePanelMode Mode { get; set; } = IdePanelMode.Docked;

    /// <summary>Whether the panel is currently collapsed. Used for standalone (no-layout) collapsing;
    /// inside a layout the layout owns the collapsed state.</summary>
    [Parameter] public bool Collapsed { get; set; }

    /// <summary>Callback when the collapsed state changes (standalone use).</summary>
    [Parameter] public EventCallback<bool> CollapsedChanged { get; set; }

    protected override string ComponentCssClass => Css.Classes.Ide.ToolPanel.Root;

    private readonly string _tabSuffix = Guid.NewGuid().ToString("N")[..8];
    internal string TabId => $"flare-toolpanel-tab-{_tabSuffix}";
    internal string PanelId => $"flare-toolpanel-panel-{_tabSuffix}";
    internal ElementReference TabElement { get; set; }

    /// <summary>Identity of the panel inside its group: <see cref="Key"/> when set, otherwise
    /// <see cref="Title"/>; null when the panel has neither.</summary>
    internal string? EffectiveKey =>
        !string.IsNullOrEmpty(Key) ? Key : string.IsNullOrEmpty(Title) ? null : Title;

    private FlareToolPanelGroup? _registeredGroup;

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (!ReferenceEquals(_registeredGroup, Group))
        {
            _registeredGroup?.Unregister(this);
            _registeredGroup = Group;
        }
        _registeredGroup?.Register(this);

        // Inside a layout the panel only renders while expanded; report its title/mode so the layout
        // can label the collapsed rail and decide docked-vs-flyout sizing. A group owns the region
        // position, so its panels do not repeat it.
        Layout?.RegisterPanel(this, Group?.Position ?? Position, Title, Mode);
    }

    /// <inheritdoc />
    public override ValueTask DisposeAsync()
    {
        _registeredGroup?.Unregister(this);
        Layout?.UnregisterPanel(this);
        return base.DisposeAsync();
    }

    private string? _positionClass => Position switch
    {
        ToolPanelPosition.Right => Css.Classes.Ide.ToolPanel.Right,
        ToolPanelPosition.Bottom => Css.Classes.Ide.ToolPanel.Bottom,
        _ => Css.Classes.Ide.ToolPanel.Left
    };

    // Standalone collapse only; inside a layout the panel is unmounted when collapsed.
    private string? _collapsedClass => (Layout is null && Collapsed) ? Css.Classes.Ide.ToolPanel.Collapsed : null;

    private string? _rootStyle => (Layout is null && Collapsed)
        ? $"width:40px;min-width:40px;{Style}"
        : Style;

    private async Task ToggleCollapsed()
    {
        if (Layout is not null)
        {
            // Hand off to the layout, which renders the rail and reclaims the docked space.
            await Layout.CollapseAsync(Position);
            return;
        }

        Collapsed = !Collapsed;
        await CollapsedChanged.InvokeAsync(Collapsed);
    }
}
