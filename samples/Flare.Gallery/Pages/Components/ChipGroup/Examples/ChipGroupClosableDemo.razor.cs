namespace Flare.Gallery.Pages.Components.ChipGroup.Examples;

public partial class ChipGroupClosableDemo
{
    private List<string> _tags = new() { "Blazor", ".NET", "C#", "WebAssembly" };

    private void RemoveTag(string tag)
    {
        _tags.Remove(tag);
        StateHasChanged();
    }
}