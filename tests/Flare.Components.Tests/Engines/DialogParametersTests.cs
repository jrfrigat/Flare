using Flare.Abstractions;
using Flare.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class DialogParametersTests
{
    [Fact]
    public void AddAndIndexer_RoundTripValues()
    {
        var parameters = new DialogParameters()
            .Add("A", 1)
            .Add("B", "x");
        parameters["C"] = true;

        Assert.Equal(3, parameters.Count);
        Assert.Equal(1, parameters["A"]);
        Assert.True(parameters.Contains("B"));
        Assert.True((bool)parameters["C"]!);
    }

    [Fact]
    public void Remove_DropsParameter()
    {
        var parameters = new DialogParameters().Add("A", 1);

        Assert.True(parameters.Remove("A"));
        Assert.False(parameters.Contains("A"));
        Assert.Equal(0, parameters.Count);
    }

    [Fact]
    public void MissingKey_IndexerReturnsNull()
    {
        var parameters = new DialogParameters();

        Assert.Null(parameters["nope"]);
        Assert.False(parameters.TryGetValue("nope", out _));
    }
}

// ------------------------------------------------------------------------------
// DialogService component-dialog API  (service-level, no rendering)
// ------------------------------------------------------------------------------
