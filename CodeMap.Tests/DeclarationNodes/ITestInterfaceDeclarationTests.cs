using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes
{
    public class ITestInterfaceDeclarationTests : DeclarationNodeTests<InterfaceDeclaration>
    {
        protected override bool DeclarationNodePredicate(InterfaceDeclaration interfaceDeclaration)
            => interfaceDeclaration.Name == nameof(ITestInterface<int>);

        [Fact]
        public void TypeEqualityComparison()
        {
            Assert.True(DeclarationNode.Equals(typeof(ITestInterface<>)));
            Assert.True(DeclarationNode.Equals(typeof(ITestInterface<>) as object));
            Assert.True(typeof(ITestInterface<>) == DeclarationNode);
            Assert.True(DeclarationNode == typeof(ITestInterface<>));

            var objectType = typeof(object);
            Assert.False(DeclarationNode.Equals(objectType));
            Assert.False(DeclarationNode.Equals(objectType as object));
            Assert.True(objectType != DeclarationNode);
            Assert.True(DeclarationNode != objectType);
        }

        [Fact]
        public void HasNameSet()
            => Assert.Equal("ITestInterface", DeclarationNode.Name);

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
                new (string, object, Type)[] { ("value1", "interface test 1", typeof(object)) },
                new (string, object, Type)[] { ("Value2", "interface test 2", typeof(object)), ("Value3", "interface test 3", typeof(object)) }
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
        public void HasBaseInterfacesSet()
            => Assert.Single(DeclarationNode.BaseInterfaces);

        [Fact]
        public void HasITestExtendedBaseInterfaceBaseInterfaces()
            => Assert.Single(DeclarationNode.BaseInterfaces, baseInterface => baseInterface == typeof(ITestExtendedBaseInterface));

        [Fact]
        public void HasMembersSet()
            => Assert.Equal(
                DeclarationNode.Events.AsEnumerable<MemberDeclaration>().Concat(DeclarationNode.Properties).Concat(DeclarationNode.Methods),
                DeclarationNode.Members
            );

        [Fact]
        public void HasEventsSet()
            => Assert.Equal(2, DeclarationNode.Events.Count);

        [Fact]
        public void HasPropertiesSet()
            => Assert.Equal(3, DeclarationNode.Properties.Count);

        [Fact]
        public void HasMethodsSet()
            => Assert.Equal(2, DeclarationNode.Methods.Count);

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