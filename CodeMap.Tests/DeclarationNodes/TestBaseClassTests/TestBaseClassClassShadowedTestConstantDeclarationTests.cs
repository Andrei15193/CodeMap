using System;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestBaseClassTests
{
    public class TestBaseClassClassShadowedTestConstantDeclarationTests : DeclarationNodeTests<ConstantDeclaration>, IConstantDeclarationTests
    {
        protected override bool DeclarationNodePredicate(ConstantDeclaration constantDeclaration)
            => constantDeclaration.Name == nameof(TestBaseClass.ClassShadowedTestConstant) && constantDeclaration.DeclaringType.Name == nameof(TestBaseClass);

        [Fact]
        public void MemberEqualityComparison()
        {
            var fieldInfo = typeof(TestBaseClass).GetField("ClassShadowedTestConstant", BindingFlags.Public | BindingFlags.Static);
            Assert.True(DeclarationNode.Equals(fieldInfo));
            Assert.True(DeclarationNode.Equals(fieldInfo as object));
            Assert.True(fieldInfo == DeclarationNode);
            Assert.True(DeclarationNode == fieldInfo);

            var toStringMember = typeof(object).GetRuntimeMethod(nameof(object.ToString), Type.EmptyTypes);
            Assert.False(DeclarationNode.Equals(toStringMember));
            Assert.False(DeclarationNode.Equals(toStringMember as object));
            Assert.True(toStringMember != DeclarationNode);
            Assert.True(DeclarationNode != toStringMember);
        }

        [Fact]
        public void HasNameSet()
            => Assert.Equal("ClassShadowedTestConstant", DeclarationNode.Name);

        [Fact]
        public void HasDeclartingTypeSet()
            => Assert.True(typeof(TestBaseClass) == DeclarationNode.DeclaringType);

        [Fact]
        public void HasCircularReferenceSet()
            => Assert.Single(Assert.IsType<ClassDeclaration>(DeclarationNode.DeclaringType).Members, member => ReferenceEquals(member, DeclarationNode));

        [Fact]
        public void HasAttributesSet()
            => Assert.Empty(DeclarationNode.Attributes);

        [Fact]
        public void HasAccessModifierSet()
            => Assert.Equal(AccessModifier.Public, DeclarationNode.AccessModifier);

        [Fact]
        public void HasIsShadowingSet()
            => Assert.False(DeclarationNode.IsShadowing);

        [Fact]
        public void HasTypeSet()
            => Assert.True(typeof(float) == DeclarationNode.Type);

        [Fact]
        public void HasValueSet()
            => Assert.Equal(1f, DeclarationNode.Value);

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
        public void AcceptVisitor()
        {
            var visitor = new DeclarationNodeVisitorMock(DeclarationNode);

            DeclarationNode.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}