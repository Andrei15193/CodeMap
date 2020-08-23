using System;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestStructTests
{
    public class TestStructIndexerDeclarationTests : DeclarationNodeTests<PropertyDeclaration>, IPropertyDeclarationTests
    {
        protected override bool DeclarationNodePredicate(PropertyDeclaration propertyDeclaration)
            => propertyDeclaration.Name == "Item" && propertyDeclaration.DeclaringType.Name == nameof(TestStruct<int>);

        [Fact]
        public void MemberEqualityComparison()
        {
            var propertyInfo = typeof(TestStruct<>).GetRuntimeProperty("Item");
            Assert.True(DeclarationNode.Equals(propertyInfo));
            Assert.True(DeclarationNode.Equals(propertyInfo as object));
            Assert.True(propertyInfo == DeclarationNode);
            Assert.True(DeclarationNode == propertyInfo);

            var toStringMember = typeof(object).GetRuntimeMethod(nameof(object.ToString), Type.EmptyTypes);
            Assert.False(DeclarationNode.Equals(toStringMember));
            Assert.False(DeclarationNode.Equals(toStringMember as object));
            Assert.True(toStringMember != DeclarationNode);
            Assert.True(DeclarationNode != toStringMember);
        }

        [Fact]
        public void HasNameSet()
            => Assert.Equal("Item", DeclarationNode.Name);

        [Fact]
        public void HasAccessModifierSet()
            => Assert.Equal(AccessModifier.Public, DeclarationNode.AccessModifier);

        [Fact]
        public void HasDeclartingTypeSet()
            => Assert.True(typeof(TestStruct<>) == DeclarationNode.DeclaringType);

        [Fact]
        public void HasCircularReferenceSet()
            => Assert.Single(Assert.IsType<StructDeclaration>(DeclarationNode.DeclaringType).Members, member => ReferenceEquals(member, DeclarationNode));

        [Fact]
        public void HasAttributesSet()
            => Assert.Single(DeclarationNode.Attributes);

        [Fact]
        public void HasTestAttribute()
            => AssertAttribute<TestAttribute>(
                DeclarationNode.Attributes,
                new (string, object, Type)[] { ("value1", "struct indexer test 1", typeof(object)) },
                new (string, object, Type)[] { ("Value2", "struct indexer test 2", typeof(object)), ("Value3", "struct indexer test 3", typeof(object)) }
            );

        [Fact]
        public void HasIsAbstractSet()
            => Assert.False(DeclarationNode.IsAbstract);

        [Fact]
        public void HasIsOverrideSet()
            => Assert.False(DeclarationNode.IsOverride);

        [Fact]
        public void HasIsSealedSet()
            => Assert.False(DeclarationNode.IsSealed);

        [Fact]
        public void HasIsShadowingSet()
            => Assert.False(DeclarationNode.IsShadowing);

        [Fact]
        public void HasIsStaticSet()
            => Assert.False(DeclarationNode.IsStatic);

        [Fact]
        public void HasIsVirtualSet()
            => Assert.False(DeclarationNode.IsVirtual);

        [Fact]
        public void HasParametersSet()
            => Assert.Single(DeclarationNode.Parameters);

        [Fact]
        public void HasGetterAccessModifierSet()
            => Assert.Equal(AccessModifier.Public, DeclarationNode.Getter.AccessModifier);

        [Fact]
        public void HasGetterAttributesSet()
            => Assert.Single(DeclarationNode.Getter.Attributes);

        [Fact]
        public void HasGetterTestAttribute()
            => AssertAttribute<TestAttribute>(
                DeclarationNode.Getter.Attributes,
                new (string, object, Type)[] { ("value1", "struct indexer getter test 1", typeof(object)) },
                new (string, object, Type)[] { ("Value2", "struct indexer getter test 2", typeof(object)), ("Value3", "struct indexer getter test 3", typeof(object)) }
            );

        [Fact]
        public void HasGetterReturnAttributesSet()
            => Assert.Single(DeclarationNode.Getter.ReturnAttributes);

        [Fact]
        public void HasGetterReturnTestAttribute()
            => AssertAttribute<TestAttribute>(
                DeclarationNode.Getter.ReturnAttributes,
                new (string, object, Type)[] { ("value1", "struct indexer getter return test 1", typeof(object)) },
                new (string, object, Type)[] { ("Value2", "struct indexer getter return test 2", typeof(object)), ("Value3", "struct indexer getter return test 3", typeof(object)) }
            );

        [Fact]
        public void HasSetterAccessModifierSet()
            => Assert.Equal(AccessModifier.Public, DeclarationNode.Setter.AccessModifier);

        [Fact]
        public void HasSetterAttributesSet()
            => Assert.Single(DeclarationNode.Setter.Attributes);

        [Fact]
        public void HasSetterTestAttribute()
            => AssertAttribute<TestAttribute>(
                DeclarationNode.Setter.Attributes,
                new (string, object, Type)[] { ("value1", "struct indexer setter test 1", typeof(object)) },
                new (string, object, Type)[] { ("Value2", "struct indexer setter test 2", typeof(object)), ("Value3", "struct indexer setter test 3", typeof(object)) }
            );

        [Fact]
        public void HasSetterReturnAttributesSet()
            => Assert.Single(DeclarationNode.Setter.ReturnAttributes);

        [Fact]
        public void HasSetterReturnTestAttribute()
            => AssertAttribute<TestAttribute>(
                DeclarationNode.Setter.ReturnAttributes,
                new (string, object, Type)[] { ("value1", "struct indexer setter return test 1", typeof(object)) },
                new (string, object, Type)[] { ("Value2", "struct indexer setter return test 2", typeof(object)), ("Value3", "struct indexer setter return test 3", typeof(object)) }
            );

        [Fact]
        public void HasTypeSet()
            => Assert.True(typeof(int) == DeclarationNode.Type);

        [Fact]
        public void HasSummarySet()
            => Assert.Empty(DeclarationNode.Summary.Content);

        [Fact]
        public void HasRemarksSet()
            => Assert.Empty(DeclarationNode.Remarks.Content);

        [Fact]
        public void HasExamplesSet()
            => Assert.Empty(DeclarationNode.Examples);

        [Fact]
        public void HasRelatedMembersSet()
            => Assert.Empty(DeclarationNode.RelatedMembers);

        [Fact]
        public void HasExceptionsSet()
            => Assert.Empty(DeclarationNode.Exceptions);

        [Fact]
        public void HasValueSet()
            => Assert.Empty(DeclarationNode.Value.Content);

        [Fact]
        public void AcceptVisitor()
        {
            var visitor = new DeclarationNodeVisitorMock(DeclarationNode);

            DeclarationNode.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}