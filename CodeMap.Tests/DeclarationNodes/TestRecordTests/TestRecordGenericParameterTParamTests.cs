﻿using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestRecordTests
{
    public class TestRecordGenericParameterTParamTests : DeclarationNodeTests<RecordDeclaration>, IGenericParameterDataTests
    {
        protected override bool DeclarationNodePredicate(RecordDeclaration recordDeclaration)
            => recordDeclaration.Name == nameof(TestRecord<int>);

        protected GenericParameterData GenericParameter
            => Assert.Single(DeclarationNode.GenericParameters);

        [Fact]
        public void HasNameSet()
            => Assert.Equal("TParam", GenericParameter.Name);

        [Fact]
        public void HasPositionSet()
            => Assert.Equal(0, GenericParameter.Position);

        [Fact]
        public void HasIsCovariantSet()
            => Assert.False(GenericParameter.IsCovariant);

        [Fact]
        public void HasIsContravariantSet()
            => Assert.False(GenericParameter.IsContravariant);

        [Fact]
        public void HasHasDefaultConstructorConstraintSet()
            => Assert.False(GenericParameter.HasDefaultConstructorConstraint);

        [Fact]
        public void HasHasNonNullableValueTypeConstraintSet()
            => Assert.False(GenericParameter.HasNonNullableValueTypeConstraint);

        [Fact]
        public void HasHasReferenceTypeConstraintSet()
            => Assert.False(GenericParameter.HasReferenceTypeConstraint);

        [Fact]
        public void HasHasUnmanagedTypeConstraintSet()
            => Assert.False(GenericParameter.HasUnmanagedTypeConstraint);

        [Fact]
        public void HasTypeConstraintsSet()
            => Assert.Empty(GenericParameter.TypeConstraints);

        [Fact]
        public void HasDescriptionSet()
            => Assert.NotEmpty(GenericParameter.Description);
    }
}