using Flare.Components.Gantt;
using Flare.Components.Resources;
using Flare.Gallery.Resources;
using Microsoft.Extensions.Localization;

namespace Flare.Gallery.Services;

// App resource overlays use the standard factory; missing entries fall through to the package.
internal sealed class GalleryTranslations<T>(IStringLocalizerFactory factory) : IStringLocalizer<T>
{
    private readonly IStringLocalizer _text = factory.Create(typeof(T) == typeof(FlareStrings)
        ? typeof(CoreOverrides) : typeof(T) == typeof(GanttStrings) ? typeof(GanttOverrides) : typeof(SchedulerOverrides));
    public LocalizedString this[string name] => _text[name];
    public LocalizedString this[string name, params object[] arguments] => _text[name, arguments];
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => _text.GetAllStrings(includeParentCultures);
}
