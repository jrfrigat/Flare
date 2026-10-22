using Flare.Components.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

/// <summary>
/// Auto mode is the default: panes on a wide viewport, tabs on a narrow one. The choice follows a
/// breakpoint change after mounting, not only the viewport the picker opened with.
/// </summary>
public sealed class DateTimePickerAutoModeTests : FlareTestContext
{
    private readonly BrowserViewportService _viewport;

    public DateTimePickerAutoModeTests()
    {
        _viewport = new BrowserViewportService(new RecordingJsRuntime(JSInterop.JSRuntime));
        Services.AddScoped<IBrowserViewportService>(_ => _viewport);
    }

    [Fact]
    public async Task Auto_SwitchesBetweenPanelsAndTabsWithTheViewport()
    {
        var cut = Render<FlareDateTimePicker>(p => p.Add(x => x.Mode, DateTimeVariant.Auto));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();

        // The test context reports md, so the default Auto layout is panes without a tab list.
        Assert.Empty(cut.FindAll("[role=tablist]"));

        await _viewport.OnViewportResized(400, 800);
        cut.WaitForAssertion(() => Assert.NotEmpty(cut.FindAll("[role=tablist]")));

        await _viewport.OnViewportResized(1200, 800);
        cut.WaitForAssertion(() => Assert.Empty(cut.FindAll("[role=tablist]")));
    }
}
