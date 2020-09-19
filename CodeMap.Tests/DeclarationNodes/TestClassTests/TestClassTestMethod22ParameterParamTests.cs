using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestClassTests
{
    public class TestClassTestMethod22ParameterParamTests : DeclarationNodeTests<MethodDeclaration>, IParameterDataTests
    {
        protected override bool DeclarationNodePredicate(MethodDeclaration methodDeclaration)
            => methodDeclaration.Name == nameof(TestClass<int>.TestMethod22) && methodDeclaration.DeclaringType.Name == nameof(TestClass<int>);

        protected ParameterData Parameter
            => Assert.Single(DeclarationNode.Parameters);

        [Fact]
        public void HasNameSet()
            => Assert.Equal("param", Parameter.Name);

        [Fact]
        public void HasAttributesSet()
            => Assert.Empty(Parameter.Attributes);

        [Fact]
        public void HasDefaultValueFlagSet()
            => Assert.False(Parameter.HasDefaultValue);

        [Fact]
        public void HasDefaultValueSet()
            => Assert.Null(Parameter.DefaultValue);

        [Fact]
        public void HasIsInputByReferenceSet()
            => Assert.False(Parameter.IsInputByReference);

        [Fact]
        public void HasIsInputOutputByReferenceSet()
            => Assert.False(Parameter.IsInputOutputByReference);

        [Fact]
        public void HasIsOutputByReferenceSet()
            => Assert.False(Parameter.IsOutputByReference);

        [Fact]
        public void HasTypeSet()
            => Assert.True(Assert.Single(typeof(TestClass<>).GetGenericArguments()) == Parameter.Type);

        [Fact]
        public void HasDescriptionSet()
            => Assert.NotEmpty(Parameter.Description);
    }
}