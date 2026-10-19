namespace Flare.Components.Tests;

/// <summary>
/// A card is activated by one stretched native control, and that control either follows a link or
/// toggles a selection - never both. The combination used to paint the selection classes while dropping
/// the handler and the checked state, so the parameter is now rejected where the caller can see it.
/// </summary>
public class FlareCardSelectableLinkTests : FlareTestContext
{
    [Fact]
    public void HrefAndSelectable_AreRejectedInsteadOfIgnoringTheSelection()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            Render<FlareCard>(p => p
                .Add(x => x.Href, "/details")
                .Add(x => x.Selectable, true)
                .AddChildContent("Card")));

        Assert.Contains("Href", ex.Message);
        Assert.Contains("Selectable", ex.Message);
    }

    [Fact]
    public void ALinkCard_HasNoSelectionSemantics()
    {
        var cut = Render<FlareCard>(p => p
            .Add(x => x.Href, "/details")
            .AddChildContent("Card"));

        var hit = cut.Find($"a.{Css.Classes.Card.Hit}");
        Assert.Equal("/details", hit.GetAttribute("href"));
        Assert.Null(hit.GetAttribute("role"));
        Assert.Null(hit.GetAttribute("aria-checked"));
        Assert.DoesNotContain(Css.Classes.Card.Selected, cut.Find($".{Css.Classes.Card.Root}").ClassName);
        Assert.DoesNotContain(Css.Classes.Card.Selectable, cut.Find($".{Css.Classes.Card.Root}").ClassName);
    }

    [Fact]
    public void ASelectableCard_IsNotALink()
    {
        var cut = Render<FlareCard>(p => p
            .Add(x => x.Selectable, true)
            .AddChildContent("Card"));

        Assert.Empty(cut.FindAll("a"));
        var hit = cut.Find($"button.{Css.Classes.Card.Hit}");
        Assert.Equal("checkbox", hit.GetAttribute("role"));
        Assert.Equal("false", hit.GetAttribute("aria-checked"));
    }
}
