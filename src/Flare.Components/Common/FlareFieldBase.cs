using System.Linq.Expressions;
using Flare.Core.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Flare.Components;

/// <summary>
/// Base class for the Flare field family (text field, select, multi-select, autocomplete).
/// Centralizes the shared field-chrome plumbing so it is not re-implemented per component:
/// the EditContext validation wiring (subscription, bound-field identifier, validation-message
/// lookup and change notification) and the theme-independent visual-variant mapping.
/// Each field component keeps its own markup and CSS class prefixes; this base only removes
/// the duplicated C# logic.
/// </summary>
public abstract class FlareFieldBase : FlareComponentBase
{
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

    /// <summary>
    /// Maps a theme-independent <see cref="InputVariant"/> to its shared field token class
    /// (the <c>--flare-input-*</c> variant), or null to keep the active theme's default style.
    /// </summary>
    /// <param name="variant">The requested visual variant.</param>
    /// <returns>The variant CSS class, or null for <see cref="InputVariant.Default"/>.</returns>
    protected static string? VariantClass(InputVariant variant) => variant switch
    {
        InputVariant.Filled => Css.Classes.Input.VariantFilled,
        InputVariant.Outlined => Css.Classes.Input.VariantOutlined,
        _ => null,
    };

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        if (_validationSubscribed && EditContext is not null)
            EditContext.OnValidationStateChanged -= HandleValidationStateChanged;
        await base.DisposeAsync();
    }
}
