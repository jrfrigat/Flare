namespace Flare.Components.Tests.Component;

// ------------------------------------------------------------------------------
// FlareAvatar  (6 tests from Wave3)
// ------------------------------------------------------------------------------

public class C_FlareAvatarTests : FlareTestContext
{

    [Fact]
    public void RendersInitialsWhenTextProvided()
    {
        var cut = Render<FlareAvatar>(p => p
            .Add(x => x.Text, "John Doe"));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Avatar.Initials}"));
    }

    [Fact]
    public void RendersImgWhenSrcProvided()
    {
        var cut = Render<FlareAvatar>(p => p
            .Add(x => x.Src, "https://example.com/avatar.png"));

        Assert.NotEmpty(cut.FindAll($"img.{Css.Classes.Avatar.Img}"));
    }

    [Fact]
    public void SizeSmall_HasSmallClass()
    {
        var cut = Render<FlareAvatar>(p => p
            .Add(x => x.Size, AvatarSize.Sm));

        Assert.Contains(Css.Classes.Avatar.Sm, cut.Find($".{Css.Classes.Avatar.Root}").ClassName);
    }

    [Fact]
    public void SizeLarge_HasLargeClass()
    {
        var cut = Render<FlareAvatar>(p => p
            .Add(x => x.Size, AvatarSize.Lg));

        Assert.Contains(Css.Classes.Avatar.Lg, cut.Find($".{Css.Classes.Avatar.Root}").ClassName);
    }

    [Fact]
    public void ShapeSquare_HasSquareClass()
    {
        var cut = Render<FlareAvatar>(p => p
            .Add(x => x.Shape, AvatarShape.Square));

        Assert.Contains(Css.Classes.Avatar.Square, cut.Find($".{Css.Classes.Avatar.Root}").ClassName);
    }
}

// ------------------------------------------------------------------------------
// FlareAvatarGroup  (4 tests from Wave3)
// ------------------------------------------------------------------------------

public class C_FlareAvatarGroupTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareAvatarGroup>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Avatar.Group}"));
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareAvatarGroup>(p => p
            .AddChildContent("<div class=\"child-avatar\">A</div>"));

        Assert.NotEmpty(cut.FindAll(".child-avatar"));
    }

    [Fact]
    public void SpacingAppliedAsStyle()
    {
        var cut = Render<FlareAvatarGroup>(p => p
            .Add(x => x.Spacing, "-1rem"));

        var style = cut.Find($".{Css.Classes.Avatar.Group}").GetAttribute("style") ?? string.Empty;
        Assert.Contains("-1rem", style);
    }

    [Fact]
    public void DefaultMaxIsFive()
    {
        var cut = Render<FlareAvatarGroup>(p => p
            .Add(x => x.Max, 5));

        Assert.Empty(cut.FindAll($".{Css.Classes.Avatar.GroupOverflow}"));
    }
}

// ------------------------------------------------------------------------------
// FlareChip single  (5 tests from Wave3)
// ------------------------------------------------------------------------------

public class C_FlareChipTests : FlareTestContext
{

    [Fact]
    public void Disabled_MarksTheChipAndLeavesTheTabOrder()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Archived")
            .Add(x => x.Disabled, true));

        var chip = cut.Find($".{Css.Classes.Chip.Root}");
        Assert.Contains(Css.Classes.Chip.Disabled, chip.ClassName);
        Assert.Equal("true", chip.GetAttribute("aria-disabled"));
        Assert.Equal("-1", chip.GetAttribute("tabindex"));
    }

    [Fact]
    public void Enabled_ChipCarriesNoDisabledMarkers()
    {
        var cut = Render<FlareChip>(p => p.Add(x => x.Label, "Active"));

        var chip = cut.Find($".{Css.Classes.Chip.Root}");
        Assert.DoesNotContain(Css.Classes.Chip.Disabled, chip.ClassName);
        Assert.Null(chip.GetAttribute("aria-disabled"));
        Assert.Equal("0", chip.GetAttribute("tabindex"));
    }

    [Fact]
    public void Disabled_SwallowsClickAndKeyboardActivation()
    {
        // pointer-events:none keeps a real mouse away, but a programmatic or
        // assistive-technology activation still reaches the handler.
        int clicks = 0, selections = 0;
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Archived")
            .Add(x => x.Disabled, true)
            .Add(x => x.OnClick, () => clicks++)
            .Add(x => x.SelectedChanged, (bool _) => selections++));

        cut.Find($".{Css.Classes.Chip.Root}").Click();
        cut.Find($".{Css.Classes.Chip.Root}").KeyDown(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "Enter" });

        Assert.Equal(0, clicks);
        Assert.Equal(0, selections);
    }

    [Fact]
    public void Disabled_SwallowsClose()
    {
        int closes = 0;
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Archived")
            .Add(x => x.Closeable, true)
            .Add(x => x.Disabled, true)
            .Add(x => x.OnClose, () => closes++));

        var close = cut.Find($".{Css.Classes.Chip.Close}");
        Assert.True(close.HasAttribute("disabled"));

        close.Click();

        Assert.Equal(0, closes);
    }

    [Fact]
    public void Disabled_ChipInAGroupDoesNotChangeTheSelection()
    {
        IReadOnlyCollection<string>? selected = null;
        var cut = Render<FlareChipGroup>(p => p
            .Add(x => x.SelectedValuesChanged, (IReadOnlyCollection<string> v) => selected = v)
            .AddChildContent<FlareChip>(c => c
                .Add(x => x.Label, "Archived")
                .Add(x => x.Value, "archived")
                .Add(x => x.Disabled, true)));

        cut.Find($".{Css.Classes.Chip.Root}").Click();

        Assert.Null(selected);
    }
}

// ------------------------------------------------------------------------------
// FlareBadge  (5 tests from Wave3)
// ------------------------------------------------------------------------------

public class C_FlareBadgeTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareBadge>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Badge.Root}"));
    }

    [Fact]
    public void RendersCount()
    {
        var cut = Render<FlareBadge>(p => p
            .Add(x => x.Count, 7));

        Assert.Contains("7", cut.Find($".{Css.Classes.Badge.Indicator}").TextContent);
    }

    [Fact]
    public void MaxCount_ShowsPlusNotation()
    {
        var cut = Render<FlareBadge>(p => p
            .Add(x => x.Count, 150)
            .Add(x => x.Max, 99));

        Assert.Contains("99+", cut.Find($".{Css.Classes.Badge.Indicator}").TextContent);
    }

    [Fact]
    public void DotVariant_RendersIndicatorWithDotClass()
    {
        var cut = Render<FlareBadge>(p => p
            .Add(x => x.Dot, true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Badge.IndicatorDot}"));
    }

    [Fact]
    public void WrapsChildContent()
    {
        var cut = Render<FlareBadge>(p => p
            .AddChildContent("<span class=\"wrapped-item\">Icon</span>"));

        Assert.NotEmpty(cut.FindAll(".wrapped-item"));
    }

    [Fact]
    public void Text_OverridesCountLabel()
    {
        var cut = Render<FlareBadge>(p => p
            .Add(x => x.Text, "NEW")
            .Add(x => x.Count, 5));

        Assert.Contains("NEW", cut.Find($".{Css.Classes.Badge.Indicator}").TextContent);
    }

    [Fact]
    public void Standalone_AddsModifierClass()
    {
        var cut = Render<FlareBadge>(p => p
            .Add(x => x.Text, "Beta")
            .Add(x => x.Standalone, true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Badge.Standalone}"));
    }

    [Fact]
    public void WithoutAnchor_IsStandaloneByDefault()
    {
        var cut = Render<FlareBadge>(p => p
            .Add(x => x.Text, "Tag"));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Badge.Standalone}"));
    }
}

// ------------------------------------------------------------------------------
// FlarePaper  (4 tests from Wave3)
// ------------------------------------------------------------------------------

public class C_FlarePaperTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlarePaper>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Paper.Root}"));
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlarePaper>(p => p
            .AddChildContent("<p class=\"paper-content\">Hello</p>"));

        Assert.NotEmpty(cut.FindAll(".paper-content"));
    }

    [Fact]
    public void ElevationClass_AppliedCorrectly()
    {
        var cut = Render<FlarePaper>(p => p
            .Add(x => x.Elevation, 3));

        Assert.Contains(Css.Classes.Paper.Elevation3, cut.Find($".{Css.Classes.Paper.Root}").ClassName);
    }

    [Fact]
    public void SquarePaper_HasSquareClass()
    {
        var cut = Render<FlarePaper>(p => p
            .Add(x => x.Square, true));

        Assert.Contains(Css.Classes.Paper.Square, cut.Find($".{Css.Classes.Paper.Root}").ClassName);
    }
}

// ------------------------------------------------------------------------------
// FlareTimeline  (5 tests from Wave3)
// ------------------------------------------------------------------------------

public class C_FlareTimelineTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareTimeline>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Timeline.Root}"));
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareTimeline>(p => p
            .AddChildContent("<div class=\"custom-item\">Item</div>"));

        Assert.NotEmpty(cut.FindAll(".custom-item"));
    }

    [Fact]
    public void AlignRight_HasRightClass()
    {
        var cut = Render<FlareTimeline>(p => p
            .Add(x => x.Align, TimelineAlign.Right));

        Assert.Contains(Css.Classes.Timeline.Right, cut.Find($".{Css.Classes.Timeline.Root}").ClassName);
    }

    [Fact]
    public void AlignAlternate_HasAlternateClass()
    {
        var cut = Render<FlareTimeline>(p => p
            .Add(x => x.Align, TimelineAlign.Alternate));

        Assert.Contains(Css.Classes.Timeline.Alternate, cut.Find($".{Css.Classes.Timeline.Root}").ClassName);
    }

    [Fact]
    public void AlignLeft_HasNoAlignClass()
    {
        var cut = Render<FlareTimeline>(p => p
            .Add(x => x.Align, TimelineAlign.Left));

        var className = cut.Find($".{Css.Classes.Timeline.Root}").ClassName ?? string.Empty;
        Assert.DoesNotContain(Css.Classes.Timeline.Right, className);
        Assert.DoesNotContain(Css.Classes.Timeline.Alternate, className);
    }
}

// ------------------------------------------------------------------------------
// FlareRating  (6 tests from Wave3)
// ------------------------------------------------------------------------------

public class C_FlareRatingTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareRating>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Rating.Root}"));
    }

    [Fact]
    public void RendersDefaultFiveStars()
    {
        var cut = Render<FlareRating>();

        Assert.Equal(5, cut.FindAll($".{Css.Classes.Rating.Star}").Count);
    }

    [Fact]
    public void CustomMaxRendersCorrectStarCount()
    {
        var cut = Render<FlareRating>(p => p
            .Add(x => x.Max, 3));

        Assert.Equal(3, cut.FindAll($".{Css.Classes.Rating.Star}").Count);
    }

    [Fact]
    public void DisabledState_StarsHaveDisabledAttribute()
    {
        var cut = Render<FlareRating>(p => p
            .Add(x => x.Disabled, true));

        var stars = cut.FindAll($".{Css.Classes.Rating.Star}");
        Assert.All(stars, star => Assert.True(star.HasAttribute("disabled")));
    }

    [Fact]
    public void DisabledClass_AppliedWhenDisabled()
    {
        var cut = Render<FlareRating>(p => p
            .Add(x => x.Disabled, true));

        Assert.Contains(Css.Classes.Rating.Disabled, cut.Find($".{Css.Classes.Rating.Root}").ClassName);
    }

    [Fact]
    public void ValueParam_FilledStarsReflectValue()
    {
        var cut = Render<FlareRating>(p => p
            .Add(x => x.Value, 3)
            .Add(x => x.Max, 5));

        var filledStars = cut.FindAll($".{Css.Classes.Rating.Filled}");
        Assert.Equal(3, filledStars.Count);
    }
}
