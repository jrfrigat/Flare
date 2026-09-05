using Flare.Abstractions;
using Flare.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

// ------------------------------------------------------------------------------
// ISnackbarService options overload
// ------------------------------------------------------------------------------
public class SnackbarOptionsTests
{
    [Fact]
    public void Show_WithOptions_MapsAllFields()
    {
        var service = new SnackbarService();
        SnackbarMessage? captured = null;
        service.OnShow += m => captured = m;

        service.Show("Saved", new SnackbarOptions
        {
            Severity = SnackbarSeverity.Warning,
            DurationMs = 0,
            ShowClose = false,
            ShowProgress = true,
            CssClass = "my-snackbar",
            CloseAfterNavigation = true,
        });

        Assert.NotNull(captured);
        Assert.Equal("Saved", captured!.Text);
        Assert.Equal(SnackbarSeverity.Warning, captured.Severity);
        Assert.Equal(0, captured.DurationMs);
        Assert.False(captured.ShowClose);
        Assert.True(captured.ShowProgress);
        Assert.Equal("my-snackbar", captured.CssClass);
        Assert.True(captured.CloseAfterNavigation);
    }

    [Fact]
    public void Show_WithNullOptions_Throws()
    {
        var service = new SnackbarService();

        Assert.Throws<ArgumentNullException>(() => service.Show("x", (SnackbarOptions)null!));
    }
}
