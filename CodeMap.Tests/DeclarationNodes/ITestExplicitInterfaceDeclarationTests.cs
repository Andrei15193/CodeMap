using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using System.Linq;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes
{
    public class ITestExplicitInterfaceDeclarationTests : DeclarationNodeTests<InterfaceDeclaration>
    {
        protected override bool DeclarationNodePredicate(InterfaceDeclaration interfaceDeclaration)
            => interfaceDeclaration.Name == nameof(ITestExplicitInterface);

        [Fact]
        public void TypeEqualityComparison()
        {
            Assert.True(DeclarationNode.Equals(typeof(ITestExplicitInterface)));
            Assert.True(DeclarationNode.Equals(typeof(ITestExplicitInterface) as object));
            Assert.True(typeof(ITestExplicitInterface) == DeclarationNode);
            Assert.True(DeclarationNode == typeof(ITestExplicitInterface));

            var objectType = typeof(object);
            Assert.False(DeclarationNode.Equals(objectType));
            Assert.False(DeclarationNode.Equals(objectType as object));
            Assert.True(objectType != DeclarationNode);
            Assert.True(DeclarationNode != objectType);
        }

        [Fact]
        public void HasNameSet()
            => Assert.Equal("ITestExplicitInterface", DeclarationNode.Name);

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
        public void HasNoAttributes()
            => Assert.Empty(DeclarationNode.Attributes);

        [Fact]
        public void HasNoGenericParametersSet()
            => Assert.Empty(DeclarationNode.GenericParameters);

        [Fact]
        public void HasMembersSet()
            => Assert.Equal(
                DeclarationNode.Events.AsEnumerable<MemberDeclaration>().Concat(DeclarationNode.Properties).Concat(DeclarationNode.Methods),
                DeclarationNode.Members
            );

        [Fact]
        public void HasEventsSet()
            => Assert.Single(DeclarationNode.Events);

        [Fact]
        public void HasPropertiesSet()
            => Assert.Single(DeclarationNode.Properties);

        [Fact]
        public void HasMethodsSet()
            => Assert.Single(DeclarationNode.Methods);

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
        public void AcceptVisitor()
        {
            var visitor = new DeclarationNodeVisitorMock(DeclarationNode);

            DeclarationNode.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}