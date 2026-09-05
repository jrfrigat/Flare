using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareFormBuilderTests : FlareTestContext
{
    private sealed class Person
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
    }

    [Fact]
    public void RendersFieldsAndSubmitButton()
    {
        var cut = Render<FlareFormBuilder<Person>>(p => p
            .Add(x => x.Model, new Person()));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Formbuilder.Root}"));
        Assert.NotEmpty(cut.FindAll("input"));        // a field per model property
        Assert.NotEmpty(cut.FindAll("button[type=submit]"));
    }
}
