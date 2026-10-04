using System.Text;
using System.Text.Json;

namespace Flare.Components.Services;

// Panel options cross to the overlay script as a JSON string written field by field. Handing JSRuntime the object
// instead makes System.Text.Json build the type's metadata by reflection, which is what the first popup with
// options waited for. Names are camel-cased as JSRuntime writes them; a field left at its default is left out,
// and the script reads a missing field as that default.
internal static class AnchoredPanelOptionsJson
{
    public static string? Write(AnchoredPanelOptions? options)
    {
        if (options is null) return null;

        using var buffer = new MemoryStream();
        using (var json = new Utf8JsonWriter(buffer))
        {
            json.WriteStartObject();
            if (options.Placement != PanelPlacement.BottomStart) json.WriteString("placement", options.Placement);
            if (options.Gap != 4) json.WriteNumber("gap", options.Gap);
            if (options.GapToken is { } token) json.WriteString("gapToken", token);
            if (options.MatchWidth) json.WriteBoolean("matchWidth", true);
            if (options.AnchorPoint is { } point)
            {
                json.WriteStartObject("anchorPoint");
                json.WriteNumber("xPct", point.XPct);
                json.WriteNumber("yPct", point.YPct);
                json.WriteEndObject();
            }
            if (options.AnchorOffset is { } offset)
            {
                json.WriteStartObject("anchorOffset");
                json.WriteNumber("x", offset.X);
                json.WriteNumber("y", offset.Y);
                json.WriteEndObject();
            }
            if (options.AnchorRect is { } rect)
            {
                json.WriteStartObject("anchorRect");
                json.WriteNumber("x", rect.X);
                json.WriteNumber("y", rect.Y);
                json.WriteNumber("width", rect.Width);
                json.WriteNumber("height", rect.Height);
                json.WriteEndObject();
            }
            if (!options.TopLayer) json.WriteBoolean("topLayer", false);
            json.WriteEndObject();
        }
        return Encoding.UTF8.GetString(buffer.GetBuffer(), 0, (int)buffer.Length);
    }
}
