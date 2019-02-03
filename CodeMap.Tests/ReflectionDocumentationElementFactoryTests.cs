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