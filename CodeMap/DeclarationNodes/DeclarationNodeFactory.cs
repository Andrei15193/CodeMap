﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using CodeMap.DocumentationElements;
using CodeMap.ReferenceData;

namespace CodeMap.DeclarationNodes
{
    internal class DeclarationNodeFactory
    {
        private readonly MemberReferenceFactory _memberReferenceFactory = new MemberReferenceFactory();
        private readonly MemberDocumentation _emptyMemberDocumentation = new MemberDocumentation(string.Empty);
        private readonly BlockDescriptionDocumentationElement _emptyBlockDocumentationElementCollection = DocumentationElement.BlockDescription(Array.Empty<BlockDocumentationElement>());
        private readonly CanonicalNameResolver _canonicalNameResolver;
        private readonly MemberDocumentationCollection _membersDocumentation;
        private readonly DeclarationFilter _declarationFilter;

        public DeclarationNodeFactory(CanonicalNameResolver canonicalNameResolver, MemberDocumentationCollection membersDocumentation, DeclarationFilter declarationFilter)
            => (_canonicalNameResolver, _membersDocumentation, _declarationFilter) = (canonicalNameResolver, membersDocumentation, declarationFilter ?? new DeclarationFilter());

        public AssemblyDeclaration Create(Assembly assembly)
        {
            var assemblyName = assembly.GetName();
            var assemblyDocumentationElement = new AssemblyDeclaration(_memberReferenceFactory.Create(assemblyName))
            {
                Name = assemblyName.Name,
                Version = assemblyName.Version,
                Culture = assemblyName.CultureName,
                PublicKeyToken = assemblyName.GetPublicKeyToken().ToBase16String(),
                Dependencies = assembly
                    .GetReferencedAssemblies()
                    .OrderBy(dependency => dependency.Name, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(dependency => dependency.Version)
                    .Select(dependency => _memberReferenceFactory.Create(dependency))
                    .ToReadOnlyList(),
                Attributes = _MapAttributesDataFrom(assembly.CustomAttributes),
                Summary = DocumentationElement.Summary(),
                Remarks = DocumentationElement.Remarks(),
                Examples = Array.Empty<ExampleDocumentationElement>(),
                RelatedMembers = Array.Empty<ReferenceDocumentationElement>()
            };

            assemblyDocumentationElement.Namespaces = assembly
                .DefinedTypes
                .Where(type => type.DeclaringType is null && _declarationFilter.ShouldMap(type))
                .OrderBy(type => type.Namespace, StringComparer.OrdinalIgnoreCase)
                .GroupBy(type => type.Namespace, StringComparer.OrdinalIgnoreCase)
                .Select(
                    typesByNamespace =>
                    {
                        var @namespace = string.IsNullOrWhiteSpace(typesByNamespace.Key)
                            ? new GlobalNamespaceDeclaration(_memberReferenceFactory.CreateNamespace(string.Empty, assembly)) { Name = string.Empty }
                            : new NamespaceDeclaration(_memberReferenceFactory.CreateNamespace(typesByNamespace.Key, assembly)) { Name = typesByNamespace.Key };
                        @namespace.Assembly = assemblyDocumentationElement;
                        @namespace.Summary = DocumentationElement.Summary();
                        @namespace.Remarks = DocumentationElement.Remarks();
                        @namespace.Examples = Array.Empty<ExampleDocumentationElement>();
                        @namespace.RelatedMembers = Array.Empty<ReferenceDocumentationElement>();

                        var declaredTypes = _GetTypes(typesByNamespace, @namespace, null);
                        @namespace.DeclaredTypes = declaredTypes;

                        @namespace.Enums = declaredTypes.OfType<EnumDeclaration>().ToReadOnlyList();
                        @namespace.Delegates = declaredTypes.OfType<DelegateDeclaration>().ToReadOnlyList();
                        @namespace.Interfaces = declaredTypes.OfType<InterfaceDeclaration>().ToReadOnlyList();
                        @namespace.Classes = declaredTypes.OfType<ClassDeclaration>().ToReadOnlyList();
                        @namespace.Structs = declaredTypes.OfType<StructDeclaration>().ToReadOnlyList();
                        @namespace.Records = declaredTypes.OfType<RecordDeclaration>().ToReadOnlyList();

                        return @namespace;
                    }
                )
                .ToReadOnlyList();

            return assemblyDocumentationElement;
        }

        private IReadOnlyCollection<TypeDeclaration> _GetTypes(IEnumerable<Type> types, NamespaceDeclaration @namespace, TypeDeclaration declaringType)
        {
            return types
                .OrderBy(_GetTypeKindSortOrder)
                .ThenBy(typeDeclaration => typeDeclaration.Name, StringComparer.OrdinalIgnoreCase)
                .Select(type => _GetType(type, @namespace, declaringType))
                .ToReadOnlyList();

            int _GetTypeKindSortOrder(Type type)
            {
                if (type.IsEnum)
                    return 0;
                else if (typeof(Delegate).IsAssignableFrom(type))
                    return 1;
                else if (type.IsInterface)
                    return 2;
                else if (_IsRecordType(type))
                    return 3;
                else if (type.IsClass)
                    return 4;
                else if (type.IsValueType)
                    return 5;
                else
                    return 6;
            }
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
            else if (_IsRecordType(type))
                typeDocumentationElement = _GetRecord(type, @namespace);
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

        /// <summary>
        /// <para>
        /// For reasons unknown, there is no official way of detecting whether a type is a class or a record.
        /// I wonder how the compiler does since we have specific operators that are only available to records instances.
        /// </para>
        /// 
        /// <para>
        /// More info here: <a href="https://github.com/dotnet/roslyn/issues/45777">GH-45777 Annotate records and primary constructors with marker attributes</a>.
        /// </para>
        /// </summary>
        private static bool _IsRecordType(Type type)
            => _GetRecordMembers(type).Any();

        private static IEnumerable<MemberInfo> _GetRecordMembers(Type type)
        {
            var unspeakableCloneMethod = type.GetMethod("<Clone>$", BindingFlags.Public | BindingFlags.Instance);

            if (unspeakableCloneMethod is null || !unspeakableCloneMethod.ReturnType.IsAssignableFrom(type))
                yield break;

            yield return unspeakableCloneMethod;

            // This is a record
            var equalsMethod = type.GetMethod(nameof(object.Equals), BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new[] { typeof(object) }, null);
            if (GetBaseDefinition(equalsMethod) == typeof(object).GetMethod(nameof(object.Equals), BindingFlags.Public | BindingFlags.Instance))
                yield return equalsMethod;

            var getHashCodeMethod = type.GetMethod(nameof(object.GetHashCode), BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, Type.EmptyTypes, null);
            if (GetBaseDefinition(getHashCodeMethod) == typeof(object).GetMethod(nameof(object.GetHashCode), BindingFlags.Public | BindingFlags.Instance))
                yield return getHashCodeMethod;

            var toStringMethod = type.GetMethod(nameof(object.ToString), BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, Type.EmptyTypes, null);
            if (GetBaseDefinition(toStringMethod) == typeof(object).GetMethod(nameof(object.ToString), BindingFlags.Public | BindingFlags.Instance))
                yield return toStringMethod;

            var deconstructMethods = type
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(method => method.Name == "Deconstruct" && method.GetParameters().All(parameter => parameter.IsOut));
            foreach (var deconstructMethod in deconstructMethods)
                yield return deconstructMethod;

            var printMembersMethod = type.GetMethod("PrintMembers", BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, new[] { typeof(StringBuilder) }, null);
            if (printMembersMethod is object)
                yield return printMembersMethod;

            var baseRecordType = type;
            while (baseRecordType != typeof(object))
            {
                foreach (var equatableMethod in type.GetInterfaceMap(typeof(IEquatable<>).MakeGenericType(baseRecordType)).TargetMethods)
                    yield return equatableMethod;
                baseRecordType = baseRecordType.BaseType;
            }

            var equalityContractProperty = type.GetProperty("EqualityContract", BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, typeof(Type), Type.EmptyTypes, null);
            if (equalityContractProperty is object)
                yield return equalityContractProperty;

            var constructors = type
                .GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(constructor =>
                {
                    var constructorParameters = constructor.GetParameters();
                    if (constructorParameters.Length == 1)
                    {
                        var constructorParameter = constructorParameters.Single();
                        var constructorParameterCustomAttributes = constructorParameter.GetCustomAttributes();
                        return constructorParameter.ParameterType == type
                            && (type.IsSealed
                                || (constructorParameterCustomAttributes.Count() == 1
                                    && constructorParameterCustomAttributes
                                        .Select(attribute => attribute.GetType())
                                        .Count(attributeType => attributeType.Namespace == "System.Runtime.CompilerServices" && attributeType.Name == "NullableAttribute") == 1));
                    }
                    else
                        return false;
                });
            foreach (var constructor in constructors)
                yield return constructor;

            static MethodInfo GetBaseDefinition(MethodInfo method)
            {
                var previous = default(MethodInfo);
                var baseDefinition = method;
                do
                {
                    previous = baseDefinition;
                    baseDefinition = baseDefinition.GetBaseDefinition();
                } while (previous != baseDefinition);
                return baseDefinition;
            }
        }

        private EnumDeclaration _GetEnum(Type enumType)
        {
            var memberDocumentation = _GetMemberDocumentationFor(enumType);
            var enumDocumentationElement = new EnumDeclaration((TypeReference)_memberReferenceFactory.Create(enumType))
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
                .OrderBy(constant => constant.GetValue(null))
                .Select(constant => _GetConstant(constant, enumDocumentationElement))
                .ToReadOnlyList();

            return enumDocumentationElement;
        }

        private DelegateDeclaration _GetDelegate(Type delegateType)
        {
            var memberDocumentation = _GetMemberDocumentationFor(delegateType);
            var invokeMethodInfo = delegateType.GetMethod(nameof(Action.Invoke), BindingFlags.Public | BindingFlags.Instance);
            var delegateDocumentationElement = new DelegateDeclaration((TypeReference)_memberReferenceFactory.Create(delegateType))
            {
                Name = _GetTypeNameFor(delegateType),
                AccessModifier = _GetAccessModifierFrom(delegateType),
                Attributes = _MapAttributesDataFrom(delegateType.CustomAttributes),
                Parameters = invokeMethodInfo
                    .GetParameters()
                    .Select(parameter => _GetParameter(parameter, memberDocumentation))
                    .ToReadOnlyList(),
                Return = new MethodReturnData
                {
                    Type = invokeMethodInfo.ReturnType == typeof(object) && invokeMethodInfo.ReturnParameter.GetCustomAttribute<DynamicAttribute>() is object
                        ? _memberReferenceFactory.CreateDynamic()
                        : _memberReferenceFactory.Create(invokeMethodInfo.ReturnType),
                    Description = memberDocumentation.Returns,
                    Attributes = _MapAttributesDataFrom(invokeMethodInfo.ReturnParameter.CustomAttributes)
                },
                Summary = memberDocumentation.Summary,
                Remarks = memberDocumentation.Remarks,
                Examples = memberDocumentation.Examples,
                Exceptions = memberDocumentation.Exceptions,
                RelatedMembers = memberDocumentation.RelatedMembers
            };

            delegateDocumentationElement.GenericParameters = _MapTypeGenericParameters(delegateType, delegateDocumentationElement, memberDocumentation);

            return delegateDocumentationElement;
        }

        private TypeDeclaration _GetInterface(Type interfaceType)
        {
            var memberDocumentation = _GetMemberDocumentationFor(interfaceType);
            var interfaceDocumentationElement = new InterfaceDeclaration((TypeReference)_memberReferenceFactory.Create(interfaceType))
            {
                Name = _GetTypeNameFor(interfaceType),
                AccessModifier = _GetAccessModifierFrom(interfaceType),
                Attributes = _MapAttributesDataFrom(interfaceType.CustomAttributes),
                BaseInterfaces = interfaceType
                    .GetInterfaces()
                    .Except(interfaceType.GetInterfaces().SelectMany(baseInterface => baseInterface.GetInterfaces()))
                    .OrderBy(baseInterface => baseInterface.Namespace, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(baseInterface => baseInterface.Name, StringComparer.OrdinalIgnoreCase)
                    .Select(baseInterface => (TypeReference)_memberReferenceFactory.Create(baseInterface))
                    .ToReadOnlyList(),
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
                .ToReadOnlyList();

            return interfaceDocumentationElement;
        }

        private ClassDeclaration _GetClass(Type classType, NamespaceDeclaration @namespace)
        {
            var memberDocumentation = _GetMemberDocumentationFor(classType);
            var classDocumentationElement = new ClassDeclaration((TypeReference)_memberReferenceFactory.Create(classType))
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
                    .Except(classType.BaseType.GetInterfaces())
                    .Except(classType.GetInterfaces().SelectMany(baseInterface => baseInterface.GetInterfaces()))
                    .OrderBy(implementedInterface => implementedInterface.Namespace)
                    .ThenBy(implementedInterface => implementedInterface.Name)
                    .ThenBy(implementedInterface => implementedInterface.GetGenericArguments().Length)
                    .Select(implementedInterface => (TypeReference)_memberReferenceFactory.Create(implementedInterface))
                    .ToReadOnlyList(),
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
                .ToReadOnlyList();

            var nestedTypes = _GetTypes(classType.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic).Where(_declarationFilter.ShouldMap), @namespace, classDocumentationElement);
            classDocumentationElement.NestedTypes = nestedTypes;

            classDocumentationElement.NestedEnums = nestedTypes.OfType<EnumDeclaration>().ToReadOnlyList();
            classDocumentationElement.NestedDelegates = nestedTypes.OfType<DelegateDeclaration>().ToReadOnlyList();
            classDocumentationElement.NestedInterfaces = nestedTypes.OfType<InterfaceDeclaration>().ToReadOnlyList();
            classDocumentationElement.NestedClasses = nestedTypes.OfType<ClassDeclaration>().ToReadOnlyList();
            classDocumentationElement.NestedStructs = nestedTypes.OfType<StructDeclaration>().ToReadOnlyList();
            classDocumentationElement.NestedRecords = nestedTypes.OfType<RecordDeclaration>().ToReadOnlyList();

            return classDocumentationElement;
        }

        private TypeDeclaration _GetStruct(Type structType, NamespaceDeclaration @namespace)
        {
            var memberDocumentation = _GetMemberDocumentationFor(structType);
            var structDocumentationElement = new StructDeclaration((TypeReference)_memberReferenceFactory.Create(structType))
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
                    .ToReadOnlyList(),
                Summary = memberDocumentation.Summary,
                Remarks = memberDocumentation.Remarks,
                Examples = memberDocumentation.Examples,
                RelatedMembers = memberDocumentation.RelatedMembers
            };

            structDocumentationElement.GenericParameters = _MapTypeGenericParameters(structType, structDocumentationElement, memberDocumentation);
            structDocumentationElement.Constants = _GetConstants(structType, structDocumentationElement);
            structDocumentationElement.Fields = _GetFields(structType, structDocumentationElement);
            structDocumentationElement.Constructors =
                Enumerable
                    .Repeat(_GetDefaultConstructor(structType, structDocumentationElement), 1)
                    .Concat(_GetConstructors(structType, structDocumentationElement))
                    .ToReadOnlyList();
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
                .ToReadOnlyList();

            var nestedTypes = _GetTypes(structType.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic), @namespace, structDocumentationElement);
            structDocumentationElement.NestedTypes = nestedTypes;

            structDocumentationElement.NestedEnums = structDocumentationElement.NestedTypes.OfType<EnumDeclaration>().ToReadOnlyList();
            structDocumentationElement.NestedDelegates = nestedTypes.OfType<DelegateDeclaration>().ToReadOnlyList();
            structDocumentationElement.NestedInterfaces = nestedTypes.OfType<InterfaceDeclaration>().ToReadOnlyList();
            structDocumentationElement.NestedClasses = nestedTypes.OfType<ClassDeclaration>().ToReadOnlyList();
            structDocumentationElement.NestedStructs = nestedTypes.OfType<StructDeclaration>().ToReadOnlyList();
            structDocumentationElement.NestedRecords = nestedTypes.OfType<RecordDeclaration>().ToReadOnlyList();

            return structDocumentationElement;
        }

        private RecordDeclaration _GetRecord(Type recordType, NamespaceDeclaration @namespace)
        {
            var memberDocumentation = _GetMemberDocumentationFor(recordType);

            var recordDocumentationElement = new RecordDeclaration((TypeReference)_memberReferenceFactory.Create(recordType))
            {
                Name = _GetTypeNameFor(recordType),
                AccessModifier = _GetAccessModifierFrom(recordType),
                Attributes = _MapAttributesDataFrom(recordType
                    .CustomAttributes
                    .Where(customAttribute => customAttribute.AttributeType.Namespace != "System.Runtime.CompilerServices" || (customAttribute.AttributeType.Name != "NullableAttribute" && customAttribute.AttributeType.Name != "NullableContextAttribute"))),
                IsAbstract = recordType.IsAbstract,
                IsSealed = recordType.IsSealed,
                BaseRecord = (TypeReference)_memberReferenceFactory.Create(recordType.BaseType),
                ImplementedInterfaces = recordType
                    .GetInterfaces()
                    .Except(recordType.BaseType.GetInterfaces())
                    .Except(recordType.GetInterfaces().SelectMany(baseInterface => baseInterface.GetInterfaces()))
                    .Except(Enumerable.Repeat(typeof(IEquatable<>).MakeGenericType(recordType), 1))
                    .OrderBy(implementedInterface => implementedInterface.Namespace)
                    .ThenBy(implementedInterface => implementedInterface.Name)
                    .ThenBy(implementedInterface => implementedInterface.GetGenericArguments().Length)
                    .Select(implementedInterface => (TypeReference)_memberReferenceFactory.Create(implementedInterface))
                    .ToReadOnlyList(),
                Summary = memberDocumentation.Summary,
                Remarks = memberDocumentation.Remarks,
                Examples = memberDocumentation.Examples,
                RelatedMembers = memberDocumentation.RelatedMembers
            };

            recordDocumentationElement.GenericParameters = _MapTypeGenericParameters(recordType, recordDocumentationElement, memberDocumentation);
            recordDocumentationElement.Constants = _GetConstants(recordType, recordDocumentationElement);
            recordDocumentationElement.Fields = _GetFields(recordType, recordDocumentationElement);
            recordDocumentationElement.Constructors = _GetConstructors(recordType, recordDocumentationElement);
            recordDocumentationElement.Events = _GetEvents(recordType, recordDocumentationElement);
            recordDocumentationElement.Properties = _GetProperties(recordType, recordDocumentationElement);
            recordDocumentationElement.Methods = _GetMethods(recordType, recordDocumentationElement);
            recordDocumentationElement.Members = recordDocumentationElement
                .Constants
                .AsEnumerable<MemberDeclaration>()
                .Concat(recordDocumentationElement.Fields)
                .Concat(recordDocumentationElement.Constructors)
                .Concat(recordDocumentationElement.Events)
                .Concat(recordDocumentationElement.Properties)
                .Concat(recordDocumentationElement.Methods)
                .ToReadOnlyList();

            var nestedTypes = _GetTypes(recordType.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic).Where(_declarationFilter.ShouldMap), @namespace, recordDocumentationElement);
            recordDocumentationElement.NestedTypes = nestedTypes;

            recordDocumentationElement.NestedEnums = nestedTypes.OfType<EnumDeclaration>().ToReadOnlyList();
            recordDocumentationElement.NestedDelegates = nestedTypes.OfType<DelegateDeclaration>().ToReadOnlyList();
            recordDocumentationElement.NestedInterfaces = nestedTypes.OfType<InterfaceDeclaration>().ToReadOnlyList();
            recordDocumentationElement.NestedClasses = nestedTypes.OfType<ClassDeclaration>().ToReadOnlyList();
            recordDocumentationElement.NestedStructs = nestedTypes.OfType<StructDeclaration>().ToReadOnlyList();
            recordDocumentationElement.NestedRecords = nestedTypes.OfType<RecordDeclaration>().ToReadOnlyList();

            return recordDocumentationElement;
        }

        private IReadOnlyCollection<ConstantDeclaration> _GetConstants(Type declaringType, TypeDeclaration declaringDocumentationElement)
            => declaringType
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField | BindingFlags.DeclaredOnly)
                .Where(field => !field.IsSpecialName && field.IsLiteral && _declarationFilter.ShouldMap(field))
                .OrderBy(constant => constant.Name, StringComparer.OrdinalIgnoreCase)
                .Select(constant => _GetConstant(constant, declaringDocumentationElement))
                .ToReadOnlyList();

        private IReadOnlyCollection<FieldDeclaration> _GetFields(Type declaringType, TypeDeclaration declaringDocumentationElement)
            => declaringType
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.DeclaredOnly)
                .Where(field => !field.IsLiteral && _declarationFilter.ShouldMap(field))
                .OrderBy(field => field.Name, StringComparer.OrdinalIgnoreCase)
                .Select(field => _GetField(field, declaringDocumentationElement))
                .ToReadOnlyList();

        private IReadOnlyCollection<ConstructorDeclaration> _GetConstructors(Type declaringType, TypeDeclaration declaringDocumentationElement)
        {
            var excludedMembers = _GetRecordMembers(declaringType).ToArray();
            return declaringType
                .GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(constructor => !excludedMembers.Contains(constructor) && _declarationFilter.ShouldMap(constructor))
                .OrderBy(constructor => constructor.GetParameters().Length)
                .Select(constructor => _GetConstructor(constructor, declaringDocumentationElement))
                .ToReadOnlyList();
        }

        private IReadOnlyCollection<EventDeclaration> _GetEvents(Type declaringType, TypeDeclaration declaringDocumentationElement)
            => declaringType
                .GetEvents(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(_declarationFilter.ShouldMap)
                .OrderBy(@event => @event.Name, StringComparer.OrdinalIgnoreCase)
                .Select(@event => _GetEvent(@event, declaringDocumentationElement))
                .ToReadOnlyList();

        private IReadOnlyCollection<PropertyDeclaration> _GetProperties(Type declaringType, TypeDeclaration declaringDocumentationElement)
        {
            var excludedMembers = _GetRecordMembers(declaringType).ToArray();
            return declaringType
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(property => !excludedMembers.Contains(property) && _declarationFilter.ShouldMap(property))
                .OrderBy(property => property.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(property => property.GetIndexParameters().Length)
                .Select(property => _GetProperty(property, declaringDocumentationElement))
                .Where(property => property is object)
                .ToReadOnlyList();
        }

        private IReadOnlyCollection<MethodDeclaration> _GetMethods(Type declaringType, TypeDeclaration declaringDocumentationElement)
        {
            var excludedMembers = _GetRecordMembers(declaringType).ToArray();
            return declaringType
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(method => !method.IsSpecialName && !excludedMembers.Contains(method) && _declarationFilter.ShouldMap(method))
                .OrderBy(method => method.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(method => method.GetParameters().Length)
                .Select(method => _GetMethod(method, declaringDocumentationElement))
                .ToReadOnlyList();
        }

        private ConstantDeclaration _GetConstant(FieldInfo field, TypeDeclaration declaringType)
        {
            var memberDocumentation = _GetMemberDocumentationFor(field);
            return new ConstantDeclaration((ConstantReference)_memberReferenceFactory.Create(field))
            {
                Name = field.Name,
                AccessModifier = _GetAccessModifierFrom(field),
                IsShadowing = _IsShadowing(field),
                Value = field.GetValue(null),
                Type = field.FieldType == typeof(object) && field.GetCustomAttribute<DynamicAttribute>() is object
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
            return new FieldDeclaration((FieldReference)_memberReferenceFactory.Create(field))
            {
                Name = field.Name,
                AccessModifier = _GetAccessModifierFrom(field),
                IsReadOnly = field.IsInitOnly,
                IsStatic = field.IsStatic,
                IsShadowing = _IsShadowing(field),
                Type = field.FieldType == typeof(object) && field.GetCustomAttribute<DynamicAttribute>() is object
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

            return new ConstructorDeclaration((ConstructorReference)_memberReferenceFactory.Create(constructor))
            {
                Name = declaringType.Name,
                AccessModifier = _GetAccessModifierFrom(constructor),
                Attributes = _MapAttributesDataFrom(constructor.CustomAttributes),
                DeclaringType = declaringType,
                Parameters = constructor
                    .GetParameters()
                    .Select(parameter => _GetParameter(parameter, memberDocumentation))
                    .ToReadOnlyList(),
                Summary = memberDocumentation.Summary,
                Exceptions = memberDocumentation.Exceptions,
                Remarks = memberDocumentation.Remarks,
                Examples = memberDocumentation.Examples,
                RelatedMembers = memberDocumentation.RelatedMembers
            };
        }

        private ConstructorDeclaration _GetDefaultConstructor(Type type, TypeDeclaration declaringType)
        {
            var memberDocumentation = _GetDefaultConstructorMemberDocumentationFor(type);

            return new ConstructorDeclaration((ConstructorReference)_memberReferenceFactory.CreateDefaultConstructor(type))
            {
                Name = declaringType.Name,
                AccessModifier = AccessModifier.Public,
                Attributes = Array.Empty<AttributeData>(),
                DeclaringType = declaringType,
                Parameters = Array.Empty<ParameterData>(),
                Summary = memberDocumentation.Summary,
                Exceptions = memberDocumentation.Exceptions,
                Remarks = memberDocumentation.Remarks,
                Examples = memberDocumentation.Examples,
                RelatedMembers = memberDocumentation.RelatedMembers
            };
        }

        private EventDeclaration _GetEvent(EventInfo @event, TypeDeclaration declaringType)
        {
            var memberDocumentation = _GetMemberDocumentationFor(@event);
            var methodInfo = (@event.AddMethod ?? @event.RemoveMethod ?? @event.RaiseMethod);
            return new EventDeclaration((EventReference)_memberReferenceFactory.Create(@event))
            {
                Name = @event.Name,
                AccessModifier = _GetAccessModifierFrom(@event),
                Type = _memberReferenceFactory.Create(@event.EventHandlerType),
                Attributes = _MapAttributesDataFrom(@event.CustomAttributes),
                DeclaringType = declaringType,
                IsStatic = methodInfo.IsStatic,
                IsAbstract = methodInfo.IsAbstract && !@event.DeclaringType.IsInterface,
                IsVirtual = !methodInfo.IsAbstract && !methodInfo.IsFinal && methodInfo.IsVirtual && methodInfo.GetBaseDefinition() == methodInfo,
                IsOverride = methodInfo.IsVirtual && methodInfo.GetBaseDefinition() != methodInfo,
                IsSealed = methodInfo.IsVirtual && methodInfo.GetBaseDefinition() != methodInfo && methodInfo.IsFinal,
                IsShadowing = (!methodInfo.IsVirtual || methodInfo.GetBaseDefinition() == methodInfo) && _IsShadowing(@event),
                Adder = _GetEventAccessorData(@event.AddMethod),
                Remover = _GetEventAccessorData(@event.RemoveMethod),
                Summary = memberDocumentation.Summary,
                Exceptions = memberDocumentation.Exceptions,
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
            var getterInfo = _GetPropertyGetterData(property);
            var setterInfo = _GetPropertySetterData(property);
            if (getterInfo is null && setterInfo is null)
                return null;
            else
            {
                var memberDocumentation = _GetMemberDocumentationFor(property);
                var methodInfo = (property.GetMethod ?? property.SetMethod);
                return new PropertyDeclaration((PropertyReference)_memberReferenceFactory.Create(property))
                {
                    Name = property.Name,
                    AccessModifier = setterInfo is null || (getterInfo is object && getterInfo.AccessModifier >= setterInfo.AccessModifier)
                        ? getterInfo.AccessModifier
                        : setterInfo.AccessModifier,
                    Type = _memberReferenceFactory.Create(property.PropertyType),
                    Attributes = _MapAttributesDataFrom(property.CustomAttributes),
                    Parameters = property
                        .GetIndexParameters()
                        .Select(parameter => _GetParameter(parameter, memberDocumentation))
                        .ToReadOnlyList(),
                    DeclaringType = declaringType,
                    IsStatic = methodInfo.IsStatic,
                    IsAbstract = methodInfo.IsAbstract && !property.DeclaringType.IsInterface,
                    IsVirtual = !methodInfo.IsAbstract && !methodInfo.IsFinal && methodInfo.IsVirtual && methodInfo.GetBaseDefinition() == methodInfo,
                    IsOverride = methodInfo.IsVirtual && methodInfo.GetBaseDefinition() != methodInfo,
                    IsSealed = methodInfo.IsVirtual && methodInfo.GetBaseDefinition() != methodInfo && methodInfo.IsFinal,
                    IsShadowing = (!methodInfo.IsVirtual || methodInfo.GetBaseDefinition() == methodInfo) && _IsShadowing(property),
                    Getter = getterInfo,
                    Setter = setterInfo,
                    Summary = memberDocumentation.Summary,
                    Value = memberDocumentation.Value,
                    Exceptions = memberDocumentation.Exceptions,
                    Remarks = memberDocumentation.Remarks,
                    Examples = memberDocumentation.Examples,
                    RelatedMembers = memberDocumentation.RelatedMembers
                };
            }
        }

        private PropertyGetterData _GetPropertyGetterData(PropertyInfo property)
        {
            if (property.GetMethod is null || !_declarationFilter.ShouldMapPropertyAccessor(property.GetMethod))
                return null;

            return new PropertyGetterData
            {
                AccessModifier = _GetAccessModifierFrom(property.GetMethod),
                Attributes = _MapAttributesDataFrom(property.GetMethod.CustomAttributes),
                ReturnAttributes = _MapAttributesDataFrom(property.GetMethod.ReturnParameter.CustomAttributes)
            };
        }

        private PropertySetterData _GetPropertySetterData(PropertyInfo property)
        {
            if (property.SetMethod is null || !_declarationFilter.ShouldMapPropertyAccessor(property.SetMethod))
                return null;

            return new PropertySetterData
            {
                IsInitOnly = property
                    .SetMethod
                    .ReturnParameter
                    .GetRequiredCustomModifiers()
                    .Any(modifier => modifier.Namespace == "System.Runtime.CompilerServices" && modifier.Name == "IsExternalInit"),
                AccessModifier = _GetAccessModifierFrom(property.SetMethod),
                Attributes = _MapAttributesDataFrom(property.SetMethod.CustomAttributes),
                ReturnAttributes = _MapAttributesDataFrom(property.SetMethod.ReturnParameter.CustomAttributes)
            };
        }

        private MethodDeclaration _GetMethod(MethodInfo method, TypeDeclaration declaringType)
        {
            var memberDocumentation = _GetMemberDocumentationFor(method);

            var methodDocumentationElement = new MethodDeclaration((MethodReference)_memberReferenceFactory.Create(method))
            {
                Name = method.Name,
                AccessModifier = _GetAccessModifierFrom(method),
                Attributes = _MapAttributesDataFrom(method.CustomAttributes),
                DeclaringType = declaringType,
                IsStatic = method.IsStatic,
                IsAbstract = method.IsAbstract && !method.DeclaringType.IsInterface,
                IsVirtual = !method.IsAbstract && !method.IsFinal && method.IsVirtual && method.GetBaseDefinition() == method,
                IsOverride = method.IsVirtual && method.GetBaseDefinition() != method,
                IsSealed = method.IsVirtual && method.GetBaseDefinition() != method && method.IsFinal,
                IsShadowing = (!method.IsVirtual || method.GetBaseDefinition() == method) && _IsShadowing(method),
                Parameters = method
                    .GetParameters()
                    .Select(parameter => _GetParameter(parameter, memberDocumentation))
                    .ToReadOnlyList(),
                Return = new MethodReturnData
                {
                    Type = method.ReturnType == typeof(object) && method.ReturnParameter.GetCustomAttribute<DynamicAttribute>() is object
                        ? _memberReferenceFactory.CreateDynamic()
                        : _memberReferenceFactory.Create(method.ReturnType),
                    Description = memberDocumentation.Returns,
                    Attributes = _MapAttributesDataFrom(method.ReturnParameter.CustomAttributes)
                },
                Summary = memberDocumentation.Summary,
                Exceptions = memberDocumentation.Exceptions,
                Remarks = memberDocumentation.Remarks,
                Examples = memberDocumentation.Examples,
                RelatedMembers = memberDocumentation.RelatedMembers
            };

            methodDocumentationElement.GenericParameters = method
                .GetGenericArguments()
                .Select(typeGenericParameter => _GetMethodGenericParameter(typeGenericParameter, methodDocumentationElement, memberDocumentation))
                .ToReadOnlyList();

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
            var inheritedMembers = memberInfo.DeclaringType.IsInterface
                ? memberInfo.DeclaringType.GetInterfaces().SelectMany(baseInterface => baseInterface.GetMembers(bindingFlags))
                : memberInfo.DeclaringType.BaseType.GetMembers(bindingFlags);

            return memberInfo switch
            {
                MethodInfo methodInfo => inheritedMembers
                    .OfType<MethodInfo>()
                    .Any(
                        inheritedMethodInfo =>
                            string.Equals(methodInfo.Name, inheritedMethodInfo.Name, StringComparison.OrdinalIgnoreCase)
                            && methodInfo
                                .GetParameters()
                                .Select(parameter => _UnwrapeByRef(parameter.ParameterType))
                                .SequenceEqual(inheritedMethodInfo.GetParameters().Select(parameter => _UnwrapeByRef(parameter.ParameterType)))
                    ),
                PropertyInfo propertyInfo => inheritedMembers
                    .OfType<PropertyInfo>()
                    .Any(
                        inheritedMethodInfo =>
                            string.Equals(propertyInfo.Name, inheritedMethodInfo.Name, StringComparison.OrdinalIgnoreCase)
                            && propertyInfo
                                .GetIndexParameters()
                                .Select(parameter => _UnwrapeByRef(parameter.ParameterType))
                                .SequenceEqual(inheritedMethodInfo.GetIndexParameters().Select(parameter => _UnwrapeByRef(parameter.ParameterType)))
                    ),
                _ => inheritedMembers.Any(inheritedMemberInfo => string.Equals(memberInfo.Name, inheritedMemberInfo.Name, StringComparison.OrdinalIgnoreCase))
            };
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
                .ToReadOnlyList();

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
                (genericParameterAttributes & GenericParameterAttributes.NotNullableValueTypeConstraint) == GenericParameterAttributes.NotNullableValueTypeConstraint
                && !type.GetCustomAttributes().Any(attribute => string.Equals(attribute.GetType().Name, "IsUnmanagedAttribute", StringComparison.OrdinalIgnoreCase));
            genericParameter.HasReferenceTypeConstraint =
                (genericParameterAttributes & GenericParameterAttributes.ReferenceTypeConstraint) == GenericParameterAttributes.ReferenceTypeConstraint;
            genericParameter.HasDefaultConstructorConstraint =
                (genericParameterAttributes & (GenericParameterAttributes.DefaultConstructorConstraint | GenericParameterAttributes.NotNullableValueTypeConstraint)) == GenericParameterAttributes.DefaultConstructorConstraint;
            genericParameter.HasUnmanagedTypeConstraint =
                (genericParameterAttributes & GenericParameterAttributes.NotNullableValueTypeConstraint) == GenericParameterAttributes.NotNullableValueTypeConstraint
                && type.GetCustomAttributes().Any(attribute => string.Equals(attribute.GetType().Name, "IsUnmanagedAttribute", StringComparison.OrdinalIgnoreCase));
            genericParameter.TypeConstraints =
                type
                    .GetGenericParameterConstraints()
                    .Where(genericParameterTypeConstraint => genericParameterTypeConstraint != typeof(ValueType))
                    .OrderBy(genericParameterTypeConstraint => genericParameterTypeConstraint.Namespace, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(genericParameterTypeConstraint => genericParameterTypeConstraint.Name, StringComparer.OrdinalIgnoreCase)
                    .Select(genericParameterTypeConstaint => _memberReferenceFactory.Create(genericParameterTypeConstaint))
                    .ToReadOnlyList();

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
                Type = _UnwrapeByRef(parameter.ParameterType) == typeof(object) && parameter.GetCustomAttribute<DynamicAttribute>() is object
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
                        .ToReadOnlyList(),
                    customAttribute
                        .NamedArguments
                        .OrderBy(argument => argument.MemberName, StringComparer.OrdinalIgnoreCase)
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
                        .ToReadOnlyList()
            )
        )
        .ToReadOnlyList();

        private object _GetAttributeValue(Type type, CustomAttributeTypedArgument argument)
        {
            if (argument.Value is null)
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
            var valuesCollection = values.ToReadOnlyList();
            var result = Array.CreateInstance(
                attributeParameterType.IsArray ? attributeParameterType.GetElementType() : attributeParameterType,
                valuesCollection.Count
            );

            for (var index = 0; index < valuesCollection.Count; index++)
                result.SetValue(_GetAttributeValue(attributeParameterType, valuesCollection[index]), index);

            return result;
        }

        private MemberDocumentation _GetMemberDocumentationFor(MemberInfo memberInfo)
            => _membersDocumentation.TryFind(_canonicalNameResolver.GetCanonicalNameFrom(memberInfo), out var memberDocumentation) ? memberDocumentation : _emptyMemberDocumentation;

        private MemberDocumentation _GetDefaultConstructorMemberDocumentationFor(Type type)
            => _membersDocumentation.TryFind(_canonicalNameResolver.GetDefaultConstructorCanonicalNameFor(type), out var memberDocumentation) ? memberDocumentation : _emptyMemberDocumentation;
    }
}