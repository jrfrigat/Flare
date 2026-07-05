namespace Flare.Css.Classes;

/// <summary>
/// Shared dropdown listbox classes used by the field-family popups (FlareSelect, FlareMultiSelect,
/// FlareCombobox, FlareTagField) so the popup (rounded surface, elevation, option rows, group
/// headers and the anchored positioning) is styled once.
/// </summary>
public static class Listbox
{
    /// <summary>The <c>flare-listbox</c> CSS class.</summary>
    public const string Root = "flare-listbox";
    /// <summary>The <c>flare-listbox__dropdown</c> CSS class: anchors the popup absolutely under the
    /// field (shared by Autocomplete/MultiSelect/TagField; Select positions its popup with JS instead).</summary>
    public const string Dropdown = "flare-listbox__dropdown";
    /// <summary>The <c>flare-listbox__option</c> CSS class.</summary>
    public const string Option = "flare-listbox__option";
    /// <summary>The <c>flare-listbox__option--active</c> CSS class.</summary>
    public const string OptionActive = "flare-listbox__option--active";
    /// <summary>The <c>flare-listbox__option--selected</c> CSS class.</summary>
    public const string OptionSelected = "flare-listbox__option--selected";
    /// <summary>The <c>flare-listbox__group-header</c> CSS class.</summary>
    public const string GroupHeader = "flare-listbox__group-header";
    /// <summary>The <c>flare-listbox__check</c> CSS class.</summary>
    public const string Check = "flare-listbox__check";
    /// <summary>The <c>flare-listbox__checkbox</c> CSS class: the leading visual checkbox on a multi-select
    /// option row (an <c>aria-hidden</c> presentation element; the option's <c>aria-selected</c> conveys state).</summary>
    public const string Checkbox = "flare-listbox__checkbox";
    /// <summary>The <c>flare-listbox__empty</c> CSS class: the "no results" row shown when a query matches nothing.</summary>
    public const string Empty = "flare-listbox__empty";
}
