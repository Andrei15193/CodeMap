namespace CodeMap.Tests.DeclarationNodes
{
    public interface IPropertyDeclarationTests
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

        void HasParametersSet();

        void HasGetterAccessModifierSet();

        void HasGetterAttributesSet();

        void HasGetterReturnAttributesSet();

        void HasSetterAccessModifierSet();

        void HasSetterAttributesSet();

        void HasSetterReturnAttributesSet();

        void HasTypeSet();

        void HasSummarySet();

        void HasRemarksSet();

        void HasExamplesSet();

        void HasRelatedMembersSet();

        void HasExceptionsSet();

        void HasValueSet();

        void AcceptVisitor();
    }
}