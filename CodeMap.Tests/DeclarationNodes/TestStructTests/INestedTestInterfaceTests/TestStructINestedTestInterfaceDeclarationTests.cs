using System.Linq;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestStructTests.INestedTestInterfaceTests
{
    public class TestStructINestedTestInterfaceDeclarationTests : DeclarationNodeTests<InterfaceDeclaration>
    {
        protected override bool DeclarationNodePredicate(InterfaceDeclaration interfaceDeclaration)
            => interfaceDeclaration.Name == "INestedTestInterface" && interfaceDeclaration.DeclaringType.Name == nameof(TestStruct<int>);

        [Fact]
        public void TypeEqualityComparison()
        {
            Assert.True(DeclarationNode.Equals(typeof(TestStruct<>).GetNestedType("INestedTestInterface", BindingFlags.NonPublic)));
            Assert.True(DeclarationNode.Equals(typeof(TestStruct<>).GetNestedType("INestedTestInterface", BindingFlags.NonPublic) as object));
            Assert.True(typeof(TestStruct<>).GetNestedType("INestedTestInterface", BindingFlags.NonPublic) == DeclarationNode);
            Assert.True(DeclarationNode == typeof(TestStruct<>).GetNestedType("INestedTestInterface", BindingFlags.NonPublic));

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
            => Assert.Equal(AccessModifier.Private, DeclarationNode.AccessModifier);

        [Fact]
        public void HasAssemblySet()
            => Assert.Same(TestDataAssemblyDeclaration, DeclarationNode.Assembly);

        [Fact]
        public void HasDeclaringTypeSet()
            => Assert.NotNull(DeclarationNode.DeclaringType);

        [Fact]
        public void HasCircularReferenceSet()
            => Assert.Single(Assert.IsType<StructDeclaration>(DeclarationNode.DeclaringType).NestedTypes, type => ReferenceEquals(type, DeclarationNode));

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