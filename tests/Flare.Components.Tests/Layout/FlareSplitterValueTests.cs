namespace Flare.Components.Tests;

/// <summary>
/// A splitter with a Value controls one length instead of the split between its neighbours: it hands the browser the
/// variable to write, the limits and the default, and reports the settled size.
/// </summary>
public class FlareSplitterValueTests : FlareTestContext
{
    [Fact]
    public void AValueSplitterCarriesTheVariableTheLimitsAndTheDefault()
    {
        var cut = Render<FlareSplitter>(p => p
            .Add(x => x.Value, 240.4)
            .Add(x => x.DefaultValue, 200)
            .Add(x => x.ValueVariable, "--side")
            .Add(x => x.ValueScope, ".host")
            .Add(x => x.MinSize, "120px")
            .Add(x => x.MaxSize, "600px"));

        var handle = cut.Find("[role=separator]");
        Assert.Equal("--side", handle.GetAttribute("data-flare-splitter-variable"));
        Assert.Equal(".host", handle.GetAttribute("data-flare-splitter-scope"));
        Assert.Equal("240", handle.GetAttribute("data-flare-splitter-value"));
        Assert.Equal("200", handle.GetAttribute("data-flare-splitter-default"));
        Assert.Equal("120px", handle.GetAttribute("data-flare-splitter-min"));
        Assert.Equal("600px", handle.GetAttribute("data-flare-splitter-max"));
        Assert.Equal("240", handle.GetAttribute("aria-valuenow"));
        Assert.Equal("120", handle.GetAttribute("aria-valuemin"));
        Assert.Equal("600", handle.GetAttribute("aria-valuemax"));
    }

    // Without a Value it is the sibling splitter it always was: no variable, no value, no aria value from markup.
    [Fact]
    public void WithoutAValueTheSplitterResizesItsNeighboursAsBefore()
    {
        var cut = Render<FlareSplitter>(p => p.Add(x => x.ValueVariable, "--side").Add(x => x.MinSize, "120px"));

        var handle = cut.Find("[role=separator]");
        Assert.Null(handle.GetAttribute("data-flare-splitter-variable"));
        Assert.Null(handle.GetAttribute("data-flare-splitter-value"));
        Assert.Null(handle.GetAttribute("aria-valuenow"));
        Assert.Equal("120px", handle.GetAttribute("data-flare-splitter-min"));
    }

    [Fact]
    public async Task TheSettledSizeIsReported()
    {
        string? reported = null;
        var cut = Render<FlareSplitter>(p => p.Add(x => x.Value, 240)
            .Add(x => x.OnSizeChanged, (string size) => reported = size));

        await cut.InvokeAsync(() => cut.Instance.OnSplitChanged("256px"));

        Assert.Equal("256px", reported);
    }
}
