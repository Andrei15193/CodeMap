using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestStructTests.NestedTestDelegateTests
{
    public class TestStructNestedTestDelegateDeclarationTests : DeclarationNodeTests<DelegateDeclaration>, IDelegateDeclarationTests
    {
        protected override bool DeclarationNodePredicate(DelegateDeclaration delegateDeclaration)
            => delegateDeclaration.Name == "NestedTestDelegate" && delegateDeclaration.DeclaringType.Name == nameof(TestStruct<int>);

        [Fact]
        public void TypeEqualityComparison()
        {
            Assert.True(DeclarationNode.Equals(typeof(TestStruct<>).GetNestedType("NestedTestDelegate", BindingFlags.NonPublic)));
            Assert.True(DeclarationNode.Equals(typeof(TestStruct<>).GetNestedType("NestedTestDelegate", BindingFlags.NonPublic) as object));
            Assert.True(typeof(TestStruct<>).GetNestedType("NestedTestDelegate", BindingFlags.NonPublic) == DeclarationNode);
            Assert.True(DeclarationNode == typeof(TestStruct<>).GetNestedType("NestedTestDelegate", BindingFlags.NonPublic));

            var objectType = typeof(object);
            Assert.False(DeclarationNode.Equals(objectType));
            Assert.False(DeclarationNode.Equals(objectType as object));
            Assert.True(objectType != DeclarationNode);
            Assert.True(DeclarationNode != objectType);
        }

        [Fact]
        public void HasNameSet()
            => Assert.Equal("NestedTestDelegate", DeclarationNode.Name);

        [Fact]
        public void HasAssemblySet()
            => Assert.Same(TestDataAssemblyDeclaration, DeclarationNode.Assembly);

        [Fact]
        public void HasAccessModifierSet()
            => Assert.Equal(AccessModifier.Private, DeclarationNode.AccessModifier);

        [Fact]
        public void HasDeclaringTypeSet()
            => Assert.NotNull(DeclarationNode.DeclaringType);

        [Fact]
        public void HasCircularReferenceSet()
            => Assert.Single(Assert.IsType<StructDeclaration>(DeclarationNode.DeclaringType).NestedTypes, type => ReferenceEquals(type, DeclarationNode));

        [Fact]
        public void HasAttributesSet()
            => Assert.Empty(DeclarationNode.Attributes);

        [Fact]
        public void HasGenericParametersSet()
            => Assert.Empty(DeclarationNode.GenericParameters);

        [Fact]
        public void HasParametersSet()
            => Assert.Empty(DeclarationNode.Parameters);

        [Fact]
        public void HasReturnAttributes()
            => Assert.Empty(DeclarationNode.Return.Attributes);

        [Fact]
        public void HasReturnTypeSet()
            => Assert.True(typeof(void) == DeclarationNode.Return.Type);

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
        public void HasReturnDescriptionSet()
            => Assert.Empty(DeclarationNode.Return.Description);

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