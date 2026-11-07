using System.Globalization;
using Flare.Abstractions;
using Flare.Components.Resources;
using Flare.Theming;
using Microsoft.Extensions.DependencyInjection;
using Querio;

namespace Flare.Components.Tests;

public sealed class QueryLiveLocalizationTests : FlareTestContext
{
    [Fact]
    public async Task MountedQueryKeepsItsUnboundDraftWhenDefaultCaptionsChange()
    {
        var culture = new FlareLocalization();
        culture.SetCulture(CultureInfo.GetCultureInfo("en"));
        Services.AddSingleton<IFlareLocalization>(culture);
        var schema = new QuerySchema([new QueryEntity("requests", "Requests", [new QueryField("route", "Route", QueryFieldType.Text)])]);
        var spec = QueryBuilder.From(schema, "requests", "r").Select("r", "route", "route").Build();
        var cut = Render<FlareLocalizationProvider>(p => p.AddChildContent<FlareQueryBuilder>(g => g
            .Add(x => x.Schema, schema).Add(x => x.Spec, spec)));
        var query = cut.FindComponent<FlareQueryBuilder>().Instance;
        cut.Find("input[placeholder='Alias']").Input("draft");
        await cut.InvokeAsync(() => culture.SetCulture(CultureInfo.GetCultureInfo("ru")));
        var text = culture.GetLocalizer<QueryStrings>();
        cut.WaitForAssertion(() => Assert.Contains(text["Query_Source"].Value, cut.Markup));
        Assert.Same(query, cut.FindComponent<FlareQueryBuilder>().Instance);
        Assert.Equal("draft", cut.Find("input[placeholder='" + text["Query_Alias"].Value + "']").GetAttribute("value"));
    }
}
