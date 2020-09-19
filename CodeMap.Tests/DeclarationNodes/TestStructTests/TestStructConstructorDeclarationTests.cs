using System;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestStructTests
{
    public class TestStructConstructorDeclarationTests : DeclarationNodeTests<ConstructorDeclaration>, IConstructorDeclarationTests
    {
        protected override bool DeclarationNodePredicate(ConstructorDeclaration constructorDeclaration)
            => constructorDeclaration.Name == nameof(TestStruct<int>) && constructorDeclaration.Parameters.Count == 1 && constructorDeclaration.DeclaringType.Name == nameof(TestStruct<int>);

        [Fact]
        public void HasNameSet()
            => Assert.Equal("TestStruct", DeclarationNode.Name);

        [Fact]
        public void HasAccessModifierSet()
            => Assert.Equal(AccessModifier.Public, DeclarationNode.AccessModifier);

        [Fact]
        public void HasDeclartingTypeSet()
            => Assert.True(typeof(TestStruct<>) == DeclarationNode.DeclaringType);

        [Fact]
        public void HasCircularReferenceSet()
            => Assert.Single(Assert.IsType<StructDeclaration>(DeclarationNode.DeclaringType).Members, member => ReferenceEquals(member, DeclarationNode));

        [Fact]
        public void HasAttributesSet()
            => Assert.Single(DeclarationNode.Attributes);

        [Fact]
        public void HasTestAttribute()
            => AssertAttribute<TestAttribute>(
                DeclarationNode.Attributes,
                new (string, object, Type)[] { ("value1", "struct constructor test 1", typeof(object)) },
                new (string, object, Type)[] { ("Value2", "struct constructor test 2", typeof(object)), ("Value3", "struct constructor test 3", typeof(object)) }
            );

        [Fact]
        public void HasParametersSet()
            => Assert.Single(DeclarationNode.Parameters);

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