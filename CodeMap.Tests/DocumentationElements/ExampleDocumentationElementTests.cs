using System;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class ExampleDocumentationElementTests : DocumentationElementTests
    {
        [Fact]
        public void CreatingExampleElementWithNullContentThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("content", () => DocumentationElement.Example(null));

            Assert.Equal(new ArgumentNullException("content").Message, exception.Message);
        }

        [Fact]
        public void CreatingExampleElementWithContentContainingNullThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("content", () => DocumentationElement.Example(new BlockDocumentationElement[] { null }));

            Assert.Equal(new ArgumentException("Cannot contain 'null' elements.", "content").Message, exception.Message);
        }

        [Fact]
        public void ExampleElementCallsBeginningAndEndingVisitorMethods()
        {
            var example = DocumentationElement.Example();
            var visitor = new DocumentationVisitorMock<ExampleDocumentationElement>(example);

            example.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}