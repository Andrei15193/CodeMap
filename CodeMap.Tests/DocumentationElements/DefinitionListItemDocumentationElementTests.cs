using System;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class DefinitionListItemDocumentationElementTests : DocumentationElementTests
    {
        [Fact]
        public void CreatingDefinitionListItemElementWithNullTermThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                "term",
                () => DocumentationElement.DefinitionListItem(
                    null,
                    DocumentationElement.DefinitionListItemDescription()
                )
            );

            Assert.Equal(new ArgumentNullException("term").Message, exception.Message);
        }

        [Fact]
        public void CreatingDefinitionListItemElementWithNullTermDescriptionThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                "description",
                () => DocumentationElement.DefinitionListItem(
                    DocumentationElement.DefinitionListItemTerm(),
                    null
                )
            );

            Assert.Equal(new ArgumentNullException("description").Message, exception.Message);
        }

        [Fact]
        public void DefinitionListItemElementCallsVisitorMethod()
        {
            var definitionListItem = DocumentationElement.DefinitionListItem(DocumentationElement.DefinitionListItemTerm(), DocumentationElement.DefinitionListItemDescription());
            var visitor = new DocumentationVisitorMock<DefinitionListItemDocumentationElement>(definitionListItem);

            definitionListItem.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}