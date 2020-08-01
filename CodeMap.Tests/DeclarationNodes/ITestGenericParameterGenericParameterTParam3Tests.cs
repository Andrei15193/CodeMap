using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using System.Linq;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes
{
    public class ITestGenericParameterGenericParameterTParam3Tests : DeclarationNodeTests<InterfaceDeclaration>
    {
        protected override bool DeclarationNodePredicate(InterfaceDeclaration interfaceDeclaration)
            => interfaceDeclaration.Name == nameof(ITestGenericParameter<int, string, int, int, int, int>);

        protected GenericParameterData GenericParameter
            => DeclarationNode.GenericParameters.ElementAt(2);

        [Fact]
        public void HasNameSet()
            => Assert.Equal("TParam3", GenericParameter.Name);

        [Fact]
        public void HasPositionSet()
            => Assert.Equal(2, GenericParameter.Position);

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
            => Assert.True(GenericParameter.HasNonNullableValueTypeConstraint);

        [Fact]
        public void HasHasReferenceTypeConstraintSet()
            => Assert.False(GenericParameter.HasReferenceTypeConstraint);

        [Fact]
        public void HasTypeConstraintsSet()
            => Assert.Empty(GenericParameter.TypeConstraints);

        [Fact]
        public void HasEmptyDescription()
            => Assert.Empty(GenericParameter.Description);
    }
}