using System;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class DefinitionListTitleDocumentationElementTests : DocumentationElementTests
    {
        [Fact]
        public void CreatingDefinitionListTitleElementWithNullContentThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("content", () => DocumentationElement.DefinitionListTitle(null));

            Assert.Equal(new ArgumentNullException("content").Message, exception.Message);
        }

        [Fact]
        public void CreatingDefinitionListTitleWithContentContainingNullThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("content", () => DocumentationElement.DefinitionListTitle(new InlineDocumentationElement[] { null }));

            Assert.Equal(new ArgumentException("Cannot contain 'null' elements.", "content").Message, exception.Message);
        }

        [Fact]
        public void DefinitionListTitleElementCallsVisitorMethod()
        {
            var definitionListTitle = DocumentationElement.DefinitionListTitle();
            var visitor = new DocumentationVisitorMock<DefinitionListTitleDocumentationElement>(definitionListTitle);

            definitionListTitle.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}