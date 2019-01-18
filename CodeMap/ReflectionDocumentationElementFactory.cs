using System;
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
        private readonly DynamicTypeReference _dynamicTypeReference = new DynamicTypeReference();
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
            return new DelegateDocumentationElement
            {
                Name = _GetTypeNameFor(delegateType),
                AccessModifier = _GetAccessModifierFrom(delegateType),
                Attributes = _MapAttributesDataFrom(delegateType.CustomAttributes),
                GenericParameters = delegateType
                    .GetGenericArguments()
                    .Select(_GetTypeReference)
                    .Cast<GenericParameterDocumentationElement>()
                    .AsReadOnlyList(),
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
        }

        private TypeDocumentationElement _CreateInterface(Type interfaceType)
        {
            var memberDocumentation = _GetMemberDocumentationFor(interfaceType);
            var interfaceDocumentationElement = new InterfaceDocumentationElement
            {
                Name = _GetTypeNameFor(interfaceType),
                AccessModifier = _GetAccessModifierFrom(interfaceType),
                Attributes = _MapAttributesDataFrom(interfaceType.CustomAttributes),
                GenericParameters = interfaceType
                    .GetGenericArguments()
                    .Select(_GetTypeReference)
                    .Cast<GenericParameterDocumentationElement>()
                    .AsReadOnlyList(),
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

            interfaceDocumentationElement.Events = interfaceType
                .GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(@event => _GetEvent(@event, interfaceDocumentationElement))
                .AsReadOnlyCollection();
            interfaceDocumentationElement.Properties = interfaceType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(property => _GetProperty(property, interfaceDocumentationElement))
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
                DeclaringType = declaringType,
                IsStatic = false,
                IsAbstract = false,
                IsVirtual = false,
                IsOverride = false,
                IsSealed = false,
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

        private TypeReferenceDocumentationElement _GetTypeReference(Type type)
        {
            var typeToReference = type;
            while (typeToReference.IsByRef)
                typeToReference = typeToReference.GetElementType();
            return _referencesCache.GetFor(typeToReference, _CreateTypeReference, _InitializeTypeReference);
        }

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

                case InstanceTypeDocumentationElement instanceType:
                    instanceType.Name = _GetTypeNameFor(type);
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
                Type = parameter.ParameterType == typeof(object) && parameter.GetCustomAttribute<DynamicAttribute>() != null
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