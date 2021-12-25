using System;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestRecordTests
{
    public class TestRecordRecordShadowedTestConstantDeclarationTests : DeclarationNodeTests<ConstantDeclaration>, IConstantDeclarationTests
    {
        protected override bool DeclarationNodePredicate(ConstantDeclaration constantDeclaration)
            => constantDeclaration.Name == nameof(TestRecord<int>.RecordShadowedTestConstant) && constantDeclaration.DeclaringType.Name == nameof(TestRecord<int>);

        [Fact]
        public void MemberEqualityComparison()
        {
            var fieldInfo = typeof(TestRecord<>).GetField("RecordShadowedTestConstant", BindingFlags.Public | BindingFlags.Static);
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
            => Assert.Equal("RecordShadowedTestConstant", DeclarationNode.Name);

        [Fact]
        public void HasDeclartingTypeSet()
            => Assert.True(typeof(TestRecord<>) == DeclarationNode.DeclaringType);

        [Fact]
        public void HasCircularReferenceSet()
            => Assert.Single(Assert.IsType<RecordDeclaration>(DeclarationNode.DeclaringType).Members, member => ReferenceEquals(member, DeclarationNode));

        [Fact]
        public void HasAttributesSet()
            => Assert.Empty(DeclarationNode.Attributes);

        [Fact]
        public void HasAccessModifierSet()
            => Assert.Equal(AccessModifier.Public, DeclarationNode.AccessModifier);

        [Fact]
        public void HasIsShadowingSet()
            => Assert.True(DeclarationNode.IsShadowing);

        [Fact]
        public void HasTypeSet()
            => Assert.True(typeof(float) == DeclarationNode.Type);

        [Fact]
        public void HasValueSet()
            => Assert.Equal(2f, DeclarationNode.Value);

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