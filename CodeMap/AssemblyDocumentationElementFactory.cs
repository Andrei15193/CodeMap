using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CodeMap.Elements;

namespace CodeMap
{
    /// <summary>
    /// Represents an <see cref="AssemblyDocumentationElement"/> factory for creating <see cref="DocumentationElement"/>s
    /// from all reflection related objects and merging them with documentation related elements provided by a
    /// <see cref="MemberDocumentationCollection"/>.
    /// </summary>
    public class AssemblyDocumentationElementFactory
    {
        private readonly MemberDocumentationCollection _membersDocumentation;

        /// <summary>Initializes a new instance of the <see cref="AssemblyDocumentationElementFactory"/> class.</summary>
        public AssemblyDocumentationElementFactory()
            : this(new MemberDocumentationCollection(Enumerable.Empty<MemberDocumentation>()))
        {
        }

        /// <summary>Initializes a new instance of the <see cref="AssemblyDocumentationElementFactory"/> class.</summary>
        /// <param name="membersDocumentation">A collection of <see cref="MemberDocumentation"/> to associate to created <see cref="DocumentationElement"/>s.</param>
        public AssemblyDocumentationElementFactory(MemberDocumentationCollection membersDocumentation)
        {
            _membersDocumentation = membersDocumentation
                ?? throw new ArgumentNullException(nameof(membersDocumentation));
        }

        /// <summary>Creates an <see cref="AssemblyDocumentationElement"/> from the provided <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="Assembly"/> from which to create a <see cref="AssemblyDocumentationElement"/>.</param>
        /// <returns>Returns an <see cref="AssemblyDocumentationElement"/> from the provided <paramref name="assembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is <c>null</c>.</exception>
        public AssemblyDocumentationElement Create(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            return new DocumentationElementFactory(
                    new CanonicalNameResolver(new[] { assembly }.Concat(assembly.GetReferencedAssemblies().Select(Assembly.Load))),
                    _membersDocumentation
                )
                .Create(assembly);
        }

        /// <summary>Creates a <see cref="TypeDocumentationElement"/> from the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> from which to create a <see cref="TypeDocumentationElement"/>.</param>
        /// <returns>Returns a <see cref="TypeDocumentationElement"/> from the provided <paramref name="type"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is <c>null</c>.</exception>
        /// <remarks>
        /// This method creates the entire <see cref="AssemblyDocumentationElement"/> and returns the <see cref="TypeDocumentationElement"/>
        /// for the provided <paramref name="type"/>.
        /// </remarks>
        public TypeDocumentationElement Create(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return Create(type.Assembly)
                .Namespaces
                .Where(@namespace => string.Equals(@namespace.Name, type.Namespace, StringComparison.OrdinalIgnoreCase))
                .SelectMany(
                    @namespace => @namespace
                        .Enums
                        .AsEnumerable<TypeDocumentationElement>()
                        .Concat(@namespace.Delegates)
                        .Concat(@namespace.Interfaces)
                        .Concat(@namespace.Classes.SelectMany(_GetWithNested))
                        .Concat(@namespace.Structs.SelectMany(_GetWithNested))
                )
                .FirstOrDefault(typeDocumentationElement => typeDocumentationElement == type);
        }

        private static IEnumerable<TypeDocumentationElement> _GetWithNested(ClassDocumentationElement classDocumentationElement)
            => new[] { classDocumentationElement }
                .Concat(classDocumentationElement
                    .NestedEnums
                    .AsEnumerable<TypeDocumentationElement>()
                    .Concat(classDocumentationElement.NestedDelegates)
                    .Concat(classDocumentationElement.NestedInterfaces)
                    .Concat(classDocumentationElement.NestedClasses.SelectMany(_GetWithNested))
                    .Concat(classDocumentationElement.NestedStructs.SelectMany(_GetWithNested))
                );

        private static IEnumerable<TypeDocumentationElement> _GetWithNested(StructDocumentationElement structDocumentationElement)
            => new[] { structDocumentationElement }
                .Concat(structDocumentationElement
                    .NestedEnums
                    .AsEnumerable<TypeDocumentationElement>()
                    .Concat(structDocumentationElement.NestedDelegates)
                    .Concat(structDocumentationElement.NestedInterfaces)
                    .Concat(structDocumentationElement.NestedClasses.SelectMany(_GetWithNested))
                    .Concat(structDocumentationElement.NestedStructs.SelectMany(_GetWithNested))
                );

        private class DocumentationElementFactory
        {
            private readonly DynamicTypeData _dynamicTypeData = new DynamicTypeData();
            private readonly DocumentationElementCache _referencesCache = new DocumentationElementCache();
            private readonly MemberDocumentation _emptyMemberDocumentation = new MemberDocumentation(string.Empty);
            private readonly CanonicalNameResolver _canonicalNameResolver;
            private readonly MemberDocumentationCollection _membersDocumentation;

            public DocumentationElementFactory(CanonicalNameResolver canonicalNameResolver, MemberDocumentationCollection membersDocumentation)
            {
                _canonicalNameResolver = canonicalNameResolver;
                _membersDocumentation = membersDocumentation;
            }

            public AssemblyDocumentationElement Create(Assembly assembly)
            {
                var assemblyName = assembly.GetName();
                var assemblyDocumentationElement = new AssemblyDocumentationElement
                {
                    Name = assemblyName.Name,
                    Version = assemblyName.Version,
                    Culture = assemblyName.CultureName,
                    PublicKeyToken = assemblyName.GetPublicKeyToken().ToBase16String(),
                    Dependencies = assembly
                        .GetReferencedAssemblies()
                        .Select(dependency => _referencesCache.GetFor(dependency, _CreateAssemblyReference))
                        .AsReadOnlyCollection(),
                    Attributes = _MapAttributesDataFrom(assembly.CustomAttributes),
                    Summary = DocumentationElement.Summary(),
                    Remarks = DocumentationElement.Remarks(),
                    Examples = Enumerable.Empty<ExampleDocumentationElement>().AsReadOnlyList(),
                    RelatedMembers = Enumerable.Empty<MemberReferenceDocumentationElement>().AsReadOnlyList()
                };

                assemblyDocumentationElement.Namespaces = assembly
                    .DefinedTypes
                    .Where(type => type.DeclaringType == null)
                    .GroupBy(type => type.Namespace, StringComparer.OrdinalIgnoreCase)
                    .Select(
                        typesByNamespace =>
                        {
                            var @namespace = string.IsNullOrWhiteSpace(typesByNamespace.Key)
                                ? new GlobalNamespaceDocumentationElement
                                {
                                    Name = string.Empty
                                }
                                : new NamespaceDocumentationElement
                                {
                                    Name = typesByNamespace.Key,
                                };
                            @namespace.Assembly = assemblyDocumentationElement;
                            @namespace.Summary = DocumentationElement.Summary();
                            @namespace.Remarks = DocumentationElement.Remarks();
                            @namespace.Examples = Enumerable.Empty<ExampleDocumentationElement>().AsReadOnlyList();
                            @namespace.RelatedMembers = Enumerable.Empty<MemberReferenceDocumentationElement>().AsReadOnlyList();

                            var types = _Create(typesByNamespace, @namespace, null);
                            @namespace.Enums = types[typeof(EnumDocumentationElement)]
                                .Cast<EnumDocumentationElement>()
                                .AsReadOnlyCollection();
                            @namespace.Delegates = types[typeof(DelegateDocumentationElement)]
                                .Cast<DelegateDocumentationElement>()
                                .AsReadOnlyCollection();
                            @namespace.Interfaces = types[typeof(InterfaceDocumentationElement)]
                                .Cast<InterfaceDocumentationElement>()
                                .AsReadOnlyCollection();
                            @namespace.Classes = types[typeof(ClassDocumentationElement)]
                                .Cast<ClassDocumentationElement>()
                                .AsReadOnlyCollection();
                            @namespace.Structs = types[typeof(StructDocumentationElement)]
                                .Cast<StructDocumentationElement>()
                                .AsReadOnlyCollection();

                            return @namespace;
                        }
                    )
                    .AsReadOnlyCollection();

                return assemblyDocumentationElement;
            }

            private TypeDocumentationElement _Create(Type type, NamespaceDocumentationElement @namespace, TypeDocumentationElement declaringType)
            {
                TypeDocumentationElement typeDocumentationElement;
                if (type.IsEnum)
                    typeDocumentationElement = _CreateEnum(type);
                else if (typeof(Delegate).IsAssignableFrom(type))
                    typeDocumentationElement = _CreateDelegate(type);
                else if (type.IsInterface)
                    typeDocumentationElement = _CreateInterface(type);
                else if (type.IsClass)
                    typeDocumentationElement = _CreateClass(type, @namespace);
                else if (type.IsValueType)
                    typeDocumentationElement = _CreateStruct(type, @namespace);
                else
                    throw new ArgumentException($"Unknown type: '{type.Name}'.", nameof(type));

                typeDocumentationElement.Namespace = @namespace;
                typeDocumentationElement.DeclaringType = declaringType;
                return typeDocumentationElement;
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
                    .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField | BindingFlags.DeclaredOnly)
                    .Select(
                        field => _GetConstant(field, enumDocumentationElement)
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
                            ? _dynamicTypeData
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

                delegateDocumentationElement.GenericParameters = _MapTypeGenericParameters(delegateType, delegateDocumentationElement, memberDocumentation);

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

                interfaceDocumentationElement.GenericParameters = _MapTypeGenericParameters(interfaceType, interfaceDocumentationElement, memberDocumentation);
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

            private ClassDocumentationElement _CreateClass(Type classType, NamespaceDocumentationElement @namespace)
            {
                var memberDocumentation = _GetMemberDocumentationFor(classType);
                var classDocumentationElement = new ClassDocumentationElement
                {
                    Name = _GetTypeNameFor(classType),
                    AccessModifier = _GetAccessModifierFrom(classType),
                    Attributes = _MapAttributesDataFrom(classType.CustomAttributes),
                    IsAbstract = classType.IsAbstract && !classType.IsSealed,
                    IsSealed = !classType.IsAbstract && classType.IsSealed,
                    IsStatic = classType.IsAbstract && classType.IsSealed,
                    BaseClass = _GetTypeReference(classType.BaseType),
                    ImplementedInterfaces = classType
                        .GetInterfaces()
                        .Except(
                            classType
                                .BaseType
                                .GetInterfaces()
                        )
                        .Except(
                            classType
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

                classDocumentationElement.GenericParameters = _MapTypeGenericParameters(classType, classDocumentationElement, memberDocumentation);
                classDocumentationElement.Constants = classType
                    .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField | BindingFlags.DeclaredOnly)
                    .Where(field => !field.IsSpecialName && field.IsLiteral)
                    .Select(field => _GetConstant(field, classDocumentationElement))
                    .AsReadOnlyList();
                classDocumentationElement.Fields = classType
                    .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.DeclaredOnly)
                    .Where(field => !field.IsLiteral && field.GetCustomAttribute<CompilerGeneratedAttribute>() == null)
                    .Select(field => _GetField(field, classDocumentationElement))
                    .AsReadOnlyList();
                classDocumentationElement.Constructors = classType
                    .GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Select(constructor => _GetConstructor(constructor, classDocumentationElement))
                    .AsReadOnlyList();
                classDocumentationElement.Events = classType
                    .GetEvents(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Select(@event => _GetEvent(@event, classDocumentationElement))
                    .AsReadOnlyCollection();
                classDocumentationElement.Properties = classType
                    .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Select(property => _GetProperty(property, classDocumentationElement))
                    .AsReadOnlyCollection();
                classDocumentationElement.Methods = classType
                    .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(method => !method.IsSpecialName)
                    .Select(method => _GetMethod(method, classDocumentationElement))
                    .AsReadOnlyCollection();

                var nestedTypes = _Create(classType.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic), @namespace, classDocumentationElement);
                classDocumentationElement.NestedEnums = nestedTypes[typeof(EnumDocumentationElement)]
                    .Cast<EnumDocumentationElement>()
                    .AsReadOnlyCollection();
                classDocumentationElement.NestedDelegates = nestedTypes[typeof(DelegateDocumentationElement)]
                    .Cast<DelegateDocumentationElement>()
                    .AsReadOnlyCollection();
                classDocumentationElement.NestedInterfaces = nestedTypes[typeof(InterfaceDocumentationElement)]
                    .Cast<InterfaceDocumentationElement>()
                    .AsReadOnlyCollection();
                classDocumentationElement.NestedClasses = nestedTypes[typeof(ClassDocumentationElement)]
                    .Cast<ClassDocumentationElement>()
                    .AsReadOnlyCollection();
                classDocumentationElement.NestedStructs = nestedTypes[typeof(StructDocumentationElement)]
                    .Cast<StructDocumentationElement>()
                    .AsReadOnlyCollection();

                return classDocumentationElement;
            }

            private TypeDocumentationElement _CreateStruct(Type structType, NamespaceDocumentationElement @namespace)
            {
                var memberDocumentation = _GetMemberDocumentationFor(structType);
                var structDocumentationElement = new StructDocumentationElement
                {
                    Name = _GetTypeNameFor(structType),
                    AccessModifier = _GetAccessModifierFrom(structType),
                    Attributes = _MapAttributesDataFrom(structType.CustomAttributes),
                    ImplementedInterfaces = structType
                        .GetInterfaces()
                        .Except(
                            structType
                                .BaseType
                                .GetInterfaces()
                        )
                        .Except(
                            structType
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

                structDocumentationElement.GenericParameters = _MapTypeGenericParameters(structType, structDocumentationElement, memberDocumentation);
                structDocumentationElement.Constants = structType
                    .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField | BindingFlags.DeclaredOnly)
                    .Where(field => !field.IsSpecialName && field.IsLiteral)
                    .Select(field => _GetConstant(field, structDocumentationElement))
                    .AsReadOnlyList();
                structDocumentationElement.Fields = structType
                    .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.DeclaredOnly)
                    .Where(field => !field.IsLiteral && field.GetCustomAttribute<CompilerGeneratedAttribute>() == null)
                    .Select(field => _GetField(field, structDocumentationElement))
                    .AsReadOnlyList();
                structDocumentationElement.Constructors =
                    new[] { _GetDefaultConstructor(structType, structDocumentationElement) }
                    .Concat(
                        structType
                            .GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                            .Select(constructor => _GetConstructor(constructor, structDocumentationElement))
                    )
                    .AsReadOnlyList();
                structDocumentationElement.Events = structType
                    .GetEvents(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Select(@event => _GetEvent(@event, structDocumentationElement))
                    .AsReadOnlyCollection();
                structDocumentationElement.Properties = structType
                    .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Select(property => _GetProperty(property, structDocumentationElement))
                    .AsReadOnlyCollection();
                structDocumentationElement.Methods = structType
                    .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(method => !method.IsSpecialName)
                    .Select(method => _GetMethod(method, structDocumentationElement))
                    .AsReadOnlyCollection();

                var nestedTypes = _Create(structType.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic), @namespace, structDocumentationElement);
                structDocumentationElement.NestedEnums = nestedTypes[typeof(EnumDocumentationElement)]
                    .Cast<EnumDocumentationElement>()
                    .AsReadOnlyCollection();
                structDocumentationElement.NestedDelegates = nestedTypes[typeof(DelegateDocumentationElement)]
                    .Cast<DelegateDocumentationElement>()
                    .AsReadOnlyCollection();
                structDocumentationElement.NestedInterfaces = nestedTypes[typeof(InterfaceDocumentationElement)]
                    .Cast<InterfaceDocumentationElement>()
                    .AsReadOnlyCollection();
                structDocumentationElement.NestedClasses = nestedTypes[typeof(ClassDocumentationElement)]
                    .Cast<ClassDocumentationElement>()
                    .AsReadOnlyCollection();
                structDocumentationElement.NestedStructs = nestedTypes[typeof(StructDocumentationElement)]
                    .Cast<StructDocumentationElement>()
                    .AsReadOnlyCollection();

                return structDocumentationElement;
            }

            private IReadOnlyDictionary<Type, IEnumerable<TypeDocumentationElement>> _Create(IEnumerable<Type> types, NamespaceDocumentationElement @namespace, TypeDocumentationElement declaringType)
                => (
                    from elementType in new[]
                        {
                            typeof(EnumDocumentationElement),
                            typeof(DelegateDocumentationElement),
                            typeof(InterfaceDocumentationElement),
                            typeof(ClassDocumentationElement),
                            typeof(StructDocumentationElement)
                        }
                    join type in types.Select(type => _Create(type, @namespace, declaringType))
                        on elementType equals type.GetType() into documentationElementsByType
                    select new
                    {
                        ElementType = elementType,
                        TypeDocumentationElements = documentationElementsByType
                    }
                )
                .ToDictionary(pair => pair.ElementType, pair => pair.TypeDocumentationElements);

            private ConstantDocumentationElement _GetConstant(FieldInfo field, TypeDocumentationElement declaringType)
            {
                var memberDocumentation = _GetMemberDocumentationFor(field);
                return new ConstantDocumentationElement
                {
                    Name = field.Name,
                    AccessModifier = _GetAccessModifierFrom(field),
                    Value = field.GetValue(null),
                    Type = field.FieldType == typeof(object) && field.GetCustomAttribute<DynamicAttribute>() != null
                        ? _dynamicTypeData
                        : _GetTypeReference(field.FieldType),
                    Attributes = _MapAttributesDataFrom(field.CustomAttributes),
                    DeclaringType = declaringType,
                    Summary = memberDocumentation.Summary,
                    Remarks = memberDocumentation.Remarks,
                    Examples = memberDocumentation.Examples,
                    RelatedMembers = memberDocumentation.RelatedMembers
                };
            }

            private FieldDocumentationElement _GetField(FieldInfo field, TypeDocumentationElement declaringType)
            {
                var memberDocumentation = _GetMemberDocumentationFor(field);
                return new FieldDocumentationElement
                {
                    Name = field.Name,
                    AccessModifier = _GetAccessModifierFrom(field),
                    IsReadOnly = field.IsInitOnly,
                    IsStatic = field.IsStatic,
                    IsShadowing = _IsShadowing(field),
                    Type = field.FieldType == typeof(object) && field.GetCustomAttribute<DynamicAttribute>() != null
                        ? _dynamicTypeData
                        : _GetTypeReference(field.FieldType),
                    Attributes = _MapAttributesDataFrom(field.CustomAttributes),
                    DeclaringType = declaringType,
                    Summary = memberDocumentation.Summary,
                    Remarks = memberDocumentation.Remarks,
                    Examples = memberDocumentation.Examples,
                    RelatedMembers = memberDocumentation.RelatedMembers
                };
            }

            private ConstructorDocumentationElement _GetConstructor(ConstructorInfo constructor, TypeDocumentationElement declaringType)
            {
                var memberDocumentation = _GetMemberDocumentationFor(constructor);

                return new ConstructorDocumentationElement
                {
                    Name = declaringType.Name,
                    AccessModifier = _GetAccessModifierFrom(constructor),
                    Attributes = _MapAttributesDataFrom(constructor.CustomAttributes),
                    DeclaringType = declaringType,
                    Parameters = constructor
                        .GetParameters()
                        .Select(parameter => _CreateParameter(parameter, memberDocumentation))
                        .AsReadOnlyList(),
                    Summary = memberDocumentation.Summary,
                    Exceptions = _MapExceptions(memberDocumentation.Exceptions),
                    Remarks = memberDocumentation.Remarks,
                    Examples = memberDocumentation.Examples,
                    RelatedMembers = memberDocumentation.RelatedMembers
                };
            }

            private ConstructorDocumentationElement _GetDefaultConstructor(Type type, TypeDocumentationElement declaringType)
            {
                var memberDocumentation = _GetDefaultConstructorMemberDocumentationFor(type);

                return new ConstructorDocumentationElement
                {
                    Name = declaringType.Name,
                    AccessModifier = AccessModifier.Public,
                    Attributes = Enumerable.Empty<AttributeData>().AsReadOnlyCollection(),
                    DeclaringType = declaringType,
                    Parameters = Enumerable
                        .Empty<ParameterDocumentationElement>()
                        .AsReadOnlyList(),
                    Summary = memberDocumentation.Summary,
                    Exceptions = _MapExceptions(memberDocumentation.Exceptions),
                    Remarks = memberDocumentation.Remarks,
                    Examples = memberDocumentation.Examples,
                    RelatedMembers = memberDocumentation.RelatedMembers
                };
            }

            private ConstructorDocumentationElement _GetDefaultConstructor(ConstructorInfo constructor, TypeDocumentationElement declaringType)
            {

                var memberDocumentation = _GetMemberDocumentationFor(constructor);

                return new ConstructorDocumentationElement
                {
                    Name = declaringType.Name,
                    AccessModifier = _GetAccessModifierFrom(constructor),
                    Attributes = _MapAttributesDataFrom(constructor.CustomAttributes),
                    DeclaringType = declaringType,
                    Parameters = constructor
                        .GetParameters()
                        .Select(parameter => _CreateParameter(parameter, memberDocumentation))
                        .AsReadOnlyList(),
                    Summary = memberDocumentation.Summary,
                    Exceptions = _MapExceptions(memberDocumentation.Exceptions),
                    Remarks = memberDocumentation.Remarks,
                    Examples = memberDocumentation.Examples,
                    RelatedMembers = memberDocumentation.RelatedMembers
                };
            }

            private EventDocumentationElement _GetEvent(EventInfo @event, TypeDocumentationElement declaringType)
            {
                var memberDocumentation = _GetMemberDocumentationFor(@event);
                var methodInfo = (@event.AddMethod ?? @event.RemoveMethod ?? @event.RaiseMethod);
                return new EventDocumentationElement
                {
                    Name = @event.Name,
                    AccessModifier = _GetAccessModifierFrom(@event),
                    Type = _GetTypeReference(@event.EventHandlerType),
                    Attributes = _MapAttributesDataFrom(@event.CustomAttributes),
                    DeclaringType = declaringType,
                    IsStatic = methodInfo.IsStatic,
                    IsAbstract = methodInfo.IsAbstract && !@event.DeclaringType.IsInterface,
                    IsVirtual = !methodInfo.IsAbstract && methodInfo.IsVirtual && methodInfo.GetBaseDefinition() == methodInfo,
                    IsOverride = methodInfo.IsVirtual && methodInfo.GetBaseDefinition() != methodInfo,
                    IsSealed = methodInfo.IsFinal,
                    IsShadowing = (!methodInfo.IsVirtual || methodInfo.GetBaseDefinition() == methodInfo) && _IsShadowing(@event),
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
                var methodInfo = (property.GetMethod ?? property.SetMethod);
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
                    IsStatic = methodInfo.IsStatic,
                    IsAbstract = methodInfo.IsAbstract && !property.DeclaringType.IsInterface,
                    IsVirtual = !methodInfo.IsAbstract && methodInfo.IsVirtual && methodInfo.GetBaseDefinition() == methodInfo,
                    IsOverride = methodInfo.IsVirtual && methodInfo.GetBaseDefinition() != methodInfo,
                    IsSealed = methodInfo.IsFinal,
                    IsShadowing = (!methodInfo.IsVirtual || methodInfo.GetBaseDefinition() == methodInfo) && _IsShadowing(property),
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
                    IsStatic = method.IsStatic,
                    IsAbstract = method.IsAbstract && !method.DeclaringType.IsInterface,
                    IsVirtual = !method.IsAbstract && method.IsVirtual && method.GetBaseDefinition() == method,
                    IsOverride = method.IsVirtual && method.GetBaseDefinition() != method,
                    IsSealed = method.IsFinal,
                    IsShadowing = (!method.IsVirtual || method.GetBaseDefinition() == method) && _IsShadowing(method),
                    Parameters = method
                        .GetParameters()
                        .Select(parameter => _CreateParameter(parameter, memberDocumentation))
                        .AsReadOnlyList(),
                    Return = new ReturnsDocumentationElement
                    {
                        Type = method.ReturnType == typeof(object) && method.ReturnParameter.GetCustomAttribute<DynamicAttribute>() != null
                            ? _dynamicTypeData
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
                    .Select(typeGenericParameter => _GetMethodGenericParameter(typeGenericParameter, methodDocumentationElement, memberDocumentation))
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

            private TypeReferenceData _GetTypeReference(Type type)
                => _referencesCache.GetFor(_UnwrapeByRef(type), _CreateTypeReference, _InitializeTypeReference);

            private IReadOnlyList<TypeGenericParameterData> _MapTypeGenericParameters(Type type, TypeDocumentationElement declaringDocumentationElement, MemberDocumentation memberDocumentation)
                => type
                    .GetGenericArguments()
                    .Skip(
                        type
                        .DeclaringType
                        ?.GetGenericArguments()
                        .Length ?? 0
                    )
                    .Select(typeGenericParameter => _GetTypeGenericParameter(typeGenericParameter, declaringDocumentationElement, memberDocumentation))
                    .AsReadOnlyList();

            private TypeGenericParameterData _GetTypeGenericParameter(Type type, TypeDocumentationElement declaringType, MemberDocumentation memberDocumentation)
            {
                var typeGenericParameter = (TypeGenericParameterData)_GetTypeReference(type);
                typeGenericParameter.DeclaringType = declaringType;
                typeGenericParameter.Description = (
                    memberDocumentation.GenericParameters.Contains(typeGenericParameter.Name)
                        ? memberDocumentation.GenericParameters[typeGenericParameter.Name]
                        : Enumerable.Empty<BlockDocumentationElement>()
                    )
                    .AsReadOnlyList();
                return typeGenericParameter;
            }

            private MethodGenericParameterTypeData _GetMethodGenericParameter(Type type, MethodDocumentationElement declaringMethod, MemberDocumentation memberDocumentation)
            {
                var methodGenericParameter = (MethodGenericParameterTypeData)_GetTypeReference(type);
                methodGenericParameter.DeclaringMethod = declaringMethod;
                methodGenericParameter.Description = (
                    memberDocumentation.GenericParameters.Contains(methodGenericParameter.Name)
                        ? memberDocumentation.GenericParameters[methodGenericParameter.Name]
                        : Enumerable.Empty<BlockDocumentationElement>()
                    )
                    .AsReadOnlyList();
                return methodGenericParameter;
            }

            private static Type _UnwrapeByRef(Type type)
            {
                var referentType = type;
                while (referentType.IsByRef)
                    referentType = referentType.GetElementType();
                return referentType;
            }

            private TypeReferenceData _CreateTypeReference(Type type)
            {
                if (type.IsGenericTypeParameter)
                    return new TypeGenericParameterData();
                else if (type.IsGenericMethodParameter)
                    return new MethodGenericParameterTypeData();
                else if (type == typeof(void))
                    return new VoidTypeData();
                else if (type.IsPointer)
                    return new PointerTypeData();
                else if (type.IsArray)
                    return new ArrayTypeData();
                else
                    return new TypeData();
            }

            private void _InitializeTypeReference(Type type, TypeReferenceData typeReference)
            {
                switch (typeReference)
                {
                    case GenericParameterData genericParameter:
                        var genericParameterAttributes = type.GenericParameterAttributes;
                        genericParameter.Name = _GetTypeNameFor(type);
                        var genericParameterPositionOffset = type
                            .DeclaringType
                            .DeclaringType
                            ?.GetGenericArguments()
                            .Length ?? 0;
                        genericParameter.Position = type.GenericParameterPosition - genericParameterPositionOffset;
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

                    case PointerTypeData pointerType:
                        pointerType.ReferentType = _GetTypeReference(type.GetElementType());
                        break;

                    case ArrayTypeData arrayType:
                        arrayType.Rank = type.GetArrayRank();
                        arrayType.ItemType = _GetTypeReference(type.GetElementType());
                        break;

                    case TypeData instanceType:
                        instanceType.Name = _GetTypeNameFor(type);
                        instanceType.DeclaringType = type.DeclaringType != null
                            ? (TypeData)_GetTypeReference(
                                type.DeclaringType.IsGenericTypeDefinition
                                    ? type.DeclaringType.MakeGenericType(
                                        type
                                            .GetGenericArguments()
                                            .Take(type.DeclaringType.GetGenericArguments().Length)
                                            .ToArray()
                                    )
                                    : type.DeclaringType
                            )
                            : null;
                        instanceType.Namespace = type.Namespace;
                        instanceType.GenericArguments = type
                            .GetGenericArguments()
                            .Skip(
                                type
                                    .DeclaringType
                                    ?.GetGenericArguments()
                                    .Length ?? 0
                            )
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
                        ? _dynamicTypeData
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

            private AssemblyReference _CreateAssemblyReference(AssemblyName assemblyName)
                => new AssemblyReference
                {
                    Name = assemblyName.Name,
                    Version = assemblyName.Version,
                    Culture = assemblyName.CultureName,
                    PublicKeyToken = assemblyName.GetPublicKeyToken().ToBase16String()
                };

            private IReadOnlyCollection<AttributeData> _MapAttributesDataFrom(IEnumerable<CustomAttributeData> customAttributes)
                => (
                    from customAttribute in customAttributes
                    where customAttribute.AttributeType != typeof(CompilerGeneratedAttribute)
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

            private MemberDocumentation _GetDefaultConstructorMemberDocumentationFor(Type type)
                => _membersDocumentation.TryFind(_canonicalNameResolver.GetDefaultConstructorCanonicalNameFor(type), out var memberDocumentation) ? memberDocumentation : _emptyMemberDocumentation;
        }
    }
}