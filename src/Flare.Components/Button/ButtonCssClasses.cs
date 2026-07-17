namespace Flare.Components;

/// <summary>
/// Maps the button appearance enums onto the <c>flare-btn</c> class names.
/// </summary>
/// <remarks>
/// This exists so that a control which is button-shaped but cannot nest a <c>FlareButton</c> can still wear
/// the real button classes instead of re-implementing the chrome in its own CSS. <c>FlareFileUploadButton</c>
/// is the case: its trigger has to be a <c>&lt;label for&gt;</c> over the hidden file input, because a
/// <c>&lt;button&gt;</c> inside a label does not open the picker. Sharing the mapping - rather than copying
/// the switch - is what keeps the two from drifting apart as the button family grows.
/// </remarks>
public static class ButtonCssClasses
{
    /// <summary>Class for a <see cref="ButtonVariant"/>. Unknown values fall back to filled.</summary>
    public static string Variant(ButtonVariant variant) => variant switch
    {
        ButtonVariant.Filled => Css.Classes.Button.Filled,
        ButtonVariant.Outlined => Css.Classes.Button.Outlined,
        ButtonVariant.Text => Css.Classes.Button.Text,
        ButtonVariant.Elevated => Css.Classes.Button.Elevated,
        ButtonVariant.Tonal => Css.Classes.Button.Tonal,
        _ => Css.Classes.Button.Filled,
    };

    /// <summary>Class for a <see cref="ButtonSize"/>. Unknown values fall back to medium.</summary>
    public static string Size(ButtonSize size) => size switch
    {
        ButtonSize.Xs => Css.Classes.Button.Xs,
        ButtonSize.Sm => Css.Classes.Button.Sm,
        ButtonSize.Md => Css.Classes.Button.Md,
        ButtonSize.Lg => Css.Classes.Button.Lg,
        ButtonSize.Xl => Css.Classes.Button.Xl,
        _ => Css.Classes.Button.Md,
    };

    /// <summary>Class for a <see cref="ButtonShape"/>, or an empty string for the default shape.</summary>
    public static string Shape(ButtonShape shape) => shape switch
    {
        ButtonShape.Rounded => Css.Classes.Button.Rounded,
        ButtonShape.Circular => Css.Classes.Button.Circular,
        ButtonShape.Square => Css.Classes.Button.Square,
        _ => string.Empty,
    };
}
