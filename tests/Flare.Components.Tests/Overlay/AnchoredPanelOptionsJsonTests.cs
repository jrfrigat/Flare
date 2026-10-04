using System.Text.Json;
using Flare.Components.Services;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-193: panel options reach the overlay script as a JSON string written without reflection. Read back, the
/// string gives the same options System.Text.Json would have sent - a field left out stands for its default.
/// </summary>
public class AnchoredPanelOptionsJsonTests
{
    // Case-sensitive, as the script reads the names.
    private static readonly JsonSerializerOptions Web = new(JsonSerializerDefaults.Web) { PropertyNameCaseInsensitive = false };

    public static IEnumerable<object[]> Options() =>
    [
        [new AnchoredPanelOptions()],
        [new AnchoredPanelOptions { MatchWidth = true }],
        [new AnchoredPanelOptions { Placement = PanelPlacement.TopEnd, Gap = 9, GapToken = Flare.Css.Tokens.MenuPanel.Offset }],
        [new AnchoredPanelOptions { AnchorPoint = new PanelAnchorPoint(12.5, 80), TopLayer = false }],
        [new AnchoredPanelOptions { AnchorOffset = new PanelAnchorOffset(3, -4.25) }],
        [new AnchoredPanelOptions { AnchorRect = new PanelAnchorRect(10.5, 20, 30, 0.75), Gap = 0 }],
    ];

    [Theory]
    [MemberData(nameof(Options))]
    public void ReadBack_MatchesTheOptions(AnchoredPanelOptions options)
    {
        var json = AnchoredPanelOptionsJson.Write(options)!;
        var back = JsonSerializer.Deserialize<AnchoredPanelOptions>(json, Web)!;

        Assert.Equal(JsonSerializer.Serialize(options, Web), JsonSerializer.Serialize(back, Web));
    }

    [Fact]
    public void Defaults_WriteAnEmptyObject_AndNullStaysNull()
    {
        Assert.Equal("{}", AnchoredPanelOptionsJson.Write(new AnchoredPanelOptions()));
        Assert.Null(AnchoredPanelOptionsJson.Write(null));
    }
}
