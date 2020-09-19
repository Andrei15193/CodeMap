using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestStructTests.NestedTestEnumTests
{
    public class TestStructNestedTestEnumDeclarationTests : DeclarationNodeTests<EnumDeclaration>, IEnumDeclarationTests
    {
        protected override bool DeclarationNodePredicate(EnumDeclaration enumDeclaration)
            => enumDeclaration.Name == "NestedTestEnum" && enumDeclaration.DeclaringType.Name == nameof(TestStruct<int>);

        [Fact]
        public void TypeEqualityComparison()
        {
            Assert.True(DeclarationNode.Equals(typeof(TestStruct<>).GetNestedType("NestedTestEnum", BindingFlags.NonPublic)));
            Assert.True(DeclarationNode.Equals(typeof(TestStruct<>).GetNestedType("NestedTestEnum", BindingFlags.NonPublic) as object));
            Assert.True(typeof(TestStruct<>).GetNestedType("NestedTestEnum", BindingFlags.NonPublic) == DeclarationNode);
            Assert.True(DeclarationNode == typeof(TestStruct<>).GetNestedType("NestedTestEnum", BindingFlags.NonPublic));

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
            => Assert.Equal(AccessModifier.Private, DeclarationNode.AccessModifier);

        [Fact]
        public void HasUnderlyingTypeSet()
            => Assert.True(typeof(int) == DeclarationNode.UnderlyingType);

        [Fact]
        public void HasDeclaringTypeSet()
            => Assert.NotNull(DeclarationNode.DeclaringType);

        [Fact]
        public void HasCircularReferenceSet()
            => Assert.Single(Assert.IsType<StructDeclaration>(DeclarationNode.DeclaringType).NestedTypes, type => ReferenceEquals(type, DeclarationNode));

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