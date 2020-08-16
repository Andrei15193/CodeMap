using System;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class InlineCodeDocumentationElementTests : DocumentationElementTests
    {
        [Fact]
        public void CreatingInlineCodeElementWithNullContentThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("code", () => DocumentationElement.InlineCode(null));

            Assert.Equal(new ArgumentNullException("code").Message, exception.Message);
        }

        [Fact]
        public void InlineCodeElementCallsVisitorMethod()
        {
            var inlineCode = DocumentationElement.InlineCode("piece of code");
            var visitor = new DocumentationVisitorMock<InlineCodeDocumentationElement>(inlineCode);

            inlineCode.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}