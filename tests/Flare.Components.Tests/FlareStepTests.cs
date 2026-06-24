// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

// -----------------------------------------------------------------------------
// FlareStep  (5 tests)
// -----------------------------------------------------------------------------

public class FlareStepTests : FlareTestContext
{
    [Fact]
    public void RendersStepLabel()
    {
        var cut = Render<FlareStepper>(p => p
            .AddChildContent<FlareStep>(s => s.Add(x => x.Label, "My Step")));

        Assert.Contains("My Step", cut.Markup);
    }

    [Fact]
    public void RendersDescription_WhenProvided()
    {
        var cut = Render<FlareStepper>(p => p
            .AddChildContent<FlareStep>(s => s
                .Add(x => x.Label, "Step")
                .Add(x => x.Description, "Step description text")));

        Assert.Contains("Step description text", cut.Markup);
    }

    [Fact]
    public void NoDescription_DescriptionElementAbsent()
    {
        var cut = Render<FlareStepper>(p => p
            .AddChildContent<FlareStep>(s => s.Add(x => x.Label, "Step")));

        Assert.Empty(cut.FindAll(".flare-stepper__description"));
    }

    [Fact]
    public void StepNumber_DisplayedInCircle()
    {
        var cut = Render<FlareStepper>(p => p
            .AddChildContent<FlareStep>(s => s.Add(x => x.Label, "First")));

        // First step shows number "1" in the circle
        var circle = cut.Find(".flare-stepper__circle");
        Assert.Contains("1", circle.TextContent);
    }

    [Fact]
    public void TwoSteps_LabelsRenderedInOrder()
    {
        var cut = Render<FlareStepper>(p => p
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareStep>(0);
                b.AddAttribute(1, "Label", "Alpha");
                b.CloseComponent();
                b.OpenComponent<FlareStep>(2);
                b.AddAttribute(3, "Label", "Beta");
                b.CloseComponent();
            }));

        var labels = cut.FindAll(".flare-stepper__label");
        Assert.Equal(2, labels.Count);
        Assert.Contains("Alpha", labels[0].TextContent);
        Assert.Contains("Beta", labels[1].TextContent);
    }
}
