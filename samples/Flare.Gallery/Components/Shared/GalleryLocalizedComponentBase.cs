using Flare.Components;
using Microsoft.AspNetCore.Components;

namespace Flare.Gallery.Components.Shared;

/// <summary>Refreshes application-owned Gallery resource bindings through the same culture cascade as Flare.</summary>
public abstract class GalleryLocalizedComponentBase : ComponentBase
{
    [CascadingParameter] private FlareLocalizationState? Localization { get; set; }
}
