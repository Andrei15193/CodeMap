using System;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class TableRowDocumentationElementTests : DocumentationElementTests
    {
        [Fact]
        public void CreatingTableRowElementWithNullCellsThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("cells", () => DocumentationElement.TableRow(null));

            Assert.Equal(new ArgumentNullException("cells").Message, exception.Message);
        }

        [Fact]
        public void CreatinTableRowElementWithCellsContainingNullThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("cells", () => DocumentationElement.TableRow(new TableCellDocumentationElement[] { null }));

            Assert.Equal(new ArgumentException("Cannot contain 'null' cells.", "cells").Message, exception.Message);
        }

        [Fact]
        public void TableRowElementCallsVisitorMethod()
        {
            var tableRow = DocumentationElement.TableRow();
            var visitor = new DocumentationVisitorMock<TableRowDocumentationElement>(tableRow);

            tableRow.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}