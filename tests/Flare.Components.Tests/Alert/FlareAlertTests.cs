using Flare.Infrastructure;
using Flare.Abstractions;
using Flare.Theming;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class FlareAlertTests : FlareTestContext
{
    [Fact]
    public void RendersRootDiv()
    {
        var cut = Render<FlareAlert>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Alert.Root}"));
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareAlert>(p => p
            .AddChildContent("Alert body text"));

        Assert.Contains("Alert body text", cut.Find($".{Css.Classes.Alert.Body}").TextContent);
    }

    [Fact]
    public void RendersTitle()
    {
        var cut = Render<FlareAlert>(p => p
            .Add(x => x.Title, "Important!"));

        Assert.Contains("Important!", cut.Find($".{Css.Classes.Alert.Title}").TextContent);
    }

    [Fact]
    public void SeverityInfo_HasInfoClass()
    {
        var cut = Render<FlareAlert>(p => p
            .Add(x => x.Severity, AlertSeverity.Info));

        Assert.Contains(Css.Classes.Alert.Info, cut.Find($".{Css.Classes.Alert.Root}").ClassName);
    }

    [Fact]
    public void SeveritySuccess_HasSuccessClass()
    {
        var cut = Render<FlareAlert>(p => p
            .Add(x => x.Severity, AlertSeverity.Success));

        Assert.Contains(Css.Classes.Alert.Success, cut.Find($".{Css.Classes.Alert.Root}").ClassName);
    }

    [Fact]
    public void SeverityWarning_HasWarningClass()
    {
        var cut = Render<FlareAlert>(p => p
            .Add(x => x.Severity, AlertSeverity.Warning));

        Assert.Contains(Css.Classes.Alert.Warning, cut.Find($".{Css.Classes.Alert.Root}").ClassName);
    }

    [Fact]
    public void SeverityError_HasErrorClass()
    {
        var cut = Render<FlareAlert>(p => p
            .Add(x => x.Severity, AlertSeverity.Error));

        Assert.Contains(Css.Classes.Alert.Error, cut.Find($".{Css.Classes.Alert.Root}").ClassName);
    }
}

// ------------------------------------------------------------------------------
// FlareSnackbarProvider  (6 tests from Wave5)
// ------------------------------------------------------------------------------
