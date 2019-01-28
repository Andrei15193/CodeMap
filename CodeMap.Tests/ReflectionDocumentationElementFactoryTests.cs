﻿using System;
using System.Collections.Generic;
using System.Linq;
using CodeMap.Elements;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests
{
    public class ReflectionDocumentationElementFactoryTests
    {
        [Fact]
        public void EnumDocumentationElement()
        {
            var factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = factory.Create(typeof(TestEnum));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestEnum")
                .AssertEqual(() => typeDocumentationElement.AccessModifier, AccessModifier.Public)
                .AssertNull(() => typeDocumentationElement.DeclaringType)
                .AssertAttributes(() => typeDocumentationElement.Attributes, "enum")
                .AssertIs<EnumDocumentationElement>(
                    enumDocumentationElement => enumDocumentationElement
                        .AssertType(() => enumDocumentationElement.UnderlyingType, typeof(byte))
                        .AssertCollectionMember(
                            () => enumDocumentationElement.Members,
                            enumMember => enumMember
                                .AssertEqual(() => enumMember.Name, "TestMember1")
                                .AssertAttributes(() => enumMember.Attributes, "enum member")
                                .AssertEqual(() => enumMember.AccessModifier, AccessModifier.Public)
                                .AssertSame(() => enumMember.DeclaringType, enumDocumentationElement)
                                .AssertType(() => enumMember.Type, typeof(TestEnum))
                                .AssertEqual(() => enumMember.Value, TestEnum.TestMember1)
                                .AssertNoDocumentation(),
                            enumMember => enumMember
                                .AssertEqual(() => enumMember.Name, "TestMember2")
                                .AssertEmpty(() => enumMember.Attributes)
                                .AssertEqual(() => enumMember.AccessModifier, AccessModifier.Public)
                                .AssertSame(() => enumMember.DeclaringType, enumDocumentationElement)
                                .AssertType(() => enumMember.Type, typeof(TestEnum))
                                .AssertEqual(() => enumMember.Value, TestEnum.TestMember2)
                                .AssertNoDocumentation(),
                            enumMember => enumMember
                                .AssertEqual(() => enumMember.Name, "TestMember3")
                                .AssertEmpty(() => enumMember.Attributes)
                                .AssertEqual(() => enumMember.AccessModifier, AccessModifier.Public)
                                .AssertSame(() => enumMember.DeclaringType, enumDocumentationElement)
                                .AssertType(() => enumMember.Type, typeof(TestEnum))
                                .AssertEqual(() => enumMember.Value, TestEnum.TestMember3)
                                .AssertNoDocumentation()
                        )
                )
                .AssertNoDocumentation();
        }

        [Fact]
        public void EnumDocumentationElementDocumentation()
        {
            var enumDocumentation = _CreateMemberDocumentationMock("T:CodeMap.Tests.Data.TestEnum");
            var enumMember1Documentation = _CreateMemberDocumentationMock("F:CodeMap.Tests.Data.TestEnum.TestMember1");
            var enumMember2Documentation = _CreateMemberDocumentationMock("F:CodeMap.Tests.Data.TestEnum.TestMember2");
            var enumMember3Documentation = _CreateMemberDocumentationMock("F:CodeMap.Tests.Data.TestEnum.TestMember3");
            var factory = new ReflectionDocumentationElementFactory(
                new MemberDocumentationCollection(
                    new[]
                    {
                        enumDocumentation,
                        enumMember1Documentation,
                        enumMember2Documentation,
                        enumMember3Documentation
                    }
                )
            );

            var typeDocumentationElement = factory.Create(typeof(TestEnum));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestEnum")
                .AssertIs<EnumDocumentationElement>(
                    enumDocumentationElement => enumDocumentationElement
                        .AssertCollectionMember(
                            () => enumDocumentationElement.Members,
                            enumMember => enumMember
                                .AssertEqual(() => enumMember.Name, "TestMember1")
                                .AssertDocumentation(enumMember1Documentation),
                            enumMember => enumMember
                                .AssertEqual(() => enumMember.Name, "TestMember2")
                                .AssertDocumentation(enumMember2Documentation),
                            enumMember => enumMember
                                .AssertEqual(() => enumMember.Name, "TestMember3")
                                .AssertDocumentation(enumMember3Documentation)
                        )
                )
                .AssertDocumentation(enumDocumentation);
        }

        [Fact]
        public void CreateDelegateDocumentationElement()
        {
            var _factory = new ReflectionDocumentationElementFactory();

            var delegateType = typeof(TestDelegate<>);
            var genericParameter = delegateType.GetGenericArguments().Single();
            var typeDocumentationElement = _factory.Create(delegateType);

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestDelegate")
                .AssertEqual(() => typeDocumentationElement.AccessModifier, AccessModifier.Public)
                .AssertNull(() => typeDocumentationElement.DeclaringType)
                .AssertAttributes(() => typeDocumentationElement.Attributes, "delegate")
                .AssertIs<DelegateDocumentationElement>(
                    delegateDocumentationElement => delegateDocumentationElement
                        .AssertType(() => delegateDocumentationElement.Return.Type, typeof(void))
                        .AssertMember(
                            () => delegateDocumentationElement.Return.Type,
                            returnType => returnType.AssertIs<VoidTypeReferenceDocumentationElement>()
                        )
                        .AssertAttributes(() => delegateDocumentationElement.Return.Attributes, "delegate return")
                        .AssertTypeGenericParameters(() => delegateDocumentationElement.GenericParameters)
                )
                .AssertDelegateParameters(genericParameter)
                .AssertNoDocumentation();
        }

        [Fact]
        public void CreateDelegateDocumentationElementDocumentation()
        {
            var memberDocumentation = _CreateMemberDocumentationMock("T:CodeMap.Tests.Data.TestDelegate`1");
            var _factory = new ReflectionDocumentationElementFactory(
                new MemberDocumentationCollection(
                    new[]
                    {
                        memberDocumentation
                    }
                )
            );

            var delegateType = typeof(TestDelegate<>);
            var genericParameter = delegateType.GetGenericArguments().Single();
            var typeDocumentationElement = _factory.Create(delegateType);

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestDelegate")
                .AssertIs<DelegateDocumentationElement>()
                .AssertDocumentation(memberDocumentation);
        }

        [Obsolete, Fact(Skip = "under refactorment")]
        public void CreateInterfaceDocumentationElementCheckBasicInformation()
        {
            var factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = factory.Create(typeof(ITestInterface<>));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "ITestInterface")
                .AssertEqual(() => typeDocumentationElement.AccessModifier, AccessModifier.Assembly)
                .AssertNull(() => typeDocumentationElement.DeclaringType)
                .AssertEmpty(() => typeDocumentationElement.Summary.Content)
                .AssertEmpty(() => typeDocumentationElement.Remarks.Content)
                .AssertEmpty(() => typeDocumentationElement.Examples)
                .AssertEmpty(() => typeDocumentationElement.RelatedMembers)
                .AssertIs<InterfaceDocumentationElement>(
                    interfaceDocumentationElement =>
                        interfaceDocumentationElement.AssertCollectionMember(
                            () => interfaceDocumentationElement.BaseInterfaces,
                            baseInterface => baseInterface
                                .AssertTypeReference("CodeMap.Tests.Data", "ITestExtendedInterface")
                                .AssertTypeReferenceAssembly("CodeMap.Tests", new Version(1, 2, 3, 4))
                        )
                );
        }

        [Obsolete, Fact(Skip = "under refactorment")]
        public void CreateInterfaceDocumentationElementCheckAttributes()
        {
            var _factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = _factory.Create(typeof(ITestInterface<>));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "ITestInterface")
                .AssertCollectionMember(
                    () => typeDocumentationElement.Attributes,
                    attribute =>
                        attribute
                            .AssertTypeReference(() => attribute.Type, "CodeMap.Tests.Data", "TestAttribute")
                            .AssertTypeReferenceAssembly(() => attribute.Type, "CodeMap.Tests", new Version(1, 2, 3, 4))
                            .AssertCollectionMember(
                                () => attribute.PositionalParameters,
                                parameter => parameter
                                    .AssertEqual(() => parameter.Name, "value1")
                                    .AssertEqual(() => parameter.Value, "test 1")
                                    .AssertTypeReference(() => parameter.Type, "System", "Object")
                                    .AssertTypeReferenceAssembly(() => parameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                            )
                            .AssertCollectionMember(
                                () => attribute.NamedParameters.OrderBy(namedParameter => namedParameter.Name),
                                parameter => parameter
                                    .AssertEqual(() => parameter.Name, "Value2")
                                    .AssertEqual(() => parameter.Value, "test 2")
                                    .AssertTypeReference(() => parameter.Type, "System", "Object")
                                    .AssertTypeReferenceAssembly(() => parameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0)),
                                parameter => parameter
                                    .AssertEqual(() => parameter.Name, "Value3")
                                    .AssertEqual(() => parameter.Value, "test 3")
                                    .AssertTypeReference(() => parameter.Type, "System", "Object")
                                    .AssertTypeReferenceAssembly(() => parameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                            )
                );
        }

        [Obsolete, Fact(Skip = "under refactorment")]
        public void CreateInterfaceDocumentationElementCheckDocumentation()
        {
            var memberDocumentation = _CreateMemberDocumentationMock("T:CodeMap.Tests.Data.ITestInterface`3");
            var factory = new ReflectionDocumentationElementFactory(new MemberDocumentationCollection(new[] { memberDocumentation }));

            var typeDocumentationElement = factory.Create(typeof(ITestInterface<>));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "ITestInterface")
                .AssertSame(() => typeDocumentationElement.Summary, memberDocumentation.Summary)
                .AssertSame(() => typeDocumentationElement.Remarks, memberDocumentation.Remarks)
                .AssertSame(() => typeDocumentationElement.Examples, memberDocumentation.Examples)
                .AssertSame(() => typeDocumentationElement.RelatedMembers, memberDocumentation.RelatedMembers);
        }

        [Obsolete, Fact(Skip = "under refactorment")]
        public void CreateInterfaceDocumentationElementCheckGenericParameters()
        {
            var factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = factory.Create(typeof(ITestInterface<>));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "ITestInterface")
                .AssertEqual(() => typeDocumentationElement.AccessModifier, AccessModifier.Assembly)
                .AssertNull(() => typeDocumentationElement.DeclaringType)
                .AssertEmpty(() => typeDocumentationElement.Summary.Content)
                .AssertEmpty(() => typeDocumentationElement.Remarks.Content)
                .AssertEmpty(() => typeDocumentationElement.Examples)
                .AssertEmpty(() => typeDocumentationElement.RelatedMembers)
                .AssertIs<InterfaceDocumentationElement>(
                    interfaceDocumentationElement =>
                        interfaceDocumentationElement.AssertCollectionMember(
                            () => interfaceDocumentationElement.GenericParameters,
                            genericParameter =>
                                genericParameter
                                    .AssertEqual(() => genericParameter.Name, "TParam1")
                                    .AssertTrue(() => genericParameter.IsCovariant)
                                    .AssertFalse(() => genericParameter.IsContravariant)
                                    .AssertTrue(() => genericParameter.HasReferenceTypeConstraint)
                                    .AssertFalse(() => genericParameter.HasNonNullableValueTypeConstraint)
                                    .AssertTrue(() => genericParameter.HasDefaultConstructorConstraint)
                                    .AssertCollectionMember(
                                        () => genericParameter.TypeConstraints,
                                        typeConstraint => typeConstraint
                                            .AssertSame(interfaceDocumentationElement.GenericParameters.ElementAt(1)),
                                        typeConstraint => typeConstraint
                                            .AssertTypeReference("System", "IComparable")
                                            .AssertGenericArguments(
                                                genericArgument => genericArgument.AssertSame(interfaceDocumentationElement.GenericParameters.ElementAt(0))
                                            )
                                            .AssertTypeReferenceAssembly("System.Private.CoreLib", new Version(4, 0, 0, 0))
                                    )
                                    .AssertEmpty(() => genericParameter.Description),
                            genericParameter =>
                                genericParameter
                                    .AssertEqual(() => genericParameter.Name, "TParam2")
                                    .AssertFalse(() => genericParameter.IsCovariant)
                                    .AssertTrue(() => genericParameter.IsContravariant)
                                    .AssertFalse(() => genericParameter.HasReferenceTypeConstraint)
                                    .AssertFalse(() => genericParameter.HasNonNullableValueTypeConstraint)
                                    .AssertFalse(() => genericParameter.HasDefaultConstructorConstraint)
                                    .AssertEmpty(() => genericParameter.TypeConstraints)
                                    .AssertEmpty(() => genericParameter.Description),
                            genericParameter =>
                                genericParameter
                                    .AssertEqual(() => genericParameter.Name, "TParam3")
                                    .AssertFalse(() => genericParameter.IsCovariant)
                                    .AssertFalse(() => genericParameter.IsContravariant)
                                    .AssertFalse(() => genericParameter.HasReferenceTypeConstraint)
                                    .AssertTrue(() => genericParameter.HasNonNullableValueTypeConstraint)
                                    .AssertFalse(() => genericParameter.HasDefaultConstructorConstraint)
                                    .AssertEmpty(() => genericParameter.TypeConstraints)
                                    .AssertEmpty(() => genericParameter.Description)
                    )
                );
        }

        [Obsolete, Fact(Skip = "under refactorment")]
        public void CreateInterfaceDocumentationElementCheckEventBasicInformation()
        {
            var factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = factory.Create(typeof(ITestInterface<>));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "ITestInterface")
                .AssertIs<InterfaceDocumentationElement>(
                    interfaceDocumentationElement =>
                        interfaceDocumentationElement.AssertCollectionMember(
                            () => interfaceDocumentationElement.Events,
                            @event =>
                                @event
                                    .AssertEqual(() => @event.Name, "TestEvent")
                                    .AssertEqual(() => @event.AccessModifier, AccessModifier.Public)
                                    .AssertSame(() => @event.DeclaringType, interfaceDocumentationElement)
                                    .AssertTypeReference(() => @event.Type, "System", "EventHandler")
                                    .AssertTypeReferenceAssembly(() => @event.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                    .AssertGenericArguments(
                                        () => @event.Type,
                                        genericArgument => genericArgument
                                            .AssertTypeReference("System", "EventArgs")
                                            .AssertTypeReferenceAssembly("System.Private.CoreLib", new Version(4, 0, 0, 0))
                                    )
                                    .AssertFalse(() => @event.IsStatic)
                                    .AssertFalse(() => @event.IsAbstract)
                                    .AssertFalse(() => @event.IsVirtual)
                                    .AssertFalse(() => @event.IsOverride)
                                    .AssertFalse(() => @event.IsSealed)
                                    .AssertEmpty(() => @event.Summary.Content)
                                    .AssertEmpty(() => @event.Remarks.Content)
                                    .AssertEmpty(() => @event.Examples)
                                    .AssertEmpty(() => @event.Exceptions)
                    )
                );
        }

        [Obsolete, Fact(Skip = "under refactorment")]
        public void CreateInterfaceDocumentationElementCheckEventAttributes()
        {
            var factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = factory.Create(typeof(ITestInterface<>));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "ITestInterface")
                .AssertIs<InterfaceDocumentationElement>(
                    interfaceDocumentationElement =>
                        interfaceDocumentationElement.AssertCollectionMember(
                            () => interfaceDocumentationElement.Events,
                            @event =>
                                @event
                                    .AssertEqual(() => @event.Name, "TestEvent")
                                    .AssertCollectionMember(
                                        () => @event.Attributes,
                                        attribute => attribute
                                            .AssertTypeReference(() => attribute.Type, "CodeMap.Tests.Data", "TestAttribute")
                                            .AssertTypeReferenceAssembly(() => attribute.Type, "CodeMap.Tests", new Version(1, 2, 3, 4))
                                            .AssertCollectionMember(
                                                () => attribute.PositionalParameters,
                                                attributeParameter => attributeParameter
                                                    .AssertEqual(() => attributeParameter.Name, "value1")
                                                    .AssertEqual(() => attributeParameter.Value, "event test 1")
                                                    .AssertTypeReference(() => attributeParameter.Type, "System", "Object")
                                                    .AssertTypeReferenceAssembly(() => attributeParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                            )
                                            .AssertCollectionMember(
                                                () => attribute.NamedParameters.OrderBy(attributeParameter => attributeParameter.Name),
                                                attributeParameter => attributeParameter
                                                    .AssertEqual(() => attributeParameter.Name, "Value2")
                                                    .AssertEqual(() => attributeParameter.Value, "event test 2")
                                                    .AssertTypeReference(() => attributeParameter.Type, "System", "Object")
                                                    .AssertTypeReferenceAssembly(() => attributeParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0)),
                                                attributeParameter => attributeParameter
                                                    .AssertEqual(() => attributeParameter.Name, "Value3")
                                                    .AssertEqual(() => attributeParameter.Value, "event test 3")
                                                    .AssertTypeReference(() => attributeParameter.Type, "System", "Object")
                                                    .AssertTypeReferenceAssembly(() => attributeParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                            )
                                    )
                                    .AssertCollectionMember(
                                        () => @event.Adder.Attributes,
                                        attribute => attribute
                                            .AssertTypeReference(() => attribute.Type, "System.Runtime.CompilerServices", "CompilerGeneratedAttribute")
                                            .AssertTypeReferenceAssembly(() => attribute.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                            .AssertEmpty(() => attribute.PositionalParameters)
                                            .AssertEmpty(() => attribute.NamedParameters)
                                    )
                                    .AssertEmpty(() => @event.Adder.ReturnAttributes)
                                    .AssertCollectionMember(
                                        () => @event.Remover.Attributes,
                                        attribute => attribute
                                            .AssertTypeReference(() => attribute.Type, "System.Runtime.CompilerServices", "CompilerGeneratedAttribute")
                                            .AssertTypeReferenceAssembly(() => attribute.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                            .AssertEmpty(() => attribute.PositionalParameters)
                                            .AssertEmpty(() => attribute.NamedParameters)
                                    )
                                    .AssertEmpty(() => @event.Remover.ReturnAttributes)
                    )
                );
        }

        [Obsolete, Fact(Skip = "under refactorment")]
        public void CreateInterfaceDocumentationElementCheckEventDocumentation()
        {
            var memberDocumentation = _CreateMemberDocumentationMock("E:CodeMap.Tests.Data.ITestInterface`3.TestEvent", exceptions: new[] { "T:System.ArgumentException" });
            var factory = new ReflectionDocumentationElementFactory(new MemberDocumentationCollection(new[] { memberDocumentation }));

            var typeDocumentationElement = factory.Create(typeof(ITestInterface<>));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "ITestInterface")
                .AssertIs<InterfaceDocumentationElement>(
                    interfaceDocumentationElement =>
                        interfaceDocumentationElement.AssertCollectionMember(
                            () => interfaceDocumentationElement.Events,
                            @event =>
                                @event
                                    .AssertEqual(() => @event.Name, "TestEvent")
                                    .AssertSame(() => @event.Summary, memberDocumentation.Summary)
                                    .AssertSame(() => @event.Remarks, memberDocumentation.Remarks)
                                    .AssertSame(() => @event.Examples, memberDocumentation.Examples)
                                    .AssertCollectionMember(
                                        () => @event.Exceptions,
                                        exception => exception
                                            .AssertTypeReference(() => exception.Type, "System", "ArgumentException")
                                            .AssertTypeReferenceAssembly(() => exception.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                            .AssertSameItems(() => exception.Description, memberDocumentation.Exceptions["T:System.ArgumentException"])
                                    )
                    )
                );
        }

        [Obsolete, Fact(Skip = "under refactorment")]
        public void CreateInterfaceDocumentationElementCheckPropertyBasicInformation()
        {
            var factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = factory.Create(typeof(ITestInterface<>));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "ITestInterface")
                .AssertIs<InterfaceDocumentationElement>(
                    interfaceDocumentationElement =>
                        interfaceDocumentationElement.AssertCollectionMember(
                            () => interfaceDocumentationElement.Properties,
                            property =>
                                property
                                    .AssertEqual(() => property.Name, "TestProperty")
                                    .AssertEqual(() => property.AccessModifier, AccessModifier.Public)
                                    .AssertSame(() => property.DeclaringType, interfaceDocumentationElement)
                                    .AssertTypeReference(() => property.Type, "System", "Int32")
                                    .AssertTypeReferenceAssembly(() => property.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                    .AssertFalse(() => property.IsStatic)
                                    .AssertFalse(() => property.IsAbstract)
                                    .AssertFalse(() => property.IsVirtual)
                                    .AssertFalse(() => property.IsOverride)
                                    .AssertFalse(() => property.IsSealed)
                                    .AssertEmpty(() => property.Summary.Content)
                                    .AssertEmpty(() => property.Value.Content)
                                    .AssertEmpty(() => property.Remarks.Content)
                                    .AssertEmpty(() => property.Examples)
                                    .AssertEmpty(() => property.Exceptions)
                    )
                );
        }

        [Obsolete, Fact(Skip = "under refactorment")]
        public void CreateInterfaceDocumentationElementCheckPropertyAttributes()
        {
            var factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = factory.Create(typeof(ITestInterface<>));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "ITestInterface")
                .AssertIs<InterfaceDocumentationElement>(
                    interfaceDocumentationElement =>
                        interfaceDocumentationElement.AssertCollectionMember(
                            () => interfaceDocumentationElement.Properties,
                            property =>
                                property
                                    .AssertEqual(() => property.Name, "TestProperty")
                                    .AssertCollectionMember(
                                        () => property.Attributes,
                                        attribute => attribute
                                            .AssertTypeReference(() => attribute.Type, "CodeMap.Tests.Data", "TestAttribute")
                                            .AssertTypeReferenceAssembly(() => attribute.Type, "CodeMap.Tests", new Version(1, 2, 3, 4))
                                            .AssertCollectionMember(
                                                () => attribute.PositionalParameters,
                                                attributeParameter => attributeParameter
                                                    .AssertEqual(() => attributeParameter.Name, "value1")
                                                    .AssertEqual(() => attributeParameter.Value, "property test 1")
                                                    .AssertTypeReference(() => attributeParameter.Type, "System", "Object")
                                                    .AssertTypeReferenceAssembly(() => attributeParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                            )
                                            .AssertCollectionMember(
                                                () => attribute.NamedParameters.OrderBy(attributeParameter => attributeParameter.Name),
                                                attributeParameter => attributeParameter
                                                    .AssertEqual(() => attributeParameter.Name, "Value2")
                                                    .AssertEqual(() => attributeParameter.Value, "property test 2")
                                                    .AssertTypeReference(() => attributeParameter.Type, "System", "Object")
                                                    .AssertTypeReferenceAssembly(() => attributeParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0)),
                                                attributeParameter => attributeParameter
                                                    .AssertEqual(() => attributeParameter.Name, "Value3")
                                                    .AssertEqual(() => attributeParameter.Value, "property test 3")
                                                    .AssertTypeReference(() => attributeParameter.Type, "System", "Object")
                                                    .AssertTypeReferenceAssembly(() => attributeParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                            )
                                    )
                    )
                );
        }

        [Obsolete, Fact(Skip = "under refactorment")]
        public void CreateInterfaceDocumentationElementCheckPropertyGetterAttributes()
        {
            var factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = factory.Create(typeof(ITestInterface<>));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "ITestInterface")
                .AssertIs<InterfaceDocumentationElement>(
                    interfaceDocumentationElement =>
                        interfaceDocumentationElement.AssertCollectionMember(
                            () => interfaceDocumentationElement.Properties,
                            property =>
                                property
                                    .AssertEqual(() => property.Name, "TestProperty")
                                    .AssertCollectionMember(
                                        () => property.Getter.Attributes,
                                        attribute => attribute
                                            .AssertTypeReference(() => attribute.Type, "CodeMap.Tests.Data", "TestAttribute")
                                            .AssertTypeReferenceAssembly(() => attribute.Type, "CodeMap.Tests", new Version(1, 2, 3, 4))
                                            .AssertCollectionMember(
                                                () => attribute.PositionalParameters,
                                                attributeParameter => attributeParameter
                                                    .AssertEqual(() => attributeParameter.Name, "value1")
                                                    .AssertEqual(() => attributeParameter.Value, "property getter test 1")
                                                    .AssertTypeReference(() => attributeParameter.Type, "System", "Object")
                                                    .AssertTypeReferenceAssembly(() => attributeParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                            )
                                            .AssertCollectionMember(
                                                () => attribute.NamedParameters.OrderBy(attributeParameter => attributeParameter.Name),
                                                attributeParameter => attributeParameter
                                                    .AssertEqual(() => attributeParameter.Name, "Value2")
                                                    .AssertEqual(() => attributeParameter.Value, "property getter test 2")
                                                    .AssertTypeReference(() => attributeParameter.Type, "System", "Object")
                                                    .AssertTypeReferenceAssembly(() => attributeParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0)),
                                                attributeParameter => attributeParameter
                                                    .AssertEqual(() => attributeParameter.Name, "Value3")
                                                    .AssertEqual(() => attributeParameter.Value, "property getter test 3")
                                                    .AssertTypeReference(() => attributeParameter.Type, "System", "Object")
                                                    .AssertTypeReferenceAssembly(() => attributeParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                            )
                                    )
                                    .AssertCollectionMember(
                                        () => property.Getter.ReturnAttributes,
                                        attribute => attribute
                                            .AssertTypeReference(() => attribute.Type, "CodeMap.Tests.Data", "TestAttribute")
                                            .AssertTypeReferenceAssembly(() => attribute.Type, "CodeMap.Tests", new Version(1, 2, 3, 4))
                                            .AssertCollectionMember(
                                                () => attribute.PositionalParameters,
                                                attributeParameter => attributeParameter
                                                    .AssertEqual(() => attributeParameter.Name, "value1")
                                                    .AssertEqual(() => attributeParameter.Value, "return property getter test 1")
                                                    .AssertTypeReference(() => attributeParameter.Type, "System", "Object")
                                                    .AssertTypeReferenceAssembly(() => attributeParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                            )
                                            .AssertCollectionMember(
                                                () => attribute.NamedParameters.OrderBy(attributeParameter => attributeParameter.Name),
                                                attributeParameter => attributeParameter
                                                    .AssertEqual(() => attributeParameter.Name, "Value2")
                                                    .AssertEqual(() => attributeParameter.Value, "return property getter test 2")
                                                    .AssertTypeReference(() => attributeParameter.Type, "System", "Object")
                                                    .AssertTypeReferenceAssembly(() => attributeParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0)),
                                                attributeParameter => attributeParameter
                                                    .AssertEqual(() => attributeParameter.Name, "Value3")
                                                    .AssertEqual(() => attributeParameter.Value, "return property getter test 3")
                                                    .AssertTypeReference(() => attributeParameter.Type, "System", "Object")
                                                    .AssertTypeReferenceAssembly(() => attributeParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                            )
                                    )
                    )
                );
        }

        [Obsolete, Fact(Skip = "under refactorment")]
        public void CreateInterfaceDocumentationElementCheckPropertySetterAttributes()
        {
            var factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = factory.Create(typeof(ITestInterface<>));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "ITestInterface")
                .AssertIs<InterfaceDocumentationElement>(
                    interfaceDocumentationElement =>
                        interfaceDocumentationElement.AssertCollectionMember(
                            () => interfaceDocumentationElement.Properties,
                            property =>
                                property
                                    .AssertEqual(() => property.Name, "TestProperty")
                                    .AssertCollectionMember(
                                        () => property.Setter.Attributes,
                                        attribute => attribute
                                            .AssertTypeReference(() => attribute.Type, "CodeMap.Tests.Data", "TestAttribute")
                                            .AssertTypeReferenceAssembly(() => attribute.Type, "CodeMap.Tests", new Version(1, 2, 3, 4))
                                            .AssertCollectionMember(
                                                () => attribute.PositionalParameters,
                                                attributeParameter => attributeParameter
                                                    .AssertEqual(() => attributeParameter.Name, "value1")
                                                    .AssertEqual(() => attributeParameter.Value, "property setter test 1")
                                                    .AssertTypeReference(() => attributeParameter.Type, "System", "Object")
                                                    .AssertTypeReferenceAssembly(() => attributeParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                            )
                                            .AssertCollectionMember(
                                                () => attribute.NamedParameters.OrderBy(attributeParameter => attributeParameter.Name),
                                                attributeParameter => attributeParameter
                                                    .AssertEqual(() => attributeParameter.Name, "Value2")
                                                    .AssertEqual(() => attributeParameter.Value, "property setter test 2")
                                                    .AssertTypeReference(() => attributeParameter.Type, "System", "Object")
                                                    .AssertTypeReferenceAssembly(() => attributeParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0)),
                                                attributeParameter => attributeParameter
                                                    .AssertEqual(() => attributeParameter.Name, "Value3")
                                                    .AssertEqual(() => attributeParameter.Value, "property setter test 3")
                                                    .AssertTypeReference(() => attributeParameter.Type, "System", "Object")
                                                    .AssertTypeReferenceAssembly(() => attributeParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                            )
                                    )
                                    .AssertCollectionMember(
                                        () => property.Setter.ReturnAttributes,
                                        attribute => attribute
                                            .AssertTypeReference(() => attribute.Type, "CodeMap.Tests.Data", "TestAttribute")
                                            .AssertTypeReferenceAssembly(() => attribute.Type, "CodeMap.Tests", new Version(1, 2, 3, 4))
                                            .AssertCollectionMember(
                                                () => attribute.PositionalParameters,
                                                attributeParameter => attributeParameter
                                                    .AssertEqual(() => attributeParameter.Name, "value1")
                                                    .AssertEqual(() => attributeParameter.Value, "return property setter test 1")
                                                    .AssertTypeReference(() => attributeParameter.Type, "System", "Object")
                                                    .AssertTypeReferenceAssembly(() => attributeParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                            )
                                            .AssertCollectionMember(
                                                () => attribute.NamedParameters.OrderBy(attributeParameter => attributeParameter.Name),
                                                attributeParameter => attributeParameter
                                                    .AssertEqual(() => attributeParameter.Name, "Value2")
                                                    .AssertEqual(() => attributeParameter.Value, "return property setter test 2")
                                                    .AssertTypeReference(() => attributeParameter.Type, "System", "Object")
                                                    .AssertTypeReferenceAssembly(() => attributeParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0)),
                                                attributeParameter => attributeParameter
                                                    .AssertEqual(() => attributeParameter.Name, "Value3")
                                                    .AssertEqual(() => attributeParameter.Value, "return property setter test 3")
                                                    .AssertTypeReference(() => attributeParameter.Type, "System", "Object")
                                                    .AssertTypeReferenceAssembly(() => attributeParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                            )
                                    )
                    )
                );
        }

        [Obsolete, Fact(Skip = "under refactorment")]
        public void CreateInterfaceDocumentationElementCheckPropertyDocumentation()
        {
            var memberDocumentation = _CreateMemberDocumentationMock("P:CodeMap.Tests.Data.ITestInterface`3.TestProperty", exceptions: new[] { "T:System.ArgumentException" });
            var factory = new ReflectionDocumentationElementFactory(new MemberDocumentationCollection(new[] { memberDocumentation }));

            var typeDocumentationElement = factory.Create(typeof(ITestInterface<>));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "ITestInterface")
                .AssertIs<InterfaceDocumentationElement>(
                    interfaceDocumentationElement =>
                        interfaceDocumentationElement.AssertCollectionMember(
                            () => interfaceDocumentationElement.Properties,
                            property =>
                                property
                                    .AssertEqual(() => property.Name, "TestProperty")
                                    .AssertSame(() => property.Summary, memberDocumentation.Summary)
                                    .AssertSame(() => property.Value, memberDocumentation.Value)
                                    .AssertSame(() => property.Remarks, memberDocumentation.Remarks)
                                    .AssertSame(() => property.Examples, memberDocumentation.Examples)
                                    .AssertCollectionMember(
                                        () => property.Exceptions,
                                        exception => exception
                                            .AssertTypeReference(() => exception.Type, "System", "ArgumentException")
                                            .AssertTypeReferenceAssembly(() => exception.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                            .AssertSameItems(() => exception.Description, memberDocumentation.Exceptions["T:System.ArgumentException"])
                                    )
                    )
                );
        }

        [Fact]
        public void ConstructorWithNullMembersDocumentationCollectionThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new ReflectionDocumentationElementFactory(null));
            Assert.Equal(new ArgumentNullException("membersDocumentation").Message, exception.Message);
        }

        private static MemberDocumentation _CreateMemberDocumentationMock(string canonicalName, IEnumerable<string> genericParameters = null, IEnumerable<string> parameters = null, IEnumerable<string> exceptions = null)
            => new MemberDocumentation(
                canonicalName,
                DocumentationElement.Summary(DocumentationElement.Paragraph()),
                Enumerable
                    .Range(1, 6)
                    .Select(genericParameterNumber => $"TParam{genericParameterNumber}")
                    .ToLookup(
                        genericParameter => genericParameter,
                        genericParameter => DocumentationElement.Paragraph() as BlockDocumentationElement
                    ),
                Enumerable
                    .Range(1, 42)
                    .Select(parameterNumber => $"param{parameterNumber}")
                    .ToLookup(
                        parameter => parameter,
                        parameter => DocumentationElement.Paragraph() as BlockDocumentationElement
                    ),
                new[] { DocumentationElement.Paragraph() },
                new[] { "T:System.ArgumentException", "T:System.ArgumentNullException" }
                    .ToLookup(
                        exception => exception,
                        exception => DocumentationElement.Paragraph() as BlockDocumentationElement
                    ),
                DocumentationElement.Remarks(DocumentationElement.Paragraph()),
                new[] { DocumentationElement.Example(DocumentationElement.Paragraph()) },
                DocumentationElement.Value(DocumentationElement.Paragraph()),
                new[] { DocumentationElement.MemberReference(typeof(object)) }
            );
    }
}