using System.Reflection;

namespace Querio.Tests;

/// <summary>
/// Guards the promise the whole model rests on: a query can be written down and read back. The
/// types are positional records, so a serializer matches JSON to constructor parameters by NAME -
/// and a trimmed publish strips parameter names from anything nothing appears to reflect on. The
/// failure then appears only at run time in Release, as "ConstructorContainsNullParameterNames".
/// </summary>
public sealed class QuerySerializationTrimmingTests
{
    /// <summary>
    /// The descriptor telling the trimmer to keep the assembly whole. Losing it does not break any
    /// build, and does not break a Debug run - it breaks a published app, which is the worst place
    /// to find out.
    /// </summary>
    [Fact]
    public void ShipsTheTrimmerDescriptorThatKeepsTheContractReadable()
    {
        var names = typeof(QuerySpec).Assembly.GetManifestResourceNames();

        Assert.Contains("ILLink.Descriptors.xml", names);
    }

    /// <summary>
    /// Every serialized member has to be reachable through the constructor a serializer will use.
    /// A positional record whose parameters do not line up with its properties cannot be read back
    /// whatever the trimmer does.
    /// </summary>
    [Theory]
    [InlineData(typeof(QuerySpec))]
    [InlineData(typeof(QuerySource))]
    [InlineData(typeof(QueryJoin))]
    [InlineData(typeof(QueryJoinCondition))]
    [InlineData(typeof(QuerySelect))]
    [InlineData(typeof(QueryGroupBy))]
    [InlineData(typeof(QuerySort))]
    [InlineData(typeof(QueryCondition))]
    [InlineData(typeof(QueryFilterGroup))]
    [InlineData(typeof(QueryOperand))]
    [InlineData(typeof(QueryFieldRef))]
    [InlineData(typeof(QueryFunctionCall))]
    [InlineData(typeof(QueryRelativeValue))]
    public void NamesEveryConstructorParameterAfterThePropertyItFills(Type type)
    {
        var constructor = type.GetConstructors()
            .OrderByDescending(candidate => candidate.GetParameters().Length)
            .First();

        foreach (var parameter in constructor.GetParameters())
        {
            Assert.False(
                string.IsNullOrEmpty(parameter.Name),
                $"{type.Name} has a constructor parameter with no name, so it cannot be read back.");

            var property = type.GetProperty(
                parameter.Name!,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            Assert.True(
                property is not null,
                $"{type.Name} takes '{parameter.Name}' but exposes no property of that name, "
                + "so a serializer cannot fill it.");
        }
    }
}
