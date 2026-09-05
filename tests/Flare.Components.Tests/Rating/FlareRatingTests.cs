namespace Flare.Components.Tests;

public class FlareRatingTests : FlareTestContext
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
