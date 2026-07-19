#if NETSTANDARD2_0
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Compiler-required marker for <c>init</c> accessors, absent from netstandard2.0. Declaring it
    /// here lets the record-based query model compile for .NET Framework consumers.
    /// </summary>
    internal static class IsExternalInit
    {
    }
}
#endif
