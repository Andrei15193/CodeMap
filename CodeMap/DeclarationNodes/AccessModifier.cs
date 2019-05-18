namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents the access modifier of a declared member of an assembly.</summary>
    public enum AccessModifier
    {
        /// <summary>The member is visible only to the declaring type.</summary>
        Private,
        /// <summary>The member is visible only to the declaring assembly and only to subtypes declared by the same assembly.</summary>
        FamilyAndAssembly,
        /// <summary>The member is visible only to the declaring assembly.</summary>
        Assembly,
        /// <summary>The member is visible to all subtypes.</summary>
        Family,
        /// <summary>The member is visible to the declaring assembly and to all subtypes declared by any assembly.</summary>
        FamilyOrAssembly,
        /// <summary>The member is visible to all types regradless of the assembly they are declared by.</summary>
        Public
    }
}