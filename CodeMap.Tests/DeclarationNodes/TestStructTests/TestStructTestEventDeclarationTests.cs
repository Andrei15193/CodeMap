using System;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestStructTests
{
    public class TestStructTestEventDeclarationTests : DeclarationNodeTests<EventDeclaration>
    {
        protected override bool DeclarationNodePredicate(EventDeclaration eventDeclaration)
            => eventDeclaration.Name == nameof(TestStruct<int>.TestEvent) && eventDeclaration.DeclaringType.Name == nameof(TestStruct<int>);

        [Fact]
        public void MemberEqualityComparison()
        {
            var eventInfo = typeof(TestStruct<>).GetRuntimeEvent(nameof(TestStruct<int>.TestEvent));
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
        public void HasAccessModifierSet()
            => Assert.Equal(AccessModifier.Public, DeclarationNode.AccessModifier);

        [Fact]
        public void HasDeclartingTypeSet()
            => Assert.True(typeof(TestStruct<>) == DeclarationNode.DeclaringType);

        [Fact]
        public void HasCircularReferenceSet()
            => Assert.Single(Assert.IsType<StructDeclaration>(DeclarationNode.DeclaringType).Members, member => ReferenceEquals(member, DeclarationNode));

        [Fact]
        public void HasAttributesSet()
            => Assert.Single(DeclarationNode.Attributes);

        [Fact]
        public void HasTestAttribute()
            => AssertAttribute<TestAttribute>(
                DeclarationNode.Attributes,
                new (string, object, Type)[] { ("value1", "struct event test 1", typeof(object)) },
                new (string, object, Type)[] { ("Value2", "struct event test 2", typeof(object)), ("Value3", "struct event test 3", typeof(object)) }
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
        public void HasAdderAttributesSet()
            => Assert.Single(DeclarationNode.Adder.Attributes);

        [Fact]
        public void HasAdderTestAttribute()
            => AssertAttribute<TestAttribute>(
                DeclarationNode.Adder.Attributes,
                new (string, object, Type)[] { ("value1", "struct event adder test 1", typeof(object)) },
                new (string, object, Type)[] { ("Value2", "struct event adder test 2", typeof(object)), ("Value3", "struct event adder test 3", typeof(object)) }
            );

        [Fact]
        public void HasAdderReturnAttributesSet()
            => Assert.Single(DeclarationNode.Adder.ReturnAttributes);

        [Fact]
        public void HasAdderReturnTestAttribute()
            => AssertAttribute<TestAttribute>(
                DeclarationNode.Adder.ReturnAttributes,
                new (string, object, Type)[] { ("value1", "struct event adder return test 1", typeof(object)) },
                new (string, object, Type)[] { ("Value2", "struct event adder return test 2", typeof(object)), ("Value3", "struct event adder return test 3", typeof(object)) }
            );

        [Fact]
        public void HasRemoverAttributesSet()
            => Assert.Single(DeclarationNode.Remover.Attributes);

        [Fact]
        public void HasRemoverTestAttribute()
            => AssertAttribute<TestAttribute>(
                DeclarationNode.Remover.Attributes,
                new (string, object, Type)[] { ("value1", "struct event remover test 1", typeof(object)) },
                new (string, object, Type)[] { ("Value2", "struct event remover test 2", typeof(object)), ("Value3", "struct event remover test 3", typeof(object)) }
            );

        [Fact]
        public void HasRemoverReturnAttributesSet()
            => Assert.Single(DeclarationNode.Remover.ReturnAttributes);

        [Fact]
        public void HasRemoverReturnTestAttribute()
            => AssertAttribute<TestAttribute>(
                DeclarationNode.Remover.ReturnAttributes,
                new (string, object, Type)[] { ("value1", "struct event remover return test 1", typeof(object)) },
                new (string, object, Type)[] { ("Value2", "struct event remover return test 2", typeof(object)), ("Value3", "struct event remover return test 3", typeof(object)) }
            );

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