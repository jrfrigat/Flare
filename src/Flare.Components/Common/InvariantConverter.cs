using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Flare.Components;

/// <summary>
/// The <see cref="TypeConverter"/> a field turns its text into a value with, read invariantly. The converters of the
/// built-in value types - numbers, dates and times, <see cref="Guid"/>, enums and their nullable forms - are
/// registered with <see cref="TypeDescriptor"/> intrinsically and survive trimming; a custom value type with its
/// own <see cref="TypeConverterAttribute"/> is kept by the app that uses it.
/// </summary>
internal static class InvariantConverter
{
    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode",
        Justification = "The converters of the built-in value types are intrinsic to TypeDescriptor and are not trimmed.")]
    [UnconditionalSuppressMessage("Trimming", "IL2067:UnrecognizedReflectionPattern",
        Justification = "The converters of the built-in value types are intrinsic to TypeDescriptor and are not trimmed.")]
    public static TypeConverter For(Type type) => TypeDescriptor.GetConverter(type);
}
