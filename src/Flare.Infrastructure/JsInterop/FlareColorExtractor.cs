using Flare.Components.Services;
using Microsoft.JSInterop;

namespace Flare.Components;

/// <summary>Default <see cref="IFlareColorExtractor"/> backed by a canvas + quantize/score JS module.</summary>
public sealed class FlareColorExtractor : FlareJsModule, IFlareColorExtractor
{
    /// <summary>Initializes a new <see cref="FlareColorExtractor"/>.</summary>
    public FlareColorExtractor(IJSRuntime js)
        : base(js, "./_content/Flare.Components/js/colorextractor.js") { }

    /// <summary>Extracts the dominant color from the image at the given source.</summary>
    public ValueTask<string?> GetDominantColorAsync(string imageUrl, int sampleSize = 32) =>
        InvokeAsync<string?>("dominantColor", imageUrl, sampleSize);

    /// <summary>Extracts a representative color palette from the image at the given source.</summary>
    public async ValueTask<IReadOnlyList<string>> GetPaletteAsync(string imageUrl, int count = 5, int sampleSize = 32) =>
        await InvokeAsync<string[]>("palette", imageUrl, count, sampleSize) ?? [];
}
