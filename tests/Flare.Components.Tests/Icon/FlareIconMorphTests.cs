using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// FlareIconView.Morph cross-fades the outgoing glyph against the incoming one. The transition itself is
// CSS; what the component owes is the DOM shape that lets a CSS animation fire at all - a keyed slot per
// glyph, so Blazor INSERTS the new node instead of patching the path data into the existing one.
public class FlareIconMorphTests : FlareTestContext
{
    private const string Wrapper = $".{Css.Classes.Icon.Morph}";
    private const string Slot = $".{Css.Classes.Icon.MorphSlot}";
    private const string Enter = $".{Css.Classes.Icon.MorphSlotEnter}";
    private const string Exit = $".{Css.Classes.Icon.MorphSlotExit}";

    [Fact]
    public void MorphNone_IsTheDefault_AndRendersNoWrapper()
    {
        var cut = Render<FlareIconView>(p => p.Add(x => x.Value, FlareIcons.Home));

        Assert.Empty(cut.FindAll(Wrapper));
        Assert.Equal("svg", cut.Nodes[0].NodeName.ToLowerInvariant());
    }

    [Fact]
    public void MorphNone_SwapsInPlace_WithoutKeepingTheOldGlyph()
    {
        var cut = Render<FlareIconView>(p => p
            .Add(x => x.Value, FlareIcons.Home)
            .Add(x => x.Morph, FlareIconMorph.None));

        cut.Render(p => p.Add(x => x.Value, FlareIcons.Menu));

        Assert.Single(cut.FindAll("svg"));
        Assert.Equal(FlareIcons.Menu.Data, cut.Find("path").GetAttribute("d"));
    }

    // A first paint has nothing to transition from, so the initial glyph must not carry the enter
    // animation - an icon that animated itself in on page load would be noise on every page.
    [Fact]
    public void FirstRender_HasOneUnanimatedSlot()
    {
        var cut = Render<FlareIconView>(p => p
            .Add(x => x.Value, FlareIcons.Home)
            .Add(x => x.Morph, FlareIconMorph.Fade));

        Assert.Single(cut.FindAll(Wrapper));
        Assert.Single(cut.FindAll(Slot));
        Assert.Empty(cut.FindAll(Enter));
        Assert.Empty(cut.FindAll(Exit));
    }

    [Fact]
    public void ValueChange_KeepsBothGlyphs_OldExiting_NewEntering()
    {
        var cut = Render<FlareIconView>(p => p
            .Add(x => x.Value, FlareIcons.Home)
            .Add(x => x.Morph, FlareIconMorph.Fade));

        cut.Render(p => p.Add(x => x.Value, FlareIcons.Menu));

        Assert.Equal(2, cut.FindAll(Slot).Count);
        Assert.Equal(FlareIcons.Home.Data, cut.Find($"{Exit} path").GetAttribute("d"));
        Assert.Equal(FlareIcons.Menu.Data, cut.Find($"{Enter} path").GetAttribute("d"));
    }

    // Transparent is not hidden: the parked glyph is still in the accessibility tree, so an icon with an
    // AriaLabel would be announced twice unless the slot itself is hidden.
    [Fact]
    public void OutgoingSlot_IsHiddenFromAssistiveTechnology()
    {
        var labelled = FlareIcons.Home with { AriaLabel = "Home" };
        var cut = Render<FlareIconView>(p => p
            .Add(x => x.Value, labelled)
            .Add(x => x.Morph, FlareIconMorph.Fade));

        cut.Render(p => p.Add(x => x.Value, FlareIcons.Menu with { AriaLabel = "Menu" }));

        Assert.Equal("true", cut.Find(Exit).GetAttribute("aria-hidden"));
        Assert.Null(cut.Find(Enter).GetAttribute("aria-hidden"));
    }

    // The swap must be driven by the icon's CONTENT, not by instance identity: FlareIcon is a record, so a
    // freshly built but identical descriptor - what a parent re-render typically produces - is not a change.
    [Fact]
    public void EqualValue_DoesNotStartASwap()
    {
        var cut = Render<FlareIconView>(p => p
            .Add(x => x.Value, FlareIcons.Home)
            .Add(x => x.Morph, FlareIconMorph.Fade));

        cut.Render(p => p.Add(x => x.Value, new FlareSvgIcon { Data = FlareIcons.Home.Data }));

        Assert.Single(cut.FindAll(Slot));
        Assert.Empty(cut.FindAll(Exit));
    }

    // The parked ghost is recycled by the next swap rather than removed on a timer, so however many swaps
    // happen there are never more than the two slots the cross-fade needs.
    [Fact]
    public void RepeatedSwaps_NeverExceedTwoSlots()
    {
        var cut = Render<FlareIconView>(p => p
            .Add(x => x.Value, FlareIcons.Home)
            .Add(x => x.Morph, FlareIconMorph.Fade));

        cut.Render(p => p.Add(x => x.Value, FlareIcons.Menu));
        cut.Render(p => p.Add(x => x.Value, FlareIcons.Search));
        cut.Render(p => p.Add(x => x.Value, FlareIcons.Home));

        Assert.Equal(2, cut.FindAll(Slot).Count);
        Assert.Equal(FlareIcons.Search.Data, cut.Find($"{Exit} path").GetAttribute("d"));
        Assert.Equal(FlareIcons.Home.Data, cut.Find($"{Enter} path").GetAttribute("d"));
    }

    [Theory]
    [InlineData(FlareIconMorph.Fade, Css.Classes.Icon.MorphFade)]
    [InlineData(FlareIconMorph.Scale, Css.Classes.Icon.MorphScale)]
    [InlineData(FlareIconMorph.Rotate, Css.Classes.Icon.MorphRotate)]
    public void Mode_SelectsTheWrapperModifier(FlareIconMorph morph, string expected)
    {
        var cut = Render<FlareIconView>(p => p
            .Add(x => x.Value, FlareIcons.Home)
            .Add(x => x.Morph, morph));

        Assert.Contains(expected, cut.Find(Wrapper).ClassName);
    }

    // Turning the morph off has to forget the pair as well as the wrapper, or switching it back on later
    // would animate whatever glyph happened to be current when it was turned off.
    [Fact]
    public void TurningMorphOff_DropsTheWrapperAndTheHistory()
    {
        var cut = Render<FlareIconView>(p => p
            .Add(x => x.Value, FlareIcons.Home)
            .Add(x => x.Morph, FlareIconMorph.Fade));

        cut.Render(p => p.Add(x => x.Value, FlareIcons.Menu));
        cut.Render(p => p.Add(x => x.Morph, FlareIconMorph.None));
        Assert.Empty(cut.FindAll(Wrapper));

        cut.Render(p => p.Add(x => x.Morph, FlareIconMorph.Fade));

        Assert.Single(cut.FindAll(Slot));
        Assert.Empty(cut.FindAll(Enter));
    }

    // ---- The scope: an app turns transitions on library-wide without touching call sites ----------

    // bUnit's AddCascadingValue constrains TValue to notnull, and the scope is deliberately a NULLABLE
    // enum (null = "not set here"), so the cascade goes in through the root render tree instead.
    private void ScopeIs(FlareIconMorph morph) =>
        RenderTree.Add<CascadingValue<FlareIconMorph?>>(p => p.Add(c => c.Value, morph));

    [Fact]
    public void Scope_AppliesWhenMorphIsUnset()
    {
        ScopeIs(FlareIconMorph.Rotate);

        var cut = Render<FlareIconView>(p => p.Add(x => x.Value, FlareIcons.Home));
        cut.Render(p => p.Add(x => x.Value, FlareIcons.Menu));

        Assert.Contains(Css.Classes.Icon.MorphRotate, cut.Find(Wrapper).ClassName);
        Assert.Equal(2, cut.FindAll(Slot).Count);
    }

    [Fact]
    public void ExplicitMorph_WinsOverTheScope()
    {
        ScopeIs(FlareIconMorph.Rotate);

        var cut = Render<FlareIconView>(p => p
            .Add(x => x.Value, FlareIcons.Home)
            .Add(x => x.Morph, FlareIconMorph.Fade));

        Assert.Contains(Css.Classes.Icon.MorphFade, cut.Find(Wrapper).ClassName);
    }

    // The direction that matters most: one call site opting OUT of a scope that is on.
    [Fact]
    public void ExplicitNone_OptsOutOfTheScope()
    {
        ScopeIs(FlareIconMorph.Rotate);

        var cut = Render<FlareIconView>(p => p
            .Add(x => x.Value, FlareIcons.Home)
            .Add(x => x.Morph, FlareIconMorph.None));
        cut.Render(p => p.Add(x => x.Value, FlareIcons.Menu));

        Assert.Empty(cut.FindAll(Wrapper));
    }

    // ---- FlareMorphIcon: the outline transitions itself, so it is never cross-faded ---------------

    [Fact]
    public void MorphIcon_RendersBothTheAttributeAndTheCssProperty()
    {
        var cut = Render<FlareIconView>(p => p.Add(x => x.Value, FlareMorphIcons.Plus));

        var path = cut.Find("path");
        Assert.Equal(FlareMorphIcons.Plus.Data, path.GetAttribute("d"));
        Assert.Contains($"d:path('{FlareMorphIcons.Plus.Data}')", path.GetAttribute("style"));
        Assert.Contains(Css.Classes.Icon.PathMorph, cut.Find("svg").ClassName);
    }

    // Cross-fading a morph icon would replace the very element whose geometry is being interpolated, so a
    // mode that is on must still leave it alone.
    [Fact]
    public void MorphIcon_IsNeverWrapped_EvenWithAModeOn()
    {
        var cut = Render<FlareIconView>(p => p
            .Add(x => x.Value, FlareMorphIcons.Plus)
            .Add(x => x.Morph, FlareIconMorph.Scale));

        cut.Render(p => p.Add(x => x.Value, FlareMorphIcons.Minus));

        Assert.Empty(cut.FindAll(Wrapper));
        Assert.Single(cut.FindAll("path"));
        Assert.Equal(FlareMorphIcons.Minus.Data, cut.Find("path").GetAttribute("d"));
    }

    // The pairs are only useful if they actually interpolate, which needs the same command list on both
    // sides. Guarding the shape here is cheaper than discovering a mismatched pair as a mid-animation jump.
    [Theory]
    [InlineData(nameof(FlareMorphIcons.Plus), nameof(FlareMorphIcons.Minus))]
    [InlineData(nameof(FlareMorphIcons.ChevronDown), nameof(FlareMorphIcons.ChevronUp))]
    public void BuiltInPairs_ShareOneCommandList(string first, string second)
    {
        static string Commands(string data) =>
            new(data.Where(char.IsLetter).ToArray());

        static string DataOf(string name) =>
            ((FlareMorphIcon)typeof(FlareMorphIcons).GetProperty(name)!.GetValue(null)!).Data;

        Assert.Equal(Commands(DataOf(first)), Commands(DataOf(second)));
    }

    // The caller-facing overrides are merged onto the descriptor, not onto the wrapper, so the icon element
    // itself carries them exactly as it does without a morph.
    [Fact]
    public void CallerOverrides_StillReachTheGlyph_WhenMorphing()
    {
        var cut = Render<FlareIconView>(p => p
            .Add(x => x.Value, FlareIcons.Home)
            .Add(x => x.Morph, FlareIconMorph.Scale)
            .Add(x => x.Size, "3rem"));

        cut.Render(p => p.Add(x => x.Value, FlareIcons.Menu));

        foreach (var svg in cut.FindAll($"{Wrapper} svg"))
            Assert.Contains("font-size:3rem", svg.GetAttribute("style"));
    }
}
