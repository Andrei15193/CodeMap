using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestClassTests.NestedTestEnumTests
{
    public class TestClassNestedTestEnumDeclarationTests : DeclarationNodeTests<EnumDeclaration>, IEnumDeclarationTests
    {
        protected override bool DeclarationNodePredicate(EnumDeclaration enumDeclaration)
            => enumDeclaration.Name == nameof(TestClass<int>.NestedTestEnum) && enumDeclaration.DeclaringType.Name == nameof(TestClass<int>);

        [Fact]
        public void TypeEqualityComparison()
        {
            Assert.True(DeclarationNode.Equals(typeof(TestClass<>.NestedTestEnum)));
            Assert.True(DeclarationNode.Equals(typeof(TestClass<>.NestedTestEnum) as object));
            Assert.True(typeof(TestClass<>.NestedTestEnum) == DeclarationNode);
            Assert.True(DeclarationNode == typeof(TestClass<>.NestedTestEnum));

            var objectType = typeof(object);
            Assert.False(DeclarationNode.Equals(objectType));
            Assert.False(DeclarationNode.Equals(objectType as object));
            Assert.True(objectType != DeclarationNode);
            Assert.True(DeclarationNode != objectType);
        }

        [Fact]
        public void HasNameSet()
            => Assert.Equal("NestedTestEnum", DeclarationNode.Name);

        [Fact]
        public void HasAssemblySet()
            => Assert.Same(TestDataAssemblyDeclaration, DeclarationNode.Assembly);

        [Fact]
        public void HasAccessModifierSet()
            => Assert.Equal(AccessModifier.Public, DeclarationNode.AccessModifier);

        [Fact]
        public void HasUnderlyingTypeSet()
            => Assert.True(typeof(int) == DeclarationNode.UnderlyingType);

        [Fact]
        public void HasDeclaringTypeSet()
            => Assert.NotNull(DeclarationNode.DeclaringType);

        [Fact]
        public void HasCircularReferenceSet()
            => Assert.Single(Assert.IsType<ClassDeclaration>(DeclarationNode.DeclaringType).NestedTypes, type => ReferenceEquals(type, DeclarationNode));

        [Fact]
        public void HasAttributesSet()
            => Assert.Empty(DeclarationNode.Attributes);

        [Fact]
        public void HasMembersSet()
            => Assert.Empty(DeclarationNode.Members);

        [Fact]
        public void HasSummarySet()
            => Assert.NotEmpty(DeclarationNode.Summary.Content);

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
        public void AcceptVisitor()
        {
            var visitor = new DeclarationNodeVisitorMock(DeclarationNode);

            DeclarationNode.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}