using Flare.Abstractions;
using Flare.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests.Component;

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

public class C_DialogResultTests
{
    [Fact]
    public void Ok_IsNotCancelled_AndCarriesPayload()
    {
        var result = DialogResult.Ok(123);

        Assert.False(result.Cancelled);
        Assert.Equal(123, result.GetData<int>());
    }

    [Fact]
    public void Cancel_IsCancelled_WithNoData()
    {
        var result = DialogResult.Cancel();

        Assert.True(result.Cancelled);
        Assert.Null(result.Data);
    }

    [Fact]
    public void GetData_WrongType_ReturnsDefault()
    {
        var result = DialogResult.Ok("text");

        Assert.Equal(0, result.GetData<int>());
        Assert.Null(result.GetData<string[]>());
    }
}

public class C_DialogParametersTests
{
    [Fact]
    public void AddAndIndexer_RoundTripValues()
    {
        var parameters = new DialogParameters()
            .Add("A", 1)
            .Add("B", "x");
        parameters["C"] = true;

        Assert.Equal(3, parameters.Count);
        Assert.Equal(1, parameters["A"]);
        Assert.True(parameters.Contains("B"));
        Assert.True((bool)parameters["C"]!);
    }

    [Fact]
    public void Remove_DropsParameter()
    {
        var parameters = new DialogParameters().Add("A", 1);

        Assert.True(parameters.Remove("A"));
        Assert.False(parameters.Contains("A"));
        Assert.Equal(0, parameters.Count);
    }

    [Fact]
    public void MissingKey_IndexerReturnsNull()
    {
        var parameters = new DialogParameters();

        Assert.Null(parameters["nope"]);
        Assert.False(parameters.TryGetValue("nope", out _));
    }
}

// ------------------------------------------------------------------------------
// DialogService component-dialog API  (service-level, no rendering)
// ------------------------------------------------------------------------------

public class C_DialogServiceComponentTests
{
    [Fact]
    public void Show_AddsToOpenDialogs_AndRaisesStateChanged()
    {
        var service = new DialogService();
        var stateChanges = 0;
        service.OnStateChanged += () => stateChanges++;

        var reference = service.Show<TestDialogBody>("Edit",
            new DialogParameters().Add(nameof(TestDialogBody.Payload), "x"),
            new DialogOptions { Size = DialogSize.Sm });

        Assert.Single(service.OpenDialogs);
        Assert.Equal("Edit", service.OpenDialogs[0].Title);
        Assert.Equal(DialogSize.Sm, service.OpenDialogs[0].Options.Size);
        Assert.Equal(typeof(TestDialogBody), service.OpenDialogs[0].ContentType);
        Assert.False(reference.Result.IsCompleted);
        Assert.Equal(1, stateChanges);
    }

    [Fact]
    public async Task ClosingInstance_ResolvesResultWithPayload_AndRemovesDialog()
    {
        var service = new DialogService();
        var stateChanges = 0;
        service.OnStateChanged += () => stateChanges++;

        var reference = service.Show<TestDialogBody>("Edit");
        reference.Instance.Close("payload-42");

        var result = await reference.Result;
        Assert.False(result.Cancelled);
        Assert.Equal("payload-42", result.GetData<string>());
        Assert.Empty(service.OpenDialogs);
        Assert.Equal(2, stateChanges); // one for show, one for close
    }

    [Fact]
    public async Task Cancel_ResolvesCancelled_AndRemovesDialog()
    {
        var service = new DialogService();

        var reference = service.Show<TestDialogBody>("Edit");
        reference.Cancel();

        var result = await reference.Result;
        Assert.True(result.Cancelled);
        Assert.Null(result.Data);
        Assert.Empty(service.OpenDialogs);
    }

    [Fact]
    public async Task SecondClose_IsIgnored_FirstResultWins()
    {
        var service = new DialogService();

        var reference = service.Show<TestDialogBody>("Edit");
        reference.Instance.Close("first");
        reference.Instance.Cancel(); // should be a no-op

        var result = await reference.Result;
        Assert.False(result.Cancelled);
        Assert.Equal("first", result.GetData<string>());
        Assert.True(reference.Instance.IsClosed);
    }

    [Fact]
    public async Task ShowAsync_ReturnsAwaitableResult()
    {
        var service = new DialogService();

        var task = service.ShowAsync<TestDialogBody>("Edit");
        Assert.False(task.IsCompleted);

        service.OpenDialogs[0].Close("done");

        var result = await task;
        Assert.Equal("done", result.GetData<string>());
    }

    [Fact]
    public void Show_MultipleDialogs_StackInOrder()
    {
        var service = new DialogService();

        var first = service.Show<TestDialogBody>("First");
        var second = service.Show<TestDialogBody>("Second");

        Assert.Equal(2, service.OpenDialogs.Count);
        Assert.Equal("First", service.OpenDialogs[0].Title);
        Assert.Equal("Second", service.OpenDialogs[1].Title);

        first.Cancel();
        Assert.Single(service.OpenDialogs);
        Assert.Equal("Second", service.OpenDialogs[0].Title);
        second.Cancel();
        Assert.Empty(service.OpenDialogs);
    }
}

// ------------------------------------------------------------------------------
// FlareDialogProvider rendering of component dialogs  (bUnit)
// ------------------------------------------------------------------------------

public class C_FlareDialogProviderComponentTests : FlareTestContext
{
    public C_FlareDialogProviderComponentTests()
    {
        Services.AddSingleton<IDialogService, DialogService>();
    }

    [Fact]
    public void NoComponentDialog_NoScrim()
    {
        var cut = Render<FlareDialogProvider>();

        Assert.Empty(cut.FindAll(".flare-dialog-scrim"));
    }

    [Fact]
    public void Show_RendersBodyAndTitle()
    {
        var service = Services.GetRequiredService<IDialogService>();
        var cut = Render<FlareDialogProvider>();

        service.Show<TestDialogBody>("Edit profile",
            new DialogParameters().Add(nameof(TestDialogBody.Payload), "hello"));
        cut.WaitForState(() => cut.FindAll(".flare-dialog-scrim").Count > 0);

        Assert.NotEmpty(cut.FindAll(".test-ok"));
        Assert.Contains("Edit profile", cut.Find(".flare-dialog__title").TextContent);
    }

    [Fact]
    public async Task ClickingBodyButton_ClosesDialog_AndResolvesResult()
    {
        var service = Services.GetRequiredService<IDialogService>();
        var cut = Render<FlareDialogProvider>();

        var reference = service.Show<TestDialogBody>("Edit",
            new DialogParameters().Add(nameof(TestDialogBody.Payload), "hello"));
        cut.WaitForState(() => cut.FindAll(".test-ok").Count > 0);

        cut.Find(".test-ok").Click();
        cut.WaitForState(() => cut.FindAll(".flare-dialog-scrim").Count == 0);

        var result = await reference.Result;
        Assert.False(result.Cancelled);
        Assert.Equal("hello", result.GetData<string>());
        Assert.Empty(service.OpenDialogs);
    }
}
