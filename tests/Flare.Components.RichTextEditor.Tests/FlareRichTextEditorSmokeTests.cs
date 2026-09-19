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

/// <summary>
/// ReadOnly used to reach the toolbar and nothing else: the init script set contentEditable
/// unconditionally, so a read-only editor still took typing and still raised ValueChanged, and
/// aria-readonly was bound to a bool, which lands in the DOM as an empty attribute - the accessibility
/// tree read the read-only example as settable.
/// </summary>
public class FlareRichTextEditorReadOnlyTests : FlareTestContext
{
    private static AngleSharp.Dom.IElement Surface(IRenderedComponent<FlareRichTextEditor> cut)
        => cut.Find($".{Css.Classes.Rte.Content}");

    [Fact]
    public void Editable_ByDefault()
    {
        var surface = Surface(Render<FlareRichTextEditor>());

        Assert.Equal("true", surface.GetAttribute("contenteditable"));
        Assert.Equal("false", surface.GetAttribute("aria-readonly"));
    }

    [Fact]
    public void ReadOnly_MarksTheSurfaceNotJustTheToolbar()
    {
        var surface = Surface(Render<FlareRichTextEditor>(p => p.Add(x => x.ReadOnly, true)));

        Assert.Equal("false", surface.GetAttribute("contenteditable"));
        Assert.Equal("true", surface.GetAttribute("aria-readonly"));
    }

    // The attribute has to be a word. A bool renders as an empty attribute, and `aria-readonly=""` is
    // not `true` to anything reading the tree.
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AriaReadOnly_IsAlwaysAnExplicitWord(bool readOnly)
    {
        var surface = Surface(Render<FlareRichTextEditor>(p => p.Add(x => x.ReadOnly, readOnly)));

        var value = surface.GetAttribute("aria-readonly");
        Assert.True(value is "true" or "false", $"aria-readonly was \"{value}\"");
    }

    // Toggling has to reach the DOM without the editor being torn down and built again - the surface
    // holds the caller's content, and re-initialising it would be visible.
    [Fact]
    public void TogglingAtRuntime_SyncsWithoutRemountingTheSurface()
    {
        var cut = Render<FlareRichTextEditor>(p => p.Add(x => x.ReadOnly, true));
        var idWhileReadOnly = Surface(cut).GetAttribute("id");

        cut.Render(p => p.Add(x => x.ReadOnly, false));

        Assert.Equal("true", Surface(cut).GetAttribute("contenteditable"));
        Assert.Equal("false", Surface(cut).GetAttribute("aria-readonly"));
        Assert.Equal(idWhileReadOnly, Surface(cut).GetAttribute("id"));   // same element, not a new one
    }

    // contenteditable="false" stops a real keystroke, but an input event can still be dispatched, and a
    // read-only editor must not report a change the caller never allowed.
    [Fact]
    public async Task ReadOnly_SwallowsAContentChange()
    {
        string? reported = null;
        var cut = Render<FlareRichTextEditor>(p => p
            .Add(x => x.ReadOnly, true)
            .Add(x => x.ValueChanged, (string v) => reported = v));

        await cut.Instance.OnContentChanged("<p>typed anyway</p>");

        Assert.Null(reported);
    }

    [Fact]
    public async Task Editable_ReportsAContentChange()
    {
        string? reported = null;
        var cut = Render<FlareRichTextEditor>(p => p
            .Add(x => x.ValueChanged, (string v) => reported = v));

        await cut.Instance.OnContentChanged("<p>typed</p>");

        Assert.Equal("<p>typed</p>", reported);
    }
}
