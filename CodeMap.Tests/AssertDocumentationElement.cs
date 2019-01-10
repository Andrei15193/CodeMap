using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeMap.Elements;
using Xunit;

namespace CodeMap.Tests
{
    internal static class AssertDocumentationElement
    {
        public static void AssertTypeDefinition(Type expected, TypeDocumentationElement actual)
        {
            if (expected == null)
                Assert.Null(actual);
            else
            {
                Assert.NotNull(actual);
                Assert.Equal(expected.Name, actual.Name);
                Assert.Equal(expected.IsPublic ? AccessModifier.Public : AccessModifier.Assembly, actual.AccessModifier);
                Assert.Null(actual.DeclaringType);
                AssertAttributes(CustomAttributeData.GetCustomAttributes(expected), actual.Attributes);
            }
        }

        public static void AssertTypeDefinitions(IEnumerable<Type> expected, IEnumerable<TypeDocumentationElement> actual)
        {
            var expectedTypes = expected as IReadOnlyCollection<Type> ?? expected.ToList();
            var actualTypes = actual as IReadOnlyCollection<TypeDocumentationElement> ?? actual.ToList();
            Assert.Equal(expectedTypes.Count, actualTypes.Count);
            foreach (var pair in expectedTypes.Zip(actualTypes, (expectedType, actualType) => new { ExpectedType = expectedType, ActualType = actualType }))
                AssertTypeDefinition(pair.ExpectedType, pair.ActualType);
        }

        public static void AssertGenericParameter(Type expected, GenericParameterDocumentationElement actual, IReadOnlyDictionary<Type, TypeReferenceDocumentationElement> visited = null)
        {
            if (expected == null)
                Assert.Null(actual);
            else
            {
                Assert.NotNull(actual);
                Assert.True(expected.IsGenericParameter);
                Assert.Equal(expected.Name, actual.Name);
                _AssertGenericParameterAttribute(expected, GenericParameterAttributes.Covariant, () => actual.IsCovariant);
                _AssertGenericParameterAttribute(expected, GenericParameterAttributes.Contravariant, () => actual.IsContravariant);
                _AssertGenericParameterAttribute(expected, GenericParameterAttributes.ReferenceTypeConstraint, () => actual.HasReferenceTypeConstraint);
                _AssertGenericParameterAttribute(expected, GenericParameterAttributes.NotNullableValueTypeConstraint, () => actual.HasNonNullableValueTypeConstraint);
                _AssertGenericParameterAttribute(expected, GenericParameterAttributes.DefaultConstructorConstraint, () => actual.HasDefaultConstructorConstraint);
                AssertTypeReferences(
                    expected.GetGenericParameterConstraints(),
                    actual.TypeConstraints,
                    new Dictionary<Type, TypeReferenceDocumentationElement>(visited ?? Enumerable.Empty<KeyValuePair<Type, TypeReferenceDocumentationElement>>())
                    {
                        { expected, actual }
                    }
                );
            }
        }

        public static void AssertGenericParameters(IEnumerable<Type> expected, IEnumerable<GenericParameterDocumentationElement> actual)
        {
            var expectedGenericParameters = expected as IReadOnlyCollection<Type> ?? expected.ToList();
            var actualGenericParameters = actual as IReadOnlyCollection<GenericParameterDocumentationElement> ?? actual.ToList();
            Assert.Equal(expectedGenericParameters.Count, actualGenericParameters.Count);
            foreach (var pair in expectedGenericParameters.Zip(actualGenericParameters, (expectedGenericParameter, actualGenericParameter) => new { ExpectedGenericParameter = expectedGenericParameter, ActualGenericParameter = actualGenericParameter }))
                AssertGenericParameter(pair.ExpectedGenericParameter, pair.ActualGenericParameter);
        }

        public static void AssertParameter(ParameterInfo expected, ParameterDocumentationElement actual)
        {
            if (expected == null)
                Assert.Null(actual);
            else
            {
                Assert.NotNull(actual);
                Assert.Equal(expected.Name, actual.Name);
                AssertTypeReference(expected.ParameterType, actual.Type);
                if (expected.HasDefaultValue)
                {
                    Assert.True(actual.HasDefaultValue);
                    Assert.Equal(expected.RawDefaultValue, actual.DefaultValue);
                }
                else
                {
                    Assert.False(actual.HasDefaultValue);
                    Assert.Null(actual.DefaultValue);
                }
                AssertAttributes(expected.CustomAttributes, actual.Attributes);
            }
        }

        public static void AssertParameters(IEnumerable<ParameterInfo> expected, IEnumerable<ParameterDocumentationElement> actual)
        {
            var expectedParameters = expected as IReadOnlyCollection<ParameterInfo> ?? expected.ToList();
            var actualParameters = actual as IReadOnlyCollection<ParameterDocumentationElement> ?? actual.ToList();
            Assert.Equal(expectedParameters.Count, actualParameters.Count);
            foreach (var pair in expectedParameters.Zip(actualParameters, (expectedParameter, actualParameter) => new { ExpectedParameter = expectedParameter, ActualParameter = actualParameter }))
                AssertParameter(pair.ExpectedParameter, pair.ActualParameter);
        }

        private static void _AssertGenericParameterAttribute(Type genericParameter, GenericParameterAttributes genericParameterAttribute, Func<bool> selector)
        {
            if (genericParameter.GenericParameterAttributes.HasFlag(genericParameterAttribute))
                Assert.True(selector());
            else
                Assert.False(selector());
        }

        public static void AssertConstantDefinition(TypeDocumentationElement expectedDeclaringType, FieldInfo expected, ConstantDocumentationElement actual)
        {
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(AccessModifier.Public, actual.AccessModifier);
            Assert.Same(expectedDeclaringType, actual.DeclaringType);
            AssertTypeReference(expected.FieldType, actual.Type);
            Assert.Equal(expected.GetValue(null), actual.Value);
            AssertAttributes(CustomAttributeData.GetCustomAttributes(expected), actual.Attributes);
        }

        public static void AssertConstantDefinitions(TypeDocumentationElement expectedDeclaringType, IEnumerable<FieldInfo> expected, IEnumerable<ConstantDocumentationElement> actual)
        {
            var expectedConstants = expected as IReadOnlyCollection<FieldInfo> ?? expected.ToList();
            var actualConstants = actual as IReadOnlyCollection<ConstantDocumentationElement> ?? actual.ToList();
            Assert.Equal(expectedConstants.Count, actualConstants.Count);
            foreach (var pair in expectedConstants.Zip(actualConstants, (expectedConstant, actualConstant) => new { ExpectedConstant = expectedConstant, ActualConstant = actualConstant }))
                AssertConstantDefinition(expectedDeclaringType, pair.ExpectedConstant, pair.ActualConstant);
        }

        public static void AssertTypeReference(Type expected, TypeReferenceDocumentationElement actual, IReadOnlyDictionary<Type, TypeReferenceDocumentationElement> visited = null)
        {
            if (expected == null)
                Assert.Null(actual);
            else
            {
                Assert.NotNull(actual);
                switch (actual)
                {
                    case InstanceTypeDocumentationElement actualInstanceTypeDocumentationElement:
                        _AssertType(expected, actualInstanceTypeDocumentationElement, visited);
                        break;

                    case GenericParameterDocumentationElement genericParameterDocumentationElement:
                        if (visited == null || !visited.ContainsKey(expected))
                            AssertGenericParameter(expected, genericParameterDocumentationElement, visited);
                        break;

                    case VoidTypeReferenceDocumentationElement voidTypeReference:
                        Assert.Equal(typeof(void), expected);
                        break;

                    case DynamicTypeReference dynamicTypeReference:
                        Assert.Equal(typeof(object), expected);
                        break;

                    default:
                        throw new InvalidOperationException($"Untreated concrete type reference class, {actual.GetType().Name}");
                }
            }
        }

        public static void AssertTypeReferences(IEnumerable<Type> expected, IEnumerable<TypeReferenceDocumentationElement> actual, IReadOnlyDictionary<Type, TypeReferenceDocumentationElement> visited = null)
        {
            var expectedTypes = expected as IReadOnlyCollection<Type> ?? expected.ToList();
            var actualTypes = actual as IReadOnlyCollection<TypeReferenceDocumentationElement> ?? actual.ToList();
            Assert.Equal(expectedTypes.Count, actualTypes.Count);
            foreach (var pair in expectedTypes.Zip(actualTypes, (expectedType, actualType) => new { ExpectedType = expectedType, ActualType = actualType }))
                AssertTypeReference(pair.ExpectedType, pair.ActualType, visited);
        }

        public static void AssertAttribute(CustomAttributeData expected, AttributeData actual)
        {
            if (expected == null)
                Assert.Null(actual);
            else
            {
                Assert.NotNull(actual);
                AssertTypeReference(expected.AttributeType, actual.Type);

                var positionalArguments = expected
                    .Constructor
                    .GetParameters()
                    .Zip(
                        expected.ConstructorArguments,
                        (parameter, argument) => new ArgumentData
                        {
                            Name = parameter.Name,
                            Type = parameter.ParameterType,
                            Value = argument.Value
                        }
                    );
                _AssertArguments(positionalArguments, actual.PositionalParameters);

                var namedArguments = expected
                    .NamedArguments
                    .Select(
                        namedArgument => new ArgumentData
                        {
                            Name = namedArgument.MemberName,
                            Type = namedArgument.MemberInfo is FieldInfo fieldInfo ? fieldInfo.FieldType : ((PropertyInfo)namedArgument.MemberInfo).PropertyType,
                            Value = namedArgument.TypedValue.Value
                        }
                    );
                _AssertArguments(namedArguments, actual.NamedParameters);
            }
        }

        public static void AssertAttributes(IEnumerable<CustomAttributeData> expected, IEnumerable<AttributeData> actual)
        {
            var expectedAttributes = expected as IReadOnlyCollection<CustomAttributeData> ?? expected.ToList();
            var actualAttributes = actual as IReadOnlyCollection<AttributeData> ?? actual.ToList();
            Assert.Equal(expectedAttributes.Count, actualAttributes.Count);
            foreach (var pair in expectedAttributes.Zip(actualAttributes, (expectedAttribute, actualAttribute) => new { ExpectedAttribute = expectedAttribute, ActualAttribute = actualAttribute }))
                AssertAttribute(pair.ExpectedAttribute, pair.ActualAttribute);
        }

        public static void AssertAssemblyReference(AssemblyName expected, AssemblyReferenceDocumentationElement actual)
        {
            if (expected == null)
                Assert.Null(actual);
            else
            {
                Assert.NotNull(actual);
                Assert.Equal(expected.Name, actual.Name);
                Assert.Equal(expected.Version, actual.Version);
                Assert.Equal(expected.CultureName, actual.Culture);
                Assert.Equal(
                    string.Join(string.Empty, expected.GetPublicKeyToken().Select(@byte => @byte.ToString("X2"))),
                    actual.PublicKeyToken,
                    ignoreCase: true
                );
            }
        }

        public static void AssertAssemblyReferences(IEnumerable<AssemblyName> expected, IEnumerable<AssemblyReferenceDocumentationElement> actual)
        {
            var expectedAssemblyReferences = expected as IReadOnlyCollection<AssemblyName> ?? expected.ToList();
            var actualAssemblyReferences = actual as IReadOnlyCollection<AssemblyReferenceDocumentationElement> ?? actual.ToList();
            Assert.Equal(expectedAssemblyReferences.Count, actualAssemblyReferences.Count);
            foreach (var pair in expectedAssemblyReferences.Zip(actualAssemblyReferences, (expectedAssemblyReference, actualAssemblyReference) => new { ExpectedAssemblyReference = expectedAssemblyReference, ActualAssemblyReference = actualAssemblyReference }))
                AssertAssemblyReference(pair.ExpectedAssemblyReference, pair.ActualAssemblyReference);
        }

        private static void _AssertType(Type expected, InstanceTypeDocumentationElement actual, IReadOnlyDictionary<Type, TypeReferenceDocumentationElement> visited)
        {
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Namespace, actual.Namespace);
            AssertTypeReferences(expected.GetGenericArguments(), actual.GenericArguments, visited);
            AssertTypeReference(expected.DeclaringType, actual.DeclaringType);
            AssertAssemblyReference(expected.Assembly.GetName(), actual.Assembly);
        }

        private static void _AssertArguments(IEnumerable<ArgumentData> expected, IReadOnlyList<AttributeParameterData> actual)
        {
            var expectedArguments = expected as IReadOnlyCollection<ArgumentData> ?? expected.ToList();
            var actualArguments = actual as IReadOnlyCollection<AttributeParameterData> ?? actual.ToList();
            Assert.Equal(expectedArguments.Count, actualArguments.Count);
            foreach (var pair in expectedArguments.Zip(actualArguments, (expectedArgument, actualArgument) => new { ExpectedArgument = expectedArgument, ActualArgument = actualArgument }))
                _AssertArgument(pair.ExpectedArgument, pair.ActualArgument);
        }

        private static void _AssertArgument(ArgumentData expected, AttributeParameterData actual)
        {
            Assert.Equal(expected.Name, actual.Name);
            AssertTypeReference(expected.Type, actual.Type);
            Assert.Equal(expected.Value, actual.Value);
        }

        private class ArgumentData
        {
            public string Name { get; set; }

            public Type Type { get; set; }

            public object Value { get; set; }
        }
    }
}