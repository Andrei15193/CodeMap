using System;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class TableColumnDocumentationElementTests : DocumentationElementTests
    {
        [Fact]
        public void CreatingTableColumnElementWithNullNameThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("name", () => DocumentationElement.TableColumn(null));

            Assert.Equal(new ArgumentNullException("name").Message, exception.Message);
        }

        [Fact]
        public void CreatingTableColumnElementWithContentContainingNullThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("name", () => DocumentationElement.TableColumn(new InlineDocumentationElement[] { null }));

            Assert.Equal(new ArgumentException("Cannot contain 'null' elements.", "name").Message, exception.Message);
        }

        [Fact]
        public void TableColumnElementCallsVisitorMethod()
        {
            var tableColumn = DocumentationElement.TableColumn();
            var visitor = new DocumentationVisitorMock<TableColumnDocumentationElement>(tableColumn);

            tableColumn.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}