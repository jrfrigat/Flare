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

    /// <summary>Family of a chip label. A chip is as often an identifier - a ticket key, a version, a
    /// type tag - as it is a word, and an identifier wants the face the rest of the identifiers use.</summary>
    public const string LabelFont = "--flare-chip-label-font";
    /// <summary>Weight of a chip label.</summary>
    public const string LabelWeight = "--flare-chip-label-weight";
    /// <summary>Letter-spacing of a chip label, which a small all-caps tag usually opens up.</summary>
    public const string LabelSpacing = "--flare-chip-label-spacing";

    /// <summary>Size of a chip label, one per size step - the chip's other measurements are already a
    /// ramp, and a label that did not follow it would be the one thing a theme could not scale.</summary>
    public static class LabelSize
    {
        /// <summary>CSS custom-property name for the xs chip label size.</summary>
        public const string Xs = "--flare-chip-label-size-xs";
        /// <summary>CSS custom-property name for the sm chip label size.</summary>
        public const string Sm = "--flare-chip-label-size-sm";
        /// <summary>CSS custom-property name for the md chip label size.</summary>
        public const string Md = "--flare-chip-label-size-md";
        /// <summary>CSS custom-property name for the lg chip label size.</summary>
        public const string Lg = "--flare-chip-label-size-lg";
        /// <summary>CSS custom-property name for the xl chip label size.</summary>
        public const string Xl = "--flare-chip-label-size-xl";
    }

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

    /// <summary>Inline padding of a chip across the 5 sizes.</summary>
    public static class PaddingInline
    {
        /// <summary>CSS custom-property name for the xs chip inline padding.</summary>
        public const string Xs = "--flare-chip-padding-inline-xs";
        /// <summary>CSS custom-property name for the sm chip inline padding.</summary>
        public const string Sm = "--flare-chip-padding-inline-sm";
        /// <summary>CSS custom-property name for the md chip inline padding.</summary>
        public const string Md = "--flare-chip-padding-inline-md";
        /// <summary>CSS custom-property name for the lg chip inline padding.</summary>
        public const string Lg = "--flare-chip-padding-inline-lg";
        /// <summary>CSS custom-property name for the xl chip inline padding.</summary>
        public const string Xl = "--flare-chip-padding-inline-xl";
    }
}
