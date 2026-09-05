using Flare.Abstractions;
using Flare.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class DialogResultTests
{
    [Fact]
    public void Ok_IsNotCancelled_AndCarriesPayload()
    {
        var result = DialogResult.Ok(123);

        Assert.False(result.Cancelled);
        Assert.Equal(123, result.GetData<int>());
    }

    [Fact]
    public void Cancel_IsCancelled_WithNoData()
    {
        var result = DialogResult.Cancel();

        Assert.True(result.Cancelled);
        Assert.Null(result.Data);
    }

    [Fact]
    public void GetData_WrongType_ReturnsDefault()
    {
        var result = DialogResult.Ok("text");

        Assert.Equal(0, result.GetData<int>());
        Assert.Null(result.GetData<string[]>());
    }
}
