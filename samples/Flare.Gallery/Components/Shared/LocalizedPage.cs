using Flare.Gallery.Services;
using Microsoft.AspNetCore.Components;

namespace Flare.Gallery.Components.Shared;

public abstract class LocalizedPage : ComponentBase, IDisposable
{
    [Inject]
    protected LanguageService Lang { get; set; } = null!;

    protected override void OnInitialized()
    {
        Lang.LanguageChanged += OnLanguageChanged;
    }

    private void OnLanguageChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        Lang.LanguageChanged -= OnLanguageChanged;
    }
}
