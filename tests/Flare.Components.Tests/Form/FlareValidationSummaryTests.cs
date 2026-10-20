using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components.Tests;

public class FlareValidationSummaryTests : FlareTestContext
{
    private sealed class Model
    {
        public string? Name { get; set; }
    }

    // A context handed in from outside, so the same summary instance sees a DIFFERENT cascading
    // context after a render - what an EditForm does when its Model parameter changes.
    private sealed class ContextHost : ComponentBase
    {
        [Parameter] public EditContext? Context { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder b)
        {
            b.OpenComponent<CascadingValue<EditContext?>>(0);
            b.AddAttribute(1, "Value", Context);
            b.AddAttribute(3, "ChildContent", (RenderFragment)(c =>
            {
                c.OpenComponent<FlareValidationSummary>(0);
                c.CloseComponent();
            }));
            b.CloseComponent();
        }
    }

    // The smallest real field: registers its control id with the form through FlareFieldBase.
    private sealed class ProbeField : FlareFieldBase
    {
        [Parameter] public Expression<Func<string?>>? For { get; set; }
        [Parameter] public Action<EditContext>? ContextReady { get; set; }

        protected override string ComponentCssClass => "probe-field";

        protected override void OnParametersSet() => UpdateFieldIdentifier(For);

        protected override void BuildRenderTree(RenderTreeBuilder b)
        {
            if (EditContext is not null) ContextReady?.Invoke(EditContext);
            b.OpenElement(0, "input");
            b.AddAttribute(1, "id", ControlId);
            b.CloseElement();
        }
    }

    [Fact]
    public void SwitchingTheCascadingEditContextMovesTheSubscription()
    {
        var first = new EditContext(new Model());
        var second = new EditContext(new Model());
        var firstStore = new ValidationMessageStore(first);
        var secondStore = new ValidationMessageStore(second);
        firstStore.Add(new FieldIdentifier(first.Model, nameof(Model.Name)), "from first");
        secondStore.Add(new FieldIdentifier(second.Model, nameof(Model.Name)), "from second");

        var cut = Render<ContextHost>(p => p.Add(x => x.Context, first));
        Assert.Contains("from first", cut.Markup);

        cut.Render(p => p.Add(x => x.Context, second));
        cut.WaitForAssertion(() => Assert.Contains("from second", cut.Markup));

        // The old context must no longer drive the summary: clear it and notify.
        firstStore.Clear();
        first.NotifyValidationStateChanged();
        Assert.DoesNotContain("from first", cut.Markup);
        Assert.Contains("from second", cut.Markup);
    }

    [Fact]
    public void AMessageForARegisteredFieldLinksToItsControl()
    {
        var model = new Model { Name = null };
        EditContext? captured = null;

        var cut = Render<FlareForm>(p => p
            .Add(x => x.Model, model)
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareValidationSummary>(0);
                b.CloseComponent();
                b.OpenComponent<ProbeField>(2);
                b.AddAttribute(3, "For", (Expression<Func<string?>>)(() => model.Name));
                b.AddAttribute(4, "ContextReady", (Action<EditContext>)(ctx => captured = ctx));
                b.CloseComponent();
            }));

        Assert.NotNull(captured);
        var store = new ValidationMessageStore(captured!);
        var fieldId = new FieldIdentifier(model, nameof(Model.Name));
        store.Add(fieldId, "Name is required");
        captured!.NotifyValidationStateChanged();

        cut.WaitForAssertion(() => Assert.Contains("Name is required", cut.Markup));
        var link = cut.Find($"a.{Css.Classes.Validation.SummaryLink}");
        Assert.Equal("Name is required", link.TextContent);
        var controlId = cut.Find("input").GetAttribute("id");
        Assert.Equal($"#{controlId}", link.GetAttribute("href"));
    }
}
