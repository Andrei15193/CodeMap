using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using System;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes
{
    public class TestDelegateTests : DeclarationNodeTests<DelegateDeclaration>
    {
        protected override bool DeclarationNodePredicate(DelegateDeclaration delegateDeclaration)
            => delegateDeclaration.Name == nameof(TestDelegate<int>);

        [Fact]
        public void TypeEqualityComparison()
        {
            Assert.True(DeclarationNode.Equals(typeof(TestDelegate<>)));
            Assert.True(DeclarationNode.Equals(typeof(TestDelegate<>) as object));
            Assert.True(typeof(TestDelegate<>) == DeclarationNode);
            Assert.True(DeclarationNode == typeof(TestDelegate<>));

            var objectType = typeof(object);
            Assert.False(DeclarationNode.Equals(objectType));
            Assert.False(DeclarationNode.Equals(objectType as object));
            Assert.True(objectType != DeclarationNode);
            Assert.True(DeclarationNode != objectType);
        }

        [Fact]
        public void HasNameSet()
            => Assert.Equal("TestDelegate", DeclarationNode.Name);

        [Fact]
        public void HasAssemblySet()
            => Assert.Same(TestDataAssemblyDeclaration, DeclarationNode.Assembly);

        [Fact]
        public void HasAccessModifierSet()
            => Assert.Equal(AccessModifier.Public, DeclarationNode.AccessModifier);

        [Fact]
        public void HasCircularReferenceSet()
            => Assert.Single(DeclarationNode.Namespace.DeclaredTypes, type => ReferenceEquals(type, DeclarationNode));

        [Fact]
        public void HasAttributes()
            => Assert.Single(DeclarationNode.Attributes);

        [Fact]
        public void HasTestAttribute()
            => AssertAttribute<TestAttribute>(
                DeclarationNode.Attributes,
                new (string, object, Type)[] { ("value1", "delegate test 1", typeof(object)) },
                new (string, object, Type)[] { ("Value2", "delegate test 2", typeof(object)), ("Value3", "delegate test 3", typeof(object)) }
            );

        [Fact]
        public void HasGenericParametersSet()
            => Assert.Single(DeclarationNode.GenericParameters);

        [Fact]
        public void HasParametersSet()
            => Assert.Single(DeclarationNode.Parameters);

        [Fact]
        public void HasReturnAttributes()
            => Assert.Single(DeclarationNode.Return.Attributes);

        [Fact]
        public void HasTestReturnAttribute()
            => AssertAttribute<TestAttribute>(
                DeclarationNode.Return.Attributes,
                new (string, object, Type)[] { ("value1", "delegate return test 1", typeof(object)) },
                new (string, object, Type)[] { ("Value2", "delegate return test 2", typeof(object)), ("Value3", "delegate return test 3", typeof(object)) }
            );

        [Fact]
        public void HasReturnTypeSet()
            => Assert.True(typeof(void) == DeclarationNode.Return.Type);

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
        public void HasReturnDescription()
            => Assert.Empty(DeclarationNode.Return.Description);

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