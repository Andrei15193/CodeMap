using System;
using System.Linq;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestStructTests
{
    public class TestStructTestMethodParameterParamTests : DeclarationNodeTests<MethodDeclaration>
    {
        protected override bool DeclarationNodePredicate(MethodDeclaration methodDeclaration)
            => methodDeclaration.Name == nameof(TestStruct<int>.TestMethod) && methodDeclaration.DeclaringType.Name == nameof(TestStruct<int>);

        protected ParameterData Parameter
            => DeclarationNode.Parameters.ElementAt(0);

        [Fact]
        public void HasNameSet()
            => Assert.Equal("param", Parameter.Name);

        [Fact]
        public void HasAttributesSet()
            => Assert.Single(Parameter.Attributes);

        [Fact]
        public void HasTestAttribute()
            => AssertAttribute<TestAttribute>(
                Parameter.Attributes,
                new (string, object, Type)[] { ("value1", "struct method parameter test 1", typeof(object)) },
                new (string, object, Type)[] { ("Value2", "struct method parameter test 2", typeof(object)), ("Value3", "struct method parameter test 3", typeof(object)) }
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
            => Assert.True(typeof(int) == Parameter.Type);

        [Fact]
        public void HasEmptyDescription()
            => Assert.Empty(Parameter.Description);
    }
}