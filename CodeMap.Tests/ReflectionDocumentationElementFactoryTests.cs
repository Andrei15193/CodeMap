using System;
using System.Linq;
using System.Reflection;
using CodeMap.Elements;
using Xunit;
using static CodeMap.Tests.AssertDocumentationElement;

namespace CodeMap.Tests
{
    public class ReflectionDocumentationElementFactoryTests
    {
        private readonly ReflectionDocumentationElementFactory _factory = new ReflectionDocumentationElementFactory();

        [Fact]
        public void CreatingEnumDocumentationElement()
        {
            var enumDocumentationElement = _factory.Create(typeof(TestEnum)) as EnumDocumentationElement;

            AssertTypeDefinition(typeof(TestEnum), enumDocumentationElement);
            AssertTypeReference(typeof(TestEnum).GetEnumUnderlyingType(), enumDocumentationElement.UnderlyingType);
            AssertConstantDefinitions(
                enumDocumentationElement,
                typeof(TestEnum)
                    .GetRuntimeFields()
                    .Where(fieldInfo => Enum.TryParse<TestEnum>(fieldInfo.Name, out var value)),
                enumDocumentationElement.Members
            );
        }

        [Fact]
        public void CreatingDelegateDocumentationElement()
        {
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