namespace Flare.Components.Tests;

/// <summary>
/// The package shipped with no test at all. The editor is a toolbar over a contenteditable surface, and
/// both halves are what the JS looks for, so this says they are still drawn and still disabled together.
/// </summary>
public class FlareRichTextEditorSmokeTests : FlareTestContext
{
    [Fact]
    public void RendersItsToolbarAndAnEditableSurface()
    {
        var cut = Render<FlareRichTextEditor>();

        Assert.NotEmpty(cut.FindAll(".flare-rte"));
        Assert.NotEmpty(cut.FindAll(".flare-rte__toolbar"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Rte.Tool}"));
    }

    [Fact]
    public void ReadOnly_DisablesEveryTool()
    {
        var cut = Render<FlareRichTextEditor>(p => p.Add(x => x.ReadOnly, true));

        Assert.All(cut.FindAll($".{Css.Classes.Rte.Tool}"), b => Assert.True(b.HasAttribute("disabled")));
    }
}
