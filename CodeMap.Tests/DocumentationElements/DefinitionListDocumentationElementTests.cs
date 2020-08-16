using System;
using System.Collections.Generic;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class DefinitionListDocumentationElementTests : DocumentationElementTests
    {
        [Fact]
        public void CreatingDefinitionListElementWithNullItemsThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("items", () => DocumentationElement.DefinitionList((IEnumerable<DefinitionListItemDocumentationElement>)null));

            Assert.Equal(new ArgumentNullException("items").Message, exception.Message);
        }

        [Fact]
        public void CreatingDefinitionListElementWithContentContainingNullItemsThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("items", () => DocumentationElement.DefinitionList(new DefinitionListItemDocumentationElement[] { null }));

            Assert.Equal(new ArgumentException("Cannot contain 'null' items.", "items").Message, exception.Message);
        }

        [Fact]
        public void DefinitionListElementCallsVisitorMethod()
        {
            var definitionList = DocumentationElement.DefinitionList();
            var visitor = new DocumentationVisitorMock<DefinitionListDocumentationElement>(definitionList);

            definitionList.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}