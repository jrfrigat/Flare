using System.Globalization;

namespace Flare.Components.Tests;

/// <summary>
/// The toolbar used to carry English titles and accessible names as literals, so a Russian application
/// announced English buttons. The strings come from the package resources now and follow
/// <see cref="CultureInfo.CurrentUICulture"/> like the editor's own label and placeholder.
/// </summary>
public class FlareRichTextEditorCultureTests : FlareTestContext
{
    private sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo _previous = CultureInfo.CurrentUICulture;

        public CultureScope(string name) => CultureInfo.CurrentUICulture = new CultureInfo(name);

        public void Dispose() => CultureInfo.CurrentUICulture = _previous;
    }

    private static AngleSharp.Dom.IElement Tool(IRenderedComponent<FlareRichTextEditor> cut, string name)
    {
        var matches = cut.FindAll($"button.{Css.Classes.Rte.Tool}")
            .Where(b => b.GetAttribute("aria-label") == name)
            .ToList();
        return Assert.Single(matches);
    }

    private static void AssertTool(IRenderedComponent<FlareRichTextEditor> cut, string name)
        => Assert.Equal(name, Tool(cut, name).GetAttribute("title"));

    [Fact]
    public void RussianCulture_LocalizesTheBasicToolbar()
    {
        using var scope = new CultureScope("ru");
        var cut = Render<FlareRichTextEditor>();

        AssertTool(cut, "Полужирный");
        AssertTool(cut, "Курсив");
        AssertTool(cut, "Подчеркнутый");
        AssertTool(cut, "Маркированный список");
        AssertTool(cut, "Нумерованный список");
    }

    [Fact]
    public void RussianCulture_LocalizesTheExpandedToolbarAndTheLinkField()
    {
        using var scope = new CultureScope("ru");
        var cut = Render<FlareRichTextEditor>(p => p
            .Add(x => x.ShowHeadings, true)
            .Add(x => x.ShowLink, true));

        AssertTool(cut, "Заголовок 1");
        AssertTool(cut, "Заголовок 2");
        AssertTool(cut, "Заголовок 3");
        AssertTool(cut, "Вставить ссылку");

        Tool(cut, "Вставить ссылку").Click();

        var input = cut.Find($"input.{Css.Classes.Rte.LinkInput}");
        Assert.Equal("Адрес ссылки", input.GetAttribute("aria-label"));
        AssertTool(cut, "Применить ссылку");
    }

    [Fact]
    public void EnglishCulture_KeepsTheEnglishToolbar()
    {
        using var scope = new CultureScope("en");
        var cut = Render<FlareRichTextEditor>(p => p.Add(x => x.ShowHeadings, true));

        AssertTool(cut, "Heading 1");
        AssertTool(cut, "Bold");
        AssertTool(cut, "Numbered list");
    }
}
