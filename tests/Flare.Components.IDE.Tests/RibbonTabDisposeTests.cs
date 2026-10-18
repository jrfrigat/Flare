using Flare.Components.IDE;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components.Tests;

/// <summary>
/// A removed ribbon tab has to unregister itself. Its cleanup lives in <see cref="IDisposable.Dispose"/>, and the
/// component inherits <see cref="FlareComponentBase"/>, which owns the asynchronous disposal contract - Blazor used
/// to choose the asynchronous one and never call Dispose, so the ribbon kept the removed tab in its list and no tab
/// was left active.
/// </summary>
public sealed class RibbonTabDisposeTests : FlareTestContext
{
    public RibbonTabDisposeTests() => Services.AddFlareIde();

    private sealed class RibbonHost : ComponentBase
    {
        [Parameter] public bool ShowSecond { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<FlareRibbon>(0);
            builder.AddAttribute(1, "ChildContent", (RenderFragment)(content =>
            {
                content.OpenComponent<FlareRibbonTab>(0);
                content.AddAttribute(1, "Title", "First");
                content.CloseComponent();

                if (ShowSecond)
                {
                    content.OpenComponent<FlareRibbonTab>(2);
                    content.AddAttribute(3, "Title", "Second");
                    content.CloseComponent();
                }
            }));
            builder.CloseComponent();
        }
    }

    [Fact]
    public void RemovingTheActiveTab_ActivatesARemainingTab()
    {
        var cut = Render<RibbonHost>(p => p.Add(x => x.ShowSecond, true));
        cut.FindAll("[role=tab]")[1].Click();
        Assert.Equal("Second", Active(cut));

        cut.Render(p => p.Add(x => x.ShowSecond, false));

        Assert.Equal("First", Active(cut));
        Assert.NotEmpty(cut.FindAll("[role=tabpanel]"));
    }

    private static string Active(IRenderedComponent<RibbonHost> cut) =>
        cut.Find("[role=tab][aria-selected=true]").TextContent.Trim();
}
