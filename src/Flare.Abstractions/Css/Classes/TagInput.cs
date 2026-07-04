namespace Flare.Css.Classes;

/// <summary>CSS classes for tag input.</summary>
public static class TagInput
{
    /// <summary>The <c>flare-tag-input</c> CSS class.</summary>
    public const string Root = "flare-tag-input";
    // Focus ring is pure :focus-within on the shared flare-input__field; error/disabled use the shared
    // Css.Classes.Input.Error / Input.Disabled (flare-input--error/--disabled) - no tag-specific modifiers.
    /// <summary>The <c>flare-tag-input__field</c> CSS class.</summary>
    public const string Field = "flare-tag-input__field";
    /// <summary>The <c>flare-tag-input__input</c> CSS class.</summary>
    public const string Input = "flare-tag-input__input";
    /// <summary>The <c>flare-tag-input__chip</c> CSS class.</summary>
    public const string Chip = "flare-tag-input__chip";
    /// <summary>The <c>flare-tag-input__remove</c> CSS class.</summary>
    public const string Remove = "flare-tag-input__remove";
    /// <summary>The <c>flare-tag-input__wrapper</c> CSS class.</summary>
    public const string Wrapper = "flare-tag-input__wrapper";
    // The suggestion popup uses the shared Listbox.Dropdown (flare-listbox__dropdown) for anchoring.
    /// <summary>The <c>flare-tag-input__empty</c> CSS class.</summary>
    public const string Empty = "flare-tag-input__empty";
}
