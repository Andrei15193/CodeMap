namespace CodeMap.Tests.DeclarationNodes
{
    public interface IInterfaceDeclarationTests
    {
        void TypeEqualityComparison();

        void HasNameSet();

        void HasAccessModifierSet();

        void HasAssemblySet();

        void HasCircularReferenceSet();

        void HasAttributesSet();

        void HasGenericParametersSet();

        void HasBaseInterfacesSet();

        void HasDeclaringTypeSet();

        void HasMembersSet();

        void HasEventsSet();

        void HasPropertiesSet();

        void HasMethodsSet();

        void HasSummarySet();

        void HasRemarksSet();

        void HasExamplesSet();

        void HasRelatedMembersSet();

        void AcceptVisitor();
    }
}