using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace Flare.Components.Tests;

public class FlareFormLayoutTests : FlareTestContext
{
    private readonly object _model = new();

    [Fact]
    public void RendersRootFlareForm()
    {
        var cut = Render<FlareForm>(p => p.Add(x => x.Model, _model));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Form.Root}"));
    }

    [Fact]
    public void DefaultLayoutHasNoVariantClass()
    {
        var cut = Render<FlareForm>(p => p.Add(x => x.Model, _model));
        var cls = cut.Find($".{Css.Classes.Form.Root}").ClassName;
        Assert.DoesNotContain(Css.Classes.Form.Horizontal, cls);
        Assert.DoesNotContain(Css.Classes.Form.Inline, cls);
    }

    [Fact]
    public void HorizontalLayoutAppliesClass()
    {
        var cut = Render<FlareForm>(p =>
        {
            p.Add(x => x.Model, _model);
            p.Add(x => x.Layout, FormLayout.Horizontal);
        });
        Assert.Contains(Css.Classes.Form.Horizontal, cut.Find($".{Css.Classes.Form.Root}").ClassName);
    }

    [Fact]
    public void InlineLayoutAppliesClass()
    {
        var cut = Render<FlareForm>(p =>
        {
            p.Add(x => x.Model, _model);
            p.Add(x => x.Layout, FormLayout.Inline);
        });
        Assert.Contains(Css.Classes.Form.Inline, cut.Find($".{Css.Classes.Form.Root}").ClassName);
    }

    [Fact]
    public void DenseAppliesClass()
    {
        var cut = Render<FlareForm>(p =>
        {
            p.Add(x => x.Model, _model);
            p.Add(x => x.Dense, true);
        });
        Assert.Contains(Css.Classes.Form.Dense, cut.Find($".{Css.Classes.Form.Root}").ClassName);
    }

    [Fact]
    public void NotDenseByDefault()
    {
        var cut = Render<FlareForm>(p => p.Add(x => x.Model, _model));
        Assert.DoesNotContain(Css.Classes.Form.Dense, cut.Find($".{Css.Classes.Form.Root}").ClassName);
    }

    [Fact]
    public void DenseAndHorizontalCombine()
    {
        var cut = Render<FlareForm>(p =>
        {
            p.Add(x => x.Model, _model);
            p.Add(x => x.Layout, FormLayout.Horizontal);
            p.Add(x => x.Dense, true);
        });
        var cls = cut.Find($".{Css.Classes.Form.Root}").ClassName;
        Assert.Contains(Css.Classes.Form.Horizontal, cls);
        Assert.Contains(Css.Classes.Form.Dense, cls);
    }

    [Fact]
    public void ChildContentRenderedInsideEditForm()
    {
        var cut = Render<FlareForm>(p =>
        {
            p.Add(x => x.Model, _model);
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenElement(0, "span");
                b.AddAttribute(1, "id", "inner-content");
                b.CloseElement();
            }));
        });
        Assert.NotEmpty(cut.FindAll("#inner-content"));
    }
}
