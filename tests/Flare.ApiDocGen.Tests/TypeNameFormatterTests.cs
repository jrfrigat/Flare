using System.Reflection;
using Flare.Components;

namespace Flare.ApiDocGen.Tests;

/// <summary>
/// Guards the type strings the reference prints. The compiler's NullableAttribute is the only source
/// that tells <c>T</c> from <c>T?</c> for an unconstrained generic parameter, so these tests exercise
/// the real annotations the compiler emits into this assembly rather than hand-built flag arrays.
/// </summary>
public class TypeNameFormatterTests
{
    private sealed class Callback<T>
    {
        public T Value { get; init; } = default!;
    }

    private sealed class Sample<T>
    {
        public T Plain { get; set; } = default!;
        public T? Annotated { get; set; }
        public Callback<T> PlainCallback { get; set; } = new();
        public Callback<T?> AnnotatedCallback { get; set; } = new();
        public Dictionary<string, T> PlainMap { get; set; } = [];
        public Dictionary<string, T?> AnnotatedMap { get; set; } = [];
        public T[] PlainArray { get; set; } = [];
        public T[]? NullableArray { get; set; }
        public T?[] ArrayOfAnnotated { get; set; } = [];
        public Func<T, string?> Function { get; set; } = _ => null;
        public string? NullableText { get; set; }
        public string Text { get; set; } = string.Empty;
        public string?[]? NullableTextArray { get; set; }
        public int Number { get; set; }
        public int? NullableNumber { get; set; }
    }

    private static string Format(Type owner, string property)
    {
        var info = owner.GetProperty(property) ?? throw new InvalidOperationException($"No property {property}");
        return TypeNameFormatter.Format(info.PropertyType, NullabilityAnnotations.Read(info));
    }

    [Theory]
    [InlineData(nameof(Sample<int>.Plain), "T")]
    [InlineData(nameof(Sample<int>.Annotated), "T?")]
    [InlineData(nameof(Sample<int>.PlainCallback), "Callback<T>")]
    [InlineData(nameof(Sample<int>.AnnotatedCallback), "Callback<T?>")]
    [InlineData(nameof(Sample<int>.PlainMap), "Dictionary<string, T>")]
    [InlineData(nameof(Sample<int>.AnnotatedMap), "Dictionary<string, T?>")]
    [InlineData(nameof(Sample<int>.PlainArray), "T[]")]
    [InlineData(nameof(Sample<int>.NullableArray), "T[]?")]
    [InlineData(nameof(Sample<int>.ArrayOfAnnotated), "T?[]")]
    [InlineData(nameof(Sample<int>.Function), "Func<T, string?>")]
    [InlineData(nameof(Sample<int>.NullableText), "string?")]
    [InlineData(nameof(Sample<int>.Text), "string")]
    [InlineData(nameof(Sample<int>.NullableTextArray), "string?[]?")]
    [InlineData(nameof(Sample<int>.Number), "int")]
    [InlineData(nameof(Sample<int>.NullableNumber), "int?")]
    public void Types_KeepTheCompilerAnnotations(string property, string expected) =>
        Assert.Equal(expected, Format(typeof(Sample<>), property));

    [Theory]
    [InlineData("Value", "TValue")]
    [InlineData("ValueChanged", "EventCallback<TValue>")]
    public void NonNullableComponentParameter_IsNotMarkedNullable(string property, string expected)
    {
        var info = typeof(FlareCheckbox<>).GetProperty(property)!;
        Assert.Equal(expected, TypeNameFormatter.Format(info.PropertyType, NullabilityAnnotations.Read(info)));
    }

    [Theory]
    [InlineData("Value", "TValue?")]
    [InlineData("ValueChanged", "EventCallback<TValue?>")]
    public void NullableComponentParameter_StaysNullable(string property, string expected)
    {
        var info = typeof(FlareSelect<>).GetProperty(property)!;
        Assert.Equal(expected, TypeNameFormatter.Format(info.PropertyType, NullabilityAnnotations.Read(info)));
    }

    [Fact]
    public void SupportingTypeDelegate_KeepsItsPlainAndNullableArguments()
    {
        var info = typeof(FlareTableColumn<>).GetProperty(nameof(FlareTableColumn<int>.Field))!;
        Assert.Equal("Func<TItem, object?>?", TypeNameFormatter.Format(info.PropertyType, NullabilityAnnotations.Read(info)));
    }

    [Fact]
    public void PropertyWithoutAnnotations_IsFormattedWithoutThem()
    {
        var info = typeof(Sample<>).GetProperty(nameof(Sample<int>.Plain))!;
        Assert.Equal("T", TypeNameFormatter.Format(info.PropertyType));
    }
}
