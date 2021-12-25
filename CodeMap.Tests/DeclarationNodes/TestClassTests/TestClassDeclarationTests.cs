using System;
using System.Linq;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestClassTests
{
    public class TestClassDeclarationTests : DeclarationNodeTests<ClassDeclaration>, IClassDeclarationTests
    {
        protected override bool DeclarationNodePredicate(ClassDeclaration classDeclaration)
            => classDeclaration.Name == nameof(TestClass<int>);

        [Fact]
        public void TypeEqualityComparison()
        {
            Assert.True(DeclarationNode.Equals(typeof(TestClass<>)));
            Assert.True(DeclarationNode.Equals(typeof(TestClass<>) as object));
            Assert.True(typeof(TestClass<>) == DeclarationNode);
            Assert.True(DeclarationNode == typeof(TestClass<>));

            var objectType = typeof(object);
            Assert.False(DeclarationNode.Equals(objectType));
            Assert.False(DeclarationNode.Equals(objectType as object));
            Assert.True(objectType != DeclarationNode);
            Assert.True(DeclarationNode != objectType);
        }

        [Fact]
        public void HasNameSet()
            => Assert.Equal("TestClass", DeclarationNode.Name);

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
        public void HasAttributesSet()
            => Assert.Equal(2, DeclarationNode.Attributes.Count);

        [Fact]
        public void HasTestAttribute()
            => AssertAttribute<TestAttribute>(
                DeclarationNode.Attributes,
                new (string, object, Type)[] { ("value1", "class test 1", typeof(object)) },
                new (string, object, Type)[] { ("Value2", "class test 2", typeof(object)), ("Value3", "class test 3", typeof(object)) }
            );

        [Fact]
        public void HasDefaultMemberAttribute()
            => AssertAttribute<DefaultMemberAttribute>(
                DeclarationNode.Attributes,
                new (string, object, Type)[] { ("memberName", "Item", typeof(string)) },
                Enumerable.Empty<(string, object, Type)>()
            );

        [Fact]
        public void HasGenericParametersSet()
            => Assert.Single(DeclarationNode.GenericParameters);

        [Fact]
        public void HasBaseClassSet()
            => Assert.True(typeof(TestBaseClass) == DeclarationNode.BaseClass);

        [Fact]
        public void HasImplementedInterfacesSet()
            => Assert.Single(DeclarationNode.ImplementedInterfaces);

        [Fact]
        public void HasITestExtendedBaseInterfaceImplementedInterface()
            => Assert.Single(DeclarationNode.ImplementedInterfaces, implementedInterface => implementedInterface == typeof(ITestExtendedBaseInterface));

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
        public void HasDeclaringTypeSet()
            => Assert.Null(DeclarationNode.DeclaringType);

        [Fact]
        public void HasMembersSet()
            => Assert.Equal(
                DeclarationNode.Constants.AsEnumerable<MemberDeclaration>().Concat(DeclarationNode.Fields).Concat(DeclarationNode.Constructors).Concat(DeclarationNode.Events).Concat(DeclarationNode.Properties).Concat(DeclarationNode.Methods),
                DeclarationNode.Members
            );

        [Fact]
        public void HasConstantsSet()
            => Assert.Equal(2, DeclarationNode.Constants.Count);

        [Fact]
        public void HasFieldsSet()
            => Assert.Equal(4, DeclarationNode.Fields.Count);

        [Fact]
        public void HasConstructorsSet()
            => Assert.Single(DeclarationNode.Constructors);

        [Fact]
        public void HasEventsSet()
            => Assert.Equal(6, DeclarationNode.Events.Count);

        [Fact]
        public void HasPropertiesSet()
            => Assert.Equal(10, DeclarationNode.Properties.Count);

        [Fact]
        public void HasMethodsSet()
            => Assert.Equal(47, DeclarationNode.Methods.Count);

        [Fact]
        public void HasNestedTypesSet()
            => Assert.Equal(
                DeclarationNode.NestedEnums.AsEnumerable<TypeDeclaration>().Concat(DeclarationNode.NestedDelegates).Concat(DeclarationNode.NestedInterfaces).Concat(DeclarationNode.NestedRecords).Concat(DeclarationNode.NestedClasses).Concat(DeclarationNode.NestedStructs),
                DeclarationNode.NestedTypes
            );

        [Fact]
        public void HasNestedEnumsSet()
            => Assert.Single(DeclarationNode.NestedEnums);

        [Fact]
        public void HasNestedDelegatesSet()
            => Assert.Single(DeclarationNode.NestedDelegates);

        [Fact]
        public void HasNestedInterfacesSet()
            => Assert.Single(DeclarationNode.NestedInterfaces);

        [Fact]
        public void HasNestedRecordsSet()
            => Assert.Single(DeclarationNode.NestedRecords);

        [Fact]
        public void HasNestedClassesSet()
            => Assert.Single(DeclarationNode.NestedClasses);

        [Fact]
        public void HasNestedStructsSet()
            => Assert.Single(DeclarationNode.NestedStructs);

        [Fact]
        public void HasSummarySet()
            => Assert.NotEmpty(DeclarationNode.Summary.Content);

        [Fact]
        public void HasRemarksSet()
            => Assert.NotEmpty(DeclarationNode.Remarks.Content);

        [Fact]
        public void HasExamplesSet()
            => Assert.NotEmpty(DeclarationNode.Examples);

        [Fact]
        public void HasRelatedMembersSet()
            => Assert.NotEmpty(DeclarationNode.RelatedMembers);

        [Fact]
        public void AcceptVisitor()
        {
            var visitor = new DeclarationNodeVisitorMock(DeclarationNode);

            DeclarationNode.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}