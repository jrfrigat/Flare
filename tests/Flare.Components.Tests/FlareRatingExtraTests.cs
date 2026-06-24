namespace Flare.Components.Tests;

public class FlareRatingExtraTests : FlareTestContext
{
    [Fact]
    public void ReadOnly_HasReadonlyClass()
    {
        var cut = Render<FlareRating>(p => p
            .Add(x => x.ReadOnly, true));

        Assert.Contains("flare-rating--readonly", cut.Find(".flare-rating").ClassName);
    }

    [Fact]
    public void ReadOnly_StarsHaveDisabledAttribute()
    {
        var cut = Render<FlareRating>(p => p
            .Add(x => x.ReadOnly, true));

        var stars = cut.FindAll(".flare-rating__star");
        Assert.All(stars, star => Assert.True(star.HasAttribute("disabled")));
    }
}
