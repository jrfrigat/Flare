using Microsoft.AspNetCore.Components;

namespace Flare.Components;

/// <summary>
/// Base class for the EDITABLE Flare fields - the ones backed by a real focusable <c>&lt;input&gt;</c>/
/// <c>&lt;textarea&gt;</c> (text/password/numeric/mask/textarea). It adds the imperative focus surface
/// (<see cref="FocusAsync"/> + <see cref="Autofocus"/>) on top of <see cref="FlareFieldBase"/>.
/// The select-family fields (select, multi-select, combobox, tag field, pickers) deliberately do NOT
/// derive from this: they own their own focus/trigger model, so exposing a single-input focus API on
/// them would be misleading. They stay on <see cref="FlareFieldBase"/>.
/// </summary>
public abstract class FlareEditableFieldBase : FlareFieldBase
{
    /// <summary>Requests focus on the field's control after its first render (best-effort). Only one field
    /// per page should set this. Focus is applied via <see cref="FocusCoreAsync"/>, which each field wires
    /// to its real input.</summary>
    [Parameter] public bool Autofocus { get; set; }

    /// <summary>Focuses the field's editable control. Each field overrides this to point at its real
    /// input; the default is a no-op so a field that has not wired it is simply unaffected.</summary>
    protected virtual ValueTask FocusCoreAsync() => ValueTask.CompletedTask;

    /// <summary>Sets keyboard focus to this field's control.</summary>
    public ValueTask FocusAsync() => FocusCoreAsync();

    private bool _autofocused;

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender && Autofocus && !_autofocused)
        {
            _autofocused = true;
            try { await FocusCoreAsync(); } catch { /* control may not be in the DOM yet - best-effort */ }
        }
    }
}
