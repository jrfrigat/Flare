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
    public async Task ValidationRendersOnlyFieldsWhoseDisplayedErrorChanged()
    {
        var models = Enumerable.Range(0, 80).Select(_ => new Model()).ToArray();
        var context = new EditContext(models);
        var messages = new ValidationMessageStore(context);
        var cut = Render<CascadingValue<EditContext>>(p => p
            .Add(x => x.Value, context)
            .AddChildContent(b =>
            {
                foreach (var model in models)
                {
                    b.OpenComponent<FlareTextField>(0);
                    b.AddAttribute(1, "For", (Expression<Func<string?>>)(() => model.Name));
                    b.CloseComponent();
                }
            }));
        var fields = cut.FindComponents<FlareTextField>();
        var before = fields.Select(field => field.RenderCount).ToArray();
        int ChangedFieldCount() => fields.Where((field, index) => field.RenderCount > before[index]).Count();
        var name = new FieldIdentifier(models[0], nameof(Model.Name));
        messages.Add(name, "Name is required");

        await cut.InvokeAsync(context.NotifyValidationStateChanged);

        Assert.Equal(1, ChangedFieldCount());
        Assert.Contains("Name is required", fields[0].Markup);
        var withError = fields[0].RenderCount;

        messages.Add(name, "Another message for the same field");
        await cut.InvokeAsync(context.NotifyValidationStateChanged);

        Assert.Equal(withError, fields[0].RenderCount);
        Assert.Equal(1, ChangedFieldCount());

        messages.Clear(name);
        await cut.InvokeAsync(context.NotifyValidationStateChanged);

        Assert.True(fields[0].RenderCount > withError);
        Assert.Equal(1, ChangedFieldCount());
        Assert.DoesNotContain("Name is required", fields[0].Markup);
    }

    [Fact]
    public async Task FieldTemplatesCanStillReadFormLevelValidation()
    {
        var model = new Model();
        var context = new EditContext(model);
        var messages = new ValidationMessageStore(context);
        RenderFragment label = b => b.AddContent(0, context.GetValidationMessages().FirstOrDefault() ?? "Name");
        var cut = Render<CascadingValue<EditContext>>(p => p
            .Add(x => x.Value, context)
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareTextField>(0);
                b.AddAttribute(1, "For", (Expression<Func<string?>>)(() => model.Name));
                b.AddAttribute(2, "LabelContent", label);
                b.CloseComponent();
            }));
        Assert.Contains("Name", cut.Markup);

        messages.Add(new FieldIdentifier(model, string.Empty), "Form-level message");
        await cut.InvokeAsync(context.NotifyValidationStateChanged);

        Assert.Contains("Form-level message", cut.Markup);
    }

    [Fact]
    public async Task ReplacingTheContextMovesTheFieldSubscription()
    {
        var model = new Model();
        var original = new EditContext(model);
        var replacement = new EditContext(model);
        RenderFragment fieldContent = b =>
        {
            b.OpenComponent<FlareTextField>(0);
            b.AddAttribute(1, "For", (Expression<Func<string?>>)(() => model.Name));
            b.CloseComponent();
        };
        var cut = Render<CascadingValue<EditContext>>(p => p
            .Add(x => x.Value, original)
            .AddChildContent(fieldContent));
        cut.Render(p => p.Add(x => x.Value, replacement).AddChildContent(fieldContent));
        var field = cut.FindComponent<FlareTextField>();
        var before = field.RenderCount;
        new ValidationMessageStore(replacement).Add(new FieldIdentifier(model, nameof(Model.Name)), "Replacement error");

        await cut.InvokeAsync(original.NotifyValidationStateChanged);
        Assert.Equal(before, field.RenderCount);

        await cut.InvokeAsync(replacement.NotifyValidationStateChanged);
        Assert.Contains("Replacement error", field.Markup);
    }

    [Fact]
    public void RebindingTheFieldReplacesItsSummaryRegistration()
    {
        var first = new Model();
        var second = new Model();
        var context = new EditContext(first);
        var registry = new Dictionary<FieldIdentifier, string>();
        Expression<Func<string?>>? accessor = () => first.Name;
        RenderFragment content = b =>
        {
            b.OpenComponent<CascadingValue<Dictionary<FieldIdentifier, string>>>(0);
            b.AddAttribute(1, "Value", registry);
            b.AddAttribute(2, "IsFixed", true);
            b.AddAttribute(3, "ChildContent", (RenderFragment)(field =>
            {
                field.OpenComponent<FlareTextField>(0);
                field.AddAttribute(1, "For", accessor);
                field.CloseComponent();
            }));
            b.CloseComponent();
        };
        var cut = Render<CascadingValue<EditContext>>(p => p
            .Add(x => x.Value, context).AddChildContent(content));
        var id = Assert.Single(registry).Value;

        accessor = () => second.Name;
        cut.Render(p => p.Add(x => x.Value, context).AddChildContent(content));

        var rebound = Assert.Single(registry);
        Assert.Equal(new FieldIdentifier(second, nameof(Model.Name)), rebound.Key);
        Assert.Equal(id, rebound.Value);

        accessor = null;
        cut.Render(p => p.Add(x => x.Value, context).AddChildContent(content));

        Assert.Empty(registry);
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
