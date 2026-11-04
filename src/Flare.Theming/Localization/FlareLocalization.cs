using System.Globalization;
using System.Collections.Concurrent;
using Flare.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Flare.Theming;

/// <summary>Scoped culture engine using standard localizers over the component packages' resources.</summary>
public sealed class FlareLocalization(IServiceProvider? services = null) : IFlareLocalization
{
    private static readonly IStringLocalizerFactory BuiltIns = new ResourceManagerStringLocalizerFactory(
        Options.Create(new LocalizationOptions()), NullLoggerFactory.Instance);
    private readonly ConcurrentDictionary<Type, object> _localizers = new();
    private CultureInfo? _culture, _uiCulture, _requestedCulture, _requestedUI;

    /// <inheritdoc />
    public CultureInfo Culture => _culture ?? CultureInfo.CurrentCulture;
    /// <inheritdoc />
    public CultureInfo UICulture => _uiCulture ?? CultureInfo.CurrentUICulture;
    /// <inheritdoc />
    public event Action? Changed;

    /// <inheritdoc />
    public void SetCulture(CultureInfo culture, CultureInfo? uiCulture = null)
    {
        ArgumentNullException.ThrowIfNull(culture);
        uiCulture ??= culture;
        if (ReferenceEquals(_requestedCulture, culture) && ReferenceEquals(_requestedUI, uiCulture)) return;
        _requestedCulture = culture;
        _requestedUI = uiCulture;
        _culture = CultureInfo.ReadOnly((CultureInfo)culture.Clone());
        _uiCulture = CultureInfo.ReadOnly((CultureInfo)uiCulture.Clone());
        Changed?.Invoke();
    }

    /// <inheritdoc />
    public IStringLocalizer<TResource> GetLocalizer<TResource>()
    {
        if (_localizers.TryGetValue(typeof(TResource), out var cached)) return (IStringLocalizer<TResource>)cached;
        var localizer = new CultureLocalizer<TResource>(this, services?.GetService<IStringLocalizer<TResource>>(),
            BuiltIns.Create(typeof(TResource)));
        return (IStringLocalizer<TResource>)_localizers.GetOrAdd(typeof(TResource), localizer);
    }

    private sealed class CultureLocalizer<T>(IFlareLocalization context, IStringLocalizer<T>? application,
        IStringLocalizer builtIn) : IStringLocalizer<T>
    {
        public LocalizedString this[string name] => Read(name, null);
        public LocalizedString this[string name, params object[] arguments] => Read(name, arguments);

        private LocalizedString Read(string name, object[]? arguments)
        {
            var culture = CultureInfo.CurrentCulture;
            var ui = CultureInfo.CurrentUICulture;
            try
            {
                CultureInfo.CurrentCulture = context.Culture;
                CultureInfo.CurrentUICulture = context.UICulture;
                var text = application?[name];
                if (text is null || text.ResourceNotFound) text = builtIn[name];
                return arguments is null ? text : new LocalizedString(name,
                    string.Format(context.Culture, text.Value, arguments), text.ResourceNotFound, text.SearchedLocation);
            }
            finally
            {
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = ui;
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var culture = CultureInfo.CurrentCulture;
            var ui = CultureInfo.CurrentUICulture;
            try
            {
                CultureInfo.CurrentCulture = context.Culture;
                CultureInfo.CurrentUICulture = context.UICulture;
                var strings = builtIn.GetAllStrings(includeParentCultures).ToDictionary(x => x.Name);
                if (application is not null)
                    foreach (var text in application.GetAllStrings(includeParentCultures))
                        if (!text.ResourceNotFound) strings[text.Name] = text;
                return strings.Values.ToArray();
            }
            finally
            {
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = ui;
            }
        }
    }
}
