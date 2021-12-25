namespace CodeMap.Tests.DeclarationNodes
{
    public interface IRecordDeclarationTests
    {
        void TypeEqualityComparison();

        void HasNameSet();

        void HasAssemblySet();

        void HasAccessModifierSet();

        void HasCircularReferenceSet();

        void HasAttributesSet();

        void HasGenericParametersSet();

        void HasBaseRecordSet();

        void HasImplementedInterfacesSet();

        void HasIsAbstractSet();

        void HasIsSealedSet();

        void HasDeclaringTypeSet();

        void HasMembersSet();

        void HasConstantsSet();

        void HasFieldsSet();

        void HasConstructorsSet();

        void HasEventsSet();

        void HasPropertiesSet();

        void HasMethodsSet();

        void HasNestedTypesSet();

        void HasNestedEnumsSet();

        void HasNestedDelegatesSet();

        void HasNestedInterfacesSet();

        void HasNestedClassesSet();

        void HasNestedStructsSet();

        void HasNestedRecordsSet();

        void HasSummarySet();

        void HasRemarksSet();

        void HasExamplesSet();

        void HasRelatedMembersSet();

        void AcceptVisitor();
    }
}