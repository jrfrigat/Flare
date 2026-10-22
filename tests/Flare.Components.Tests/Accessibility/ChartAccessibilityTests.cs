using System.Globalization;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests.Accessibility;

public class ChartAccessibilityTests : FlareTestContext
{
    private static readonly ChartData Data = new([new ChartSeries("First", [1, 2]), new ChartSeries("Second", [3, 4])]);

    [Fact]
    public void LegendButtonsExposeVisibilityAndDoNotSubmitForms()
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Data, Data));
        var buttons = cut.FindAll($"button.{Css.Classes.Chart.LegendItem}");
        Assert.Equal(2, buttons.Count);
        Assert.All(buttons, b => { Assert.Equal("button", b.GetAttribute("type")); Assert.Equal("true", b.GetAttribute("aria-pressed")); });
        buttons[0].Click();
        Assert.Equal("false", cut.FindAll($"button.{Css.Classes.Chart.LegendItem}")[0].GetAttribute("aria-pressed"));
        Assert.Equal("First", cut.FindAll($"button.{Css.Classes.Chart.LegendItem}")[0].TextContent);
    }

    [Theory]
    [InlineData("en", null, "Chart")]
    [InlineData("en", "", "Chart")]
    [InlineData("ru", null, "\u0414\u0438\u0430\u0433\u0440\u0430\u043c\u043c\u0430")]
    [InlineData("ru", "  ", "\u0414\u0438\u0430\u0433\u0440\u0430\u043c\u043c\u0430")]
    public void MissingTitleUsesTheLocalizedAccessibleName(string culture, string? title, string expected)
    {
        var previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
            var cut = Render<FlareChart>(p => p.Add(x => x.Data, Data).Add(x => x.Title, title));
            Assert.Equal(expected, cut.Find("svg").GetAttribute("aria-label"));
            Assert.Equal(expected, cut.Find("svg title").TextContent);
        }
        finally { CultureInfo.CurrentUICulture = previous; }
    }

    [Fact]
    public void SvgDescriptionReferencesItsOwnElement()
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Data, Data).Add(x => x.Title, "Sales").Add(x => x.Description, "Monthly totals"));
        Assert.Equal("Sales", cut.Find("svg").GetAttribute("aria-label"));
        Assert.Equal(cut.Find("desc").Id, cut.Find("svg").GetAttribute("aria-describedby"));
        Assert.False(string.IsNullOrEmpty(cut.Find("desc").Id));
    }

    [Fact]
    public void HoverDetailHasTooltipSemanticsAndUniqueId()
    {
        var first = Render<FlareChart>(p => p.Add(x => x.Data, Data));
        var second = Render<FlareChart>(p => p.Add(x => x.Data, Data));
        foreach (var cut in new[] { first, second })
            cut.Find("svg rect[fill='transparent']").MouseEnter(new MouseEventArgs { ClientX = 100, ClientY = 100 });
        Assert.False(string.IsNullOrEmpty(first.Find("[role='tooltip']").Id));
        Assert.NotEqual(first.Find("[role='tooltip']").Id, second.Find("[role='tooltip']").Id);
    }
}
