using System;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class UnorderedListDocumentationElementTests : DocumentationElementTests
    {
        [Fact]
        public void CreatingUnorderedListElementWithNullContentThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("items", () => DocumentationElement.UnorderedList(null));

            Assert.Equal(new ArgumentNullException("items").Message, exception.Message);
        }

        [Fact]
        public void CreatingUnorderedListElementWithContentContainingNullItemsThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("items", () => DocumentationElement.UnorderedList(new ListItemDocumentationElement[] { null }));

            Assert.Equal(new ArgumentException("Cannot contain 'null' items.", "items").Message, exception.Message);
        }

        [Fact]
        public void UnorderedListElementCallsVisitorMethod()
        {
            var unorderedList = DocumentationElement.UnorderedList();
            var visitor = new DocumentationVisitorMock<UnorderedListDocumentationElement>(unorderedList);

            unorderedList.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}