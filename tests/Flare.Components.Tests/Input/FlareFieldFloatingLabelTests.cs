namespace Flare.Components.Tests;

public class FlareFieldFloatingLabelTests : FlareTestContext
{
    [Fact]
    public void NoFloatingByDefault()
    {
        var cut = Render<FlareField<string>>(p => p.Add(x => x.Label, "Name"));
        var cls = cut.Find($".{Css.Classes.Input.Root}").ClassName;
        Assert.DoesNotContain(Css.Classes.Input.Floating, cls);
    }

    [Fact]
    public void FloatingLabelAppliesClass()
    {
        var cut = Render<FlareField<string>>(p =>
        {
            p.Add(x => x.Label, "Name");
            p.Add(x => x.FloatingLabel, true);
        });
        Assert.Contains(Css.Classes.Input.Floating, cut.Find($".{Css.Classes.Input.Root}").ClassName);
    }

    [Fact]
    public void NonFloatingLabelRenderedBeforeField()
    {
        var cut = Render<FlareField<string>>(p => p.Add(x => x.Label, "Email"));
        var label = cut.Find($"label.{Css.Classes.Input.Label}");
        Assert.NotNull(label);
        Assert.DoesNotContain(Css.Classes.Input.LabelFloating, label.ClassName);
    }

    [Fact]
    public void FloatingLabelRenderedInsideField()
    {
        var cut = Render<FlareField<string>>(p =>
        {
            p.Add(x => x.Label, "Email");
            p.Add(x => x.FloatingLabel, true);
        });
        var label = cut.Find($".{Css.Classes.Input.Field} label.{Css.Classes.Input.LabelFloating}");
        Assert.NotNull(label);
        Assert.Contains("Email", label.TextContent);
    }

    [Fact]
    public void NoLabelNoLabelElement()
    {
        var cut = Render<FlareField<string>>();
        Assert.Empty(cut.FindAll("label"));
    }

    [Fact]
    public void FloatingAndNoLabelNoLabelElement()
    {
        var cut = Render<FlareField<string>>(p => p.Add(x => x.FloatingLabel, true));
        Assert.Empty(cut.FindAll("label"));
    }

    [Fact]
    public void FloatingLabelHasForAttribute()
    {
        var cut = Render<FlareField<string>>(p =>
        {
            p.Add(x => x.Label, "Search");
            p.Add(x => x.FloatingLabel, true);
        });
        var label = cut.Find($".{Css.Classes.Input.LabelFloating}");
        Assert.NotNull(label.GetAttribute("for"));
    }
}
