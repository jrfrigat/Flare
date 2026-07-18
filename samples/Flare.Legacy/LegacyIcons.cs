using Flare.Components;

namespace Flare.Legacy;

/// <summary>
/// Resolves the Material-Symbols-style icon-name strings used in this demo app to typed
/// <see cref="FlareIcon"/> values (typed members, so a trimmed publish ships only these).
/// </summary>
public static class LegacyIcons
{
    private static readonly Dictionary<string, FlareIcon> _map = new(StringComparer.OrdinalIgnoreCase)
    {
        ["alternate_email"] = MaterialDesign3Icons.Regular.AlternateEmail,
        ["call"] = MaterialDesign3Icons.Regular.Call,
        ["chat"] = MaterialDesign3Icons.Regular.Chat,
        ["content_copy"] = FlareIcons.ContentCopy,
        ["description"] = FlareIcons.Description,
        ["event_available"] = MaterialDesign3Icons.Regular.EventAvailable,
        ["folder"] = FlareIcons.Folder,
        ["grid_view"] = MaterialDesign3Icons.Regular.GridView,
        ["home"] = FlareIcons.Home,
        ["insights"] = MaterialDesign3Icons.Regular.Analytics,
        ["logout"] = MaterialDesign3Icons.Regular.Logout,
        ["mail"] = MaterialDesign3Icons.Regular.Mail,
        ["receipt_long"] = MaterialDesign3Icons.Regular.ReceiptLong,
        ["schedule"] = FlareIcons.Schedule,
        ["send"] = FlareIcons.Send,
        ["sms"] = MaterialDesign3Icons.Regular.Sms,
        ["warning"] = FlareIcons.Warning,
        ["work"] = MaterialDesign3Icons.Regular.Work,
    };

    /// <summary>The typed icon for a demo icon-name string; built-in set or an empty icon as fallback.</summary>
    public static FlareIcon Resolve(string? name) =>
        name is not null && _map.TryGetValue(name, out var icon)
            ? icon
            : FlareIcons.Find(name ?? string.Empty) ?? FlareIcons.Empty;
}
