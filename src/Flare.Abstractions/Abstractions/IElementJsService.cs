using Microsoft.AspNetCore.Components;

namespace Flare.Components.Services;

/// <summary>An element's viewport rectangle plus the viewport size, used to position fixed popups.</summary>
/// <param name="Top">Distance from the viewport top to the element's top edge (px).</param>
/// <param name="Bottom">Distance from the viewport top to the element's bottom edge (px).</param>
/// <param name="Left">Distance from the viewport left to the element's left edge (px).</param>
/// <param name="Width">The element's width (px).</param>
/// <param name="ViewportHeight">The viewport height (px).</param>
/// <param name="ViewportWidth">The viewport width (px).</param>
public readonly record struct ElementBounds(
    double Top, double Bottom, double Left, double Width, double ViewportHeight, double ViewportWidth);

/// <summary>
/// Typed JS-interop for small DOM helpers backed by the global <c>flare-components.js</c> bundle:
/// focusing an input and measuring an element's viewport bounds. Inject instead of calling
/// <see cref="Microsoft.JSInterop.IJSRuntime"/> directly.
/// </summary>
public interface IElementJsService
{
    /// <summary>Focuses <paramref name="element"/> and selects its contents (e.g. an OTP digit input).</summary>
    ValueTask FocusAndSelectAsync(ElementReference element);

    /// <summary>Selects (highlights) all the text in <paramref name="element"/> without moving focus elsewhere.</summary>
    ValueTask SelectAsync(ElementReference element);

    /// <summary>Removes focus from <paramref name="element"/> (programmatic blur).</summary>
    ValueTask BlurAsync(ElementReference element);

    /// <summary>Focuses <paramref name="element"/> and selects the character range [<paramref name="start"/>, <paramref name="end"/>).</summary>
    ValueTask SelectRangeAsync(ElementReference element, int start, int end);

    /// <summary>Returns the element's viewport rectangle and the current viewport size.</summary>
    ValueTask<ElementBounds> GetBoundsAsync(ElementReference element);
}
