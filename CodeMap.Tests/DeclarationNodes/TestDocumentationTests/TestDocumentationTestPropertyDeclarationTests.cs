using System;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestDocumentationTests
{
    public class TestDocumentationTestPropertyDeclarationTests : DeclarationNodeTests<PropertyDeclaration>
    {
        protected override bool DeclarationNodePredicate(PropertyDeclaration propertyDeclaration)
            => propertyDeclaration.Name == nameof(TestDocumentation.Member) && propertyDeclaration.DeclaringType.Name == nameof(TestDocumentation);

        [Fact]
        public void MemberEqualityComparison()
        {
            var propertyInfo = typeof(TestDocumentation).GetRuntimeProperty(nameof(TestDocumentation.Member));
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
            => Assert.Equal("Member", DeclarationNode.Name);

        [Fact]
        public void HasAccessModifierSet()
            => Assert.Equal(AccessModifier.Public, DeclarationNode.AccessModifier);

        [Fact]
        public void HasDeclartingTypeSet()
            => Assert.True(typeof(TestDocumentation) == DeclarationNode.DeclaringType);

        [Fact]
        public void HasCircularReferenceSet()
            => Assert.Single(Assert.IsType<ClassDeclaration>(DeclarationNode.DeclaringType).Members, member => ReferenceEquals(member, DeclarationNode));

        [Fact]
        public void HasNoAttributes()
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
        public void HasNoParameters()
            => Assert.Empty(DeclarationNode.Parameters);

        [Fact]
        public void HasGetterAccessModifierSet()
            => Assert.Equal(AccessModifier.Public, DeclarationNode.Getter.AccessModifier);

        [Fact]
        public void HasNoGetterAttributes()
            => Assert.Empty(DeclarationNode.Getter.Attributes);

        [Fact]
        public void HasNoGetterReturnAttributes()
            => Assert.Empty(DeclarationNode.Getter.ReturnAttributes);

        [Fact]
        public void HasNoSetter()
            => Assert.Null(DeclarationNode.Setter);

        [Fact]
        public void HasTypeSet()
            => Assert.True(typeof(int) == DeclarationNode.Type);

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
        public void HasEmptyExceptions()
            => Assert.Empty(DeclarationNode.Exceptions);

        [Fact]
        public void HasEmptyValue()
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