using System;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class TextDocumentationElementTests : DocumentationElementTests
    {
        [Fact]
        public void CreatingTextElementWithNullContentThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("text", () => DocumentationElement.Text(null));

            Assert.Equal(new ArgumentNullException("text").Message, exception.Message);
        }

        [Fact]
        public void TextElementCallsVisitorMethod()
        {
            var textElement = DocumentationElement.Text("some text");
            var visitor = new DocumentationVisitorMock<TextDocumentationElement>(textElement);

            textElement.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}