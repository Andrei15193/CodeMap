namespace CodeMap.Tests.DeclarationNodes
{
    public interface IAssemblyDeclarationTests
    {
        void AssemblyEqualityComparison();

        void AssemblyNameEqualityComparison();

        void HasNameSet();

        void HasCultureSet();

        void HasPublicKeyTokenSet();

        void HasVersionSet();

        void HasAttributesSet();

        void HasNamespacesSet();

        void HasDependenciesSet();

        void HasSummarySet();

        void HasRemarksSet();

        void HasExamplesSet();

        void HasRelatedMembersSet();

        void ApplyNullAdditionsThrowsException();

        void ApplySummaryDocumentationAddition();

        void ApplyRemarksDocumentationAddition();

        void ApplyExamplesDocumentationAddition();

        void ApplyRelatedMembersDocumenationAddition();

        void ApplyNamespaceDocumentationAddition();

        void AcceptVisitor();
    }
}