using Querio.OneC;

namespace Querio.Renderers.Tests;

/// <summary>
/// Covers the 1C renderer. This is the renderer that proves the model is semantic rather than
/// SQL-shaped: the same spec comes out in a different language, with its own keywords, its own
/// parameter syntax, and a narrower set of things it can do at all.
/// </summary>
public sealed class OneCRendererTests
{
    // A 1C-shaped schema: names are Cyrillic and unquoted, and the physical name carries the
    // object kind, which is what a real configuration looks like.
    private static QuerySchema Schema() => new(
        [
            new QueryEntity("Номенклатура", "Номенклатура",
            [
                new QueryField("Наименование", "Наименование", QueryFieldType.Text),
                new QueryField("Цена", "Цена", QueryFieldType.Number),
                new QueryField("ДатаСоздания", "Дата создания", QueryFieldType.DateTime),
            ])
            { Source = "Справочник.Номенклатура" },
        ]);

    private static QuerySpec Totals(QuerySchema schema) => QueryBuilder.From(schema, "Номенклатура", "Ном")
        .Select("Ном", "Наименование")
        .Sum("Ном", "Цена", "Итог")
        .Where(f => f.GreaterThan("Ном", "Цена", 100))
        .GroupBy("Ном", "Наименование")
        .OrderBySelectDescending("Итог")
        .Limit(5)
        .Build();

    [Fact]
    public void RendersTheQueryInTheOneCLanguage()
    {
        var schema = Schema();

        var result = OneCRenderer.Render(Totals(schema), schema);

        Assert.Equal(
            "ВЫБРАТЬ ПЕРВЫЕ 5 Ном.Наименование, СУММА(Ном.Цена) КАК Итог " +
            "ИЗ Справочник.Номенклатура КАК Ном " +
            "ГДЕ Ном.Цена > &p0 " +
            "СГРУППИРОВАТЬ ПО Ном.Наименование " +
            "УПОРЯДОЧИТЬ ПО Итог УБЫВ",
            result.Query);
    }

    [Fact]
    public void PassesValuesAsNamedParameters()
    {
        var schema = Schema();

        var result = OneCRenderer.Render(Totals(schema), schema);

        var parameter = Assert.Single(result.Parameters);
        Assert.Equal("p0", parameter.Name);
        Assert.Equal(100L, parameter.Value);
        Assert.DoesNotContain("100", result.Query, StringComparison.Ordinal);
    }

    [Fact]
    public void ResolvesARelativeWindowAgainstTheGivenMoment()
    {
        // 1C queries have no current-moment function, so the window becomes a parameter value. The
        // spec keeps the offset, so rendering it again tomorrow moves the window.
        var schema = Schema();
        var now = new DateTime(2026, 7, 19, 12, 0, 0, DateTimeKind.Utc);
        var spec = QueryBuilder.From(schema, "Номенклатура", "Ном")
            .Select("Ном", "Наименование")
            .Where(f => f.Since("Ном", "ДатаСоздания", 30, QueryTimeUnit.Day))
            .Build();

        var result = OneCRenderer.Render(spec, schema, now);

        Assert.Contains("Ном.ДатаСоздания >= &p0", result.Query, StringComparison.Ordinal);
        Assert.Equal(new DateTime(2026, 6, 19, 12, 0, 0, DateTimeKind.Utc), result.Parameters[0].Value);
    }

    [Fact]
    public void UsesTheOneCPatternMatchWithItsEscapeClause()
    {
        var schema = Schema();
        var spec = QueryBuilder.From(schema, "Номенклатура", "Ном")
            .Select("Ном", "Наименование")
            .Where(f => f.Contains("Ном", "Наименование", "болт"))
            .Build();

        var result = OneCRenderer.Render(spec, schema);

        Assert.Contains("Ном.Наименование ПОДОБНО &p0 СПЕЦСИМВОЛ \"\\\"", result.Query, StringComparison.Ordinal);
        Assert.Equal("%болт%", result.Parameters[0].Value);
    }

    [Fact]
    public void SpellsNullChecksTheOneCWay()
    {
        var schema = Schema();
        var spec = QueryBuilder.From(schema, "Номенклатура", "Ном")
            .Select("Ном", "Наименование")
            .Where(f => f.IsNull("Ном", "ДатаСоздания"))
            .Build();

        Assert.Contains("Ном.ДатаСоздания ЕСТЬ NULL", OneCRenderer.Render(spec, schema).Query, StringComparison.Ordinal);
    }

    [Fact]
    public void RefusesARowOffsetWhichOneCHasNoWayToExpress()
    {
        var schema = Schema();
        var spec = QueryBuilder.From(schema, "Номенклатура", "Ном")
            .Select("Ном", "Наименование")
            .Offset(20)
            .Build();

        var error = Assert.Throws<QueryRenderException>(() => OneCRenderer.Render(spec, schema));

        Assert.Equal(QueryFeature.Offset, error.Feature);
    }

    [Fact]
    public void RefusesAPercentile()
    {
        var schema = Schema();
        var spec = QueryBuilder.From(schema, "Номенклатура", "Ном")
            .Percentile("Ном", "Цена", 0.95, "p95")
            .Build();

        var error = Assert.Throws<QueryRenderException>(() => OneCRenderer.Render(spec, schema));

        Assert.Equal(QueryFeature.Percentile, error.Feature);
    }

    [Fact]
    public void DeclaresTheThingsItCannotDo()
    {
        Assert.False(OneCRenderer.Capabilities.Supports(QueryFeature.Percentile));
        Assert.False(OneCRenderer.Capabilities.Supports(QueryFeature.Offset));
        Assert.False(OneCRenderer.Capabilities.Supports(QueryFeature.CrossJoin));
        Assert.True(OneCRenderer.Capabilities.Supports(QueryFeature.Grouping));
        Assert.True(OneCRenderer.Capabilities.Supports(QueryFeature.LeftJoin));
    }

    [Fact]
    public void RejectsAnAliasItCouldNotSafelyWrite()
    {
        // 1C cannot quote a name, so a name it cannot write has to be refused rather than escaped.
        var schema = Schema();
        var spec = new QuerySpec(new QuerySource("Номенклатура", "a b"))
        {
            Select = [new QuerySelect { Field = new QueryFieldRef("a b", "Наименование") }],
        };

        var error = Assert.Throws<QueryRenderException>(() => OneCRenderer.Render(spec, schema));

        Assert.Contains("identifier", error.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GroupsATimestampIntoPeriodsTheOneCWay()
    {
        var schema = Schema();
        var spec = QueryBuilder.From(schema, "Номенклатура", "Ном")
            .SelectAndGroupByDay("Ном", "ДатаСоздания", "День")
            .CountRows("Всего")
            .Build();

        var result = OneCRenderer.Render(spec, schema);

        Assert.Contains("НАЧАЛОПЕРИОДА(Ном.ДатаСоздания, ДЕНЬ) КАК День", result.Query, StringComparison.Ordinal);
        Assert.Contains("КОЛИЧЕСТВО(*) КАК Всего", result.Query, StringComparison.Ordinal);
    }

    [Fact]
    public void RefusesAnIncoherentQueryBeforeWritingAnything()
    {
        var schema = Schema();
        var spec = new QuerySpec(new QuerySource("НетТакого", "н"));

        Assert.Throws<QueryValidationException>(() => OneCRenderer.Render(spec, schema));
    }
}
