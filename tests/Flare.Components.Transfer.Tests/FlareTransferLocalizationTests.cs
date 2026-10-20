using System.Globalization;

namespace Flare.Components.Tests;

/// <summary>
/// The panel titles used to be English literals while the move buttons were already localized, so a
/// Russian consumer got a mixed interface without overriding anything.
/// </summary>
public class FlareTransferLocalizationTests : FlareTestContext
{
    private static readonly string[] Items = ["alpha", "beta"];

    [Fact]
    public void RussianCulture_LocalizesDefaultTitlesAndMoveButtons()
    {
        WithUiCulture("ru-RU", () =>
        {
            var cut = Render<FlareTransfer<string>>(p => p.Add(x => x.SourceItems, Items));

            var headers = cut.FindAll($".{Css.Classes.Transfer.PanelHeader}");
            Assert.Equal("Доступные (2)", headers[0].TextContent.Trim());
            Assert.Equal("Выбранные (0)", headers[1].TextContent.Trim());

            var lists = cut.FindAll("[role=listbox]");
            Assert.Equal("Доступные", lists[0].GetAttribute("aria-label"));
            Assert.Equal("Выбранные", lists[1].GetAttribute("aria-label"));

            var labels = cut.FindAll("button").Select(button => button.GetAttribute("aria-label")).ToList();
            Assert.Contains("Переместить выбранные", labels);
            Assert.Contains("Вернуть все", labels);
        });
    }

    [Fact]
    public void ExplicitTitles_TakePriorityOverLocalizedDefaults()
    {
        WithUiCulture("ru-RU", () =>
        {
            var cut = Render<FlareTransfer<string>>(p => p
                .Add(x => x.SourceItems, Items)
                .Add(x => x.SourceTitle, "All permissions")
                .Add(x => x.TargetTitle, "Granted"));

            var lists = cut.FindAll("[role=listbox]");
            Assert.Equal("All permissions", lists[0].GetAttribute("aria-label"));
            Assert.Equal("Granted", lists[1].GetAttribute("aria-label"));
        });
    }

    [Fact]
    public void ChangingCulture_UpdatesTitlesOnTheNextRender()
    {
        IRenderedComponent<FlareTransfer<string>>? cut = null;
        WithUiCulture("en-US", () =>
        {
            cut = Render<FlareTransfer<string>>(p => p.Add(x => x.SourceItems, Items));
            Assert.Equal("Available", cut.FindAll("[role=listbox]")[0].GetAttribute("aria-label"));
        });

        WithUiCulture("ru-RU", () => cut!.Render());

        Assert.Equal("Доступные", cut!.FindAll("[role=listbox]")[0].GetAttribute("aria-label"));
    }

    private static void WithUiCulture(string name, Action action)
    {
        var previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(name);
            action();
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }
}
