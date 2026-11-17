using Flare.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.IDE;

/// <summary>
/// A title-bar caption control: one native button carrying the host's glyph, accessible label and
/// command. The component owns the measured title-bar box and every visual state through the public
/// <c>--flare-caption-*</c> tokens; it adds no product text, icon or command of its own. Mousedown
/// propagation is always stopped, so pressing a caption never starts a title-bar drag, and
/// <see cref="OnMouseDown"/> lets the host observe the press when it needs to.
/// </summary>
public partial class FlareCaptionButton : FlareComponentBase
{
    /// <summary>Which window command the control represents. The kind marks the control for tests and
    /// styling and gives the close control its danger paint; it does not decide the glyph, label or
    /// command. Defaults to <see cref="CaptionButtonKind.Minimize"/>, the harmless choice when a host
    /// omits it.</summary>
    [Parameter] public CaptionButtonKind Kind { get; set; } = CaptionButtonKind.Minimize;

    /// <summary>Disables the control. The disabled state still reserves the measured title-bar box but
    /// draws no hover or pressed surface.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>Callback invoked when the control is clicked. The host runs its window command here.</summary>
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>Callback invoked when the control receives a mousedown, after propagation has been
    /// stopped. Use it when the host needs to observe the press; the stop itself is always applied, so
    /// a title-bar drag never starts from a caption control.</summary>
    [Parameter] public EventCallback<MouseEventArgs> OnMouseDown { get; set; }

    /// <summary>Accessible label forwarded as <c>aria-label</c>. Strongly recommended: the control is
    /// icon-only and the library supplies no product text.</summary>
    [Parameter] public string? AriaLabel { get; set; }

    /// <summary>Hint shown on hover (native <c>title</c>), normally the same words as the accessible
    /// label. For a styled bubble, wrap the control in a <see cref="FlareTooltip"/> instead.</summary>
    [Parameter] public string? Tooltip { get; set; }

    /// <summary>The glyph rendered inside the control - any provider (<see cref="FlareSvgIcon"/> or an
    /// add-on pack type). It is drawn at the theme's caption glyph size unless the descriptor states
    /// its own size. Ignored when <see cref="ChildContent"/> is supplied.</summary>
    [Parameter] public FlareIcon? Icon { get; set; }

    /// <summary>Custom glyph content, used in place of <see cref="Icon"/>. The host owns its size and
    /// paint when it supplies this.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    // The control is the button element itself, so it has no wrapper class of its own beyond its root.
    /// <inheritdoc />
    protected override string ComponentCssClass => Css.Classes.Ide.CaptionButton.Root;

    private ElementReference _element;

    private string ButtonCssClass => BuildCssClass(
        Kind == CaptionButtonKind.Close ? Css.Classes.Ide.CaptionButton.Close : null);

    // Switch rather than Enum.ToString: the marker is read by hosts and tests, and a generated name
    // table is one allocation and one reflection path the hot path does not need.
    private string _kindValue => Kind switch
    {
        CaptionButtonKind.Maximize => "maximize",
        CaptionButtonKind.Restore => "restore",
        CaptionButtonKind.Close => "close",
        _ => "minimize",
    };

    // Propagation is stopped by the event modifier in the markup whether or not a callback is attached;
    // this handler only forwards the press to the host when it asked for it.
    private async Task HandleMouseDown(MouseEventArgs e)
    {
        if (OnMouseDown.HasDelegate) await OnMouseDown.InvokeAsync(e);
    }

    /// <summary>Sets keyboard focus to the control.</summary>
    public ValueTask FocusAsync() => _element.FocusAsync();
}
