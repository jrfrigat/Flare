using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// The layout parameters a row of actions and a mobile bar need: wrapping, stacking, full width, and a
// pinned bar that reserves its own space.
public class CardActionsAndBottomNavTests : FlareTestContext
{
    private static RenderFragment TwoButtons => b =>
    {
        b.OpenComponent<FlareButton>(0);
        b.AddAttribute(1, "ChildContent", (RenderFragment)(c => c.AddContent(0, "Cancel")));
        b.CloseComponent();
        b.OpenComponent<FlareButton>(2);
        b.AddAttribute(3, "ChildContent", (RenderFragment)(c => c.AddContent(0, "Save")));
        b.CloseComponent();
    };

    // ---- FlareCardActions --------------------------------------------------------------------------

    [Fact]
    public void ActionsWrapByDefault()
    {
        var cut = Render<FlareCardActions>(p => p.Add(x => x.ChildContent, TwoButtons));
        Assert.DoesNotContain(Css.Classes.Card.ActionsNoWrap, cut.Find($".{Css.Classes.Card.Actions}").ClassName);
    }

    [Fact]
    public void WrapFalse_AddsTheNoWrapModifier()
    {
        var cut = Render<FlareCardActions>(p => p
            .Add(x => x.Wrap, false)
            .Add(x => x.ChildContent, TwoButtons));

        Assert.Contains(Css.Classes.Card.ActionsNoWrap, cut.Find($".{Css.Classes.Card.Actions}").ClassName);
    }

    [Theory]
    [InlineData(CardActionsAlign.Start, null)]
    [InlineData(CardActionsAlign.Center, Css.Classes.Card.ActionsCenter)]
    [InlineData(CardActionsAlign.End, Css.Classes.Card.ActionsEnd)]
    [InlineData(CardActionsAlign.Between, Css.Classes.Card.ActionsBetween)]
    [InlineData(CardActionsAlign.Stretch, Css.Classes.Card.ActionsStretch)]
    public void Align_MapsToItsModifier(CardActionsAlign align, string? expected)
    {
        var cut = Render<FlareCardActions>(p => p
            .Add(x => x.Align, align)
            .Add(x => x.ChildContent, TwoButtons));

        var className = cut.Find($".{Css.Classes.Card.Actions}").ClassName ?? "";
        if (expected is null) Assert.DoesNotContain($"{Css.Classes.Card.Root}__actions--", className);
        else Assert.Contains(expected, className);
    }

    [Fact]
    public void VerticalAndFullWidth_AddTheirModifiers()
    {
        var cut = Render<FlareCardActions>(p => p
            .Add(x => x.Vertical, true)
            .Add(x => x.FullWidth, true)
            .Add(x => x.Reverse, true)
            .Add(x => x.ChildContent, TwoButtons));

        var className = cut.Find($".{Css.Classes.Card.Actions}").ClassName ?? "";
        Assert.Contains(Css.Classes.Card.ActionsVertical, className);
        Assert.Contains(Css.Classes.Card.ActionsFullWidth, className);
        Assert.Contains(Css.Classes.Card.ActionsReverse, className);
    }

    [Fact]
    public void StackBelow_MapsToTheContainerQueryModifier()
    {
        var cut = Render<FlareCardActions>(p => p
            .Add(x => x.StackBelow, CardActionsStack.Compact)
            .Add(x => x.ChildContent, TwoButtons));

        Assert.Contains(Css.Classes.Card.ActionsStackCompact, cut.Find($".{Css.Classes.Card.Actions}").ClassName);
    }

    [Fact]
    public void StackNever_MapsToNothing()
    {
        var cut = Render<FlareCardActions>(p => p
            .Add(x => x.StackBelow, CardActionsStack.Never)
            .Add(x => x.ChildContent, TwoButtons));

        Assert.DoesNotContain("stack-", cut.Find($".{Css.Classes.Card.Actions}").ClassName ?? "");
    }

    [Fact]
    public void Gap_TravelsAsTheCardsOwnLocalProperty()
    {
        var cut = Render<FlareCardActions>(p => p
            .Add(x => x.Gap, FlareSpacing.Large)
            .Add(x => x.ChildContent, TwoButtons));

        Assert.Contains($"--_card-actions-gap:var({Css.Tokens.Spacing.S12})",
            cut.Find($".{Css.Classes.Card.Actions}").GetAttribute("style") ?? "");
    }

    // ---- FlareBottomNav ----------------------------------------------------------------------------

    [Fact]
    public void BottomNav_IsStaticByDefault_AndReservesNothing()
    {
        var cut = Render<FlareBottomNav>();
        Assert.DoesNotContain("flare-bottom-nav--", cut.Find("nav").ClassName ?? "");
        Assert.Empty(cut.FindAll($".{Css.Classes.BottomNav.Spacer}"));
    }

    [Theory]
    [InlineData(BottomNavPosition.Fixed, Css.Classes.BottomNav.Fixed)]
    [InlineData(BottomNavPosition.Sticky, Css.Classes.BottomNav.Sticky)]
    public void PinnedBar_CarriesItsModifierAndReservesItsHeight(BottomNavPosition position, string expected)
    {
        var cut = Render<FlareBottomNav>(p => p.Add(x => x.Position, position));

        Assert.Contains(expected, cut.Find("nav").ClassName ?? "");
        Assert.Single(cut.FindAll($".{Css.Classes.BottomNav.Spacer}"));
    }

    [Fact]
    public void ReserveSpaceFalse_DropsTheSpacerButKeepsThePin()
    {
        var cut = Render<FlareBottomNav>(p => p
            .Add(x => x.Position, BottomNavPosition.Fixed)
            .Add(x => x.ReserveSpace, false));

        Assert.Contains(Css.Classes.BottomNav.Fixed, cut.Find("nav").ClassName ?? "");
        Assert.Empty(cut.FindAll($".{Css.Classes.BottomNav.Spacer}"));
    }

    [Fact]
    public void TheSpacer_IsHiddenFromAssistiveTechnology()
    {
        var cut = Render<FlareBottomNav>(p => p.Add(x => x.Position, BottomNavPosition.Fixed));
        Assert.Equal("true", cut.Find($".{Css.Classes.BottomNav.Spacer}").GetAttribute("aria-hidden"));
    }
}
