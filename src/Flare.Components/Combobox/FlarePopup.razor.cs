using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components;

/// <summary>
/// The anchored dropdown panel for the select family (see the markup partial for the rationale). Owns the
/// fixed-position anchoring and the single unified dismissal handler through <see cref="IOverlayJsService"/>,
/// so no shell re-implements the open/position/dismiss lifecycle or a blur timer.
/// </summary>
public partial class FlarePopup
{
    [Inject] private IOverlayJsService Overlay { get; set; } = default!;

    /// <inheritdoc />
    protected override string ComponentCssClass => string.Empty;

    /// <summary>Whether the panel is open (rendered + positioned).</summary>
    [Parameter] public bool Open { get; set; }

    /// <summary>The element the panel is positioned under (the trigger).</summary>
    [Parameter, EditorRequired] public ElementReference Anchor { get; set; }

    /// <summary>The widget root used for dismissal containment (interactions inside it do not dismiss).</summary>
    [Parameter, EditorRequired] public ElementReference DismissRoot { get; set; }

    /// <summary>Invoked when a pointer-down outside the widget or a focus-out escaping it should dismiss.</summary>
    [Parameter] public EventCallback OnDismiss { get; set; }

    /// <summary>Sizes the panel to the anchor width (default true).</summary>
    [Parameter] public bool MatchWidth { get; set; } = true;

    // The panel CSS class is the inherited FlareComponentBase.Class.

    /// <summary>The panel content (typically a <c>FlareOptionList</c>).</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private ElementReference _panel;
    private readonly string _id = $"flare-popup-{Guid.NewGuid():N}";
    private DotNetObjectReference<FlarePopup>? _selfRef;
    private bool _registered;

    /// <summary>The panel element, for callers that need to measure or focus it.</summary>
    public ElementReference Panel => _panel;

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (Open == _registered) return;
        try
        {
            if (Open)
            {
                _selfRef ??= DotNetObjectReference.Create(this);
                await Overlay.PositionAnchoredPanelAsync(_id, Anchor, _panel, new { matchWidth = MatchWidth });
                await Overlay.RegisterDismissAsync(_id, DismissRoot, _selfRef, nameof(DismissFromJs));
                _registered = true;
            }
            else
            {
                await Overlay.RemoveDismissAsync(_id);
                await Overlay.RemoveAnchoredPanelAsync(_id);
                _registered = false;
            }
        }
        catch (JSDisconnectedException) { }
    }

    /// <summary>Scrolls the option element with <paramref name="optionId"/> into view within the panel.</summary>
    /// <param name="optionId">The option element id to reveal.</param>
    public async ValueTask ScrollOptionIntoViewAsync(string optionId)
    {
        try { await Overlay.ScrollIntoViewAsync(optionId); }
        catch (JSDisconnectedException) { }
    }

    /// <summary>Invoked from JS when a pointer-down outside the widget or a focus-out escaping it occurs.</summary>
    [JSInvokable]
    public Task DismissFromJs() => OnDismiss.InvokeAsync();

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        if (_registered)
        {
            try
            {
                await Overlay.RemoveDismissAsync(_id);
                await Overlay.RemoveAnchoredPanelAsync(_id);
            }
            catch (JSDisconnectedException) { }
        }
        _selfRef?.Dispose();
        await base.DisposeAsync();
    }
}
