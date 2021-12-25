using System.Linq;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestRecordTests.INestedTestInterfaceTests
{
    public class TestRecordINestedTestInterfaceDeclarationTests : DeclarationNodeTests<InterfaceDeclaration>, IInterfaceDeclarationTests
    {
        protected override bool DeclarationNodePredicate(InterfaceDeclaration interfaceDeclaration)
            => interfaceDeclaration.Name == nameof(TestRecord<int>.INestedTestInterface) && interfaceDeclaration.DeclaringType.Name == nameof(TestRecord<int>);

        [Fact]
        public void TypeEqualityComparison()
        {
            Assert.True(DeclarationNode.Equals(typeof(TestRecord<>.INestedTestInterface)));
            Assert.True(DeclarationNode.Equals(typeof(TestRecord<>.INestedTestInterface) as object));
            Assert.True(typeof(TestRecord<>.INestedTestInterface) == DeclarationNode);
            Assert.True(DeclarationNode == typeof(TestRecord<>.INestedTestInterface));

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
            => Assert.Single(Assert.IsType<RecordDeclaration>(DeclarationNode.DeclaringType).NestedTypes, type => ReferenceEquals(type, DeclarationNode));

        [Fact]
        public void HasAttributesSet()
            => Assert.Empty(DeclarationNode.Attributes);

        [Fact]
        public void HasGenericParametersSet()
            => Assert.Empty(DeclarationNode.GenericParameters);

        [Fact]
        public void HasBaseInterfacesSet()
            => Assert.Empty(DeclarationNode.BaseInterfaces);

        [Fact]
        public void HasMembersSet()
            => Assert.Equal(
                DeclarationNode.Events.AsEnumerable<MemberDeclaration>().Concat(DeclarationNode.Properties).Concat(DeclarationNode.Methods),
                DeclarationNode.Members
            );

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
            => Assert.Empty(DeclarationNode.Examples);

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