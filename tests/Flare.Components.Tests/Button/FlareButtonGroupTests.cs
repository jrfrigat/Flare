using Flare.Components.Tests;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareButtonGroupTests : FlareTestContext
{
    [Fact]
    public void RendersHorizontally()
    {
        var cut = Render<FlareButtonGroup>(p => p
            .Add(x => x.Vertical, false));

        var div = cut.Find($".{Css.Classes.ButtonGroup.Root}");
        Assert.NotNull(div);
        Assert.DoesNotContain(Css.Classes.ButtonGroup.Vertical, div.ClassName);
    }

    [Fact]
    public void RendersVertically()
    {
        var cut = Render<FlareButtonGroup>(p => p
            .Add(x => x.Vertical, true));

        Assert.Contains(Css.Classes.ButtonGroup.Vertical, cut.Find($".{Css.Classes.ButtonGroup.Root}").ClassName);
    }

    [Fact]
    public void RendersFullWidth()
    {
        var cut = Render<FlareButtonGroup>(p => p
            .Add(x => x.FullWidth, true));

        Assert.Contains(Css.Classes.ButtonGroup.Full, cut.Find($".{Css.Classes.ButtonGroup.Root}").ClassName);
    }

    [Fact]
    public void CollapsibleRendersAnOverflowControlAndASecondCopy()
    {
        // The fold itself is a measurement and only a browser can make it, so what is testable here is
        // the contract the measurer needs: the trailing overflow control exists, and the panel holds a
        // second copy of the same content for it to reveal. Without the copy there would be nothing to
        // fold INTO, and the group would just clip.
        var cut = Render<FlareButtonGroup>(p => p
            .Add(x => x.Collapsible, true)
            .AddChildContent($"<button class=\"{Css.Classes.Button.Root}\">One</button><button class=\"{Css.Classes.Button.Root}\">Two</button>"));

        var root = cut.Find($".{Css.Classes.ButtonGroup.Root}");
        Assert.Contains(Css.Classes.ButtonGroup.Collapsible, root.ClassName);
        Assert.Single(cut.FindAll($".{Css.Classes.ButtonGroup.More}"));

        // The copies live behind the menu, so they exist only once it is open - which is also why
        // the measurer treats the panel as optional rather than assuming it is there.
        Assert.Empty(cut.FindAll($".{Css.Classes.ButtonGroup.OverflowList}"));
        cut.Find($".{Css.Classes.ButtonGroup.More} .{Css.Classes.Button.Root}").Click();
        Assert.Single(cut.FindAll($".{Css.Classes.ButtonGroup.OverflowList}"));
        Assert.Equal(2, cut.FindAll($".{Css.Classes.ButtonGroup.OverflowList} .{Css.Classes.Button.Root}").Count);
    }

    [Fact]
    public void AToggleIsAButtonAndCarriesTheButtonFamily()
    {
        // The whole point of the rebuild: a toggle segment and a plain segment are the same element with
        // the same classes, so every group rule reaches both with the same token family. While the toggle
        // was a control of its own, the group's press rule found it holding a different padding token and
        // GREW the neighbours it was supposed to shrink.
        var cut = Render<FlareToggleButton>(p => p
            .Add(x => x.Variant, ButtonVariant.Outlined)
            .Add(x => x.Size, ButtonSize.Lg)
            .Add(x => x.Toggled, true)
            .AddChildContent("Bold"));

        var btn = cut.Find("button");
        Assert.Contains(Css.Classes.Button.Root, btn.ClassName);
        Assert.Contains(Css.Classes.Button.Outlined, btn.ClassName);
        Assert.Contains(Css.Classes.Button.Lg, btn.ClassName);
        Assert.Contains(Css.Classes.Button.Selected, btn.ClassName);
        // An unselected toggle must still say it is a toggle: an absent aria-pressed reads as a plain
        // command, so "false" is the state and not the absence of one.
        Assert.Equal("true", btn.GetAttribute("aria-pressed"));

        var off = Render<FlareToggleButton>(p => p.AddChildContent("Bold"));
        Assert.Equal("false", off.Find("button").GetAttribute("aria-pressed"));
        Assert.DoesNotContain(Css.Classes.Button.Selected, off.Find("button").ClassName);
    }

    [Fact]
    public void TheLabelCanChangeWithTheState()
    {
        // A toggle whose two states are different verbs says so in the label, not only in the icon.
        var cut = Render<FlareToggleButton>(p => p
            .Add(x => x.OnLabel, (RenderFragment)(b => b.AddContent(0, "Following")))
            .AddChildContent("Follow"));

        Assert.Contains("Follow", cut.Find("button").TextContent);
        Assert.DoesNotContain("Following", cut.Find("button").TextContent);

        cut.Find("button").Click();
        Assert.Contains("Following", cut.Find("button").TextContent);
    }

    [Fact]
    public void AToggleWithNoLabelIsIconOnly()
    {
        // FlareButton decides "icon-only" by ChildContent being null, so the label has to reach it as a
        // parameter: markup between the tags compiles to a fragment that renders nothing but is not null,
        // which left an icon-only toggle full width with an empty label span and a gap beside its glyph.
        var cut = Render<FlareToggleButton>(p => p
            .Add(x => x.OffIcon, (RenderFragment)(b => b.AddMarkupContent(0, "<i class=\"icon\"></i>")))
            .Add(x => x.AriaLabel, "Star"));

        Assert.Contains(Css.Classes.Button.IconOnly, cut.Find("button").ClassName);
        Assert.Empty(cut.FindAll($".{Css.Classes.Button.Label}"));
    }

    [Fact]
    public void TheLabelStaysPutWhenOnlyOneIsGiven()
    {
        // Falling back keeps a toggle that changes only colour and shape from having to say its label
        // twice.
        var cut = Render<FlareToggleButton>(p => p.AddChildContent("Bold"));
        cut.Find("button").Click();
        Assert.Contains("Bold", cut.Find("button").TextContent);
    }

    [Fact]
    public void APlainButtonIsNotAToggle()
    {
        // The tri-state is what keeps that promise in the other direction: an ordinary button must not
        // grow an aria-pressed just because the parameter exists.
        var cut = Render<FlareButton>(p => p.AddChildContent("Save"));
        Assert.False(cut.Find("button").HasAttribute("aria-pressed"));
    }

    [Fact]
    public void OverflowPanelIsAGroupAndNotAMenu()
    {
        // The folded segments are buttons, not menu items: they take the focus themselves. A menu panel
        // keeps the focus and swallows the keys that would move it, which is right for items that cannot
        // be focused and wrong here - it would leave every folded button reachable by nothing but Escape.
        var cut = Render<FlareButtonGroup>(p => p
            .Add(x => x.Collapsible, true)
            .AddChildContent($"<button class=\"{Css.Classes.Button.Root}\">One</button>"));

        cut.Find($".{Css.Classes.ButtonGroup.More} .{Css.Classes.Button.Root}").Click();
        Assert.Equal("group", cut.Find($".{Css.Classes.Menu.Panel}").GetAttribute("role"));
    }

    [Fact]
    public void OverflowEllipsisTurnsWithTheGroup()
    {
        // The dots run ACROSS the bar they fold, so a row gets the vertical ellipsis and a column the
        // horizontal one - the same convention an app bar and a navigation rail follow.
        var horizontal = Render<FlareButtonGroup>(p => p.Add(x => x.Collapsible, true));
        Assert.Contains(FlareIcons.MoreVert.Data, horizontal.Markup);

        var vertical = Render<FlareButtonGroup>(p => p
            .Add(x => x.Collapsible, true)
            .Add(x => x.Vertical, true));
        Assert.Contains(FlareIcons.MoreHoriz.Data, vertical.Markup);
    }

    [Fact]
    public void NotCollapsibleRendersNoOverflowControl()
    {
        var cut = Render<FlareButtonGroup>(p => p.AddChildContent($"<button class=\"{Css.Classes.Button.Root}\">One</button>"));
        Assert.DoesNotContain(Css.Classes.ButtonGroup.Collapsible, cut.Find($".{Css.Classes.ButtonGroup.Root}").ClassName);
        Assert.Empty(cut.FindAll($".{Css.Classes.ButtonGroup.More}"));
    }

    [Fact]
    public void NamesItsModel()
    {
        // The two models behave differently under a press - a standard group's segments trade width
        // with each other, a connected group's only change shape - so the markup has to say which one
        // it is rather than one being the absence of the other. Standard is the default.
        foreach (var (connected, expected, notExpected) in new[]
                 {
                     (false, Css.Classes.ButtonGroup.Standard, Css.Classes.ButtonGroup.Connected),
                     (true, Css.Classes.ButtonGroup.Connected, Css.Classes.ButtonGroup.Standard),
                 })
        {
            var cut = Render<FlareButtonGroup>(p => p.Add(x => x.Connected, connected));
            var cls = cut.Find($".{Css.Classes.ButtonGroup.Root}").ClassName;
            Assert.Contains(expected, cls);
            Assert.DoesNotContain(notExpected, cls);
        }
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareButtonGroup>(p => p
            .AddChildContent("<button class=\"child-btn\">A</button><button class=\"child-btn\">B</button>"));

        Assert.Equal(2, cut.FindAll(".child-btn").Count);
    }

    [Fact]
    public void RendersDefaultVariant_NoVerticalModifier()
    {
        var cut = Render<FlareButtonGroup>();

        var div = cut.Find($".{Css.Classes.ButtonGroup.Root}");
        Assert.DoesNotContain(Css.Classes.ButtonGroup.Vertical, div.ClassName);
        Assert.DoesNotContain(Css.Classes.ButtonGroup.Full, div.ClassName);
    }

    [Fact]
    public void RendersFlareButtonsInside()
    {
        var cut = Render<FlareButtonGroup>(p => p
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareButton>(0);
                b.AddAttribute(1, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(2, "Save")));
                b.CloseComponent();
            }));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Button.Filled}"));
    }

    [Fact]
    public void CascadesSizeAndVariantToChildButtons()
    {
        var cut = Render<FlareButtonGroup>(p => p
            .Add(x => x.Size, ButtonSize.Lg)
            .Add(x => x.Variant, ButtonVariant.Tonal)
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareButton>(0);
                b.AddAttribute(1, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(2, "Save")));
                b.CloseComponent();
            }));

        var btn = cut.Find($".{Css.Classes.Button.Root}");
        Assert.Contains(Css.Classes.Button.Lg, btn.ClassName);
        Assert.Contains(Css.Classes.Button.Tonal, btn.ClassName);
    }

    [Fact]
    public void ButtonKeepsOwnVariantWhenGroupDoesNotOverride()
    {
        var cut = Render<FlareButtonGroup>(p => p
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareButton>(0);
                b.AddAttribute(1, "Variant", ButtonVariant.Outlined);
                b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(3, "Save")));
                b.CloseComponent();
            }));

        Assert.Contains(Css.Classes.Button.Outlined, cut.Find($".{Css.Classes.Button.Root}").ClassName);
    }
}

// ------------------------------------------------------------------------------
// FlareFloatingActionButton  (8 tests)
// ------------------------------------------------------------------------------
