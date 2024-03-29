﻿using System;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestRecordTests
{
    public class TestRecordTestMethodDeclarationTests : DeclarationNodeTests<MethodDeclaration>, IMethodDeclarationTests
    {
        protected override bool DeclarationNodePredicate(MethodDeclaration methodDeclaration)
            => methodDeclaration.Name == nameof(TestRecord<int>.TestMethod) && methodDeclaration.DeclaringType.Name == nameof(TestRecord<int>);

        [Fact]
        public void MemberEqualityComparison()
        {
            var methodInfo = typeof(TestRecord<>).GetRuntimeMethod(nameof(TestRecord<int>.TestMethod), new[] { typeof(int), typeof(string) });
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
            => Assert.Equal("TestMethod", DeclarationNode.Name);

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
            => Assert.Single(DeclarationNode.Attributes);

        [Fact]
        public void HasTestAttribute()
            => AssertAttribute<TestAttribute>(
                DeclarationNode.Attributes,
                new (string, object, Type)[] { ("value1", "record method test 1", typeof(object)) },
                new (string, object, Type)[] { ("Value2", "record method test 2", typeof(object)), ("Value3", "record method test 3", typeof(object)) }
            );

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
        public void HasGenericParametersSet()
            => Assert.Empty(DeclarationNode.GenericParameters);

        [Fact]
        public void HasParametersSet()
            => Assert.Equal(2, DeclarationNode.Parameters.Count);

        [Fact]
        public void HasReturnAttributesSet()
            => Assert.Single(DeclarationNode.Return.Attributes);

        [Fact]
        public void HasReturnTestAttribute()
            => AssertAttribute<TestAttribute>(
                DeclarationNode.Return.Attributes,
                new (string, object, Type)[] { ("value1", "record method return test 1", typeof(object)) },
                new (string, object, Type)[] { ("Value2", "record method return test 2", typeof(object)), ("Value3", "record method return test 3", typeof(object)) }
            );

        [Fact]
        public void HasReturnTypeSet()
            => Assert.True(typeof(int) == DeclarationNode.Return.Type);

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
        public void HasReturnDescriptionSet()
            => Assert.NotEmpty(DeclarationNode.Return.Description);

        [Fact]
        public void HasExceptionsSet()
            => Assert.NotEmpty(DeclarationNode.Exceptions);

        [Fact]
        public void AcceptVisitor()
        {
            var visitor = new DeclarationNodeVisitorMock(DeclarationNode);

            DeclarationNode.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}