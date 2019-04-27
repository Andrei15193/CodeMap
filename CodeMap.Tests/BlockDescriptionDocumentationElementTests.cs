using CodeMap.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CodeMap.Tests
{
    public class BlockDescriptionDocumentationElementTests
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

            var blockElementCollection = DocumentationElement.BlockDescription(blockElements);

            Assert.Equal(6, blockElementCollection.Count);
            for (var index = 0; index < blockElementCollection.Count; index++)
                Assert.Same(blockElements[index], blockElementCollection[index]);
            foreach (var (expected, actual) in blockElements.Zip(blockElementCollection, (expected, actual) => (expected, actual)))
                Assert.Same(expected, actual);
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

            var blockElementCollection = DocumentationElement.BlockDescription(blockElements);

            var exception = Assert.Throws<ArgumentOutOfRangeException>("index", () => blockElementCollection[-1]);
            Assert.Equal(new ArgumentOutOfRangeException("index", -1, new ArgumentOutOfRangeException().Message).Message, exception.Message);

            exception = Assert.Throws<ArgumentOutOfRangeException>("index", () => blockElementCollection[blockElementCollection.Count]);
            Assert.Equal(new ArgumentOutOfRangeException("index", 6, new ArgumentOutOfRangeException().Message).Message, exception.Message);
        }

        [Fact]
        public void CreateWithNullBlockElementCollectionThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                "blockElements",
                () => DocumentationElement.BlockDescription(null, new Dictionary<string, string>())
            );

            Assert.Equal(new ArgumentNullException("blockElements").Message, exception.Message);
        }

        [Fact]
        public void CreateWithNullXmlAttributeValueThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(
                "xmlAttributes",
                () => DocumentationElement.BlockDescription(Enumerable.Empty<BlockDocumentationElement>(), new Dictionary<string, string> { { "key", null } })
            );

            Assert.Equal(new ArgumentException("Cannot contain 'null' values.", "xmlAttributes").Message, exception.Message);
        }
    }
}