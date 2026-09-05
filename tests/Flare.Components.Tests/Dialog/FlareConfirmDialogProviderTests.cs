using Flare.Infrastructure;
using Flare.Abstractions;
using Flare.Theming;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class FlareConfirmDialogProviderTests : FlareTestContext
{
    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareConfirmDialogProvider>(p => p
            .AddChildContent("<span id=\"child-of-confirm\">Content</span>"));

        Assert.NotEmpty(cut.FindAll("#child-of-confirm"));
    }

    [Fact]
    public void DialogNotVisibleInitially()
    {
        var cut = Render<FlareConfirmDialogProvider>();

        Assert.Empty(cut.FindAll($".{Css.Classes.Confirmdialog.Backdrop}"));
    }

    [Fact]
    public void ConfirmButtonNotVisibleInitially()
    {
        var cut = Render<FlareConfirmDialogProvider>();

        Assert.Empty(cut.FindAll($".{Css.Classes.Confirmdialog.BtnConfirm}"));
    }

    [Fact]
    public void CancelButtonNotVisibleInitially()
    {
        var cut = Render<FlareConfirmDialogProvider>();

        Assert.Empty(cut.FindAll($".{Css.Classes.Confirmdialog.BtnCancel}"));
    }

    [Fact]
    public void AfterConfirmAsync_DialogVisible()
    {
        var cut = Render<FlareConfirmDialogProvider>();
        var provider = cut.Instance;

        _ = provider.ConfirmAsync("Delete?", "Are you sure?");
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Confirmdialog.Backdrop}").Count > 0);

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Confirmdialog.Backdrop}"));
    }

    [Fact]
    public void AfterConfirmAsync_TitleRendered()
    {
        var cut = Render<FlareConfirmDialogProvider>();
        var provider = cut.Instance;

        _ = provider.ConfirmAsync("Delete item", "Cannot be undone.");
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Confirmdialog.Title}").Count > 0);

        Assert.Contains("Delete item", cut.Find($".{Css.Classes.Confirmdialog.Title}").TextContent);
    }

    [Fact]
    public void AfterConfirmAsync_MessageRendered()
    {
        var cut = Render<FlareConfirmDialogProvider>();
        var provider = cut.Instance;

        _ = provider.ConfirmAsync("Title", "Please confirm this action.");
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Confirmdialog.Body}").Count > 0);

        Assert.Contains("Please confirm this action.", cut.Find($".{Css.Classes.Confirmdialog.Body}").TextContent);
    }

    [Fact]
    public void ClickConfirm_DialogDismisses()
    {
        var cut = Render<FlareConfirmDialogProvider>();
        var provider = cut.Instance;

        _ = provider.ConfirmAsync("Sure?", "Yes or No");
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Confirmdialog.BtnConfirm}").Count > 0);

        cut.Find($".{Css.Classes.Confirmdialog.BtnConfirm}").Click();

        Assert.Empty(cut.FindAll($".{Css.Classes.Confirmdialog.Backdrop}"));
    }
}
