using System;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestAbstractClassTests
{
    public class TestAbstractClassConstructorDeclarationTests : DeclarationNodeTests<ConstructorDeclaration>
    {
        protected override bool DeclarationNodePredicate(ConstructorDeclaration constructorDeclaration)
            => constructorDeclaration.Name == nameof(TestAbstractClass) && constructorDeclaration.DeclaringType.Name == nameof(TestAbstractClass);

        [Fact]
        public void MemberEqualityComparison()
        {
            var constructorInfo = typeof(TestAbstractClass).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, Type.EmptyTypes, null);
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
            => Assert.Equal("TestAbstractClass", DeclarationNode.Name);

        [Fact]
        public void HasAccessModifierSet()
            => Assert.Equal(AccessModifier.Family, DeclarationNode.AccessModifier);

        [Fact]
        public void HasDeclartingTypeSet()
            => Assert.True(typeof(TestAbstractClass) == DeclarationNode.DeclaringType);

        [Fact]
        public void HasCircularReferenceSet()
            => Assert.Single(Assert.IsType<ClassDeclaration>(DeclarationNode.DeclaringType).Members, member => ReferenceEquals(member, DeclarationNode));

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