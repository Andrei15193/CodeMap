﻿using System.Linq;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.ITestGenericParameter
{
    public class ITestGenericParameterGenericParameterTParam7Tests : DeclarationNodeTests<InterfaceDeclaration>, IGenericParameterDataTests
    {
        protected override bool DeclarationNodePredicate(InterfaceDeclaration interfaceDeclaration)
            => interfaceDeclaration.Name == nameof(ITestGenericParameter<int, string, int, int, int, int, int>);

        protected GenericParameterData GenericParameter
            => DeclarationNode.GenericParameters.ElementAt(6);

        [Fact]
        public void HasNameSet()
            => Assert.Equal("TParam7", GenericParameter.Name);

        [Fact]
        public void HasPositionSet()
            => Assert.Equal(6, GenericParameter.Position);

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
            => Assert.True(GenericParameter.HasUnmanagedTypeConstraint);

        [Fact]
        public void HasTypeConstraintsSet()
            => Assert.Empty(GenericParameter.TypeConstraints);

        [Fact]
        public void HasDescriptionSet()
            => Assert.NotEmpty(GenericParameter.Description);
    }
}