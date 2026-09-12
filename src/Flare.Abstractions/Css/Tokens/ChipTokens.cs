namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for chip.</summary>
public static class Chip
{
    /// <summary>CSS custom-property name for the radius token.</summary>
    public const string Radius = "--flare-chip-radius";
    /// <summary>CSS custom-property name for the height token.</summary>
    public const string Height = "--flare-chip-height";
    /// <summary>CSS custom-property name for the filled container background token.</summary>
    public const string FilledBg = "--flare-chip-filled-bg";
    /// <summary>CSS custom-property name for the elevated container background token.</summary>
    public const string ElevatedBg = "--flare-chip-elevated-bg";

    /// <summary>Glyph size of a chip's leading and trailing icons, one per size step. Both ends read
    /// the same ramp: a language that sizes the icon at one end sizes it at the other.</summary>
    public static class IconSize
    {
        /// <summary>CSS custom-property name for the xs chip icon size.</summary>
        public const string Xs = "--flare-chip-icon-size-xs";
        /// <summary>CSS custom-property name for the sm chip icon size.</summary>
        public const string Sm = "--flare-chip-icon-size-sm";
        /// <summary>CSS custom-property name for the md chip icon size.</summary>
        public const string Md = "--flare-chip-icon-size-md";
        /// <summary>CSS custom-property name for the lg chip icon size.</summary>
        public const string Lg = "--flare-chip-icon-size-lg";
        /// <summary>CSS custom-property name for the xl chip icon size.</summary>
        public const string Xl = "--flare-chip-icon-size-xl";
    }

    /// <summary>Box of a chip's leading avatar, one per size step. Separate from the icon because an
    /// avatar is a cropped picture filling a shape, not a glyph sitting in one.</summary>
    public static class AvatarSize
    {
        /// <summary>CSS custom-property name for the xs chip avatar size.</summary>
        public const string Xs = "--flare-chip-avatar-size-xs";
        /// <summary>CSS custom-property name for the sm chip avatar size.</summary>
        public const string Sm = "--flare-chip-avatar-size-sm";
        /// <summary>CSS custom-property name for the md chip avatar size.</summary>
        public const string Md = "--flare-chip-avatar-size-md";
        /// <summary>CSS custom-property name for the lg chip avatar size.</summary>
        public const string Lg = "--flare-chip-avatar-size-lg";
        /// <summary>CSS custom-property name for the xl chip avatar size.</summary>
        public const string Xl = "--flare-chip-avatar-size-xl";
    }
}
