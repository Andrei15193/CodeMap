using System;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class OrderedListDocumentationElementTests : DocumentationElementTests
    {
        [Fact]
        public void CreatingOrderedListElementWithNullContentThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("items", () => DocumentationElement.OrderedList(null));

            Assert.Equal(new ArgumentNullException("items").Message, exception.Message);
        }

        [Fact]
        public void CreatingOrderedListElementWithContentContainingNullItemsThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("items", () => DocumentationElement.OrderedList(new ListItemDocumentationElement[] { null }));

            Assert.Equal(new ArgumentException("Cannot contain 'null' items.", "items").Message, exception.Message);
        }

        [Fact]
        public void OrderedListElementCallsVisitorMethod()
        {
            var orderedList = DocumentationElement.OrderedList();
            var visitor = new DocumentationVisitorMock<OrderedListDocumentationElement>(orderedList);

            orderedList.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}