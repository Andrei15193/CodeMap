using CodeMap.ReferenceData;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented global namespace.</summary>
    public sealed class GlobalNamespaceDeclaration : NamespaceDeclaration
    {
        internal GlobalNamespaceDeclaration(NamespaceReference namespaceReference)
            : base(namespaceReference)
        {
        }
    }
}