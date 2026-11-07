using System.Globalization;
using Flare.Abstractions;
using Flare.Theming;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Gantt.Tests;

public sealed class GanttLiveLocalizationTests : GanttTestBase
{
    [Fact]
    public async Task MountedGanttKeepsAxisAndRelocalizesLastKeyboardAnnouncement()
    {
        var culture = new FlareLocalization();
        culture.SetCulture(CultureInfo.GetCultureInfo("en-US"));
        Services.AddSingleton<IFlareLocalization>(culture);
        var cut = Render<FlareLocalizationProvider>(p => p.AddChildContent<FlareGantt>(g => g
            .Add(x => x.Range, GanttSpan.FromDays(Monday, Day(13))).Add(x => x.Culture, CultureInfo.GetCultureInfo("en-US"))
            .AddChildContent<FlareGanttPanel<Node, Work>>(panel => panel
                .Add(x => x.Rows, new[] { new Node("r1", new Node("r2")) })
                .Add(x => x.RowKey, n => n.Key).Add(x => x.RowChildren, n => n.Children)
                .Add(x => x.Items, new[] { new Work("a", "r1", Day(0), Day(1)) })
                .Add(x => x.ItemKey, x => x.Key).Add(x => x.ItemLabel, x => x.Key)
                .Add(x => x.ItemRow, x => x.Row).Add(x => x.ItemSpan, x => GanttSpan.FromDays(x.Start, x.End))
                .Add(x => x.Editable, true))));
        var gantt = cut.FindComponent<FlareGantt>().Instance;
        cut.Find(".flare-gantt__bar[data-key='a']").KeyDown(new KeyboardEventArgs { Key = "ArrowRight", CtrlKey = true });
        Assert.Contains("moved to", cut.Find("[role=status]").TextContent);
        await cut.InvokeAsync(() => culture.SetCulture(CultureInfo.GetCultureInfo("de-DE"), CultureInfo.GetCultureInfo("ru")));
        var text = culture.GetLocalizer<GanttStrings>();
        cut.WaitForAssertion(() => Assert.Equal(text["Gantt_Collapse"].Value, cut.Find(".flare-gantt__toggle").GetAttribute("aria-label")));
        Assert.StartsWith(text["Gantt_Moved"].Value.Split("{1}")[0].Replace("{0}", "a"), cut.Find("[role=status]").TextContent);
        Assert.Equal("en-US", gantt.Axis.Culture.Name);
        Assert.Same(gantt, cut.FindComponent<FlareGantt>().Instance);
    }
}
