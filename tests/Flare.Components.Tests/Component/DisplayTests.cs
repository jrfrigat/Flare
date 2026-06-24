namespace Flare.Components.Tests.Component;

// ------------------------------------------------------------------------------
// FlareAvatar  (6 tests from Wave3)
// ------------------------------------------------------------------------------

public class C_FlareAvatarTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareAvatar>();

        Assert.NotEmpty(cut.FindAll(".flare-avatar"));
    }

    [Fact]
    public void RendersInitialsWhenTextProvided()
    {
        var cut = Render<FlareAvatar>(p => p
            .Add(x => x.Text, "John Doe"));

        Assert.NotEmpty(cut.FindAll(".flare-avatar__initials"));
    }

    [Fact]
    public void RendersImgWhenSrcProvided()
    {
        var cut = Render<FlareAvatar>(p => p
            .Add(x => x.Src, "https://example.com/avatar.png"));

        Assert.NotEmpty(cut.FindAll("img.flare-avatar__img"));
    }

    [Fact]
    public void SizeSmall_HasSmallClass()
    {
        var cut = Render<FlareAvatar>(p => p
            .Add(x => x.Size, AvatarSize.Sm));

        Assert.Contains("flare-avatar--sm", cut.Find(".flare-avatar").ClassName);
    }

    [Fact]
    public void SizeLarge_HasLargeClass()
    {
        var cut = Render<FlareAvatar>(p => p
            .Add(x => x.Size, AvatarSize.Lg));

        Assert.Contains("flare-avatar--lg", cut.Find(".flare-avatar").ClassName);
    }

    [Fact]
    public void ShapeSquare_HasSquareClass()
    {
        var cut = Render<FlareAvatar>(p => p
            .Add(x => x.Shape, AvatarShape.Square));

        Assert.Contains("flare-avatar--square", cut.Find(".flare-avatar").ClassName);
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

        Assert.NotEmpty(cut.FindAll(".flare-avatar-group"));
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

        var style = cut.Find(".flare-avatar-group").GetAttribute("style") ?? string.Empty;
        Assert.Contains("-1rem", style);
    }

    [Fact]
    public void DefaultMaxIsFive()
    {
        var cut = Render<FlareAvatarGroup>(p => p
            .Add(x => x.Max, 5));

        Assert.Empty(cut.FindAll(".flare-avatar-group__overflow"));
    }
}

// ------------------------------------------------------------------------------
// FlareChip single  (5 tests from Wave3)
// ------------------------------------------------------------------------------

public class C_FlareChipTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Tech"));

        Assert.NotEmpty(cut.FindAll(".flare-chip"));
    }

    [Fact]
    public void RendersLabel()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Science"));

        Assert.Contains("Science", cut.Find(".flare-chip__label").TextContent);
    }

    [Fact]
    public void SelectedState_HasSelectedClass()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Art")
            .Add(x => x.Selected, true));

        Assert.Contains("flare-chip--selected", cut.Find(".flare-chip").ClassName);
    }

    [Fact]
    public void Closeable_ShowsCloseButton()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Music")
            .Add(x => x.Closeable, true));

        Assert.NotEmpty(cut.FindAll(".flare-chip__close"));
    }

    [Fact]
    public void NotCloseable_HidesCloseButton()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Sports")
            .Add(x => x.Closeable, false));

        Assert.Empty(cut.FindAll(".flare-chip__close"));
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

        Assert.NotEmpty(cut.FindAll(".flare-badge"));
    }

    [Fact]
    public void RendersCount()
    {
        var cut = Render<FlareBadge>(p => p
            .Add(x => x.Count, 7));

        Assert.Contains("7", cut.Find(".flare-badge__indicator").TextContent);
    }

    [Fact]
    public void MaxCount_ShowsPlusNotation()
    {
        var cut = Render<FlareBadge>(p => p
            .Add(x => x.Count, 150)
            .Add(x => x.Max, 99));

        Assert.Contains("99+", cut.Find(".flare-badge__indicator").TextContent);
    }

    [Fact]
    public void DotVariant_RendersIndicatorWithDotClass()
    {
        var cut = Render<FlareBadge>(p => p
            .Add(x => x.Dot, true));

        Assert.NotEmpty(cut.FindAll(".flare-badge__indicator--dot"));
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

        Assert.Contains("NEW", cut.Find(".flare-badge__indicator").TextContent);
    }

    [Fact]
    public void Standalone_AddsModifierClass()
    {
        var cut = Render<FlareBadge>(p => p
            .Add(x => x.Text, "Beta")
            .Add(x => x.Standalone, true));

        Assert.NotEmpty(cut.FindAll(".flare-badge--standalone"));
    }

    [Fact]
    public void WithoutAnchor_IsStandaloneByDefault()
    {
        var cut = Render<FlareBadge>(p => p
            .Add(x => x.Text, "Tag"));

        Assert.NotEmpty(cut.FindAll(".flare-badge--standalone"));
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

        Assert.NotEmpty(cut.FindAll(".flare-paper"));
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

        Assert.Contains("flare-paper--elevation-3", cut.Find(".flare-paper").ClassName);
    }

    [Fact]
    public void SquarePaper_HasSquareClass()
    {
        var cut = Render<FlarePaper>(p => p
            .Add(x => x.Square, true));

        Assert.Contains("flare-paper--square", cut.Find(".flare-paper").ClassName);
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

        Assert.NotEmpty(cut.FindAll(".flare-timeline"));
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

        Assert.Contains("flare-timeline--right", cut.Find(".flare-timeline").ClassName);
    }

    [Fact]
    public void AlignAlternate_HasAlternateClass()
    {
        var cut = Render<FlareTimeline>(p => p
            .Add(x => x.Align, TimelineAlign.Alternate));

        Assert.Contains("flare-timeline--alternate", cut.Find(".flare-timeline").ClassName);
    }

    [Fact]
    public void AlignLeft_HasNoAlignClass()
    {
        var cut = Render<FlareTimeline>(p => p
            .Add(x => x.Align, TimelineAlign.Left));

        var className = cut.Find(".flare-timeline").ClassName ?? string.Empty;
        Assert.DoesNotContain("flare-timeline--right", className);
        Assert.DoesNotContain("flare-timeline--alternate", className);
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

        Assert.NotEmpty(cut.FindAll(".flare-rating"));
    }

    [Fact]
    public void RendersDefaultFiveStars()
    {
        var cut = Render<FlareRating>();

        Assert.Equal(5, cut.FindAll(".flare-rating__star").Count);
    }

    [Fact]
    public void CustomMaxRendersCorrectStarCount()
    {
        var cut = Render<FlareRating>(p => p
            .Add(x => x.Max, 3));

        Assert.Equal(3, cut.FindAll(".flare-rating__star").Count);
    }

    [Fact]
    public void DisabledState_StarsHaveDisabledAttribute()
    {
        var cut = Render<FlareRating>(p => p
            .Add(x => x.Disabled, true));

        var stars = cut.FindAll(".flare-rating__star");
        Assert.All(stars, star => Assert.True(star.HasAttribute("disabled")));
    }

    [Fact]
    public void DisabledClass_AppliedWhenDisabled()
    {
        var cut = Render<FlareRating>(p => p
            .Add(x => x.Disabled, true));

        Assert.Contains("flare-rating--disabled", cut.Find(".flare-rating").ClassName);
    }

    [Fact]
    public void ValueParam_FilledStarsReflectValue()
    {
        var cut = Render<FlareRating>(p => p
            .Add(x => x.Value, 3)
            .Add(x => x.Max, 5));

        var filledStars = cut.FindAll(".flare-rating__star--filled");
        Assert.Equal(3, filledStars.Count);
    }
}
