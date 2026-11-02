using Flare.Abstractions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components;

internal sealed class DialogCloseConfirmation : ComponentBase
{
    [CascadingParameter] public FlareDialogInstance Dialog { get; set; } = default!;
    [Parameter] public string Message { get; set; } = string.Empty;
    [Parameter] public string ConfirmLabel { get; set; } = string.Empty;
    [Parameter] public string CancelLabel { get; set; } = string.Empty;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "p");
        builder.AddAttribute(1, "class", Css.Classes.Dialog.ProviderMessage);
        builder.AddContent(2, Message);
        builder.CloseElement();
        builder.OpenElement(3, "div");
        builder.AddAttribute(4, "class", Css.Classes.Dialog.Actions);
        builder.OpenComponent<FlareButton>(5);
        builder.AddAttribute(6, nameof(FlareButton.Variant), ButtonVariant.Text);
        builder.AddAttribute(7, nameof(FlareButton.OnClick), EventCallback.Factory.Create<MouseEventArgs>(this, () => Dialog.CancelAsync()));
        builder.AddAttribute(8, nameof(FlareButton.ChildContent), (RenderFragment)(b => b.AddContent(0, CancelLabel)));
        builder.CloseComponent();
        builder.OpenComponent<FlareButton>(9);
        builder.AddAttribute(10, nameof(FlareButton.Variant), ButtonVariant.Filled);
        builder.AddAttribute(11, nameof(FlareButton.OnClick), EventCallback.Factory.Create<MouseEventArgs>(this, () => Dialog.CloseAsync()));
        builder.AddAttribute(12, nameof(FlareButton.ChildContent), (RenderFragment)(b => b.AddContent(0, ConfirmLabel)));
        builder.CloseComponent();
        builder.CloseElement();
    }
}
