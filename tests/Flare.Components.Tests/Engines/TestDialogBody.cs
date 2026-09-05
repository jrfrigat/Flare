using Flare.Abstractions;
using Flare.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

// A minimal dialog body used by the component-dialog tests: it reads a [Parameter] payload and
// closes itself through the cascaded FlareDialogInstance (the same contract a real dialog body uses).
internal sealed class TestDialogBody : ComponentBase
{
    [CascadingParameter] public FlareDialogInstance Dialog { get; set; } = default!;
    [Parameter] public string Payload { get; set; } = "";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "button");
        builder.AddAttribute(1, "class", "test-ok");
        builder.AddAttribute(2, "onclick", EventCallback.Factory.Create(this, () => Dialog.Close(Payload)));
        builder.AddContent(3, "OK");
        builder.CloseElement();

        builder.OpenElement(4, "button");
        builder.AddAttribute(5, "class", "test-cancel");
        builder.AddAttribute(6, "onclick", EventCallback.Factory.Create(this, () => Dialog.Cancel()));
        builder.AddContent(7, "Cancel");
        builder.CloseElement();
    }
}

// ------------------------------------------------------------------------------
// DialogResult / DialogParameters  (pure unit tests)
// ------------------------------------------------------------------------------
