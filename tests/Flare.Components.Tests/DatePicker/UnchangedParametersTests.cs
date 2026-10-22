using System.Globalization;
using System.Linq.Expressions;
using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-187: a closed picker is not re-rendered when its parent re-renders with the same values and new callback
/// lambdas, so one field committing a value does not redraw every field on the page - while any change that can
/// alter its markup still does, and its own events and validation still render.
/// </summary>
public class UnchangedParametersTests : FlareTestContext
{
    private static readonly CultureInfo Ru = CultureInfo.GetCultureInfo("ru-RU");
    private static readonly DateOnly Day = new(2026, 10, 15);

    // A page of fields: every render passes each field its value and a new callback lambda, as Razor markup does.
    private sealed class EditablePage : ComponentBase
    {
        public string? Text;
        public string? OtherText;
        public int Number;
        public string Otp = string.Empty;

        protected override void BuildRenderTree(RenderTreeBuilder b)
        {
            // Capture a local, as Razor markup does for a loop item: a lambda that captures only `this`
            // is cached by the compiler, so the callback would be the same instance on every render.
            var owner = this;
            b.OpenComponent<FlareTextField>(0);
            b.AddAttribute(1, nameof(FlareTextField.Value), Text);
            b.AddAttribute(2, nameof(FlareTextField.ValueChanged),
                EventCallback.Factory.Create<string?>(owner, value => owner.Text = value));
            b.CloseComponent();

            b.OpenComponent<FlareTextField>(10);
            b.AddAttribute(11, nameof(FlareTextField.Value), OtherText);
            b.AddAttribute(12, nameof(FlareTextField.ValueChanged),
                EventCallback.Factory.Create<string?>(owner, value => owner.OtherText = value));
            b.CloseComponent();

            b.OpenComponent<FlareNumericField<int>>(20);
            b.AddAttribute(21, "Value", Number);
            b.AddAttribute(22, "ValueChanged",
                EventCallback.Factory.Create<int>(owner, value => owner.Number = value));
            b.CloseComponent();

            b.OpenComponent<FlareOtpField>(30);
            b.AddAttribute(31, "Value", Otp);
            b.AddAttribute(32, "ValueChanged",
                EventCallback.Factory.Create<string>(owner, value => owner.Otp = value));
            b.CloseComponent();
        }
    }

    [Fact]
    public void CommittingAnEditableField_LeavesTheOtherEditableFieldsAlone()
    {
        var page = Render<EditablePage>();
        var texts = page.FindComponents<FlareTextField>();
        var numeric = page.FindComponents<FlareNumericField<int>>().Single();
        var otp = page.FindComponents<FlareOtpField>().Single();
        var (first, second) = (texts[0].RenderCount, texts[1].RenderCount);
        var (numberCount, otpCount) = (numeric.RenderCount, otp.RenderCount);

        texts[0].Find("input").Change("changed");

        Assert.Equal("changed", page.Instance.Text);
        Assert.True(texts[0].RenderCount > first);
        Assert.Equal(second, texts[1].RenderCount);
        Assert.Equal(numberCount, numeric.RenderCount);
        Assert.Equal(otpCount, otp.RenderCount);
    }

    [Fact]
    public void PasswordStrengthEvaluatorChange_IsRendered()
    {
        Func<string?, FlarePasswordStrength> weak = _ => new(1, []);
        Func<string?, FlarePasswordStrength> strong = _ => new(4, []);
        var cut = Render<FlarePasswordField>(p => p
            .Add(x => x.ShowStrength, true)
            .Add(x => x.Value, "abc")
            .Add(x => x.StrengthEvaluator, weak));
        var count = cut.RenderCount;

        cut.Render(p => p
            .Add(x => x.ShowStrength, true)
            .Add(x => x.Value, "abc")
            .Add(x => x.StrengthEvaluator, strong));

        Assert.True(cut.RenderCount > count);
        Assert.Contains(FlareStrings.Password_Strong, cut.Find($".{Css.Classes.Input.StrengthLabel}").TextContent);
    }

    private sealed class Page : ComponentBase
    {
        public readonly DateOnly?[] Dates = new DateOnly?[3];
        public readonly TimeOnly?[] Times = new TimeOnly?[3];

        protected override void BuildRenderTree(RenderTreeBuilder b)
        {
            for (var i = 0; i < 3; i++)
            {
                var index = i;
                b.OpenComponent<FlareDatePicker>(0);
                b.SetKey(index);
                b.AddAttribute(1, nameof(FlareDatePicker.Label), "Date");
                b.AddAttribute(2, nameof(FlareDatePicker.Culture), Ru);
                b.AddAttribute(3, nameof(FlareDatePicker.Value), Dates[index]);
                b.AddAttribute(4, nameof(FlareDatePicker.ValueChanged),
                    EventCallback.Factory.Create<DateOnly?>(this, v => Dates[index] = v));
                b.AddAttribute(5, nameof(FlareDatePicker.IsDateDisabled), (Func<DateOnly, bool>)(d => d.Day == index));
                b.CloseComponent();
            }
            for (var i = 0; i < 3; i++)
            {
                var index = i;
                b.OpenComponent<FlareTimePicker>(10);
                b.SetKey(100 + index);
                b.AddAttribute(11, nameof(FlareTimePicker.Value), Times[index]);
                b.AddAttribute(12, nameof(FlareTimePicker.ValueChanged),
                    EventCallback.Factory.Create<TimeOnly?>(this, v => Times[index] = v));
                b.CloseComponent();
            }
        }
    }

    [Fact]
    public void CommittingOneField_DoesNotRerenderTheOthers()
    {
        var page = Render<Page>();
        var dates = page.FindComponents<FlareDatePicker>();
        var times = page.FindComponents<FlareTimePicker>();
        var before = dates.Select(d => d.RenderCount).Concat(times.Select(t => t.RenderCount)).ToArray();

        dates[0].Find("input").Change("15.10.2026");

        Assert.Equal(Day, page.Instance.Dates[0]);
        Assert.Equal(Day.ToString("dd.MM.yyyy", Ru), dates[0].Find("input").GetAttribute("value"));
        Assert.True(dates[0].RenderCount > before[0]);
        Assert.Equal(before[1..], dates.Skip(1).Select(d => d.RenderCount).Concat(times.Select(t => t.RenderCount)));
    }

    private IRenderedComponent<FlareDatePicker> Picker(Action<ComponentParameterCollectionBuilder<FlareDatePicker>>? more = null) =>
        Render<FlareDatePicker>(p =>
        {
            p.Add(x => x.Label, "Date").Add(x => x.Culture, Ru).Add(x => x.Value, Day)
             .Add(x => x.ValueChanged, (DateOnly? _) => { }).Add(x => x.IsDateDisabled, d => false);
            more?.Invoke(p);
        });

    private static void Push(IRenderedComponent<FlareDatePicker> cut, string label = "Date", DateOnly? value = null,
        Action<ComponentParameterCollectionBuilder<FlareDatePicker>>? more = null) =>
        cut.Render(p =>
        {
            p.Add(x => x.Label, label).Add(x => x.Culture, Ru).Add(x => x.Value, value ?? Day)
             .Add(x => x.ValueChanged, (DateOnly? _) => { }).Add(x => x.IsDateDisabled, d => false);
            more?.Invoke(p);
        });

    [Fact]
    public void SameValuesAndNewLambdas_AreNotRendered()
    {
        var cut = Picker();
        Push(cut);
        var count = cut.RenderCount;
        Push(cut);
        Assert.Equal(count, cut.RenderCount);
    }

    [Fact]
    public void AnyChangedValue_IsRendered()
    {
        var cut = Picker();
        Push(cut);
        var count = cut.RenderCount;

        Push(cut, label: "Start");
        Assert.True(cut.RenderCount > count); count = cut.RenderCount;
        Assert.Contains("Start", cut.Markup);

        Push(cut, label: "Start", value: Day.AddDays(1));
        Assert.True(cut.RenderCount > count); count = cut.RenderCount;
        Assert.Equal(Day.AddDays(1).ToString("dd.MM.yyyy", Ru), cut.Find("input").GetAttribute("value"));

        Push(cut, label: "Start", value: Day.AddDays(1), more: p => p.Add(x => x.Disabled, true));
        Assert.True(cut.RenderCount > count); count = cut.RenderCount;
        Assert.True(cut.Find("input").HasAttribute("disabled"));
    }

    [Fact]
    public void NewDelegate_IsRendered_WhileTheCalendarIsShown()
    {
        var cut = Picker();
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        Push(cut);
        var count = cut.RenderCount;
        Push(cut);
        Assert.True(cut.RenderCount > count);
    }

    [Fact]
    public void Inline_ReadsItsDelegates_AndIsRendered()
    {
        var cut = Picker(p => p.Add(x => x.Inline, true));
        Push(cut, more: p => p.Add(x => x.Inline, true));
        var count = cut.RenderCount;
        cut.Render(p => p.Add(x => x.Label, "Date").Add(x => x.Culture, Ru).Add(x => x.Value, Day)
            .Add(x => x.ValueChanged, (DateOnly? _) => { }).Add(x => x.IsDateDisabled, d => d == Day).Add(x => x.Inline, true));
        Assert.True(cut.RenderCount > count);
        Assert.Contains(cut.FindAll("button[role=gridcell]"), b => b.TextContent.Trim() == "15" && b.HasAttribute("disabled"));
    }

    [Fact]
    public void DelegateAddedOrRemoved_IsRendered()
    {
        var cut = Picker();
        Push(cut);
        var count = cut.RenderCount;
        Push(cut, more: p => p.Add(x => x.ParseInput, s => null));
        Assert.True(cut.RenderCount > count);
    }

    [Fact]
    public void LabelContent_IsAlwaysRendered()
    {
        var cut = Picker(p => p.Add(x => x.LabelContent, b => b.AddContent(0, "A")));
        Push(cut, more: p => p.Add(x => x.LabelContent, b => b.AddContent(0, "A")));
        var count = cut.RenderCount;
        Push(cut, more: p => p.Add(x => x.LabelContent, b => b.AddContent(0, "B")));
        Assert.True(cut.RenderCount > count);
        Assert.Contains("B", cut.Find($".{Css.Classes.Input.Label}").TextContent);
    }

    [Fact]
    public void Attributes_AreComparedByContent()
    {
        var cut = Picker(p => p.AddUnmatched("data-x", "1"));
        Push(cut, more: p => p.AddUnmatched("data-x", "1"));
        var count = cut.RenderCount;
        Push(cut, more: p => p.AddUnmatched("data-x", "1"));
        Assert.Equal(count, cut.RenderCount);
        Push(cut, more: p => p.AddUnmatched("data-x", "2"));
        Assert.True(cut.RenderCount > count);
        Assert.Equal("2", cut.Find($".{Css.Classes.DatePicker.Root}").GetAttribute("data-x"));
    }

    [Fact]
    public void AttributeDictionary_WithAChangedEventCallback_CountsAsChanged()
    {
        var fired = 0;
        EventCallback<MouseEventArgs> Callback(int value) => EventCallback.Factory.Create<MouseEventArgs>(this, _ => fired = value);
        Dictionary<string, object?> Attributes(EventCallback<MouseEventArgs> callback) =>
            new() { ["onclick"] = callback, ["data-x"] = "1" };
        var parameters = new UnchangedParameters();
        Assert.NotEqual(Callback(1), Callback(2));
        ParameterView Push(EventCallback<MouseEventArgs> callback) =>
            ParameterView.FromDictionary(new Dictionary<string, object?> { ["AdditionalAttributes"] = Attributes(callback) });

        Assert.False(parameters.Matches(Push(Callback(1)), false));
        Assert.False(parameters.Matches(Push(Callback(2)), false));
    }

    [Fact]
    public void ReplacedEventCallbackInAttributes_IsRendered_AndTheNewHandlerRuns()
    {
        var fired = 0;
        EventCallback<MouseEventArgs> Callback(int value) => EventCallback.Factory.Create<MouseEventArgs>(this, _ => fired = value);
        static Dictionary<string, object> Attributes(EventCallback<MouseEventArgs> callback) => new() { ["onclick"] = callback };
        var cut = Picker(p => p.Add(x => x.AdditionalAttributes, Attributes(Callback(1))));
        Push(cut, more: p => p.Add(x => x.AdditionalAttributes, Attributes(Callback(1))));
        var count = cut.RenderCount;

        Push(cut, more: p => p.Add(x => x.AdditionalAttributes, Attributes(Callback(2))));

        Assert.True(cut.RenderCount > count);
        cut.Find($".{Css.Classes.DatePicker.Root}").Click();
        Assert.Equal(2, fired);
    }

    [Fact]
    public void ListParameter_IsAlwaysRendered()
    {
        var values = new List<DateOnly> { Day };
        var cut = Render<FlareMultiDatePicker>(p => p.Add(x => x.Culture, Ru).Add(x => x.Values, values));
        cut.Render(p => p.Add(x => x.Culture, Ru).Add(x => x.Values, values));
        var count = cut.RenderCount;
        values.Add(Day.AddDays(1));
        cut.Render(p => p.Add(x => x.Culture, Ru).Add(x => x.Values, values));
        Assert.True(cut.RenderCount > count);
    }

    [Fact]
    public void OwnEvents_StillRender()
    {
        var cut = Picker();
        Push(cut);
        Push(cut);
        var count = cut.RenderCount;
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        Assert.True(cut.RenderCount > count);
        Assert.NotEmpty(cut.FindAll("[role=dialog]"));
    }

    private sealed class Model { public DateOnly? When { get; set; } }

    private static Expression<Func<DateOnly?>> ForWhen(Model row) => () => row.When;

    [Fact]
    public void For_OverTheSameModel_IsNotRendered_AndOverAnotherModel_Is()
    {
        var first = new Model();
        var cut = Picker(p => p.Add(x => x.For, ForWhen(first)));
        Push(cut, more: p => p.Add(x => x.For, ForWhen(first)));
        var count = cut.RenderCount;
        Push(cut, more: p => p.Add(x => x.For, ForWhen(first)));
        Assert.Equal(count, cut.RenderCount);
        Push(cut, more: p => p.Add(x => x.For, ForWhen(new Model())));
        Assert.True(cut.RenderCount > count);
    }

    [Fact]
    public void Validation_StillRenders()
    {
        var model = new Model();
        var ctx = new EditContext(model);
        var store = new ValidationMessageStore(ctx);
        RenderFragment Form() => b =>
        {
            b.OpenComponent<CascadingValue<EditContext>>(0);
            b.AddAttribute(1, "Value", ctx);
            b.AddAttribute(2, "ChildContent", (RenderFragment)(c =>
            {
                c.OpenComponent<FlareDatePicker>(0);
                c.AddAttribute(1, nameof(FlareDatePicker.Value), model.When);
                c.AddAttribute(2, nameof(FlareDatePicker.For), (Expression<Func<DateOnly?>>)(() => model.When));
                c.CloseComponent();
            }));
            b.CloseComponent();
        };
        var cut = Render(Form());

        store.Add(ctx.Field(nameof(Model.When)), "Pick a date");
        ctx.NotifyValidationStateChanged();

        Assert.Contains("Pick a date", cut.Find($".{Css.Classes.Input.HelperError}").TextContent);
    }
}
