using CodeMap.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CodeMap.Tests
{
    public class DescriptionDocumentationElementTests
    {
        [Fact]
        public void CreateBlockElementCollection()
        {
            var blockElements = new BlockDocumentationElement[]
            {
                DocumentationElement.Paragraph(),
                DocumentationElement.UnorderedList(),
                DocumentationElement.UnorderedList(),
                DocumentationElement.DefinitionList(),
                DocumentationElement.Table(),
                DocumentationElement.CodeBlock(string.Empty)
            };

            var blockElementCollection = new DescriptionDocumentationElement(blockElements);

            Assert.Equal(6, blockElementCollection.Count);
            for (var index = 0; index < blockElementCollection.Count; index++)
                Assert.Same(blockElements[index], blockElementCollection[index]);
            foreach (var pair in blockElements.Zip(blockElementCollection, (expected, actual) => new { Expected = expected, Actual = actual }))
                Assert.Same(pair.Expected, pair.Actual);
        }

        [Fact]
        public void IndexerThrowsExceptionWhenOutOfRange()
        {
            var blockElements = new BlockDocumentationElement[]
            {
                DocumentationElement.Paragraph(),
                DocumentationElement.UnorderedList(),
                DocumentationElement.UnorderedList(),
                DocumentationElement.DefinitionList(),
                DocumentationElement.Table(),
                DocumentationElement.CodeBlock(string.Empty)
            };

            var blockElementCollection = new DescriptionDocumentationElement(blockElements);

            var exception = Assert.Throws<ArgumentOutOfRangeException>("index", () => blockElementCollection[-1]);
            Assert.Equal(new ArgumentOutOfRangeException("index", -1, new ArgumentOutOfRangeException().Message).Message, exception.Message);

            exception = Assert.Throws<ArgumentOutOfRangeException>("index", () => blockElementCollection[blockElementCollection.Count]);
            Assert.Equal(new ArgumentOutOfRangeException("index", 6, new ArgumentOutOfRangeException().Message).Message, exception.Message);
        }

        [Fact]
        public void CreateWithNullBlockElementCollectionThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                "blockDocumentationElements",
                () => new DescriptionDocumentationElement(null, new Dictionary<string, string>())
            );

            Assert.Equal(new ArgumentNullException("blockDocumentationElements").Message, exception.Message);
        }
        [Fact]
        public void CreateWithNullXmlAttributesThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                "xmlAttributes",
                () => new DescriptionDocumentationElement(Enumerable.Empty<BlockDocumentationElement>(), null)
            );

            Assert.Equal(new ArgumentNullException("xmlAttributes").Message, exception.Message);
        }
    }
}