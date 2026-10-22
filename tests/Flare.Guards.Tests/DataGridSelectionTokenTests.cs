using Flare.Css.Tokens;

namespace Flare.Guards.Tests;

/// <summary>
/// TASK-660: the grid's state chips and the editing row are theme decisions. The stylesheet paints them
/// through DataGrid tokens whose defaults keep the roles it drew before; the selected row itself keeps
/// using the shared selected layer, because that state belongs to the whole library.
/// </summary>
public class DataGridSelectionTokenTests
{
    private static readonly string Css = File.ReadAllText(
        Path.Combine(FindRepoRoot(), "src", "Flare.Components", "wwwroot", "css", "datagrid.css"));

    [Fact]
    public void StateSurfaces_PaintThroughDataGridTokens()
    {
        Assert.Contains($"var({DataGridField.GroupCountBg})", Css);
        Assert.Contains($"var({DataGridField.GroupCountColor})", Css);
        Assert.Contains($"var({DataGridField.FilterCountBg})", Css);
        Assert.Contains($"var({DataGridField.FilterCountColor})", Css);
        Assert.Contains($"var({DataGridField.ConnectorBg})", Css);
        Assert.Contains($"var({DataGridField.ConnectorColor})", Css);
        Assert.Contains($"var({DataGridField.ConnectorBorder})", Css);
        Assert.Contains($"var({DataGridField.FilterGroupRailColor})", Css);
        Assert.Contains($"var({DataGridField.EditingBg})", Css);
    }

    [Fact]
    public void StateSurfaces_NoLongerNameTheContainerRoles()
    {
        Assert.DoesNotContain($"var({Color.SecondaryContainer})", Css);
        Assert.DoesNotContain($"var({Color.OnSecondaryContainer})", Css);
        Assert.DoesNotContain($"var({Color.PrimaryContainer})", Css);
        Assert.DoesNotContain($"var({Color.OnPrimaryContainer})", Css);
    }

    private static string FindRepoRoot()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
            if (Directory.Exists(Path.Combine(dir.FullName, "src", "Flare.Components")))
                return dir.FullName;

        throw new DirectoryNotFoundException("Could not find the repository root from " + AppContext.BaseDirectory);
    }
}
