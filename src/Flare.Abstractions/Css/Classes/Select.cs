namespace Flare.Css.Classes;

/// <summary>CSS classes for select.</summary>
public static class Select
{
    /// <summary>The <c>flare-select</c> CSS class.</summary>
    public const string Root = "flare-select";
    /// <summary>The <c>flare-select--open</c> CSS class.</summary>
    public const string Open = "flare-select--open";
    // Disabled + error use the shared Css.Classes.Input.Disabled / Input.Error (flare-input--*);
    // the fused trigger's descendant rules key off those in select.css.
    /// <summary>The <c>flare-select__control</c> CSS class.</summary>
    public const string Control = "flare-select__control";
    /// <summary>The <c>flare-select__value</c> CSS class.</summary>
    public const string Value = "flare-select__value";
    // The placeholder span uses the shared Css.Classes.Input.Placeholder (flare-input__placeholder).
    // The trailing chevron uses the shared Css.Classes.Input.Arrow (flare-input__arrow).
    /// <summary>The <c>flare-select__dropdown</c> CSS class.</summary>
    public const string Dropdown = "flare-select__dropdown";
    /// <summary>The <c>flare-select__option</c> CSS class (option row; visuals come from the shared <see cref="Listbox.Option"/>).</summary>
    public const string Option = "flare-select__option";
    /// <summary>The <c>flare-select__search</c> CSS class: the in-field search input in the trigger, shared
    /// by Select and MultiSelect when <c>Searchable</c> is set.</summary>
    public const string Search = "flare-select__search";
    /// <summary>The <c>flare-select__empty</c> CSS class: the "no results" row shown in the dropdown when a
    /// search query matches no options.</summary>
    public const string Empty = "flare-select__empty";
}
