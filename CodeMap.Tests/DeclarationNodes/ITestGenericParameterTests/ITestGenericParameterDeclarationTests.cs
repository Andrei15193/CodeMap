using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.ITestGenericParameter
{
    public class ITestGenericParameterDeclarationTests : DeclarationNodeTests<InterfaceDeclaration>, IInterfaceDeclarationTests
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
        public void HasAccessModifierSet()
            => Assert.Equal(AccessModifier.Public, DeclarationNode.AccessModifier);

        [Fact]
        public void HasAssemblySet()
            => Assert.Same(TestDataAssemblyDeclaration, DeclarationNode.Assembly);

        [Fact]
        public void HasCircularReferenceSet()
            => Assert.Single(DeclarationNode.Namespace.DeclaredTypes, type => ReferenceEquals(type, DeclarationNode));

        [Fact]
        public void HasAttributesSet()
            => Assert.Empty(DeclarationNode.Attributes);

        [Fact]
        public void HasGenericParametersSet()
            => Assert.Equal(6, DeclarationNode.GenericParameters.Count);

        [Fact]
        public void HasBaseInterfacesSet()
            => Assert.Empty(DeclarationNode.BaseInterfaces);

        [Fact]
        public void HasDeclaringTypeSet()
            => Assert.Null(DeclarationNode.DeclaringType);

        [Fact]
        public void HasMembersSet()
            => Assert.Empty(DeclarationNode.Members);

        [Fact]
        public void HasEventsSet()
            => Assert.Empty(DeclarationNode.Events);

        [Fact]
        public void HasPropertiesSet()
            => Assert.Empty(DeclarationNode.Properties);

        [Fact]
        public void HasMethodsSet()
            => Assert.Empty(DeclarationNode.Methods);

        [Fact]
        public void HasSummarySet()
            => Assert.NotEmpty(DeclarationNode.Summary.Content);

        [Fact]
        public void HasRemarksSet()
            => Assert.Empty(DeclarationNode.Remarks.Content);

        [Fact]
        public void HasExamplesSet()
            => Assert.NotEmpty(DeclarationNode.Examples);

        [Fact]
        public void HasRelatedMembersSet()
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