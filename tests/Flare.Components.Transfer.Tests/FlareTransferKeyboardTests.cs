using AngleSharp.Dom;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareTransferKeyboardTests : FlareTestContext
{
    private static readonly string[] Source = ["alpha", "beta", "gamma"];
    private static readonly string[] Target = ["delta"];

    private static string List => "[role=listbox]";
    private static string ActiveItem => $".{Css.Classes.Transfer.ItemActive}";

    private IRenderedComponent<FlareTransfer<string>> RenderTransfer(
        List<IReadOnlyList<string>>? captured = null)
        => Render<FlareTransfer<string>>(p => p
            .Add(x => x.SourceItems, Source)
            .Add(x => x.TargetItems, Target)
            .Add(x => x.TargetItemsChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(
                captured ?? new List<IReadOnlyList<string>>(), v => captured?.Add(v))));

    private static void FocusList(IRenderedComponent<FlareTransfer<string>> cut, int index)
        => cut.FindAll(List)[index].TriggerEvent("onfocus", new FocusEventArgs());

    private static void Key(IRenderedComponent<FlareTransfer<string>> cut, int list, string key)
        => cut.FindAll(List)[list].KeyDown(new KeyboardEventArgs { Key = key });

    private static IElement Row(IRenderedComponent<FlareTransfer<string>> cut, int list, int index)
        => cut.FindAll(List)[list].QuerySelectorAll("[role=option]")[index];

    [Fact]
    public void BothLists_AreNamedListboxesWithOneTabStop()
    {
        var cut = RenderTransfer();

        var lists = cut.FindAll(List);
        Assert.Equal(2, lists.Count);
        Assert.Equal("Available", lists[0].GetAttribute("aria-label"));
        Assert.Equal("Selected", lists[1].GetAttribute("aria-label"));
        Assert.All(lists, l => Assert.Equal("0", l.GetAttribute("tabindex")));
        Assert.All(lists, l => Assert.Equal("true", l.GetAttribute("aria-multiselectable")));
        Assert.All(cut.FindAll("[role=option]"), o => Assert.Null(o.GetAttribute("tabindex")));
    }

    [Fact]
    public void SpaceTogglesTheActiveSourceRow()
    {
        var cut = RenderTransfer();
        FocusList(cut, 0);

        Key(cut, 0, " ");

        Assert.Equal("true", Row(cut, 0, 0).GetAttribute("aria-selected"));
        Assert.Contains(Css.Classes.Transfer.ItemSelected, Row(cut, 0, 0).ClassName ?? string.Empty);
    }

    [Fact]
    public void ArrowsMoveTheActiveRow()
    {
        var cut = RenderTransfer();
        FocusList(cut, 0);

        Key(cut, 0, "ArrowDown");
        Assert.Equal("beta", cut.Find(ActiveItem).TextContent.Trim());

        Key(cut, 0, "End");
        Assert.Equal("gamma", cut.Find(ActiveItem).TextContent.Trim());

        Key(cut, 0, "Home");
        Assert.Equal("alpha", cut.Find(ActiveItem).TextContent.Trim());
    }

    [Fact]
    public void EnterTogglesTheActiveTargetRow()
    {
        var cut = RenderTransfer();
        FocusList(cut, 1);

        Key(cut, 1, "Enter");

        Assert.Equal("true", Row(cut, 1, 0).GetAttribute("aria-selected"));
    }

    [Fact]
    public void KeyboardSelection_ThenTheMoveButton_MovesTheItem()
    {
        var captured = new List<IReadOnlyList<string>>();
        var cut = RenderTransfer(captured);
        FocusList(cut, 0);
        Key(cut, 0, " ");
        Key(cut, 0, "ArrowDown");
        Key(cut, 0, " ");

        cut.FindAll("button")[0].Click();

        Assert.Equal(["delta", "alpha", "beta"], captured[^1]);
    }

    [Fact]
    public void APointerClick_AlsoMovesTheKeyboardCursor()
    {
        var cut = RenderTransfer();

        Row(cut, 0, 1).Click();

        Assert.Contains(Css.Classes.Transfer.ItemActive, Row(cut, 0, 1).ClassName ?? string.Empty);
        Assert.Equal("true", Row(cut, 0, 1).GetAttribute("aria-selected"));
    }

    [Fact]
    public void AnEmptyList_IgnoresTheKeyboard()
    {
        var cut = Render<FlareTransfer<string>>(p => p.Add(x => x.SourceItems, []));
        FocusList(cut, 0);

        Key(cut, 0, "ArrowDown");
        Key(cut, 0, " ");
        Key(cut, 0, "Enter");

        Assert.Empty(cut.FindAll(ActiveItem));
    }
}
