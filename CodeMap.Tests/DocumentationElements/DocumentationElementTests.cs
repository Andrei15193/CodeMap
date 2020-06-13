using CodeMap.DocumentationElements;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class DocumentationElementTests
    {
        [Fact]
        public void CreatingTextElementWithNullContentThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("text", () => DocumentationElement.Text(null));

            Assert.Equal(new ArgumentNullException("text").Message, exception.Message);
        }

        [Fact]
        public void TextElementCallsVisitorMethod()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var textElement = DocumentationElement.Text("some text");

            visitorMock.VerifyAcceptMethods(
                textElement,
                visitor => visitor.VisitText(textElement)
            );
        }

        [Fact]
        public void CreatingInlineCodeElementWithNullContentThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("code", () => DocumentationElement.InlineCode(null));

            Assert.Equal(new ArgumentNullException("code").Message, exception.Message);
        }

        [Fact]
        public void InlineCodeElementCallsVisitorMethod()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var inlineCode = DocumentationElement.InlineCode("piece of code");

            visitorMock.VerifyAcceptMethods(
                inlineCode,
                visitor => visitor.VisitInlineCode(inlineCode)
            );
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        [InlineData("\r")]
        [InlineData("\n")]
        public void CreatingInlineReferenceElementWithNullValueThrowsException(string canonicalName)
        {
            var exception = Assert.Throws<ArgumentException>("canonicalName", () => DocumentationElement.MemberReference(canonicalName));

            Assert.Equal(new ArgumentException("Cannot be 'null', empty or white space.", "canonicalName").Message, exception.Message);
        }

        [Fact]
        public void InlineReferenceElementCallsVisitorMethod()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var memberReference = DocumentationElement.MemberReference(GetType().GetMembers().First());

            visitorMock.VerifyAcceptMethods(
                memberReference,
                visitor => visitor.VisitInlineReference(memberReference)
            );
        }

        [Fact]
        public void CreatingParameterReferenceElementWithNullValueThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("parameterName", () => DocumentationElement.ParameterReference(null));

            Assert.Equal(new ArgumentNullException("parameterName").Message, exception.Message);
        }

        [Fact]
        public void ParameterReferenceElementCallsVisitorMethod()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var parameterReference = DocumentationElement.ParameterReference("parameter");

            visitorMock.VerifyAcceptMethods(
                parameterReference,
                visitor => visitor.VisitParameterReference(parameterReference)
            );
        }

        [Fact]
        public void CreatingGenericParameterReferenceElementWithNullValueThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("genericParameterName", () => DocumentationElement.GenericParameterReference(null));

            Assert.Equal(new ArgumentNullException("genericParameterName").Message, exception.Message);
        }

        [Fact]
        public void ParameterGenericReferenceElementCallsVisitorMethod()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var genericParameterReference = DocumentationElement.GenericParameterReference("genericParameter");

            visitorMock.VerifyAcceptMethods(
               genericParameterReference,
               visitor => visitor.VisitGenericParameterReference(genericParameterReference)
            );
        }

        [Fact]
        public void CreatingParagraphElementWithNullContentThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("content", () => DocumentationElement.Paragraph(null));

            Assert.Equal(new ArgumentNullException("content").Message, exception.Message);
        }

        [Fact]
        public void CreatingParagraphWithContentContainingNullThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("content", () => DocumentationElement.Paragraph(new InlineDocumentationElement[] { null }));

            Assert.Equal(new ArgumentException("Cannot contain 'null' elements.", "content").Message, exception.Message);
        }

        [Fact]
        public void ParagraphElementCallsVisitorMethod()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var paragraph = DocumentationElement.Paragraph(Enumerable.Empty<InlineDocumentationElement>());

            visitorMock.VerifyAcceptMethods(
                paragraph,
                visitor => visitor.VisitParagraph(paragraph)
            );
        }

        [Fact]
        public void CreatingCodeBlockWithNullValueThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("code", () => DocumentationElement.CodeBlock(null));

            Assert.Equal(new ArgumentNullException("code").Message, exception.Message);
        }

        [Fact]
        public void CodeBlockElementCallsVisitorMethods()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var codeBlock = DocumentationElement.CodeBlock("code block");

            visitorMock.VerifyAcceptMethods(
                codeBlock,
                visitor => visitor.VisitCodeBlock(codeBlock)
            );
        }

        [Fact]
        public void CreatingListItemElementWithNullContentThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("content", () => DocumentationElement.ListItem(null));

            Assert.Equal(new ArgumentNullException("content").Message, exception.Message);
        }

        [Fact]
        public void CreatingListItemElementWithContentContainingNullThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("content", () => DocumentationElement.ListItem(new InlineDocumentationElement[] { null }));

            Assert.Equal(new ArgumentException("Cannot contain 'null' elements.", "content").Message, exception.Message);
        }

        [Fact]
        public void ListItemElementCallsVisitorMethod()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var listItem = DocumentationElement.ListItem(Enumerable.Empty<InlineDocumentationElement>());

            visitorMock.VerifyAcceptMethods(
                listItem,
                visitor => visitor.VisitListItem(listItem)
            );
        }

        [Fact]
        public void CreatingUnorderedListElementWithNullContentThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("items", () => DocumentationElement.UnorderedList(null));

            Assert.Equal(new ArgumentNullException("items").Message, exception.Message);
        }

        [Fact]
        public void CreatingUnorderedListElementWithContentContainingNullItemsThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("items", () => DocumentationElement.UnorderedList(new ListItemDocumentationElement[] { null }));

            Assert.Equal(new ArgumentException("Cannot contain 'null' items.", "items").Message, exception.Message);
        }

        [Fact]
        public void UnorderedListElementCallsVisitorMethod()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var unorderedList = DocumentationElement.UnorderedList(Enumerable.Empty<ListItemDocumentationElement>());

            visitorMock.VerifyAcceptMethods(
                unorderedList,
                visitor => visitor.VisitUnorderedList(unorderedList)
            );
        }

        [Fact]
        public void CreatingOrderedListElementWithNullContentThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("items", () => DocumentationElement.OrderedList(null));

            Assert.Equal(new ArgumentNullException("items").Message, exception.Message);
        }

        [Fact]
        public void CreatingOrderedListElementWithContentContainingNullItemsThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("items", () => DocumentationElement.OrderedList(new ListItemDocumentationElement[] { null }));

            Assert.Equal(new ArgumentException("Cannot contain 'null' items.", "items").Message, exception.Message);
        }

        [Fact]
        public void OrderedListElementCallsVisitorMethod()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var orderedList = DocumentationElement.OrderedList(Enumerable.Empty<ListItemDocumentationElement>());

            visitorMock.VerifyAcceptMethods(
                orderedList,
                visitor => visitor.VisitOrderedList(orderedList)
            );
        }

        [Fact]
        public void CreatingDefinitionListItemElementWithNullTermThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                "term",
                () => DocumentationElement.DefinitionListItem(
                    null,
                    Enumerable.Empty<InlineDocumentationElement>()
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
                    Enumerable.Empty<InlineDocumentationElement>(),
                    null
                )
            );

            Assert.Equal(new ArgumentNullException("description").Message, exception.Message);
        }

        [Fact]
        public void CreatingDefinitionListItemElementWithTermContainingNullItemsThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(
                "term",
                () => DocumentationElement.DefinitionListItem(
                    new InlineDocumentationElement[] { null },
                    Enumerable.Empty<InlineDocumentationElement>()
                )
            );

            Assert.Equal(new ArgumentException("Cannot contain 'null' elements.", "term").Message, exception.Message);
        }

        [Fact]
        public void CreatingDefinitionListItemElementWithTermDescriptionContainingNullItemsThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(
                "description",
                () => DocumentationElement.DefinitionListItem(
                    Enumerable.Empty<InlineDocumentationElement>(),
                    new InlineDocumentationElement[] { null }
                )
            );

            Assert.Equal(new ArgumentException("Cannot contain 'null' elements.", "description").Message, exception.Message);
        }

        [Fact]
        public void DefinitionListItemElementCallsVisitorMethod()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var definitionListItem = DocumentationElement.DefinitionListItem(
                Enumerable.Empty<InlineDocumentationElement>(),
                Enumerable.Empty<InlineDocumentationElement>()
            );

            visitorMock.VerifyAcceptMethods(
                definitionListItem,
                visitor => visitor.VisitDefinitionListItem(definitionListItem)
            );
        }

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
        public void CreatingDefinitionListElementWithTitleContainingNullValuesThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(
                "inlineElements",
                () => DocumentationElement.DefinitionList(
                    DocumentationElement.InlineDescription(new InlineDocumentationElement[] { null }),
                    Enumerable.Empty<DefinitionListItemDocumentationElement>()
                )
            );

            Assert.Equal(new ArgumentException("Cannot contain 'null' elements.", "inlineElements").Message, exception.Message);
        }

        [Fact]
        public void DefinitionListElementCallsVisitorMethod()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var definitionList = DocumentationElement.DefinitionList(Enumerable.Empty<DefinitionListItemDocumentationElement>());

            visitorMock.VerifyAcceptMethods(
                definitionList,
                visitor => visitor.VisitDefinitionList(definitionList)
            );
        }

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
            var visitorMock = new Mock<IDocumentationVisitor>();
            var tableCell = DocumentationElement.TableCell(Enumerable.Empty<InlineDocumentationElement>());

            visitorMock.VerifyAcceptMethods(
                tableCell,
                visitor => visitor.VisitTableCell(tableCell)
            );
        }

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
            var visitorMock = new Mock<IDocumentationVisitor>();
            var tableRow = DocumentationElement.TableRow(Enumerable.Empty<TableCellDocumentationElement>());

            visitorMock.VerifyAcceptMethods(
                tableRow,
                visitor => visitor.VisitTableRow(tableRow)
            );
        }

        [Fact]
        public void CreatingTableColumnElementWithNullNameThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("name", () => DocumentationElement.TableColumn(null));

            Assert.Equal(new ArgumentNullException("name").Message, exception.Message);
        }

        [Fact]
        public void CreatinTableColumnElementWithContentContainingNullThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("name", () => DocumentationElement.TableColumn(new InlineDocumentationElement[] { null }));

            Assert.Equal(new ArgumentException("Cannot contain 'null' elements.", "name").Message, exception.Message);
        }

        [Fact]
        public void TableColumnElementCallsVisitorMethod()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var tableColumn = DocumentationElement.TableColumn(Enumerable.Empty<InlineDocumentationElement>());

            visitorMock.VerifyAcceptMethods(
                tableColumn,
                visitor => visitor.VisitTableColumn(tableColumn)
            );
        }

        [Fact]
        public void CreatingTableElementWithNullColumnsThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("columns", () => DocumentationElement.Table(null, Enumerable.Empty<TableRowDocumentationElement>()));

            Assert.Equal(new ArgumentNullException("columns").Message, exception.Message);
        }

        [Fact]
        public void CreatinTableElementWithColumnsContainingNullThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(
                "columns",
                () => DocumentationElement.Table(
                    new TableColumnDocumentationElement[] { null },
                    Enumerable.Empty<TableRowDocumentationElement>()
                )
            );

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
            var exception = Assert.Throws<ArgumentException>(
                "rows",
                () => DocumentationElement.Table(
                    Enumerable.Empty<TableColumnDocumentationElement>(),
                    new TableRowDocumentationElement[] { null }
                )
            );

            Assert.Equal(new ArgumentException("Cannot contain 'null' rows.", "rows").Message, exception.Message);
        }

        [Fact]
        public void TableElementCallsVisitorMethod()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var table = DocumentationElement.Table(
                Enumerable.Empty<TableColumnDocumentationElement>(),
                Enumerable.Empty<TableRowDocumentationElement>()
            );

            visitorMock.VerifyAcceptMethods(
                table,
                visitor => visitor.VisitTable(table)
            );
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
            var exception = Assert.Throws<ArgumentException>(
                "rows",
                () => DocumentationElement.Table(
                    new TableRowDocumentationElement[] { null }
                )
            );

            Assert.Equal(new ArgumentException("Cannot contain 'null' rows.", "rows").Message, exception.Message);
        }

        [Fact]
        public void HeaderlessTableElementCallsVisitorMethod()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var table = DocumentationElement.Table(
                Enumerable.Empty<TableRowDocumentationElement>()
            );

            visitorMock.VerifyAcceptMethods(
                table,
                visitor => visitor.VisitTable(table)
            );
        }

        [Fact]
        public void CreatingSummaryElementWithNullContentThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("content", () => DocumentationElement.Summary(null));

            Assert.Equal(new ArgumentNullException("content").Message, exception.Message);
        }

        [Fact]
        public void CreatingSummaryElementWithContentContainingNullThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("content", () => DocumentationElement.Summary(new BlockDocumentationElement[] { null }));

            Assert.Equal(new ArgumentException("Cannot contain 'null' elements.", "content").Message, exception.Message);
        }

        [Fact]
        public void SummaryElementCallsVisitorMethod()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var summary = DocumentationElement.Summary(Enumerable.Empty<BlockDocumentationElement>());

            visitorMock.VerifyAcceptMethods(
                summary,
                visitor => visitor.VisitSummary(summary)
            );
        }

        [Fact]
        public void CreatingRemarksElementWithNullContentThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("content", () => DocumentationElement.Remarks(null));

            Assert.Equal(new ArgumentNullException("content").Message, exception.Message);
        }

        [Fact]
        public void CreatingRemarksElementWithContentContainingNullThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("content", () => DocumentationElement.Remarks(new BlockDocumentationElement[] { null }));

            Assert.Equal(new ArgumentException("Cannot contain 'null' elements.", "content").Message, exception.Message);
        }

        [Fact]
        public void RemarksElementCallsBeginningAndEndingVisitorMethods()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var remarks = DocumentationElement.Remarks(Enumerable.Empty<BlockDocumentationElement>());

            visitorMock.VerifyAcceptMethods(
                remarks,
                visitor => visitor.VisitRemarks(remarks)
            );
        }

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
            var visitorMock = new Mock<IDocumentationVisitor>();
            var example = DocumentationElement.Example(Enumerable.Empty<BlockDocumentationElement>());

            visitorMock.VerifyAcceptMethods(
                example,
                visitor => visitor.VisitExample(example)
            );
        }

        [Fact]
        public void CreatingValueElementWithNullContentThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("content", () => DocumentationElement.Value(null));

            Assert.Equal(new ArgumentNullException("content").Message, exception.Message);
        }

        [Fact]
        public void CreatingValueElementWithContentContainingNullThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("content", () => DocumentationElement.Value(new BlockDocumentationElement[] { null }));

            Assert.Equal(new ArgumentException("Cannot contain 'null' elements.", "content").Message, exception.Message);
        }

        [Fact]
        public void ValueElementCallsBeginningAndEndingVisitorMethods()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var value = DocumentationElement.Value(Enumerable.Empty<BlockDocumentationElement>());

            visitorMock.VerifyAcceptMethods(
                value,
                visitor => visitor.VisitValue(value)
            );
        }
    }
}