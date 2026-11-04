using System.Globalization;
using Flare.Components.Resources;
using Flare.Theming;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Flare.Components;

/// <summary>Opt-in culture cascade for components that supply localized interface text or formatting.</summary>
public abstract class FlareLocalizedComponentBase : FlareComponentBase
{
    private static readonly FlareLocalization Ambient = new();
    /// <summary>Changes only when the localization provider signals a real update.</summary>
    [CascadingParameter] protected FlareLocalizationState? LocalizationState { get; set; }
    /// <summary>Formatting culture unless the component has an explicit Culture parameter.</summary>
    protected CultureInfo FormattingCulture => LocalizationState?.Context.Culture ?? CultureInfo.CurrentCulture;
    /// <summary>Culture selecting interface translations.</summary>
    protected CultureInfo InterfaceCulture => LocalizationState?.Context.UICulture ?? CultureInfo.CurrentUICulture;
    /// <summary>Core component translations, including partial application overrides.</summary>
    protected IStringLocalizer<FlareStrings> CoreText => Localizer<FlareStrings>();
    /// <summary>Translations from the component package's own resource marker.</summary>
    /// <typeparam name="TResource">Package resource marker.</typeparam>
    protected IStringLocalizer<TResource> Localizer<TResource>() =>
        (LocalizationState?.Context ?? Ambient).GetLocalizer<TResource>();
}
