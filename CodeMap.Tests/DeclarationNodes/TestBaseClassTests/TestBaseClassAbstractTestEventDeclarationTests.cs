using System;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestBaseClassTests
{
    public class TestBaseClassAbstractTestEventDeclarationTests : DeclarationNodeTests<EventDeclaration>
    {
        protected override bool DeclarationNodePredicate(EventDeclaration eventDeclaration)
            => eventDeclaration.Name == nameof(TestBaseClass.AbstractTestEvent) && eventDeclaration.DeclaringType.Name == nameof(TestBaseClass);

        [Fact]
        public void MemberEqualityComparison()
        {
            var eventInfo = typeof(TestBaseClass).GetRuntimeEvent(nameof(TestBaseClass.AbstractTestEvent));
            Assert.True(DeclarationNode.Equals(eventInfo));
            Assert.True(DeclarationNode.Equals(eventInfo as object));
            Assert.True(eventInfo == DeclarationNode);
            Assert.True(DeclarationNode == eventInfo);

            var toStringMember = typeof(object).GetRuntimeMethod(nameof(object.ToString), Type.EmptyTypes);
            Assert.False(DeclarationNode.Equals(toStringMember));
            Assert.False(DeclarationNode.Equals(toStringMember as object));
            Assert.True(toStringMember != DeclarationNode);
            Assert.True(DeclarationNode != toStringMember);
        }

        [Fact]
        public void HasNameSet()
            => Assert.Equal("AbstractTestEvent", DeclarationNode.Name);

        [Fact]
        public void HasAccessModifierSet()
            => Assert.Equal(AccessModifier.Public, DeclarationNode.AccessModifier);

        [Fact]
        public void HasDeclartingTypeSet()
            => Assert.True(typeof(TestBaseClass) == DeclarationNode.DeclaringType);

        [Fact]
        public void HasCircularReferenceSet()
            => Assert.Single(Assert.IsType<ClassDeclaration>(DeclarationNode.DeclaringType).Members, member => ReferenceEquals(member, DeclarationNode));

        [Fact]
        public void HasNoAttributes()
            => Assert.Empty(DeclarationNode.Attributes);

        [Fact]
        public void HasIsAbstractSet()
            => Assert.True(DeclarationNode.IsAbstract);

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
        public void HasNoAdderAttributes()
            => Assert.Empty(DeclarationNode.Adder.Attributes);

        [Fact]
        public void HasNoAdderReturnAttributes()
            => Assert.Empty(DeclarationNode.Adder.ReturnAttributes);

        [Fact]
        public void HasNoRemoverAttributes()
            => Assert.Empty(DeclarationNode.Remover.Attributes);

        [Fact]
        public void HasNoRemoverReturnAttributes()
            => Assert.Empty(DeclarationNode.Remover.ReturnAttributes);

        [Fact]
        public void HasTypeSet()
            => Assert.True(typeof(EventHandler) == DeclarationNode.Type);

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