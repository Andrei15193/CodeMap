using System;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestClassTests
{
    public class TestClassStaticTestEventDeclarationTests : DeclarationNodeTests<EventDeclaration>, IEventDeclarationTests
    {
        protected override bool DeclarationNodePredicate(EventDeclaration eventDeclaration)
            => eventDeclaration.Name == nameof(TestClass<int>.StaticTestEvent) && eventDeclaration.DeclaringType.Name == nameof(TestClass<int>);

        [Fact]
        public void MemberEqualityComparison()
        {
            var eventInfo = typeof(TestClass<>).GetRuntimeEvent(nameof(TestClass<int>.StaticTestEvent));
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
            => Assert.Equal("StaticTestEvent", DeclarationNode.Name);

        [Fact]
        public void HasAccessModifierSet()
            => Assert.Equal(AccessModifier.Public, DeclarationNode.AccessModifier);

        [Fact]
        public void HasDeclartingTypeSet()
            => Assert.True(typeof(TestClass<>) == DeclarationNode.DeclaringType);

        [Fact]
        public void HasCircularReferenceSet()
            => Assert.Single(Assert.IsType<ClassDeclaration>(DeclarationNode.DeclaringType).Members, member => ReferenceEquals(member, DeclarationNode));

        [Fact]
        public void HasAttributesSet()
            => Assert.Empty(DeclarationNode.Attributes);

        [Fact]
        public void HasIsAbstractSet()
            => Assert.False(DeclarationNode.IsAbstract);

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
            => Assert.True(DeclarationNode.IsStatic);

        [Fact]
        public void HasIsVirtualSet()
            => Assert.False(DeclarationNode.IsVirtual);

        [Fact]
        public void HasAdderAttributesSet()
            => Assert.Empty(DeclarationNode.Adder.Attributes);

        [Fact]
        public void HasAdderReturnAttributesSet()
            => Assert.Empty(DeclarationNode.Adder.ReturnAttributes);

        [Fact]
        public void HasRemoverAttributesSet()
            => Assert.Empty(DeclarationNode.Remover.Attributes);

        [Fact]
        public void HasRemoverReturnAttributesSet()
            => Assert.Empty(DeclarationNode.Remover.ReturnAttributes);

        [Fact]
        public void HasTypeSet()
            => Assert.True(typeof(EventHandler) == DeclarationNode.Type);

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