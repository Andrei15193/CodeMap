using CodeMap.DocumentationElements;
using CodeMap.ReferenceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CodeMap.DeclarationNodes
{
    internal class DeclarationNodeFactory
    {
        private readonly MemberReferenceFactory _memberReferenceFactory = new MemberReferenceFactory();
        private readonly MemberDocumentation _emptyMemberDocumentation = new MemberDocumentation(string.Empty);
        private readonly BlockDescriptionDocumentationElement _emptyBlockDocumentationElementCollection = DocumentationElement.BlockDescription(Enumerable.Empty<BlockDocumentationElement>());
        private readonly CanonicalNameResolver _canonicalNameResolver;
        private readonly MemberDocumentationCollection _membersDocumentation;

        public DeclarationNodeFactory(CanonicalNameResolver canonicalNameResolver, MemberDocumentationCollection membersDocumentation)
        {
            _canonicalNameResolver = canonicalNameResolver;
            _membersDocumentation = membersDocumentation;
        }

        public AssemblyDeclaration Create(Assembly assembly)
        {
            var assemblyName = assembly.GetName();
            var assemblyDocumentationElement = new AssemblyDeclaration
            {
                Name = assemblyName.Name,
                Version = assemblyName.Version,
                Culture = assemblyName.CultureName,
                PublicKeyToken = assemblyName.GetPublicKeyToken().ToBase16String(),
                Dependencies = assembly
                    .GetReferencedAssemblies()
                    .OrderBy(dependency => dependency.Name)
                    .ThenBy(dependency => dependency.Version)
                    .Select(dependency => _memberReferenceFactory.Create(dependency))
                    .AsReadOnlyCollection(),
                Attributes = _MapAttributesDataFrom(assembly.CustomAttributes),
                Summary = DocumentationElement.Summary(),
                Remarks = DocumentationElement.Remarks(),
                Examples = Enumerable.Empty<ExampleDocumentationElement>().AsReadOnlyList(),
                RelatedMembers = Enumerable.Empty<MemberReferenceDocumentationElement>().AsReadOnlyList()
            };

            assemblyDocumentationElement.Namespaces = assembly
                .DefinedTypes
                .Where(type => type.DeclaringType == null && !Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute)))
                .OrderBy(type => type.Namespace, StringComparer.OrdinalIgnoreCase)
                .GroupBy(type => type.Namespace, StringComparer.OrdinalIgnoreCase)
                .Select(
                    typesByNamespace =>
                    {
                        var @namespace = string.IsNullOrWhiteSpace(typesByNamespace.Key)
                            ? new GlobalNamespaceDeclaration
                            {
                                Name = string.Empty
                            }
                            : new NamespaceDeclaration
                            {
                                Name = typesByNamespace.Key,
                            };
                        @namespace.Assembly = assemblyDocumentationElement;
                        @namespace.Summary = DocumentationElement.Summary();
                        @namespace.Remarks = DocumentationElement.Remarks();
                        @namespace.Examples = Enumerable.Empty<ExampleDocumentationElement>().AsReadOnlyList();
                        @namespace.RelatedMembers = Enumerable.Empty<MemberReferenceDocumentationElement>().AsReadOnlyList();

                        var declaredTypes = _GetTypes(typesByNamespace, @namespace, null);
                        @namespace.DeclaredTypes = declaredTypes;

                        @namespace.Enums = declaredTypes.OfType<EnumDeclaration>().AsReadOnlyCollection();
                        @namespace.Delegates = declaredTypes.OfType<DelegateDeclaration>().AsReadOnlyCollection();
                        @namespace.Interfaces = declaredTypes.OfType<InterfaceDeclaration>().AsReadOnlyCollection();
                        @namespace.Classes = declaredTypes.OfType<ClassDeclaration>().AsReadOnlyCollection();
                        @namespace.Structs = declaredTypes.OfType<StructDeclaration>().AsReadOnlyCollection();

                        return @namespace;
                    }
                )
                .AsReadOnlyCollection();

            return assemblyDocumentationElement;
        }

        private TypeDeclaration _GetType(Type type, NamespaceDeclaration @namespace, TypeDeclaration declaringType)
        {
            TypeDeclaration typeDocumentationElement;
            if (type.IsEnum)
                typeDocumentationElement = _GetEnum(type);
            else if (typeof(Delegate).IsAssignableFrom(type))
                typeDocumentationElement = _GetDelegate(type);
            else if (type.IsInterface)
                typeDocumentationElement = _GetInterface(type);
            else if (type.IsClass)
                typeDocumentationElement = _GetClass(type, @namespace);
            else if (type.IsValueType)
                typeDocumentationElement = _GetStruct(type, @namespace);
            else
                throw new ArgumentException($"Unknown type: '{type.Name}'.", nameof(type));

            typeDocumentationElement.Namespace = @namespace;
            typeDocumentationElement.DeclaringType = declaringType;
            return typeDocumentationElement;
        }

        private IReadOnlyCollection<TypeDeclaration> _GetTypes(IEnumerable<Type> types, NamespaceDeclaration @namespace, TypeDeclaration declaringType)
        {
            return (
                from type in types
                orderby _GetTypeKindSortOrder(type), type.Name, type.GetGenericArguments().Length
                select _GetType(type, @namespace, declaringType)
            ).AsReadOnlyCollection();

            int _GetTypeKindSortOrder(Type type)
            {
                if (type.IsEnum)
                    return 0;
                else if (typeof(Delegate).IsAssignableFrom(type))
                    return 1;
                else if (type.IsInterface)
                    return 2;
                else if (type.IsClass)
                    return 3;
                else
                    return 4;
            }
        }

        private EnumDeclaration _GetEnum(Type enumType)
        {
            var memberDocumentation = _GetMemberDocumentationFor(enumType);
            var enumDocumentationElement = new EnumDeclaration
            {
                Name = _GetTypeNameFor(enumType),
                AccessModifier = _GetAccessModifierFrom(enumType),
                UnderlyingType = (TypeReference)_memberReferenceFactory.Create(enumType.GetEnumUnderlyingType()),
                Attributes = _MapAttributesDataFrom(enumType.CustomAttributes),
                Summary = memberDocumentation.Summary,
                Remarks = memberDocumentation.Remarks,
                Examples = memberDocumentation.Examples,
                RelatedMembers = memberDocumentation.RelatedMembers
            };
            enumDocumentationElement.Members = enumType
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField | BindingFlags.DeclaredOnly)
                .Select(constant => _GetConstant(constant, enumDocumentationElement))
                .OrderBy(constant => constant.Value)
                .AsReadOnlyList();

            return enumDocumentationElement;
        }

        private DelegateDeclaration _GetDelegate(Type delegateType)
        {
            var memberDocumentation = _GetMemberDocumentationFor(delegateType);
            var invokeMethodInfo = delegateType.GetMethod(nameof(Action.Invoke), BindingFlags.Public | BindingFlags.Instance);
            var delegateDocumentationElement = new DelegateDeclaration
            {
                Name = _GetTypeNameFor(delegateType),
                AccessModifier = _GetAccessModifierFrom(delegateType),
                Attributes = _MapAttributesDataFrom(delegateType.CustomAttributes),
                Parameters = invokeMethodInfo
                    .GetParameters()
                    .Select(parameter => _GetParameter(parameter, memberDocumentation))
                    .AsReadOnlyList(),
                Return = new MethodReturnData
                {
                    Type = invokeMethodInfo.ReturnType == typeof(object) && invokeMethodInfo.ReturnParameter.GetCustomAttribute<DynamicAttribute>() != null
                        ? _memberReferenceFactory.CreateDynamic()
                        : _memberReferenceFactory.Create(invokeMethodInfo.ReturnType),
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

        private TypeDeclaration _GetInterface(Type interfaceType)
        {
            var memberDocumentation = _GetMemberDocumentationFor(interfaceType);
            var interfaceDocumentationElement = new InterfaceDeclaration
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
                    .OrderBy(baseInterface => baseInterface.Namespace)
                    .ThenBy(baseInterface => baseInterface.Name)
                    .ThenBy(baseInterface => baseInterface.GetGenericArguments().Length)
                    .Select(baseInterface => (TypeReference)_memberReferenceFactory.Create(baseInterface))
                    .AsReadOnlyCollection(),
                Summary = memberDocumentation.Summary,
                Remarks = memberDocumentation.Remarks,
                Examples = memberDocumentation.Examples,
                RelatedMembers = memberDocumentation.RelatedMembers
            };

            interfaceDocumentationElement.GenericParameters = _MapTypeGenericParameters(interfaceType, interfaceDocumentationElement, memberDocumentation);
            interfaceDocumentationElement.Events = _GetEvents(interfaceType, interfaceDocumentationElement);
            interfaceDocumentationElement.Properties = _GetProperties(interfaceType, interfaceDocumentationElement);
            interfaceDocumentationElement.Methods = _GetMethods(interfaceType, interfaceDocumentationElement);
            interfaceDocumentationElement.Members = interfaceDocumentationElement
                .Events
                .AsEnumerable<MemberDeclaration>()
                .Concat(interfaceDocumentationElement.Properties)
                .Concat(interfaceDocumentationElement.Methods)
                .AsReadOnlyCollection();

            return interfaceDocumentationElement;
        }

        private ClassDeclaration _GetClass(Type classType, NamespaceDeclaration @namespace)
        {
            var memberDocumentation = _GetMemberDocumentationFor(classType);
            var classDocumentationElement = new ClassDeclaration
            {
                Name = _GetTypeNameFor(classType),
                AccessModifier = _GetAccessModifierFrom(classType),
                Attributes = _MapAttributesDataFrom(classType.CustomAttributes),
                IsAbstract = classType.IsAbstract && !classType.IsSealed,
                IsSealed = !classType.IsAbstract && classType.IsSealed,
                IsStatic = classType.IsAbstract && classType.IsSealed,
                BaseClass = (TypeReference)_memberReferenceFactory.Create(classType.BaseType),
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
                    .OrderBy(implementedInterface => implementedInterface.Namespace)
                    .ThenBy(implementedInterface => implementedInterface.Name)
                    .ThenBy(implementedInterface => implementedInterface.GetGenericArguments().Length)
                    .Select(implementedInterface => (TypeReference)_memberReferenceFactory.Create(implementedInterface))
                    .AsReadOnlyCollection(),
                Summary = memberDocumentation.Summary,
                Remarks = memberDocumentation.Remarks,
                Examples = memberDocumentation.Examples,
                RelatedMembers = memberDocumentation.RelatedMembers
            };

            classDocumentationElement.GenericParameters = _MapTypeGenericParameters(classType, classDocumentationElement, memberDocumentation);
            classDocumentationElement.Constants = _GetConstants(classType, classDocumentationElement);
            classDocumentationElement.Fields = _GetFields(classType, classDocumentationElement);
            classDocumentationElement.Constructors = _GetConstructors(classType, classDocumentationElement);
            classDocumentationElement.Events = _GetEvents(classType, classDocumentationElement);
            classDocumentationElement.Properties = _GetProperties(classType, classDocumentationElement);
            classDocumentationElement.Methods = _GetMethods(classType, classDocumentationElement);
            classDocumentationElement.Members = classDocumentationElement
                .Constants
                .AsEnumerable<MemberDeclaration>()
                .Concat(classDocumentationElement.Fields)
                .Concat(classDocumentationElement.Constructors)
                .Concat(classDocumentationElement.Events)
                .Concat(classDocumentationElement.Properties)
                .Concat(classDocumentationElement.Methods)
                .AsReadOnlyCollection();

            var nestedTypes = _GetTypes(classType.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic), @namespace, classDocumentationElement);
            classDocumentationElement.NestedTypes = nestedTypes;

            classDocumentationElement.NestedEnums = nestedTypes.OfType<EnumDeclaration>().AsReadOnlyCollection();
            classDocumentationElement.NestedDelegates = nestedTypes.OfType<DelegateDeclaration>().AsReadOnlyCollection();
            classDocumentationElement.NestedInterfaces = nestedTypes.OfType<InterfaceDeclaration>().AsReadOnlyCollection();
            classDocumentationElement.NestedClasses = nestedTypes.OfType<ClassDeclaration>().AsReadOnlyCollection();
            classDocumentationElement.NestedStructs = nestedTypes.OfType<StructDeclaration>().AsReadOnlyCollection();

            return classDocumentationElement;
        }

        private TypeDeclaration _GetStruct(Type structType, NamespaceDeclaration @namespace)
        {
            var memberDocumentation = _GetMemberDocumentationFor(structType);
            var structDocumentationElement = new StructDeclaration
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
                    .OrderBy(implementedInterface => implementedInterface.Namespace)
                    .ThenBy(implementedInterface => implementedInterface.Name)
                    .ThenBy(implementedInterface => implementedInterface.GetGenericArguments().Length)
                    .Select(implementedInterface => (TypeReference)_memberReferenceFactory.Create(implementedInterface))
                    .AsReadOnlyCollection(),
                Summary = memberDocumentation.Summary,
                Remarks = memberDocumentation.Remarks,
                Examples = memberDocumentation.Examples,
                RelatedMembers = memberDocumentation.RelatedMembers
            };

            structDocumentationElement.GenericParameters = _MapTypeGenericParameters(structType, structDocumentationElement, memberDocumentation);
            structDocumentationElement.Constants = _GetConstants(structType, structDocumentationElement);
            structDocumentationElement.Fields = _GetFields(structType, structDocumentationElement);
            structDocumentationElement.Constructors =
                new[] { _GetDefaultConstructor(structType, structDocumentationElement) }
                .Concat(_GetConstructors(structType, structDocumentationElement))
                .AsReadOnlyCollection();
            structDocumentationElement.Events = _GetEvents(structType, structDocumentationElement);
            structDocumentationElement.Properties = _GetProperties(structType, structDocumentationElement);
            structDocumentationElement.Methods = _GetMethods(structType, structDocumentationElement);
            structDocumentationElement.Members = structDocumentationElement
                .Constants
                .AsEnumerable<MemberDeclaration>()
                .Concat(structDocumentationElement.Fields)
                .Concat(structDocumentationElement.Constructors)
                .Concat(structDocumentationElement.Events)
                .Concat(structDocumentationElement.Properties)
                .Concat(structDocumentationElement.Methods)
                .AsReadOnlyCollection();

            var nestedTypes = _GetTypes(structType.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic), @namespace, structDocumentationElement);
            structDocumentationElement.NestedTypes = nestedTypes;

            structDocumentationElement.NestedEnums = structDocumentationElement.NestedTypes.OfType<EnumDeclaration>().AsReadOnlyCollection();
            structDocumentationElement.NestedDelegates = nestedTypes.OfType<DelegateDeclaration>().AsReadOnlyCollection();
            structDocumentationElement.NestedInterfaces = nestedTypes.OfType<InterfaceDeclaration>().AsReadOnlyCollection();
            structDocumentationElement.NestedClasses = nestedTypes.OfType<ClassDeclaration>().AsReadOnlyCollection();
            structDocumentationElement.NestedStructs = nestedTypes.OfType<StructDeclaration>().AsReadOnlyCollection();

            return structDocumentationElement;
        }

        private IReadOnlyCollection<ConstantDeclaration> _GetConstants(Type declaringType, TypeDeclaration declaringDocumentationElement)
            => declaringType
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField | BindingFlags.DeclaredOnly)
                .Where(field => !field.IsSpecialName && field.IsLiteral)
                .OrderBy(constant => constant.Name)
                .Select(constant => _GetConstant(constant, declaringDocumentationElement))
                .AsReadOnlyCollection();

        private IReadOnlyCollection<FieldDeclaration> _GetFields(Type declaringType, TypeDeclaration declaringDocumentationElement)
            => declaringType
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.DeclaredOnly)
                .Where(field => !field.IsLiteral && field.GetCustomAttribute<CompilerGeneratedAttribute>() == null)
                .OrderBy(field => field.Name)
                .Select(field => _GetField(field, declaringDocumentationElement))
                .AsReadOnlyCollection();

        private IReadOnlyCollection<ConstructorDeclaration> _GetConstructors(Type declaringType, TypeDeclaration declaringDocumentationElement)
            => declaringType
                .GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .OrderBy(constructor => constructor.GetParameters().Length)
                .Select(constructor => _GetConstructor(constructor, declaringDocumentationElement))
                .AsReadOnlyCollection();

        private IReadOnlyCollection<EventDeclaration> _GetEvents(Type declaringType, TypeDeclaration declaringDocumentationElement)
            => declaringType
                .GetEvents(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .OrderBy(@event => @event.Name)
                .Select(@event => _GetEvent(@event, declaringDocumentationElement))
                .AsReadOnlyCollection();

        private IReadOnlyCollection<PropertyDeclaration> _GetProperties(Type declaringType, TypeDeclaration declaringDocumentationElement)
            => declaringType
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .OrderBy(property => property.Name)
                .ThenBy(property => property.GetIndexParameters().Length)
                .Select(property => _GetProperty(property, declaringDocumentationElement))
                .AsReadOnlyCollection();

        private IReadOnlyCollection<MethodDeclaration> _GetMethods(Type declaringType, TypeDeclaration declaringDocumentationElement)
            => declaringType
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .OrderBy(method => method.Name)
                .ThenBy(method => method.GetParameters().Length)
                .ThenBy(method => method.GetGenericArguments().Length)
                .Where(method => !method.IsSpecialName && !Attribute.IsDefined(method, typeof(CompilerGeneratedAttribute)))
                .Select(method => _GetMethod(method, declaringDocumentationElement))
                .AsReadOnlyCollection();

        private ConstantDeclaration _GetConstant(FieldInfo field, TypeDeclaration declaringType)
        {
            var memberDocumentation = _GetMemberDocumentationFor(field);
            return new ConstantDeclaration
            {
                Name = field.Name,
                AccessModifier = _GetAccessModifierFrom(field),
                Value = field.GetValue(null),
                Type = field.FieldType == typeof(object) && field.GetCustomAttribute<DynamicAttribute>() != null
                    ? _memberReferenceFactory.CreateDynamic()
                    : _memberReferenceFactory.Create(field.FieldType),
                Attributes = _MapAttributesDataFrom(field.CustomAttributes),
                DeclaringType = declaringType,
                Summary = memberDocumentation.Summary,
                Remarks = memberDocumentation.Remarks,
                Examples = memberDocumentation.Examples,
                RelatedMembers = memberDocumentation.RelatedMembers
            };
        }

        private FieldDeclaration _GetField(FieldInfo field, TypeDeclaration declaringType)
        {
            var memberDocumentation = _GetMemberDocumentationFor(field);
            return new FieldDeclaration
            {
                Name = field.Name,
                AccessModifier = _GetAccessModifierFrom(field),
                IsReadOnly = field.IsInitOnly,
                IsStatic = field.IsStatic,
                IsShadowing = _IsShadowing(field),
                Type = field.FieldType == typeof(object) && field.GetCustomAttribute<DynamicAttribute>() != null
                    ? _memberReferenceFactory.CreateDynamic()
                    : _memberReferenceFactory.Create(field.FieldType),
                Attributes = _MapAttributesDataFrom(field.CustomAttributes),
                DeclaringType = declaringType,
                Summary = memberDocumentation.Summary,
                Remarks = memberDocumentation.Remarks,
                Examples = memberDocumentation.Examples,
                RelatedMembers = memberDocumentation.RelatedMembers
            };
        }

        private ConstructorDeclaration _GetConstructor(ConstructorInfo constructor, TypeDeclaration declaringType)
        {
            var memberDocumentation = _GetMemberDocumentationFor(constructor);

            return new ConstructorDeclaration
            {
                Name = declaringType.Name,
                AccessModifier = _GetAccessModifierFrom(constructor),
                Attributes = _MapAttributesDataFrom(constructor.CustomAttributes),
                DeclaringType = declaringType,
                Parameters = constructor
                    .GetParameters()
                    .Select(parameter => _GetParameter(parameter, memberDocumentation))
                    .AsReadOnlyList(),
                Summary = memberDocumentation.Summary,
                Exceptions = _MapExceptions(memberDocumentation.Exceptions),
                Remarks = memberDocumentation.Remarks,
                Examples = memberDocumentation.Examples,
                RelatedMembers = memberDocumentation.RelatedMembers
            };
        }

        private ConstructorDeclaration _GetDefaultConstructor(Type type, TypeDeclaration declaringType)
        {
            var memberDocumentation = _GetDefaultConstructorMemberDocumentationFor(type);

            return new ConstructorDeclaration
            {
                Name = declaringType.Name,
                AccessModifier = AccessModifier.Public,
                Attributes = Enumerable.Empty<AttributeData>().AsReadOnlyCollection(),
                DeclaringType = declaringType,
                Parameters = Enumerable
                    .Empty<ParameterData>()
                    .AsReadOnlyList(),
                Summary = memberDocumentation.Summary,
                Exceptions = _MapExceptions(memberDocumentation.Exceptions),
                Remarks = memberDocumentation.Remarks,
                Examples = memberDocumentation.Examples,
                RelatedMembers = memberDocumentation.RelatedMembers
            };
        }

        private EventDeclaration _GetEvent(EventInfo @event, TypeDeclaration declaringType)
        {
            var memberDocumentation = _GetMemberDocumentationFor(@event);
            var methodInfo = (@event.AddMethod ?? @event.RemoveMethod ?? @event.RaiseMethod);
            return new EventDeclaration
            {
                Name = @event.Name,
                AccessModifier = _GetAccessModifierFrom(@event),
                Type = _memberReferenceFactory.Create(@event.EventHandlerType),
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
            => new EventAccessorData
            {
                Attributes = _MapAttributesDataFrom(accessorMethod.CustomAttributes),
                ReturnAttributes = _MapAttributesDataFrom(accessorMethod.ReturnParameter.CustomAttributes)
            };

        private PropertyDeclaration _GetProperty(PropertyInfo property, TypeDeclaration declaringType)
        {
            var memberDocumentation = _GetMemberDocumentationFor(property);

            var getterInfo = _GetPropertyAccessorData(property.GetMethod);
            var setterInfo = _GetPropertyAccessorData(property.SetMethod);
            var methodInfo = (property.GetMethod ?? property.SetMethod);
            return new PropertyDeclaration
            {
                Name = property.Name,
                AccessModifier = setterInfo == null || (getterInfo != null && getterInfo.AccessModifier >= setterInfo.AccessModifier)
                    ? getterInfo.AccessModifier
                    : setterInfo.AccessModifier,
                Type = _memberReferenceFactory.Create(property.PropertyType),
                Attributes = _MapAttributesDataFrom(property.CustomAttributes),
                Parameters = property
                    .GetIndexParameters()
                    .Select(parameter => _GetParameter(parameter, memberDocumentation))
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

        private MethodDeclaration _GetMethod(MethodInfo method, TypeDeclaration declaringType)
        {
            var memberDocumentation = _GetMemberDocumentationFor(method);

            var methodDocumentationElement = new MethodDeclaration
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
                    .Select(parameter => _GetParameter(parameter, memberDocumentation))
                    .AsReadOnlyList(),
                Return = new MethodReturnData
                {
                    Type = method.ReturnType == typeof(object) && method.ReturnParameter.GetCustomAttribute<DynamicAttribute>() != null
                        ? _memberReferenceFactory.CreateDynamic()
                        : _memberReferenceFactory.Create(method.ReturnType),
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
                    return AccessModifier.FamilyOrAssembly;
                else if (type.IsNestedFamANDAssem)
                    return AccessModifier.FamilyAndAssembly;
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
                return AccessModifier.FamilyOrAssembly;
            else if (field.IsFamilyAndAssembly)
                return AccessModifier.FamilyAndAssembly;
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
                return AccessModifier.FamilyOrAssembly;
            else if (method.IsFamilyAndAssembly)
                return AccessModifier.FamilyAndAssembly;
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

        private IReadOnlyList<GenericTypeParameterData> _MapTypeGenericParameters(Type type, TypeDeclaration declaringDocumentationElement, MemberDocumentation memberDocumentation)
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

        private GenericTypeParameterData _GetTypeGenericParameter(Type type, TypeDeclaration declaringType, MemberDocumentation memberDocumentation)
        {
            var typeGenericParameter = new GenericTypeParameterData();
            _InitializeGenericParameter(typeGenericParameter, type);
            typeGenericParameter.DeclaringType = declaringType;

            memberDocumentation.GenericParameters.TryGetValue(typeGenericParameter.Name, out var description);
            typeGenericParameter.Description = description ?? _emptyBlockDocumentationElementCollection;

            return typeGenericParameter;
        }

        private GenericMethodParameterData _GetMethodGenericParameter(Type type, MethodDeclaration declaringMethod, MemberDocumentation memberDocumentation)
        {
            var methodGenericParameter = new GenericMethodParameterData();
            _InitializeGenericParameter(methodGenericParameter, type);
            methodGenericParameter.DeclaringMethod = declaringMethod;

            memberDocumentation.GenericParameters.TryGetValue(methodGenericParameter.Name, out var description);
            methodGenericParameter.Description = description ?? _emptyBlockDocumentationElementCollection;

            return methodGenericParameter;
        }

        private void _InitializeGenericParameter(GenericParameterData genericParameter, Type type)
        {
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
                    .OrderBy(genericParameterTypeConstraint => genericParameterTypeConstraint.Namespace)
                    .ThenBy(genericParameterTypeConstraint => genericParameterTypeConstraint.Name)
                    .Select(genericParameterTypeConstaint => _memberReferenceFactory.Create(genericParameterTypeConstaint))
                    .AsReadOnlyList();

            var memberDocumentation = _GetMemberDocumentationFor(type.DeclaringType);
            memberDocumentation.GenericParameters.TryGetValue(genericParameter.Name, out var description);
            genericParameter.Description = description ?? _emptyBlockDocumentationElementCollection;
        }

        private static Type _UnwrapeByRef(Type type)
        {
            var referentType = type;
            while (referentType.IsByRef)
                referentType = referentType.GetElementType();
            return referentType;
        }

        private ParameterData _GetParameter(ParameterInfo parameter, MemberDocumentation memberDocumentation)
        {
            var parameterData = new ParameterData
            {
                Name = parameter.Name,
                Type = _UnwrapeByRef(parameter.ParameterType) == typeof(object) && parameter.GetCustomAttribute<DynamicAttribute>() != null
                    ? _memberReferenceFactory.CreateDynamic()
                    : _memberReferenceFactory.Create(_UnwrapeByRef(parameter.ParameterType)),
                Attributes = _MapAttributesDataFrom(parameter.CustomAttributes),
                IsInputByReference = parameter.ParameterType.IsByRef && parameter.IsIn,
                IsInputOutputByReference = parameter.ParameterType.IsByRef && !parameter.IsIn && !parameter.IsOut,
                IsOutputByReference = parameter.ParameterType.IsByRef && parameter.IsOut,
                HasDefaultValue = parameter.HasDefaultValue,
                DefaultValue = parameter.HasDefaultValue ? parameter.RawDefaultValue : null
            };

            memberDocumentation.Parameters.TryGetValue(parameter.Name, out var description);
            parameterData.Description = description ?? _emptyBlockDocumentationElementCollection;

            return parameterData;
        }

        private IReadOnlyCollection<AttributeData> _MapAttributesDataFrom(IEnumerable<CustomAttributeData> customAttributes)
            => (
                from customAttribute in customAttributes
                where customAttribute.AttributeType != typeof(CompilerGeneratedAttribute)
                orderby customAttribute.AttributeType.Namespace, customAttribute.AttributeType.Name
                let constructorParameters = customAttribute.Constructor.GetParameters()
                select new AttributeData(
                    (TypeReference)_memberReferenceFactory.Create(customAttribute.AttributeType),
                    constructorParameters
                        .Zip(
                            customAttribute.ConstructorArguments,
                            (parameter, argument) =>
                                new AttributeParameterData
                                {
                                    Name = parameter.Name,
                                    Type = _memberReferenceFactory.Create(parameter.ParameterType),
                                    Value = _GetAttributeValue(parameter.ParameterType, argument)
                                }
                        )
                        .ToList(),
                    customAttribute
                        .NamedArguments
                        .OrderBy(argument => argument.MemberName)
                        .Select(
                            namedArgument =>
                            {
                                var parameterType = namedArgument.IsField
                                    ? ((FieldInfo)namedArgument.MemberInfo).FieldType
                                    : ((PropertyInfo)namedArgument.MemberInfo).PropertyType;
                                return new AttributeParameterData
                                {
                                    Name = namedArgument.MemberName,
                                    Type = _memberReferenceFactory.Create(parameterType),
                                    Value = _GetAttributeValue(parameterType, namedArgument.TypedValue)
                                };
                            }
                        )
                        .ToList()
            )
        )
        .ToList();

        private object _GetAttributeValue(Type type, CustomAttributeTypedArgument argument)
        {
            if (argument.Value == null)
                return null;
            if (argument.ArgumentType.IsArray)
                return _GetAttributeValues(type, (IEnumerable<CustomAttributeTypedArgument>)argument.Value);
            if (type.IsEnum)
                return Enum.Parse(type, Convert.ToString(argument.Value));
            if (argument.Value is Type typeArgument)
                return _memberReferenceFactory.Create(typeArgument);

            return argument.Value;
        }

        private object _GetAttributeValues(Type attributeParameterType, IEnumerable<CustomAttributeTypedArgument> values)
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

        private IReadOnlyCollection<ExceptionData> _MapExceptions(IReadOnlyDictionary<string, BlockDescriptionDocumentationElement> exceptions)
            => (
                from exception in exceptions
                let exceptionType = _canonicalNameResolver.TryFindMemberInfoFor(exception.Key) as Type
                where exceptionType != null && typeof(Exception).IsAssignableFrom(exceptionType)
                orderby exceptionType.Namespace, exceptionType.Name
                select new ExceptionData
                {
                    Type = (TypeReference)_memberReferenceFactory.Create(exceptionType),
                    Description = exception.Value
                }
        ).ToList();

        private MemberDocumentation _GetMemberDocumentationFor(MemberInfo memberInfo)
            => _membersDocumentation.TryFind(_canonicalNameResolver.GetCanonicalNameFrom(memberInfo), out var memberDocumentation) ? memberDocumentation : _emptyMemberDocumentation;

        private MemberDocumentation _GetDefaultConstructorMemberDocumentationFor(Type type)
            => _membersDocumentation.TryFind(_canonicalNameResolver.GetDefaultConstructorCanonicalNameFor(type), out var memberDocumentation) ? memberDocumentation : _emptyMemberDocumentation;
    }
}