using System;
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
                .AssertCollectionMember(
                    () => typeDocumentationElement.Attributes,
                    attribute => attribute.AssertTestAttribute("enum")
                )
                .AssertIs<EnumDocumentationElement>(
                    enumDocumentationElement => enumDocumentationElement
                        .AssertType(() => enumDocumentationElement.UnderlyingType, typeof(byte))
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
            var genericParameterType = delegateType.GetGenericArguments().Single();
            var typeDocumentationElement = _factory.Create(delegateType);

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestDelegate")
                .AssertEqual(() => typeDocumentationElement.AccessModifier, AccessModifier.Public)
                .AssertNull(() => typeDocumentationElement.DeclaringType)
                .AssertCollectionMember(
                    () => typeDocumentationElement.Attributes,
                    attribute => attribute.AssertTestAttribute("delegate")
                )
                .AssertIs<DelegateDocumentationElement>(
                    delegateDocumentationElement => delegateDocumentationElement
                        .AssertType(() => delegateDocumentationElement.Return.Type, typeof(void))
                        .AssertMember(
                            () => delegateDocumentationElement.Return.Type,
                            returnType => returnType.AssertIs<VoidTypeReferenceDocumentationElement>()
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
            var _factory = new ReflectionDocumentationElementFactory(
                new MemberDocumentationCollection(
                    new[]
                    {
                        memberDocumentation
                    }
                )
            );

            var delegateType = typeof(TestDelegate<>);
            var typeDocumentationElement = _factory.Create(delegateType);

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestDelegate")
                .AssertIs<DelegateDocumentationElement>()
                .AssertDocumentation(memberDocumentation);
        }

        [Fact]
        public void CreateInterfaceDocumentationElement()
        {
            var interfaceType = typeof(ITestInterface<>);
            var typeGenericParameter = interfaceType.GetGenericArguments().Single();
            var methodGenericParameter = interfaceType.GetMethod("TestMethod").GetGenericArguments().Single();
            var factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = factory.Create(interfaceType);

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "ITestInterface")
                .AssertEqual(() => typeDocumentationElement.AccessModifier, AccessModifier.Public)
                .AssertNull(() => typeDocumentationElement.DeclaringType)
                .AssertCollectionMember(
                    () => typeDocumentationElement.Attributes,
                    attribute => attribute.AssertDefaultMemberAttribute(),
                    attribute => attribute.AssertTestAttribute("interface")
                )
                .AssertIs<InterfaceDocumentationElement>(
                    interfaceDocumentationElement => interfaceDocumentationElement
                        .AssertTypeGenericParameters(() => interfaceDocumentationElement.GenericParameters)
                        .AssertCollectionMember(
                            () => interfaceDocumentationElement.BaseInterfaces,
                            baseInterface => baseInterface.AssertType(typeof(ITestExtendedBaseInterface))
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
            var factory = new ReflectionDocumentationElementFactory(
                new MemberDocumentationCollection(
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
                )
            );

            var typeDocumentationElement = factory.Create(typeof(ITestInterface<>));

            typeDocumentationElement
                .AssertIs<InterfaceDocumentationElement>(
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
            var factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = factory.Create(classType);
            var baseTypeDocumentationElement = factory.Create(typeof(TestBaseClass));

            baseTypeDocumentationElement
                .AssertEqual(() => baseTypeDocumentationElement.Name, "TestBaseClass")
                .AssertIs<ClassDocumentationElement>(
                    baseClassDocumentationElement => baseClassDocumentationElement
                        .AssertType(() => baseClassDocumentationElement.BaseClass, typeof(object))
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
                .AssertEqual(() => typeDocumentationElement.AccessModifier, AccessModifier.Public)
                .AssertNull(() => typeDocumentationElement.DeclaringType)
                .AssertCollectionMember(
                    () => typeDocumentationElement.Attributes,
                    attribute => attribute.AssertDefaultMemberAttribute(),
                    attribute => attribute.AssertTestAttribute("class")
                )
                .AssertIs<ClassDocumentationElement>(
                    classDocumentationElement => classDocumentationElement
                        .AssertFalse(() => classDocumentationElement.IsAbstract)
                        .AssertFalse(() => classDocumentationElement.IsSealed)
                        .AssertFalse(() => classDocumentationElement.IsStatic)
                        .AssertTypeGenericParameters(() => classDocumentationElement.GenericParameters)
                        .AssertType(() => classDocumentationElement.BaseClass, typeof(TestBaseClass))
                        .AssertCollectionMember(
                            () => classDocumentationElement.ImplementedInterfaces,
                            baseInterface => baseInterface.AssertType(typeof(ITestExtendedBaseInterface))
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
                                .AssertType(() => constant.Type, typeof(double))
                                .AssertEqual(() => constant.Value, 1.0)
                                .AssertNoDocumentation()
                        )
                        .AssertTestFields(() => classDocumentationElement.Fields)
                        .AssertTestEvent(() => classDocumentationElement.Events, "TestEvent", "class event")

                        .AssertCollectionMember(
                            () => classDocumentationElement.Constructors,
                            constructor => constructor.AssertTestConstructor(classDocumentationElement, typeGenericParameter, "class constructor")
                        )

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

            var factory = new ReflectionDocumentationElementFactory(
                new MemberDocumentationCollection(
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
                )
            );

            var typeDocumentationElement = factory.Create(typeof(TestClass<>));

            typeDocumentationElement
                .AssertIs<ClassDocumentationElement>(
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
                                field => field.AssertDocumentation(constructorMemberDocumentation)
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
            var factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = factory.Create(typeof(TestAbstractClass));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestAbstractClass")
                .AssertIs<ClassDocumentationElement>(
                    classDocumentationElement => classDocumentationElement
                        .AssertTrue(() => classDocumentationElement.IsAbstract)
                        .AssertFalse(() => classDocumentationElement.IsSealed)
                        .AssertFalse(() => classDocumentationElement.IsStatic)
                );
        }

        [Fact]
        public void CreateClassDocumentationElementForSealedClass()
        {
            var factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = factory.Create(typeof(TestSealedClass));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestSealedClass")
                .AssertIs<ClassDocumentationElement>(
                    classDocumentationElement => classDocumentationElement
                        .AssertFalse(() => classDocumentationElement.IsAbstract)
                        .AssertTrue(() => classDocumentationElement.IsSealed)
                        .AssertFalse(() => classDocumentationElement.IsStatic)
                );
        }

        [Fact]
        public void CreateClassDocumentationElementForStaticClass()
        {
            var factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = factory.Create(typeof(TestStaticClass));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestStaticClass")
                .AssertIs<ClassDocumentationElement>(
                    classDocumentationElement => classDocumentationElement
                        .AssertFalse(() => classDocumentationElement.IsAbstract)
                        .AssertFalse(() => classDocumentationElement.IsSealed)
                        .AssertTrue(() => classDocumentationElement.IsStatic)
                );
        }

        [Fact]
        public void CreateClassDocumentationElementNestedTypes()
        {
            var factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = factory.Create(typeof(TestClass<>));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestClass")
                .AssertIs<ClassDocumentationElement>(
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
                );
        }

        [Fact]
        public void ConstructorWithNullMembersDocumentationCollectionThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new ReflectionDocumentationElementFactory(null));
            Assert.Equal(new ArgumentNullException("membersDocumentation").Message, exception.Message);
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