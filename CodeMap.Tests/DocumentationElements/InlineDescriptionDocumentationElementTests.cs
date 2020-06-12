using CodeMap.DocumentationElements;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class InlineDescriptionDocumentationElementTests
    {
        [Fact]
        public void CreateInlineElementCollection()
        {
            var inlineElements = new InlineDocumentationElement[]
            {
                DocumentationElement.Text(string.Empty),
                DocumentationElement.InlineCode(string.Empty),
                DocumentationElement.ParameterReference(string.Empty),
                DocumentationElement.GenericParameterReference(string.Empty),
                DocumentationElement.MemberReference("member"),
                DocumentationElement.MemberReference(typeof(string))
            };

            var inlineElementCollection = DocumentationElement.InlineDescription(inlineElements);

            Assert.Equal(6, inlineElementCollection.Count);
            for (var index = 0; index < inlineElementCollection.Count; index++)
                Assert.Same(inlineElements[index], inlineElementCollection[index]);
            foreach (var (expected, actual) in inlineElements.Zip(inlineElementCollection, (expected, actual) => (expected, actual)))
                Assert.Same(expected, actual);
        }

        [Fact]
        public void IndexerThrowsExceptionWhenOutOfRange()
        {
            var inlineElements = new InlineDocumentationElement[]
            {
                DocumentationElement.Text(string.Empty),
                DocumentationElement.InlineCode(string.Empty),
                DocumentationElement.ParameterReference(string.Empty),
                DocumentationElement.GenericParameterReference(string.Empty),
                DocumentationElement.MemberReference("member"),
                DocumentationElement.MemberReference(typeof(string))
            };

            var inlineElementCollection = DocumentationElement.InlineDescription(inlineElements);

            var exception = Assert.Throws<ArgumentOutOfRangeException>("index", () => inlineElementCollection[-1]);
            Assert.Equal(new ArgumentOutOfRangeException("index", -1, new ArgumentOutOfRangeException().Message).Message, exception.Message);

            exception = Assert.Throws<ArgumentOutOfRangeException>("index", () => inlineElementCollection[inlineElementCollection.Count]);
            Assert.Equal(new ArgumentOutOfRangeException("index", 6, new ArgumentOutOfRangeException().Message).Message, exception.Message);
        }

        [Fact]
        public void CreateWithNullInlineElementCollectionThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                "inlineElements",
                () => DocumentationElement.InlineDescription(null, new Dictionary<string, string>())
            );

            Assert.Equal(new ArgumentNullException("inlineElements").Message, exception.Message);
        }

        [Fact]
        public void CreateWithNullXmlAttributeValueThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(
                "xmlAttributes",
                () => DocumentationElement.InlineDescription(Enumerable.Empty<InlineDocumentationElement>(), new Dictionary<string, string> { { "key", null } })
            );

            Assert.Equal(new ArgumentException("Cannot contain 'null' values.", "xmlAttributes").Message, exception.Message);
        }
    }
}