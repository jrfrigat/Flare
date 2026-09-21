using System.Globalization;
using Flare.Abstractions;
using Flare.Components;

namespace Flare.Components.Tests;

public class CoreDefaultLabelsLocalizationTests : FlareTestContext
{
    [Fact]
    public void RussianCulture_LocalizesVisibleAndAccessibleDefaults()
    {
        WithUiCulture("ru-RU", () =>
        {
            var splitButton = Render<FlareSplitButton>(p => p.AddChildContent("Save"));
            Assert.Equal("Другие действия", splitButton.Find($"button.{Css.Classes.SplitButton.Trigger}").GetAttribute("aria-label"));

            var drawer = Render<FlareDrawer>();
            Assert.Equal("Боковая панель", drawer.Find($".{Css.Classes.Drawer.Root}").GetAttribute("aria-label"));

            var colorMode = Render<FlareColorModeToggle>(p => p.Add(x => x.ShowLabel, true));
            Assert.Equal("Темная", colorMode.Find($".{Css.Classes.Color.ModeToggleLabel}").TextContent);
            Assert.Equal("Переключить на темную тему", colorMode.Find("button").GetAttribute("aria-label"));

            var stepper = Render<FlareStepper>(p => p.AddChildContent<FlareStep>(s => s
                .Add(x => x.Label, "Step")
                .Add(x => x.Optional, true)));
            Assert.Equal("Необязательно", stepper.Find($".{Css.Classes.Stepper.Optional}").TextContent);

            var splitter = Render<FlareSplitter>();
            Assert.Equal("Изменить размер", splitter.Find($".{Css.Classes.Splitter.Root}").GetAttribute("aria-label"));

            var password = Render<FlarePasswordField>();
            var reveal = password.FindAll("button").Single(button => button.GetAttribute("aria-pressed") is not null);
            Assert.Equal("Показать пароль", reveal.GetAttribute("aria-label"));
        });
    }

    [Fact]
    public void ExplicitOverrides_TakePriorityOverLocalizedDefaults()
    {
        WithUiCulture("ru-RU", () =>
        {
            var splitButton = Render<FlareSplitButton>(p => p
                .Add(x => x.MenuAriaLabel, "Actions")
                .AddChildContent("Save"));
            Assert.Equal("Actions", splitButton.Find($"button.{Css.Classes.SplitButton.Trigger}").GetAttribute("aria-label"));

            var drawer = Render<FlareDrawer>(p => p.Add(x => x.AriaLabel, "Navigation"));
            Assert.Equal("Navigation", drawer.Find($".{Css.Classes.Drawer.Root}").GetAttribute("aria-label"));

            var colorMode = Render<FlareColorModeToggle>(p => p
                .Add(x => x.ShowLabel, true)
                .Add(x => x.DarkLabel, "Night"));
            Assert.Equal("Night", colorMode.Find($".{Css.Classes.Color.ModeToggleLabel}").TextContent);

            var stepper = Render<FlareStepper>(p => p.AddChildContent<FlareStep>(s => s
                .Add(x => x.Label, "Step")
                .Add(x => x.Optional, true)
                .Add(x => x.OptionalText, "Skippable")));
            Assert.Equal("Skippable", stepper.Find($".{Css.Classes.Stepper.Optional}").TextContent);

            var splitter = Render<FlareSplitter>(p => p.Add(x => x.AriaLabel, "Adjust"));
            Assert.Equal("Adjust", splitter.Find($".{Css.Classes.Splitter.Root}").GetAttribute("aria-label"));
        });
    }

    private static void WithUiCulture(string name, Action action)
    {
        var previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(name);
            action();
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }
}
