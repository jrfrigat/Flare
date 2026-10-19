namespace Flare.Components.Tests;

/// <summary>
/// Initials split on any whitespace and take whole text elements: a repeated space or a tab must not
/// become an initial, and a surrogate pair or a combining mark must not be cut in half.
/// </summary>
public class FlareAvatarInitialsTests : FlareTestContext
{
    private string Initials(string? text)
    {
        var cut = Render<FlareAvatar>(p => p.Add(x => x.Text, text));
        return cut.Find($".{Css.Classes.Avatar.Initials}").TextContent;
    }

    [Fact]
    public void RepeatedSpaces_DoNotBecomeAnInitial()
    {
        Assert.Equal("JD", Initials("John  Doe"));
    }

    [Fact]
    public void OuterWhitespaceAndTabs_AreIgnored()
    {
        var name = " John" + (char)9 + "Doe ";
        Assert.Equal("JD", Initials(name));
    }

    [Fact]
    public void ASingleName_GivesOneInitial()
    {
        Assert.Equal("C", Initials("Cher"));
    }

    [Fact]
    public void Cyrillic_GivesUppercaseInitials()
    {
        var expected = char.ConvertFromUtf32(0x0416);
        Assert.Equal(expected, Initials(expected));
    }

    [Fact]
    public void ASurrogatePair_StaysWhole()
    {
        var emoji = char.ConvertFromUtf32(0x1F600);

        var initials = Initials(emoji + " Smith");

        Assert.Equal(emoji + "S", initials);
        Assert.Equal(0x1F600, char.ConvertToUtf32(initials, 0));
    }

    [Fact]
    public void ACombiningMark_StaysWithItsLetter()
    {
        var aWithDiaeresis = "A" + (char)0x0308;

        var initials = Initials(aWithDiaeresis + "nna Lee");

        Assert.Equal(aWithDiaeresis + "L", initials);
        Assert.Equal(3, initials.Length);
    }

    [Fact]
    public void AWhitespaceOnlyName_GivesTheQuestionMark()
    {
        Assert.Equal("?", Initials("   " + (char)9 + " "));
    }
}
