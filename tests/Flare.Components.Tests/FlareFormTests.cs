// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
using System.ComponentModel.DataAnnotations;

namespace Flare.Components.Tests;

// ------------------------------------------------------------------------------
// FlareForm  (6 tests)
// ------------------------------------------------------------------------------

public class FlareFormTests : FlareTestContext
{
    private class PersonModel
    {
        [Required] public string Name { get; set; } = "";
        public string Email { get; set; } = "";
    }

    [Fact]
    public void RendersRootDiv()
    {
        var model = new PersonModel();
        var cut = RenderComponent<FlareForm>(p => p
            .Add(x => x.Model, model));

        Assert.NotEmpty(cut.FindAll(".flare-form"));
    }

    [Fact]
    public void ContainsFormElement()
    {
        var model = new PersonModel();
        var cut = RenderComponent<FlareForm>(p => p
            .Add(x => x.Model, model));

        Assert.NotEmpty(cut.FindAll("form"));
    }

    [Fact]
    public void RendersChildContent()
    {
        var model = new PersonModel();
        var cut = RenderComponent<FlareForm>(p => p
            .Add(x => x.Model, model)
            .AddChildContent("<span class=\"child-item\">Inside form</span>"));

        Assert.NotEmpty(cut.FindAll(".child-item"));
    }

    [Fact]
    public void RendersFlareFieldInside()
    {
        var model = new PersonModel();
        var cut = RenderComponent<FlareForm>(p => p
            .Add(x => x.Model, model)
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareField<string>>(0);
                b.AddAttribute(1, "Label", "Name");
                b.CloseComponent();
            }));

        Assert.NotEmpty(cut.FindAll("input"));
    }

    [Fact]
    public void RendersValidationSummaryInside()
    {
        var model = new PersonModel();
        var cut = RenderComponent<FlareForm>(p => p
            .Add(x => x.Model, model)
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareValidationSummary>(0);
                b.CloseComponent();
            }));

        // No errors initially so flare-validation-summary should not render
        Assert.Empty(cut.FindAll(".flare-validation-summary"));
    }

    [Fact]
    public void AcceptsAdditionalAttributes()
    {
        var model = new PersonModel();
        var cut = RenderComponent<FlareForm>(p => p
            .Add(x => x.Model, model)
            .AddUnmatched("data-testid", "my-form"));

        Assert.Equal("my-form", cut.Find(".flare-form").GetAttribute("data-testid"));
    }
}
