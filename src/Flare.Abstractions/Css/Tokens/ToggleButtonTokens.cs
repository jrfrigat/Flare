namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for the segmented toggle group. The buttons inside it read the
/// <see cref="Button"/> family - a toggle is a button with a selected state - so only the container's
/// own chrome is named here.</summary>
public static class ToggleButton
{
    /// <summary>CSS custom-property name for the group border token.</summary>
    public const string GroupBorder = "--flare-togglegroup-border";
    /// <summary>CSS custom-property name for the group radius token.</summary>
    public const string GroupRadius = "--flare-togglegroup-radius";
    /// <summary>CSS custom-property name for the group radius vertical token.</summary>
    public const string GroupRadiusVertical = "--flare-togglegroup-radius-vertical";
    /// <summary>CSS custom-property name for the group divider token.</summary>
    public const string GroupDivider = "--flare-togglegroup-divider";
}
