using System;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.ITestBaseInterfaceTests
{
    public class ITestBaseInterfaceBaseTestMethodDeclarationTests : DeclarationNodeTests<MethodDeclaration>
    {
        protected override bool DeclarationNodePredicate(MethodDeclaration methodDeclaration)
            => methodDeclaration.Name == nameof(ITestBaseInterface.BaseTestMethod) && methodDeclaration.DeclaringType.Name == nameof(ITestBaseInterface);

        [Fact]
        public void MemberEqualityComparison()
        {
            var methodInfo = typeof(ITestBaseInterface).GetRuntimeMethod(nameof(ITestBaseInterface.BaseTestMethod), Type.EmptyTypes);
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
            => Assert.Equal("BaseTestMethod", DeclarationNode.Name);

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
        public void HasNoReturnAttributes()
            => Assert.Empty(DeclarationNode.Return.Attributes);

        [Fact]
        public void HasReturnTypeSet()
            => Assert.True(typeof(void) == DeclarationNode.Return.Type);

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
        public void HasEmptyReturnDescription()
            => Assert.Empty(DeclarationNode.Return.Description);

        [Fact]
        public void HasEmptyExceptions()
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