namespace CodeMap.Tests.DeclarationNodes
{
    public interface IFieldDeclarationTests
    {
        void MemberEqualityComparison();

        void HasNameSet();

        void HasDeclartingTypeSet();

        void HasCircularReferenceSet();

        void HasAttributesSet();

        void HasAccessModifierSet();

        void HasIsShadowingSet();

        void HasIsReadOnlySet();

        void HasIsStaticSet();

        void HasTypeSet();

        void HasSummarySet();

        void HasRemarksSet();

        void HasExamplesSet();

        void HasRelatedMembersSet();

        void AcceptVisitor();
    }
}