using System.Linq;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes.TestClassTests.NestedTestClassTests
{
    public class TestClassNestedTestClassGenericParameterTParam3Tests : DeclarationNodeTests<ClassDeclaration>
    {
        protected override bool DeclarationNodePredicate(ClassDeclaration classDeclaration)
            => classDeclaration.Name == nameof(TestClass<int>.NestedTestClass<int, int>) && classDeclaration.DeclaringType.Name == nameof(TestClass<int>);

        protected GenericParameterData GenericParameter
            => DeclarationNode.GenericParameters.ElementAt(1);

        [Fact]
        public void HasNameSet()
            => Assert.Equal("TParam3", GenericParameter.Name);

        [Fact]
        public void HasPositionSet()
            => Assert.Equal(1, GenericParameter.Position);

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
        public void HasTypeConstraintsSet()
            => Assert.Empty(GenericParameter.TypeConstraints);

        [Fact]
        public void HasEmptyDescription()
            => Assert.Empty(GenericParameter.Description);
    }
}