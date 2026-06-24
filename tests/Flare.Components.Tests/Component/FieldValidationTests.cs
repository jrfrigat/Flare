using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Flare.Components.Tests.Component;

// ------------------------------------------------------------------------------
// Shared field chrome (FlareFieldBase) - EditContext validation wiring used by
// FlareField / FlareSelect / FlareAutocomplete.
// ------------------------------------------------------------------------------

public class C_FlareFieldValidationTests : FlareTestContext
{
    private sealed class Model
    {
        public string? Name { get; set; }
    }

    private static RenderFragment InEditContext(EditContext ctx, RenderFragment child) => builder =>
    {
        builder.OpenComponent<CascadingValue<EditContext>>(0);
        builder.AddAttribute(1, "Value", ctx);
        builder.AddAttribute(2, "ChildContent", child);
        builder.CloseComponent();
    };

    [Fact]
    public void FlareField_ShowsValidationMessageFromEditContext()
    {
        var model = new Model();
        var ctx = new EditContext(model);
        var store = new ValidationMessageStore(ctx);

        var cut = Render(InEditContext(ctx, b =>
        {
            b.OpenComponent<FlareField<string>>(0);
            b.AddAttribute(1, "Value", model.Name);
            b.AddAttribute(2, "For", (Expression<Func<string>>)(() => model.Name!));
            b.CloseComponent();
        }));

        // No validation error yet.
        Assert.Empty(cut.FindAll(".flare-input__helper--error"));

        store.Add(ctx.Field(nameof(Model.Name)), "Name is required");
        ctx.NotifyValidationStateChanged();

        Assert.Contains("Name is required", cut.Find(".flare-input__helper--error").TextContent);
    }

    [Fact]
    public void FlareSelect_ShowsValidationMessageFromEditContext()
    {
        var model = new Model();
        var ctx = new EditContext(model);
        var store = new ValidationMessageStore(ctx);

        var cut = Render(InEditContext(ctx, b =>
        {
            b.OpenComponent<FlareSelect<string>>(0);
            b.AddAttribute(1, "Value", model.Name);
            b.AddAttribute(2, "For", (Expression<Func<string>>)(() => model.Name!));
            b.CloseComponent();
        }));

        store.Add(ctx.Field(nameof(Model.Name)), "Pick a value");
        ctx.NotifyValidationStateChanged();

        Assert.Contains("Pick a value", cut.Find(".flare-select__helper--error").TextContent);
    }

    [Fact]
    public void ErrorText_OverridesValidationMessage()
    {
        var model = new Model();
        var ctx = new EditContext(model);
        var store = new ValidationMessageStore(ctx);
        store.Add(ctx.Field(nameof(Model.Name)), "From validation");

        var cut = Render(InEditContext(ctx, b =>
        {
            b.OpenComponent<FlareField<string>>(0);
            b.AddAttribute(1, "Value", model.Name);
            b.AddAttribute(2, "For", (Expression<Func<string>>)(() => model.Name!));
            b.AddAttribute(3, "ErrorText", "Explicit override");
            b.CloseComponent();
        }));

        var helper = cut.Find(".flare-input__helper--error").TextContent;
        Assert.Contains("Explicit override", helper);
        Assert.DoesNotContain("From validation", helper);
    }
}
