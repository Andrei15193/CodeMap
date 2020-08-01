using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.ITestGenericParameter
{
    public class ITestGenericParameterDeclarationTests : DeclarationNodeTests<InterfaceDeclaration>
    {
        protected override bool DeclarationNodePredicate(InterfaceDeclaration interfaceDeclaration)
            => interfaceDeclaration.Name == nameof(ITestGenericParameter<int, string, int, int, int, int>);

        [Fact]
        public void TypeEqualityComparison()
        {
            Assert.True(DeclarationNode.Equals(typeof(ITestGenericParameter<,,,,,>)));
            Assert.True(DeclarationNode.Equals(typeof(ITestGenericParameter<,,,,,>) as object));
            Assert.True(typeof(ITestGenericParameter<,,,,,>) == DeclarationNode);
            Assert.True(DeclarationNode == typeof(ITestGenericParameter<,,,,,>));

            var constructedGenericType = typeof(ITestGenericParameter<int, string, int, int, int, int>);
            Assert.False(DeclarationNode.Equals(constructedGenericType));
            Assert.False(DeclarationNode.Equals(constructedGenericType as object));
            Assert.True(constructedGenericType != DeclarationNode);
            Assert.True(DeclarationNode != constructedGenericType);
        }

        [Fact]
        public void HasNameSet()
            => Assert.Equal("ITestGenericParameter", DeclarationNode.Name);

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
        public void HasGenericParametersSet()
            => Assert.Equal(6, DeclarationNode.GenericParameters.Count);

        [Fact]
        public void HasNoBaseInterfaces()
            => Assert.Empty(DeclarationNode.BaseInterfaces);

        [Fact]
        public void HasNoDeclaringType()
            => Assert.Null(DeclarationNode.DeclaringType);

        [Fact]
        public void HasNoMembers()
            => Assert.Empty(DeclarationNode.Members);

        [Fact]
        public void HasNoEvents()
            => Assert.Empty(DeclarationNode.Events);

        [Fact]
        public void HasNoProperties()
            => Assert.Empty(DeclarationNode.Properties);

        [Fact]
        public void HasNoMethods()
            => Assert.Empty(DeclarationNode.Methods);

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