namespace CodeMap.Tests.DeclarationNodes
{
    public interface IGenericParameterDataTests
    {
        void HasNameSet();

        void HasPositionSet();

        void HasIsCovariantSet();

        void HasIsContravariantSet();

        void HasHasDefaultConstructorConstraintSet();

        void HasHasNonNullableValueTypeConstraintSet();

        void HasHasReferenceTypeConstraintSet();

        void HasHasUnmanagedTypeConstraintSet();

        void HasTypeConstraintsSet();

        void HasDescriptionSet();
    }
}