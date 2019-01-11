﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CodeMap.Elements;

namespace CodeMap
{
    /// <summary>
    /// Represents a <see cref="DocumentationElement"/> factory for elements corresponding to reflection objects
    /// (<see cref="Type"/>s, <see cref="MethodInfo"/>s and so on).
    /// </summary>
    public class ReflectionDocumentationElementFactory
    {
        private readonly DynamicTypeReference _dynamicTypeReference;
        private readonly DocumentationElementCache _referencesCache;
        private readonly MemberDocumentationCollection _membersDocumentation;

        /// <summary>Initializes a new instance of the <see cref="ReflectionDocumentationElementFactory"/> class.</summary>
        public ReflectionDocumentationElementFactory()
            : this(new MemberDocumentationCollection(Enumerable.Empty<MemberDocumentation>()))
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ReflectionDocumentationElementFactory"/> class.</summary>
        /// <param name="membersDocumentation">A collection of <see cref="MemberDocumentation"/> to associate to created <see cref="DocumentationElement"/>s.</param>
        public ReflectionDocumentationElementFactory(MemberDocumentationCollection membersDocumentation)
        {
            _dynamicTypeReference = new DynamicTypeReference();
            _referencesCache = new DocumentationElementCache();
            _membersDocumentation = membersDocumentation
                ?? throw new ArgumentNullException(nameof(membersDocumentation));
        }

        /// <summary>Creates a <see cref="TypeDocumentationElement"/> from the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> from which to create a <see cref="TypeDocumentationElement"/>.</param>
        /// <returns>Returns a <see cref="TypeDocumentationElement"/> from the provided <paramref name="type"/>.</returns>
        public TypeDocumentationElement Create(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.IsEnum)
                return _CreateEnum(type);
            else if (typeof(Delegate).IsAssignableFrom(type))
                return _CreateDelegate(type);
            else if (type.IsInterface)
                return _CreateInterface(type);
            else if (type.IsClass)
                return _CreateClass(type);
            else if (type.IsValueType)
                return _CreateStruct(type);

            throw new ArgumentException($"Unknown type: '{type.Name}'.", nameof(type));
        }

        private EnumDocumentationElement _CreateEnum(Type enumType)
        {
            _membersDocumentation.TryFind(enumType, out var memberDocumentation);
            var enumDocumentationElement = new EnumDocumentationElement
            {
                Name = enumType.Name,
                AccessModifier = _GetAccessModifierFrom(enumType),
                UnderlyingType = _GetTypeReference(enumType.GetEnumUnderlyingType()),
                Attributes = _MapAttributesDataFrom(enumType.CustomAttributes),
                Summary = memberDocumentation?.Summary,
                Remarks = memberDocumentation?.Remarks,
                Examples = memberDocumentation?.Examples,
                RelatedMembers = memberDocumentation?.RelatedMembersList
            };
            enumDocumentationElement.Members = enumType
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField)
                .Select(
                    field => _CreateConstant(field, enumDocumentationElement)
                )
                .OrderBy(constant => constant.Value)
                .AsReadOnlyList();

            return enumDocumentationElement;
        }

        private ConstantDocumentationElement _CreateConstant(FieldInfo field, TypeDocumentationElement declaringType)
        {
            _membersDocumentation.TryFind(field, out var memberDocumentation);
            return new ConstantDocumentationElement
            {
                Name = field.Name,
                AccessModifier = _GetAccessModifierFrom(field),
                Value = field.GetValue(null),
                Type = field.FieldType == typeof(object) && field.GetCustomAttribute<DynamicAttribute>() != null
                                        ? _dynamicTypeReference
                                        : _GetTypeReference(field.FieldType),
                Attributes = _MapAttributesDataFrom(field.CustomAttributes),
                DeclaringType = declaringType,
                Summary = memberDocumentation?.Summary,
                Remarks = memberDocumentation?.Remarks,
                Examples = memberDocumentation?.Examples,
                RelatedMembers = memberDocumentation?.RelatedMembersList
            };
        }

        private DelegateDocumentationElement _CreateDelegate(Type delegateType)
        {
            var invokeMethodInfo = delegateType.GetMethod(nameof(Action.Invoke), BindingFlags.Public | BindingFlags.Instance);
            return new DelegateDocumentationElement
            {
                Name = delegateType.Name,
                AccessModifier = _GetAccessModifierFrom(delegateType),
                Attributes = _MapAttributesDataFrom(delegateType.CustomAttributes),
                GenericParameters = delegateType
                    .GetGenericArguments()
                    .Select(_GetTypeReference)
                    .Cast<GenericParameterDocumentationElement>()
                    .AsReadOnlyList(),
                Parameters = invokeMethodInfo
                    .GetParameters()
                    .Select(parameter => _CreateParameter(parameter))
                    .AsReadOnlyList(),
                Return = new ReturnsDocumentationElement
                {
                    Type = invokeMethodInfo.ReturnType == typeof(object) && invokeMethodInfo.ReturnParameter.GetCustomAttribute<DynamicAttribute>() != null
                        ? _dynamicTypeReference
                        : _GetTypeReference(invokeMethodInfo.ReturnType),
                    Attributes = _MapAttributesDataFrom(invokeMethodInfo.ReturnParameter.CustomAttributes)
                }
            };
        }

        private TypeDocumentationElement _CreateInterface(Type type)
        {
            throw new NotImplementedException();
        }

        private TypeDocumentationElement _CreateClass(Type type)
        {
            throw new NotImplementedException();
        }

        private TypeDocumentationElement _CreateStruct(Type type)
        {
            throw new NotImplementedException();
        }

        private AccessModifier _GetAccessModifierFrom(Type type)
        {
            if (type.IsNested)
                if (type.IsNestedPublic)
                    return AccessModifier.Public;
                else if (type.IsNestedFamily)
                    return AccessModifier.Family;
                else if (type.IsNestedFamORAssem)
                    return AccessModifier.AssemblyOrFamily;
                else if (type.IsNestedFamANDAssem)
                    return AccessModifier.AssemblyAndFamily;
                else if (type.IsNestedAssembly)
                    return AccessModifier.Assembly;
                else if (type.IsNestedPrivate)
                    return AccessModifier.Private;
            if (type.IsPublic)
                return AccessModifier.Public;
            else
                return AccessModifier.Assembly;
        }

        private AccessModifier _GetAccessModifierFrom(FieldInfo field)
        {
            if (field.IsPublic)
                return AccessModifier.Public;
            else if (field.IsFamily)
                return AccessModifier.Family;
            else if (field.IsFamilyOrAssembly)
                return AccessModifier.AssemblyOrFamily;
            else if (field.IsFamilyAndAssembly)
                return AccessModifier.AssemblyAndFamily;
            else if (field.IsAssembly)
                return AccessModifier.Assembly;
            else
                return AccessModifier.Private;
        }

        private TypeReferenceDocumentationElement _GetTypeReference(Type type)
            => _referencesCache.GetFor(type, _CreateTypeReference, _InitializeTypeReference);

        private TypeReferenceDocumentationElement _CreateTypeReference(Type type)
        {
            if (type.IsGenericParameter)
                return new GenericParameterDocumentationElement();
            else if (type == typeof(void))
                return new VoidTypeReferenceDocumentationElement();
            else
                return new InstanceTypeDocumentationElement();
        }

        private void _InitializeTypeReference(Type type, TypeReferenceDocumentationElement typeReference)
        {
            switch (typeReference)
            {
                case GenericParameterDocumentationElement genericParameter:
                    var genericParameterAttributes = type.GenericParameterAttributes;
                    genericParameter.Name = type.Name;
                    genericParameter.IsCovariant = (genericParameterAttributes & GenericParameterAttributes.Covariant) == GenericParameterAttributes.Covariant;
                    genericParameter.IsContravariant = (genericParameterAttributes & GenericParameterAttributes.Contravariant) == GenericParameterAttributes.Contravariant;
                    genericParameter.HasNonNullableValueTypeConstraint = (genericParameterAttributes & GenericParameterAttributes.NotNullableValueTypeConstraint) == GenericParameterAttributes.NotNullableValueTypeConstraint;
                    genericParameter.HasReferenceTypeConstraint = (genericParameterAttributes & GenericParameterAttributes.ReferenceTypeConstraint) == GenericParameterAttributes.ReferenceTypeConstraint;
                    genericParameter.HasDefaultConstructorConstraint = (genericParameterAttributes & GenericParameterAttributes.DefaultConstructorConstraint) == GenericParameterAttributes.DefaultConstructorConstraint;
                    genericParameter.TypeConstraints = type.GetGenericParameterConstraints().Select(_GetTypeReference).ToList();
                    break;

                case InstanceTypeDocumentationElement instanceType:
                    instanceType.Name = type.Name;
                    instanceType.Namespace = type.Namespace;
                    instanceType.GenericArguments = type
                        .GetGenericArguments()
                        .Select(_GetTypeReference)
                        .AsReadOnlyList();
                    instanceType.Assembly = _referencesCache.GetFor(type.Assembly.GetName(), _CreateAssemblyReference);
                    break;
            }
        }

        private ParameterDocumentationElement _CreateParameter(ParameterInfo parameter)
            => new ParameterDocumentationElement
            {
                Name = parameter.Name,
                Type = parameter.ParameterType == typeof(object) && parameter.GetCustomAttribute<DynamicAttribute>() != null
                    ? _dynamicTypeReference
                    : _GetTypeReference(parameter.ParameterType),
                Attributes = _MapAttributesDataFrom(parameter.CustomAttributes),
                HasDefaultValue = parameter.HasDefaultValue,
                DefaultValue = parameter.HasDefaultValue ? parameter.RawDefaultValue : null
            };

        private AssemblyReferenceDocumentationElement _CreateAssemblyReference(AssemblyName assemblyName)
            => new AssemblyReferenceDocumentationElement
            {
                Name = assemblyName.Name,
                Version = assemblyName.Version,
                Culture = assemblyName.CultureName,
                PublicKeyToken = assemblyName.GetPublicKeyToken().ToBase16String()
            };

        private IReadOnlyCollection<AttributeData> _MapAttributesDataFrom(IEnumerable<CustomAttributeData> customAttributes)
            => (
                from customAttribute in customAttributes
                let constructorParameters = customAttribute.Constructor.GetParameters()
                select new AttributeData(
                    _GetTypeReference(customAttribute.AttributeType),
                    constructorParameters
                        .Zip(
                            customAttribute.ConstructorArguments,
                            (parameter, argument) =>
                                new AttributeParameterData
                                {
                                    Name = parameter.Name,
                                    Type = _GetTypeReference(parameter.ParameterType),
                                    Value = _CreateAttributeValue(parameter.ParameterType, argument)
                                }
                        )
                        .ToList(),
                    customAttribute
                        .NamedArguments
                        .Select(
                            namedArgument =>
                            {
                                var parameterType = namedArgument.IsField
                                    ? ((FieldInfo)namedArgument.MemberInfo).FieldType
                                    : ((PropertyInfo)namedArgument.MemberInfo).PropertyType;
                                return new AttributeParameterData
                                {
                                    Name = namedArgument.MemberName,
                                    Type = _GetTypeReference(parameterType),
                                    Value = _CreateAttributeValue(parameterType, namedArgument.TypedValue.Value)
                                };
                            }
                        )
                        .ToList()
            )
        )
        .ToList();

        private static object _CreateAttributeValue(Type type, CustomAttributeTypedArgument argument)
        {
            if (argument.Value == null)
                return null;

            if (type == typeof(object[]))
                return _GetAttributeValue(argument.ArgumentType, (object[])argument.Value);
            if (type == typeof(object))
                return _CreateAttributeValue(argument.ArgumentType, argument.Value);
            if (type.IsArray)
                return _GetAttributeValue(type, (object[])argument.Value);

            return _CreateAttributeValue(type, argument.Value);
        }

        private static object _GetAttributeValue(Type type, object[] values)
        {
            var result = new object[values.Length];
            for (var index = 0; index < values.Length; index++)
                result[index] = _CreateAttributeValue(type, values[index]);
            return result;
        }

        private static object _CreateAttributeValue(Type type, object value)
        {
            if (value == null)
                return null;

            if (type.IsEnum)
                return Enum.Parse(type, Convert.ToString(value));

            return value;
        }
    }
}