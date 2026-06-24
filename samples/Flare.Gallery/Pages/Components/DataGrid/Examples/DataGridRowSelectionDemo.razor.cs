namespace Flare.Gallery.Pages.Components.DataGrid.Examples;

public partial class DataGridRowSelectionDemo
{
    private HashSet<Person> _selected = [];

    private void OnSelectionChanged(HashSet<Person> selection) => _selected = selection;
}
