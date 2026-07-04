namespace Flare.Css.Classes;

/// <summary>
/// CSS classes for the textarea. The chrome (root, field, label, supporting text, helper/error and
/// counter) is shared with the input family via <see cref="Input"/>; only the multi-line control
/// keeps its own classes.
/// </summary>
public static class Textarea
{
    /// <summary>The <c>flare-textarea__control</c> CSS class: the multi-line control (resize/auto-grow).</summary>
    public const string Control = "flare-textarea__control";
    /// <summary>The <c>flare-textarea--autogrow</c> CSS class: auto-resizes the control to its content.</summary>
    public const string Autogrow = "flare-textarea--autogrow";
}
