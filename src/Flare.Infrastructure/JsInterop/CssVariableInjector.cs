using Flare.Abstractions;
using Flare.Theming;
using Flare.Abstractions.Tokens;
using Microsoft.JSInterop;

namespace Flare.Components.Services;

/// <summary>Default <see cref="Flare.Abstractions.ICssVariableInjector"/> that writes theme CSS variables via JS interop.</summary>
public sealed class CssVariableInjector : ICssVariableInjector, IAsyncDisposable
{
    private readonly IJSRuntime _js;
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

    private string[] _appliedKeys = [];

    /// <summary>Initializes a new <see cref="CssVariableInjector"/>.</summary>
    public CssVariableInjector(IJSRuntime js)
    {
        _js = js;
        _moduleTask = new(() =>
            js.InvokeAsync<IJSObjectReference>(
                "import", "./_content/Flare.Components/js/flare-theme.js").AsTask());
    }

    /// <summary>Injects the given theme CSS variable map onto the document root.</summary>
    public async ValueTask InjectAsync(DesignTokens design, ColorScheme colors, CancellationToken ct = default)
    {
        try
        {
            var module = await _moduleTask.Value;
            var vars = design.Flatten(colors);
            await module.InvokeVoidAsync("setCssVariables", vars);

            var stale = _appliedKeys.Where(k => !vars.ContainsKey(k)).ToArray();
            if (stale.Length > 0)
                await module.InvokeVoidAsync("clearCustomTokens", stale);

            _appliedKeys = [.. vars.Keys];
        }
        catch (InvalidOperationException) { }
        catch (JSDisconnectedException) { }
    }

    /// <summary>Sets the static (non-variable) theme CSS block.</summary>
    public async ValueTask SetStaticCssAsync(string css, CancellationToken ct = default)
    {
        try
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("setStaticThemeCss", css);
        }
        catch (InvalidOperationException) { }
        catch (JSDisconnectedException) { }
    }

    /// <summary>Applies custom token overrides on top of the active theme.</summary>
    public async ValueTask SetCustomTokensAsync(IReadOnlyDictionary<string, string> tokens, CancellationToken ct = default)
    {
        try
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("setCustomTokens", tokens);
        }
        catch (InvalidOperationException) { }
        catch (JSDisconnectedException) { }
    }

    /// <summary>Removes previously applied custom token overrides.</summary>
    public async ValueTask ClearCustomTokensAsync(IEnumerable<string> tokenNames, CancellationToken ct = default)
    {
        try
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("clearCustomTokens", tokenNames);
        }
        catch (InvalidOperationException) { }
        catch (JSDisconnectedException) { }
    }

    /// <summary>Disposes the injector and releases its JS interop module.</summary>
    public async ValueTask DisposeAsync()
    {
        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}
