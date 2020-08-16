using System;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class ValueDocumentationElementTests : DocumentationElementTests
    {
        [Fact]
        public void CreatingValueElementWithNullContentThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("content", () => DocumentationElement.Value(null));

            Assert.Equal(new ArgumentNullException("content").Message, exception.Message);
        }

        [Fact]
        public void CreatingValueElementWithContentContainingNullThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("content", () => DocumentationElement.Value(new BlockDocumentationElement[] { null }));

            Assert.Equal(new ArgumentException("Cannot contain 'null' elements.", "content").Message, exception.Message);
        }

        [Fact]
        public void ValueElementCallsBeginningAndEndingVisitorMethods()
        {
            var value = DocumentationElement.Value();
            var visitor = new DocumentationVisitorMock<ValueDocumentationElement>(value);

            value.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}