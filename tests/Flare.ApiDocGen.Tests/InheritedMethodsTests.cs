using Flare.Components;
using Microsoft.AspNetCore.Components;

namespace Flare.ApiDocGen.Tests;

public sealed class InheritedMethodsTests
{
    private static ComponentExtractor Extractor()
    {
        var docs = new XmlDocReader();
        docs.Load([Path.ChangeExtension(typeof(InheritedMethodsTests).Assembly.Location, ".xml"),
            Path.ChangeExtension(typeof(FlareFileUploadZone).Assembly.Location, ".xml")]);
        return new(docs);
    }

    [Fact]
    public void FileUploadCommandsHaveInheritedSignaturesAndDocumentation()
    {
        var methods = Extractor().Extract(typeof(FlareFileUploadZone)).Methods;
        foreach (var name in new[] { "UploadAsync", "CancelAsync", "RetryAsync", "RemoveAsync" })
        {
            var method = Assert.Single(methods, x => x.Name == name);
            Assert.False(string.IsNullOrWhiteSpace(method.Summary), name);
        }
        Assert.DoesNotContain(methods, x => x.Name is "SetParametersAsync" or "Equals" or "GetHashCode" or "DisposeAsync");
    }

    [Fact]
    public void GenericBaseUsesClosedSignaturesAndDefinitionXmlWithoutHiddenDuplicates()
    {
        var methods = Extractor().Extract(typeof(ClosedCommands)).Methods;
        var echo = Assert.Single(methods, x => x.Name == "Echo");
        Assert.Equal("Echo(string value)", echo.Signature);
        Assert.Equal("Echoes the inherited value.", echo.Summary);
        Assert.Equal("The input value.", Assert.Single(echo.Parameters).Summary);
        Assert.Equal("The echoed value.", echo.ReturnSummary);
        var convert = Assert.Single(methods, x => x.Name == "ConvertTo");
        Assert.Equal("ConvertTo<U>(string value, U fallback)", convert.Signature);
        Assert.Equal("Converts an inherited argument.", convert.Summary);
        var format = Assert.Single(methods, x => x.Name == "Format");
        Assert.Equal("Format<TResult>(string value, TResult other)", format.Signature);
        Assert.Equal("Formats an inherited argument.", format.Summary);
        var hidden = Assert.Single(methods, x => x.Name == "Hidden");
        Assert.Equal("The derived command.", hidden.Summary);
        Assert.Equal(2, methods.Count(x => x.Name == "Choose"));
        Assert.DoesNotContain(methods, x => x.Name is "DisposeAsync" or "SetParametersAsync" or "ToString");
    }

    private abstract class Commands<T> : ComponentBase
    {
        /// <summary>Echoes the inherited value.</summary>
        /// <param name="value">The input value.</param>
        /// <returns>The echoed value.</returns>
        public virtual T Echo(T value) => value;
        /// <summary>Converts an inherited argument.</summary>
        public U ConvertTo<U>(T value, U fallback) => fallback;
        /// <summary>Formats an inherited argument.</summary>
        public virtual U Format<U>(T value, U other) => other;
        /// <summary>The base command.</summary>
        public object Hidden(int value) => value;
        /// <summary>Chooses one generic type.</summary>
        public void Choose<TItem>() { }
        /// <summary>Chooses two generic types.</summary>
        public void Choose<TItem, TExtra>() { }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    private sealed class ClosedCommands : Commands<string>
    {
        /// <inheritdoc />
        public override string Echo(string value) => value;
        /// <summary>The derived command.</summary>
        public new string Hidden(int number) => number.ToString();
        /// <inheritdoc />
        public override TResult Format<TResult>(string value, TResult other) => other;
    }
}
