namespace Flare.Components.Tests;

public class TimeFocusRenderTests : FlareTestContext
{
    [Theory]
    [InlineData(TimePickerVariant.Dial, false)]
    [InlineData(TimePickerVariant.Dial, true)]
    [InlineData(TimePickerVariant.Dropdown, false)]
    [InlineData(TimePickerVariant.Dropdown, true)]
    [InlineData(TimePickerVariant.List, false)]
    [InlineData(TimePickerVariant.List, true)]
    public async Task InputFocus_DoesNotRenderTheFieldOrPopup(TimePickerVariant variant, bool open)
    {
        var cut = Render<ObservedTime>(p => p.Add(x => x.PopupVariant, variant));
        if (open) await cut.InvokeAsync(cut.Instance.OpenAsync);
        var before = cut.Instance.Rendered;
        var markup = cut.Markup;
        cut.Find("input").Focus();
        Assert.Equal(markup, cut.Markup);
        Assert.Equal(before, cut.Instance.Rendered);
    }

    [Fact]
    public void InputFocus_StillDefersDisplayResyncUntilEditingEnds()
    {
        var cut = Render<FlareTimePicker>(p => p.Add(x => x.Use24Hour, true)
            .Add(x => x.Value, new TimeOnly(10, 30)));
        cut.Find("input").Focus();
        cut.Render(p => p.Add(x => x.ShowSeconds, true));
        Assert.Equal("10:30", cut.Find("input").GetAttribute("value"));
        cut.Find("input").Blur();
        cut.Render(p => p.Add(x => x.Label, "After editing"));
        Assert.Equal("10:30:00", cut.Find("input").GetAttribute("value"));
    }

    public class ObservedTime : FlareTimePicker
    {
        public int Rendered { get; private set; }
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            Rendered++;
            return base.OnAfterRenderAsync(firstRender);
        }
    }
}
