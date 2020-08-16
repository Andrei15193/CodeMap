using System;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class ParagraphDocumentationElementTests : DocumentationElementTests
    {
        [Fact]
        public void CreatingParagraphElementWithNullContentThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("content", () => DocumentationElement.Paragraph(null));

            Assert.Equal(new ArgumentNullException("content").Message, exception.Message);
        }

        [Fact]
        public void CreatingParagraphWithContentContainingNullThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("content", () => DocumentationElement.Paragraph(new InlineDocumentationElement[] { null }));

            Assert.Equal(new ArgumentException("Cannot contain 'null' elements.", "content").Message, exception.Message);
        }

        [Fact]
        public void ParagraphElementCallsVisitorMethod()
        {
            var paragraph = DocumentationElement.Paragraph();
            var visitor = new DocumentationVisitorMock<ParagraphDocumentationElement>(paragraph);

            paragraph.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}