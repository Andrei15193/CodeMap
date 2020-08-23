namespace CodeMap.Tests.DeclarationNodes
{
    public interface IConstantDeclarationTests
    {
        void MemberEqualityComparison();

        void HasNameSet();

        void HasDeclartingTypeSet();

        void HasCircularReferenceSet();

        void HasAttributesSet();

        void HasAccessModifierSet();

        void HasIsShadowingSet();

        void HasTypeSet();

        void HasValueSet();

        void HasSummarySet();

        void HasRemarksSet();

        void HasExamplesSet();

        void HasRelatedMembersSet();

        void AcceptVisitor();
    }
}