using System;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestEnumTests
{
    public class TestEnumDeclarationTests : DeclarationNodeTests<EnumDeclaration>, IEnumDeclarationTests
    {
        protected override bool DeclarationNodePredicate(EnumDeclaration enumDeclaration)
            => enumDeclaration.Name == nameof(TestEnum);

        [Fact]
        public void TypeEqualityComparison()
        {
            Assert.True(DeclarationNode.Equals(typeof(TestEnum)));
            Assert.True(DeclarationNode.Equals(typeof(TestEnum) as object));
            Assert.True(typeof(TestEnum) == DeclarationNode);
            Assert.True(DeclarationNode == typeof(TestEnum));

            var objectType = typeof(object);
            Assert.False(DeclarationNode.Equals(objectType));
            Assert.False(DeclarationNode.Equals(objectType as object));
            Assert.True(objectType != DeclarationNode);
            Assert.True(DeclarationNode != objectType);
        }

        [Fact]
        public void HasNameSet()
            => Assert.Equal("TestEnum", DeclarationNode.Name);

        [Fact]
        public void HasAssemblySet()
            => Assert.Same(TestDataAssemblyDeclaration, DeclarationNode.Assembly);

        [Fact]
        public void HasAccessModifierSet()
            => Assert.Equal(AccessModifier.Public, DeclarationNode.AccessModifier);

        [Fact]
        public void HasUnderlyingTypeSet()
            => Assert.True(typeof(byte) == DeclarationNode.UnderlyingType);

        [Fact]
        public void HasCircularReferenceSet()
            => Assert.Single(DeclarationNode.Namespace.DeclaredTypes, type => ReferenceEquals(type, DeclarationNode));

        [Fact]
        public void HasAttributesSet()
            => Assert.Single(DeclarationNode.Attributes);

        [Fact]
        public void HasTestAttribute()
            => AssertAttribute<TestAttribute>(
                DeclarationNode.Attributes,
                new (string, object, Type)[] { ("value1", "enum test 1", typeof(object)) },
                new (string, object, Type)[] { ("Value2", "enum test 2", typeof(object)), ("Value3", "enum test 3", typeof(object)) }
            );

        [Fact]
        public void HasDeclaringTypeSet()
            => Assert.Null(DeclarationNode.DeclaringType);

        [Fact]
        public void HasMembersSet()
            => Assert.Equal(3, DeclarationNode.Members.Count);

        [Fact]
        public void HasSummarySet()
            => Assert.NotEmpty(DeclarationNode.Summary.Content);

        [Fact]
        public void HasRemarksSet()
            => Assert.NotEmpty(DeclarationNode.Remarks.Content);

        [Fact]
        public void HasExamplesSet()
            => Assert.NotEmpty(DeclarationNode.Examples);

        [Fact]
        public void HasRelatedMembersSet()
            => Assert.NotEmpty(DeclarationNode.RelatedMembers);

        [Fact]
        public void AcceptVisitor()
        {
            var visitor = new DeclarationNodeVisitorMock(DeclarationNode);

            DeclarationNode.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}