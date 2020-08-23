namespace CodeMap.Tests.DeclarationNodes
{
    public interface IEnumDeclarationTests
    {
        void TypeEqualityComparison();

        void HasNameSet();

        void HasAssemblySet();

        void HasAccessModifierSet();

        void HasUnderlyingTypeSet();

        void HasCircularReferenceSet();

        void HasAttributesSet();

        void HasDeclaringTypeSet();

        void HasMembersSet();

        void HasSummarySet();

        void HasRemarksSet();

        void HasExamplesSet();

        void HasRelatedMembersSet();

        void AcceptVisitor();
    }
}