using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Flare.Css;

namespace Flare.Theming;

public static partial class CssVarMap
{
    /// <summary>Maps an independent component's string properties marked with CssVarAttribute to CSS variables.
    /// Use the result with DesignTokens.Extended or IThemeService.SetCustomTokens. Null values are omitted;
    /// semantic defaults on the record remain available. Getter metadata is cached per type, and values are read
    /// from each supplied instance. The component package need not be known to the core DesignTokens type.</summary>
    /// <typeparam name="T">Token record with public attributed string properties.</typeparam>
    /// <param name="tokens">Default or customized token values.</param>
    /// <returns>A new map suitable for the existing theme injection pipeline.</returns>
    public static Dictionary<string, string> FlattenTokens<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(this T tokens)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(tokens);
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var (name, read) in TokenRecord<T>.Getters.Value)
            if (read(tokens) is { } value) map.Add(name, value);
        return map;
    }

    private static class TokenRecord<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T> where T : class
    {
        internal static readonly Lazy<(string Name, Func<T, string?> Read)[]> Getters = new(Create);

        private static (string Name, Func<T, string?> Read)[] Create()
        {
            var names = new HashSet<string>(StringComparer.Ordinal);
            var result = new List<(string, Func<T, string?>)>();
            foreach (var property in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (property.GetCustomAttribute<CssVarAttribute>() is not { } attribute) continue;
                if (property.PropertyType != typeof(string) || property.GetIndexParameters().Length != 0 || property.GetMethod is not { IsPublic: true } getter)
                    throw new InvalidOperationException($"{typeof(T).Name}.{property.Name}: CssVar requires a readable public string property.");
                if (!names.Add(attribute.Name)) throw new InvalidOperationException($"{typeof(T).Name}: duplicate CSS variable '{attribute.Name}'.");
                result.Add((attribute.Name, getter.CreateDelegate<Func<T, string?>>()));
            }
            return result.ToArray();
        }
    }
}
