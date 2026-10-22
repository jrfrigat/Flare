using System.Linq.Expressions;
using AngleSharp.Dom;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Flare.Components.Tests.Accessibility;

/// <summary>
/// TASK-468: a radio group's error is an alert with a stable id, each radio describes itself with it and
/// reports the invalid state while it is shown, and the legend still names the group. The EditForm flow
/// adds, updates and clears the message, and a new group Value reaches the radios.
/// </summary>
public class FlareRadioGroupAccessibilityTests : FlareTestContext
{
    private sealed class Model
    {
        public string? Choice { get; set; }
    }

    private static RenderFragment InEditContext(EditContext ctx, RenderFragment child) => builder =>
    {
        builder.OpenComponent<CascadingValue<EditContext>>(0);
        builder.AddAttribute(1, "Value", ctx);
        builder.AddAttribute(2, "ChildContent", child);
        builder.CloseComponent();
    };

    private static RenderFragment TwoRadios() => b =>
    {
        b.OpenComponent<FlareRadio<string>>(0);
        b.AddAttribute(1, nameof(FlareRadio<string>.Value), "basic");
        b.AddAttribute(2, nameof(FlareRadio<string>.Label), "Basic");
        b.CloseComponent();
        b.OpenComponent<FlareRadio<string>>(3);
        b.AddAttribute(4, nameof(FlareRadio<string>.Value), "pro");
        b.AddAttribute(5, nameof(FlareRadio<string>.Label), "Pro");
        b.CloseComponent();
    };

    private static RenderFragment Group(Expression<Func<string>> forValue, string? errorText = null, string? value = null) => b =>
    {
        b.OpenComponent<FlareRadioGroup<string>>(0);
        b.AddAttribute(1, nameof(FlareRadioGroup<string>.For), forValue);
        b.AddAttribute(2, nameof(FlareRadioGroup<string>.Label), "Plan");
        b.AddAttribute(3, nameof(FlareRadioGroup<string>.Value), value);
        b.AddAttribute(4, nameof(FlareRadioGroup<string>.ErrorText), errorText);
        b.AddAttribute(5, nameof(FlareRadioGroup<string>.ChildContent), TwoRadios());
        b.CloseComponent();
    };

    private static string ErrorSelector => $".{Css.Classes.RadioGroup.ErrorMsg}";

    private static void AssertDescribedByTheError(IReadOnlyList<IElement> inputs, string errorId)
    {
        Assert.Equal(2, inputs.Count);
        foreach (var input in inputs)
        {
            Assert.Contains(errorId, input.GetAttribute("aria-describedby"));
            Assert.Equal("true", input.GetAttribute("aria-invalid"));
        }
    }

    private static void AssertNoErrorState(IReadOnlyList<IElement> inputs)
    {
        foreach (var input in inputs)
        {
            Assert.False(input.HasAttribute("aria-describedby"));
            Assert.False(input.HasAttribute("aria-invalid"));
        }
    }

    [Fact]
    public void ExplicitError_IsAnAlertThatDescribesEveryRadio()
    {
        var model = new Model();
        var ctx = new EditContext(model);
        var cut = Render(InEditContext(ctx, Group(() => model.Choice!, "Pick a plan")));

        var error = cut.Find(ErrorSelector);
        var errorId = error.GetAttribute("id");
        Assert.False(string.IsNullOrEmpty(errorId));
        Assert.Equal("alert", error.GetAttribute("role"));
        Assert.Equal("Pick a plan", error.TextContent.Trim());
        AssertDescribedByTheError(cut.FindAll("input[type=radio]"), errorId!);
        Assert.Equal("Plan", cut.Find("legend").TextContent.Trim());
    }

    [Fact]
    public void ValidationError_AddsUpdatesAndClearsTheAlertAndTheDescription()
    {
        var model = new Model();
        var ctx = new EditContext(model);
        var store = new ValidationMessageStore(ctx);
        var cut = Render(InEditContext(ctx, Group(() => model.Choice!)));

        Assert.Empty(cut.FindAll(ErrorSelector));
        AssertNoErrorState(cut.FindAll("input[type=radio]"));

        var field = ctx.Field(nameof(Model.Choice));
        store.Add(field, "Pick a plan");
        ctx.NotifyValidationStateChanged();

        cut.WaitForAssertion(() => Assert.Single(cut.FindAll(ErrorSelector)));
        var error = cut.Find(ErrorSelector);
        var errorId = error.GetAttribute("id");
        Assert.Equal("alert", error.GetAttribute("role"));
        Assert.Equal("Pick a plan", error.TextContent.Trim());
        AssertDescribedByTheError(cut.FindAll("input[type=radio]"), errorId!);

        store.Clear(field);
        store.Add(field, "Pick a pro plan");
        ctx.NotifyValidationStateChanged();

        cut.WaitForAssertion(() => Assert.Equal("Pick a pro plan", cut.Find(ErrorSelector).TextContent.Trim()));
        Assert.Equal(errorId, cut.Find(ErrorSelector).GetAttribute("id"));

        store.Clear(field);
        ctx.NotifyValidationStateChanged();

        cut.WaitForAssertion(() => Assert.Empty(cut.FindAll(ErrorSelector)));
        AssertNoErrorState(cut.FindAll("input[type=radio]"));
    }

    [Fact]
    public void GroupValueChange_ReachesTheAlreadyRenderedRadios()
    {
        var cut = Render<FlareRadioGroup<string>>(p => p
            .Add(x => x.Value, "basic")
            .Add(x => x.ChildContent, TwoRadios()));

        var inputs = cut.FindAll("input[type=radio]");
        Assert.True(inputs[0].HasAttribute("checked"));
        Assert.False(inputs[1].HasAttribute("checked"));
        Assert.Equal(inputs[0].GetAttribute("name"), inputs[1].GetAttribute("name"));

        cut.Render(p => p.Add(x => x.Value, "pro"));

        var updated = cut.FindAll("input[type=radio]");
        Assert.False(updated[0].HasAttribute("checked"));
        Assert.True(updated[1].HasAttribute("checked"));
    }
}
