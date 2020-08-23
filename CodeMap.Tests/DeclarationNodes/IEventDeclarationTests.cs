namespace CodeMap.Tests.DeclarationNodes
{
    public interface IEventDeclarationTests
    {
        void MemberEqualityComparison();

        void HasNameSet();

        void HasAccessModifierSet();

        void HasDeclartingTypeSet();

        void HasCircularReferenceSet();

        void HasAttributesSet();

        void HasIsAbstractSet();

        void HasIsOverrideSet();

        void HasIsSealedSet();

        void HasIsShadowingSet();

        void HasIsStaticSet();

        void HasIsVirtualSet();

        void HasAdderAttributesSet();

        void HasAdderReturnAttributesSet();

        void HasRemoverAttributesSet();

        void HasRemoverReturnAttributesSet();

        void HasTypeSet();

        void HasSummarySet();

        void HasRemarksSet();

        void HasExamplesSet();

        void HasRelatedMembersSet();

        void HasExceptionsSet();

        void AcceptVisitor();
    }
}