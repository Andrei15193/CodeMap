using System;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestStructTests
{
    public class TestStructIndexerParameterParamTests : DeclarationNodeTests<PropertyDeclaration>
    {
        protected override bool DeclarationNodePredicate(PropertyDeclaration propertyDeclaration)
            => propertyDeclaration.Name == "Item" && propertyDeclaration.DeclaringType.Name == nameof(TestStruct<int>);

        protected ParameterData Parameter
            => Assert.Single(DeclarationNode.Parameters);

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
                new (string, object, Type)[] { ("value1", "struct indexer parameter test 1", typeof(object)) },
                new (string, object, Type)[] { ("Value2", "struct indexer parameter test 2", typeof(object)), ("Value3", "struct indexer parameter test 3", typeof(object)) }
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