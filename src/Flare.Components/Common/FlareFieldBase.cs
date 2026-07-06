using Flare.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq.Expressions;

namespace Flare.Components;

/// <summary>
/// Base class for the Flare field family (text field, numeric, mask, textarea, password, select,
/// multi-select, combobox, tag field, and the date/time pickers).
/// Centralizes the shared field plumbing so it is not re-implemented per component: the EditContext
/// validation wiring (subscription, bound-field identifier, validation-message lookup and change
/// notification) and the shared field parameters (label, placeholder, helper/error text, disabled,
/// read-only, required, and the visual Variant/Size/Typo). Every member forwards Variant/Size/Typo
/// to the shared <see cref="FlareFieldChrome"/> frame, so they live here once rather than being
/// re-declared per component.
/// </summary>
public abstract class FlareFieldBase : FlareComponentBase, IFlareField
{
    /// <summary>Label text shown for the field.</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>Placeholder text shown when the field is empty. Not every field renders it (e.g. toggles).</summary>
    [Parameter] public string? Placeholder { get; set; }

    /// <summary>Helper text shown below the field.</summary>
    [Parameter] public string? HelperText { get; set; }

    /// <summary>Error text; when set it overrides <see cref="HelperText"/> and marks the field invalid.</summary>
    [Parameter] public string? ErrorText { get; set; }

    /// <summary>Disables the field (no input, dimmed).</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>Makes the field read-only (value shown but not editable). Not every field renders it.</summary>
    [Parameter] public bool ReadOnly { get; set; }

    /// <summary>Marks the field as required (visual indicator + native <c>required</c> where applicable).</summary>
    [Parameter] public bool Required { get; set; }

    /// <summary>Visual variant (Filled/Outlined) of the field, independent of the active theme.
    /// <see cref="InputVariant.Default"/> (the default) keeps the theme's own field style.</summary>
    [Parameter] public InputVariant Variant { get; set; } = InputVariant.Default;

    /// <summary>Control size (Xs..Xl). <see cref="FieldSize.Md"/> (the default) is the standard field height.</summary>
    [Parameter] public FieldSize Size { get; set; } = FieldSize.Md;

    /// <summary>Optional typography scale for the field's text. Overrides the size-derived font;
    /// null (the default) keeps the size default.</summary>
    [Parameter] public TypographyScale? Typo { get; set; }

    /// <summary>The cascaded edit context used for validation when the field is bound with a <c>For</c> accessor.</summary>
    [CascadingParameter] protected EditContext? EditContext { get; set; }

    /// <summary>Identifier of the bound model field, created from the component's <c>For</c> accessor.</summary>
    protected FieldIdentifier FieldId { get; private set; }

    private bool _validationSubscribed;
    private bool _hasBoundField;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (EditContext is not null)
        {
            EditContext.OnValidationStateChanged += HandleValidationStateChanged;
            _validationSubscribed = true;
        }
    }

    /// <summary>
    /// Recomputes <see cref="FieldId"/> from the supplied validation accessor. Call from
    /// <see cref="ComponentBase.OnParametersSet"/> with the component's <c>For</c> expression.
    /// </summary>
    /// <typeparam name="TField">Type of the bound model field.</typeparam>
    /// <param name="accessor">Lambda selecting the bound model field, or null when the field is unbound.</param>
    protected void UpdateFieldIdentifier<TField>(Expression<Func<TField>>? accessor)
    {
        if (EditContext is not null && accessor is not null)
        {
            FieldId = FieldIdentifier.Create(accessor);
            _hasBoundField = true;
        }
    }

    /// <summary>The first validation message for the bound field, or null when the field is unbound or valid.</summary>
    protected string? ValidationError =>
        EditContext is not null && _hasBoundField
            ? EditContext.GetValidationMessages(FieldId).FirstOrDefault()
            : null;

    /// <summary>The error shown to the user: an explicit <paramref name="errorText"/> override wins over validation.</summary>
    /// <param name="errorText">The component's <c>ErrorText</c> parameter value.</param>
    /// <returns>The override, the first validation message, or null.</returns>
    protected string? DisplayedError(string? errorText) => errorText ?? ValidationError;

    /// <summary>Notifies the edit context that the bound field changed, when the field is bound.</summary>
    protected void NotifyFieldChanged()
    {
        if (EditContext is not null && _hasBoundField)
            EditContext.NotifyFieldChanged(FieldId);
    }

    private void HandleValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
        => InvokeAsync(StateHasChanged);

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        if (_validationSubscribed && EditContext is not null)
            EditContext.OnValidationStateChanged -= HandleValidationStateChanged;
        await base.DisposeAsync();
    }
}
