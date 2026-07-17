using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components;

/// <summary>
/// Common contract shared by every Flare field-family component (text field, select, autocomplete,
/// pickers, numeric/masked/OTP fields, tag field, sliders, switches, ...). It captures the label and
/// helper/error "field chrome" plus the disabled state, so a consumer (or a generic host such as
/// <c>FlareFormBuilder</c>) can treat any field uniformly and the components declare these parameters
/// once via <see cref="FlareFieldBase"/> instead of re-declaring them each.
/// </summary>
public interface IFlareField
{
    /// <summary>Label text shown for the field.</summary>
    string? Label { get; set; }
    /// <summary>Placeholder text shown when the field is empty. (Not meaningful for every field, e.g. toggles.)</summary>
    string? Placeholder { get; set; }
    /// <summary>Helper text shown below the field.</summary>
    string? HelperText { get; set; }
    /// <summary>Error text; when set it overrides <see cref="HelperText"/> and marks the field invalid.</summary>
    string? ErrorText { get; set; }
    /// <summary>Disables the field (no input, dimmed).</summary>
    bool Disabled { get; set; }
    /// <summary>Makes the field read-only (value shown but not editable). (Not meaningful for every field.)</summary>
    bool ReadOnly { get; set; }
    /// <summary>Marks the field as required (visual indicator + native <c>required</c> where applicable).</summary>
    bool Required { get; set; }
}

/// <summary>A single-value Flare field: its bound value and the change callback for <c>@bind-Value</c>.</summary>
/// <typeparam name="TValue">The bound value type.</typeparam>
public interface IFlareField<TValue> : IFlareField
{
    /// <summary>The bound value.</summary>
    TValue? Value { get; set; }
    /// <summary>Raised when the value changes, enabling <c>@bind-Value</c>.</summary>
    EventCallback<TValue?> ValueChanged { get; set; }
}

/// <summary>A multi-value Flare field (multi-select, tag field, toggle group): a collection value.</summary>
/// <typeparam name="TValue">The element type of the bound collection.</typeparam>
public interface IFlareMultiField<TValue> : IFlareField
{
    /// <summary>The bound collection of selected values.</summary>
    IReadOnlyList<TValue> Values { get; set; }
    /// <summary>Raised when the selection changes, enabling <c>@bind-Values</c>.</summary>
    EventCallback<IReadOnlyList<TValue>> ValuesChanged { get; set; }
}

/// <summary>
/// The look and interactive state every button-shaped thing in Flare shares: variant, size, color, and
/// whether it is disabled or busy. Anything a caller would recognise as a button in a toolbar row exposes
/// these under the same names and types, so knowing one means knowing the rest.
///
/// This is deliberately separate from <see cref="IFlareButton"/>, which adds <c>OnClick</c>. Some members
/// of the family are button-shaped but are not activated by a plain click: <c>FlareClipboard</c> raises
/// <c>OnCopied</c> and <c>FlareFileUpload</c> raises <c>OnFilesChanged</c>. Giving those an <c>OnClick</c>
/// too would hand a caller two competing "the user pressed it" events, so they take the appearance only.
/// </summary>
public interface IFlareButtonAppearance
{
    /// <summary>Visual variant (filled/outlined/text/...).</summary>
    ButtonVariant Variant { get; set; }
    /// <summary>Size scale (Xs..Xl).</summary>
    ButtonSize Size { get; set; }
    /// <summary>Semantic color of the button. Part of the shared appearance surface so button wrappers
    /// (clipboard, split button, ...) forward it consistently instead of silently dropping it.</summary>
    FlareColor Color { get; set; }
    /// <summary>Disables the button.</summary>
    bool Disabled { get; set; }
    /// <summary>Shows a busy/loading state and blocks activation.</summary>
    bool Loading { get; set; }
}

/// <summary>
/// A button-shaped control whose action IS the press: it carries the whole appearance surface plus
/// <c>OnClick</c>. Implemented by <c>FlareButton</c>, <c>FlareIconButton</c> and <c>FlareSplitButton</c>.
/// Controls with their own action event implement <see cref="IFlareButtonAppearance"/> instead.
/// </summary>
public interface IFlareButton : IFlareButtonAppearance
{
    /// <summary>Raised when the button is activated.</summary>
    EventCallback<MouseEventArgs> OnClick { get; set; }
}

/// <summary>
/// Shared open/close contract for the Flare elevated-surface family (menu, popover, tooltip, dialog,
/// drawer, snackbar): a two-way-bindable open state. Overlays converge on <c>@bind-Open</c> so hosts
/// can drive any of them the same way.
/// </summary>
public interface IFlareOverlay
{
    /// <summary>Whether the surface is currently open.</summary>
    bool Open { get; set; }
    /// <summary>Raised when the open state changes, enabling <c>@bind-Open</c>.</summary>
    EventCallback<bool> OpenChanged { get; set; }
}
