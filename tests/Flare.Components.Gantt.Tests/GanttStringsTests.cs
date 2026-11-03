using System.Globalization;
using Flare.Components.Gantt;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Gantt.Tests;

public sealed class GanttStringsTests : GanttTestBase
{
    [Theory]
    [InlineData("en-US", "Expand")]
    [InlineData("ru-RU", "\u0420\u0430\u0437\u0432\u0435\u0440\u043d\u0443\u0442\u044c")]
    [InlineData("fr-FR", "Expand")]
    public void ResourcesUseUiCultureWithEnglishFallback(string culture, string expand)
    {
        using var ui = new GanttUiCulture(culture);
        Assert.Equal(expand, GanttStrings.Expand);
        Assert.All(new[] { GanttStrings.SelfLink, GanttStrings.Cycle, GanttStrings.Duplicate,
            GanttStrings.Expand, GanttStrings.Collapse, GanttStrings.LinkStarted, GanttStrings.LinkCreated,
            GanttStrings.Moved, GanttStrings.ResizeSide },
            text => Assert.False(string.IsNullOrEmpty(text) || text.StartsWith("Gantt_", StringComparison.Ordinal)));
    }

    [Fact]
    public void UiCultureIsIndependentOfAxisCultureAndChangesOnRender()
    {
        using var ui = new GanttUiCulture("en-US");
        var plan = new[] { new Work("a", "r1", Day(0), Day(1)) };
        var cut = Chart(p => p.Add(x => x.Rows, new[] { new Node("r1", new Node("r2")) })
            .Add(x => x.Items, plan).Add(x => x.Editable, true));
        Assert.Equal("en-US", CultureInfo.CurrentUICulture.Name);
        Assert.Equal("ru-RU", cut.Instance.Axis.Culture.Name);
        Assert.Equal("Collapse", cut.Find(".flare-gantt__toggle").GetAttribute("aria-label"));
        cut.Find(".flare-gantt__bar[data-key='a']").KeyDown(new KeyboardEventArgs { Key = "ArrowRight", CtrlKey = true });
        Assert.Contains("moved to", cut.Find("[role=status]").TextContent);

        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("ru-RU");
        cut.Render();
        Assert.Equal(GanttStrings.Collapse, cut.Find(".flare-gantt__toggle").GetAttribute("aria-label"));
    }
}

internal sealed class GanttUiCulture(string name) : IDisposable
{
    private readonly CultureInfo _previous = Set(name);
    private static CultureInfo Set(string name)
    {
        var previous = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(name);
        return previous;
    }
    public void Dispose() => CultureInfo.CurrentUICulture = _previous;
}
