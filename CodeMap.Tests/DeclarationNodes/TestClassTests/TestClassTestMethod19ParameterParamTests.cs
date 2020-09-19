using System;
using System.Linq;
using System.Runtime.CompilerServices;
using CodeMap.DeclarationNodes;
using CodeMap.ReferenceData;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestClassTests
{
    public class TestClassTestMethod19ParameterParamTests : DeclarationNodeTests<MethodDeclaration>, IParameterDataTests
    {
        protected override bool DeclarationNodePredicate(MethodDeclaration methodDeclaration)
            => methodDeclaration.Name == nameof(TestClass<int>.TestMethod19) && methodDeclaration.DeclaringType.Name == nameof(TestClass<int>);

        protected ParameterData Parameter
            => Assert.Single(DeclarationNode.Parameters);

        [Fact]
        public void HasNameSet()
            => Assert.Equal("param", Parameter.Name);

        [Fact]
        public void HasAttributesSet()
            => Assert.Single(Parameter.Attributes);

        [Fact]
        public void HasDynamicAttribute()
            => AssertAttribute<DynamicAttribute>(
                Parameter.Attributes,
                Enumerable.Empty<(string, object, Type)>(),
                Enumerable.Empty<(string, object, Type)>()
            );

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
            => Assert.True(typeof(object) == Parameter.Type);

        [Fact]
        public void HasDynamicType()
            => Assert.IsType<DynamicTypeReference>(Parameter.Type);

        [Fact]
        public void HasDescriptionSet()
            => Assert.NotEmpty(Parameter.Description);
    }
}