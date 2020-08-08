using System.Linq;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestClassTests.INestedTestInterfaceTests
{
    public class ITestBaseInterfaceDeclarationTests : DeclarationNodeTests<InterfaceDeclaration>
    {
        protected override bool DeclarationNodePredicate(InterfaceDeclaration interfaceDeclaration)
            => interfaceDeclaration.Name == nameof(TestClass<int>.INestedTestInterface) && interfaceDeclaration.DeclaringType.Name == nameof(TestClass<int>);

        [Fact]
        public void TypeEqualityComparison()
        {
            Assert.True(DeclarationNode.Equals(typeof(TestClass<>.INestedTestInterface)));
            Assert.True(DeclarationNode.Equals(typeof(TestClass<>.INestedTestInterface) as object));
            Assert.True(typeof(TestClass<>.INestedTestInterface) == DeclarationNode);
            Assert.True(DeclarationNode == typeof(TestClass<>.INestedTestInterface));

            var objectType = typeof(object);
            Assert.False(DeclarationNode.Equals(objectType));
            Assert.False(DeclarationNode.Equals(objectType as object));
            Assert.True(objectType != DeclarationNode);
            Assert.True(DeclarationNode != objectType);
        }

        [Fact]
        public void HasNameSet()
            => Assert.Equal("INestedTestInterface", DeclarationNode.Name);

        [Fact]
        public void HasAccessModifierSet()
            => Assert.Equal(AccessModifier.Public, DeclarationNode.AccessModifier);

        [Fact]
        public void HasAssemblySet()
            => Assert.Same(TestDataAssemblyDeclaration, DeclarationNode.Assembly);

        [Fact]
        public void HasDeclaringTypeSet()
            => Assert.NotNull(DeclarationNode.DeclaringType);

        [Fact]
        public void HasCircularReferenceSet()
            => Assert.Single(Assert.IsType<ClassDeclaration>(DeclarationNode.DeclaringType).NestedTypes, type => ReferenceEquals(type, DeclarationNode));

        [Fact]
        public void HasNoAttributes()
            => Assert.Empty(DeclarationNode.Attributes);

        [Fact]
        public void HasNoGenericParameters()
            => Assert.Empty(DeclarationNode.GenericParameters);

        [Fact]
        public void HasNoBaseInterfaces()
            => Assert.Empty(DeclarationNode.BaseInterfaces);

        [Fact]
        public void HasMembersSet()
            => Assert.Equal(
                DeclarationNode.Events.AsEnumerable<MemberDeclaration>().Concat(DeclarationNode.Properties).Concat(DeclarationNode.Methods),
                DeclarationNode.Members
            );

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