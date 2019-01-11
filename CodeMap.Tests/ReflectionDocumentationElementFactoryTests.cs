using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var factory = new ReflectionDocumentationElementFactory(
                new MemberDocumentationCollection(
                    new[]
                    {
                        new MemberDocumentation(
                            "T:CodeMap.TestEnum",
                            _summaryDocumentationElement,
                            null,
                            null,
                            null,
                            null,
                            _remarksDocumentationElement,
                            _exampleDocumentationElements,
                            null,
                            _relatedMembersList
                        )
                    }
                )
            );

            var enumDocumentationElement = factory.Create(typeof(TestEnum)) as EnumDocumentationElement;

            AssertTypeDefinition(typeof(TestEnum), enumDocumentationElement);
            AssertTypeReference(typeof(TestEnum).GetEnumUnderlyingType(), enumDocumentationElement.UnderlyingType);
            AssertConstantDefinitions(
                enumDocumentationElement,
                typeof(TestEnum)
                    .GetRuntimeFields()
                    .Where(fieldInfo => Enum.TryParse<TestEnum>(fieldInfo.Name, out var value)),
                enumDocumentationElement.Members
            );
            Assert.Same(_summaryDocumentationElement, enumDocumentationElement.Summary);
            Assert.Same(_remarksDocumentationElement, enumDocumentationElement.Remarks);
            Assert.Same(_exampleDocumentationElements, enumDocumentationElement.Examples);
            Assert.Same(_relatedMembersList, enumDocumentationElement.RelatedMembers);
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
    }
}