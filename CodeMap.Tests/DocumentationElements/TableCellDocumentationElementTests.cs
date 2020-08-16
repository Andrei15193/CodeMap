using System;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class TableCellDocumentationElementTests : DocumentationElementTests
    {
        [Fact]
        public void CreatingTableCellElementWithNullContentThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("content", () => DocumentationElement.TableCell(null));

            Assert.Equal(new ArgumentNullException("content").Message, exception.Message);
        }

        [Fact]
        public void CreatinTableCellElementWithContentContainingNullThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("content", () => DocumentationElement.TableCell(new InlineDocumentationElement[] { null }));

            Assert.Equal(new ArgumentException("Cannot contain 'null' elements.", "content").Message, exception.Message);
        }

        [Fact]
        public void TableCellElementCallsVisitorMethod()
        {
            var tableCell = DocumentationElement.TableCell();
            var visitor = new DocumentationVisitorMock<TableCellDocumentationElement>(tableCell);

            tableCell.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}