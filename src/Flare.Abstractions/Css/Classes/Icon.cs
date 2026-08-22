namespace Flare.Css.Classes;

/// <summary>CSS classes for icon.</summary>
public static class Icon
{
    /// <summary>The <c>flare-icon</c> CSS class.</summary>
    public const string Root = "flare-icon";
    /// <summary>The <c>flare-icon--svg</c> CSS class.</summary>
    public const string Svg = "flare-icon--svg";

    /// <summary>The <c>flare-icon--path-morph</c> CSS class; the outline itself transitions instead of being replaced.</summary>
    public const string PathMorph = "flare-icon--path-morph";

    /// <summary>The <c>flare-icon-morph</c> CSS class - the wrapper that stacks the outgoing and incoming glyph.</summary>
    public const string Morph = "flare-icon-morph";
    /// <summary>The <c>flare-icon-morph--fade</c> CSS class; the swap is opacity only.</summary>
    public const string MorphFade = "flare-icon-morph--fade";
    /// <summary>The <c>flare-icon-morph--scale</c> CSS class; the swap also travels through the scale token.</summary>
    public const string MorphScale = "flare-icon-morph--scale";
    /// <summary>The <c>flare-icon-morph--rotate</c> CSS class; the swap also travels through the rotation token.</summary>
    public const string MorphRotate = "flare-icon-morph--rotate";
    /// <summary>The <c>flare-icon-morph__slot</c> CSS class - one glyph, sharing the wrapper's single grid cell.</summary>
    public const string MorphSlot = "flare-icon-morph__slot";
    /// <summary>The <c>flare-icon-morph__slot--enter</c> CSS class, carried by the glyph being swapped in.</summary>
    public const string MorphSlotEnter = "flare-icon-morph__slot--enter";
    /// <summary>The <c>flare-icon-morph__slot--exit</c> CSS class, carried by the glyph being swapped out.</summary>
    public const string MorphSlotExit = "flare-icon-morph__slot--exit";
}
