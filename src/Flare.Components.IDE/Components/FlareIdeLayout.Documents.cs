using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.IDE;

public partial class FlareIdeLayout
{
    /// <summary>Enables docking and supplies documents keyed within this layout.</summary>
    [Parameter] public IReadOnlyList<IdeDocument>? Documents { get; set; }

    /// <summary>Renders each document. The rendered instance stays mounted while its key stays in <see cref="Documents"/>.</summary>
    [Parameter] public RenderFragment<IdeDocument>? DocumentContent { get; set; }

    /// <summary>Controlled selection per region. A null value in the dictionary selects the existing tool or main content.</summary>
    [Parameter] public IReadOnlyDictionary<IdeDocumentRegion, string?>? ActiveDocuments { get; set; }

    /// <summary>Reports the user's selection in a region.</summary>
    [Parameter] public EventCallback<IdeDocumentSelection> OnActiveDocumentChanged { get; set; }

    /// <summary>Requests a move. Accept it by changing the document's region in <see cref="Documents"/>.</summary>
    [Parameter] public EventCallback<IdeDocumentMove> OnDocumentMove { get; set; }

    /// <summary>Reports a request to close a document from its dock tab. Accept it by removing the document
    /// from <see cref="Documents"/>; the layout keeps the document until the host does so.</summary>
    [Parameter] public EventCallback<IdeDocument> OnDocumentCloseRequested { get; set; }

    /// <summary>Context menu entries rendered for a docked document when its dock tab is right-clicked.
    /// The host decides which actions to offer, including close variants or confirmation.</summary>
    [Parameter] public RenderFragment<IdeDocument>? DocumentContextMenu { get; set; }

    private static readonly IdeDocumentRegion[] DocumentRegions =
        [IdeDocumentRegion.Left, IdeDocumentRegion.Center, IdeDocumentRegion.Right];

    private readonly Dictionary<IdeDocumentRegion, string?> _documentSelection = new();
    private readonly Dictionary<string, ElementReference> _documentHeaders = new(StringComparer.Ordinal);
    private string? _draggedDocument;
    private IdeDocumentMove? _pendingDocumentMove;
    private string? _focusDocument;

    private bool Docking => Documents is not null;

    protected override void OnParametersSet()
    {
        foreach (var key in _documentHeaders.Keys.Where(key => FindDocument(key) is null).ToArray())
            _documentHeaders.Remove(key);
        if (_pendingDocumentMove is not { } move ||
            FindDocument(move.Document.Key)?.Region != move.Destination)
            return;

        _pendingDocumentMove = null;
        if (ActiveDocuments is null) _documentSelection[move.Destination] = move.Document.Key;
        _focusDocument = move.Document.Key;
    }

    private IdeDocument? FindDocument(string key) => Documents?.FirstOrDefault(doc => doc.Key == key);

    private string? ActiveDocument(IdeDocumentRegion region)
    {
        var selection = ActiveDocuments ?? _documentSelection;
        if (selection.TryGetValue(region, out var key) &&
            (key is null || FindDocument(key)?.Region == region))
            return key;
        return Documents?.FirstOrDefault(doc => doc.Region == region)?.Key;
    }

    private bool DocumentVisible(IdeDocument document) => document.Key == ActiveDocument(document.Region) &&
        (document.Region == IdeDocumentRegion.Center || !IsCollapsed(SidePosition(document.Region)));

    private bool ToolContentCovered(IdeDocumentRegion region) =>
        Docking && ActiveDocument(region) is not null &&
        (region == IdeDocumentRegion.Center || !IsCollapsed(SidePosition(region)));

    private static ToolPanelPosition SidePosition(IdeDocumentRegion region) =>
        region == IdeDocumentRegion.Left ? ToolPanelPosition.Left : ToolPanelPosition.Right;

    private bool DocumentFillsRegion(ToolPanelPosition position) => Docking && (position switch
    {
        ToolPanelPosition.Left => ActiveDocument(IdeDocumentRegion.Left) is not null,
        ToolPanelPosition.Right => ActiveDocument(IdeDocumentRegion.Right) is not null,
        _ => false
    });

    private async Task SelectDocumentAsync(IdeDocumentRegion region, string? key)
    {
        if (ActiveDocuments is null) _documentSelection[region] = key;
        await OnActiveDocumentChanged.InvokeAsync(new(region, key));
    }

    private void BeginDocumentDrag(string key) => _draggedDocument = key;

    private void EndDocumentDrag() => _draggedDocument = null;

    private Task DropDocumentAsync(IdeDocumentRegion destination)
    {
        var key = _draggedDocument;
        EndDocumentDrag();
        return key is not null && FindDocument(key) is { } document
            ? RequestDocumentMoveAsync(document, destination)
            : Task.CompletedTask;
    }

    private async Task RequestDocumentMoveAsync(IdeDocument document, IdeDocumentRegion destination)
    {
        if (document.Region == destination || !OnDocumentMove.HasDelegate) return;
        _pendingDocumentMove = new(document, destination);
        await OnDocumentMove.InvokeAsync(_pendingDocumentMove);
    }

    private Task RequestDocumentCloseAsync(IdeDocument document) =>
        document.Closeable && OnDocumentCloseRequested.HasDelegate
            ? OnDocumentCloseRequested.InvokeAsync(document)
            : Task.CompletedTask;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_focusDocument is not { } key || !_documentHeaders.TryGetValue(key, out var header)) return;
        _focusDocument = null;
        try { await header.FocusAsync(preventScroll: false); }
        catch (InvalidOperationException) { }
        catch (JSException) { }
    }
}
