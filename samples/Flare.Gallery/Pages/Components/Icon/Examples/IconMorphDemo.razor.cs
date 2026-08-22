using Flare.Components;

namespace Flare.Gallery.Pages.Components.Icon.Examples;

public partial class IconMorphDemo
{
    private static readonly FlareIconMorph[] _modes =
        [FlareIconMorph.None, FlareIconMorph.Fade, FlareIconMorph.Scale, FlareIconMorph.Rotate];

    // Deliberately unrelated outlines: the point of the mode is that any pair transitions, with no shared
    // path structure to interpolate.
    private static readonly FlareIcon[] _icons =
        [FlareIcons.Menu, FlareIcons.Close, FlareIcons.Search, FlareIcons.Favorite];

    private int _index;

    private void Next() => _index = (_index + 1) % _icons.Length;
}
