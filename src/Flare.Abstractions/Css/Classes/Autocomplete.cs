namespace Flare.Css.Classes;

/// <summary>CSS classes for autocomplete.</summary>
public static class Autocomplete
{
    /// <summary>The <c>flare-autocomplete</c> CSS class.</summary>
    public const string Root = "flare-autocomplete";
    // Error + disabled state use the shared Css.Classes.Input.Error / Input.Disabled.
    // The editable input renders the shared flare-input__control (input.css); no component-specific
    // control class is needed.
    /// <summary>The <c>flare-autocomplete__field--open</c> CSS class.</summary>
    public const string FieldOpen = "flare-autocomplete__field--open";
    /// <summary>The <c>flare-autocomplete__icon</c> CSS class.</summary>
    public const string Icon = "flare-autocomplete__icon";
    // The popup uses the shared Listbox.Dropdown (flare-listbox__dropdown) for anchoring.
    /// <summary>The <c>flare-autocomplete__empty</c> CSS class.</summary>
    public const string Empty = "flare-autocomplete__empty";
    /// <summary>The <c>flare-autocomplete__wrapper</c> CSS class.</summary>
    public const string Wrapper = "flare-autocomplete__wrapper";
    /// <summary>The <c>flare-autocomplete__clear</c> CSS class.</summary>
    public const string Clear = "flare-autocomplete__clear";
    /// <summary>The <c>flare-autocomplete__field</c> CSS class.</summary>
    public const string Field = "flare-autocomplete__field";
    /// <summary>The <c>flare-autocomplete__spinner</c> CSS class.</summary>
    public const string Spinner = "flare-autocomplete__spinner";
}
