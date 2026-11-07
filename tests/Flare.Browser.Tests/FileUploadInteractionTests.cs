using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Flare.Browser.Tests;

public sealed class FileUploadInteractionTests(BrowserFixture fixture) : IClassFixture<BrowserFixture>
{
    private readonly List<string> _errors = [];
    [Fact]
    public async Task RealChooserTransfersBytesAndCanSelectTheSameFileAgain()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var input = page.Locator("#upload-zone input[type=file]");
        for (var i = 1; i <= 2; i++)
        {
            var chooser = await page.RunAndWaitForFileChooserAsync(() => input.ClickAsync());
            await chooser.SetFilesAsync(Payload("same.txt", 12000));
            await Expect(page.Locator("#upload-zone ." + Css.Classes.FileUpload.FileCompleted)).ToHaveCountAsync(1);
            await Expect(page.Locator("#bytes")).ToHaveTextAsync((12000 * i).ToString());
            await Expect(page.Locator("#selections")).ToHaveTextAsync(i.ToString());
        }
        for (var i = 3; i <= 4; i++)
        {
            var chooser = await page.RunAndWaitForFileChooserAsync(() => page.Locator("#upload-button label").ClickAsync());
            await chooser.SetFilesAsync(Payload("same.txt", 12000));
            await Expect(page.Locator("#selections")).ToHaveTextAsync(i.ToString());
        }
        Assert.Empty(_errors);
    }

    [Fact]
    public async Task TrustedDropTransfersFilesAndProtectsPaddingDisabledZonesAndLists()
    {
        var file = Path.Combine(Path.GetTempPath(), "flare-drop-" + Guid.NewGuid().ToString("N") + ".txt");
        var second = file + ".second.txt";
        await File.WriteAllTextAsync(file, new string('x', 12000), TestContext.Current.CancellationToken);
        await File.WriteAllTextAsync(second, new string('x', 3000), TestContext.Current.CancellationToken);
        try
        {
            await using var context = await fixture.Browser.NewContextAsync();
            var page = await OpenAsync(context);
            var downloads = new List<string>();
            page.Download += (_, d) => downloads.Add(d.SuggestedFilename);
            var url = page.Url;
            await page.EvaluateAsync("document.addEventListener('drop',e=>window.uploadDropTrusted=e.isTrusted,true)");
            await DropAsync(page, "#upload-zone input", file);
            await Expect(page.Locator("#bytes")).ToHaveTextAsync("12000");
            await DropAsync(page, "#upload-zone", file, padding: true);
            await Expect(page.Locator("#bytes")).ToHaveTextAsync("24000");
            await DropAsync(page, "#upload-zone ul", file);
            await page.Locator("#disabled").ClickAsync();
            await DropAsync(page, "#upload-zone", file, padding: true);
            await Expect(page.Locator("#selections")).ToHaveTextAsync("2");
            await page.Locator("#disabled").ClickAsync();
            await page.Locator("#single").ClickAsync();
            await DropAsync(page, "#upload-zone input", file, second: second);
            await Expect(page.Locator("#bytes")).ToHaveTextAsync("36000");
            await Expect(page.Locator("#selected-count")).ToHaveTextAsync("1");
            Assert.True(await page.EvaluateAsync<bool>("window.uploadDropTrusted"));
            Assert.Equal(url, page.Url);
            Assert.Empty(downloads);
            Assert.Empty(_errors);
        }
        finally { File.Delete(file); File.Delete(second); }
    }

    [Fact]
    public async Task QueuedProgressFailureRetryAndCancellationKeepTheRendererAlive()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var zone = page.Locator("#upload-zone");
        await page.Locator("#manual").ClickAsync();
        var chooser = await page.RunAndWaitForFileChooserAsync(() => zone.Locator("input").ClickAsync());
        await chooser.SetFilesAsync(new[] { Payload("one.txt", 48000), Payload("two.txt", 48000) });
        await Expect(zone.GetByRole(AriaRole.Progressbar)).ToHaveCountAsync(2);
        Assert.Empty(await zone.Locator("[role=progressbar][aria-valuenow]").AllAsync());
        await page.Locator("#fail").ClickAsync();
        await page.Locator("#start").ClickAsync();
        var failed = zone.Locator("." + Css.Classes.FileUpload.FileFailed);
        await Expect(failed).ToHaveCountAsync(1);
        await failed.GetByRole(AriaRole.Button, new() { Name = "Retry upload", Exact = true }).ClickAsync();
        await Expect(zone.Locator("." + Css.Classes.FileUpload.FileCompleted)).ToHaveCountAsync(2, new() { Timeout = 15000 });
        var again = await page.RunAndWaitForFileChooserAsync(() => zone.Locator("input").ClickAsync());
        await again.SetFilesAsync(Payload("cancel.txt", 48000));
        await page.Locator("#start").ClickAsync();
        await zone.GetByRole(AriaRole.Button, new() { Name = "Cancel upload", Exact = true }).ClickAsync();
        await Expect(zone.Locator("." + Css.Classes.FileUpload.FileCancelled)).ToHaveCountAsync(1);
        await zone.GetByRole(AriaRole.Button, new() { Name = "Retry upload", Exact = true }).ClickAsync();
        await Expect(zone.Locator("." + Css.Classes.FileUpload.FileCompleted)).ToHaveCountAsync(1);
        await zone.GetByRole(AriaRole.Button, new() { Name = "Remove file", Exact = true }).ClickAsync();
        await Expect(zone.Locator("li")).ToHaveCountAsync(0);
        Assert.Empty(_errors);
    }

    private async Task<IPage> OpenAsync(IBrowserContext context)
    {
        var page = await context.NewPageAsync();
        page.PageError += (_, e) => _errors.Add(e);
        page.Console += (_, e) => { if (e.Type == "error") _errors.Add(e.Text); };
        await page.GotoAsync($"{fixture.BaseUrl}/file-upload");
        await ListenerWait.ForAsync(page, "#upload-zone input", "change");
        return page;
    }

    [Fact]
    public async Task RemovingAWaitingFileDoesNotUploadItAfterItsRowDisappears()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await OpenAsync(context);
        var zone = page.Locator("#upload-zone");
        await page.Locator("#manual").ClickAsync();
        var chooser = await page.RunAndWaitForFileChooserAsync(() => zone.Locator("input").ClickAsync());
        await chooser.SetFilesAsync(new[] { Payload("keep.txt", 48000), Payload("remove.txt", 48000) });
        await page.Locator("#start").ClickAsync();
        await zone.Locator("li").Filter(new() { HasText = "remove.txt" })
            .GetByRole(AriaRole.Button, new() { Name = "Remove file", Exact = true }).ClickAsync();
        await Expect(page.Locator("#finished")).ToHaveTextAsync("1");
        await Expect(page.Locator("#bytes")).ToHaveTextAsync("48000");
        await Expect(zone.Locator("li")).ToHaveCountAsync(1);
        Assert.Empty(_errors);
    }

    private static FilePayload Payload(string name, int size) => new() { Name = name, MimeType = "text/plain", Buffer = new byte[size] };

    private static async Task DropAsync(IPage page, string selector, string file, bool padding = false, string? second = null)
    {
        var target = page.Locator(selector);
        await target.ScrollIntoViewIfNeededAsync();
        var bounds = (await target.BoundingBoxAsync())!;
        var x = bounds.X + (padding ? 2 : bounds.Width / 2);
        var y = bounds.Y + (padding ? 2 : bounds.Height / 2);
        var cdp = await page.Context.NewCDPSessionAsync(page);
        var data = new { items = Array.Empty<object>(), files = second is null ? new[] { file } : new[] { file, second }, dragOperationsMask = 1 };
        foreach (var type in new[] { "dragEnter", "dragOver", "drop" })
            await cdp.SendAsync("Input.dispatchDragEvent", new() { ["type"] = type, ["x"] = x, ["y"] = y, ["data"] = data });
        await cdp.DetachAsync();
    }
}
