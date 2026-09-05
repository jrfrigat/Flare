namespace Flare.Components.Tests;

/// <summary>
/// The package shipped with no test at all. Both components take a schema they cannot work without, and
/// the interesting floor is what they do WITHOUT one: say so, rather than throw or draw an empty frame
/// that looks like a component with nothing in it.
/// </summary>
public class QuerySmokeTests : FlareTestContext
{
    [Fact]
    public void QueryBuilder_WithoutASchema_RendersItsRootAndSaysWhatIsMissing()
    {
        var cut = Render<FlareQueryBuilder>();

        Assert.NotEmpty(cut.FindAll(".flare-qb"));
        Assert.NotEmpty(cut.FindAll(".flare-qb__ok"));
        Assert.Empty(cut.FindAll(".flare-qb__section"));
    }

    [Fact]
    public void QueryEditor_WithoutASchema_StillRendersItsRoot()
    {
        var cut = Render<FlareQueryEditor>();

        Assert.NotEmpty(cut.FindAll(".flare-qe"));
    }
}
