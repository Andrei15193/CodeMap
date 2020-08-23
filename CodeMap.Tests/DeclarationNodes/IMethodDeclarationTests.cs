namespace CodeMap.Tests.DeclarationNodes
{
    public interface IMethodDeclarationTests
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

        void HasGenericParametersSet();

        void HasParametersSet();

        void HasReturnAttributesSet();

        void HasReturnTypeSet();

        void HasSummarySet();

        void HasRemarksSet();

        void HasExamplesSet();

        void HasRelatedMembersSet();

        void HasReturnDescriptionSet();

        void HasExceptionsSet();

        void AcceptVisitor();
    }
}