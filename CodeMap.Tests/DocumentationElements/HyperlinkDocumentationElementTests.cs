using System;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class HyperlinkDocumentationElementTests : DocumentationElementTests
    {
        [Fact]
        public void CreatingHyperlinkElementWithNullDestinationThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("destination", () => DocumentationElement.Hyperlink(null, DocumentationElement.Text("text")));

            Assert.Equal(new ArgumentNullException("destination").Message, exception.Message);
        }

        [Fact]
        public void CreatingHyperlinkElementWithNullTextThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("content", () => DocumentationElement.Hyperlink("destination", null));

            Assert.Equal(new ArgumentNullException("content").Message, exception.Message);
        }

        [Fact]
        public void HyperlinkElementCallsVisitorMethod()
        {
            var hyperlinkElement = DocumentationElement.Hyperlink("destination", DocumentationElement.Text("text"));
            var visitor = new DocumentationVisitorMock<HyperlinkDocumentationElement>(hyperlinkElement);

            hyperlinkElement.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}