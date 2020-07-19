﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using CodeMap.DeclarationNodes;
using CodeMap.DocumentationElements;
using CodeMap.ReferenceData;
using CodeMap.Tests.Data;
using CodeMap.Tests.DeclarationNodes.Mocks;
using CodeMap.Tests.DocumentationElements;
using Moq;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes
{
    public class DeclarationNodeTests
    {
        private static Assembly _TestDataAssembly
            => typeof(DeclarationNodeTests)
                .Assembly
                .GetReferencedAssemblies()
                .Where(dependency => dependency.Name == "CodeMap.Tests.Data")
                .Select(Assembly.Load)
                .Single();

        [Fact]
        public void CreateEnumDocumentationElement()
        {
            var typeDocumentationElement = DeclarationNode.Create(typeof(TestEnum));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestEnum")
                .AssertEqual(() => typeDocumentationElement.Namespace.Name, "CodeMap.Tests.Data")
                .AssertAssembly(() => typeDocumentationElement.Assembly, typeof(TestEnum).Assembly)
                .AssertSame(() => typeDocumentationElement.Assembly, typeDocumentationElement.Namespace.Assembly)
                .AssertEqual(() => typeDocumentationElement.AccessModifier, AccessModifier.Public)
                .AssertNull(() => typeDocumentationElement.DeclaringType)
                .AssertCollectionMember(
                    () => typeDocumentationElement.Attributes,
                    attribute => attribute.AssertTestAttribute("enum")
                )
                .AssertIs<EnumDeclaration>(
                    enumDocumentationElement => enumDocumentationElement
                        .AssertTypeReference(() => enumDocumentationElement.UnderlyingType, typeof(byte))
                        .AssertCollectionMember(
                            () => enumDocumentationElement.Members,
                            enumMember => enumMember
                                .AssertEqual(() => enumMember.Name, "TestMember1")
                                .AssertCollectionMember(
                                    () => enumMember.Attributes,
                                    attribute => attribute.AssertTestAttribute("enum member")
                                )
                                .AssertEqual(() => enumMember.AccessModifier, AccessModifier.Public)
                                .AssertSame(() => enumMember.DeclaringType, enumDocumentationElement)
                                .AssertTypeReference(() => enumMember.Type, typeof(TestEnum))
                                .AssertEqual(() => enumMember.Value, TestEnum.TestMember1)
                                .AssertNoDocumentation(),
                            enumMember => enumMember
                                .AssertEqual(() => enumMember.Name, "TestMember2")
                                .AssertEmpty(() => enumMember.Attributes)
                                .AssertEqual(() => enumMember.AccessModifier, AccessModifier.Public)
                                .AssertSame(() => enumMember.DeclaringType, enumDocumentationElement)
                                .AssertTypeReference(() => enumMember.Type, typeof(TestEnum))
                                .AssertEqual(() => enumMember.Value, TestEnum.TestMember2)
                                .AssertNoDocumentation(),
                            enumMember => enumMember
                                .AssertEqual(() => enumMember.Name, "TestMember3")
                                .AssertEmpty(() => enumMember.Attributes)
                                .AssertEqual(() => enumMember.AccessModifier, AccessModifier.Public)
                                .AssertSame(() => enumMember.DeclaringType, enumDocumentationElement)
                                .AssertTypeReference(() => enumMember.Type, typeof(TestEnum))
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
            var membersDocumentation = new MemberDocumentationCollection(
                new[]
                {
                    enumDocumentation,
                    enumMember1Documentation,
                    enumMember2Documentation,
                    enumMember3Documentation
                }
            );

            var typeDocumentationElement = DeclarationNode.Create(typeof(TestEnum), membersDocumentation);

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestEnum")
                .AssertIs<EnumDeclaration>(
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
            var delegateType = typeof(TestDelegate<>);
            var genericParameterType = delegateType.GetGenericArguments().Single();
            var typeDocumentationElement = DeclarationNode.Create(delegateType);

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestDelegate")
                .AssertEqual(() => typeDocumentationElement.Namespace.Name, "CodeMap.Tests.Data")
                .AssertAssembly(() => typeDocumentationElement.Assembly, typeof(TestDelegate<>).Assembly)
                .AssertSame(() => typeDocumentationElement.Assembly, typeDocumentationElement.Namespace.Assembly)
                .AssertEqual(() => typeDocumentationElement.AccessModifier, AccessModifier.Public)
                .AssertNull(() => typeDocumentationElement.DeclaringType)
                .AssertCollectionMember(
                    () => typeDocumentationElement.Attributes,
                    attribute => attribute.AssertTestAttribute("delegate")
                )
                .AssertIs<DelegateDeclaration>(
                    delegateDocumentationElement => delegateDocumentationElement
                        .AssertTypeReference(() => delegateDocumentationElement.Return.Type, typeof(void))
                        .AssertMember(
                            () => delegateDocumentationElement.Return.Type,
                            returnType => returnType.AssertIs<VoidTypeReference>()
                        )
                        .AssertCollectionMember(
                            () => delegateDocumentationElement.Return.Attributes,
                            attribute => attribute.AssertTestAttribute("delegate return")
                        )
                        .AssertTypeGenericParameters(() => delegateDocumentationElement.GenericParameters)
                )
                .AssertDelegateParameters(genericParameterType)
                .AssertNoDocumentation();
        }

        [Fact]
        public void CreateDelegateDocumentationElementDocumentation()
        {
            var memberDocumentation = _CreateMemberDocumentationMock("T:CodeMap.Tests.Data.TestDelegate`1");
            var membersDocumentation = new MemberDocumentationCollection(
                new[]
                {
                        memberDocumentation
                }
            );

            var delegateType = typeof(TestDelegate<>);
            var typeDocumentationElement = DeclarationNode.Create(delegateType, membersDocumentation);

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestDelegate")
                .AssertIs<DelegateDeclaration>()
                .AssertDocumentation(memberDocumentation);
        }

        [Fact]
        public void CreateInterfaceDocumentationElement()
        {
            var interfaceType = typeof(ITestInterface<>);
            var typeGenericParameter = interfaceType.GetGenericArguments().Single();
            var methodGenericParameter = interfaceType.GetMethod("TestMethod").GetGenericArguments().Single();

            var typeDocumentationElement = DeclarationNode.Create(interfaceType);

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "ITestInterface")
                .AssertEqual(() => typeDocumentationElement.Namespace.Name, "CodeMap.Tests.Data")
                .AssertAssembly(() => typeDocumentationElement.Assembly, typeof(ITestInterface<>).Assembly)
                .AssertSame(() => typeDocumentationElement.Assembly, typeDocumentationElement.Namespace.Assembly)
                .AssertEqual(() => typeDocumentationElement.AccessModifier, AccessModifier.Public)
                .AssertNull(() => typeDocumentationElement.DeclaringType)
                .AssertCollectionMember(
                    () => typeDocumentationElement.Attributes,
                    attribute => attribute.AssertTestAttribute("interface"),
                    attribute => attribute.AssertDefaultMemberAttribute()
                )
                .AssertIs<InterfaceDeclaration>(
                    interfaceDocumentationElement => interfaceDocumentationElement
                        .AssertTypeGenericParameters(() => interfaceDocumentationElement.GenericParameters)
                        .AssertCollectionMember(
                            () => interfaceDocumentationElement.BaseInterfaces,
                            baseInterface => baseInterface.AssertTypeReference(typeof(ITestExtendedBaseInterface))
                        )
                        .AssertTestEvent(() => interfaceDocumentationElement.Events, "TestEvent", "interface event")
                        .AssertShadowingEvent(() => interfaceDocumentationElement.Events, "InterfaceShadowedTestEvent")
                        .AssertTestProperty(() => interfaceDocumentationElement.Properties, "TestProperty", "interface property")
                        .AssertIndexProperty(() => interfaceDocumentationElement.Properties, typeGenericParameter, "interface indexer")
                        .AssertShadowingProperty(() => interfaceDocumentationElement.Properties, "InterfaceShadowedTestProperty")
                        .AssertTestMethod(() => interfaceDocumentationElement.Methods, "TestMethod", typeGenericParameter, methodGenericParameter, "interface method")
                        .AssertShadowingMethod(() => interfaceDocumentationElement.Methods, "InterfaceShadowedTestMethod")
                )
                .AssertNoDocumentation();
        }

        [Fact]
        public void CreateInterfaceDocumentationElementDocumentation()
        {
            var interfaceMemberDocumentation = _CreateMemberDocumentationMock("T:CodeMap.Tests.Data.ITestInterface`1");
            var shadowingEventMemberDocumentation = _CreateMemberDocumentationMock("E:CodeMap.Tests.Data.ITestInterface`1.InterfaceShadowedTestEvent");
            var testEventMemberDocumentation = _CreateMemberDocumentationMock("E:CodeMap.Tests.Data.ITestInterface`1.TestEvent");
            var indexerPropertyMemberDocumentation = _CreateMemberDocumentationMock("P:CodeMap.Tests.Data.ITestInterface`1.Item(" + CanonicalNameResolverTests.IndexerParameters + ")");
            var shadowingPropertyMemberDocumentation = _CreateMemberDocumentationMock("P:CodeMap.Tests.Data.ITestInterface`1.InterfaceShadowedTestProperty");
            var testPropertyMemberDocumentation = _CreateMemberDocumentationMock("P:CodeMap.Tests.Data.ITestInterface`1.TestProperty");
            var shadowingMethodMemberDocumentation = _CreateMemberDocumentationMock("M:CodeMap.Tests.Data.ITestInterface`1.InterfaceShadowedTestMethod");
            var testMethodMemberDocumentation = _CreateMemberDocumentationMock("M:CodeMap.Tests.Data.ITestInterface`1.TestMethod``1(" + CanonicalNameResolverTests.MethodParameters + ")");
            var membersDocumentation = new MemberDocumentationCollection(
                new[]
                {
                    interfaceMemberDocumentation,
                    shadowingEventMemberDocumentation,
                    testEventMemberDocumentation,
                    indexerPropertyMemberDocumentation,
                    shadowingPropertyMemberDocumentation,
                    testPropertyMemberDocumentation,
                    shadowingMethodMemberDocumentation,
                    testMethodMemberDocumentation
                }
            );

            var typeDocumentationElement = DeclarationNode.Create(typeof(ITestInterface<>), membersDocumentation);

            typeDocumentationElement
                .AssertIs<InterfaceDeclaration>(
                    interfaceDocumentationElement =>
                        interfaceDocumentationElement
                            .AssertCollectionMember(
                                () => interfaceDocumentationElement.Events.OrderBy(@event => @event.Name),
                                @event => @event.AssertDocumentation(shadowingEventMemberDocumentation),
                                @event => @event.AssertDocumentation(testEventMemberDocumentation)
                            )
                            .AssertCollectionMember(
                                () => interfaceDocumentationElement.Properties.OrderBy(property => property.Name),
                                property => property.AssertDocumentation(shadowingPropertyMemberDocumentation),
                                property => property.AssertDocumentation(indexerPropertyMemberDocumentation),
                                property => property.AssertDocumentation(testPropertyMemberDocumentation)
                            )
                            .AssertCollectionMember(
                                () => interfaceDocumentationElement.Methods.OrderBy(method => method.Name),
                                method => method.AssertDocumentation(shadowingMethodMemberDocumentation),
                                method => method.AssertDocumentation(testMethodMemberDocumentation)
                            )
                )
                .AssertDocumentation(interfaceMemberDocumentation);
        }

        [Fact]
        public void CreateClassDocumentationElement()
        {
            var classType = typeof(TestClass<>);
            var typeGenericParameter = classType.GetGenericArguments().Single();
            var methodGenericParameter = classType.GetMethod("TestMethod").GetGenericArguments().Single();

            var typeDocumentationElement = DeclarationNode.Create(classType);
            var baseTypeDocumentationElement = DeclarationNode.Create(typeof(TestBaseClass));

            baseTypeDocumentationElement
                .AssertEqual(() => baseTypeDocumentationElement.Name, "TestBaseClass")
                .AssertEqual(() => baseTypeDocumentationElement.Namespace.Name, "CodeMap.Tests.Data")
                .AssertAssembly(() => baseTypeDocumentationElement.Assembly, typeof(TestBaseClass).Assembly)
                .AssertSame(() => baseTypeDocumentationElement.Assembly, baseTypeDocumentationElement.Namespace.Assembly)
                .AssertIs<ClassDeclaration>(
                    baseClassDocumentationElement => baseClassDocumentationElement
                        .AssertTypeReference(() => baseClassDocumentationElement.BaseClass, typeof(object))
                        .AssertAbstractEvent(() => baseClassDocumentationElement.Events, "AbstractTestEvent")
                        .AssertVirtualEvent(() => baseClassDocumentationElement.Events, "VirtualTestEvent")

                        .AssertAbstractProperty(() => baseClassDocumentationElement.Properties, "AbstractTestProperty")
                        .AssertVirtualProperty(() => baseClassDocumentationElement.Properties, "VirtualTestProperty")
                        .AssertStaticProperty(() => baseClassDocumentationElement.Properties, "StaticTestProperty")

                        .AssertAbstractMethod(() => baseClassDocumentationElement.Methods, "AbstractTestMethod")
                        .AssertVirtualMethod(() => baseClassDocumentationElement.Methods, "VirtualTestMethod")
                        .AssertStaticMethod(() => baseClassDocumentationElement.Methods, "StaticTestMethod")
                );

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestClass")
                .AssertEqual(() => baseTypeDocumentationElement.Namespace.Name, "CodeMap.Tests.Data")
                .AssertAssembly(() => baseTypeDocumentationElement.Assembly, typeof(TestClass<>).Assembly)
                .AssertEqual(() => typeDocumentationElement.AccessModifier, AccessModifier.Public)
                .AssertNull(() => typeDocumentationElement.DeclaringType)
                .AssertCollectionMember(
                    () => typeDocumentationElement.Attributes,
                    attribute => attribute.AssertTestAttribute("class"),
                    attribute => attribute.AssertDefaultMemberAttribute()
                )
                .AssertIs<ClassDeclaration>(
                    classDocumentationElement => classDocumentationElement
                        .AssertFalse(() => classDocumentationElement.IsAbstract)
                        .AssertFalse(() => classDocumentationElement.IsSealed)
                        .AssertFalse(() => classDocumentationElement.IsStatic)
                        .AssertTypeGenericParameters(() => classDocumentationElement.GenericParameters)
                        .AssertTypeReference(() => classDocumentationElement.BaseClass, typeof(TestBaseClass))
                        .AssertCollectionMember(
                            () => classDocumentationElement.ImplementedInterfaces,
                            baseInterface => baseInterface.AssertTypeReference(typeof(ITestExtendedBaseInterface))
                        )
                        .AssertCollectionMember(
                            () => classDocumentationElement.Constants,
                            constant => constant
                                .AssertEqual(() => constant.Name, "TestConstant")
                                .AssertCollectionMember(
                                    () => constant.Attributes,
                                    attribute => attribute.AssertTestAttribute("class constant")
                                )
                                .AssertEqual(() => constant.AccessModifier, AccessModifier.Private)
                                .AssertSame(() => constant.DeclaringType, classDocumentationElement)
                                .AssertTypeReference(() => constant.Type, typeof(double))
                                .AssertEqual(() => constant.Value, 1.0)
                                .AssertNoDocumentation()
                        )
                        .AssertCollectionMember(
                            () => classDocumentationElement.Fields.OrderBy(field => field.Name),
                            field => field.AssertReadOnlyField(classDocumentationElement),
                            field => field.AssertShadowingField(classDocumentationElement),
                            field => field.AssertStaticField(classDocumentationElement),
                            field => field.AssertTestField(classDocumentationElement, "class field")
                        )

                        .AssertCollectionMember(
                            () => classDocumentationElement.Constructors,
                            constructor => constructor.AssertTestConstructor(classDocumentationElement, typeGenericParameter, "class constructor")
                        )

                        .AssertTestEvent(() => classDocumentationElement.Events, "TestEvent", "class event")

                        .AssertOverrideEvent(() => classDocumentationElement.Events, "AbstractTestEvent")
                        .AssertSealedEvent(() => classDocumentationElement.Events, "VirtualTestEvent")
                        .AssertStaticEvent(() => classDocumentationElement.Events, "StaticTestEvent")
                        .AssertShadowingEvent(() => classDocumentationElement.Events, "ClassShadowedTestEvent")

                        .AssertTestProperty(() => classDocumentationElement.Properties, "TestProperty", "class property")
                        .AssertIndexProperty(() => classDocumentationElement.Properties, typeGenericParameter, "class indexer")
                        .AssertOverrideProperty(() => classDocumentationElement.Properties, "AbstractTestProperty")
                        .AssertSealedProperty(() => classDocumentationElement.Properties, "VirtualTestProperty")
                        .AssertShadowingProperty(() => classDocumentationElement.Properties, "ClassShadowedTestProperty")

                        .AssertTestMethod(() => classDocumentationElement.Methods, "TestMethod", typeGenericParameter, methodGenericParameter, "class method")
                        .AssertOverrideMethod(() => classDocumentationElement.Methods, "AbstractTestMethod")
                        .AssertSealedMethod(() => classDocumentationElement.Methods, "VirtualTestMethod")
                        .AssertShadowingMethod(() => classDocumentationElement.Methods, "ClassShadowedTestMethod")
                )
                .AssertNoDocumentation();
        }

        [Fact]
        public void CreateClassDocumentationElementDocumentation()
        {
            var classMemberDocumentation = _CreateMemberDocumentationMock("T:CodeMap.Tests.Data.TestClass`1");

            var testConstantMemberDocumentation = _CreateMemberDocumentationMock("F:CodeMap.Tests.Data.TestClass`1.TestConstant");

            var readonlyTestFieldMemberDocumentation = _CreateMemberDocumentationMock("F:CodeMap.Tests.Data.TestClass`1.ReadonlyTestField");
            var shadowedTestFieldMemberDocumentation = _CreateMemberDocumentationMock("F:CodeMap.Tests.Data.TestClass`1.ShadowedTestField");
            var staticTestFieldMemberDocumentation = _CreateMemberDocumentationMock("F:CodeMap.Tests.Data.TestClass`1.StaticTestField");
            var testFieldMemberDocumentation = _CreateMemberDocumentationMock("F:CodeMap.Tests.Data.TestClass`1.TestField");

            var constructorMemberDocumentation = _CreateMemberDocumentationMock("M:CodeMap.Tests.Data.TestClass`1.#ctor(" + CanonicalNameResolverTests.ConstructorParameters + ")");

            var abstractTestEventMemberDocumentation = _CreateMemberDocumentationMock("E:CodeMap.Tests.Data.TestClass`1.AbstractTestEvent");
            var shadowingEventMemberDocumentation = _CreateMemberDocumentationMock("E:CodeMap.Tests.Data.TestClass`1.ClassShadowedTestEvent");
            var interfaceShadowedTestEventMemberDocumentation = _CreateMemberDocumentationMock("E:CodeMap.Tests.Data.TestClass`1.CodeMap#Tests#Data#ITestBaseInterface#InterfaceShadowedTestEvent");
            var staticTestEventMemberDocumentation = _CreateMemberDocumentationMock("E:CodeMap.Tests.Data.TestClass`1.StaticTestEvent");
            var testEventMemberDocumentation = _CreateMemberDocumentationMock("E:CodeMap.Tests.Data.TestClass`1.TestEvent");
            var virtualTestEventMemberDocumentation = _CreateMemberDocumentationMock("E:CodeMap.Tests.Data.TestClass`1.VirtualTestEvent");

            var abstractTestPropertyMemberDocumentation = _CreateMemberDocumentationMock("P:CodeMap.Tests.Data.TestClass`1.AbstractTestProperty");
            var shadowingPropertyMemberDocumentation = _CreateMemberDocumentationMock("P:CodeMap.Tests.Data.TestClass`1.ClassShadowedTestProperty");
            var interfaceShadowedTestPropertyMemberDocumentation = _CreateMemberDocumentationMock("P:CodeMap.Tests.Data.TestClass`1.InterfaceShadowedTestProperty");
            var indexerPropertyMemberDocumentation = _CreateMemberDocumentationMock("P:CodeMap.Tests.Data.TestClass`1.Item(" + CanonicalNameResolverTests.IndexerParameters + ")");
            var testPropertyMemberDocumentation = _CreateMemberDocumentationMock("P:CodeMap.Tests.Data.TestClass`1.TestProperty");
            var virtualTestPropertyMemberDocumentation = _CreateMemberDocumentationMock("P:CodeMap.Tests.Data.TestClass`1.VirtualTestProperty");

            var abstractTestMethodMemberDocumentation = _CreateMemberDocumentationMock("M:CodeMap.Tests.Data.TestClass`1.AbstractTestMethod");
            var shadowingMethodMemberDocumentation = _CreateMemberDocumentationMock("M:CodeMap.Tests.Data.TestClass`1.ClassShadowedTestMethod");
            var baseTestMethodMemberDocumentation = _CreateMemberDocumentationMock("M:CodeMap.Tests.Data.TestClass`1.CodeMap#Tests#Data#ITestBaseInterface#BaseTestMethod");
            var interfaceShadowedTestMethod = _CreateMemberDocumentationMock("M:CodeMap.Tests.Data.TestClass`1.CodeMap#Tests#Data#ITestBaseInterface#InterfaceShadowedTestMethod");
            var testMethodMemberDocumentation = _CreateMemberDocumentationMock("M:CodeMap.Tests.Data.TestClass`1.TestMethod``1(" + CanonicalNameResolverTests.MethodParameters + ")");
            var virtualTestMethodMemberDocumentation = _CreateMemberDocumentationMock("M:CodeMap.Tests.Data.TestClass`1.VirtualTestMethod");
            var membersDocumentation = new MemberDocumentationCollection(
                new[]
                {
                    classMemberDocumentation,

                    testConstantMemberDocumentation,

                    readonlyTestFieldMemberDocumentation,
                    shadowedTestFieldMemberDocumentation,
                    staticTestFieldMemberDocumentation,
                    testFieldMemberDocumentation,

                    constructorMemberDocumentation,

                    abstractTestEventMemberDocumentation,
                    shadowingEventMemberDocumentation,
                    interfaceShadowedTestEventMemberDocumentation,
                    staticTestEventMemberDocumentation,
                    testEventMemberDocumentation,
                    virtualTestEventMemberDocumentation,

                    abstractTestPropertyMemberDocumentation,
                    shadowingPropertyMemberDocumentation,
                    interfaceShadowedTestPropertyMemberDocumentation,
                    indexerPropertyMemberDocumentation,
                    testPropertyMemberDocumentation,
                    virtualTestPropertyMemberDocumentation,

                    abstractTestMethodMemberDocumentation,
                    shadowingMethodMemberDocumentation,
                    baseTestMethodMemberDocumentation,
                    interfaceShadowedTestMethod,
                    testMethodMemberDocumentation,
                    virtualTestMethodMemberDocumentation
                }
            );

            var typeDocumentationElement = DeclarationNode.Create(typeof(TestClass<>), membersDocumentation);

            typeDocumentationElement
                .AssertIs<ClassDeclaration>(
                    classDocumentationElement =>
                        classDocumentationElement
                            .AssertCollectionMember(
                                () => classDocumentationElement.Constants.OrderBy(constant => constant.Name),
                                constant => constant.AssertDocumentation(testConstantMemberDocumentation)
                            )
                            .AssertCollectionMember(
                                () => classDocumentationElement.Fields.OrderBy(field => field.Name),
                                field => field.AssertDocumentation(readonlyTestFieldMemberDocumentation),
                                field => field.AssertDocumentation(shadowedTestFieldMemberDocumentation),
                                field => field.AssertDocumentation(staticTestFieldMemberDocumentation),
                                field => field.AssertDocumentation(testFieldMemberDocumentation)
                            )
                            .AssertCollectionMember(
                                () => classDocumentationElement.Constructors,
                                constructor => constructor.AssertDocumentation(constructorMemberDocumentation)
                            )
                            .AssertCollectionMember(
                                () => classDocumentationElement.Events.OrderBy(@event => @event.Name),
                                @event => @event.AssertDocumentation(abstractTestEventMemberDocumentation),
                                @event => @event.AssertDocumentation(shadowingEventMemberDocumentation),
                                @event => @event.AssertDocumentation(interfaceShadowedTestEventMemberDocumentation),
                                @event => @event.AssertDocumentation(staticTestEventMemberDocumentation),
                                @event => @event.AssertDocumentation(testEventMemberDocumentation),
                                @event => @event.AssertDocumentation(virtualTestEventMemberDocumentation)
                            )
                            .AssertCollectionMember(
                                () => classDocumentationElement.Properties.OrderBy(property => property.Name),
                                property => property.AssertDocumentation(abstractTestPropertyMemberDocumentation),
                                property => property.AssertDocumentation(shadowingPropertyMemberDocumentation),
                                property => property.AssertDocumentation(interfaceShadowedTestPropertyMemberDocumentation),
                                property => property.AssertDocumentation(indexerPropertyMemberDocumentation),
                                property => property.AssertDocumentation(testPropertyMemberDocumentation),
                                property => property.AssertDocumentation(virtualTestPropertyMemberDocumentation)
                            )
                            .AssertCollectionMember(
                                () => classDocumentationElement.Methods.OrderBy(method => method.Name),
                                method => method.AssertDocumentation(abstractTestMethodMemberDocumentation),
                                method => method.AssertDocumentation(shadowingMethodMemberDocumentation),
                                method => method.AssertDocumentation(baseTestMethodMemberDocumentation),
                                method => method.AssertDocumentation(interfaceShadowedTestMethod),
                                method => method.AssertDocumentation(testMethodMemberDocumentation),
                                method => method.AssertDocumentation(virtualTestMethodMemberDocumentation)
                            )
                )
                .AssertDocumentation(classMemberDocumentation);
        }

        [Fact]
        public void CreateClassDocumentationElementForAbstractClass()
        {
            var typeDocumentationElement = DeclarationNode.Create(typeof(TestAbstractClass));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestAbstractClass")
                .AssertIs<ClassDeclaration>(
                    classDocumentationElement => classDocumentationElement
                        .AssertTrue(() => classDocumentationElement.IsAbstract)
                        .AssertFalse(() => classDocumentationElement.IsSealed)
                        .AssertFalse(() => classDocumentationElement.IsStatic)
                );
        }

        [Fact]
        public void CreateClassDocumentationElementForSealedClass()
        {
            var typeDocumentationElement = DeclarationNode.Create(typeof(TestSealedClass));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestSealedClass")
                .AssertIs<ClassDeclaration>(
                    classDocumentationElement => classDocumentationElement
                        .AssertFalse(() => classDocumentationElement.IsAbstract)
                        .AssertTrue(() => classDocumentationElement.IsSealed)
                        .AssertFalse(() => classDocumentationElement.IsStatic)
                );
        }

        [Fact]
        public void CreateClassDocumentationElementForStaticClass()
        {
            var typeDocumentationElement = DeclarationNode.Create(typeof(TestStaticClass));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestStaticClass")
                .AssertIs<ClassDeclaration>(
                    classDocumentationElement => classDocumentationElement
                        .AssertFalse(() => classDocumentationElement.IsAbstract)
                        .AssertFalse(() => classDocumentationElement.IsSealed)
                        .AssertTrue(() => classDocumentationElement.IsStatic)
                );
        }

        [Fact]
        public void CreateClassDocumentationElementNestedTypes()
        {
            var typeDocumentationElement = DeclarationNode.Create(typeof(TestClass<>));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestClass")
                .AssertIs<ClassDeclaration>(
                    classDocumentationElement => classDocumentationElement
                        .AssertCollectionMember(
                            () => classDocumentationElement.NestedEnums,
                            @enum => @enum
                                .AssertEqual(() => @enum.Name, "NestedTestEnum")
                                .AssertSame(() => @enum.DeclaringType, classDocumentationElement)
                        )
                        .AssertCollectionMember(
                            () => classDocumentationElement.NestedDelegates,
                            @delegate => @delegate
                                .AssertEqual(() => @delegate.Name, "NestedTestDelegate")
                                .AssertSame(() => @delegate.DeclaringType, classDocumentationElement)
                        )
                        .AssertCollectionMember(
                            () => classDocumentationElement.NestedInterfaces,
                            @interface => @interface
                                .AssertEqual(() => @interface.Name, "INestedTestInterface")
                                .AssertSame(() => @interface.DeclaringType, classDocumentationElement)
                        )
                        .AssertCollectionMember(
                            () => classDocumentationElement.NestedClasses,
                            @class => @class
                                .AssertEqual(() => @class.Name, "NestedTestClass")
                                .AssertSame(() => @class.DeclaringType, classDocumentationElement)
                        )
                        .AssertCollectionMember(
                            () => classDocumentationElement.NestedStructs,
                            @struct => @struct
                                .AssertEqual(() => @struct.Name, "NestedTestStruct")
                                .AssertSame(() => @struct.DeclaringType, classDocumentationElement)
                        )
                );
        }

        [Fact]
        public void CreateStructDocumentationElement()
        {
            var structType = typeof(TestStruct<>);
            var typeGenericParameter = structType.GetGenericArguments().Single();
            var methodGenericParameter = structType.GetMethod("TestMethod").GetGenericArguments().Single();

            var typeDocumentationElement = DeclarationNode.Create(structType);

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestStruct")
                .AssertEqual(() => typeDocumentationElement.Namespace.Name, "CodeMap.Tests.Data")
                .AssertAssembly(() => typeDocumentationElement.Assembly, typeof(TestStruct<>).Assembly)
                .AssertSame(() => typeDocumentationElement.Assembly, typeDocumentationElement.Namespace.Assembly)
                .AssertEqual(() => typeDocumentationElement.AccessModifier, AccessModifier.Public)
                .AssertNull(() => typeDocumentationElement.DeclaringType)
                .AssertCollectionMember(
                    () => typeDocumentationElement.Attributes,
                    attribute => attribute.AssertTestAttribute("struct"),
                    attribute => attribute.AssertDefaultMemberAttribute()
                )
                .AssertIs<StructDeclaration>(
                    structDocumentationElement => structDocumentationElement
                        .AssertTypeGenericParameters(() => structDocumentationElement.GenericParameters)
                        .AssertCollectionMember(
                            () => structDocumentationElement.ImplementedInterfaces,
                            baseInterface => baseInterface.AssertTypeReference(typeof(ITestExtendedBaseInterface))
                        )
                        .AssertCollectionMember(
                            () => structDocumentationElement.Constants,
                            constant => constant
                                .AssertEqual(() => constant.Name, "TestConstant")
                                .AssertCollectionMember(
                                    () => constant.Attributes,
                                    attribute => attribute.AssertTestAttribute("struct constant")
                                )
                                .AssertEqual(() => constant.AccessModifier, AccessModifier.Private)
                                .AssertSame(() => constant.DeclaringType, structDocumentationElement)
                                .AssertTypeReference(() => constant.Type, typeof(double))
                                .AssertEqual(() => constant.Value, 1.0)
                                .AssertNoDocumentation()
                        )
                        .AssertCollectionMember(
                            () => structDocumentationElement.Fields.OrderBy(field => field.Name),
                            field => field.AssertReadOnlyField(structDocumentationElement),
                            field => field.AssertStaticField(structDocumentationElement),
                            field => field.AssertTestField(structDocumentationElement, "struct field")
                        )

                        .AssertCollectionMember(
                            () => structDocumentationElement.Constructors.OrderBy(constructor => constructor.Parameters.Count),
                            constructor => constructor.AssertDefaultConstructor(structDocumentationElement),
                            constructor => constructor.AssertTestConstructor(structDocumentationElement, typeGenericParameter, "struct constructor")
                        )

                        .AssertTestEvent(() => structDocumentationElement.Events, "TestEvent", "struct event")

                        .AssertStaticEvent(() => structDocumentationElement.Events, "StaticTestEvent")

                        .AssertReadOnlyTestProperty(() => structDocumentationElement.Properties, "TestProperty", "struct property")
                        .AssertIndexProperty(() => structDocumentationElement.Properties, typeGenericParameter, "struct indexer")

                        .AssertTestMethod(() => structDocumentationElement.Methods, "TestMethod", typeGenericParameter, methodGenericParameter, "struct method")
                        .AssertOverrideMethod(() => structDocumentationElement.Methods, "ToString")
                        .AssertShadowingMethod(() => structDocumentationElement.Methods, "GetHashCode")
                )
            .AssertNoDocumentation();
        }

        [Fact]
        public void CreateStructDocumentationElementDocumentation()
        {
            var structMemberDocumentation = _CreateMemberDocumentationMock("T:CodeMap.Tests.Data.TestStruct`1");

            var testConstantMemberDocumentation = _CreateMemberDocumentationMock("F:CodeMap.Tests.Data.TestStruct`1.TestConstant");

            var readonlyTestFieldMemberDocumentation = _CreateMemberDocumentationMock("F:CodeMap.Tests.Data.TestStruct`1.ReadonlyTestField");
            var staticTestFieldMemberDocumentation = _CreateMemberDocumentationMock("F:CodeMap.Tests.Data.TestStruct`1.StaticTestField");
            var testFieldMemberDocumentation = _CreateMemberDocumentationMock("F:CodeMap.Tests.Data.TestStruct`1.TestField");

            var defaultConstructorMemberDocumentation = _CreateMemberDocumentationMock("M:CodeMap.Tests.Data.TestStruct`1.#ctor");
            var constructorMemberDocumentation = _CreateMemberDocumentationMock("M:CodeMap.Tests.Data.TestStruct`1.#ctor(" + CanonicalNameResolverTests.ConstructorParameters + ")");

            var interfaceShadowedTestEventMemberDocumentation = _CreateMemberDocumentationMock("E:CodeMap.Tests.Data.TestStruct`1.CodeMap#Tests#Data#ITestBaseInterface#InterfaceShadowedTestEvent");
            var staticTestEventMemberDocumentation = _CreateMemberDocumentationMock("E:CodeMap.Tests.Data.TestStruct`1.StaticTestEvent");
            var testEventMemberDocumentation = _CreateMemberDocumentationMock("E:CodeMap.Tests.Data.TestStruct`1.TestEvent");

            var interfaceShadowedTestPropertyMemberDocumentation = _CreateMemberDocumentationMock("P:CodeMap.Tests.Data.TestStruct`1.CodeMap#Tests#Data#ITestBaseInterface#InterfaceShadowedTestProperty");
            var indexerPropertyMemberDocumentation = _CreateMemberDocumentationMock("P:CodeMap.Tests.Data.TestStruct`1.Item(" + CanonicalNameResolverTests.IndexerParameters + ")");
            var testPropertyMemberDocumentation = _CreateMemberDocumentationMock("P:CodeMap.Tests.Data.TestStruct`1.TestProperty");

            var baseTestMethodMemberDocumentation = _CreateMemberDocumentationMock("M:CodeMap.Tests.Data.TestStruct`1.CodeMap#Tests#Data#ITestBaseInterface#BaseTestMethod");
            var interfaceShadowedTestMethod = _CreateMemberDocumentationMock("M:CodeMap.Tests.Data.TestStruct`1.CodeMap#Tests#Data#ITestBaseInterface#InterfaceShadowedTestMethod");
            var shadowingMethodMemberDocumentation = _CreateMemberDocumentationMock("M:CodeMap.Tests.Data.TestStruct`1.GetHashCode");
            var testMethodMemberDocumentation = _CreateMemberDocumentationMock("M:CodeMap.Tests.Data.TestStruct`1.TestMethod``1(" + CanonicalNameResolverTests.MethodParameters + ")");
            var virtualTestMethodMemberDocumentation = _CreateMemberDocumentationMock("M:CodeMap.Tests.Data.TestStruct`1.ToString");
            var membersDocumentation = new MemberDocumentationCollection(
                new[]
                {
                    structMemberDocumentation,

                    testConstantMemberDocumentation,

                    readonlyTestFieldMemberDocumentation,
                    staticTestFieldMemberDocumentation,
                    testFieldMemberDocumentation,

                    defaultConstructorMemberDocumentation,
                    constructorMemberDocumentation,

                    interfaceShadowedTestEventMemberDocumentation,
                    staticTestEventMemberDocumentation,
                    testEventMemberDocumentation,

                    interfaceShadowedTestPropertyMemberDocumentation,
                    indexerPropertyMemberDocumentation,
                    testPropertyMemberDocumentation,

                    baseTestMethodMemberDocumentation,
                    interfaceShadowedTestMethod,
                    shadowingMethodMemberDocumentation,
                    testMethodMemberDocumentation,
                    virtualTestMethodMemberDocumentation
                }
            );

            var typeDocumentationElement = DeclarationNode.Create(typeof(TestStruct<>), membersDocumentation);

            typeDocumentationElement
                .AssertIs<StructDeclaration>(
                    structDocumentationElement =>
                        structDocumentationElement
                            .AssertCollectionMember(
                                () => structDocumentationElement.Constants.OrderBy(constant => constant.Name),
                                constant => constant.AssertDocumentation(testConstantMemberDocumentation)
                            )
                            .AssertCollectionMember(
                                () => structDocumentationElement.Fields.OrderBy(field => field.Name),
                                field => field.AssertDocumentation(readonlyTestFieldMemberDocumentation),
                                field => field.AssertDocumentation(staticTestFieldMemberDocumentation),
                                field => field.AssertDocumentation(testFieldMemberDocumentation)
                            )
                            .AssertCollectionMember(
                                () => structDocumentationElement.Constructors.OrderBy(constructor => constructor.Parameters.Count),
                                constructor => constructor.AssertDocumentation(defaultConstructorMemberDocumentation),
                                constructor => constructor.AssertDocumentation(constructorMemberDocumentation)
                            )
                            .AssertCollectionMember(
                                () => structDocumentationElement.Events.OrderBy(@event => @event.Name),
                                @event => @event.AssertDocumentation(interfaceShadowedTestEventMemberDocumentation),
                                @event => @event.AssertDocumentation(staticTestEventMemberDocumentation),
                                @event => @event.AssertDocumentation(testEventMemberDocumentation)
                            )
                            .AssertCollectionMember(
                                () => structDocumentationElement.Properties.OrderBy(property => property.Name),
                                property => property.AssertDocumentation(interfaceShadowedTestPropertyMemberDocumentation),
                                property => property.AssertDocumentation(indexerPropertyMemberDocumentation),
                                property => property.AssertDocumentation(testPropertyMemberDocumentation)
                            )
                            .AssertCollectionMember(
                                () => structDocumentationElement.Methods.OrderBy(method => method.Name),
                                method => method.AssertDocumentation(baseTestMethodMemberDocumentation),
                                method => method.AssertDocumentation(interfaceShadowedTestMethod),
                                method => method.AssertDocumentation(shadowingMethodMemberDocumentation),
                                method => method.AssertDocumentation(testMethodMemberDocumentation),
                                method => method.AssertDocumentation(virtualTestMethodMemberDocumentation)
                            )
                )
                .AssertDocumentation(structMemberDocumentation);
        }

        [Fact]
        public void CreateStructDocumentationElementNestedTypes()
        {
            var typeDocumentationElement = DeclarationNode.Create(typeof(TestStruct<>));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestStruct")
                .AssertIs<StructDeclaration>(
                    structDocumentationElement => structDocumentationElement
                        .AssertCollectionMember(
                            () => structDocumentationElement.NestedEnums,
                            @enum => @enum
                                .AssertEqual(() => @enum.Name, "NestedTestEnum")
                                .AssertSame(() => @enum.DeclaringType, structDocumentationElement)
                        )
                        .AssertCollectionMember(
                            () => structDocumentationElement.NestedDelegates,
                            @delegate => @delegate
                                .AssertEqual(() => @delegate.Name, "NestedTestDelegate")
                                .AssertSame(() => @delegate.DeclaringType, structDocumentationElement)
                        )
                        .AssertCollectionMember(
                            () => structDocumentationElement.NestedInterfaces,
                            @interface => @interface
                                .AssertEqual(() => @interface.Name, "INestedTestInterface")
                                .AssertSame(() => @interface.DeclaringType, structDocumentationElement)
                        )
                        .AssertCollectionMember(
                            () => structDocumentationElement.NestedClasses,
                            @class => @class
                                .AssertEqual(() => @class.Name, "NestedTestClass")
                                .AssertSame(() => @class.DeclaringType, structDocumentationElement)
                        )
                        .AssertCollectionMember(
                            () => structDocumentationElement.NestedStructs,
                            @struct => @struct
                                .AssertEqual(() => @struct.Name, "NestedTestStruct")
                                .AssertSame(() => @struct.DeclaringType, structDocumentationElement)
                        )
                );
        }

        [Fact]
        public void GenericParameterConstraints()
        {
            var interfaceType = typeof(ITestGenericParameter<,,,,,>);
            var typeGenericParameters = interfaceType.GetGenericArguments();

            var typeDocumentationElement = DeclarationNode.Create(interfaceType);

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "ITestGenericParameter")
                .AssertIs<InterfaceDeclaration>(
                    interfaceDocumentationElement =>
                        interfaceDocumentationElement
                            .AssertCollectionMember(
                                () => interfaceDocumentationElement.GenericParameters,
                                genericParameter =>
                                    genericParameter
                                        .AssertEqual(() => genericParameter.Name, "TParam1")
                                        .AssertEqual(() => genericParameter.Position, 0)
                                        .AssertGenericParameter(typeGenericParameters[0])
                                        .AssertSame(() => genericParameter.DeclaringType, interfaceDocumentationElement)
                                        .AssertTrue(() => genericParameter.IsCovariant)
                                        .AssertFalse(() => genericParameter.IsContravariant)
                                        .AssertFalse(() => genericParameter.HasReferenceTypeConstraint)
                                        .AssertFalse(() => genericParameter.HasNonNullableValueTypeConstraint)
                                        .AssertFalse(() => genericParameter.HasDefaultConstructorConstraint)
                                        .AssertEmpty(() => genericParameter.TypeConstraints),
                                genericParameter =>
                                    genericParameter
                                        .AssertEqual(() => genericParameter.Name, "TParam2")
                                        .AssertEqual(() => genericParameter.Position, 1)
                                        .AssertGenericParameter(typeGenericParameters[1])
                                        .AssertSame(() => genericParameter.DeclaringType, interfaceDocumentationElement)
                                        .AssertFalse(() => genericParameter.IsCovariant)
                                        .AssertTrue(() => genericParameter.IsContravariant)
                                        .AssertTrue(() => genericParameter.HasReferenceTypeConstraint)
                                        .AssertFalse(() => genericParameter.HasNonNullableValueTypeConstraint)
                                        .AssertFalse(() => genericParameter.HasDefaultConstructorConstraint)
                                        .AssertEmpty(() => genericParameter.TypeConstraints),
                                genericParameter =>
                                    genericParameter
                                        .AssertEqual(() => genericParameter.Name, "TParam3")
                                        .AssertEqual(() => genericParameter.Position, 2)
                                        .AssertGenericParameter(typeGenericParameters[2])
                                        .AssertSame(() => genericParameter.DeclaringType, interfaceDocumentationElement)
                                        .AssertFalse(() => genericParameter.IsCovariant)
                                        .AssertFalse(() => genericParameter.IsContravariant)
                                        .AssertFalse(() => genericParameter.HasReferenceTypeConstraint)
                                        .AssertTrue(() => genericParameter.HasNonNullableValueTypeConstraint)
                                        .AssertFalse(() => genericParameter.HasDefaultConstructorConstraint)
                                        .AssertEmpty(() => genericParameter.TypeConstraints),
                                genericParameter =>
                                    genericParameter
                                        .AssertEqual(() => genericParameter.Name, "TParam4")
                                        .AssertEqual(() => genericParameter.Position, 3)
                                        .AssertGenericParameter(typeGenericParameters[3])
                                        .AssertSame(() => genericParameter.DeclaringType, interfaceDocumentationElement)
                                        .AssertFalse(() => genericParameter.IsCovariant)
                                        .AssertFalse(() => genericParameter.IsContravariant)
                                        .AssertFalse(() => genericParameter.HasReferenceTypeConstraint)
                                        .AssertFalse(() => genericParameter.HasNonNullableValueTypeConstraint)
                                        .AssertTrue(() => genericParameter.HasDefaultConstructorConstraint)
                                        .AssertEmpty(() => genericParameter.TypeConstraints),
                                genericParameter =>
                                    genericParameter
                                        .AssertEqual(() => genericParameter.Name, "TParam5")
                                        .AssertEqual(() => genericParameter.Position, 4)
                                        .AssertGenericParameter(typeGenericParameters[4])
                                        .AssertSame(() => genericParameter.DeclaringType, interfaceDocumentationElement)
                                        .AssertFalse(() => genericParameter.IsCovariant)
                                        .AssertFalse(() => genericParameter.IsContravariant)
                                        .AssertFalse(() => genericParameter.HasReferenceTypeConstraint)
                                        .AssertFalse(() => genericParameter.HasNonNullableValueTypeConstraint)
                                        .AssertFalse(() => genericParameter.HasDefaultConstructorConstraint)
                                        .AssertCollectionMember(
                                            () => genericParameter.TypeConstraints,
                                            type => type.AssertIs<GenericTypeParameterReference>(
                                                genericParameterReference => genericParameterReference.AssertEqual(
                                                    () => genericParameterReference.Name,
                                                    interfaceDocumentationElement.GenericParameters.First().Name
                                                )
                                            )
                                        ),
                                genericParameter =>
                                    genericParameter
                                        .AssertEqual(() => genericParameter.Name, "TParam6")
                                        .AssertEqual(() => genericParameter.Position, 5)
                                        .AssertGenericParameter(typeGenericParameters[5])
                                        .AssertSame(() => genericParameter.DeclaringType, interfaceDocumentationElement)
                                        .AssertFalse(() => genericParameter.IsCovariant)
                                        .AssertFalse(() => genericParameter.IsContravariant)
                                        .AssertFalse(() => genericParameter.HasReferenceTypeConstraint)
                                        .AssertFalse(() => genericParameter.HasNonNullableValueTypeConstraint)
                                        .AssertFalse(() => genericParameter.HasDefaultConstructorConstraint)
                                        .AssertCollectionMember(
                                            () => genericParameter.TypeConstraints,
                                            type => type.AssertIs<GenericTypeParameterReference>(
                                                genericParameterReference => genericParameterReference.AssertEqual(
                                                    () => genericParameterReference.Name,
                                                    interfaceDocumentationElement.GenericParameters.First().Name
                                                )
                                            ),
                                            type => type.AssertTypeReference(
                                                typeof(IComparable<>).MakeGenericType(typeGenericParameters[0])
                                            )
                                        )
                            )
                );
        }

        [Fact]
        public void GenericParametersOfNestedClass()
        {
            var classType = typeof(TestClass<>.NestedTestClass<,>);
            var typeGenericParameters = classType.GetGenericArguments();

            var typeDocumentationElement = DeclarationNode.Create(classType);

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "NestedTestClass")
                .AssertIs<ClassDeclaration>(
                    classDocumentationElement =>
                        classDocumentationElement
                            .AssertCollectionMember(
                                () => classDocumentationElement.GenericParameters,
                                genericParameter =>
                                    genericParameter
                                        .AssertEqual(() => genericParameter.Name, "TParam2")
                                        .AssertEqual(() => genericParameter.Position, 0),
                                genericParameter =>
                                    genericParameter
                                        .AssertEqual(() => genericParameter.Name, "TParam3")
                                        .AssertEqual(() => genericParameter.Position, 1)
                            )
                );
        }

        [Fact]
        public void CreateAssemblyDocumentationElement()
        {
            var assemblyDocumentationElement = DeclarationNode.Create(_TestDataAssembly);

            assemblyDocumentationElement
                .AssertEqual(() => assemblyDocumentationElement.Name, "CodeMap.Tests.Data")
                .AssertEqual(() => assemblyDocumentationElement.Version, new Version(1, 2, 3, 4))
                .AssertEqual(() => assemblyDocumentationElement.Culture, string.Empty)
                .AssertEqual(
                    () => assemblyDocumentationElement.PublicKeyToken,
                    string.Join(string.Empty, _TestDataAssembly.GetName().GetPublicKeyToken().Select(@byte => @byte.ToString("x2")))
                )
                .AssertCollectionMember(
                    () => assemblyDocumentationElement.Dependencies,
                    _TestDataAssembly
                        .GetReferencedAssemblies()
                        .OrderBy(dependency => dependency.Name)
                        .Select(referredAssemblyName => new Action<AssemblyReference>(dependency => referredAssemblyName.AssertAssemblyReference(dependency)))
                        .ToArray()
                )
                .AssertCollectionMember(
                    () => assemblyDocumentationElement.Attributes,
                    attribute => attribute.AssertTypeReference(() => attribute.Type, typeof(DebuggableAttribute)),
                    attribute => attribute.AssertTypeReference(() => attribute.Type, typeof(CompilationRelaxationsAttribute)),
                    attribute => attribute.AssertTypeReference(() => attribute.Type, typeof(RuntimeCompatibilityAttribute)),
                    attribute => attribute.AssertTypeReference(() => attribute.Type, typeof(TargetFrameworkAttribute))
                )
                .AssertCollectionMember(
                    () => assemblyDocumentationElement.Namespaces.OrderBy(@namespace => @namespace.Name),
                    @namespace =>
                        @namespace
                            .AssertEqual(() => @namespace.Name, string.Empty)
                            .AssertSame(() => @namespace.Assembly, assemblyDocumentationElement)
                            .AssertEmpty(() => @namespace.Enums)
                            .AssertEmpty(() => @namespace.Delegates)
                            .AssertEmpty(() => @namespace.Interfaces)
                            .AssertCollectionMember(
                                () => @namespace.Classes,
                                @class => @class.AssertType(typeof(GlobalTestClass))
                            )
                            .AssertEmpty(() => @namespace.Structs)
                            .AssertIs<GlobalNamespaceDeclaration>()
                            .AssertNoDocumentation(),
                    @namespace =>
                        @namespace
                            .AssertEqual(() => @namespace.Name, "CodeMap.Tests.Data")
                            .AssertSame(() => @namespace.Assembly, assemblyDocumentationElement)
                            .AssertCollectionMember(
                                () => @namespace.Enums,
                                @enum => @enum.AssertType(typeof(TestEnum))
                            )
                            .AssertCollectionMember(
                                () => @namespace.Delegates,
                                @delegate => @delegate.AssertType(typeof(TestDelegate<>))
                            )
                            .AssertCollectionMember(
                                () => @namespace.Interfaces.OrderBy(@interface => @interface.Name),
                                @interface => @interface.AssertType(typeof(ITestBaseInterface)),
                                @interface => @interface.AssertType(typeof(ITestDocumentation)),
                                @interface => @interface.AssertType(typeof(ITestExplicitInterface)),
                                @interface => @interface.AssertType(typeof(ITestExtendedBaseInterface)),
                                @interface => @interface.AssertType(typeof(ITestGenericParameter<,,,,,>)),
                                @interface => @interface.AssertType(typeof(ITestInterface<>))
                            )
                            .AssertCollectionMember(
                                () => @namespace.Classes.OrderBy(@class => @class.Name),
                                @class => @class.AssertType(typeof(TestAbstractClass)),
                                @class => @class.AssertType(typeof(TestAttribute)),
                                @class => @class.AssertType(typeof(TestBaseClass)),
                                @class => @class
                                    .AssertType(typeof(TestClass<>))
                                    .AssertCollectionMember(
                                        () => @class.NestedEnums,
                                        nestedEnum =>
                                            nestedEnum
                                                .AssertType(typeof(TestClass<>.NestedTestEnum))
                                                .AssertSame(() => nestedEnum.DeclaringType, @class)
                                    )
                                    .AssertCollectionMember(
                                        () => @class.NestedDelegates,
                                        nestedDelegate =>
                                            nestedDelegate
                                                .AssertType(typeof(TestClass<>.NestedTestDelegate))
                                                .AssertSame(() => nestedDelegate.DeclaringType, @class)
                                                .AssertEmpty(() => nestedDelegate.GenericParameters)
                                    )
                                    .AssertCollectionMember(
                                        () => @class.NestedInterfaces,
                                        nestedInterface =>
                                            nestedInterface
                                                .AssertType(typeof(TestClass<>.INestedTestInterface))
                                                .AssertSame(() => nestedInterface.DeclaringType, @class)
                                                .AssertEmpty(() => nestedInterface.GenericParameters)
                                    )
                                    .AssertCollectionMember(
                                        () => @class.NestedClasses,
                                        nestedClass =>
                                            nestedClass
                                                .AssertType(typeof(TestClass<>.NestedTestClass<,>))
                                                .AssertSame(() => nestedClass.DeclaringType, @class)
                                                .AssertEqual(() => nestedClass.GenericParameters.Count, 2)
                                    )
                                    .AssertCollectionMember(
                                        () => @class.NestedStructs,
                                        nestedStruct =>
                                            nestedStruct
                                                .AssertType(typeof(TestClass<>.NestedTestStruct))
                                                .AssertSame(() => nestedStruct.DeclaringType, @class)
                                                .AssertEmpty(() => nestedStruct.GenericParameters)
                                    ),
                                @class => @class.AssertType(typeof(TestExplicitClass)),
                                @class => @class.AssertType(typeof(TestSealedClass)),
                                @class => @class.AssertType(typeof(TestStaticClass))
                            )
                            .AssertCollectionMember(
                                () => @namespace.Structs,
                                @struct => @struct.AssertType(typeof(TestStruct<>))
                            )
                            .AssertNoDocumentation()
                )
                .AssertNoDocumentation();
        }

        [Fact]
        public void ApplyDocumentationAddition()
        {
            var assemblyDocumentationElement = DeclarationNode.Create(_TestDataAssembly);
            var summary = DocumentationElement.Summary();
            var remakrs = DocumentationElement.Remarks();
            var examples = new List<ExampleDocumentationElement>();
            var relatedMembers = new List<MemberReferenceDocumentationElement>();

            assemblyDocumentationElement.Apply(
                new AssemblyDocumentationAdditionMock
                {
                    Summary = summary,
                    Remarks = remakrs,
                    Examples = examples,
                    RelatedMembers = relatedMembers,
                    NamespaceAdditions = new[]
                    {
                        new NamespaceDocumentationAdditionMock
                        {
                            Summary = summary,
                            Remarks = remakrs,
                            Examples = examples,
                            RelatedMembers = relatedMembers
                        }
                    }
                });

            Assert.Same(summary, assemblyDocumentationElement.Summary);
            Assert.Same(remakrs, assemblyDocumentationElement.Remarks);
            Assert.Same(examples, assemblyDocumentationElement.Examples);
            Assert.Same(relatedMembers, assemblyDocumentationElement.RelatedMembers);
            foreach (var namespaceDeclaration in assemblyDocumentationElement.Namespaces)
            {
                Assert.Same(summary, namespaceDeclaration.Summary);
                Assert.Same(remakrs, namespaceDeclaration.Remarks);
                Assert.Same(examples, namespaceDeclaration.Examples);
                Assert.Same(relatedMembers, namespaceDeclaration.RelatedMembers);
            }
        }

        [Fact]
        public void AssertTypeReferenceToNestedGenericType()
        {
            var classDocumentationElement = (ClassDeclaration)DeclarationNode.Create(typeof(TestClass<>));
            var typeReference = (TypeReference)classDocumentationElement
                .Methods
                .Single(method => method.Name == "TestMethod")
                .Parameters
                .Single(parameter => parameter.Name == "param13")
                .Type;

            typeReference
                .AssertEqual(() => typeReference.Name, "NestedTestClass")
                .AssertEqual(() => typeReference.Namespace, "CodeMap.Tests.Data")
                .AssertCollectionMember(
                    () => typeReference.GenericArguments,
                    genericArgument => genericArgument.AssertTypeReference(typeof(byte[])),
                    genericArgument => genericArgument.AssertTypeReference(typeof(IEnumerable<string>))
                )
                .AssertMember(
                    () => typeReference.DeclaringType,
                    declaringType =>
                        declaringType
                            .AssertEqual(() => declaringType.Name, "TestClass")
                            .AssertEqual(() => declaringType.Namespace, "CodeMap.Tests.Data")
                        .AssertCollectionMember(
                            () => declaringType.GenericArguments,
                            genericArgument => genericArgument.AssertTypeReference(typeof(int))
                        )
                );
        }

        [Fact]
        public void AssemblyDocumentationElementCallVisitorMethods()
        {
            var assemblyDocumentationElement = DeclarationNode.Create(_TestDataAssembly);
            _VerifyVisitor(
                assemblyDocumentationElement,
                visitor => visitor.VisitAssembly(assemblyDocumentationElement)
            );
        }

        [Fact]
        public void NamespaceDocumentationElementCallVisitorMethods()
        {
            var namespaceDocumentationElement = DeclarationNode.Create(_TestDataAssembly).Namespaces.Single(@namespace => @namespace.Name == "CodeMap.Tests.Data");
            _VerifyVisitor(
                namespaceDocumentationElement,
                visitor => visitor.VisitNamespace(namespaceDocumentationElement)
            );
        }

        [Fact]
        public void EnumDocumentationElementCallVisitorMethods()
        {
            var enumDocumentationElement = (EnumDeclaration)DeclarationNode.Create(typeof(TestEnum));
            _VerifyVisitor(
                enumDocumentationElement,
                visitor => visitor.VisitEnum(enumDocumentationElement)
            );
        }

        [Fact]
        public void DelegateDocumentationElementCallVisitorMethods()
        {
            var delegateDocumentationElement = (DelegateDeclaration)DeclarationNode.Create(typeof(TestDelegate<>));
            _VerifyVisitor(
                delegateDocumentationElement,
                visitor => visitor.VisitDelegate(delegateDocumentationElement)
            );
        }

        [Fact]
        public void InterfaceDocumentationElementCallVisitorMethods()
        {
            var interfaceDocumentationElement = (InterfaceDeclaration)DeclarationNode.Create(typeof(ITestInterface<>));
            _VerifyVisitor(
                interfaceDocumentationElement,
                visitor => visitor.VisitInterface(interfaceDocumentationElement)
            );
        }

        [Fact]
        public void ClassDocumentationElementCallVisitorMethods()
        {
            var classDocumentationElement = (ClassDeclaration)DeclarationNode.Create(typeof(TestClass<>));
            _VerifyVisitor(
                classDocumentationElement,
                visitor => visitor.VisitClass(classDocumentationElement)
            );
        }

        [Fact]
        public void StructDocumentationElementCallVisitorMethods()
        {
            var structDocumentationElement = (StructDeclaration)DeclarationNode.Create(typeof(TestStruct<>));
            _VerifyVisitor(
                structDocumentationElement,
                visitor => visitor.VisitStruct(structDocumentationElement)
            );
        }

        [Fact]
        public void ConstantDocumentationElementCallVisitorMethods()
        {
            var classDocumentationElement = (ClassDeclaration)DeclarationNode.Create(typeof(TestClass<>));
            var constantDocumentationElement = classDocumentationElement.Constants.Single(constant => constant.Name == "TestConstant");
            _VerifyVisitor(
                constantDocumentationElement,
                visitor => visitor.VisitConstant(constantDocumentationElement)
            );
        }

        [Fact]
        public void FieldDocumentationElementCallVisitorMethods()
        {
            var classDocumentationElement = (ClassDeclaration)DeclarationNode.Create(typeof(TestClass<>));
            var fieldDocumentationElement = classDocumentationElement.Fields.Single(field => field.Name == "TestField");
            _VerifyVisitor(
                fieldDocumentationElement,
                visitor => visitor.VisitField(fieldDocumentationElement)
            );
        }

        [Fact]
        public void ConstructorDocumentationElementCallVisitorMethods()
        {
            var classDocumentationElement = (ClassDeclaration)DeclarationNode.Create(typeof(TestClass<>));
            var constructorDocumentationElement = classDocumentationElement.Constructors.Single(constructor => constructor.Name == "TestClass");
            _VerifyVisitor(
                constructorDocumentationElement,
                visitor => visitor.VisitConstructor(constructorDocumentationElement)
            );
        }

        [Fact]
        public void EventDocumentationElementCallVisitorMethods()
        {
            var classDocumentationElement = (ClassDeclaration)DeclarationNode.Create(typeof(TestClass<>));
            var eventDocumentationElement = classDocumentationElement.Events.Single(@event => @event.Name == "TestEvent");
            _VerifyVisitor(
                eventDocumentationElement,
                visitor => visitor.VisitEvent(eventDocumentationElement)
            );
        }

        [Fact]
        public void PropertyDocumentationElementCallVisitorMethods()
        {
            var classDocumentationElement = (ClassDeclaration)DeclarationNode.Create(typeof(TestClass<>));
            var propertyDocumentationElement = classDocumentationElement.Properties.Single(property => property.Name == "TestProperty");
            _VerifyVisitor(
                propertyDocumentationElement,
                visitor => visitor.VisitProperty(propertyDocumentationElement)
            );
        }

        [Fact]
        public void MethodDocumentationElementCallVisitorMethods()
        {
            var classDocumentationElement = (ClassDeclaration)DeclarationNode.Create(typeof(TestClass<>));
            var methodDocumentationElement = classDocumentationElement.Methods.Single(method => method.Name == "TestMethod");
            _VerifyVisitor(
                methodDocumentationElement,
                visitor => visitor.VisitMethod(methodDocumentationElement)
            );
        }

        [Fact]
        public void CreateFromNullAssemblyThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => DeclarationNode.Create(assembly: null));
            Assert.Equal(new ArgumentNullException("assembly").Message, exception.Message);
        }

        [Fact]
        public void CreateFromAssemblyAndNullMembersDocumentationThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => DeclarationNode.Create(typeof(object).Assembly, null));
            Assert.Equal(new ArgumentNullException("membersDocumentation").Message, exception.Message);
        }

        [Fact]
        public void CreateFromNullTypeThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => DeclarationNode.Create(type: null));
            Assert.Equal(new ArgumentNullException("type").Message, exception.Message);
        }

        [Fact]
        public void CreateFromTypeAndNullMembersDocumentationThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => DeclarationNode.Create(typeof(object), null));
            Assert.Equal(new ArgumentNullException("membersDocumentation").Message, exception.Message);
        }

        private static void _VerifyVisitor<TDocumentationElement>(TDocumentationElement documentationElement, Expression<Action<IDeclarationNodeVisitor>> verifyExpression)
            where TDocumentationElement : DeclarationNode
        {
            var visitorMock = new Mock<IDeclarationNodeVisitor>();
            documentationElement.Accept(new DeclarationNodeVisitorAdapter(visitorMock.Object));
            visitorMock.Verify(verifyExpression, Times.Once());
            visitorMock.VerifyNoOtherCalls();
        }

        private static MemberDocumentation _CreateMemberDocumentationMock(string canonicalName)
            => new MemberDocumentation(
                canonicalName,
                DocumentationElement.Summary(DocumentationElement.Paragraph()),
                Enumerable
                    .Range(1, 6)
                    .Select(genericParameterNumber => $"TParam{genericParameterNumber}")
                    .Concat(
                        Enumerable
                            .Range(1, 1)
                            .Select(genericParameterNumber => $"TMethodParam{genericParameterNumber}")
                        )
                    .ToDictionary(
                        genericParameter => genericParameter,
                        genericParameter => DocumentationElement.BlockDescription(new[] { DocumentationElement.Paragraph() }),
                        StringComparer.Ordinal
                    ),
                Enumerable
                    .Range(1, 42)
                    .Select(parameterNumber => $"param{parameterNumber}")
                    .ToDictionary(
                        parameter => parameter,
                        parameter => DocumentationElement.BlockDescription(new[] { DocumentationElement.Paragraph() }),
                        StringComparer.Ordinal
                    ),
                DocumentationElement.BlockDescription(new[] { DocumentationElement.Paragraph() }),
                new[] { "T:System.ArgumentException", "T:System.ArgumentNullException" }
                    .ToDictionary(
                        exception => exception,
                        exception => DocumentationElement.BlockDescription(new[] { DocumentationElement.Paragraph() }),
                        StringComparer.Ordinal
                    ),
                DocumentationElement.Remarks(DocumentationElement.Paragraph()),
                new[] { DocumentationElement.Example(DocumentationElement.Paragraph()) },
                DocumentationElement.Value(DocumentationElement.Paragraph()),
                new[] { DocumentationElement.MemberReference(typeof(object)) }
            );
    }
}