using System;
using System.Linq;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestAttributeTests
{
    public class TestAttributeDeclarationTests : DeclarationNodeTests<ClassDeclaration>
    {
        protected override bool DeclarationNodePredicate(ClassDeclaration classDeclaration)
            => classDeclaration.Name == nameof(TestAttribute);

        [Fact]
        public void TypeEqualityComparison()
        {
            Assert.True(DeclarationNode.Equals(typeof(TestAttribute)));
            Assert.True(DeclarationNode.Equals(typeof(TestAttribute) as object));
            Assert.True(typeof(TestAttribute) == DeclarationNode);
            Assert.True(DeclarationNode == typeof(TestAttribute));

            var objectType = typeof(object);
            Assert.False(DeclarationNode.Equals(objectType));
            Assert.False(DeclarationNode.Equals(objectType as object));
            Assert.True(objectType != DeclarationNode);
            Assert.True(DeclarationNode != objectType);
        }

        [Fact]
        public void HasNameSet()
            => Assert.Equal("TestAttribute", DeclarationNode.Name);

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
            => Assert.Single(DeclarationNode.Attributes);

        [Fact]
        public void HasAttributeUsageAttribute()
            => AssertAttribute<AttributeUsageAttribute>(
                DeclarationNode.Attributes,
                new (string, object, Type)[] { ("validOn", AttributeTargets.All, typeof(AttributeTargets)) },
                new (string, object, Type)[] { (nameof(AttributeUsageAttribute.AllowMultiple), true, typeof(bool)), (nameof(AttributeUsageAttribute.Inherited), true, typeof(bool)) }
            );

        [Fact]
        public void HasNoGenericParameters()
            => Assert.Empty(DeclarationNode.GenericParameters);

        [Fact]
        public void HasBaseClassSet()
            => Assert.True(typeof(Attribute) == DeclarationNode.BaseClass);

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
        public void HasNoDeclaringType()
            => Assert.Null(DeclarationNode.DeclaringType);

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
        public void HasFieldsSet()
            => Assert.Single(DeclarationNode.Fields);

        [Fact]
        public void HasConstructorsSet()
            => Assert.Single(DeclarationNode.Constructors);

        [Fact]
        public void HasNoEvents()
            => Assert.Empty(DeclarationNode.Events);

        [Fact]
        public void HasPropertiesSet()
            => Assert.Equal(2, DeclarationNode.Properties.Count);

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