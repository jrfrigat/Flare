using System.Globalization;
using Microsoft.Extensions.Localization;

namespace Flare.Abstractions;

/// <summary>Culture and translation context owned by one application scope or Server circuit.</summary>
public interface IFlareLocalization
{
    /// <summary>Culture used to format dates, numbers and translation arguments.</summary>
    CultureInfo Culture { get; }
    /// <summary>Culture used to select interface translations.</summary>
    CultureInfo UICulture { get; }
    /// <summary>Raised after either culture changes; consumers must dispatch UI work to their renderer.</summary>
    event Action? Changed;
    /// <summary>Updates this context without changing process defaults or the caller's ambient culture.</summary>
    /// <param name="culture">Formatting culture.</param>
    /// <param name="uiCulture">Interface culture; omitted to use the formatting culture.</param>
    void SetCulture(CultureInfo culture, CultureInfo? uiCulture = null);
    /// <summary>Returns a live localizer with application overrides and built-in resource fallback.</summary>
    /// <typeparam name="TResource">Resource marker belonging to the component's own package.</typeparam>
    IStringLocalizer<TResource> GetLocalizer<TResource>();
}
