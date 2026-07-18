using Flare.Components;

namespace Flare.Gallery.Models;

/// <summary>
/// Resolves the Material-Symbols-style icon-name strings used in the gallery demo DATA (global search
/// index, carousel slides, card/select samples, ...) to typed <see cref="FlareIcon"/> values. Listed as
/// typed members so a trimmed publish ships only these; the demo data stays as readable string ids.
/// </summary>
public static class GalleryIcons
{
    private static readonly Dictionary<string, FlareIcon> _map = new(StringComparer.OrdinalIgnoreCase)
    {
        ["add"] = FlareIcons.Add,
        ["arrow_back"] = FlareIcons.ArrowBack,
        ["arrow_downward"] = FlareIcons.ArrowDownward,
        ["arrow_forward"] = FlareIcons.ArrowForward,
        ["arrow_upward"] = FlareIcons.ArrowUpward,
        ["bar_chart"] = MaterialDesign3Icons.Regular.BarChart,
        ["border_outer"] = MaterialDesign3Icons.Regular.BorderOuter,
        ["check"] = FlareIcons.Check,
        ["close"] = FlareIcons.Close,
        ["code"] = FlareIcons.Code,
        ["data_object"] = FlareIcons.DataObject,
        ["delete"] = FlareIcons.Delete,
        ["download"] = FlareIcons.Download,
        ["drag_handle"] = FlareIcons.DragHandle,
        ["edit"] = FlareIcons.Edit,
        ["expand_less"] = MaterialDesign3Icons.Regular.KeyboardArrowUp,
        ["favorite"] = FlareIcons.Favorite,
        ["format_color_fill"] = MaterialDesign3Icons.Regular.FormatColorFill,
        ["home"] = FlareIcons.Home,
        ["info"] = FlareIcons.Info,
        ["layers"] = MaterialDesign3Icons.Regular.Layers,
        ["menu"] = FlareIcons.Menu,
        ["menu_open"] = MaterialDesign3Icons.Regular.MenuOpen,
        ["notifications"] = FlareIcons.Notifications,
        ["palette"] = FlareIcons.Palette,
        ["people"] = MaterialDesign3Icons.Regular.Group,
        ["person"] = FlareIcons.Person,
        ["rocket_launch"] = FlareIcons.RocketLaunch,
        ["search"] = FlareIcons.Search,
        ["settings"] = FlareIcons.Settings,
        ["share"] = FlareIcons.Share,
        ["star"] = FlareIcons.Star,
        ["tune"] = FlareIcons.Tune,
        ["upload"] = FlareIcons.Upload,
        ["wallpaper"] = MaterialDesign3Icons.Regular.Wallpaper,
        ["warning"] = FlareIcons.Warning,
        ["widgets"] = MaterialDesign3Icons.Regular.Widgets,
    };

    /// <summary>The typed icon for a demo icon-name string; built-in set or an empty icon as fallback.</summary>
    public static FlareIcon Resolve(string? name) =>
        name is not null && _map.TryGetValue(name, out var icon)
            ? icon
            : FlareIcons.Find(name ?? string.Empty) ?? FlareIcons.Empty;
}
