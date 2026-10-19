using Flare.Abstractions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class OtpReadOnlyTests : FlareTestContext
{
    private readonly List<string> _values = [];

    public OtpReadOnlyTests()
    {
        Services.AddScoped<IFlareClipboard>(_ => new StubClipboard("999999"));
    }

    private IRenderedComponent<FlareOtpField> RenderOtp(bool readOnly = true, bool disabled = false)
        => Render<FlareOtpField>(p => p
            .Add(x => x.Value, "123456")
            .Add(x => x.ReadOnly, readOnly)
            .Add(x => x.Disabled, disabled)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string>(this, _values.Add)));

    private static KeyboardEventArgs Key(string key) => new() { Key = key };

    [Fact]
    public void Delete_DoesNotChangeAReadOnlyCode()
    {
        var cut = RenderOtp();

        cut.FindAll("input")[0].KeyDown(Key("Delete"));

        Assert.Empty(_values);
        Assert.Equal("1", cut.FindAll("input")[0].GetAttribute("value"));
    }

    [Fact]
    public void Backspace_DoesNotChangeAReadOnlyCode()
    {
        var cut = RenderOtp();

        cut.FindAll("input")[0].KeyDown(Key("Backspace"));

        Assert.Empty(_values);
    }

    [Fact]
    public void Input_DoesNotChangeAReadOnlyCode()
    {
        var cut = RenderOtp();

        cut.FindAll("input")[0].Input("9");

        Assert.Empty(_values);
    }

    [Fact]
    public void Paste_DoesNotChangeAReadOnlyCode()
    {
        var cut = RenderOtp();

        cut.FindAll("input")[0].TriggerEvent("onpaste", new ClipboardEventArgs());

        Assert.Empty(_values);
    }

    [Fact]
    public void Delete_DoesNotChangeADisabledCode()
    {
        var cut = RenderOtp(readOnly: false, disabled: true);

        cut.FindAll("input")[0].KeyDown(Key("Delete"));

        Assert.Empty(_values);
    }

    [Fact]
    public void Arrows_StillMoveBetweenReadOnlyCells()
    {
        var cut = RenderOtp();

        cut.FindAll("input")[0].KeyDown(Key("ArrowRight"));

        Assert.Contains(Css.Classes.Otp.CellFocused, cut.FindAll("input")[1].ClassName ?? string.Empty);
    }

    private sealed class StubClipboard(string text) : IFlareClipboard
    {
        public ValueTask CopyAsync(string text) => ValueTask.CompletedTask;
        public ValueTask<string> ReadAsync() => new(text);
    }
}
