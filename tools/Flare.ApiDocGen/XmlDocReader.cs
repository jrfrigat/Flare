using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Flare.ApiDocGen;

/// <summary>
/// Reads compiler-generated XML documentation files and resolves doc comments for
/// reflected members by computing their documentation member IDs (e.g. "P:NS.Type.Prop").
/// </summary>
internal sealed class XmlDocReader
{
    private readonly Dictionary<string, XElement> _members = new(StringComparer.Ordinal);

    /// <summary>Loads and merges every supplied XML documentation file.</summary>
    public void Load(IEnumerable<string> xmlFilePaths)
    {
        foreach (var path in xmlFilePaths)
        {
            if (!File.Exists(path))
                continue;

            XDocument doc;
            try { doc = XDocument.Load(path); }
            catch { continue; }

            var members = doc.Root?.Element("members")?.Elements("member");
            if (members is null)
                continue;

            foreach (var member in members)
            {
                var name = member.Attribute("name")?.Value;
                if (!string.IsNullOrEmpty(name))
                    _members[name!] = member;
            }
        }
    }

    /// <summary>Resolves the doc comment for a type, following &lt;inheritdoc/&gt; up the hierarchy.</summary>
    public XmlMemberDoc? ForType(Type type) => Resolve("T:" + DocId.ForType(type), () => InheritTargetsForType(type));

    /// <summary>Resolves the doc comment for a property, following &lt;inheritdoc/&gt;.</summary>
    public XmlMemberDoc? ForProperty(PropertyInfo property) =>
        Resolve("P:" + DocId.DeclaringName(property.DeclaringType!) + "." + property.Name,
            () => InheritTargetsForProperty(property));

    /// <summary>Resolves the doc comment for a field (e.g. an enum member).</summary>
    public XmlMemberDoc? ForField(Type declaringType, string fieldName) =>
        Resolve("F:" + DocId.DeclaringName(declaringType) + "." + fieldName, () => Array.Empty<string>());

    /// <summary>Resolves the doc comment for a method, following &lt;inheritdoc/&gt;.</summary>
    public XmlMemberDoc? ForMethod(MethodInfo method) =>
        Resolve(DocId.ForMethod(method), () => InheritTargetsForMethod(method));

    private XmlMemberDoc? Resolve(string id, Func<IEnumerable<string>> inheritTargets, int depth = 0)
    {
        if (depth > 8 || !_members.TryGetValue(id, out var element))
        {
            if (depth > 8)
                return null;
            // No element at all: still try inherited definitions.
            return ResolveInherited(inheritTargets, depth);
        }

        // Explicit <inheritdoc/> with no own content -> resolve from base definitions.
        if (element.Element("inheritdoc") is not null && element.Element("summary") is null)
            return ResolveInherited(inheritTargets, depth) ?? Parse(element);

        return Parse(element);
    }

    private XmlMemberDoc? ResolveInherited(Func<IEnumerable<string>> inheritTargets, int depth)
    {
        foreach (var targetId in inheritTargets())
        {
            if (_members.TryGetValue(targetId, out var baseElement))
                return Parse(baseElement);
        }
        return null;
    }

    private static IEnumerable<string> InheritTargetsForType(Type type)
    {
        for (var b = type.BaseType; b is not null && b != typeof(object); b = b.BaseType)
            yield return "T:" + DocId.ForType(b);
        foreach (var i in type.GetInterfaces())
            yield return "T:" + DocId.ForType(i);
    }

    private static IEnumerable<string> InheritTargetsForProperty(PropertyInfo property)
    {
        var name = property.Name;
        for (var b = property.DeclaringType?.BaseType; b is not null && b != typeof(object); b = b.BaseType)
            yield return "P:" + DocId.DeclaringName(b) + "." + name;
    }

    private static IEnumerable<string> InheritTargetsForMethod(MethodInfo method)
    {
        for (var b = method.DeclaringType?.BaseType; b is not null && b != typeof(object); b = b.BaseType)
        {
            var candidate = b.GetMethod(method.Name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                binder: null,
                types: Array.ConvertAll(method.GetParameters(), p => p.ParameterType),
                modifiers: null);
            if (candidate is not null)
                yield return DocId.ForMethod(candidate);
        }
    }

    private static XmlMemberDoc Parse(XElement member)
    {
        var paramDocs = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var p in member.Elements("param"))
        {
            var n = p.Attribute("name")?.Value;
            if (!string.IsNullOrEmpty(n))
                paramDocs[n!] = NormalizeText(p);
        }

        return new XmlMemberDoc(
            Summary: TextOrNull(member.Element("summary")),
            Remarks: TextOrNull(member.Element("remarks")),
            Returns: TextOrNull(member.Element("returns")),
            Value: TextOrNull(member.Element("value")),
            Parameters: paramDocs);
    }

    private static string? TextOrNull(XElement? element)
    {
        if (element is null)
            return null;
        var text = NormalizeText(element);
        return string.IsNullOrWhiteSpace(text) ? null : text;
    }

    /// <summary>Flattens XML doc inline markup (&lt;see&gt;, &lt;c&gt;, &lt;para&gt;, &lt;paramref&gt;) into readable text.</summary>
    private static string NormalizeText(XElement element)
    {
        var sb = new StringBuilder();
        AppendNodes(element, sb);
        // Collapse all runs of whitespace into single spaces.
        var collapsed = new StringBuilder(sb.Length);
        var lastWasSpace = false;
        foreach (var ch in sb.ToString())
        {
            if (char.IsWhiteSpace(ch))
            {
                if (!lastWasSpace)
                    collapsed.Append(' ');
                lastWasSpace = true;
            }
            else
            {
                collapsed.Append(ch);
                lastWasSpace = false;
            }
        }
        return collapsed.ToString().Trim();
    }

    private static void AppendNodes(XElement parent, StringBuilder sb)
    {
        foreach (var node in parent.Nodes())
        {
            switch (node)
            {
                case XText text:
                    sb.Append(text.Value);
                    break;
                case XElement el:
                    AppendElement(el, sb);
                    break;
            }
        }
    }

    private static void AppendElement(XElement el, StringBuilder sb)
    {
        switch (el.Name.LocalName)
        {
            case "see":
            case "seealso":
                sb.Append(ReferenceText(el));
                break;
            case "paramref":
            case "typeparamref":
                sb.Append(el.Attribute("name")?.Value ?? string.Empty);
                break;
            case "c":
            case "code":
                sb.Append(el.Value.Trim());
                break;
            case "para":
                sb.Append(' ');
                AppendNodes(el, sb);
                sb.Append(' ');
                break;
            default:
                AppendNodes(el, sb);
                break;
        }
    }

    private static string ReferenceText(XElement el)
    {
        var langword = el.Attribute("langword")?.Value;
        if (!string.IsNullOrEmpty(langword))
            return langword!;

        var cref = el.Attribute("cref")?.Value;
        if (!string.IsNullOrEmpty(cref))
        {
            // Strip the "T:"/"P:"/"M:"... prefix and reduce to the last identifier segment.
            var value = cref!;
            var colon = value.IndexOf(':');
            if (colon >= 0)
                value = value[(colon + 1)..];
            var paren = value.IndexOf('(');
            if (paren >= 0)
                value = value[..paren];
            var lastDot = value.LastIndexOf('.');
            if (lastDot >= 0)
                value = value[(lastDot + 1)..];
            var tick = value.IndexOf('`');
            if (tick >= 0)
                value = value[..tick];
            return value;
        }

        return el.Attribute("href")?.Value ?? el.Value.Trim();
    }
}
