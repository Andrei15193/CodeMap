#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a base class for type reference such as concrete types, generic type definitions, arrays and so on.</summary>
    public abstract class BaseTypeReference : MemberReference
    {
        internal BaseTypeReference()
        {
        }
    }
}