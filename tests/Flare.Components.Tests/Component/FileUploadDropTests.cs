using AngleSharp.Dom;

namespace Flare.Components.Tests.Component;

/// <summary>
/// The drop zone gets its drag-and-drop from the browser, not from its own code: the hidden file input
/// is stretched over the drop area, and a file input takes a dropped file as the DEFAULT ACTION of the
/// `drop` event. The root used to cancel that event, which killed the drop the component exists to
/// receive - the zone lit up, stayed lit, and nothing arrived. Nothing in the C# was wrong, which is why
/// no test saw it: the defect is one attribute, and the only thing that can observe it is the markup.
///
/// So these read the rendered attributes. `dragover` and `dragenter` must stay cancelled - without them
/// the region is not a drop target at all - and `drop` must not be, except on the file list, the one
/// part of the zone with no input beneath it, where the browser's default is to navigate away to the
/// dropped file.
/// </summary>
public sealed class FileUploadDropTests : FlareTestContext
{
    // Blazor renders `@ondrop:preventDefault="true"` as this attribute on the element.
    private const string PreventDrop = "blazor:ondrop:preventdefault";
    private const string PreventDragOver = "blazor:ondragover:preventdefault";

    private static IElement Root(IRenderedComponent<FlareFileUploadZone> cut) =>
        cut.Find(".flare-file-upload");

    [Fact]
    public void Zone_DoesNotCancelTheDropItsInputHasToReceive()
    {
        var root = Root(Render<FlareFileUploadZone>());

        Assert.False(root.HasAttribute(PreventDrop),
            "The root must not cancel `drop`: the hidden input under it takes the file as that event's "
            + "default action, so cancelling here throws the drop away and the change that would have "
            + "followed it never fires.");
    }

    // The other half of the same contract, and the reason the fix is one attribute and not a deletion.
    [Fact]
    public void Zone_StillCancelsDragOverSoItIsADropTargetAtAll()
    {
        var root = Root(Render<FlareFileUploadZone>());

        Assert.True(root.HasAttribute(PreventDragOver),
            "Without a cancelled `dragover` the browser never treats the region as a drop target, and "
            + "no drop is delivered anywhere.");
    }

    [Fact]
    public void FileList_RefusesTheBrowserDefaultWhereNoInputCoversIt()
    {
        // The list only renders once something is queued, so this asserts the markup the list itself
        // declares rather than trying to queue a file through the browser dialog.
        var cut = Render<FlareUploadFileList>(p => p
            .Add(x => x.Items, (IReadOnlyList<FlareUploadFile>)[]));

        var list = cut.Find(".flare-file-upload__list");
        Assert.True(list.HasAttribute(PreventDrop),
            "A drop that lands on the file list has no input under it, so the browser default applies - "
            + "and that default is to navigate away from the app to the dropped file.");
        Assert.True(list.HasAttribute(PreventDragOver));
    }
}
