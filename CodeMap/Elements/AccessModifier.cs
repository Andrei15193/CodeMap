namespace CodeMap.Elements
{
    /// <summary>Represents the access modifier of a declared member of an assembly.</summary>
    public enum AccessModifier
    {
        /// <summary>The member is visible only to the declaring type.</summary>
        Private,
        /// <summary>The member is visible only to the declaring assembly.</summary>
        Assembly,
        /// <summary>The member is visible only to the declaring assembly or subtypes declared by any assembly.</summary>
        AssemblyOrFamily,
        /// <summary>The member is visible only to the declaring assembly and subtypes declared in the same assembly.</summary>
        AssemblyAndFamily,
        /// <summary>The member is visible to all subtypes.</summary>
        Family,
        /// <summary>The member is visible to all members regradless of the assembly they are declared by.</summary>
        Public
    }
}