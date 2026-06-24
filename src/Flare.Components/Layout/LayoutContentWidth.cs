namespace Flare.Components;

/// <summary>
/// Maps a <see cref="ContainerMaxWidth"/> to the shared content-frame CSS modifier used by
/// FlareLayout (slot API) and FlareLayoutContent (component API) to cap and center content.
/// </summary>
internal static class LayoutContentWidth
{
    public static string ToClass(ContainerMaxWidth maxWidth) => maxWidth switch
    {
        ContainerMaxWidth.Xs => Css.Classes.Layout.ContentXs,
        ContainerMaxWidth.Sm => Css.Classes.Layout.ContentSm,
        ContainerMaxWidth.Md => Css.Classes.Layout.ContentMd,
        ContainerMaxWidth.Lg => Css.Classes.Layout.ContentLg,
        ContainerMaxWidth.Xl => Css.Classes.Layout.ContentXl,
        _ => string.Empty,
    };

    /// <summary>Maps a content alignment to the shared content-frame placement modifier.</summary>
    public static string ToAlignClass(LayoutContentAlignment alignment) => alignment switch
    {
        LayoutContentAlignment.Start => Css.Classes.Layout.ContentAlignStart,
        LayoutContentAlignment.End => Css.Classes.Layout.ContentAlignEnd,
        _ => Css.Classes.Layout.ContentAlignCenter,
    };
}
