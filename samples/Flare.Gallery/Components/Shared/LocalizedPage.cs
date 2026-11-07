using Flare.Gallery.Services;
using Microsoft.AspNetCore.Components;

namespace Flare.Gallery.Components.Shared;

public abstract class LocalizedPage : GalleryLocalizedComponentBase, IDisposable
{
    [Inject]
    protected LanguageService Lang { get; set; } = null!;

    public virtual void Dispose() { }
}
