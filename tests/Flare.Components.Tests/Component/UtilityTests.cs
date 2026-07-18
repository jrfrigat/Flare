using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests.Component;

// ------------------------------------------------------------------------------
// FlareClipboard  (8 tests from Wave6)
// ------------------------------------------------------------------------------

public class C_FlareClipboardTests : FlareTestContext
{
    [Fact]
    public void RendersRootButton()
    {
        var cut = Render<FlareClipboard>(p => p
            .Add(x => x.Text, "hello"));

        Assert.NotEmpty(cut.FindAll("button.flare-clipboard"));
    }

    [Fact]
    public void RendersDefaultCopyIcon_WhenNoChildContent()
    {
        var cut = Render<FlareClipboard>(p => p
            .Add(x => x.Text, "hello"));

        // The default copy icon is now the built-in SVG (no Material Symbols font dependency).
        Assert.NotEmpty(cut.FindAll("svg path"));
    }

    [Fact]
    public void RendersChildContent_WhenProvided()
    {
        var cut = Render<FlareClipboard>(p => p
            .Add(x => x.Text, "copy me")
            .AddChildContent("<span id=\"copy-label\">Copy</span>"));

        Assert.NotEmpty(cut.FindAll("#copy-label"));
    }

    [Fact]
    public void InitialState_NotCopied_NoCheckedClass()
    {
        var cut = Render<FlareClipboard>(p => p
            .Add(x => x.Text, "hello"));

        Assert.Empty(cut.FindAll(".flare-clipboard--copied"));
    }

    [Fact]
    public void TextParam_IsRequired_RendersWithValue()
    {
        var cut = Render<FlareClipboard>(p => p
            .Add(x => x.Text, "some content"));

        Assert.NotEmpty(cut.FindAll("button.flare-clipboard"));
    }

    [Fact]
    public void ButtonHasTypeButton()
    {
        var cut = Render<FlareClipboard>(p => p
            .Add(x => x.Text, "value"));

        Assert.Equal("button", cut.Find("button").GetAttribute("type"));
    }

    [Fact]
    public void AdditionalAttributes_AppliedToButton()
    {
        var cut = Render<FlareClipboard>(p => p
            .Add(x => x.Text, "data")
            .AddUnmatched("data-testid", "clipboard-btn"));

        Assert.Equal("clipboard-btn", cut.Find("button").GetAttribute("data-testid"));
    }

    [Fact]
    public void FeedbackContent_NotRenderedInitially()
    {
        var cut = Render<FlareClipboard>(p => p
            .Add(x => x.Text, "hello")
            .Add(x => x.FeedbackContent, (RenderFragment)(b =>
            {
                b.OpenElement(0, "span");
                b.AddAttribute(1, "id", "feedback-content");
                b.CloseElement();
            })));

        Assert.Empty(cut.FindAll("#feedback-content"));
    }

    [Fact]
    public async Task OnCopied_IsNotHeldBackByTheFeedbackAnimation()
    {
        // It used to be raised AFTER the confirmation delay, so a caller learned the copy had succeeded a
        // full two seconds late. A long delay here means the test only passes if OnCopied runs before it.
        var copied = false;
        var cut = Render<FlareClipboard>(p => p
            .Add(x => x.Text, "hello")
            .Add(x => x.FeedbackDurationMs, 30_000)
            .Add(x => x.OnCopied, EventCallback.Factory.Create(this, () => copied = true)));

        _ = cut.Find("button.flare-clipboard").ClickAsync(new MouseEventArgs());

        // Let the copy + callback run, but nowhere near the 30s confirmation.
        await Task.Delay(200);
        Assert.True(copied);
    }

    [Fact]
    public void DisabledAndLoading_ReachTheInnerButton()
    {
        var cut = Render<FlareClipboard>(p => p
            .Add(x => x.Text, "hello")
            .Add(x => x.Disabled, true)
            .Add(x => x.Loading, true));

        var button = cut.Find("button.flare-clipboard");
        Assert.True(button.HasAttribute("disabled"));
        Assert.Contains("flare-btn--loading", button.ClassList);
    }
}

// ------------------------------------------------------------------------------
// FlareShortcuts  (3 tests from Wave6)
// ------------------------------------------------------------------------------

public class C_FlareShortcutsTests : FlareTestContext
{
    [Fact]
    public void RendersWithoutError()
    {
        var cut = Render<FlareShortcuts>();

        Assert.NotNull(cut.Instance);
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareShortcuts>(p => p
            .AddChildContent("<span id=\"shortcut-child\">Help</span>"));

        Assert.NotEmpty(cut.FindAll("#shortcut-child"));
    }

    [Fact]
    public void RendersMultipleChildren()
    {
        var cut = Render<FlareShortcuts>(p => p
            .AddChildContent("<p id=\"a\">A</p><p id=\"b\">B</p>"));

        Assert.NotEmpty(cut.FindAll("#a"));
        Assert.NotEmpty(cut.FindAll("#b"));
    }
}

// ------------------------------------------------------------------------------
// FlareScrollTop  (6 tests from Wave6)
// ------------------------------------------------------------------------------

public class C_FlareScrollTopTests : FlareTestContext
{
    [Fact]
    public void RendersButtonElement()
    {
        var cut = Render<FlareScrollTop>();

        Assert.NotEmpty(cut.FindAll("button.flare-scroll-top"));
    }

    [Fact]
    public void ButtonHasAriaLabel()
    {
        var cut = Render<FlareScrollTop>();

        Assert.Equal("Back to top", cut.Find("button").GetAttribute("aria-label"));
    }

    [Fact]
    public void ButtonHasTypeButton()
    {
        var cut = Render<FlareScrollTop>();

        Assert.Equal("button", cut.Find("button").GetAttribute("type"));
    }

    [Fact]
    public void NotVisibleByDefault_NoVisibleClass()
    {
        var cut = Render<FlareScrollTop>();

        Assert.DoesNotContain("flare-scroll-top--visible", cut.Find("button").ClassName ?? "");
    }

    [Fact]
    public void RendersDefaultArrowIcon()
    {
        var cut = Render<FlareScrollTop>();

        // The default arrow is now the built-in SVG (no Material Symbols font dependency).
        Assert.NotEmpty(cut.FindAll("svg path"));
    }

    [Fact]
    public void ThresholdParam_AcceptsCustomValue()
    {
        var cut = Render<FlareScrollTop>(p => p
            .Add(x => x.Threshold, 500));

        Assert.Equal(500, cut.Instance.Threshold);
    }
}

// ------------------------------------------------------------------------------
// FlareFloatingActionButton + FlareFloatingActionMenu (FAB speed-dial)  (7 tests)
// ------------------------------------------------------------------------------

public class C_FlareFabMenuTests : FlareTestContext
{
    private IRenderedComponent<FlareFloatingActionButton> RenderFabMenu() =>
        Render<FlareFloatingActionButton>(p => p
            .Add(x => x.AriaLabel, "Actions")
            .Add(x => x.Position, FabPosition.Static)
            .AddChildContent<FlareFloatingActionMenu>(menu => menu
                .Add(m => m.Direction, FabMenuDirection.Up)
                .AddChildContent<FlareFloatingActionMenuItem>(item => item
                    .Add(i => i.Icon, FlareIcons.Edit)
                    .Add(i => i.Label, "Edit"))));

    [Fact]
    public void PlainFab_NoMenu_RendersFabWithoutWrapper()
    {
        var cut = Render<FlareFloatingActionButton>(p => p
            .Add(x => x.Position, FabPosition.Static)
            .Add(x => x.AriaLabel, "Add"));

        Assert.NotEmpty(cut.FindAll("button.flare-fab"));
        Assert.Empty(cut.FindAll(".flare-fab-menu"));
    }

    [Fact]
    public void MenuMode_RendersWrapperAndTriggerFab()
    {
        var cut = RenderFabMenu();

        Assert.NotEmpty(cut.FindAll(".flare-fab-menu"));
        Assert.NotEmpty(cut.FindAll(".flare-fab-menu > button.flare-fab"));
    }

    [Fact]
    public void MenuMode_RendersMenuList()
    {
        var cut = RenderFabMenu();

        Assert.NotEmpty(cut.FindAll(".flare-fab-menu__list"));
    }

    [Fact]
    public void RendersActionItem_AsSmallFab()
    {
        var cut = RenderFabMenu();

        Assert.NotEmpty(cut.FindAll(".flare-fab-menu__item"));
        Assert.NotEmpty(cut.FindAll("button.flare-fab-menu__btn.flare-fab--sm"));
    }

    [Fact]
    public void ClosedByDefault_NoOpenClass()
    {
        var cut = RenderFabMenu();

        Assert.DoesNotContain("flare-fab-menu--open", cut.Find(".flare-fab-menu").ClassName ?? "");
        Assert.DoesNotContain("flare-fab-menu__list--open", cut.Find(".flare-fab-menu__list").ClassName ?? "");
    }

    [Fact]
    public void ClickTrigger_OpensMenu()
    {
        var cut = RenderFabMenu();

        cut.Find(".flare-fab-menu > button.flare-fab").Click();

        Assert.Contains("flare-fab-menu--open", cut.Find(".flare-fab-menu").ClassName ?? "");
        Assert.Contains("flare-fab-menu__list--open", cut.Find(".flare-fab-menu__list").ClassName ?? "");
    }

    [Fact]
    public void Trigger_HasAriaExpanded()
    {
        var cut = RenderFabMenu();

        var trigger = cut.Find(".flare-fab-menu > button.flare-fab");
        Assert.Equal("false", trigger.GetAttribute("aria-expanded"));

        trigger.Click();
        Assert.Equal("true", cut.Find(".flare-fab-menu > button.flare-fab").GetAttribute("aria-expanded"));
    }
}

// ------------------------------------------------------------------------------
// FlareToggleGroup  (8 tests from Wave7)
// ------------------------------------------------------------------------------

public class C_FlareToggleGroupTests : FlareTestContext
{
    [Fact]
    public void RendersRootDiv()
    {
        var cut = Render<FlareToggleGroup<string>>();

        Assert.NotEmpty(cut.FindAll(".flare-togglegroup"));
    }

    [Fact]
    public void Horizontal_Default_NoVerticalClass()
    {
        var cut = Render<FlareToggleGroup<string>>(p => p
            .Add(x => x.Orientation, "horizontal"));

        Assert.DoesNotContain("flare-togglegroup--vertical", cut.Find("div").ClassName ?? "");
    }

    [Fact]
    public void Vertical_Orientation_AddsVerticalClass()
    {
        var cut = Render<FlareToggleGroup<string>>(p => p
            .Add(x => x.Orientation, "vertical"));

        Assert.Contains("flare-togglegroup--vertical", cut.Find("div").ClassName ?? "");
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareToggleGroup<string>>(p => p
            .AddChildContent("<span id=\"toggle-child\">Item</span>"));

        Assert.NotEmpty(cut.FindAll("#toggle-child"));
    }

    [Fact]
    public void MultiSelect_False_IsDefault()
    {
        var cut = Render<FlareToggleGroup<string>>();

        Assert.False(cut.Instance.MultiSelect);
    }

    [Fact]
    public void MultiSelect_True_AcceptedWithoutError()
    {
        var cut = Render<FlareToggleGroup<string>>(p => p
            .Add(x => x.MultiSelect, true));

        Assert.True(cut.Instance.MultiSelect);
    }

    [Fact]
    public void ChildToggleButton_RendersInsideGroup()
    {
        var cut = Render<FlareToggleGroup<string>>(p => p
            .AddChildContent<FlareToggleButton>(bp => bp
                .Add(x => x.Value, "A")));

        Assert.NotEmpty(cut.FindAll("button.flare-toggle-btn"));
    }

    [Fact]
    public void AdditionalAttributes_PassThrough()
    {
        var cut = Render<FlareToggleGroup<string>>(p => p
            .AddUnmatched("data-testid", "toggle-group"));

        Assert.Equal("toggle-group", cut.Find(".flare-togglegroup").GetAttribute("data-testid"));
    }
}

// ------------------------------------------------------------------------------
// FlareTagField  (5 tests from Wave3)
// ------------------------------------------------------------------------------

public class C_FlareTagFieldTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareTagField<string>>();

        Assert.NotEmpty(cut.FindAll(".flare-tag-input"));
    }

    [Fact]
    public void RendersInputField()
    {
        var cut = Render<FlareTagField<string>>();

        Assert.NotEmpty(cut.FindAll("input.flare-tag-input__input"));
    }

    [Fact]
    public void PlaceholderRendered()
    {
        var cut = Render<FlareTagField<string>>(p => p
            .Add(x => x.Placeholder, "Add a tag..."));

        Assert.Equal("Add a tag...", cut.Find("input.flare-tag-input__input").GetAttribute("placeholder"));
    }

    [Fact]
    public void DisabledState_HidesInput()
    {
        var cut = Render<FlareTagField<string>>(p => p
            .Add(x => x.Disabled, true));

        Assert.Empty(cut.FindAll("input.flare-tag-input__input"));
    }

    [Fact]
    public void RendersExistingTags()
    {
        var cut = Render<FlareTagField<string>>(p => p
            .Add(x => x.Values, (IReadOnlyList<string>)["Alpha", "Beta"]));

        var chips = cut.FindAll(".flare-multiselect__chip");   // shared FlareChipStrip renders the unified chip class
        Assert.Equal(2, chips.Count);
    }
}

// ------------------------------------------------------------------------------
// FlareSlider  (5 tests from Wave3)
// ------------------------------------------------------------------------------

public class C_FlareSliderTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareSlider>();

        Assert.NotEmpty(cut.FindAll(".flare-slider"));
    }

    [Fact]
    public void RendersRangeInput()
    {
        var cut = Render<FlareSlider>();

        var input = cut.Find("input[type='range']");
        Assert.NotNull(input);
    }

    [Fact]
    public void DisabledState_InputHasDisabledAttribute()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Disabled, true));

        Assert.True(cut.Find("input[type='range']").HasAttribute("disabled"));
    }

    [Fact]
    public void MinMaxAttributes_AppliedToInput()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, 10.0)
            .Add(x => x.Max, 200.0));

        var input = cut.Find("input[type='range']");
        Assert.Equal("10", input.GetAttribute("min"));
        Assert.Equal("200", input.GetAttribute("max"));
    }

    [Fact]
    public void RendersLabel()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Label, "Volume"));

        Assert.Contains("Volume", cut.Find(".flare-slider__label").TextContent);
    }

    [Theory]
    [InlineData(TrackSize.Xs, "flare-slider--xs")]
    [InlineData(TrackSize.Sm, "flare-slider--sm")]
    [InlineData(TrackSize.Md, "flare-slider--md")]
    [InlineData(TrackSize.Lg, "flare-slider--lg")]
    [InlineData(TrackSize.Xl, "flare-slider--xl")]
    public void Size_AppliesSizeClass(TrackSize size, string expected)
    {
        var cut = Render<FlareSlider>(p => p.Add(x => x.Size, size));

        Assert.Contains(expected, cut.Find(".flare-slider").ClassName);
    }

    [Fact]
    public void StartAndEndIcons_Rendered()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.StartIcon, FlareIcons.VolumeOff)
            .Add(x => x.EndIcon, FlareIcons.VolumeUp));

        Assert.NotEmpty(cut.FindAll(".flare-slider__icon--start"));
        Assert.NotEmpty(cut.FindAll(".flare-slider__icon--end"));
    }

    [Fact]
    public void Stepper_RendersStopIndicators()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, 0.0).Add(x => x.Max, 4.0).Add(x => x.Step, 1.0)
            .Add(x => x.Stepper, true));

        // 0,1,2,3,4 -> 5 stops
        Assert.Equal(5, cut.FindAll(".flare-slider__tick").Count);
    }

    [Fact]
    public void Stepper_ActiveStops_MarkedBelowValue()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, 0.0).Add(x => x.Max, 4.0).Add(x => x.Step, 1.0)
            .Add(x => x.Value, 2.0).Add(x => x.Stepper, true));

        // values 0,1,2 are <= 2 -> 3 active ticks
        Assert.Equal(3, cut.FindAll(".flare-slider__tick--active").Count);
    }

    [Fact]
    public void Indicator_RendersValueBubble()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Indicator, true).Add(x => x.Value, 42.0));

        Assert.Contains("42", cut.Find(".flare-slider__bubble").TextContent);
    }

    [Fact]
    public void Stepper_HugeStepCount_SkipsRendering()
    {
        // range/step = 100000 > MaxStops cap -> no ticks rendered
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, 0.0).Add(x => x.Max, 100000.0).Add(x => x.Step, 1.0)
            .Add(x => x.Stepper, true));

        Assert.Empty(cut.FindAll(".flare-slider__tick"));
    }

    [Fact]
    public void RendersRailSegments_WithActiveFill()
    {
        // Init=Min (default), Value mid -> inactive|active|inactive split, >=1 active
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, 0.0).Add(x => x.Max, 100.0).Add(x => x.Value, 50.0));

        Assert.NotEmpty(cut.FindAll(".flare-slider__seg"));
        Assert.NotEmpty(cut.FindAll(".flare-slider__seg--active"));
    }

    [Fact]
    public void Vertical_AppliesClassAndAriaOrientation()
    {
        var cut = Render<FlareSlider>(p => p.Add(x => x.Vertical, true));

        Assert.Contains("flare-slider--vertical", cut.Find(".flare-slider").ClassName);
        Assert.Equal("vertical", cut.Find("input[type='range']").GetAttribute("aria-orientation"));
    }

    [Fact]
    public void Horizontal_NoAriaOrientation()
    {
        var cut = Render<FlareSlider>();

        Assert.False(cut.Find("input[type='range']").HasAttribute("aria-orientation"));
    }

    [Fact]
    public void Color_Role_AppliesColorClass()
    {
        var cut = Render<FlareSlider>(p => p.Add(x => x.Color, FlareColor.Secondary));

        Assert.Contains("flare-color-secondary", cut.Find(".flare-slider").ClassName);
    }

    [Fact]
    public void Color_Custom_InlinesAccentToken()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Color, FlareColor.Custom("#E91E63")));

        Assert.Contains("--fc-main:#E91E63", cut.Find(".flare-slider").GetAttribute("style"));
    }

    [Fact]
    public void Range_RendersTwoInputsAndRangeClass()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Range, true)
            .Add(x => x.Value, 20.0).Add(x => x.ValueEnd, 80.0));

        Assert.Contains("flare-slider--range", cut.Find(".flare-slider").ClassName);
        Assert.Equal(2, cut.FindAll("input[type='range']").Count);
        Assert.NotEmpty(cut.FindAll(".flare-slider__input--low"));
        Assert.NotEmpty(cut.FindAll(".flare-slider__input--high"));
    }

    [Fact]
    public void Range_SetsBothPctVars()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Range, true)
            .Add(x => x.Min, 0.0).Add(x => x.Max, 100.0)
            .Add(x => x.Value, 20.0).Add(x => x.ValueEnd, 80.0));

        var style = cut.Find(".flare-slider__track-wrap").GetAttribute("style")!;
        Assert.Contains("--_pct:20.00%", style);
        Assert.Contains("--_pct-end:80.00%", style);
    }

    [Fact]
    public void Range_InvertedValues_OrderedForDisplay()
    {
        // Value > ValueEnd -> the rendered low/high pct never invert
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Range, true)
            .Add(x => x.Min, 0.0).Add(x => x.Max, 100.0)
            .Add(x => x.Value, 80.0).Add(x => x.ValueEnd, 20.0));

        var style = cut.Find(".flare-slider__track-wrap").GetAttribute("style")!;
        Assert.Contains("--_pct:20.00%", style);
        Assert.Contains("--_pct-end:80.00%", style);
    }

    [Fact]
    public void Range_ShowValue_RendersLowHigh()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Range, true).Add(x => x.Label, "Band")
            .Add(x => x.Value, 20.0).Add(x => x.ValueEnd, 80.0));

        Assert.Contains("20", cut.Find(".flare-slider__value").TextContent);
        Assert.Contains("80", cut.Find(".flare-slider__value").TextContent);
    }

    [Fact]
    public void InteriorInit_AddsAnchorGap_ThreeSegments()
    {
        // Init interior (0) between Min=-100/Max=100 with Value=-50 -> notch at handle AND
        // at the anchor -> three rail segments.
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, -100.0).Add(x => x.Max, 100.0)
            .Add(x => x.Init, 0.0).Add(x => x.Value, -50.0));

        Assert.Equal(3, cut.FindAll(".flare-slider__seg").Count);
    }

    [Fact]
    public void Init_DefaultsToMin_FillStartsAtZero()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, 0.0).Add(x => x.Max, 100.0).Add(x => x.Value, 50.0));

        var wrap = cut.Find(".flare-slider__track-wrap");
        Assert.Contains("--_init:0.00%", wrap.GetAttribute("style"));
    }

    [Fact]
    public void Init_SetsAnchorPercent()
    {
        // Min=-100, Max=100, Init=0 -> init anchor at 50%
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, -100.0).Add(x => x.Max, 100.0)
            .Add(x => x.Init, 0.0).Add(x => x.Value, -50.0));

        var wrap = cut.Find(".flare-slider__track-wrap");
        var style = wrap.GetAttribute("style")!;
        Assert.Contains("--_init:50.00%", style);
        Assert.Contains("--_pct:25.00%", style);   // value -50 -> 25%
    }

    [Fact]
    public void Init_ActiveStops_SpanInitToValue()
    {
        // Min=-100,Max=100,Init=0,Value=-50,Step=50 -> stops at -100,-50,0,50,100
        // active span [-50, 0] -> stops -50 and 0 active = 2
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, -100.0).Add(x => x.Max, 100.0)
            .Add(x => x.Init, 0.0).Add(x => x.Value, -50.0)
            .Add(x => x.Step, 50.0).Add(x => x.Stepper, true));

        Assert.Equal(2, cut.FindAll(".flare-slider__tick--active").Count);
    }

    [Fact]
    public void MouseWheel_ScrollUp_IncrementsByStep()
    {
        double? changed = null;
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.MouseWheel, true)
            .Add(x => x.Min, 0.0).Add(x => x.Max, 100.0).Add(x => x.Step, 5.0)
            .Add(x => x.Value, 50.0)
            .Add(x => x.ValueChanged, (double v) => changed = v));

        cut.Find(".flare-slider__track-area").TriggerEvent("onwheel", new WheelEventArgs { DeltaY = -1 });

        Assert.Equal(55.0, changed);
    }

    [Fact]
    public void MouseWheel_ScrollDown_DecrementsByStep()
    {
        double? changed = null;
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.MouseWheel, true)
            .Add(x => x.Min, 0.0).Add(x => x.Max, 100.0).Add(x => x.Step, 5.0)
            .Add(x => x.Value, 50.0)
            .Add(x => x.ValueChanged, (double v) => changed = v));

        cut.Find(".flare-slider__track-area").TriggerEvent("onwheel", new WheelEventArgs { DeltaY = 1 });

        Assert.Equal(45.0, changed);
    }

    [Fact]
    public void MouseWheel_ClampsAtMax_NoCallbackWhenAlreadyAtBound()
    {
        var invoked = false;
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.MouseWheel, true)
            .Add(x => x.Min, 0.0).Add(x => x.Max, 100.0).Add(x => x.Step, 5.0)
            .Add(x => x.Value, 100.0)
            .Add(x => x.ValueChanged, (double _) => invoked = true));

        cut.Find(".flare-slider__track-area").TriggerEvent("onwheel", new WheelEventArgs { DeltaY = -1 });

        Assert.False(invoked);
    }

    [Fact]
    public void MouseWheel_Disabled_DoesNothing()
    {
        var invoked = false;
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.MouseWheel, true).Add(x => x.Disabled, true)
            .Add(x => x.Value, 50.0)
            .Add(x => x.ValueChanged, (double _) => invoked = true));

        cut.Find(".flare-slider__track-area").TriggerEvent("onwheel", new WheelEventArgs { DeltaY = -1 });

        Assert.False(invoked);
    }

    [Fact]
    public void MouseWheel_Off_DoesNothing()
    {
        var invoked = false;
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Value, 50.0)
            .Add(x => x.ValueChanged, (double _) => invoked = true));

        cut.Find(".flare-slider__track-area").TriggerEvent("onwheel", new WheelEventArgs { DeltaY = -1 });

        Assert.False(invoked);
    }

    [Fact]
    public void MouseWheel_Range_PlainWheel_MovesLowHandleOnly()
    {
        double? low = null, high = null;
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.MouseWheel, true).Add(x => x.Range, true)
            .Add(x => x.Min, 0.0).Add(x => x.Max, 100.0).Add(x => x.Step, 5.0)
            .Add(x => x.Value, 20.0).Add(x => x.ValueEnd, 60.0)
            .Add(x => x.ValueChanged, (double v) => low = v)
            .Add(x => x.ValueEndChanged, (double v) => high = v));

        cut.Find(".flare-slider__track-area").TriggerEvent("onwheel", new WheelEventArgs { DeltaY = -1 });

        Assert.Equal(25.0, low);   // low handle moved
        Assert.Null(high);         // high handle untouched
    }

    [Fact]
    public void MouseWheel_Range_CtrlWheel_MovesHighHandleOnly()
    {
        double? low = null, high = null;
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.MouseWheel, true).Add(x => x.Range, true)
            .Add(x => x.Min, 0.0).Add(x => x.Max, 100.0).Add(x => x.Step, 5.0)
            .Add(x => x.Value, 20.0).Add(x => x.ValueEnd, 60.0)
            .Add(x => x.ValueChanged, (double v) => low = v)
            .Add(x => x.ValueEndChanged, (double v) => high = v));

        cut.Find(".flare-slider__track-area").TriggerEvent("onwheel", new WheelEventArgs { DeltaY = -1, CtrlKey = true });

        Assert.Equal(65.0, high);  // high handle moved
        Assert.Null(low);          // low handle untouched
    }

    [Fact]
    public void MouseWheel_Range_PlainWheel_LowCannotPassHigh()
    {
        var invoked = false;
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.MouseWheel, true).Add(x => x.Range, true)
            .Add(x => x.Min, 0.0).Add(x => x.Max, 100.0).Add(x => x.Step, 5.0)
            .Add(x => x.Value, 60.0).Add(x => x.ValueEnd, 60.0)   // low already at high
            .Add(x => x.ValueChanged, (double _) => invoked = true));

        cut.Find(".flare-slider__track-area").TriggerEvent("onwheel", new WheelEventArgs { DeltaY = -1 });

        Assert.False(invoked);
    }

    [Fact]
    public void MouseWheel_Range_CtrlWheel_HighCannotDropBelowLow()
    {
        var invoked = false;
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.MouseWheel, true).Add(x => x.Range, true)
            .Add(x => x.Min, 0.0).Add(x => x.Max, 100.0).Add(x => x.Step, 5.0)
            .Add(x => x.Value, 60.0).Add(x => x.ValueEnd, 60.0)   // high already at low
            .Add(x => x.ValueEndChanged, (double _) => invoked = true));

        cut.Find(".flare-slider__track-area").TriggerEvent("onwheel", new WheelEventArgs { DeltaY = 1, CtrlKey = true });

        Assert.False(invoked);
    }
}

// ------------------------------------------------------------------------------
// FlareList  (6 tests from Wave5)
// ------------------------------------------------------------------------------

public class C_FlareListTests : FlareTestContext
{
    [Fact]
    public void RendersRootUl()
    {
        var cut = Render<FlareList<object>>();

        Assert.NotEmpty(cut.FindAll("ul.flare-list"));
    }

    [Fact]
    public void HasRoleList()
    {
        var cut = Render<FlareList<object>>();

        Assert.Equal("list", cut.Find("ul.flare-list").GetAttribute("role"));
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareList<object>>(p => p
            .AddChildContent("<li id=\"custom-li\">Item</li>"));

        Assert.NotEmpty(cut.FindAll("#custom-li"));
    }

    [Fact]
    public void Dense_HasDenseClass()
    {
        var cut = Render<FlareList<object>>(p => p
            .Add(x => x.Dense, true));

        Assert.NotEmpty(cut.FindAll(".flare-list--dense"));
    }

    [Fact]
    public void NotDense_NoDenseClass()
    {
        var cut = Render<FlareList<object>>(p => p
            .Add(x => x.Dense, false));

        Assert.Empty(cut.FindAll(".flare-list--dense"));
    }

    [Fact]
    public void RendersListItems()
    {
        var cut = Render<FlareList<object>>(p => p
            .AddChildContent<FlareListItem>(li =>
                li.Add(x => x.Primary, "First Item")));

        Assert.NotEmpty(cut.FindAll(".flare-list-item"));
    }
}

// ------------------------------------------------------------------------------
// FlareListItem  (8 tests from Wave5)
// ------------------------------------------------------------------------------

public class C_FlareListItemTests : FlareTestContext
{
    [Fact]
    public void RendersLiElement()
    {
        var cut = Render<FlareListItem>();

        Assert.NotEmpty(cut.FindAll("li.flare-list-item"));
    }

    [Fact]
    public void RendersPrimaryText()
    {
        var cut = Render<FlareListItem>(p => p
            .Add(x => x.Primary, "Primary Label"));

        Assert.Contains("Primary Label", cut.Markup);
        Assert.NotEmpty(cut.FindAll(".flare-list-item__primary"));
    }

    [Fact]
    public void RendersSecondaryText()
    {
        var cut = Render<FlareListItem>(p => p
            .Add(x => x.Primary, "Title")
            .Add(x => x.Secondary, "Subtitle text"));

        Assert.Contains("Subtitle text", cut.Markup);
        Assert.NotEmpty(cut.FindAll(".flare-list-item__secondary"));
    }

    [Fact]
    public void NoPrimary_NoPrimarySpan()
    {
        var cut = Render<FlareListItem>();

        Assert.Empty(cut.FindAll(".flare-list-item__primary"));
    }

    [Fact]
    public void Disabled_HasDisabledClass()
    {
        var cut = Render<FlareListItem>(p => p
            .Add(x => x.Disabled, true));

        Assert.NotEmpty(cut.FindAll(".flare-list-item--disabled"));
    }

    [Fact]
    public void Selected_HasSelectedClass()
    {
        var cut = Render<FlareListItem>(p => p
            .Add(x => x.Selected, true));

        Assert.NotEmpty(cut.FindAll(".flare-list-item--selected"));
    }

    [Fact]
    public void NotSelected_NoSelectedClass()
    {
        var cut = Render<FlareListItem>(p => p
            .Add(x => x.Selected, false));

        Assert.Empty(cut.FindAll(".flare-list-item--selected"));
    }

    [Fact]
    public void WithClickHandler_HasClickableClass()
    {
        var cut = Render<FlareListItem>(p => p
            .Add(x => x.OnClick, EventCallback.Factory.Create(this, () => { })));

        Assert.NotEmpty(cut.FindAll(".flare-list-item--clickable"));
    }
}
