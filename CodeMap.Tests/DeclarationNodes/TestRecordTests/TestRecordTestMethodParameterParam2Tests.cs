using System.Linq;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestRecordTests
{
    public class TestRecordTestMethodParameterParam2Tests : DeclarationNodeTests<MethodDeclaration>, IParameterDataTests
    {
        protected override bool DeclarationNodePredicate(MethodDeclaration methodDeclaration)
            => methodDeclaration.Name == nameof(TestRecord<int>.TestMethod) && methodDeclaration.DeclaringType.Name == nameof(TestRecord<int>);

        protected ParameterData Parameter
            => DeclarationNode.Parameters.ElementAt(1);

        [Fact]
        public void HasNameSet()
            => Assert.Equal("param2", Parameter.Name);

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
            => Assert.True(typeof(string) == Parameter.Type);

        [Fact]
        public void HasDescriptionSet()
            => Assert.NotEmpty(Parameter.Description);
    }
}