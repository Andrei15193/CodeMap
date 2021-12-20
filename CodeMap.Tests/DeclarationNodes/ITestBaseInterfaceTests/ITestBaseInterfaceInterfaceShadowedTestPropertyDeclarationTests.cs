using System;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.ITestBaseInterfaceTests
{
    public class ITestBaseInterfaceInterfaceShadowedTestPropertyDeclarationTests : DeclarationNodeTests<PropertyDeclaration>, IPropertyDeclarationTests
    {
        protected override bool DeclarationNodePredicate(PropertyDeclaration propertyDeclaration)
            => propertyDeclaration.Name == nameof(ITestBaseInterface.InterfaceShadowedTestProperty) && propertyDeclaration.DeclaringType.Name == nameof(ITestBaseInterface);

        [Fact]
        public void MemberEqualityComparison()
        {
            var propertyInfo = typeof(ITestBaseInterface).GetRuntimeProperty(nameof(ITestBaseInterface.InterfaceShadowedTestProperty));
            Assert.True(DeclarationNode.Equals(propertyInfo));
            Assert.True(DeclarationNode.Equals(propertyInfo as object));
            Assert.True(propertyInfo == DeclarationNode);
            Assert.True(DeclarationNode == propertyInfo);

            var toStringMember = typeof(object).GetRuntimeMethod(nameof(object.ToString), Type.EmptyTypes);
            Assert.False(DeclarationNode.Equals(toStringMember));
            Assert.False(DeclarationNode.Equals(toStringMember as object));
            Assert.True(toStringMember != DeclarationNode);
            Assert.True(DeclarationNode != toStringMember);
        }

        [Fact]
        public void HasNameSet()
            => Assert.Equal("InterfaceShadowedTestProperty", DeclarationNode.Name);

        [Fact]
        public void HasAccessModifierSet()
            => Assert.Equal(AccessModifier.Public, DeclarationNode.AccessModifier);

        [Fact]
        public void HasDeclartingTypeSet()
            => Assert.True(typeof(ITestBaseInterface) == DeclarationNode.DeclaringType);

        [Fact]
        public void HasCircularReferenceSet()
            => Assert.Single(Assert.IsType<InterfaceDeclaration>(DeclarationNode.DeclaringType).Members, member => ReferenceEquals(member, DeclarationNode));

        [Fact]
        public void HasAttributesSet()
            => Assert.Empty(DeclarationNode.Attributes);

        [Fact]
        public void HasIsAbstractSet()
            => Assert.False(DeclarationNode.IsAbstract);

        [Fact]
        public void HasIsOverrideSet()
            => Assert.False(DeclarationNode.IsOverride);

        [Fact]
        public void HasIsSealedSet()
            => Assert.False(DeclarationNode.IsSealed);

        [Fact]
        public void HasIsShadowingSet()
            => Assert.False(DeclarationNode.IsShadowing);

        [Fact]
        public void HasIsStaticSet()
            => Assert.False(DeclarationNode.IsStatic);

        [Fact]
        public void HasIsVirtualSet()
            => Assert.False(DeclarationNode.IsVirtual);

        [Fact]
        public void HasParametersSet()
            => Assert.Empty(DeclarationNode.Parameters);

        [Fact]
        public void HasGetterAccessModifierSet()
            => Assert.Equal(AccessModifier.Public, DeclarationNode.Getter.AccessModifier);

        [Fact]
        public void HasGetterAttributesSet()
            => Assert.Empty(DeclarationNode.Getter.Attributes);

        [Fact]
        public void HasGetterReturnAttributesSet()
            => Assert.Empty(DeclarationNode.Getter.ReturnAttributes);

        [Fact]
        public void HasSetterAccessModifierSet()
            => Assert.Equal(AccessModifier.Public, DeclarationNode.Setter.AccessModifier);

        [Fact]
        public void HasSetterIsInitOnlySet()
            => Assert.False(DeclarationNode.Setter.IsInitOnly);

        [Fact]
        public void HasSetterAttributesSet()
            => Assert.Empty(DeclarationNode.Setter.Attributes);

        [Fact]
        public void HasSetterReturnAttributesSet()
            => Assert.Empty(DeclarationNode.Setter.ReturnAttributes);

        [Fact]
        public void HasTypeSet()
            => Assert.True(typeof(int) == DeclarationNode.Type);

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
            => Assert.NotEmpty(DeclarationNode.RelatedMembers);

        [Fact]
        public void HasExceptionsSet()
            => Assert.Empty(DeclarationNode.Exceptions);

        [Fact]
        public void HasValueSet()
            => Assert.Empty(DeclarationNode.Value.Content);

        [Fact]
        public void AcceptVisitor()
        {
            var visitor = new DeclarationNodeVisitorMock(DeclarationNode);

            DeclarationNode.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}