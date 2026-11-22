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
            Assert.Equal("Поле имени", IdeStrings.FormulaBarNameBox);

            CultureInfo.CurrentUICulture = new CultureInfo("en-US");
            Assert.Equal("Close", IdeStrings.BackstageClose);
            Assert.Equal("Name box", IdeStrings.FormulaBarNameBox);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }
}
