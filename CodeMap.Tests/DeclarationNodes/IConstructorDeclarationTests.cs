namespace CodeMap.Tests.DeclarationNodes
{
    public interface IConstructorDeclarationTests
    {
        void HasNameSet();

        void HasAccessModifierSet();

        void HasDeclartingTypeSet();

        void HasCircularReferenceSet();

        void HasAttributesSet();

        void HasParametersSet();

        void HasSummarySet();

        void HasRemarksSet();

        void HasExamplesSet();

        void HasRelatedMembersSet();

        void HasExceptionsSet();

        void AcceptVisitor();
    }
}