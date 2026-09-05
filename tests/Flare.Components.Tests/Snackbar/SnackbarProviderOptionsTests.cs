using Flare.Abstractions;
using Flare.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

// ------------------------------------------------------------------------------
// FlareSnackbarProvider per-message CssClass + CloseAfterNavigation
// ------------------------------------------------------------------------------
public class SnackbarProviderOptionsTests : FlareTestContext
{
    public SnackbarProviderOptionsTests()
    {
        Services.AddSingleton<ISnackbarService, SnackbarService>();
    }

    [Fact]
    public void CssClass_AppliedToSnackbarElement()
    {
        var cut = Render<FlareSnackbarProvider>();
        var service = Services.GetRequiredService<ISnackbarService>();

        service.Show("Hi", new SnackbarOptions { DurationMs = 0, CssClass = "promo-snackbar" });
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count > 0);

        Assert.NotEmpty(cut.FindAll(".promo-snackbar"));
    }

    [Fact]
    public void CloseAfterNavigation_DismissesOnNavigate()
    {
        var cut = Render<FlareSnackbarProvider>();
        var service = Services.GetRequiredService<ISnackbarService>();
        var nav = Services.GetRequiredService<NavigationManager>();

        service.Show("Hi", new SnackbarOptions { DurationMs = 0, CloseAfterNavigation = true });
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count > 0);

        nav.NavigateTo("/other");

        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count == 0);
        Assert.Empty(cut.FindAll($".{Css.Classes.Snackbar.Root}"));
    }
}
