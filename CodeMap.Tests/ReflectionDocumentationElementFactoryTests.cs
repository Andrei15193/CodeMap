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
        [Fact]
        public void CreatingEnumDocumentationNode()
        {
            var factory = new ReflectionDocumentationElementFactory();

            var enumDocumentationElement = factory.Create(typeof(CallingConventions)) as EnumDocumentationElement;

            AssertTypeDefinition(typeof(CallingConventions), enumDocumentationElement);
            AssertConstantDefinitions(
                enumDocumentationElement,
                typeof(CallingConventions)
                    .GetRuntimeFields()
                    .Where(fieldInfo => Enum.TryParse<CallingConventions>(fieldInfo.Name, out var value)),
                enumDocumentationElement.Members
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