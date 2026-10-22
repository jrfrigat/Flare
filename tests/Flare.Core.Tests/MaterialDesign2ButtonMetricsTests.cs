using Flare.Theme.MaterialDesign2;

namespace Flare.Core.Tests;

/// <summary>
/// The Material 2 redline states a 64dp minimum button width and a 48dp extended FAB. Both are theme
/// values, not core decisions: the core only exposes the tokens, so this keeps the theme's numbers
/// from drifting away from the specification the theme exists to follow.
/// </summary>
public sealed class MaterialDesign2ButtonMetricsTests
{
    [Fact]
    public void LabelledButtonAndExtendedFabUseTheRedlineMetrics()
    {
        var design = new MaterialDesign2Theme().Design;

        Assert.Equal("var(--flare-spacing-32)", design.Button.MinWidth);
        Assert.Equal("var(--flare-spacing-6)", design.Fab.ExtendedPaddingBlock);
        Assert.Equal("var(--flare-spacing-10)", design.Fab.ExtendedPaddingInline);
    }
}
