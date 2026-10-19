using Flare.Abstractions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class OtpInputDistributionTests : FlareTestContext
{
    private readonly List<string> _values = [];
    private int _completions;
    private readonly StubClipboard _clipboard = new();

    public OtpInputDistributionTests()
    {
        Services.AddScoped<IFlareClipboard>(_ => _clipboard);
    }

    private IRenderedComponent<FlareOtpField> RenderOtp(string value = "123456")
        => Render<FlareOtpField>(p => p
            .Add(x => x.Value, value)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string>(this, _values.Add))
            .Add(x => x.OnComplete, EventCallback.Factory.Create<string>(this, _ => _completions++)));

    private static string[] Cells(IRenderedComponent<FlareOtpField> cut)
        => cut.FindAll("input").Select(i => i.GetAttribute("value") ?? string.Empty).ToArray();

    private void Paste(IRenderedComponent<FlareOtpField> cut, int idx, string text)
    {
        _clipboard.Text = text;
        cut.FindAll("input")[idx].TriggerEvent("onpaste", new ClipboardEventArgs());
    }

    [Fact]
    public void AutofillOfAFullCode_FillsEveryCellAndCompletesOnce()
    {
        var cut = RenderOtp(value: "");

        cut.FindAll("input")[0].Input("123456");

        Assert.Equal(["1", "2", "3", "4", "5", "6"], Cells(cut));
        Assert.Equal("123456", _values[^1]);
        Assert.Equal(1, _completions);
    }

    [Fact]
    public void AutofillAtALaterCell_FillsFromThere()
    {
        var cut = RenderOtp(value: "12");

        cut.FindAll("input")[2].Input("3456");

        Assert.Equal(["1", "2", "3", "4", "5", "6"], Cells(cut));
        Assert.Equal(1, _completions);
    }

    [Fact]
    public void AutofillOfAShortCode_DropsTheOldTail()
    {
        var cut = RenderOtp();

        cut.FindAll("input")[0].Input("12");

        Assert.Equal(["1", "2", "", "", "", ""], Cells(cut));
        Assert.Equal("12", _values[^1]);
        Assert.Equal(0, _completions);
    }

    [Fact]
    public void PastingAFullCode_ReplacesTheWholeCodeAndCompletesOnce()
    {
        var cut = RenderOtp();

        Paste(cut, 0, "999999");

        Assert.Equal(["9", "9", "9", "9", "9", "9"], Cells(cut));
        Assert.Equal(1, _completions);
    }

    [Fact]
    public void PastingAShortCode_ReplacesFromTheFocusedCellAndClearsTheTail()
    {
        var cut = RenderOtp();

        Paste(cut, 2, "99");

        Assert.Equal(["1", "2", "9", "9", "", ""], Cells(cut));
        Assert.Equal("1299", _values[^1]);
        Assert.Contains(Css.Classes.Otp.CellFocused, cut.FindAll("input")[4].ClassName ?? string.Empty);
        Assert.Equal(0, _completions);
    }

    [Fact]
    public void ClipboardUnavailable_LeavesTheCodeAndTypingStillWorks()
    {
        var cut = RenderOtp();

        Paste(cut, 0, "");

        Assert.Empty(_values);
        cut.FindAll("input")[0].Input("9");
        Assert.Equal("923456", _values[^1]);
        Assert.Equal(1, _completions);
    }

    [Fact]
    public void AnEchoedValue_DoesNotCompleteAgain()
    {
        var cut = RenderOtp();
        Paste(cut, 0, "999999");

        cut.Render(p => p
            .Add(x => x.Value, "999999")
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string>(this, _values.Add))
            .Add(x => x.OnComplete, EventCallback.Factory.Create<string>(this, _ => _completions++)));

        Assert.Equal(1, _completions);
    }

    private sealed class StubClipboard : IFlareClipboard
    {
        public string Text { get; set; } = string.Empty;
        public ValueTask CopyAsync(string text) => ValueTask.CompletedTask;
        public ValueTask<string> ReadAsync() => new(Text);
    }
}
