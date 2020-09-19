using System.Linq;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestStructTests.NestedTestStructTests
{
    public class TestStructNestedTestStructDeclarationTests : DeclarationNodeTests<StructDeclaration>, IStructDefitionTests
    {
        protected override bool DeclarationNodePredicate(StructDeclaration structDeclaration)
            => structDeclaration.Name == "NestedTestStruct" && structDeclaration.DeclaringType.Name == nameof(TestStruct<int>);

        [Fact]
        public void TypeEqualityComparison()
        {
            Assert.True(DeclarationNode.Equals(typeof(TestStruct<>).GetNestedType("NestedTestStruct", BindingFlags.NonPublic)));
            Assert.True(DeclarationNode.Equals(typeof(TestStruct<>).GetNestedType("NestedTestStruct", BindingFlags.NonPublic) as object));
            Assert.True(typeof(TestStruct<>).GetNestedType("NestedTestStruct", BindingFlags.NonPublic) == DeclarationNode);
            Assert.True(DeclarationNode == typeof(TestStruct<>).GetNestedType("NestedTestStruct", BindingFlags.NonPublic));

            var objectType = typeof(object);
            Assert.False(DeclarationNode.Equals(objectType));
            Assert.False(DeclarationNode.Equals(objectType as object));
            Assert.True(objectType != DeclarationNode);
            Assert.True(DeclarationNode != objectType);
        }

        [Fact]
        public void HasNameSet()
            => Assert.Equal("NestedTestStruct", DeclarationNode.Name);

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
        public void HasImplementedInterfacesSet()
            => Assert.Empty(DeclarationNode.ImplementedInterfaces);

        [Fact]
        public void HasMembersSet()
            => Assert.Equal(
                DeclarationNode.Constants.AsEnumerable<MemberDeclaration>().Concat(DeclarationNode.Fields).Concat(DeclarationNode.Constructors).Concat(DeclarationNode.Events).Concat(DeclarationNode.Properties).Concat(DeclarationNode.Methods),
                DeclarationNode.Members
            );

        [Fact]
        public void HasConstantsSet()
            => Assert.Empty(DeclarationNode.Constants);

        [Fact]
        public void HasFieldsSet()
            => Assert.Empty(DeclarationNode.Fields);

        [Fact]
        public void HasConstructorsSet()
            => Assert.Single(DeclarationNode.Constructors);

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
        public void HasNestedTypesSet()
            => Assert.Equal(
                DeclarationNode.NestedEnums.AsEnumerable<TypeDeclaration>().Concat(DeclarationNode.NestedDelegates).Concat(DeclarationNode.NestedInterfaces).Concat(DeclarationNode.NestedClasses).Concat(DeclarationNode.NestedStructs),
                DeclarationNode.NestedTypes
            );

        [Fact]
        public void HasNestedEnumsSet()
            => Assert.Empty(DeclarationNode.NestedEnums);

        [Fact]
        public void HasNestedDelegatesSet()
            => Assert.Empty(DeclarationNode.NestedDelegates);

        [Fact]
        public void HasNestedInterfacesSet()
            => Assert.Empty(DeclarationNode.NestedInterfaces);

        [Fact]
        public void HasNestedClassesSet()
            => Assert.Empty(DeclarationNode.NestedClasses);

        [Fact]
        public void HasNestedStructsSet()
            => Assert.Empty(DeclarationNode.NestedStructs);

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