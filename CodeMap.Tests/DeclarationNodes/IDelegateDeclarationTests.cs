namespace CodeMap.Tests.DeclarationNodes
{
    public interface IDelegateDeclarationTests
    {
        void TypeEqualityComparison();

        void HasNameSet();

        void HasAssemblySet();

        void HasAccessModifierSet();

        void HasCircularReferenceSet();

        void HasAttributesSet();

        void HasGenericParametersSet();

        void HasParametersSet();

        void HasReturnAttributes();

        void HasReturnTypeSet();

        void HasDeclaringTypeSet();

        void HasSummarySet();

        void HasRemarksSet();

        void HasExamplesSet();

        void HasRelatedMembersSet();

        void HasReturnDescriptionSet();

        void HasExceptionsSet();

        void AcceptVisitor();
    }
}