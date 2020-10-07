using System;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class ExceptionDocumentationElementTests : DocumentationElementTests
    {
        [Fact]
        public void CreatingExceptionElementWithNullExceptionThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("exception", () => DocumentationElement.Exception(null));

            Assert.Equal(new ArgumentNullException("exception").Message, exception.Message);
        }

        [Fact]
        public void CreatingExceptionElementWithNullDescriptionThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("description", () => DocumentationElement.Exception(DocumentationElement.MemberReference("canonical-name"), null));

            Assert.Equal(new ArgumentNullException("description").Message, exception.Message);
        }

        [Fact]
        public void CreatingExceptionElementWithDescriptionContainingNullThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("description", () => DocumentationElement.Exception(DocumentationElement.MemberReference("canonical-name"), new BlockDocumentationElement[] { null }));

            Assert.Equal(new ArgumentException("Cannot contain 'null' elements.", "description").Message, exception.Message);
        }

        [Fact]
        public void ExceptionElementCallsBeginningAndEndingVisitorMethods()
        {
            var exception = DocumentationElement.Exception(DocumentationElement.MemberReference("canonical-name"));
            var visitor = new DocumentationVisitorMock<ExceptionDocumentationElement>(exception);

            exception.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}