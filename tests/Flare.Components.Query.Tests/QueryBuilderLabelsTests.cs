using System.Globalization;
using Flare.Components;

namespace Flare.Components.Tests;

// The designer's default captions belong to the package and follow the UI culture; AND and OR are
// query-language tokens and must read the same in every language.
public class QueryBuilderLabelsTests
{
    [Fact]
    public void For_RussianCulture_UsesRussianCaptionsAndKeepsTheLanguageTokens()
    {
        var labels = QueryBuilderLabels.For(new CultureInfo("ru-RU"));

        Assert.Equal("Источник", labels.Source);
        Assert.Equal("Требуется схема.", labels.SchemaMissing);
        Assert.Equal("AND", labels.And);
        Assert.Equal("OR", labels.Or);
    }

    [Fact]
    public void For_EnglishCulture_UsesEnglishCaptionsAndKeepsTheLanguageTokens()
    {
        var labels = QueryBuilderLabels.For(new CultureInfo("en-US"));

        Assert.Equal("Source", labels.Source);
        Assert.Equal("A schema is required.", labels.SchemaMissing);
        Assert.Equal("AND", labels.And);
        Assert.Equal("OR", labels.Or);
    }

    [Fact]
    public void Default_FollowsTheCurrentUiCulture()
    {
        var previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = new CultureInfo("ru-RU");

            Assert.Equal("Фильтр", QueryBuilderLabels.Default.Filter);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }
}
