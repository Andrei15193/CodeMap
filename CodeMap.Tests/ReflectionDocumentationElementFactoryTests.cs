using System;
using System.Collections.Generic;
using System.Linq;
using CodeMap.Elements;
using Xunit;
using static CodeMap.Tests.AssertDocumentationElement;

namespace CodeMap.Tests
{
    public class ReflectionDocumentationElementFactoryTests
    {
        private readonly SummaryDocumentationElement _summaryDocumentationElement = DocumentationElement.Summary();
        private readonly RemarksDocumentationElement _remarksDocumentationElement = DocumentationElement.Remarks();
        private readonly IReadOnlyCollection<ExampleDocumentationElement> _exampleDocumentationElements = new[] { DocumentationElement.Example() };
        private readonly RelatedMembersList _relatedMembersList = DocumentationElement.RelatedMembersList(Enumerable.Empty<MemberReferenceDocumentationElement>());

        [Fact]
        public void CreatingEnumDocumentationElement()
        {
            var factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = factory.Create(typeof(TestEnum));

            typeDocumentationElement
                .AssertEquals(() => typeDocumentationElement.Name, "TestEnum")
                .AssertEquals(() => typeDocumentationElement.AccessModifier, AccessModifier.Assembly)
                .AssertNull(() => typeDocumentationElement.DeclaringType)
                .AssertNull(() => typeDocumentationElement.Summary)
                .AssertNull(() => typeDocumentationElement.Remarks)
                .AssertNull(() => typeDocumentationElement.Examples)
                .AssertNull(() => typeDocumentationElement.RelatedMembers)
                .AssertIs<EnumDocumentationElement>(
                    enumDocumentationElement => enumDocumentationElement
                        .AssertTypeReference(() => enumDocumentationElement.UnderlyingType, "System", "Byte")
                        .AssertTypeReferenceAssembly(() => enumDocumentationElement.UnderlyingType, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                );
        }

        [Fact]
        public void CreatingEnumDocumentationElement_Documentation()
        {
            var enumDocumentationMember = _CreateMemberDocumentationMock("T:CodeMap.Tests.TestEnum");
            var factory = new ReflectionDocumentationElementFactory(
                new MemberDocumentationCollection(new[] { enumDocumentationMember })
            );

            var typeDocumentationElement = factory.Create(typeof(TestEnum));

            typeDocumentationElement
                .AssertSame(() => typeDocumentationElement.Summary, enumDocumentationMember.Summary)
                .AssertSame(() => typeDocumentationElement.Remarks, enumDocumentationMember.Remarks)
                .AssertSame(() => typeDocumentationElement.Examples, enumDocumentationMember.Examples)
                .AssertSame(() => typeDocumentationElement.RelatedMembers, enumDocumentationMember.RelatedMembers);
        }

        [Fact]
        public void CreatingEnumDocumentationElement_Attributes()
        {
            var factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = factory.Create(typeof(TestEnum));

            typeDocumentationElement
                .AssertCollectionMember(
                    () => typeDocumentationElement.Attributes,
                    attribute => attribute
                        .AssertTypeReference(() => attribute.Type, "CodeMap.Tests", "TestAttribute")
                        .AssertTypeReferenceAssembly(() => attribute.Type, "CodeMap.Tests", new Version(1, 2, 3, 4))
                        .AssertCollectionMember(
                            () => attribute.PositionalParameters,
                            positionalParameter => positionalParameter
                                .AssertEquals(() => positionalParameter.Name, "value1")
                                .AssertEquals(() => positionalParameter.Value, "test 1")
                                .AssertTypeReference(() => positionalParameter.Type, "System", "Object")
                                .AssertTypeReferenceAssembly(() => positionalParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                        )
                        .AssertCollectionMember(
                            () => attribute.NamedParameters.OrderBy(namedParameter => namedParameter.Name),
                            namedParameter => namedParameter
                                .AssertEquals(() => namedParameter.Name, "Value2")
                                .AssertEquals(() => namedParameter.Value, "test 2")
                                .AssertTypeReference(() => namedParameter.Type, "System", "Object")
                                .AssertTypeReferenceAssembly(() => namedParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0)),
                            namedParameter => namedParameter
                                .AssertEquals(() => namedParameter.Name, "Value3")
                                .AssertEquals(() => namedParameter.Value, "test 3")
                                .AssertTypeReference(() => namedParameter.Type, "System", "Object")
                                .AssertTypeReferenceAssembly(() => namedParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                        )
                );
        }

        [Fact]
        public void CreateEnumDocumentationElement_Members()
        {
            var factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = factory.Create(typeof(TestEnum));

            typeDocumentationElement
                .AssertIs<EnumDocumentationElement>(
                    enumDocumentationElement => enumDocumentationElement
                        .AssertCollectionMember(
                            () => enumDocumentationElement.Members,
                            enumMember => enumMember
                                .AssertEquals(() => enumMember.Name, "TestMember1")
                                .AssertEquals(() => enumMember.AccessModifier, AccessModifier.Public)
                                .AssertSame(() => enumMember.DeclaringType, enumDocumentationElement)
                                .AssertTypeReference(() => enumMember.Type, "CodeMap.Tests", "TestEnum")
                                .AssertTypeReferenceAssembly(() => enumMember.Type, "CodeMap.Tests", new Version(1, 2, 3, 4))
                                .AssertEquals(() => enumMember.Value, TestEnum.TestMember1)
                                .AssertNull(() => enumMember.Summary)
                                .AssertNull(() => enumMember.Remarks)
                                .AssertNull(() => enumMember.Examples)
                                .AssertNull(() => enumMember.RelatedMembers),
                            enumMember => enumMember
                                .AssertEquals(() => enumMember.Name, "TestMember2")
                                .AssertEquals(() => enumMember.AccessModifier, AccessModifier.Public)
                                .AssertSame(() => enumMember.DeclaringType, enumDocumentationElement)
                                .AssertTypeReference(() => enumMember.Type, "CodeMap.Tests", "TestEnum")
                                .AssertTypeReferenceAssembly(() => enumMember.Type, "CodeMap.Tests", new Version(1, 2, 3, 4))
                                .AssertEquals(() => enumMember.Value, TestEnum.TestMember2)
                                .AssertNull(() => enumMember.Summary)
                                .AssertNull(() => enumMember.Remarks)
                                .AssertNull(() => enumMember.Examples)
                                .AssertNull(() => enumMember.RelatedMembers),
                            enumMember => enumMember
                                .AssertEquals(() => enumMember.Name, "TestMember3")
                                .AssertEquals(() => enumMember.AccessModifier, AccessModifier.Public)
                                .AssertSame(() => enumMember.DeclaringType, enumDocumentationElement)
                                .AssertTypeReference(() => enumMember.Type, "CodeMap.Tests", "TestEnum")
                                .AssertTypeReferenceAssembly(() => enumMember.Type, "CodeMap.Tests", new Version(1, 2, 3, 4))
                                .AssertEquals(() => enumMember.Value, TestEnum.TestMember3)
                                .AssertNull(() => enumMember.Summary)
                                .AssertNull(() => enumMember.Remarks)
                                .AssertNull(() => enumMember.Examples)
                                .AssertNull(() => enumMember.RelatedMembers)
                        )
                );
        }

        [Fact]
        public void CreateEnumDocumentationElement_MemberAttributes()
        {
            var factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = factory.Create(typeof(TestEnum));

            typeDocumentationElement
                .AssertIs<EnumDocumentationElement>(
                    enumDocumentationElement => enumDocumentationElement
                        .AssertCollectionMember(
                            () => enumDocumentationElement.Members,
                            enumMember => enumMember
                                .AssertEquals(() => enumMember.Name, "TestMember1")
                                .AssertCollectionMember(
                                    () => enumMember.Attributes,
                                    attribute => attribute
                                        .AssertTypeReference(() => attribute.Type, "CodeMap.Tests", "TestAttribute")
                                        .AssertTypeReferenceAssembly(() => attribute.Type, "CodeMap.Tests", new Version(1, 2, 3, 4))
                                        .AssertCollectionMember(
                                            () => attribute.PositionalParameters,
                                            positionalParameter => positionalParameter
                                                .AssertEquals(() => positionalParameter.Name, "value1")
                                                .AssertEquals(() => positionalParameter.Value, "member test 1")
                                                .AssertTypeReference(() => positionalParameter.Type, "System", "Object")
                                                .AssertTypeReferenceAssembly(() => positionalParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                        )
                                        .AssertCollectionMember(
                                            () => attribute.NamedParameters.OrderBy(namedParameter => namedParameter.Name),
                                            namedParameter => namedParameter
                                                .AssertEquals(() => namedParameter.Name, "Value2")
                                                .AssertEquals(() => namedParameter.Value, "member test 2")
                                                .AssertTypeReference(() => namedParameter.Type, "System", "Object")
                                                .AssertTypeReferenceAssembly(() => namedParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0)),
                                            namedParameter => namedParameter
                                                .AssertEquals(() => namedParameter.Name, "Value3")
                                                .AssertEquals(() => namedParameter.Value, "member test 3")
                                                .AssertTypeReference(() => namedParameter.Type, "System", "Object")
                                                .AssertTypeReferenceAssembly(() => namedParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                        )
                                ),
                            enumMember => enumMember
                                .AssertEquals(() => enumMember.Name, "TestMember2")
                                .AssertEmpty(() => enumMember.Attributes),
                            enumMember => enumMember
                                .AssertEquals(() => enumMember.Name, "TestMember3")
                                .AssertEmpty(() => enumMember.Attributes)
                        )
                );
        }

        [Fact]
        public void CreatingEnumDocumentationElement_MemberDocumentation()
        {
            var enumMemberDocumentationMember = _CreateMemberDocumentationMock("F:CodeMap.Tests.TestEnum.TestMember1");
            var factory = new ReflectionDocumentationElementFactory(
                new MemberDocumentationCollection(new[] { enumMemberDocumentationMember })
            );

            var typeDocumentationElement = factory.Create(typeof(TestEnum));

            typeDocumentationElement
                .AssertIs<EnumDocumentationElement>(
                    enumDocumentationElement => enumDocumentationElement
                        .AssertTypeReference(() => enumDocumentationElement.UnderlyingType, "System", "Byte")
                        .AssertTypeReferenceAssembly(() => enumDocumentationElement.UnderlyingType, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                        .AssertCollectionMember(
                            () => enumDocumentationElement.Members,
                            enumMember => enumMember
                                .AssertEquals(() => enumMember.Name, "TestMember1")
                                .AssertSame(() => enumMember.Summary, enumMemberDocumentationMember.Summary)
                                .AssertSame(() => enumMember.Remarks, enumMemberDocumentationMember.Remarks)
                                .AssertSame(() => enumMember.Examples, enumMemberDocumentationMember.Examples)
                                .AssertSame(() => enumMember.RelatedMembers, enumMemberDocumentationMember.RelatedMembers),
                            enumMember => enumMember
                                .AssertEquals(() => enumMember.Name, "TestMember2")
                                .AssertNull(() => enumMember.Summary)
                                .AssertNull(() => enumMember.Remarks)
                                .AssertNull(() => enumMember.Examples)
                                .AssertNull(() => enumMember.RelatedMembers),
                            enumMember => enumMember
                                .AssertEquals(() => enumMember.Name, "TestMember3")
                                .AssertNull(() => enumMember.Summary)
                                .AssertNull(() => enumMember.Remarks)
                                .AssertNull(() => enumMember.Examples)
                                .AssertNull(() => enumMember.RelatedMembers)
                        )
                );
        }

        [Fact]
        public void CreatingDelegateDocumentationElement()
        {
            var _factory = new ReflectionDocumentationElementFactory();

            var delegateDocumentationElement = _factory.Create(typeof(TestDelegate<,,>)) as DelegateDocumentationElement;

            AssertTypeDefinition(typeof(TestDelegate<,,>), delegateDocumentationElement);
            AssertGenericParameters(typeof(TestDelegate<,,>).GetGenericArguments(), delegateDocumentationElement.GenericParameters);
            AssertParameters(
                typeof(TestDelegate<,,>).GetMethod(nameof(Action.Invoke)).GetParameters(),
                delegateDocumentationElement.Parameters
            );
            AssertTypeReference(typeof(void), delegateDocumentationElement.Return.Type);
            AssertAttributes(
                typeof(TestDelegate<,,>)
                    .GetMethod(nameof(Action.Invoke))
                    .ReturnParameter
                    .GetCustomAttributesData(),
                delegateDocumentationElement.Return.Attributes
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
                DocumentationElement.Summary(),
                genericParameters?.ToLookup(
                    genericParameter => genericParameter,
                    genericParameter => DocumentationElement.Paragraph() as BlockDocumentationElement
                ),
                parameters?.ToLookup(
                    parameter => parameter,
                    parameter => DocumentationElement.Paragraph() as BlockDocumentationElement
                ),
                Enumerable.Empty<BlockDocumentationElement>(),
                exceptions?.ToLookup(
                    exception => exception,
                    exception => DocumentationElement.Paragraph() as BlockDocumentationElement
                ),
                DocumentationElement.Remarks(),
                new[] { DocumentationElement.Example() },
                DocumentationElement.Value(),
                DocumentationElement.RelatedMembersList()
            );
    }
}