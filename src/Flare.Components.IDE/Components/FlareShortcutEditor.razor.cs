using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace Flare.Components.IDE;

/// <summary>Captures and validates a shortcut assignment without executing the associated command.</summary>
public partial class FlareShortcutEditor
{
    [Inject] private IJSRuntime Js { get; set; } = default!;

    /// <summary>Accessible label for the capture field.</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>Current shortcut, using the same gesture format as <see cref="FlareShortcutEntry.Keys"/>.</summary>
    [Parameter] public string? Value { get; set; }

    /// <summary>Reports a new shortcut or null when the assignment is cleared.</summary>
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>Assignment restored by Reset.</summary>
    [Parameter] public string? DefaultValue { get; set; }

    /// <summary>Assignments of other commands in this scope. Exclude the command currently being edited.</summary>
    [Parameter] public IEnumerable<string> AssignedShortcuts { get; set; } = [];

    /// <summary>Additional host-reserved shortcuts. Browser navigation keys are reserved by default.</summary>
    [Parameter] public IEnumerable<string> ReservedShortcuts { get; set; } = [];

    /// <summary>Disables capture, clearing and resetting.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Renders a single compact row: the clear action becomes an icon button, the reset button is
    /// omitted and the shortcut hint is shown only while an input error is present.
    /// </summary>
    [Parameter] public bool Compact { get; set; }

    /// <summary>Shows the reset-to-default button in the non-compact layout.</summary>
    [Parameter] public bool ShowReset { get; set; } = true;

    /// <summary>Shows the shortcut hint and error line in the non-compact layout.</summary>
    [Parameter] public bool ShowStatus { get; set; } = true;

    protected override string ComponentCssClass => Css.Classes.Ide.Shortcuts.Editor;

    private IStringLocalizer<IdeStrings> Text => Localizer<IdeStrings>();
    private readonly string _statusId = $"flare-shortcut-status-{Guid.NewGuid():N}";

    private string? FieldLabel => Compact ? Label : Label ?? Text["Shortcut_Label"];

    private bool StatusVisible => Compact ? _message is not null : ShowStatus;
    private static readonly HashSet<string> BrowserShortcuts = new(StringComparer.Ordinal)
    {
        "alt+arrowleft", "alt+arrowright", "alt+f4", "ctrl+l", "ctrl+r", "ctrl+t", "ctrl+n",
        "ctrl+w", "ctrl+shift+t", "ctrl+shift+n", "ctrl+shift+w", "ctrl+f5", "f5", "f1"
    };

    private ElementReference _capture;
    private IJSObjectReference? _module;
    private IJSObjectReference? _binding;
    private bool _recording;
    private string? _message;

    private void BeginCapture()
    {
        if (Disabled) return;
        _recording = true;
        _message = null;
    }

    private void CancelCapture()
    {
        _recording = false;
        _message = null;
    }

    private async Task CaptureAsync(KeyboardEventArgs args)
    {
        if (Disabled || !_recording || args.Repeat) return;
#if NET9_0_OR_GREATER
        if (args.IsComposing) return;
#endif
        var key = args.Key?.ToLowerInvariant();
        if (key is "tab" or "escape")
        {
            CancelCapture();
            return;
        }

        if (key is null or "" or "control" or "alt" or "shift" or "meta" or "altgraph") return;
        var isFunctionKey = key.Length is >= 2 and <= 3 && key[0] == 'f' &&
            int.TryParse(key.AsSpan(1), out var function) && function is >= 1 and <= 24;
        if ((!args.CtrlKey && !args.MetaKey && !args.AltKey && !isFunctionKey) ||
            key is "+" or " " or "unidentified" or "dead" or "process")
        {
            _message = "Shortcut_Invalid";
            return;
        }

        var value = Format(args.CtrlKey || args.MetaKey, args.AltKey, args.ShiftKey, key);
        if (Validate(value)) await SetValueAsync(value);
    }

    private bool Validate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return true;
        var normalized = Normalize(value);
        if (BrowserShortcuts.Contains(normalized) || ReservedShortcuts.Any(item => Normalize(item) == normalized))
        {
            _message = "Shortcut_Reserved";
            return false;
        }
        if (AssignedShortcuts.Any(item => Normalize(item) == normalized))
        {
            _message = "Shortcut_Conflict";
            return false;
        }
        return true;
    }

    private Task ResetAsync() => !Disabled && Validate(DefaultValue)
        ? SetValueAsync(DefaultValue)
        : Task.CompletedTask;

    private async Task SetValueAsync(string? value)
    {
        if (Disabled) return;
        CancelCapture();
        if (Value != value) await ValueChanged.InvokeAsync(value);
    }

    private static string Normalize(string value)
    {
        var parts = value.ToLowerInvariant().Split('+', StringSplitOptions.TrimEntries);
        var ctrl = false;
        var alt = false;
        var shift = false;
        foreach (var part in parts.AsSpan(0, parts.Length - 1))
        {
            ctrl |= part is "ctrl" or "control" or "meta" or "cmd" or "command";
            alt |= part is "alt" or "option";
            shift |= part == "shift";
        }
        return Format(ctrl, alt, shift, parts[^1]);
    }

    private static string Format(bool ctrl, bool alt, bool shift, string key) =>
        $"{(ctrl ? "ctrl+" : "")}{(alt ? "alt+" : "")}{(shift ? "shift+" : "")}{key}";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        try
        {
            _module = await Js.InvokeAsync<IJSObjectReference>("import", "./_content/Flare.Components.IDE/js/flare-ide.js");
            _binding = await _module.InvokeAsync<IJSObjectReference>("registerShortcutCapture", _capture);
        }
        catch (InvalidOperationException) { }
        catch (JSDisconnectedException) { }
        catch (JSException) { }
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        try
        {
            if (_binding is not null)
            {
                await _binding.InvokeVoidAsync("dispose");
                await _binding.DisposeAsync();
            }
            if (_module is not null) await _module.DisposeAsync();
        }
        catch (InvalidOperationException) { }
        catch (JSDisconnectedException) { }
        catch (JSException) { }
        await base.DisposeAsync();
    }
}
