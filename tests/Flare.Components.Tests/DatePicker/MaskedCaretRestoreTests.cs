using System.Globalization;
using Bunit;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-122: after the mask rewrites the text, all three masked pickers put the caret back through
/// flareField.setValueAndCaret (which never moves focus) with the masked text, and none of them uses the
/// focusing selectRange helper for it.
/// </summary>
public class MaskedCaretRestoreTests : FlareTestContext
{
    private const string Module = "./_content/Flare.Components/js/flare-components.js";

    private BunitJSModuleInterop SetupCaret(int caret)
    {
        var module = JSInterop.SetupModule(Module);
        module.Setup<int[]?>("flareField.selection", _ => true).SetResult(new[] { caret, caret });
        module.SetupVoid("flareField.setValueAndCaret", _ => true);
        return module;
    }

    private static void AssertRestored(BunitJSModuleInterop module, string text, int caret)
    {
        var call = Assert.Single(module.Invocations["flareField.setValueAndCaret"]);
        Assert.Equal(text, call.Arguments[1]);
        Assert.Equal(caret, call.Arguments[2]);
        Assert.Equal(caret, call.Arguments[3]);
        Assert.Empty(module.Invocations["flareField.selectRange"]);
    }

    [Fact]
    public void DatePicker_RestoresCaretWithTheMaskedText()
    {
        var module = SetupCaret(1);
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.Culture, new CultureInfo("ru-RU"))
            .Add(x => x.Value, new DateOnly(2026, 10, 15)));

        cut.Find("input").Focus();
        cut.Find("input").Input("1.10.2026");

        AssertRestored(module, "11.02.026", 1);
    }

    [Fact]
    public void DateTimePicker_RestoresCaretWithTheMaskedText()
    {
        var module = SetupCaret(1);
        var cut = Render<FlareDateTimePicker>(p => p
            .Add(x => x.Culture, new CultureInfo("ru-RU"))
            .Add(x => x.Value, new DateTimeOffset(2026, 10, 15, 12, 30, 0, TimeSpan.FromHours(3))));

        cut.Find("input").Focus();
        cut.Find("input").Input("1.10.2026 12:30");

        AssertRestored(module, "11.02.0261 23:0", 1);
    }

    [Fact]
    public void TimePicker_RestoresCaretWithTheMaskedText()
    {
        var module = SetupCaret(1);
        var cut = Render<FlareTimePicker>(p => p.Add(x => x.Value, new TimeOnly(12, 33)));

        cut.Find("input").Focus();
        cut.Find("input").Input("1:33");

        AssertRestored(module, "13:3", 1);
    }

    [Fact]
    public void BlurredField_IsNotTouched()
    {
        var module = SetupCaret(1);
        var cut = Render<FlareTimePicker>(p => p.Add(x => x.Value, new TimeOnly(12, 33)));

        cut.Find("input").Input("1:33");

        Assert.Empty(module.Invocations["flareField.setValueAndCaret"]);
    }
}
