using System;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class DefinitionListItemDescriptionDocumentationElementTests : DocumentationElementTests
    {
        [Fact]
        public void CreatingDefinitionListItemDescriptionElementWithNullContentThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("content", () => DocumentationElement.DefinitionListItemDescription(null));

            Assert.Equal(new ArgumentNullException("content").Message, exception.Message);
        }

        [Fact]
        public void CreatingDefinitionListItemDescriptionWithContentContainingNullThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("content", () => DocumentationElement.DefinitionListItemDescription(new InlineDocumentationElement[] { null }));

            Assert.Equal(new ArgumentException("Cannot contain 'null' elements.", "content").Message, exception.Message);
        }

        [Fact]
        public void DefinitionListItemDescriptionElementCallsVisitorMethod()
        {
            var definitionListItemDescription = DocumentationElement.DefinitionListItemDescription();
            var visitor = new DocumentationVisitorMock<DefinitionListItemDescriptionDocumentationElement>(definitionListItemDescription);

            definitionListItemDescription.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}