using Flare.Components;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.IDE;

/// <summary>
/// Horizontal menu bar rendered below the ribbon. It coordinates the top-level <see cref="FlareMenu"/>
/// instances it contains through one shared <see cref="FlareMenuBarContext"/>: only one menu is open
/// at a time, another top-level menu switches on click or hover, and a single backdrop closes the open
/// menu on an outside click. Typically contains File, Edit, View, etc.; composed from base Flare CSS
/// classes - no custom tokens.
/// </summary>
public partial class FlareMenuBar : FlareComponentBase
{
    /// <summary>Menu items rendered inside the bar.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private readonly FlareMenuBarContext _context = new();

    protected override string ComponentCssClass => Css.Classes.Ide.MenuBar.Root;

    private string? _openClass => _context.HasOpenMenu ? Css.Classes.Ide.MenuBar.Open : null;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _context.StateChanged += HandleStateChanged;
    }

    // The notification can arrive from a child menu's event handler; marshal it like a callback so the
    // bar renders on the renderer's synchronization context.
    private void HandleStateChanged() => _ = InvokeAsync(StateHasChanged);

    private Task CloseOpenMenuAsync() => _context.CloseOpenMenuAsync();

    /// <inheritdoc />
    public override ValueTask DisposeAsync()
    {
        _context.StateChanged -= HandleStateChanged;
        return base.DisposeAsync();
    }
}
