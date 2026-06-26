namespace Flare.Components;

/// <summary>
/// Extracts a representative seed color (and a small palette) from an image, suitable for a
/// dynamic <c>FlareColor</c> seed. Inject it instead of calling JS directly.
/// </summary>
public interface IFlareColorExtractor
{
    /// <summary>
    /// Returns the most suitable seed color as <c>#RRGGBB</c> for the given image URL (or data URL),
    /// or <c>null</c> if the image cannot be read (e.g. a cross-origin image without CORS).
    /// </summary>
    /// <param name="imageUrl">Same-origin URL, data URL, or a CORS-enabled image URL.</param>
    /// <param name="sampleSize">Downscale dimension used for sampling (default 32x32).</param>
    ValueTask<string?> GetDominantColorAsync(string imageUrl, int sampleSize = 32);

    /// <summary>Returns up to <paramref name="count"/> representative colors (<c>#RRGGBB</c>), most prominent first.</summary>
    ValueTask<IReadOnlyList<string>> GetPaletteAsync(string imageUrl, int count = 5, int sampleSize = 32);
}
