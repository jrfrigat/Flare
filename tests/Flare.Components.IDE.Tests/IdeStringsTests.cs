using System.Globalization;
using Flare.Components.IDE;

namespace Flare.Components.Tests;

// The IDE package owns its wording: accessible names and default labels come from its EN/RU resources
// and follow the current UI culture, while anything the host sets explicitly stays as it is.
public class IdeStringsTests : FlareTestContext
{
    [Fact]
    public void Strings_FollowTheCurrentUiCulture()
    {
        var previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = new CultureInfo("ru-RU");
            Assert.Equal("Закрыть", IdeStrings.BackstageClose);
            Assert.Equal("Добавить лист", IdeStrings.SheetTabsAdd);

            CultureInfo.CurrentUICulture = new CultureInfo("en-US");
            Assert.Equal("Close", IdeStrings.BackstageClose);
            Assert.Equal("Add sheet", IdeStrings.SheetTabsAdd);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }

    [Fact]
    public void DocumentTabs_UseTheLocalizedDefaultLabel()
    {
        var previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = new CultureInfo("ru-RU");

            var cut = Render<FlareDocumentTabs>(p => p
                .AddChildContent<FlareDocumentTab>(t => t.Add(x => x.Title, "readme.md")));

            Assert.Equal("Вкладки документов",
                cut.Find($".{Css.Classes.Ide.DocTabs.Bar}").GetAttribute("aria-label"));
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }

    [Fact]
    public void SheetTabs_DefaultAddLabel_IsLocalized()
    {
        var previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = new CultureInfo("ru-RU");

            var cut = Render<FlareSheetTabs>(p => p
                .Add(x => x.Sheets, new[] { "Sheet1" }));

            Assert.Equal("Добавить лист",
                cut.Find($".{Css.Classes.Ide.SheetTabs.Add}").GetAttribute("aria-label"));
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }
}
