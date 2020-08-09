using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestStructTests
{
    public class TestStructDefaultConstructorDeclarationTests : DeclarationNodeTests<ConstructorDeclaration>
    {
        protected override bool DeclarationNodePredicate(ConstructorDeclaration constructorDeclaration)
            => constructorDeclaration.Name == nameof(TestStruct<int>) && constructorDeclaration.Parameters.Count == 0 && constructorDeclaration.DeclaringType.Name == nameof(TestStruct<int>);

        [Fact]
        public void HasNameSet()
            => Assert.Equal("TestStruct", DeclarationNode.Name);

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
        public void HasNoAttributes()
            => Assert.Empty(DeclarationNode.Attributes);

        [Fact]
        public void HasNoParameters()
            => Assert.Empty(DeclarationNode.Parameters);

        [Fact]
        public void HasEmptySummary()
            => Assert.Empty(DeclarationNode.Summary.Content);

        [Fact]
        public void HasEmptyRemarks()
            => Assert.Empty(DeclarationNode.Remarks.Content);

        [Fact]
        public void HasEmptyExamples()
            => Assert.Empty(DeclarationNode.Examples);

        [Fact]
        public void HasEmptyRelatedMembers()
            => Assert.Empty(DeclarationNode.RelatedMembers);

        [Fact]
        public void HasEmptyExceptions()
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