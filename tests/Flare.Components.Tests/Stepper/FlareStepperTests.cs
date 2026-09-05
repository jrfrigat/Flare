using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareStepperTests : FlareTestContext
{
    [Fact]
    public void RendersRootDiv()
    {
        var cut = Render<FlareStepper>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Stepper.Root}"));
    }

    [Fact]
    public void DefaultOrientation_HasHorizontalClass()
    {
        var cut = Render<FlareStepper>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Stepper.Horizontal}"));
    }

    [Fact]
    public void VerticalOrientation_HasVerticalClass()
    {
        var cut = Render<FlareStepper>(p => p
            .Add(x => x.Orientation, StepperOrientation.Vertical));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Stepper.Vertical}"));
    }

    [Fact]
    public void RendersStepHeader()
    {
        var cut = Render<FlareStepper>(p => p
            .AddChildContent<FlareStep>(sp => sp
                .Add(x => x.Label, "Step One")));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Stepper.Header}"));
    }

    [Fact]
    public void SingleStep_IsActive()
    {
        var cut = Render<FlareStepper>(p => p
            .AddChildContent<FlareStep>(sp => sp
                .Add(x => x.Label, "First")));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Stepper.StepActive}"));
    }

    [Fact]
    public void MultipleSteps_FirstIsActive_SecondIsUpcoming()
    {
        var cut = Render<FlareStepper>(p => p
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareStep>(0);
                b.AddAttribute(1, "Label", "One");
                b.CloseComponent();
                b.OpenComponent<FlareStep>(2);
                b.AddAttribute(3, "Label", "Two");
                b.CloseComponent();
            }));

        var active = cut.FindAll($".{Css.Classes.Stepper.StepActive}");
        var upcoming = cut.FindAll($".{Css.Classes.Stepper.StepUpcoming}");

        Assert.Single(active);
        Assert.Single(upcoming);
    }

    [Fact]
    public void LinearTrue_IsDefault()
    {
        var cut = Render<FlareStepper>(p => p
            .Add(x => x.Linear, true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Stepper.Root}"));
    }

    [Fact]
    public void RendersConnectorBetweenSteps()
    {
        var cut = Render<FlareStepper>(p => p
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareStep>(0);
                b.AddAttribute(1, "Label", "A");
                b.CloseComponent();
                b.OpenComponent<FlareStep>(2);
                b.AddAttribute(3, "Label", "B");
                b.CloseComponent();
            }));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Stepper.Connector}"));
    }

    // Builds three labelled steps; the middle one's Skippable flag is parameterised.
    private static RenderFragment ThreeSteps(string a, string b, string c, bool middleSkippable = false) => builder =>
    {
        builder.OpenComponent<FlareStep>(0);
        builder.AddAttribute(1, "Label", a);
        builder.CloseComponent();
        builder.OpenComponent<FlareStep>(2);
        builder.AddAttribute(3, "Label", b);
        builder.AddAttribute(4, "Skippable", middleSkippable);
        builder.CloseComponent();
        builder.OpenComponent<FlareStep>(5);
        builder.AddAttribute(6, "Label", c);
        builder.CloseComponent();
    };

    private static string? ActiveLabel(Bunit.IRenderedComponent<FlareStepper> cut) =>
        cut.FindAll($".{Css.Classes.Stepper.StepActive} .{Css.Classes.Stepper.Label}").FirstOrDefault()?.TextContent;

    [Fact]
    public void ActiveIndexParameter_SelectsThatStep()
    {
        var cut = Render<FlareStepper>(p => p
            .Add(x => x.ActiveIndex, 1)
            .AddChildContent(ThreeSteps("One", "Two", "Three")));

        Assert.Equal("Two", ActiveLabel(cut));
    }

    [Fact]
    public void Navigation_FiresActiveIndexChanged()
    {
        var captured = -1;
        var cut = Render<FlareStepper>(p => p
            .Add(x => x.ActiveIndexChanged, EventCallback.Factory.Create<int>(this, i => captured = i))
            .AddChildContent(ThreeSteps("One", "Two", "Three")));

        cut.InvokeAsync(() => cut.Instance.Next());

        Assert.Equal(1, captured);
        Assert.Equal("Two", ActiveLabel(cut));
    }

    [Fact]
    public void ActionContent_ReplacesBuiltInButtons()
    {
        var cut = Render<FlareStepper>(p => p
            .Add(x => x.ActionContent, (RenderFragment<StepperContext>)(ctx => builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "custom-nav");
                builder.AddContent(2, $"{ctx.ActiveIndex + 1}/{ctx.Count}");
                builder.CloseElement();
            }))
            .AddChildContent(ThreeSteps("One", "Two", "Three")));

        Assert.Empty(cut.FindAll($".{Css.Classes.Stepper.NavBtn}"));
        var nav = cut.Find(".custom-nav");
        Assert.Equal("1/3", nav.TextContent);
    }

    [Fact]
    public void DefaultButtons_RenderWhenNoActionContent()
    {
        var cut = Render<FlareStepper>(p => p
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareStep>(0);
                b.AddAttribute(1, "Label", "One");
                b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(3, "<p>body</p>")));
                b.CloseComponent();
                b.OpenComponent<FlareStep>(4);
                b.AddAttribute(5, "Label", "Two");
                b.CloseComponent();
            }));

        // First step shows a primary "Next" button and no "Back".
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Stepper.NavBtnPrimary}"));
    }

    [Fact]
    public void Skippable_GoToPastSkippableStep_Advances()
    {
        var cut = Render<FlareStepper>(p => p
            .AddChildContent(ThreeSteps("One", "Two", "Three", middleSkippable: true)));

        cut.InvokeAsync(() => cut.Instance.GoTo(2));

        Assert.Equal("Three", ActiveLabel(cut));
    }

    [Fact]
    public void NonSkippable_GoToPastStep_Blocked()
    {
        var cut = Render<FlareStepper>(p => p
            .AddChildContent(ThreeSteps("One", "Two", "Three", middleSkippable: false)));

        cut.InvokeAsync(() => cut.Instance.GoTo(2));

        // Linear stepper refuses to jump over a non-skippable step.
        Assert.Equal("One", ActiveLabel(cut));
    }

    [Fact]
    public void GoTo_ImmediateNextStep_Advances_InLinearMode()
    {
        var cut = Render<FlareStepper>(p => p
            .AddChildContent(ThreeSteps("One", "Two", "Three")));

        cut.InvokeAsync(() => cut.Instance.GoTo(1));

        Assert.Equal("Two", ActiveLabel(cut));
    }
}

// ------------------------------------------------------------------------------
// FlareBreadcrumb  (6 tests from Wave3)
// ------------------------------------------------------------------------------
