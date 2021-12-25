namespace CodeMap.Tests.DeclarationNodes
{
    public interface INamespaceDeclarationTests
    {
        void HasNameSet();

        void HasAssemblySet();

        void HasDeclaredMembersSet();

        void HasEnumsSet();

        void HasDelegatesSet();

        void HasInterfacesSet();

        void HasClassesSet();

        void HasStructsSet();

        void HasRecordsSet();

        void HasCircularReferenceSet();

        void HasSummarySet();

        void HasRemarksSet();

        void HasExamplesSet();

        void HasRelatedMembersSet();

        void ApplyNullAdditionsThrowsException();

        void ApplySummaryDocumentationAddition();

        void ApplyRemarksDocumentationAddition();

        void ApplyExamplesDocumentationAddition();

        void ApplyRelatedMembersDocumenationAddition();

        void AcceptVisitor();
    }
}