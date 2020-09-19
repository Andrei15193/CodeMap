using System;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestEnumTests
{
    public class TestEnumTestMember2DeclarationTests : DeclarationNodeTests<ConstantDeclaration>, IConstantDeclarationTests
    {
        protected override bool DeclarationNodePredicate(ConstantDeclaration constantDeclaration)
            => constantDeclaration.Name == nameof(TestEnum.TestMember2);

        [Fact]
        public void MemberEqualityComparison()
        {
            var fieldInfo = typeof(TestEnum).GetRuntimeField(nameof(TestEnum.TestMember2));
            Assert.True(DeclarationNode.Equals(fieldInfo));
            Assert.True(DeclarationNode.Equals(fieldInfo as object));
            Assert.True(fieldInfo == DeclarationNode);
            Assert.True(DeclarationNode == fieldInfo);

            var toStringMember = typeof(object).GetRuntimeMethod(nameof(object.ToString), Type.EmptyTypes);
            Assert.False(DeclarationNode.Equals(toStringMember));
            Assert.False(DeclarationNode.Equals(toStringMember as object));
            Assert.True(toStringMember != DeclarationNode);
            Assert.True(DeclarationNode != toStringMember);
        }

        [Fact]
        public void HasNameSet()
            => Assert.Equal("TestMember2", DeclarationNode.Name);

        [Fact]
        public void HasDeclartingTypeSet()
            => Assert.True(typeof(TestEnum) == DeclarationNode.DeclaringType);

        [Fact]
        public void HasCircularReferenceSet()
            => Assert.Single(Assert.IsType<EnumDeclaration>(DeclarationNode.DeclaringType).Members, member => ReferenceEquals(member, DeclarationNode));

        [Fact]
        public void HasAttributesSet()
            => Assert.Empty(DeclarationNode.Attributes);

        [Fact]
        public void HasAccessModifierSet()
            => Assert.Equal(AccessModifier.Public, DeclarationNode.AccessModifier);

        [Fact]
        public void HasIsShadowingSet()
            => Assert.False(DeclarationNode.IsShadowing);

        [Fact]
        public void HasTypeSet()
            => Assert.True(typeof(TestEnum) == DeclarationNode.Type);

        [Fact]
        public void HasValueSet()
            => Assert.Equal(TestEnum.TestMember2, DeclarationNode.Value);

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