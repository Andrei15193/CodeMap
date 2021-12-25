using System;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestRecordTests
{
    public class TestRecordRecordShadowedTestMethodDeclarationTests : DeclarationNodeTests<MethodDeclaration>, IMethodDeclarationTests
    {
        protected override bool DeclarationNodePredicate(MethodDeclaration methodDeclaration)
            => methodDeclaration.Name == nameof(TestRecord<int>.RecordShadowedTestMethod) && methodDeclaration.DeclaringType.Name == nameof(TestRecord<int>);

        [Fact]
        public void MemberEqualityComparison()
        {
            var methodInfo = typeof(TestRecord<>).GetRuntimeMethod(nameof(TestRecord<int>.RecordShadowedTestMethod), Type.EmptyTypes);
            Assert.True(DeclarationNode.Equals(methodInfo));
            Assert.True(DeclarationNode.Equals(methodInfo as object));
            Assert.True(methodInfo == DeclarationNode);
            Assert.True(DeclarationNode == methodInfo);

            var toStringMember = typeof(object).GetRuntimeMethod(nameof(object.ToString), Type.EmptyTypes);
            Assert.False(DeclarationNode.Equals(toStringMember));
            Assert.False(DeclarationNode.Equals(toStringMember as object));
            Assert.True(toStringMember != DeclarationNode);
            Assert.True(DeclarationNode != toStringMember);
        }

        [Fact]
        public void HasNameSet()
            => Assert.Equal("RecordShadowedTestMethod", DeclarationNode.Name);

        [Fact]
        public void HasAccessModifierSet()
            => Assert.Equal(AccessModifier.Public, DeclarationNode.AccessModifier);

        [Fact]
        public void HasDeclartingTypeSet()
            => Assert.True(typeof(TestRecord<>) == DeclarationNode.DeclaringType);

        [Fact]
        public void HasCircularReferenceSet()
            => Assert.Single(Assert.IsType<RecordDeclaration>(DeclarationNode.DeclaringType).Members, member => ReferenceEquals(member, DeclarationNode));

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
            => Assert.True(DeclarationNode.IsShadowing);

        [Fact]
        public void HasIsStaticSet()
            => Assert.False(DeclarationNode.IsStatic);

        [Fact]
        public void HasIsVirtualSet()
            => Assert.False(DeclarationNode.IsVirtual);

        [Fact]
        public void HasGenericParametersSet()
            => Assert.Empty(DeclarationNode.GenericParameters);

        [Fact]
        public void HasParametersSet()
            => Assert.Empty(DeclarationNode.Parameters);

        [Fact]
        public void HasReturnAttributesSet()
            => Assert.Empty(DeclarationNode.Return.Attributes);

        [Fact]
        public void HasReturnTypeSet()
            => Assert.True(typeof(int) == DeclarationNode.Return.Type);

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
            => Assert.NotEmpty(DeclarationNode.Return.Description);

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