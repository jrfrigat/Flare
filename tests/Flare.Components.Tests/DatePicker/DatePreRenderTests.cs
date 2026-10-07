using System.Globalization;

namespace Flare.Components.Tests;

public class DatePreRenderTests : FlareTestContext
{
    [Fact]
    public async Task Default_CreatesOnlyWhileOpen()
    {
        var cut = Render<FlareDatePicker>();
        Assert.False(cut.Instance.PreRenderCalendar);
        Assert.Empty(cut.FindAll("[role=dialog]"));
        await cut.InvokeAsync(cut.Instance.OpenAsync);
        Assert.Equal(42, cut.FindAll("[role=gridcell]").Count);
        await cut.InvokeAsync(cut.Instance.CloseAsync);
        Assert.Empty(cut.FindAll("[role=dialog]"));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task PreRendered_OnlyPlacesAndRaisesEventsWhenOpened(bool fromField)
    {
        var opened = 0;
        var closed = 0;
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.PreRenderCalendar, true)
            .Add(x => x.Opened, () => opened++).Add(x => x.Closed, () => closed++));
        var grid = cut.FindComponent<FlareMonthGrid>().Instance;
        Assert.Equal("display:none", cut.Find("[role=dialog]").GetAttribute("style"));
        Assert.Equal(42, cut.FindAll("[role=gridcell]").Count);
        Assert.DoesNotContain(JSInterop.Invocations, x => x.Identifier == "positionAnchoredPanel");
        Assert.Equal(0, opened);
        Assert.Equal(0, closed);

        cut.Find(fromField ? "input" : "button[aria-haspopup=dialog]").Click();
        Assert.Null(cut.Find("[role=dialog]").GetAttribute("style"));
        Assert.Single(JSInterop.Invocations, x => x.Identifier == "positionAnchoredPanel");
        Assert.Equal(1, opened);
        await cut.InvokeAsync(cut.Instance.CloseAsync);
        Assert.Equal("display:none", cut.Find("[role=dialog]").GetAttribute("style"));
        Assert.Same(grid, cut.FindComponent<FlareMonthGrid>().Instance);
        Assert.Equal(1, closed);
        await cut.InvokeAsync(cut.Instance.OpenAsync);
        Assert.Same(grid, cut.FindComponent<FlareMonthGrid>().Instance);
        Assert.Equal(2, JSInterop.Invocations.Count(x => x.Identifier == "positionAnchoredPanel"));
        Assert.Equal(2, opened);
    }

    [Fact]
    public void ClosedCalendar_ReadsNewValueCultureAndDelegateState()
    {
        var culture = CultureInfo.GetCultureInfo("en-US");
        var unavailable = new DateOnly(2026, 10, 15);
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.PreRenderCalendar, true)
            .Add(x => x.Value, unavailable).Add(x => x.Culture, culture)
            .Add(x => x.IsDateDisabled, d => d == unavailable));
        Assert.True(cut.Find("[role=gridcell][aria-label='Thursday, October 15, 2026']").HasAttribute("disabled"));
        unavailable = unavailable.AddDays(1);
        cut.Render();
        Assert.False(cut.Find("[role=gridcell][aria-label='Thursday, October 15, 2026']").HasAttribute("disabled"));
        Assert.True(cut.Find("[role=gridcell][aria-label='Friday, October 16, 2026']").HasAttribute("disabled"));
        var next = new DateOnly(2026, 11, 15);
        var french = CultureInfo.GetCultureInfo("fr-FR");
        cut.Render(p => p.Add(x => x.Value, next).Add(x => x.Culture, french).Add(x => x.Min, next));
        Assert.NotNull(cut.Find($"[role=gridcell][aria-label='{next.ToString("D", french)}']"));
        Assert.Equal("display:none", cut.Find("[role=dialog]").GetAttribute("style"));
    }

    [Fact]
    public async Task SelectingDate_ClosesRetainedCalendarAndReopensAtBoundValue()
    {
        DateOnly? committed = null;
        var date = new DateOnly(2026, 10, 15);
        var culture = CultureInfo.GetCultureInfo("en-US");
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.PreRenderCalendar, true)
            .Add(x => x.Value, date).Add(x => x.Culture, culture)
            .Add(x => x.ValueChanged, (DateOnly? value) => committed = value));
        var grid = cut.FindComponent<FlareMonthGrid>().Instance;
        await cut.InvokeAsync(cut.Instance.OpenAsync);
        var picked = date.AddDays(1);
        cut.Find($"[role=gridcell][aria-label='{picked.ToString("D", culture)}']").Click();
        Assert.Equal(picked, committed);
        Assert.Equal("display:none", cut.Find("[role=dialog]").GetAttribute("style"));
        Assert.Same(grid, cut.FindComponent<FlareMonthGrid>().Instance);
        cut.Render(p => p.Add(x => x.Value, committed));
        await cut.InvokeAsync(cut.Instance.OpenAsync);
        Assert.Equal(picked.ToString("D", culture),
            cut.Find("[role=gridcell][aria-selected=true]").GetAttribute("aria-label"));
        Assert.Same(grid, cut.FindComponent<FlareMonthGrid>().Instance);
    }

    [Fact]
    public async Task ChangingMode_DoesNotCloseAnOpenCalendar()
    {
        var cut = Render<FlareDatePicker>();
        cut.Render(p => p.Add(x => x.PreRenderCalendar, true));
        Assert.Equal("display:none", cut.Find("[role=dialog]").GetAttribute("style"));
        await cut.InvokeAsync(cut.Instance.OpenAsync);
        cut.Render(p => p.Add(x => x.PreRenderCalendar, false));
        Assert.Single(cut.FindAll("[role=dialog]"));
        Assert.Null(cut.Find("[role=dialog]").GetAttribute("style"));
        await cut.InvokeAsync(cut.Instance.CloseAsync);
        Assert.Empty(cut.FindAll("[role=dialog]"));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task LockedField_KeepsPreRenderedCalendarHidden(bool readOnly)
    {
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.PreRenderCalendar, true)
            .Add(x => x.ReadOnly, readOnly).Add(x => x.Disabled, !readOnly));
        await cut.InvokeAsync(cut.Instance.OpenAsync);
        Assert.Equal("false", cut.Find("button[aria-haspopup=dialog]").GetAttribute("aria-expanded"));
        Assert.Equal("display:none", cut.Find("[role=dialog]").GetAttribute("style"));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task InlineCalendar_RemainsVisible(bool preRender)
    {
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.PreRenderCalendar, preRender).Add(x => x.Inline, true));
        await cut.InvokeAsync(cut.Instance.CloseAsync);
        Assert.Equal(42, cut.FindAll("[role=gridcell]").Count);
        Assert.Null(cut.Find($".{Css.Classes.DatePicker.Panel}").GetAttribute("style"));
        Assert.Empty(cut.FindAll("[role=dialog]"));
    }
}
