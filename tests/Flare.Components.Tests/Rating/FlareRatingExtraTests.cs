namespace Flare.Components.Tests;

public class FlareRatingExtraTests : FlareTestContext
{
    [Fact]
    public void ReadOnly_HasReadonlyClass()
    {
        var cut = Render<FlareRating>(p => p
            .Add(x => x.ReadOnly, true));

        Assert.Contains(Css.Classes.Rating.Readonly, cut.Find($".{Css.Classes.Rating.Root}").ClassName);
    }

    [Fact]
    public void ReadOnly_StarsHaveDisabledAttribute()
    {
        var cut = Render<FlareRating>(p => p
            .Add(x => x.ReadOnly, true));

        var stars = cut.FindAll($".{Css.Classes.Rating.Star}");
        Assert.All(stars, star => Assert.True(star.HasAttribute("disabled")));
    }
}
