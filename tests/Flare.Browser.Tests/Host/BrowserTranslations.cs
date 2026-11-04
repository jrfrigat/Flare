using System.Globalization;
using Flare.Components.Resources;
using Microsoft.Extensions.Localization;

namespace Flare.Browser.Tests.Host;

internal sealed class BrowserTranslations : IStringLocalizer<FlareStrings>
{
    public LocalizedString this[string name] => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr"
        ? name switch
        {
            "Dialog_Close" => new(name, "Fermer"), "Dialog_Confirm" => new(name, "Confirmer"),
            "Picker_Clear" => new(name, "Effacer"), "Calendar_NextMonth" => new(name, "Mois suivant"),
            _ => new(name, name, true)
        } : new(name, name, true);
    public LocalizedString this[string name, params object[] arguments]
    {
        get { var text = this[name]; return new(name, string.Format(CultureInfo.CurrentCulture, text.Value, arguments), text.ResourceNotFound); }
    }
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
        new[] { "Dialog_Close", "Dialog_Confirm", "Picker_Clear", "Calendar_NextMonth" }.Select(x => this[x]).Where(x => !x.ResourceNotFound);
}
