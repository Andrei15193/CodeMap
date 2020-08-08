using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeMap.DeclarationNodes;
using CodeMap.ReferenceData;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestClassTests
{
    public class TestClassTestMethod21ParameterParamTests : DeclarationNodeTests<MethodDeclaration>
    {
        protected override bool DeclarationNodePredicate(MethodDeclaration methodDeclaration)
            => methodDeclaration.Name == nameof(TestClass<int>.TestMethod21) && methodDeclaration.DeclaringType.Name == nameof(TestClass<int>);

        protected ParameterData Parameter
            => Assert.Single(DeclarationNode.Parameters);

        [Fact]
        public void HasNameSet()
            => Assert.Equal("param", Parameter.Name);

        [Fact]
        public void HasAttributesSet()
            => Assert.Equal(2, Parameter.Attributes.Count);

        [Fact]
        public void HasOutAttribute()
            => AssertAttribute<OutAttribute>(
                Parameter.Attributes,
                Enumerable.Empty<(string, object, Type)>(),
                Enumerable.Empty<(string, object, Type)>()
            );

        [Fact]
        public void HasDynamicAttribute()
            => AssertAttribute<DynamicAttribute>(
                Parameter.Attributes,
                new (string, object, Type)[] { ("transformFlags", new[] { false, true }, typeof(bool[])) },
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
            => Assert.True(Parameter.IsOutputByReference);

        [Fact]
        public void HasTypeSet()
            => Assert.True(typeof(object) == Parameter.Type);

        [Fact]
        public void HasDynamicType()
            => Assert.IsType<DynamicTypeReference>(Parameter.Type);

        [Fact]
        public void HasEmptyDescription()
            => Assert.Empty(Parameter.Description);
    }
}