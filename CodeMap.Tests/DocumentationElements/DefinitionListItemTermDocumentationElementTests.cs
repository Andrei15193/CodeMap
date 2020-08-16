using System;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class DefinitionListItemTermDocumentationElementTests : DocumentationElementTests
    {
        [Fact]
        public void CreatingDefinitionListItemTermElementWithNullContentThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("content", () => DocumentationElement.DefinitionListItemTerm(null));

            Assert.Equal(new ArgumentNullException("content").Message, exception.Message);
        }

        [Fact]
        public void CreatingDefinitionListItemTermWithContentContainingNullThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("content", () => DocumentationElement.DefinitionListItemTerm(new InlineDocumentationElement[] { null }));

            Assert.Equal(new ArgumentException("Cannot contain 'null' elements.", "content").Message, exception.Message);
        }

        [Fact]
        public void DefinitionListItemTermElementCallsVisitorMethod()
        {
            var definitionListItemTerm = DocumentationElement.DefinitionListItemTerm();
            var visitor = new DocumentationVisitorMock<DefinitionListItemTermDocumentationElement>(definitionListItemTerm);

            definitionListItemTerm.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}