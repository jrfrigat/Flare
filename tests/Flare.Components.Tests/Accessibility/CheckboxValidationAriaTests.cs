using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Flare.Components.Tests.Accessibility;

public class CheckboxValidationAriaTests : FlareTestContext
{
    [Fact]
    public void AnError_IsDescribedAndMarkedInvalid()
    {
        var cut = Render<FlareCheckbox<bool>>(p => p
            .Add(x => x.Label, "Accept")
            .Add(x => x.ErrorText, "Required"));

        var input = cut.Find("input");
        var error = cut.Find("[role=alert]");
        Assert.Equal("true", input.GetAttribute("aria-invalid"));
        Assert.Equal(error.Id, input.GetAttribute("aria-describedby"));
        Assert.Equal("Required", error.TextContent.Trim());
    }

    [Fact]
    public void TheAccessibleName_DoesNotContainTheError()
    {
        var cut = Render<FlareCheckbox<bool>>(p => p
            .Add(x => x.Label, "Accept")
            .Add(x => x.ErrorText, "Required"));

        var labelId = cut.Find("input").GetAttribute("aria-labelledby");
        var label = cut.Find($"#{labelId}");
        Assert.Equal("Accept", label.TextContent.Trim());
        Assert.DoesNotContain("Required", label.TextContent);
    }

    [Fact]
    public void ClearingTheError_ClearsTheAssociations()
    {
        var cut = Render<FlareCheckbox<bool>>(p => p
            .Add(x => x.Label, "Accept")
            .Add(x => x.ErrorText, "Required"));

        cut.Render(p => p
            .Add(x => x.Label, "Accept")
            .Add(x => x.ErrorText, (string?)null));

        var input = cut.Find("input");
        Assert.Null(input.GetAttribute("aria-invalid"));
        Assert.Null(input.GetAttribute("aria-describedby"));
        Assert.Empty(cut.FindAll("[role=alert]"));
    }

    [Fact]
    public void AValidationError_FromTheEditContext_AppearsAndClears()
    {
        var model = new Model();
        Expression<Func<bool>> expression = () => model.Accepted;
        var editContext = new EditContext(model);
        var messages = new ValidationMessageStore(editContext);
        var field = FieldIdentifier.Create(expression);
        var cut = Render<CascadingValue<EditContext>>(p => p
            .Add(x => x.Value, editContext)
            .Add(x => x.IsFixed, true)
            .Add(x => x.ChildContent, b =>
            {
                b.OpenComponent<FlareCheckbox<bool>>(0);
                b.AddAttribute(1, "For", expression);
                b.CloseComponent();
            }));
        var checkbox = cut.FindComponent<FlareCheckbox<bool>>();

        messages.Add(field, "You must accept the terms.");
        editContext.NotifyValidationStateChanged();
        checkbox.WaitForAssertion(() => Assert.Equal("true", checkbox.Find("input").GetAttribute("aria-invalid")));

        messages.Clear();
        editContext.NotifyValidationStateChanged();
        checkbox.WaitForAssertion(() => Assert.Null(checkbox.Find("input").GetAttribute("aria-invalid")));
    }

    private sealed class Model
    {
        public bool Accepted { get; set; }
    }
}
