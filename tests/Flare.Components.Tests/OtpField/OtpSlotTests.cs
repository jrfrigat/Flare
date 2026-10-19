using Flare.Abstractions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class OtpSlotTests : FlareTestContext
{
    private readonly List<string> _values = [];
    private int _completions;

    private IRenderedComponent<FlareOtpField> RenderOtp()
        => Render<FlareOtpField>(p => p
            .Add(x => x.Value, "123456")
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string>(this, _values.Add))
            .Add(x => x.OnComplete, EventCallback.Factory.Create<string>(this, _ => _completions++)));

    private static string[] Cells(IRenderedComponent<FlareOtpField> cut)
        => cut.FindAll("input").Select(i => i.GetAttribute("value") ?? string.Empty).ToArray();

    private void Rerender(IRenderedComponent<FlareOtpField> cut, string value, string helper = "h")
        => cut.Render(p => p
            .Add(x => x.Value, value)
            .Add(x => x.HelperText, helper)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string>(this, _values.Add))
            .Add(x => x.OnComplete, EventCallback.Factory.Create<string>(this, _ => _completions++)));

    [Fact]
    public void DeletingAMiddleDigit_KeepsTheGapAfterTheParentRendersTheValueBack()
    {
        var cut = RenderOtp();

        cut.FindAll("input")[2].KeyDown(new KeyboardEventArgs { Key = "Delete" });
        Rerender(cut, _values[^1]);

        Assert.Equal(["1", "2", "", "4", "5", "6"], Cells(cut));
    }

    [Fact]
    public void AnUnrelatedParentRender_KeepsTheGap()
    {
        var cut = RenderOtp();

        cut.FindAll("input")[2].KeyDown(new KeyboardEventArgs { Key = "Delete" });
        Rerender(cut, _values[^1]);
        Rerender(cut, _values[^1], helper: "other");

        Assert.Equal(["1", "2", "", "4", "5", "6"], Cells(cut));
    }

    [Fact]
    public void TypingInTheGap_FillsItAndCompletesOnce()
    {
        var cut = RenderOtp();
        cut.FindAll("input")[2].KeyDown(new KeyboardEventArgs { Key = "Delete" });
        Rerender(cut, _values[^1]);

        cut.FindAll("input")[2].Input("3");

        Assert.Equal("123456", _values[^1]);
        Assert.Equal(["1", "2", "3", "4", "5", "6"], Cells(cut));
        Assert.Equal(1, _completions);
    }

    [Fact]
    public void ReplacingADigit_KeepsTheOtherCells()
    {
        var cut = RenderOtp();

        cut.FindAll("input")[1].Input("9");
        Rerender(cut, _values[^1]);

        Assert.Equal(["1", "9", "3", "4", "5", "6"], Cells(cut));
        Assert.Equal("193456", _values[^1]);
    }

    [Fact]
    public void ANewControlledValue_ResetsEveryCell()
    {
        var cut = RenderOtp();
        cut.FindAll("input")[2].KeyDown(new KeyboardEventArgs { Key = "Delete" });
        Rerender(cut, _values[^1]);

        Rerender(cut, "999999");

        Assert.Equal(["9", "9", "9", "9", "9", "9"], Cells(cut));
    }

    [Fact]
    public void NoCompletion_WhileAGapRemains()
    {
        var cut = RenderOtp();

        cut.FindAll("input")[2].KeyDown(new KeyboardEventArgs { Key = "Delete" });
        Rerender(cut, _values[^1]);

        Assert.Equal(0, _completions);
        Assert.Equal("12456", _values[^1]);
    }
}
