using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// FlareDropZone was folded into FlareFileUploadZone - same input, same drag state, same default body. Its
// coverage moves here rather than disappearing with it.
public class FlareDropZoneTests : FlareTestContext
{
    [Fact]
    public void RendersRootAndFileInput()
    {
        var cut = Render<FlareFileUploadZone>(p => p
            .AddChildContent("<span>Drop here</span>"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.FileUpload.Root}"));
        Assert.NotEmpty(cut.FindAll("input[type=file]"));
    }
}
