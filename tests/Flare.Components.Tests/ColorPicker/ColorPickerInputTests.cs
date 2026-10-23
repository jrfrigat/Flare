using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

/// <summary>
/// FlareColorPicker publishes a colour only after its text input parses: the formats it accepts, the
/// clamping of each channel and the eyedropper answer are what the host actually receives.
/// </summary>
public class ColorPickerInputTests : FlareTestContext
{
    private readonly StubUi _ui = new();

    public ColorPickerInputTests()
    {
        Services.AddScoped<IUiJsService>(_ => _ui);
    }

    private IRenderedComponent<FlareColorPicker> RenderPicker(
        Action<string>? changed = null,
        string value = "#000000",
        IReadOnlyList<string>? presets = null,
        bool eyeDropper = false)
        => Render<FlareColorPicker>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.Inline, true);
            if (changed is not null)
                p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string>(this, changed));
            if (presets is not null) p.Add(x => x.Presets, presets);
            if (eyeDropper) p.Add(x => x.ShowEyeDropper, true);
        });

    private static void SwitchTab(IRenderedComponent<FlareColorPicker> cut, string tab)
        => cut.FindAll($".{Css.Classes.Colorpicker.Tab}")
            .Single(b => b.TextContent.Trim() == tab)
            .Click();

    private static void TypeHex(IRenderedComponent<FlareColorPicker> cut, string text)
        => cut.Find($".{Css.Classes.Colorpicker.Input}").Change(text);

    private static void TypeNumber(IRenderedComponent<FlareColorPicker> cut, int index, string text)
        => cut.FindAll("input[type=number]")[index].Input(text);

    [Theory]
    [InlineData("1188ee", "#1188EE")]
    [InlineData("abc", "#AABBCC")]
    [InlineData("#112233", "#112233")]
    public async Task BareShortAndFullHex_AreNormalized(string input, string expected)
    {
        string? changed = null;
        var cut = RenderPicker(v => changed = v);

        await cut.InvokeAsync(() => TypeHex(cut, input));

        Assert.Equal(expected, changed);
        Assert.Equal(expected, cut.Find($".{Css.Classes.Colorpicker.Input}").GetAttribute("value"));
    }

    [Fact]
    public async Task CssFunctions_AreAcceptedInTheHexField()
    {
        string? changed = null;
        var cut = RenderPicker(v => changed = v);

        await cut.InvokeAsync(() => TypeHex(cut, "rgb(255, 0, 0)"));

        Assert.Equal("#FF0000", changed);
    }

    [Fact]
    public async Task HslFunction_IsAcceptedInTheHexField()
    {
        string? changed = null;
        var cut = RenderPicker(v => changed = v);

        await cut.InvokeAsync(() => TypeHex(cut, "hsl(210, 100%, 50%)"));

        Assert.Equal("#0080FF", changed);
    }

    [Fact]
    public async Task InvalidText_IsNotPublished()
    {
        string? changed = null;
        var cut = RenderPicker(v => changed = v);

        await cut.InvokeAsync(() => TypeHex(cut, "#zzz"));

        Assert.Null(changed);
        Assert.Equal("#zzz", cut.Find($".{Css.Classes.Colorpicker.Input}").GetAttribute("value"));
    }

    [Fact]
    public async Task RgbChannels_AreClamped()
    {
        string? changed = null;
        var cut = RenderPicker(v => changed = v);
        await cut.InvokeAsync(() => SwitchTab(cut, "RGB"));

        await cut.InvokeAsync(() => TypeNumber(cut, 0, "300"));
        await cut.InvokeAsync(() => TypeNumber(cut, 1, "-5"));
        await cut.InvokeAsync(() => TypeNumber(cut, 2, "128"));

        Assert.Equal("#FF0080", changed);
    }

    [Fact]
    public async Task HslChannels_AreClamped()
    {
        string? changed = null;
        var cut = RenderPicker(v => changed = v);
        await cut.InvokeAsync(() => SwitchTab(cut, "HSL"));

        await cut.InvokeAsync(() => TypeNumber(cut, 0, "400"));
        await cut.InvokeAsync(() => TypeNumber(cut, 1, "150"));
        await cut.InvokeAsync(() => TypeNumber(cut, 2, "50"));

        Assert.Equal("#FF0000", changed);
    }

    [Fact]
    public async Task Alpha_IsParsedInvariantly()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var changed = new List<string>();
            var cut = RenderPicker(changed.Add, value: "#FF0000");

            await cut.InvokeAsync(() =>
                cut.Find($".{Css.Classes.Colorpicker.SliderAlphaMod}").Input("0.5"));
            Assert.Equal("#FF000080", changed[^1]);

            await cut.InvokeAsync(() =>
                cut.Find($".{Css.Classes.Colorpicker.SliderAlphaMod}").Input("opaque"));
            Assert.Single(changed);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public async Task Presets_PublishTheClickedColour()
    {
        string? changed = null;
        var cut = RenderPicker(v => changed = v, presets: new[] { "#ff0000", "#00ff00" });

        await cut.InvokeAsync(() => cut.FindAll($".{Css.Classes.Colorpicker.Preset}")[1].Click());

        Assert.Equal("#00FF00", changed);
    }

    [Fact]
    public async Task EyeDropperSuccess_AppliesAndPublishesTheColour()
    {
        _ui.EyedropperResult = "#abcdef";
        string? changed = null;
        var cut = RenderPicker(v => changed = v, eyeDropper: true);

        await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Colorpicker.EyedropperBtn}").Click());

        Assert.Equal(1, _ui.EyedropperCalls);
        Assert.Equal("#ABCDEF", changed);
    }

    [Fact]
    public async Task EyeDropperCancelled_ChangesNothing()
    {
        _ui.EyedropperResult = null;
        string? changed = null;
        var cut = RenderPicker(v => changed = v, eyeDropper: true);

        await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Colorpicker.EyedropperBtn}").Click());

        Assert.Equal(1, _ui.EyedropperCalls);
        Assert.Null(changed);
    }

    [Fact]
    public async Task EyeDropperFailure_IsSwallowed()
    {
        _ui.EyedropperError = new JSException("cancelled");
        string? changed = null;
        var cut = RenderPicker(v => changed = v, eyeDropper: true);

        await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Colorpicker.EyedropperBtn}").Click());

        Assert.Equal(1, _ui.EyedropperCalls);
        Assert.Null(changed);
    }

    private sealed class StubUi : IUiJsService
    {
        public string? EyedropperResult { get; set; }
        public Exception? EyedropperError { get; set; }
        public int EyedropperCalls { get; private set; }

        public ValueTask<bool> SupportsEyeDropperAsync() => ValueTask.FromResult(true);

        public ValueTask<string?> OpenEyeDropperAsync()
        {
            EyedropperCalls++;
            if (EyedropperError is not null) throw EyedropperError;
            return ValueTask.FromResult(EyedropperResult);
        }

        public ValueTask RegisterTabScrollerAsync<T>(ElementReference bar, DotNetObjectReference<T> dotNetRef)
            where T : class => ValueTask.CompletedTask;

        public ValueTask ScrollTabsAsync(ElementReference bar, int direction) => ValueTask.CompletedTask;

        public ValueTask RevealActiveLinkTabAsync(ElementReference bar) => ValueTask.CompletedTask;

        public ValueTask RemoveTabScrollerAsync(ElementReference bar) => ValueTask.CompletedTask;

        public ValueTask RegisterButtonGroupCollapseAsync<T>(ElementReference root, DotNetObjectReference<T> dotNetRef)
            where T : class => ValueTask.CompletedTask;

        public ValueTask ApplyButtonGroupOverflowAsync<T>(ElementReference root, DotNetObjectReference<T> dotNetRef)
            where T : class => ValueTask.CompletedTask;

        public ValueTask RemoveButtonGroupCollapseAsync(ElementReference root) => ValueTask.CompletedTask;

        public ValueTask RegisterShortcutsAsync<T>(DotNetObjectReference<T> dotNetRef) where T : class
            => ValueTask.CompletedTask;

        public ValueTask RemoveShortcutsAsync() => ValueTask.CompletedTask;

        public ValueTask SetUnloadPromptAsync(bool enabled) => ValueTask.CompletedTask;

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
