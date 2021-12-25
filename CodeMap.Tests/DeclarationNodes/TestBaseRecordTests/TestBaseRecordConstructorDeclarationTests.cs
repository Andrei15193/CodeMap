using System;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestBaseRecordTests
{
    public class TestBaseRecordConstructorDeclarationTests : DeclarationNodeTests<ConstructorDeclaration>, IConstructorDeclarationTests
    {
        protected override bool DeclarationNodePredicate(ConstructorDeclaration constructorDeclaration)
            => constructorDeclaration.Name == nameof(TestBaseRecord) && constructorDeclaration.DeclaringType.Name == nameof(TestBaseRecord);

        [Fact]
        public void MemberEqualityComparison()
        {
            var constructorInfo = typeof(TestBaseRecord).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, new[] { typeof(int) }, null);
            Assert.True(DeclarationNode.Equals(constructorInfo));
            Assert.True(DeclarationNode.Equals(constructorInfo as object));
            Assert.True(constructorInfo == DeclarationNode);
            Assert.True(DeclarationNode == constructorInfo);

            var toStringMember = typeof(object).GetRuntimeMethod(nameof(object.ToString), Type.EmptyTypes);
            Assert.False(DeclarationNode.Equals(toStringMember));
            Assert.False(DeclarationNode.Equals(toStringMember as object));
            Assert.True(toStringMember != DeclarationNode);
            Assert.True(DeclarationNode != toStringMember);
        }

        [Fact]
        public void HasNameSet()
            => Assert.Equal("TestBaseRecord", DeclarationNode.Name);

        [Fact]
        public void HasAccessModifierSet()
            => Assert.Equal(AccessModifier.Family, DeclarationNode.AccessModifier);

        [Fact]
        public void HasDeclartingTypeSet()
            => Assert.True(typeof(TestBaseRecord) == DeclarationNode.DeclaringType);

        [Fact]
        public void HasCircularReferenceSet()
            => Assert.Single(Assert.IsType<RecordDeclaration>(DeclarationNode.DeclaringType).Members, member => ReferenceEquals(member, DeclarationNode));

        [Fact]
        public void HasAttributesSet()
            => Assert.Empty(DeclarationNode.Attributes);

        [Fact]
        public void HasParametersSet()
            => Assert.Single(DeclarationNode.Parameters);

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
        public void HasExceptionsSet()
            => Assert.Empty(DeclarationNode.Exceptions);

        [Fact]
        public void AcceptVisitor()
        {
            var visitor = new DeclarationNodeVisitorMock(DeclarationNode);

            DeclarationNode.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}