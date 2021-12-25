using System;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestRecordTests
{
    public class TestRecordReadonlyTestFieldDeclarationTests : DeclarationNodeTests<FieldDeclaration>, IFieldDeclarationTests
    {
        protected override bool DeclarationNodePredicate(FieldDeclaration fieldDeclaration)
            => fieldDeclaration.Name == "ReadonlyTestField" && fieldDeclaration.DeclaringType.Name == nameof(TestRecord<int>);

        [Fact]
        public void MemberEqualityComparison()
        {
            var fieldInfo = typeof(TestRecord<>).GetField("ReadonlyTestField", BindingFlags.NonPublic | BindingFlags.Instance);
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
            => Assert.Equal("ReadonlyTestField", DeclarationNode.Name);

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
            => Assert.Equal(AccessModifier.Private, DeclarationNode.AccessModifier);

        [Fact]
        public void HasIsShadowingSet()
            => Assert.False(DeclarationNode.IsShadowing);

        [Fact]
        public void HasIsReadOnlySet()
            => Assert.True(DeclarationNode.IsReadOnly);

        [Fact]
        public void HasIsStaticSet()
            => Assert.False(DeclarationNode.IsStatic);

        [Fact]
        public void HasTypeSet()
            => Assert.True(typeof(char) == DeclarationNode.Type);

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