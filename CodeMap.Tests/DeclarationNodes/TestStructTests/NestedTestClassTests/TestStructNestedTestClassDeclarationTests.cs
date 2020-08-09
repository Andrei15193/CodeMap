using System.Linq;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestStructTests.NestedTestClassTests
{
    public class TestStructNestedTestClassDeclarationTests : DeclarationNodeTests<ClassDeclaration>
    {
        protected override bool DeclarationNodePredicate(ClassDeclaration classDeclaration)
            => classDeclaration.Name == "NestedTestClass" && classDeclaration.DeclaringType.Name == nameof(TestStruct<int>);

        [Fact]
        public void TypeEqualityComparison()
        {
            Assert.True(DeclarationNode.Equals(typeof(TestStruct<>).GetNestedType("NestedTestClass", BindingFlags.NonPublic)));
            Assert.True(DeclarationNode.Equals(typeof(TestStruct<>).GetNestedType("NestedTestClass", BindingFlags.NonPublic) as object));
            Assert.True(typeof(TestStruct<>).GetNestedType("NestedTestClass", BindingFlags.NonPublic) == DeclarationNode);
            Assert.True(DeclarationNode == typeof(TestStruct<>).GetNestedType("NestedTestClass", BindingFlags.NonPublic));

            var objectType = typeof(object);
            Assert.False(DeclarationNode.Equals(objectType));
            Assert.False(DeclarationNode.Equals(objectType as object));
            Assert.True(objectType != DeclarationNode);
            Assert.True(DeclarationNode != objectType);
        }

        [Fact]
        public void HasNameSet()
            => Assert.Equal("NestedTestClass", DeclarationNode.Name);

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
        public void HasNoAttributes()
            => Assert.Empty(DeclarationNode.Attributes);

        [Fact]
        public void HasNoGenericParameters()
            => Assert.Empty(DeclarationNode.GenericParameters);

        [Fact]
        public void HasBaseClassSet()
            => Assert.True(typeof(object) == DeclarationNode.BaseClass);

        [Fact]
        public void HasNoImplementedInterfaces()
            => Assert.Empty(DeclarationNode.ImplementedInterfaces);

        [Fact]
        public void HasIsAbstractSet()
            => Assert.False(DeclarationNode.IsAbstract);

        [Fact]
        public void HasIsSealedSet()
            => Assert.False(DeclarationNode.IsSealed);

        [Fact]
        public void HasIsStaticSet()
            => Assert.False(DeclarationNode.IsStatic);

        [Fact]
        public void HasMembersSet()
            => Assert.Equal(
                DeclarationNode.Constants.AsEnumerable<MemberDeclaration>().Concat(DeclarationNode.Fields).Concat(DeclarationNode.Constructors).Concat(DeclarationNode.Events).Concat(DeclarationNode.Properties).Concat(DeclarationNode.Methods),
                DeclarationNode.Members
            );

        [Fact]
        public void HasNoConstants()
            => Assert.Empty(DeclarationNode.Constants);

        [Fact]
        public void HasNoFields()
            => Assert.Empty(DeclarationNode.Fields);

        [Fact]
        public void HasConstructorsSet()
            => Assert.Single(DeclarationNode.Constructors);

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
        public void HasNestedTypesSet()
            => Assert.Equal(
                DeclarationNode.NestedEnums.AsEnumerable<TypeDeclaration>().Concat(DeclarationNode.NestedDelegates).Concat(DeclarationNode.NestedInterfaces).Concat(DeclarationNode.NestedClasses).Concat(DeclarationNode.NestedStructs),
                DeclarationNode.NestedTypes
            );

        [Fact]
        public void HasNoNestedEnums()
            => Assert.Empty(DeclarationNode.NestedEnums);

        [Fact]
        public void HasNoNestedDelegates()
            => Assert.Empty(DeclarationNode.NestedDelegates);

        [Fact]
        public void HasNoNestedInterfaces()
            => Assert.Empty(DeclarationNode.NestedInterfaces);

        [Fact]
        public void HasNoNestedClasses()
            => Assert.Empty(DeclarationNode.NestedClasses);

        [Fact]
        public void HasNoNestedStructs()
            => Assert.Empty(DeclarationNode.NestedStructs);

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