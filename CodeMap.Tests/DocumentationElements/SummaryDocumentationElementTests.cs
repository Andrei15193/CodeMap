using System;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class SummaryDocumentationElementTests : DocumentationElementTests
    {
        [Fact]
        public void CreatingSummaryElementWithNullContentThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("content", () => DocumentationElement.Summary(null));

            Assert.Equal(new ArgumentNullException("content").Message, exception.Message);
        }

        [Fact]
        public void CreatingSummaryElementWithContentContainingNullThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("content", () => DocumentationElement.Summary(new BlockDocumentationElement[] { null }));

            Assert.Equal(new ArgumentException("Cannot contain 'null' elements.", "content").Message, exception.Message);
        }

        [Fact]
        public void SummaryElementCallsVisitorMethod()
        {
            var summary = DocumentationElement.Summary();
            var visitor = new DocumentationVisitorMock<SummaryDocumentationElement>(summary);

            summary.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}