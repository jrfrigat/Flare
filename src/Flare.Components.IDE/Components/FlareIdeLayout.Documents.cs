using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.IDE;

public partial class FlareIdeLayout
{
    [Inject] private IJSRuntime Js { get; set; } = default!;

    /// <summary>Enables docking and supplies documents keyed within this layout.</summary>
    [Parameter] public IReadOnlyList<IdeDocument>? Documents { get; set; }

    /// <summary>Whether the layout draws its built-in Center dock strip. Set it to false when the host
    /// renders the main document tabs in <see cref="ChildContent"/>. In that mode the Center region has
    /// no layout controls: its selection is read only from <see cref="ActiveDocuments"/>, and no entry
    /// (or a null one) shows <see cref="ChildContent"/>. The host must provide the control that returns
    /// to the main content, because the built-in Center button is gone.</summary>
    [Parameter] public bool ShowCenterDockStrip { get; set; } = true;

    /// <summary>Content rendered in place of the built-in Center dock strip - a host tab strip for the
    /// main document area. The fragment occupies the Center column of the layout's dock row. It wins over
    /// <see cref="ShowCenterDockStrip"/>; as with a suppressed strip, Center selection comes only from
    /// <see cref="ActiveDocuments"/> and the host supplies the return-to-main-content control.</summary>
    [Parameter] public RenderFragment? CenterDockHeader { get; set; }

    /// <summary>Renders each document. Which documents invoke it is chosen by <see cref="DocumentMounting"/>.</summary>
    [Parameter] public RenderFragment<IdeDocument>? DocumentContent { get; set; }

    /// <summary>Whether every document mounts its content or only the active document of each region.
    /// The default, <see cref="IdeDocumentMounting.All"/>, preserves each document's state at one content
    /// instance per document. <see cref="IdeDocumentMounting.ActiveOnly"/> disposes the content of a
    /// document that becomes inactive, so its transient state is lost when it mounts again; a document
    /// that stays active across a region move remains mounted.</summary>
    [Parameter] public IdeDocumentMounting DocumentMounting { get; set; } = IdeDocumentMounting.All;

    /// <summary>Documents whose content stays mounted in <see cref="IdeDocumentMounting.ActiveOnly"/> mode
    /// even while they are not the active document of their region. Ignored when every document mounts.</summary>
    [Parameter] public Func<IdeDocument, bool>? RetainDocumentContent { get; set; }

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
    private ToolPanelPosition? _railDropTarget;
    private IJSObjectReference? _ideModule;
    private bool _dragModuleRequested;

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

    // The built-in Center strip is the only control that returns from a Center document to the main
    // content. Without it the host owns that control, so the layout never guesses a Center document.
    private bool UsesDefaultCenterStrip => ShowCenterDockStrip && CenterDockHeader is null;

    private string? ActiveDocument(IdeDocumentRegion region)
    {
        if (region == IdeDocumentRegion.Center && !UsesDefaultCenterStrip)
            return ActiveDocuments is { } controlled &&
                   controlled.TryGetValue(region, out var selected) && selected is not null &&
                   FindDocument(selected)?.Region == region
                ? selected
                : null;

        var selection = ActiveDocuments ?? _documentSelection;
        if (selection.TryGetValue(region, out var key) &&
            (key is null || FindDocument(key)?.Region == region))
            return key;
        return Documents?.FirstOrDefault(doc => doc.Region == region)?.Key;
    }

    private bool DocumentVisible(IdeDocument document) => document.Key == ActiveDocument(document.Region) &&
        (document.Region == IdeDocumentRegion.Center || !IsCollapsed(SidePosition(document.Region)));

    private bool DocumentContentMounted(IdeDocument document) =>
        DocumentMounting == IdeDocumentMounting.All ||
        document.Key == ActiveDocument(document.Region) ||
        (RetainDocumentContent?.Invoke(document) ?? false);

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

    // A region the host hid draws no dock controls, documents or resizer; the Center column remains.
    private bool RegionVisible(IdeDocumentRegion region) => region switch
    {
        IdeDocumentRegion.Left => LeftRegionVisible,
        IdeDocumentRegion.Right => RightRegionVisible,
        _ => true,
    };

    // A region reserving only its rail has no width for a dock strip; its controls would spill into
    // the neighboring column. The rail is the only control until the region is expanded.
    private bool RegionShowsDockStrip(IdeDocumentRegion region)
    {
        if (region == IdeDocumentRegion.Center) return true;
        var position = SidePosition(region);
        return !IsCollapsed(position) && !IsFloating(position);
    }

    private async Task SelectDocumentAsync(IdeDocumentRegion region, string? key)
    {
        if (ActiveDocuments is null) _documentSelection[region] = key;
        await OnActiveDocumentChanged.InvokeAsync(new(region, key));
    }

    private void BeginDocumentDrag(string key) => _draggedDocument = key;

    private void EndDocumentDrag() => _draggedDocument = null;

    private async Task<bool> DropDocumentAsync(IdeDocumentRegion destination)
    {
        var key = _draggedDocument;
        EndDocumentDrag();

        // A tab dragged from a host FlareDocumentTabs carries its key through the IDE module; the
        // layout's own dock tabs use the field above. Reading even when the field won clears a stale
        // external key on an internal drop.
        if (_ideModule is not null)
        {
            try { key ??= await _ideModule.InvokeAsync<string?>("takeDroppedDocument"); }
            catch (JSException) { }
            catch (ObjectDisposedException) { }
        }

        if (key is null || FindDocument(key) is not { } document ||
            document.Region == destination || !OnDocumentMove.HasDelegate)
            return false;

        await RequestDocumentMoveAsync(document, destination);
        return true;
    }

    private async Task RequestDocumentMoveAsync(IdeDocument document, IdeDocumentRegion destination)
    {
        if (document.Region == destination || !OnDocumentMove.HasDelegate) return;
        _pendingDocumentMove = new(document, destination);
        await OnDocumentMove.InvokeAsync(_pendingDocumentMove);
    }

    private static IdeDocumentRegion? DocumentRegionOf(ToolPanelPosition position) => position switch
    {
        ToolPanelPosition.Left => IdeDocumentRegion.Left,
        ToolPanelPosition.Right => IdeDocumentRegion.Right,
        _ => null
    };

    private string RailClass(ToolPanelPosition position) => _railDropTarget == position
        ? $"{Css.Classes.Ide.Layout.Rail} {Css.Classes.Ide.Layout.RailDropTarget}"
        : Css.Classes.Ide.Layout.Rail;

    private void SetRailDropTarget(ToolPanelPosition position, bool over)
    {
        if (DocumentRegionOf(position) is null) return;
        var target = over ? position : (ToolPanelPosition?)null;
        if (_railDropTarget == target) return;
        _railDropTarget = target;
        StateHasChanged();
    }

    private async Task DropOnRailAsync(ToolPanelPosition position)
    {
        _railDropTarget = null;
        StateHasChanged();
        if (DocumentRegionOf(position) is not { } region) return;
        if (await DropDocumentAsync(region))
            await SetCollapsedAsync(position, false);
    }

    private Task RequestDocumentCloseAsync(IdeDocument document) =>
        document.Closeable && OnDocumentCloseRequested.HasDelegate
            ? OnDocumentCloseRequested.InvokeAsync(document)
            : Task.CompletedTask;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (Docking && _ideModule is null && !_dragModuleRequested)
            await RegisterDragSourceAsync();

        if (_focusDocument is not { } key || !_documentHeaders.TryGetValue(key, out var header)) return;
        _focusDocument = null;
        try { await header.FocusAsync(preventScroll: false); }
        catch (InvalidOperationException) { }
        catch (JSException) { }
    }

    private async Task RegisterDragSourceAsync()
    {
        _dragModuleRequested = true;
        try
        {
            _ideModule = await Js.InvokeAsync<IJSObjectReference>("import", IdeInterop.ModulePath);
            await _ideModule.InvokeVoidAsync("registerDocumentDragSource");
        }
        catch (InvalidOperationException) { _ideModule = null; }
        catch (JSException) { _ideModule = null; }
    }
}
