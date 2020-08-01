using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using System;
using System.Reflection;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes
{
    public class ITestInterfaceTestEventDeclarationTests : DeclarationNodeTests<EventDeclaration>
    {
        protected override bool DeclarationNodePredicate(EventDeclaration eventDeclaration)
            => eventDeclaration.Name == nameof(ITestInterface<int>.TestEvent) && eventDeclaration.DeclaringType.Name == nameof(ITestInterface<int>);

        [Fact]
        public void MemberEqualityComparison()
        {
            var eventInfo = typeof(ITestInterface<>).GetRuntimeEvent(nameof(ITestInterface<int>.TestEvent));
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
            => Assert.Equal("TestEvent", DeclarationNode.Name);

        [Fact]
        public void HasDeclartingTypeSet()
            => Assert.True(typeof(ITestInterface<>) == DeclarationNode.DeclaringType);

        [Fact]
        public void HasCircularReferenceSet()
            => Assert.Single(Assert.IsType<InterfaceDeclaration>(DeclarationNode.DeclaringType).Members, member => ReferenceEquals(member, DeclarationNode));

        [Fact]
        public void HasAttributesSet()
            => Assert.Single(DeclarationNode.Attributes);

        [Fact]
        public void HasTestAttribute()
            => AssertAttribute<TestAttribute>(
                DeclarationNode.Attributes,
                new (string, object, Type)[] { ("value1", "interface event test 1", typeof(object)) },
                new (string, object, Type)[] { ("Value2", "interface event test 2", typeof(object)), ("Value3", "interface event test 3", typeof(object)) }
            );

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
            => Assert.True(typeof(EventHandler<EventArgs>) == DeclarationNode.Type);

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