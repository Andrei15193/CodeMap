namespace CodeMap.Tests.DeclarationNodes
{
    public interface IParameterDataTests
    {
        void HasNameSet();

        void HasAttributesSet();

        void HasDefaultValueFlagSet();

        void HasDefaultValueSet();

        void HasIsInputByReferenceSet();

        void HasIsInputOutputByReferenceSet();

        void HasIsOutputByReferenceSet();

        void HasTypeSet();

        void HasDescriptionSet();
    }
}