using Flare.Infrastructure;
using Flare.Abstractions;
using Flare.Theming;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class FlareSnackbarProviderTests : FlareTestContext
{
    public FlareSnackbarProviderTests()
    {
        Services.AddSingleton<ISnackbarService, SnackbarService>();
    }

    [Fact]
    public void HostClassStyleAndAttributesStayOnTheProviderAndNotOnEachToast()
    {
        var cut = Render<FlareSnackbarProvider>(p => p
            .Add(x => x.Class, "custom-host")
            .Add(x => x.Style, "color:red")
            .AddUnmatched("data-testid", "toasts"));

        var host = cut.Find($".{Css.Classes.Snackbar.Provider}");
        Assert.Contains("custom-host", host.ClassName);
        Assert.Equal("color:red", host.GetAttribute("style"));
        Assert.Equal("toasts", host.GetAttribute("data-testid"));

        var service = Services.GetRequiredService<ISnackbarService>();
        service.Show("Saved", new SnackbarOptions { CssClass = "per-message" });
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count > 0);

        var toast = cut.Find($".{Css.Classes.Snackbar.Root}");
        Assert.Contains("per-message", toast.ClassName);
        Assert.DoesNotContain("custom-host", toast.ClassName);
    }

    [Fact]
    public void RendersProviderDiv()
    {
        var cut = Render<FlareSnackbarProvider>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Snackbar.Provider}"));
    }

    [Fact]
    public void NonErrorMessage_HasPoliteStatusRole()
    {
        var cut = Render<FlareSnackbarProvider>();
        var service = Services.GetRequiredService<ISnackbarService>();

        service.Show("Saved", SnackbarSeverity.Success);
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count > 0);

        // Live semantics live on the toast, not the container: a stack mixes politeness levels.
        Assert.Equal("status", cut.Find($".{Css.Classes.Snackbar.Root}").GetAttribute("role"));
    }

    [Fact]
    public void ErrorMessage_HasAssertiveAlertRole()
    {
        var cut = Render<FlareSnackbarProvider>();
        var service = Services.GetRequiredService<ISnackbarService>();

        service.Show("Boom", SnackbarSeverity.Error);
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Error}").Count > 0);

        // An error interrupts the screen reader; role="alert" is an assertive live region.
        Assert.Equal("alert", cut.Find($".{Css.Classes.Snackbar.Error}").GetAttribute("role"));
    }

    // The severity class used to be assembled from the enum name, and Normal has no accent rule at
    // all - so the neutral toast carried flare-snackbar--normal, a class the stylesheet never defines.
    [Fact]
    public void NormalSeverity_CarriesNoAccentClass()
    {
        var cut = Render<FlareSnackbarProvider>();
        var service = Services.GetRequiredService<ISnackbarService>();

        service.Show("Just so you know", SnackbarSeverity.Normal);
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count > 0);

        Assert.DoesNotContain("flare-snackbar--normal", cut.Markup);
    }

    [Fact]
    public void NoMessagesInitially_NoSnackbarDivs()
    {
        var cut = Render<FlareSnackbarProvider>();

        Assert.Empty(cut.FindAll($".{Css.Classes.Snackbar.Root}"));
    }

    [Fact]
    public void ShowMessage_SnackbarDivAppears()
    {
        var cut = Render<FlareSnackbarProvider>();
        var service = Services.GetRequiredService<ISnackbarService>();

        service.Show("Hello Snackbar");
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count > 0);

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Snackbar.Root}"));
    }

    [Fact]
    public void ShowMessage_TextIsRendered()
    {
        var cut = Render<FlareSnackbarProvider>();
        var service = Services.GetRequiredService<ISnackbarService>();

        service.Show("Important message");
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count > 0);

        Assert.Contains("Important message", cut.Markup);
    }

    [Fact]
    public void ShowErrorMessage_HasErrorClass()
    {
        var cut = Render<FlareSnackbarProvider>();
        var service = Services.GetRequiredService<ISnackbarService>();

        service.Show("Error occurred", SnackbarSeverity.Error);
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Error}").Count > 0);

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Snackbar.Error}"));
    }
}
