namespace Flare.Components.Tests;

// The add-on Font Awesome pack lives in a separate assembly yet its FlareFontAwesomeIcon subclasses the
// core FlareIcon, so it renders through FlareIconView and drops into any FlareIcon-typed slot - the whole
// point of the polymorphic model.
public class FlareFontAwesomeIconTests : FlareTestContext
{
    [Fact]
    public void SolidIcon_RendersFaSolidClasses()
    {
        var cut = Render<FlareIconView>(p => p
            .Add(x => x.Value, new FlareFontAwesomeIcon { Name = "house" }));

        var i = cut.Find("i");
        Assert.Contains("fa-solid", i.ClassList);
        Assert.Contains("fa-house", i.ClassList);
        Assert.Contains("flare-icon", i.ClassList);
    }

    [Fact]
    public void BrandFactory_RendersFaBrands()
    {
        var cut = Render<FlareIconView>(p => p
            .Add(x => x.Value, FlareFontAwesomeIcon.Brand("github")));

        var i = cut.Find("i");
        Assert.Contains("fa-brands", i.ClassList);
        Assert.Contains("fa-github", i.ClassList);
    }

    [Fact]
    public void FlowsIntoFlareIconButton_AsFlareIcon()
    {
        // FlareIconButton.Icon is typed FlareIcon; a pack type from another assembly must be accepted.
        var cut = Render<FlareIconButton>(p => p
            .Add(x => x.Icon, new FlareFontAwesomeIcon { Name = "gear", Variant = FaStyle.Regular })
            .Add(x => x.AriaLabel, "Settings"));

        var i = cut.Find("i");
        Assert.Contains("fa-regular", i.ClassList);
        Assert.Contains("fa-gear", i.ClassList);
    }
}
