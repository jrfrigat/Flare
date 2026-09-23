namespace Flare.Components.Tests;

/// <summary>
/// Bare drops the field's own surface so a surrounding row can paint it; Mono sets the entered text in
/// the monospace font. Both are root modifier classes the CSS keys off.
/// </summary>
public sealed class FlareFieldBareMonoTests : FlareTestContext
{
    [Fact]
    public void Bare_MarksTheRootAndNoOtherVariant()
    {
        var cut = Render<FlareTextField>(p => p.Add(x => x.Variant, InputVariant.Bare));

        var root = cut.Find($".{Css.Classes.Input.Root}");
        Assert.Contains(Css.Classes.Input.VariantBare, root.ClassList);
        Assert.DoesNotContain(Css.Classes.Input.VariantFilled, root.ClassList);
        Assert.DoesNotContain(Css.Classes.Input.VariantOutlined, root.ClassList);
    }

    // The variant is the family's, not the text field's: every field renders through the same frame.
    [Fact]
    public void Bare_ReachesANonTextField()
    {
        var cut = Render<FlareSelect<string>>(p => p.Add(x => x.Variant, InputVariant.Bare));

        Assert.Contains(Css.Classes.Input.VariantBare, cut.Find($".{Css.Classes.Input.Root}").ClassList);
    }

    [Fact]
    public void Mono_MarksTheFieldRoot()
    {
        var on = Render<FlareTextField>(p => p.Add(x => x.Mono, true));
        var off = Render<FlareTextField>();

        Assert.Contains(Css.Classes.Input.Mono, on.Find($".{Css.Classes.Input.Root}").ClassList);
        Assert.DoesNotContain(Css.Classes.Input.Mono, off.Find($".{Css.Classes.Input.Root}").ClassList);
    }

    [Fact]
    public void Mono_MarksTheTextAreaRootAlongsideAutoGrow()
    {
        var cut = Render<FlareTextArea>(p => p
            .Add(x => x.Mono, true)
            .Add(x => x.AutoGrow, true));

        var root = cut.Find($".{Css.Classes.Input.Root}");
        Assert.Contains(Css.Classes.Input.Mono, root.ClassList);
        Assert.Contains(Css.Classes.Textarea.Autogrow, root.ClassList);
    }
}
