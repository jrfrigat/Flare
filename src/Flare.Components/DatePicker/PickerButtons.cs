using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components;

/// <summary>
/// The small text buttons of the picker popups - a calendar's previous and next, a time popup's Cancel and OK - as
/// the same markup and classes a <see cref="FlareButton"/> renders, without being one: a popup is rendered on
/// demand, and a component per button made the first popup to open wait for the button's first render.
/// </summary>
internal static class PickerButtons
{
    private const string IconClass =
        Css.Classes.Button.Root + " " + Css.Classes.Button.Text + " " + Css.Classes.Button.SizeSm + " " + Css.Classes.Button.IconOnly;
    private const string TextClass = Css.Classes.Button.Root + " " + Css.Classes.Button.Text + " " + Css.Classes.Button.SizeSm;
    private const string IconSlotClass = Css.Classes.Button.Icon + " " + Css.Classes.Button.IconLeading;

    /// <summary>An icon-only button; the click does not reach the popup behind it.</summary>
    public static RenderFragment Icon(object receiver, Action onClick, string ariaLabel, FlareIcon icon, bool disabled = false) =>
        b => Build(b, IconClass, EventCallback.Factory.Create(receiver, onClick), ariaLabel, disabled, icon, null);

    /// <summary>A text button.</summary>
    public static RenderFragment Text(object receiver, Func<Task> onClick, string text, bool disabled = false) =>
        b => Build(b, TextClass, EventCallback.Factory.Create(receiver, onClick), null, disabled, null, text);

    private static void Build(RenderTreeBuilder b, string cssClass, EventCallback onClick, string? ariaLabel, bool disabled,
        FlareIcon? icon, string? text)
    {
        b.OpenElement(0, "button");
        b.AddAttribute(1, "class", cssClass);
        b.AddAttribute(2, "type", "button");
        b.AddAttribute(3, "disabled", disabled);
        b.AddAttribute(4, "aria-label", ariaLabel);
        b.AddAttribute(5, "onclick", onClick);
        b.AddEventStopPropagationAttribute(6, "onclick", true);
        if (icon is not null)
        {
            b.OpenElement(7, "span");
            b.AddAttribute(8, "class", IconSlotClass);
            b.OpenComponent<FlareIconView>(9);
            b.AddComponentParameter(10, nameof(FlareIconView.Value), icon);
            b.CloseComponent();
            b.CloseElement();
        }
        if (text is not null)
        {
            b.OpenElement(11, "span");
            b.AddAttribute(12, "class", Css.Classes.Button.Label);
            b.AddContent(13, text);
            b.CloseElement();
        }
        b.CloseElement();
    }
}
