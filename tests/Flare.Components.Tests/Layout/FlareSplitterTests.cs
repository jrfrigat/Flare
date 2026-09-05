using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareSplitterTests : FlareTestContext
{
    [Fact]
    public void RendersAsAStandaloneHandle()
    {
        var cut = Render<FlareSplitter>();

        // The component IS the handle (a single separator element with a grip bar) - no panes.
        Assert.Single(cut.FindAll($".{Css.Classes.Splitter.Root}"));
        Assert.Single(cut.FindAll($".{Css.Classes.Splitter.GutterBar}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Splitter.Root}__pane--first"));
        Assert.Equal("separator", cut.Find($".{Css.Classes.Splitter.Root}").GetAttribute("role"));
    }

    [Fact]
    public void DefaultOrientation_IsAuto_NoForcedAxisClass()
    {
        // Auto: the axis is detected from the parent flex direction at runtime (in JS), so no
        // orientation class is forced up front; aria defaults to a vertical separator (horizontal split).
        var cut = Render<FlareSplitter>();
        var cls = cut.Find($".{Css.Classes.Splitter.Root}").ClassName;
        Assert.DoesNotContain(Css.Classes.Splitter.Vertical, cls);
        Assert.DoesNotContain(Css.Classes.Splitter.Horizontal, cls);
        Assert.Equal("vertical", cut.Find($".{Css.Classes.Splitter.Root}").GetAttribute("aria-orientation"));
    }

    [Fact]
    public void ExplicitOrientation_AppliesClass()
    {
        var horizontal = Render<FlareSplitter>(p => p
            .Add(x => x.Orientation, FlareSplitter.SplitterOrientation.Horizontal));
        Assert.Contains(Css.Classes.Splitter.Horizontal, horizontal.Find($".{Css.Classes.Splitter.Root}").ClassName);
        Assert.Equal("vertical", horizontal.Find($".{Css.Classes.Splitter.Root}").GetAttribute("aria-orientation"));

        var vertical = Render<FlareSplitter>(p => p
            .Add(x => x.Orientation, FlareSplitter.SplitterOrientation.Vertical));
        Assert.Contains(Css.Classes.Splitter.Vertical, vertical.Find($".{Css.Classes.Splitter.Root}").ClassName);
        Assert.Equal("horizontal", vertical.Find($".{Css.Classes.Splitter.Root}").GetAttribute("aria-orientation"));
    }

    [Fact]
    public void Handle_IsKeyboardFocusable()
    {
        var cut = Render<FlareSplitter>();
        Assert.Equal("0", cut.Find($".{Css.Classes.Splitter.Root}").GetAttribute("tabindex"));
    }

    [Fact]
    public void Icon_ReplacesGripBar_AndHoverIconRenders()
    {
        var cut = Render<FlareSplitter>(p => p
            .Add(x => x.Icon, FlareIcons.DragIndicator)
            .Add(x => x.HoverIcon, FlareIcons.OpenInNew));

        Assert.Empty(cut.FindAll($".{Css.Classes.Splitter.GutterBar}"));
        // drag_indicator is built in (inline SVG); the hover icon element still renders.
        Assert.Equal(FlareIcons.DragIndicator.Data, cut.Find($".{Css.Classes.Splitter.IconBase} path").GetAttribute("d"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Splitter.IconHover}"));
    }

    [Fact]
    public void ChildContent_OverridesIconAndGrip()
    {
        var cut = Render<FlareSplitter>(p => p
            .Add(x => x.Icon, FlareIcons.DragIndicator)
            .AddChildContent("<b id=\"custom\">grip</b>"));

        Assert.Single(cut.FindAll("#custom"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Splitter.IconBase}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Splitter.GutterBar}"));
    }

    [Fact]
    public void SizeAndColors_SetCssVariables()
    {
        var cut = Render<FlareSplitter>(p => p
            .Add(x => x.Size, "14px")
            .Add(x => x.Color, $"var({Css.Tokens.Color.SurfaceContainerHigh})")
            .Add(x => x.HoverColor, $"var({Css.Tokens.Color.PrimaryContainer})"));

        var style = cut.Find($".{Css.Classes.Splitter.Root}").GetAttribute("style");
        Assert.Contains($"{Css.Tokens.Splitter.GutterSize}:14px", style);
        Assert.Contains($"{Css.Tokens.Splitter.Color}:var({Css.Tokens.Color.SurfaceContainerHigh})", style);
        Assert.Contains($"{Css.Tokens.Splitter.HoverColor}:var({Css.Tokens.Color.PrimaryContainer})", style);
    }
}
