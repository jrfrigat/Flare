namespace Flare.Components.Tests;

public class FlareAccordionFillTests : FlareTestContext
{
    [Fact]
    public void FillHeight_AddsTheSharedFillContract()
    {
        var cut = Render<FlareAccordion>(p => p
            .Add(x => x.FillHeight, true)
            .AddChildContent<FlareAccordionPanel>(panel => panel
                .Add(x => x.Header, "One")));

        var root = cut.Find($".{Css.Classes.Accordion.Root}");
        Assert.Contains(Css.Classes.Fill.Root, root.ClassName);
        Assert.Contains(Css.Classes.Accordion.Fill, root.ClassName);
    }

    [Fact]
    public void Default_DoesNotFill()
    {
        var cut = Render<FlareAccordion>(p => p
            .AddChildContent<FlareAccordionPanel>(panel => panel
                .Add(x => x.Header, "One")));

        var root = cut.Find($".{Css.Classes.Accordion.Root}");
        Assert.DoesNotContain(Css.Classes.Fill.Root, root.ClassName);
        Assert.DoesNotContain(Css.Classes.Accordion.Fill, root.ClassName);
    }
}
