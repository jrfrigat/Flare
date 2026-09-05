using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// FlareStepper.OnStepChanging is an async guard that can veto a step change (e.g. validate first).
public class FlareStepperGuardTests : FlareTestContext
{
    private static RenderFragment ThreeSteps() => b =>
    {
        for (var i = 0; i < 3; i++)
        {
            b.OpenComponent<FlareStep>(i * 2);
            b.AddAttribute(i * 2 + 1, nameof(FlareStep.Label), (object)$"Step {i}");
            b.CloseComponent();
        }
    };

    [Fact]
    public async Task OnStepChanging_ReturningFalse_BlocksAdvance()
    {
        var cut = Render<FlareStepper>(p => p
            .Add(s => s.OnStepChanging, _ => Task.FromResult(false))
            .AddChildContent(ThreeSteps()));
        await cut.InvokeAsync(() => cut.Instance.Next());
        Assert.Equal(0, cut.Instance.ActiveIndex);
    }

    [Fact]
    public async Task OnStepChanging_ReturningTrue_Advances()
    {
        var cut = Render<FlareStepper>(p => p
            .Add(s => s.OnStepChanging, _ => Task.FromResult(true))
            .AddChildContent(ThreeSteps()));
        await cut.InvokeAsync(() => cut.Instance.Next());
        Assert.Equal(1, cut.Instance.ActiveIndex);
    }

    [Fact]
    public async Task OnStepChanging_ReceivesFromAndTo()
    {
        StepperChange seen = default;
        var cut = Render<FlareStepper>(p => p
            .Add(s => s.OnStepChanging, c => { seen = c; return Task.FromResult(true); })
            .AddChildContent(ThreeSteps()));
        await cut.InvokeAsync(() => cut.Instance.Next());
        Assert.Equal(0, seen.From);
        Assert.Equal(1, seen.To);
    }
}
