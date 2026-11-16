using Flare.Components.IDE;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components.Tests;

/// <summary>
/// An expensive editor must not be instantiated once per open document. The active-only mode mounts the
/// active document of each region (and any retained document) while keeping a document that stays active
/// across a region move mounted.
/// </summary>
public class IdeLayoutDocumentMountingTests : FlareTestContext
{
    public IdeLayoutDocumentMountingTests() => Services.AddFlareIde();

    public sealed class MountProbe : ComponentBase, IDisposable
    {
        [Parameter] public string Label { get; set; } = "";
        [Parameter] public Action<string>? Mounted { get; set; }
        [Parameter] public Action<string>? Disposed { get; set; }

        protected override void OnInitialized() => Mounted?.Invoke(Label);

        protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.AddContent(0, Label);

        public void Dispose() => Disposed?.Invoke(Label);
    }

    private readonly List<string> _events = [];

    private RenderFragment<IdeDocument> Content => document => builder =>
    {
        builder.OpenComponent<MountProbe>(0);
        builder.AddAttribute(1, nameof(MountProbe.Label), document.Key);
        builder.AddAttribute(2, nameof(MountProbe.Mounted), (Action<string>)_events.Add);
        builder.AddAttribute(3, nameof(MountProbe.Disposed), (Action<string>)(key => _events.Add($"dispose:{key}")));
        builder.CloseComponent();
    };

    private IRenderedComponent<FlareIdeLayout> RenderLayout(
        IReadOnlyList<IdeDocument> documents,
        IdeDocumentMounting mounting = IdeDocumentMounting.All,
        Func<IdeDocument, bool>? retain = null,
        IReadOnlyDictionary<IdeDocumentRegion, string?>? active = null) =>
        Render<FlareIdeLayout>(p =>
        {
            p.Add(x => x.Documents, documents);
            p.Add(x => x.DocumentContent, Content);
            p.Add(x => x.DocumentMounting, mounting);
            if (retain is not null) p.Add(x => x.RetainDocumentContent, retain);
            if (active is not null) p.Add(x => x.ActiveDocuments, active);
        });

    private static Dictionary<IdeDocumentRegion, string?> Selection(string? key) =>
        new() { [IdeDocumentRegion.Center] = key };

    [Fact]
    public void All_MountsEveryDocument()
    {
        var cut = RenderLayout(
            [new("a", "A.cs"), new("b", "B.cs"), new("c", "C.md", IdeDocumentRegion.Right)]);

        Assert.Equal(["a", "b", "c"], cut.FindComponents<MountProbe>().Select(probe => probe.Instance.Label));
    }

    [Fact]
    public void ActiveOnly_MountsTheActiveDocumentOfEachRegion()
    {
        var cut = RenderLayout(
            [new("a", "A.cs"), new("b", "B.cs"), new("c", "C.md", IdeDocumentRegion.Right)],
            IdeDocumentMounting.ActiveOnly);

        Assert.Equal(["a", "c"], cut.FindComponents<MountProbe>().Select(probe => probe.Instance.Label));
    }

    [Fact]
    public void ActiveOnly_RetainsTheDocumentsTheHostNames()
    {
        var cut = RenderLayout(
            [new("a", "A.cs"), new("b", "B.cs")],
            IdeDocumentMounting.ActiveOnly,
            document => document.Key == "b");

        Assert.Equal(["a", "b"], cut.FindComponents<MountProbe>().Select(probe => probe.Instance.Label));
        Assert.Contains("b", _events);
    }

    [Fact]
    public void ActiveOnly_SwitchingTheActiveDocumentReplacesTheMountedContent()
    {
        var cut = RenderLayout(
            [new("a", "A.cs"), new("b", "B.cs")],
            IdeDocumentMounting.ActiveOnly,
            active: Selection("a"));
        Assert.Equal(["a"], cut.FindComponents<MountProbe>().Select(probe => probe.Instance.Label));

        cut.Render(p => p.Add(x => x.ActiveDocuments, Selection("b")));

        Assert.Equal(["b"], cut.FindComponents<MountProbe>().Select(probe => probe.Instance.Label));
        Assert.Contains("dispose:a", _events);
    }

    [Fact]
    public void ActiveOnly_KeepsAnActiveEditorMountedAcrossARegionMove()
    {
        var docs = new List<IdeDocument> { new("a", "A.cs"), new("c", "C.md", IdeDocumentRegion.Right) };
        var cut = RenderLayout(docs, IdeDocumentMounting.ActiveOnly);
        Assert.Equal(["a", "c"], cut.FindComponents<MountProbe>().Select(probe => probe.Instance.Label));

        cut.Render(p => p
            .Add(x => x.Documents, new List<IdeDocument>
            {
                new("a", "A.cs", IdeDocumentRegion.Right),
                new("c", "C.md", IdeDocumentRegion.Right)
            })
            .Add(x => x.ActiveDocuments, new Dictionary<IdeDocumentRegion, string?>
            {
                [IdeDocumentRegion.Right] = "a"
            }));

        Assert.Equal("a", Assert.Single(cut.FindAll($".{Css.Classes.Ide.Layout.Document}:not([hidden])")).TextContent);
        Assert.Single(cut.FindComponents<MountProbe>(), probe => probe.Instance.Label == "a");
        Assert.Single(_events, entry => entry == "a");
        Assert.DoesNotContain("dispose:a", _events);
    }
}
