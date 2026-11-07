using System.Globalization;
using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Flare.Components;

public partial class FlareDataGrid<TItem>
{
    [CascadingParameter] private FlareLocalizationState? Localization { get; set; }
    private IStringLocalizer<FlareStrings> CoreText =>
        (Localization?.Context ?? FlareLocalizationState.Ambient).GetLocalizer<FlareStrings>();
    private CultureInfo FormattingCulture => Localization?.Context.Culture ?? CultureInfo.CurrentCulture;
}
