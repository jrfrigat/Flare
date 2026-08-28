using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests.Component;

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
        Assert.DoesNotContain("flare-card__actions--nowrap", cut.Find(".flare-card__actions").ClassName);
    }

    [Fact]
    public void WrapFalse_AddsTheNoWrapModifier()
    {
        var cut = Render<FlareCardActions>(p => p
            .Add(x => x.Wrap, false)
            .Add(x => x.ChildContent, TwoButtons));

        Assert.Contains("flare-card__actions--nowrap", cut.Find(".flare-card__actions").ClassName);
    }

    [Theory]
    [InlineData(CardActionsAlign.Start, null)]
    [InlineData(CardActionsAlign.Center, "flare-card__actions--center")]
    [InlineData(CardActionsAlign.End, "flare-card__actions--end")]
    [InlineData(CardActionsAlign.Between, "flare-card__actions--between")]
    [InlineData(CardActionsAlign.Stretch, "flare-card__actions--stretch")]
    public void Align_MapsToItsModifier(CardActionsAlign align, string? expected)
    {
        var cut = Render<FlareCardActions>(p => p
            .Add(x => x.Align, align)
            .Add(x => x.ChildContent, TwoButtons));

        var className = cut.Find(".flare-card__actions").ClassName ?? "";
        if (expected is null) Assert.DoesNotContain("flare-card__actions--", className);
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

        var className = cut.Find(".flare-card__actions").ClassName ?? "";
        Assert.Contains("flare-card__actions--vertical", className);
        Assert.Contains("flare-card__actions--full", className);
        Assert.Contains("flare-card__actions--reverse", className);
    }

    [Fact]
    public void StackBelow_MapsToTheContainerQueryModifier()
    {
        var cut = Render<FlareCardActions>(p => p
            .Add(x => x.StackBelow, CardActionsStack.Compact)
            .Add(x => x.ChildContent, TwoButtons));

        Assert.Contains("flare-card__actions--stack-compact", cut.Find(".flare-card__actions").ClassName);
    }

    [Fact]
    public void StackNever_MapsToNothing()
    {
        var cut = Render<FlareCardActions>(p => p
            .Add(x => x.StackBelow, CardActionsStack.Never)
            .Add(x => x.ChildContent, TwoButtons));

        Assert.DoesNotContain("stack-", cut.Find(".flare-card__actions").ClassName ?? "");
    }

    [Fact]
    public void Gap_TravelsAsTheCardsOwnLocalProperty()
    {
        var cut = Render<FlareCardActions>(p => p
            .Add(x => x.Gap, FlareSpacing.Large)
            .Add(x => x.ChildContent, TwoButtons));

        Assert.Contains("--_card-actions-gap:var(--flare-spacing-12)",
            cut.Find(".flare-card__actions").GetAttribute("style") ?? "");
    }

    // ---- FlareBottomNav ----------------------------------------------------------------------------

    [Fact]
    public void BottomNav_IsStaticByDefault_AndReservesNothing()
    {
        var cut = Render<FlareBottomNav>();
        Assert.DoesNotContain("flare-bottom-nav--", cut.Find("nav").ClassName ?? "");
        Assert.Empty(cut.FindAll(".flare-bottom-nav__spacer"));
    }

    [Theory]
    [InlineData(BottomNavPosition.Fixed, "flare-bottom-nav--fixed")]
    [InlineData(BottomNavPosition.Sticky, "flare-bottom-nav--sticky")]
    public void PinnedBar_CarriesItsModifierAndReservesItsHeight(BottomNavPosition position, string expected)
    {
        var cut = Render<FlareBottomNav>(p => p.Add(x => x.Position, position));

        Assert.Contains(expected, cut.Find("nav").ClassName ?? "");
        Assert.Single(cut.FindAll(".flare-bottom-nav__spacer"));
    }

    [Fact]
    public void ReserveSpaceFalse_DropsTheSpacerButKeepsThePin()
    {
        var cut = Render<FlareBottomNav>(p => p
            .Add(x => x.Position, BottomNavPosition.Fixed)
            .Add(x => x.ReserveSpace, false));

        Assert.Contains("flare-bottom-nav--fixed", cut.Find("nav").ClassName ?? "");
        Assert.Empty(cut.FindAll(".flare-bottom-nav__spacer"));
    }

    [Fact]
    public void TheSpacer_IsHiddenFromAssistiveTechnology()
    {
        var cut = Render<FlareBottomNav>(p => p.Add(x => x.Position, BottomNavPosition.Fixed));
        Assert.Equal("true", cut.Find(".flare-bottom-nav__spacer").GetAttribute("aria-hidden"));
    }
}
