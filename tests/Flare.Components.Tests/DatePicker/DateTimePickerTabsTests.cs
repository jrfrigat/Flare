namespace Flare.Components.Tests;

/// <summary>
/// TASK-166: the Date / Time switch of the tabbed popup is an APG tab list - roles, the selected state, the panel
/// it controls, one tab in the Tab order, and arrows / Home / End to switch.
/// </summary>
public class DateTimePickerTabsTests : FlareTestContext
{
    private IRenderedComponent<FlareDateTimePicker> OpenTabs()
    {
        var cut = Render<FlareDateTimePicker>(p => p.Add(x => x.Mode, DateTimeVariant.Tabs));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        return cut;
    }

    private static IReadOnlyList<AngleSharp.Dom.IElement> Tabs(IRenderedComponent<FlareDateTimePicker> cut) =>
        cut.FindAll("[role=tab]");

    [Fact]
    public void Tabs_HaveRoles_SelectedState_AndThePanelTheyControl()
    {
        var cut = OpenTabs();

        var list = cut.Find("[role=tablist]");
        Assert.False(string.IsNullOrEmpty(list.GetAttribute("aria-label")));
        var tabs = Tabs(cut);
        Assert.Equal(2, tabs.Count);
        Assert.Equal(new[] { "true", "false" }, tabs.Select(t => t.GetAttribute("aria-selected")));
        Assert.Equal(new[] { "0", "-1" }, tabs.Select(t => t.GetAttribute("tabindex")));

        var panel = cut.Find("[role=tabpanel]");
        Assert.All(tabs, t => Assert.Equal(panel.Id, t.GetAttribute("aria-controls")));
        Assert.Equal(tabs[0].Id, panel.GetAttribute("aria-labelledby"));
        Assert.NotEmpty(panel.QuerySelectorAll("button[role=gridcell]"));
    }

    [Theory]
    [InlineData("ArrowRight")]
    [InlineData("ArrowLeft")]
    [InlineData("End")]
    public void Keys_SwitchToTheTimeTab(string key)
    {
        var cut = OpenTabs();

        cut.Find("[role=tablist]").KeyDown(key);

        var tabs = Tabs(cut);
        Assert.Equal(new[] { "false", "true" }, tabs.Select(t => t.GetAttribute("aria-selected")));
        Assert.Equal(new[] { "-1", "0" }, tabs.Select(t => t.GetAttribute("tabindex")));
        var panel = cut.Find("[role=tabpanel]");
        Assert.Equal(tabs[1].Id, panel.GetAttribute("aria-labelledby"));
        Assert.NotEmpty(panel.QuerySelectorAll($".{Css.Classes.DateTimePicker.TimeInput}"));
    }

    [Fact]
    public void Home_ReturnsToTheDateTab()
    {
        var cut = OpenTabs();
        Tabs(cut)[1].Click();

        cut.Find("[role=tablist]").KeyDown("Home");

        Assert.Equal("true", Tabs(cut)[0].GetAttribute("aria-selected"));
    }

    [Fact]
    public void Panels_HaveNoTabs()
    {
        var cut = Render<FlareDateTimePicker>(p => p.Add(x => x.Mode, DateTimeVariant.Panels));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        Assert.Empty(cut.FindAll("[role=tablist]"));
    }
}
