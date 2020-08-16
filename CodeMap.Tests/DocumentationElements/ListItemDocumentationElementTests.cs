using System;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class ListItemDocumentationElementTests : DocumentationElementTests
    {
        [Fact]
        public void CreatingListItemElementWithNullContentThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("content", () => DocumentationElement.ListItem(null));

            Assert.Equal(new ArgumentNullException("content").Message, exception.Message);
        }

        [Fact]
        public void CreatingListItemElementWithContentContainingNullThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("content", () => DocumentationElement.ListItem(new InlineDocumentationElement[] { null }));

            Assert.Equal(new ArgumentException("Cannot contain 'null' elements.", "content").Message, exception.Message);
        }

        [Fact]
        public void ListItemElementCallsVisitorMethod()
        {
            var listItem = DocumentationElement.ListItem();
            var visitor = new DocumentationVisitorMock<ListItemDocumentationElement>(listItem);

            listItem.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}