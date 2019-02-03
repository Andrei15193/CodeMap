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
        private readonly DynamicTypeReferenceDocumentationElement _dynamicTypeReference = new DynamicTypeReferenceDocumentationElement();
        private readonly DocumentationElementCache _referencesCache = new DocumentationElementCache();
        private readonly CanonicalNameResolver _canonicalNameResolver = new CanonicalNameResolver(
            new[] { typeof(ReflectionDocumentationElementFactory).Assembly }
                .Concat(typeof(ReflectionDocumentationElementFactory)
                    .Assembly
                    .GetReferencedAssemblies()
                    .Select(Assembly.Load))
            );
        private readonly MemberDocumentation _emptyMemberDocumentation = new MemberDocumentation(string.Empty, null, null, null, null, null, null, null, null, null);
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
            var memberDocumentation = _GetMemberDocumentationFor(enumType);
            var enumDocumentationElement = new EnumDocumentationElement
            {
                Name = _GetTypeNameFor(enumType),
                AccessModifier = _GetAccessModifierFrom(enumType),
                UnderlyingType = _GetTypeReference(enumType.GetEnumUnderlyingType()),
                Attributes = _MapAttributesDataFrom(enumType.CustomAttributes),
                Summary = memberDocumentation.Summary,
                Remarks = memberDocumentation.Remarks,
                Examples = memberDocumentation.Examples,
                RelatedMembers = memberDocumentation.RelatedMembers
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

        private DelegateDocumentationElement _CreateDelegate(Type delegateType)
        {
            var memberDocumentation = _GetMemberDocumentationFor(delegateType);
            var invokeMethodInfo = delegateType.GetMethod(nameof(Action.Invoke), BindingFlags.Public | BindingFlags.Instance);
            var delegateDocumentationElement = new DelegateDocumentationElement
            {
                Name = _GetTypeNameFor(delegateType),
                AccessModifier = _GetAccessModifierFrom(delegateType),
                Attributes = _MapAttributesDataFrom(delegateType.CustomAttributes),
                Parameters = invokeMethodInfo
                    .GetParameters()
                    .Select(parameter => _CreateParameter(parameter, memberDocumentation))
                    .AsReadOnlyList(),
                Return = new ReturnsDocumentationElement
                {
                    Type = invokeMethodInfo.ReturnType == typeof(object) && invokeMethodInfo.ReturnParameter.GetCustomAttribute<DynamicAttribute>() != null
                        ? _dynamicTypeReference
                        : _GetTypeReference(invokeMethodInfo.ReturnType),
                    Description = memberDocumentation.Returns,
                    Attributes = _MapAttributesDataFrom(invokeMethodInfo.ReturnParameter.CustomAttributes)
                },
                Summary = memberDocumentation.Summary,
                Remarks = memberDocumentation.Remarks,
                Examples = memberDocumentation.Examples,
                Exceptions = _MapExceptions(memberDocumentation.Exceptions),
                RelatedMembers = memberDocumentation.RelatedMembers
            };

            delegateDocumentationElement.GenericParameters = delegateType
                .GetGenericArguments()
                .Select(typeGenericParameter => _GetTypeGenericParameter(typeGenericParameter, delegateDocumentationElement))
                .AsReadOnlyList();

            return delegateDocumentationElement;
        }

        private TypeDocumentationElement _CreateInterface(Type interfaceType)
        {
            var memberDocumentation = _GetMemberDocumentationFor(interfaceType);
            var interfaceDocumentationElement = new InterfaceDocumentationElement
            {
                Name = _GetTypeNameFor(interfaceType),
                AccessModifier = _GetAccessModifierFrom(interfaceType),
                Attributes = _MapAttributesDataFrom(interfaceType.CustomAttributes),
                BaseInterfaces = interfaceType
                    .GetInterfaces()
                    .Except(interfaceType
                        .GetInterfaces()
                        .SelectMany(baseInterface => baseInterface.GetInterfaces())
                    )
                    .Select(_GetTypeReference)
                    .AsReadOnlyCollection(),
                Summary = memberDocumentation.Summary,
                Remarks = memberDocumentation.Remarks,
                Examples = memberDocumentation.Examples,
                RelatedMembers = memberDocumentation.RelatedMembers
            };

            interfaceDocumentationElement.GenericParameters = interfaceType
                .GetGenericArguments()
                .Select(typeGenericParameter => _GetTypeGenericParameter(typeGenericParameter, interfaceDocumentationElement))
                .AsReadOnlyList();
            interfaceDocumentationElement.Events = interfaceType
                .GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(@event => _GetEvent(@event, interfaceDocumentationElement))
                .AsReadOnlyCollection();
            interfaceDocumentationElement.Properties = interfaceType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(property => _GetProperty(property, interfaceDocumentationElement))
                .AsReadOnlyCollection();
            interfaceDocumentationElement.Methods = interfaceType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(method => !method.IsSpecialName)
                .Select(method => _GetMethod(method, interfaceDocumentationElement))
                .AsReadOnlyCollection();

            return interfaceDocumentationElement;
        }

        private TypeDocumentationElement _CreateClass(Type type)
        {
            throw new NotImplementedException();
        }

        private TypeDocumentationElement _CreateStruct(Type type)
        {
            throw new NotImplementedException();
        }

        private ConstantDocumentationElement _CreateConstant(FieldInfo field, TypeDocumentationElement declaringType)
        {
            var memberDocumentation = _GetMemberDocumentationFor(field);
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
                Summary = memberDocumentation.Summary,
                Remarks = memberDocumentation.Remarks,
                Examples = memberDocumentation.Examples,
                RelatedMembers = memberDocumentation.RelatedMembers
            };
        }

        private EventDocumentationElement _GetEvent(EventInfo @event, TypeDocumentationElement declaringType)
        {
            var memberDocumentation = _GetMemberDocumentationFor(@event);
            return new EventDocumentationElement
            {
                Name = @event.Name,
                AccessModifier = _GetAccessModifierFrom(@event),
                Type = _GetTypeReference(@event.EventHandlerType),
                Attributes = _MapAttributesDataFrom(@event.CustomAttributes),
                DeclaringType = declaringType,
                IsStatic = false,
                IsAbstract = false,
                IsVirtual = false,
                IsOverride = false,
                IsSealed = false,
                IsShadowing = _IsShadowing(@event),
                Adder = _GetEventAccessorData(@event.AddMethod),
                Remover = _GetEventAccessorData(@event.RemoveMethod),
                Summary = memberDocumentation.Summary,
                Exceptions = _MapExceptions(memberDocumentation.Exceptions),
                Remarks = memberDocumentation.Remarks,
                Examples = memberDocumentation.Examples,
                RelatedMembers = memberDocumentation.RelatedMembers
            };
        }

        private EventAccessorData _GetEventAccessorData(MethodInfo accessorMethod)
        {
            return new EventAccessorData
            {
                Attributes = _MapAttributesDataFrom(accessorMethod.CustomAttributes),
                ReturnAttributes = _MapAttributesDataFrom(accessorMethod.ReturnParameter.CustomAttributes)
            };
        }

        private PropertyDocumentationElement _GetProperty(PropertyInfo property, TypeDocumentationElement declaringType)
        {
            var memberDocumentation = _GetMemberDocumentationFor(property);

            var getterInfo = _GetPropertyAccessorData(property.GetMethod);
            var setterInfo = _GetPropertyAccessorData(property.SetMethod);
            return new PropertyDocumentationElement
            {
                Name = property.Name,
                AccessModifier = setterInfo == null || (getterInfo != null && getterInfo.AccessModifier >= setterInfo.AccessModifier)
                    ? getterInfo.AccessModifier
                    : setterInfo.AccessModifier,
                Type = _GetTypeReference(property.PropertyType),
                Attributes = _MapAttributesDataFrom(property.CustomAttributes),
                Parameters = property
                    .GetIndexParameters()
                    .Select(parameter => _CreateParameter(parameter, memberDocumentation))
                    .AsReadOnlyList(),
                DeclaringType = declaringType,
                IsStatic = false,
                IsAbstract = false,
                IsVirtual = false,
                IsOverride = false,
                IsSealed = false,
                IsShadowing = _IsShadowing(property),
                Getter = getterInfo,
                Setter = setterInfo,
                Summary = memberDocumentation.Summary,
                Value = memberDocumentation.Value,
                Exceptions = _MapExceptions(memberDocumentation.Exceptions),
                Remarks = memberDocumentation.Remarks,
                Examples = memberDocumentation.Examples,
                RelatedMembers = memberDocumentation.RelatedMembers
            };
        }

        private MethodDocumentationElement _GetMethod(MethodInfo method, TypeDocumentationElement declaringType)
        {
            var memberDocumentation = _GetMemberDocumentationFor(method);

            var methodDocumentationElement = new MethodDocumentationElement
            {
                Name = method.Name,
                AccessModifier = _GetAccessModifierFrom(method),
                Attributes = _MapAttributesDataFrom(method.CustomAttributes),
                DeclaringType = declaringType,
                IsStatic = false,
                IsAbstract = false,
                IsVirtual = false,
                IsOverride = false,
                IsSealed = false,
                IsShadowing = _IsShadowing(method),
                Parameters = method
                    .GetParameters()
                    .Select(parameter => _CreateParameter(parameter, memberDocumentation))
                    .AsReadOnlyList(),
                Return = new ReturnsDocumentationElement
                {
                    Type = method.ReturnType == typeof(object) && method.ReturnParameter.GetCustomAttribute<DynamicAttribute>() != null
                        ? _dynamicTypeReference
                        : _GetTypeReference(method.ReturnType),
                    Description = memberDocumentation.Returns,
                    Attributes = _MapAttributesDataFrom(method.ReturnParameter.CustomAttributes)
                },
                Summary = memberDocumentation.Summary,
                Exceptions = _MapExceptions(memberDocumentation.Exceptions),
                Remarks = memberDocumentation.Remarks,
                Examples = memberDocumentation.Examples,
                RelatedMembers = memberDocumentation.RelatedMembers
            };

            methodDocumentationElement.GenericParameters = method
                .GetGenericArguments()
                .Select(typeGenericParameter => _GetMethodGenericParameter(typeGenericParameter, methodDocumentationElement))
                .AsReadOnlyList();

            return methodDocumentationElement;
        }

        private PropertyAccessorData _GetPropertyAccessorData(MethodInfo accessorMethod)
        {
            if (accessorMethod == null)
                return null;

            return new PropertyAccessorData
            {
                AccessModifier = _GetAccessModifierFrom(accessorMethod),
                Attributes = _MapAttributesDataFrom(accessorMethod.CustomAttributes),
                ReturnAttributes = _MapAttributesDataFrom(accessorMethod.ReturnParameter.CustomAttributes)
            };
        }

        private string _GetTypeNameFor(Type type)
        {
            var backtickIndex = type.Name.IndexOf('`');
            return backtickIndex >= 0 ? type.Name.Substring(0, backtickIndex) : type.Name;
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

        private AccessModifier _GetAccessModifierFrom(EventInfo @event)
            => _GetAccessModifierFrom(@event.AddMethod ?? @event.RemoveMethod);

        private AccessModifier _GetAccessModifierFrom(MethodBase method)
        {
            if (method.IsPublic)
                return AccessModifier.Public;
            else if (method.IsFamily)
                return AccessModifier.Family;
            else if (method.IsFamilyOrAssembly)
                return AccessModifier.AssemblyOrFamily;
            else if (method.IsFamilyAndAssembly)
                return AccessModifier.AssemblyAndFamily;
            else if (method.IsAssembly)
                return AccessModifier.Assembly;
            else
                return AccessModifier.Private;
        }

        private bool _IsShadowing(MemberInfo memberInfo)
        {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy;
            var inheritedMembers = memberInfo
                .DeclaringType
                .GetInterfaces()
                .SelectMany(baseInterface => baseInterface.GetMembers(bindingFlags));

            if (memberInfo.DeclaringType.BaseType != null)
                inheritedMembers = inheritedMembers.Concat(
                    memberInfo
                        .DeclaringType
                        .BaseType
                        .GetMembers(bindingFlags)
                );

            return memberInfo is MethodInfo methodInfo
                ? inheritedMembers
                    .OfType<MethodInfo>()
                    .Any(
                        inheritedMethodInfo =>
                            string.Equals(methodInfo.Name, inheritedMethodInfo.Name, StringComparison.OrdinalIgnoreCase)
                            && methodInfo
                                .GetParameters()
                                .Select(parameter => _UnwrapeByRef(parameter.ParameterType))
                                .SequenceEqual(
                                    inheritedMethodInfo
                                        .GetParameters()
                                        .Select(parameter => _UnwrapeByRef(parameter.ParameterType))
                                )
                    )
                : inheritedMembers.Any(
                    inheritedMemberInfo =>
                        string.Equals(memberInfo.Name, inheritedMemberInfo.Name, StringComparison.OrdinalIgnoreCase)
                );
        }

        private TypeReferenceDocumentationElement _GetTypeReference(Type type)
            => _referencesCache.GetFor(_UnwrapeByRef(type), _CreateTypeReference, _InitializeTypeReference);

        private TypeGenericParameterDocumentationElement _GetTypeGenericParameter(Type type, TypeDocumentationElement declaringType)
        {
            var typeGenericParameter = (TypeGenericParameterDocumentationElement)_GetTypeReference(type);
            typeGenericParameter.DeclaringType = declaringType;
            return typeGenericParameter;
        }

        private MethodGenericParameterDocumentationElement _GetMethodGenericParameter(Type type, MethodDocumentationElement declaringMethod)
        {
            var typeGenericParameter = (MethodGenericParameterDocumentationElement)_GetTypeReference(type);
            typeGenericParameter.DeclaringMethod = declaringMethod;
            return typeGenericParameter;
        }

        private static Type _UnwrapeByRef(Type type)
        {
            var referentType = type;
            while (referentType.IsByRef)
                referentType = referentType.GetElementType();
            return referentType;
        }

        private TypeReferenceDocumentationElement _CreateTypeReference(Type type)
        {
            if (type.IsGenericTypeParameter)
                return new TypeGenericParameterDocumentationElement();
            else if (type.IsGenericMethodParameter)
                return new MethodGenericParameterDocumentationElement();
            else if (type == typeof(void))
                return new VoidTypeReferenceDocumentationElement();
            else if (type.IsPointer)
                return new PointerTypeDocumentationElement();
            else if (type.IsArray)
                return new ArrayTypeDocumentationElement();
            else
                return new InstanceTypeDocumentationElement();
        }

        private void _InitializeTypeReference(Type type, TypeReferenceDocumentationElement typeReference)
        {
            switch (typeReference)
            {
                case GenericParameterDocumentationElement genericParameter:
                    var genericParameterAttributes = type.GenericParameterAttributes;
                    genericParameter.Name = _GetTypeNameFor(type);
                    genericParameter.IsCovariant =
                        (genericParameterAttributes & GenericParameterAttributes.Covariant) == GenericParameterAttributes.Covariant;
                    genericParameter.IsContravariant =
                        (genericParameterAttributes & GenericParameterAttributes.Contravariant) == GenericParameterAttributes.Contravariant;
                    genericParameter.HasNonNullableValueTypeConstraint =
                        (genericParameterAttributes & GenericParameterAttributes.NotNullableValueTypeConstraint) == GenericParameterAttributes.NotNullableValueTypeConstraint;
                    genericParameter.HasReferenceTypeConstraint =
                        (genericParameterAttributes & GenericParameterAttributes.ReferenceTypeConstraint) == GenericParameterAttributes.ReferenceTypeConstraint;
                    genericParameter.HasDefaultConstructorConstraint =
                        (genericParameterAttributes & (GenericParameterAttributes.DefaultConstructorConstraint | GenericParameterAttributes.NotNullableValueTypeConstraint)) == GenericParameterAttributes.DefaultConstructorConstraint;
                    genericParameter.TypeConstraints =
                        type
                            .GetGenericParameterConstraints()
                            .Where(genericParameterTypeConstraint => genericParameterTypeConstraint != typeof(ValueType))
                            .Select(_GetTypeReference)
                            .ToList();

                    var memberDocumentation = _GetMemberDocumentationFor(type.DeclaringType);
                    genericParameter.Description = (
                            memberDocumentation.GenericParameters.Contains(genericParameter.Name)
                            ? memberDocumentation.GenericParameters[genericParameter.Name]
                            : Enumerable.Empty<BlockDocumentationElement>()
                        )
                        .AsReadOnlyList();
                    break;

                case PointerTypeDocumentationElement pointerType:
                    pointerType.ReferentType = _GetTypeReference(type.GetElementType());
                    break;

                case ArrayTypeDocumentationElement arrayType:
                    arrayType.Rank = type.GetArrayRank();
                    arrayType.ItemType = _GetTypeReference(type.GetElementType());
                    break;

                case InstanceTypeDocumentationElement instanceType:
                    instanceType.Name = _GetTypeNameFor(type);
                    instanceType.DeclaringType = type.DeclaringType != null
                        ? (InstanceTypeDocumentationElement)_GetTypeReference(
                            type.DeclaringType.MakeGenericType(
                                type
                                    .GetGenericArguments()
                                    .Take(type.DeclaringType.GetGenericArguments().Length)
                                    .ToArray()
                            )
                        )
                        : null;
                    instanceType.Namespace = type.Namespace;
                    instanceType.GenericArguments = type
                        .GetGenericArguments()
                        .Select(_GetTypeReference)
                        .AsReadOnlyList();
                    instanceType.Assembly = _referencesCache.GetFor(type.Assembly.GetName(), _CreateAssemblyReference);
                    break;
            }
        }

        private ParameterDocumentationElement _CreateParameter(ParameterInfo parameter, MemberDocumentation memberDocumentation)
            => new ParameterDocumentationElement
            {
                Name = parameter.Name,
                Type = _UnwrapeByRef(parameter.ParameterType) == typeof(object) && parameter.GetCustomAttribute<DynamicAttribute>() != null
                    ? _dynamicTypeReference
                    : _GetTypeReference(parameter.ParameterType),
                Attributes = _MapAttributesDataFrom(parameter.CustomAttributes),
                IsInputByReference = parameter.ParameterType.IsByRef && parameter.IsIn,
                IsInputOutputByReference = parameter.ParameterType.IsByRef && !parameter.IsIn && !parameter.IsOut,
                IsOutputByReference = parameter.ParameterType.IsByRef && parameter.IsOut,
                HasDefaultValue = parameter.HasDefaultValue,
                DefaultValue = parameter.HasDefaultValue ? parameter.RawDefaultValue : null,
                Description = (
                        memberDocumentation.Parameters.Contains(parameter.Name)
                        ? memberDocumentation.Parameters[parameter.Name]
                        : Enumerable.Empty<BlockDocumentationElement>()
                    )
                    .AsReadOnlyList()
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
                                    Value = _GetAttributeValue(parameter.ParameterType, argument)
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
                                    Value = _GetAttributeValue(parameterType, namedArgument.TypedValue)
                                };
                            }
                        )
                        .ToList()
            )
        )
        .ToList();

        private static object _GetAttributeValue(Type type, CustomAttributeTypedArgument argument)
        {
            if (argument.Value == null)
                return null;
            if (argument.ArgumentType.IsArray)
                return _GetAttributeValues(type, (IEnumerable<CustomAttributeTypedArgument>)argument.Value);
            if (type.IsEnum)
                return Enum.Parse(type, Convert.ToString(argument.Value));

            return argument.Value;
        }

        private static object _GetAttributeValues(Type attributeParameterType, IEnumerable<CustomAttributeTypedArgument> values)
        {
            var valuesCollection = values.AsReadOnlyList();
            var result = Array.CreateInstance(
                attributeParameterType.IsArray ? attributeParameterType.GetElementType() : attributeParameterType,
                valuesCollection.Count
            );

            for (var index = 0; index < valuesCollection.Count; index++)
                result.SetValue(_GetAttributeValue(attributeParameterType, valuesCollection[index]), index);

            return result;
        }

        private IReadOnlyCollection<ExceptionDocumentationElement> _MapExceptions(ILookup<string, BlockDocumentationElement> exceptions)
            => (
                from exception in exceptions
                let exceptionType = _canonicalNameResolver.TryFindMemberInfoFor(exception.Key) as Type
                where exceptionType != null && typeof(Exception).IsAssignableFrom(exceptionType)
                select new ExceptionDocumentationElement
                {
                    Type = _GetTypeReference(exceptionType),
                    Description = exception.AsReadOnlyList()
                }
        ).ToList();

        private MemberDocumentation _GetMemberDocumentationFor(MemberInfo memberInfo)
            => _membersDocumentation.TryFind(_canonicalNameResolver.GetCanonicalNameFrom(memberInfo), out var memberDocumentation) ? memberDocumentation : _emptyMemberDocumentation;
    }
}