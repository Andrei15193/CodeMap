using System;
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

            var typeDocumentationElement = factory.Create(typeof(ITestInterface<>));

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