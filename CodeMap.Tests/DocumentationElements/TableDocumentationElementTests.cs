using System;
using System.Collections.Generic;
using System.Linq;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class TableDocumentationElementTests : DocumentationElementTests
    {
        [Fact]
        public void CreatingTableElementWithNullColumnsThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("columns", () => DocumentationElement.Table(null, Enumerable.Empty<TableRowDocumentationElement>()));

            Assert.Equal(new ArgumentNullException("columns").Message, exception.Message);
        }

        [Fact]
        public void CreatinTableElementWithColumnsContainingNullThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("columns", () => DocumentationElement.Table(new TableColumnDocumentationElement[] { null }, Enumerable.Empty<TableRowDocumentationElement>()));

            Assert.Equal(new ArgumentException("Cannot contain 'null' columns.", "columns").Message, exception.Message);
        }

        [Fact]
        public void CreatingTableElementWithNullRowsThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("rows", () => DocumentationElement.Table(Enumerable.Empty<TableColumnDocumentationElement>(), null));

            Assert.Equal(new ArgumentNullException("rows").Message, exception.Message);
        }

        [Fact]
        public void CreatinTableElementWithRowsContainingNullThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("rows", () => DocumentationElement.Table(Enumerable.Empty<TableColumnDocumentationElement>(), new TableRowDocumentationElement[] { null }));

            Assert.Equal(new ArgumentException("Cannot contain 'null' rows.", "rows").Message, exception.Message);
        }

        [Fact]
        public void CreatingHeaderlessTableElementWithNullRowsThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("rows", () => DocumentationElement.Table((IEnumerable<TableRowDocumentationElement>)null));

            Assert.Equal(new ArgumentNullException("rows").Message, exception.Message);
        }

        [Fact]
        public void CreatinHeaderlessTableElementWithRowsContainingNullThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("rows", () => DocumentationElement.Table(new TableRowDocumentationElement[] { null }));

            Assert.Equal(new ArgumentException("Cannot contain 'null' rows.", "rows").Message, exception.Message);
        }

        [Fact]
        public void TableElementCallsVisitorMethod()
        {
            var table = DocumentationElement.Table();
            var visitor = new DocumentationVisitorMock<TableDocumentationElement>(table);

            table.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}