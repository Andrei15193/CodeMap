using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeMap.Elements;
using Moq;
using Xunit;

namespace CodeMap.Tests
{
    public class XmlDocumentationElementTests
    {
        [Fact]
        public void CreatingTextElementWithNullContentThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("text", () => DocumentationElement.Text(null));

            Assert.Equal(new ArgumentNullException("text").Message, exception.Message);
        }

        [Fact]
        public async Task TextElementCallsVisitorMethod()
        {
            const string text = "some text";

            var visitorMock = new Mock<IDocumentationVisitor>();
            var textElement = DocumentationElement.Text(text);

            await visitorMock.VerifyAcceptMethods(
                textElement,
                visitor => visitor.VisitText(text)
            );
        }

        [Fact]
        public void CreatingInlineCodeElementWithNullContentThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("code", () => DocumentationElement.InlineCode(null));

            Assert.Equal(new ArgumentNullException("code").Message, exception.Message);
        }

        [Fact]
        public async Task InlineCodeElementCallsVisitorMethod()
        {
            const string code = "piece of code";

            var visitorMock = new Mock<IDocumentationVisitor>();
            var inlineCode = DocumentationElement.InlineCode(code);

            await visitorMock.VerifyAcceptMethods(
                inlineCode,
                visitor => visitor.VisitInlineCode(code)
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
        public async Task InlineReferenceElementCallsVisitorMethod()
        {
            var referredMember = GetType().GetMembers().First();

            var visitorMock = new Mock<IDocumentationVisitor>();
            var memberReference = DocumentationElement.MemberReference(referredMember);

            await visitorMock.VerifyAcceptMethods(
                memberReference,
                visitor => visitor.VisitInlineReference(referredMember)
            );
        }

        [Fact]
        public void CreatingParameterReferenceElementWithNullValueThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("parameterName", () => DocumentationElement.ParameterReference(null));

            Assert.Equal(new ArgumentNullException("parameterName").Message, exception.Message);
        }

        [Fact]
        public async Task ParameterReferenceElementCallsVisitorMethod()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var parameterReference = DocumentationElement.ParameterReference("parameter");

            await visitorMock.VerifyAcceptMethods(
                parameterReference,
                visitor => visitor.VisitParameterReference("parameter")
            );
        }

        [Fact]
        public void CreatingGenericParameterReferenceElementWithNullValueThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("genericParameterName", () => DocumentationElement.GenericParameterReference(null));

            Assert.Equal(new ArgumentNullException("genericParameterName").Message, exception.Message);
        }

        [Fact]
        public async Task ParameterGenericReferenceElementCallsVisitorMethod()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var genericParameterReference = DocumentationElement.GenericParameterReference("genericParameter");

            await visitorMock.VerifyAcceptMethods(
                genericParameterReference,
                visitor => visitor.VisitGenericParameterReference("genericParameter")
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
        public async Task ParagraphElementCallsVisitorBeginningAndEndingMethods()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var paragraph = DocumentationElement.Paragraph(Enumerable.Empty<InlineDocumentationElement>());

            await visitorMock.VerifyAcceptMethods(
                paragraph,
                visitor => visitor.VisitParagraphBeginning(),
                visitor => visitor.VisitParagraphEnding()
            );
        }

        [Fact]
        public async Task ParagraphElementCallsVisitorMethods()
        {
            const string text = "plain text";
            const string code = "piece of code";
            var referredMember = GetType().GetMembers().First();
            const string parameter = "parameter";
            const string genericParameter = "genericParameter";

            var visitorMock = new Mock<IDocumentationVisitor>();
            var paragraph = DocumentationElement.Paragraph(
                new InlineDocumentationElement[]
                {
                    DocumentationElement.Text(text),
                    DocumentationElement.InlineCode(code),
                    DocumentationElement.MemberReference(referredMember),
                    DocumentationElement.ParameterReference(parameter),
                    DocumentationElement.GenericParameterReference(genericParameter)
                }
            );

            await visitorMock.VerifyAcceptMethods(
                paragraph,
                visitor => visitor.VisitParagraphBeginning(),

                visitor => visitor.VisitText(text),
                visitor => visitor.VisitInlineCode(code),
                visitor => visitor.VisitInlineReference(referredMember),
                visitor => visitor.VisitParameterReference(parameter),
                visitor => visitor.VisitGenericParameterReference(genericParameter),

                visitor => visitor.VisitParagraphEnding()
            );
        }

        [Fact]
        public void CreatingCodeBlockWithNullValueThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("code", () => DocumentationElement.CodeBlock(null));

            Assert.Equal(new ArgumentNullException("code").Message, exception.Message);
        }

        [Fact]
        public async Task CodeBlockElementCallsVisitorMethods()
        {
            const string code = "code block";
            var visitorMock = new Mock<IDocumentationVisitor>();
            var codeBlock = DocumentationElement.CodeBlock(code);

            await visitorMock.VerifyAcceptMethods(
                codeBlock,
                visitor => visitor.VisitCodeBlock(code)
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
        public async Task ListItemElementCallsVisitorBeginningAndEndingMethods()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var listItem = DocumentationElement.ListItem(Enumerable.Empty<InlineDocumentationElement>());

            await visitorMock.VerifyAcceptMethods(
                listItem,
                visitor => visitor.VisitListItemBeginning(),
                visitor => visitor.VisitListItemEnding()
            );
        }

        [Fact]
        public async Task ListItemElementCallsVisitorMethods()
        {
            const string text = "plain text";
            const string code = "piece of code";
            var referredMember = GetType().GetMembers().First();
            const string parameter = "parameter";
            const string genericParameter = "genericParameter";

            var visitorMock = new Mock<IDocumentationVisitor>();
            var listItem = DocumentationElement.ListItem(
                new InlineDocumentationElement[]
                {
                    DocumentationElement.Text(text),
                    DocumentationElement.InlineCode(code),
                    DocumentationElement.MemberReference(referredMember),
                    DocumentationElement.ParameterReference(parameter),
                    DocumentationElement.GenericParameterReference(genericParameter)
                }
            );

            await visitorMock.VerifyAcceptMethods(
                listItem,
                visitor => visitor.VisitListItemBeginning(),

                visitor => visitor.VisitText(text),
                visitor => visitor.VisitInlineCode(code),
                visitor => visitor.VisitInlineReference(referredMember),
                visitor => visitor.VisitParameterReference(parameter),
                visitor => visitor.VisitGenericParameterReference(genericParameter),

                visitor => visitor.VisitListItemEnding()
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
        public async Task UnorderedListElementCallsVisitorBeginningAndEndingMethods()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var unorderedList = DocumentationElement.UnorderedList(Enumerable.Empty<ListItemDocumentationElement>());

            await visitorMock.VerifyAcceptMethods(
                unorderedList,
                visitor => visitor.VisitUnorderedListBeginning(),
                visitor => visitor.VisitUnorderedListEnding()
            );
        }

        [Fact]
        public async Task UnorderedListElementCallsVisitorMethods()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var unorderedList = DocumentationElement.UnorderedList(
                new[]
                {
                    DocumentationElement.ListItem(Enumerable.Empty<InlineDocumentationElement>()),
                    DocumentationElement.ListItem(Enumerable.Empty<InlineDocumentationElement>())
                }
            );

            await visitorMock.VerifyAcceptMethods(
                unorderedList,
                new InvocationCheck(visitor => visitor.VisitUnorderedListBeginning(), Times.Once()),

                new InvocationCheck(visitor => visitor.VisitListItemBeginning(), Times.Exactly(2)),
                new InvocationCheck(visitor => visitor.VisitListItemEnding(), Times.Exactly(2)),

                new InvocationCheck(visitor => visitor.VisitUnorderedListEnding(), Times.Once())
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
        public async Task OrderedListElementCallsVisitorBeginningAndEndingMethods()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var orederedList = DocumentationElement.OrderedList(Enumerable.Empty<ListItemDocumentationElement>());

            await visitorMock.VerifyAcceptMethods(
                orederedList,
                visitor => visitor.VisitOrderedListBeginning(),
                visitor => visitor.VisitOrderedListEnding()
            );
        }

        [Fact]
        public async Task OrderedListElementCallsVisitorMethods()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var orederdList = DocumentationElement.OrderedList(
                new[]
                {
                    DocumentationElement.ListItem(Enumerable.Empty<InlineDocumentationElement>()),
                    DocumentationElement.ListItem(Enumerable.Empty<InlineDocumentationElement>())
                }
            );

            await visitorMock.VerifyAcceptMethods(
                orederdList,
                new InvocationCheck(visitor => visitor.VisitOrderedListBeginning(), Times.Once()),

                new InvocationCheck(visitor => visitor.VisitListItemBeginning(), Times.Exactly(2)),
                new InvocationCheck(visitor => visitor.VisitListItemEnding(), Times.Exactly(2)),

                new InvocationCheck(visitor => visitor.VisitOrderedListEnding(), Times.Once())
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
        public async Task DefinitionListItemElementCallsVisitorBeginningAndEndingMethods()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var definitionListItem = DocumentationElement.DefinitionListItem(
                Enumerable.Empty<InlineDocumentationElement>(),
                Enumerable.Empty<InlineDocumentationElement>()
            );

            await visitorMock.VerifyAcceptMethods(
                definitionListItem,
                visitor => visitor.VisitDefinitionListItemBeginning(),
                visitor => visitor.VisitDefinitionTermBeginning(),
                visitor => visitor.VisitDefinitionTermEnding(),
                visitor => visitor.VisitDefinitionTermDescriptionBeginning(),
                visitor => visitor.VisitDefinitionTermDescriptionEnding(),
                visitor => visitor.VisitDefinitionListItemEnding()
            );
        }

        [Fact]
        public async Task DefinitionListItemElementCallsVisitorMethods()
        {
            const string termText = "term plain text";
            const string termCode = "term piece of code";
            var termReferredMember = GetType().GetMembers().First();
            const string termParameter = "termParameter";
            const string termGenericParameter = "termGenericParameter";

            const string descriptionText = "description plain text";
            const string descriptionCode = "description piece of code";
            var descriptionReferredMember = GetType().GetMembers().Last();
            const string descriptionParameter = "descriptionParameter";
            const string descriptionGenericParameter = "descriptionGenericParameter";

            var visitorMock = new Mock<IDocumentationVisitor>();
            var definitionListItem = DocumentationElement.DefinitionListItem(
                new InlineDocumentationElement[]
                {
                    DocumentationElement.Text(termText),
                    DocumentationElement.InlineCode(termCode),
                    DocumentationElement.MemberReference(termReferredMember),
                    DocumentationElement.ParameterReference(termParameter),
                    DocumentationElement.GenericParameterReference(termGenericParameter)
                },
                new InlineDocumentationElement[]
                {
                    DocumentationElement.Text(descriptionText),
                    DocumentationElement.InlineCode(descriptionCode),
                    DocumentationElement.MemberReference(descriptionReferredMember),
                    DocumentationElement.ParameterReference(descriptionParameter),
                    DocumentationElement.GenericParameterReference(descriptionGenericParameter)
                }
            );

            await visitorMock.VerifyAcceptMethods(
                definitionListItem,
                visitor => visitor.VisitDefinitionListItemBeginning(),

                visitor => visitor.VisitDefinitionTermBeginning(),
                visitor => visitor.VisitText(termText),
                visitor => visitor.VisitInlineCode(termCode),
                visitor => visitor.VisitInlineReference(termReferredMember),
                visitor => visitor.VisitParameterReference(termParameter),
                visitor => visitor.VisitGenericParameterReference(termGenericParameter),
                visitor => visitor.VisitDefinitionTermEnding(),

                visitor => visitor.VisitDefinitionTermDescriptionBeginning(),
                visitor => visitor.VisitText(descriptionText),
                visitor => visitor.VisitInlineCode(descriptionCode),
                visitor => visitor.VisitInlineReference(descriptionReferredMember),
                visitor => visitor.VisitParameterReference(descriptionParameter),
                visitor => visitor.VisitGenericParameterReference(descriptionGenericParameter),
                visitor => visitor.VisitDefinitionTermDescriptionEnding(),

                visitor => visitor.VisitDefinitionListItemEnding()
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
                "listTitle",
                () => DocumentationElement.DefinitionList(
                    new InlineDocumentationElement[] { null },
                    Enumerable.Empty<DefinitionListItemDocumentationElement>()
                )
            );

            Assert.Equal(new ArgumentException("Cannot contain 'null' elements.", "listTitle").Message, exception.Message);
        }

        [Fact]
        public async Task DefinitionListElementCallsVisitorBeginningAndEndingMethods()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var definitionList = DocumentationElement.DefinitionList(Enumerable.Empty<DefinitionListItemDocumentationElement>());

            await visitorMock.VerifyAcceptMethods(
                definitionList,
                visitor => visitor.VisitDefinitionListBeginning(),
                visitor => visitor.VisitDefinitionListEnding()
            );
        }

        [Fact]
        public async Task DefinitionListElementCallsVisitorMethods()
        {
            const string text = "plain text";
            const string code = "piece of code";
            var referredMember = GetType().GetMembers().First();
            const string parameter = "parameter";
            const string genericParameter = "genericParameter";

            var visitorMock = new Mock<IDocumentationVisitor>();
            var definitionList = DocumentationElement.DefinitionList(
                new InlineDocumentationElement[]
                {
                    DocumentationElement.Text(text),
                    DocumentationElement.InlineCode(code),
                    DocumentationElement.MemberReference(referredMember),
                    DocumentationElement.ParameterReference(parameter),
                    DocumentationElement.GenericParameterReference(genericParameter)
                },
                new[]
                {
                    DocumentationElement.DefinitionListItem(
                        Enumerable.Empty<InlineDocumentationElement>(),
                        Enumerable.Empty<InlineDocumentationElement>()
                    ),
                    DocumentationElement.DefinitionListItem(
                        Enumerable.Empty<InlineDocumentationElement>(),
                        Enumerable.Empty<InlineDocumentationElement>()
                    )
                }
            );

            await visitorMock.VerifyAcceptMethods(
                definitionList,
                new InvocationCheck(visitor => visitor.VisitDefinitionListBeginning(), Times.Once()),

                new InvocationCheck(visitor => visitor.VisitDefinitionListTitleBeginning(), Times.Once()),
                new InvocationCheck(visitor => visitor.VisitText(text), Times.Once()),
                new InvocationCheck(visitor => visitor.VisitInlineCode(code), Times.Once()),
                new InvocationCheck(visitor => visitor.VisitInlineReference(referredMember), Times.Once()),
                new InvocationCheck(visitor => visitor.VisitParameterReference(parameter), Times.Once()),
                new InvocationCheck(visitor => visitor.VisitGenericParameterReference(genericParameter), Times.Once()),
                new InvocationCheck(visitor => visitor.VisitDefinitionListTitleEnding(), Times.Once()),

                new InvocationCheck(visitor => visitor.VisitDefinitionListItemBeginning(), Times.Exactly(2)),
                new InvocationCheck(visitor => visitor.VisitDefinitionTermBeginning(), Times.Exactly(2)),
                new InvocationCheck(visitor => visitor.VisitDefinitionTermEnding(), Times.Exactly(2)),

                new InvocationCheck(visitor => visitor.VisitDefinitionTermDescriptionBeginning(), Times.Exactly(2)),
                new InvocationCheck(visitor => visitor.VisitDefinitionTermDescriptionEnding(), Times.Exactly(2)),
                new InvocationCheck(visitor => visitor.VisitDefinitionListItemEnding(), Times.Exactly(2)),

                new InvocationCheck(visitor => visitor.VisitDefinitionListEnding(), Times.Once())
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
        public async Task TableCellElementCallsVisitorBeginningAndEndingMethods()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var tableCell = DocumentationElement.TableCell(Enumerable.Empty<InlineDocumentationElement>());

            await visitorMock.VerifyAcceptMethods(
                tableCell,
                visitor => visitor.VisitTableCellBeginning(),
                visitor => visitor.VisitTableCellEnding()
            );
        }

        [Fact]
        public async Task TableCellElementCallsVisitorMethods()
        {
            const string text = "plain text";
            const string code = "piece of code";
            var referredMember = GetType().GetMembers().First();
            var parameter = "parameter";
            var genericParameter = "genericParameter";

            var visitorMock = new Mock<IDocumentationVisitor>();
            var tableCell = DocumentationElement.TableCell(
                new InlineDocumentationElement[]
                {
                    DocumentationElement.Text(text),
                    DocumentationElement.InlineCode(code),
                    DocumentationElement.MemberReference(referredMember),
                    DocumentationElement.ParameterReference(parameter),
                    DocumentationElement.GenericParameterReference(genericParameter)
                }
            );

            await visitorMock.VerifyAcceptMethods(
                tableCell,
                visitor => visitor.VisitTableCellBeginning(),

                visitor => visitor.VisitText(text),
                visitor => visitor.VisitInlineCode(code),
                visitor => visitor.VisitInlineReference(referredMember),
                visitor => visitor.VisitParameterReference(parameter),
                visitor => visitor.VisitGenericParameterReference(genericParameter),

                visitor => visitor.VisitTableCellEnding()
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
        public async Task TableRowElementCallsVisitorBeginningAndEndingMethods()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var tableRow = DocumentationElement.TableRow(Enumerable.Empty<TableCellDocumentationElement>());

            await visitorMock.VerifyAcceptMethods(
                tableRow,
                visitor => visitor.VisitTableRowBeginning(),
                visitor => visitor.VisitTableRowEnding()
            );
        }

        [Fact]
        public async Task TableRowElementCallsVisitorMethods()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var tableRow = DocumentationElement.TableRow(
                new[]
                {
                    DocumentationElement.TableCell(Enumerable.Empty<InlineDocumentationElement>()),
                    DocumentationElement.TableCell(Enumerable.Empty<InlineDocumentationElement>())
                });

            await visitorMock.VerifyAcceptMethods(
                tableRow,
                new InvocationCheck(visitor => visitor.VisitTableRowBeginning(), Times.Once()),

                new InvocationCheck(visitor => visitor.VisitTableCellBeginning(), Times.Exactly(2)),
                new InvocationCheck(visitor => visitor.VisitTableCellEnding(), Times.Exactly(2)),

                new InvocationCheck(visitor => visitor.VisitTableRowEnding(), Times.Once())
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
        public async Task TableColumnElementCallsVisitorBeginningAndEndingMethods()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var tableColumn = DocumentationElement.TableColumn(Enumerable.Empty<InlineDocumentationElement>());

            await visitorMock.VerifyAcceptMethods(
                tableColumn,
                visitor => visitor.VisitTableColumnBeginning(),
                visitor => visitor.VisitTableColumnEnding()
            );
        }

        [Fact]
        public async Task TableColumnElementCallsVisitorMethods()
        {
            const string text = "plain text";
            const string code = "piece of code";
            var referredMember = GetType().GetMembers().First();
            const string parameter = "parameter";
            const string genericParameter = "genericParameter";

            var visitorMock = new Mock<IDocumentationVisitor>();
            var tableColumn = DocumentationElement.TableColumn(
                new InlineDocumentationElement[]
                {
                    DocumentationElement.Text(text),
                    DocumentationElement.InlineCode(code),
                    DocumentationElement.MemberReference(referredMember),
                    DocumentationElement.ParameterReference(parameter),
                    DocumentationElement.GenericParameterReference(genericParameter)
                }
            );

            await visitorMock.VerifyAcceptMethods(
                tableColumn,
                visitor => visitor.VisitTableColumnBeginning(),

                visitor => visitor.VisitText(text),
                visitor => visitor.VisitInlineCode(code),
                visitor => visitor.VisitInlineReference(referredMember),
                visitor => visitor.VisitParameterReference(parameter),
                visitor => visitor.VisitGenericParameterReference(genericParameter),

                visitor => visitor.VisitTableColumnEnding()
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
        public async Task TableElementCallsVisitorBeginningAndEndingMethods()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var tableColumn = DocumentationElement.Table(
                Enumerable.Empty<TableColumnDocumentationElement>(),
                Enumerable.Empty<TableRowDocumentationElement>()
            );

            await visitorMock.VerifyAcceptMethods(
                tableColumn,
                visitor => visitor.VisitTableBeginning(),
                visitor => visitor.VisitTableEnding()
            );
        }

        [Fact]
        public async Task TableElementCallsVisitorMethods()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var table = DocumentationElement.Table(
                new[]
                {
                    DocumentationElement.TableColumn(Enumerable.Empty<InlineDocumentationElement>()),
                    DocumentationElement.TableColumn(Enumerable.Empty<InlineDocumentationElement>())
                },
                new[]
                {
                    DocumentationElement.TableRow(Enumerable.Empty<TableCellDocumentationElement>()),
                    DocumentationElement.TableRow(Enumerable.Empty<TableCellDocumentationElement>())
                }
            );

            await visitorMock.VerifyAcceptMethods(
                table,
                new InvocationCheck(visitor => visitor.VisitTableBeginning(), Times.Once()),

                new InvocationCheck(visitor => visitor.VisitTableHeadingBeginning(), Times.Once()),
                new InvocationCheck(visitor => visitor.VisitTableColumnBeginning(), Times.Exactly(2)),
                new InvocationCheck(visitor => visitor.VisitTableColumnEnding(), Times.Exactly(2)),
                new InvocationCheck(visitor => visitor.VisitTableHeadingEnding(), Times.Once()),

                new InvocationCheck(visitor => visitor.VisitTableBodyBeginning(), Times.Once()),

                new InvocationCheck(visitor => visitor.VisitTableRowBeginning(), Times.Exactly(2)),
                new InvocationCheck(visitor => visitor.VisitTableCellBeginning(), Times.Exactly(4)),
                new InvocationCheck(visitor => visitor.VisitTableCellEnding(), Times.Exactly(4)),
                new InvocationCheck(visitor => visitor.VisitTableRowEnding(), Times.Exactly(2)),

                new InvocationCheck(visitor => visitor.VisitTableBodyEnding(), Times.Once()),

                new InvocationCheck(visitor => visitor.VisitTableEnding(), Times.Once())
            );
        }

        [Fact]
        public async Task TableElementCallsVisitorMethodsForMissingColumns()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var table = DocumentationElement.Table(
                new[]
                {
                    DocumentationElement.TableColumn(Enumerable.Empty<InlineDocumentationElement>())
                },
                new[]
                {
                    DocumentationElement.TableRow(
                        new[]
                        {
                            DocumentationElement.TableCell(Enumerable.Empty<InlineDocumentationElement>()),
                            DocumentationElement.TableCell(Enumerable.Empty<InlineDocumentationElement>())
                        }
                    ),
                    DocumentationElement.TableRow(
                        new[]
                        {
                            DocumentationElement.TableCell(Enumerable.Empty<InlineDocumentationElement>()),
                            DocumentationElement.TableCell(Enumerable.Empty<InlineDocumentationElement>())
                        }
                    )
                }
            );

            await visitorMock.VerifyAcceptMethods(
                table,
                new[]
                {
                    new InvocationCheck(visitor => visitor.VisitTableBeginning(), Times.Once()),

                    new InvocationCheck(visitor => visitor.VisitTableHeadingBeginning(), Times.Once()),
                    new InvocationCheck(visitor => visitor.VisitTableColumnBeginning(), Times.Exactly(2)),
                    new InvocationCheck(visitor => visitor.VisitTableColumnEnding(), Times.Exactly(2)),
                    new InvocationCheck(visitor => visitor.VisitTableHeadingEnding(), Times.Once()),

                    new InvocationCheck(visitor => visitor.VisitTableBodyBeginning(), Times.Once()),
                    new InvocationCheck(visitor => visitor.VisitTableRowBeginning(), Times.Exactly(2)),
                    new InvocationCheck(visitor => visitor.VisitTableCellBeginning(), Times.Exactly(4)),
                    new InvocationCheck(visitor => visitor.VisitTableCellEnding(), Times.Exactly(4)),
                    new InvocationCheck(visitor => visitor.VisitTableRowEnding(), Times.Exactly(2)),
                    new InvocationCheck(visitor => visitor.VisitTableBodyEnding(), Times.Once()),

                    new InvocationCheck(visitor => visitor.VisitTableEnding(), Times.Once())
                }
            );
        }

        [Fact]
        public async Task TableElementCallsVisitorMethodsForMissingCells()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var table = DocumentationElement.Table(
                new[]
                {
                    DocumentationElement.TableColumn(Enumerable.Empty<InlineDocumentationElement>()),
                    DocumentationElement.TableColumn(Enumerable.Empty<InlineDocumentationElement>())
                },
                new[]
                {
                    DocumentationElement.TableRow(
                        new[]
                        {
                            DocumentationElement.TableCell(Enumerable.Empty<InlineDocumentationElement>()),
                            DocumentationElement.TableCell(Enumerable.Empty<InlineDocumentationElement>())
                        }
                    ),
                    DocumentationElement.TableRow(
                        new[]
                        {
                            DocumentationElement.TableCell(Enumerable.Empty<InlineDocumentationElement>())
                        }
                    )
                }
            );

            await visitorMock.VerifyAcceptMethods(
                table,
                new[]
                {
                    new InvocationCheck(visitor => visitor.VisitTableBeginning(), Times.Once()),

                    new InvocationCheck(visitor => visitor.VisitTableHeadingBeginning(), Times.Once()),
                    new InvocationCheck(visitor => visitor.VisitTableColumnBeginning(), Times.Exactly(2)),
                    new InvocationCheck(visitor => visitor.VisitTableColumnEnding(), Times.Exactly(2)),
                    new InvocationCheck(visitor => visitor.VisitTableHeadingEnding(), Times.Once()),

                    new InvocationCheck(visitor => visitor.VisitTableBodyBeginning(), Times.Once()),
                    new InvocationCheck(visitor => visitor.VisitTableRowBeginning(), Times.Exactly(2)),
                    new InvocationCheck(visitor => visitor.VisitTableCellBeginning(), Times.Exactly(4)),
                    new InvocationCheck(visitor => visitor.VisitTableCellEnding(), Times.Exactly(4)),
                    new InvocationCheck(visitor => visitor.VisitTableRowEnding(), Times.Exactly(2)),
                    new InvocationCheck(visitor => visitor.VisitTableBodyEnding(), Times.Once()),

                    new InvocationCheck(visitor => visitor.VisitTableEnding(), Times.Once())
                }
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
        public async Task HeaderlessTableElementCallsVisitorBeginningAndEndingMethods()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var tableColumn = DocumentationElement.Table(
                Enumerable.Empty<TableRowDocumentationElement>()
            );

            await visitorMock.VerifyAcceptMethods(
                tableColumn,
                visitor => visitor.VisitTableBeginning(),
                visitor => visitor.VisitTableEnding()
            );
        }

        [Fact]
        public async Task HeaderlessTableElementCallsVisitorMethods()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var table = DocumentationElement.Table(
                DocumentationElement.TableRow(
                    DocumentationElement.TableCell(
                        Enumerable.Empty<InlineDocumentationElement>()
                    )
                ),
                DocumentationElement.TableRow(
                    DocumentationElement.TableCell(
                        Enumerable.Empty<InlineDocumentationElement>()
                    )
                )
            );

            await visitorMock.VerifyAcceptMethods(
                table,
                new InvocationCheck(visitor => visitor.VisitTableBeginning(), Times.Once()),
                new InvocationCheck(visitor => visitor.VisitTableBodyBeginning(), Times.Once()),

                new InvocationCheck(visitor => visitor.VisitTableRowBeginning(), Times.Exactly(2)),
                new InvocationCheck(visitor => visitor.VisitTableCellBeginning(), Times.Exactly(2)),
                new InvocationCheck(visitor => visitor.VisitTableCellEnding(), Times.Exactly(2)),
                new InvocationCheck(visitor => visitor.VisitTableRowEnding(), Times.Exactly(2)),

                new InvocationCheck(visitor => visitor.VisitTableBodyEnding(), Times.Once()),
                new InvocationCheck(visitor => visitor.VisitTableEnding(), Times.Once())
            );
        }

        [Fact]
        public async Task HeaderlessTableElementCallsVisitorMethodsForMissingCells()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var table = DocumentationElement.Table(
                DocumentationElement.TableRow(
                    DocumentationElement.TableCell(Enumerable.Empty<InlineDocumentationElement>()),
                    DocumentationElement.TableCell(Enumerable.Empty<InlineDocumentationElement>())
                ),
                DocumentationElement.TableRow(
                    DocumentationElement.TableCell(Enumerable.Empty<InlineDocumentationElement>())
                )
            );

            await visitorMock.VerifyAcceptMethods(
                table,
                new[]
                {
                    new InvocationCheck(visitor => visitor.VisitTableBeginning(), Times.Once()),
                    new InvocationCheck(visitor => visitor.VisitTableBodyBeginning(), Times.Once()),

                    new InvocationCheck(visitor => visitor.VisitTableRowBeginning(), Times.Exactly(2)),
                    new InvocationCheck(visitor => visitor.VisitTableCellBeginning(), Times.Exactly(4)),
                    new InvocationCheck(visitor => visitor.VisitTableCellEnding(), Times.Exactly(4)),
                    new InvocationCheck(visitor => visitor.VisitTableRowEnding(), Times.Exactly(2)),

                    new InvocationCheck(visitor => visitor.VisitTableBodyEnding(), Times.Once()),
                    new InvocationCheck(visitor => visitor.VisitTableEnding(), Times.Once())
                }
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
        public async Task SummaryElementCallsBeginningAndEndingVisitorMethods()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var summary = DocumentationElement.Summary(Enumerable.Empty<BlockDocumentationElement>());

            await visitorMock.VerifyAcceptMethods(
                summary,
                visitor => visitor.VisitSummaryBeginning(),
                visitor => visitor.VisitSummaryEnding()
            );
        }

        [Fact]
        public async Task SummaryElementCallsVisitorMethods()
        {
            const string code = "piece of code";

            var visitorMock = new Mock<IDocumentationVisitor>();
            var summary = DocumentationElement.Summary(
                new BlockDocumentationElement[]
                {
                    DocumentationElement.Paragraph(Enumerable.Empty<InlineDocumentationElement>()),
                    DocumentationElement.CodeBlock(code),
                    DocumentationElement.UnorderedList(Enumerable.Empty<ListItemDocumentationElement>()),
                    DocumentationElement.OrderedList(Enumerable.Empty<ListItemDocumentationElement>()),
                    DocumentationElement.DefinitionList(Enumerable.Empty<DefinitionListItemDocumentationElement>()),
                    DocumentationElement.Table(Enumerable.Empty<TableColumnDocumentationElement>(), Enumerable.Empty<TableRowDocumentationElement>())
                });

            await visitorMock.VerifyAcceptMethods(
                summary,
                visitor => visitor.VisitSummaryBeginning(),

                visitor => visitor.VisitParagraphBeginning(),
                visitor => visitor.VisitParagraphEnding(),

                visitor => visitor.VisitCodeBlock(code),

                visitor => visitor.VisitUnorderedListBeginning(),
                visitor => visitor.VisitUnorderedListEnding(),

                visitor => visitor.VisitOrderedListBeginning(),
                visitor => visitor.VisitOrderedListEnding(),

                visitor => visitor.VisitDefinitionListBeginning(),
                visitor => visitor.VisitDefinitionListEnding(),

                visitor => visitor.VisitTableBeginning(),
                visitor => visitor.VisitTableEnding(),

                visitor => visitor.VisitSummaryEnding()
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
        public async Task RemarksElementCallsBeginningAndEndingVisitorMethods()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var remarks = DocumentationElement.Remarks(Enumerable.Empty<BlockDocumentationElement>());

            await visitorMock.VerifyAcceptMethods(
                remarks,
                visitor => visitor.VisitRemarksBeginning(),
                visitor => visitor.VisitRemarksEnding()
            );
        }

        [Fact]
        public async Task RemarksElementCallsVisitorMethods()
        {
            const string code = "piece of code";

            var visitorMock = new Mock<IDocumentationVisitor>();
            var remarks = DocumentationElement.Remarks(
                new BlockDocumentationElement[]
                {
                    DocumentationElement.Paragraph(Enumerable.Empty<InlineDocumentationElement>()),
                    DocumentationElement.CodeBlock(code),
                    DocumentationElement.UnorderedList(Enumerable.Empty<ListItemDocumentationElement>()),
                    DocumentationElement.OrderedList(Enumerable.Empty<ListItemDocumentationElement>()),
                    DocumentationElement.DefinitionList(Enumerable.Empty<DefinitionListItemDocumentationElement>()),
                    DocumentationElement.Table(Enumerable.Empty<TableColumnDocumentationElement>(), Enumerable.Empty<TableRowDocumentationElement>())
                });

            await visitorMock.VerifyAcceptMethods(
                remarks,
                visitor => visitor.VisitRemarksBeginning(),

                visitor => visitor.VisitParagraphBeginning(),
                visitor => visitor.VisitParagraphEnding(),

                visitor => visitor.VisitCodeBlock(code),

                visitor => visitor.VisitUnorderedListBeginning(),
                visitor => visitor.VisitUnorderedListEnding(),

                visitor => visitor.VisitOrderedListBeginning(),
                visitor => visitor.VisitOrderedListEnding(),

                visitor => visitor.VisitDefinitionListBeginning(),
                visitor => visitor.VisitDefinitionListEnding(),

                visitor => visitor.VisitTableBeginning(),
                visitor => visitor.VisitTableEnding(),

                visitor => visitor.VisitRemarksEnding()
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
        public async Task ExampleElementCallsBeginningAndEndingVisitorMethods()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var example = DocumentationElement.Example(Enumerable.Empty<BlockDocumentationElement>());

            await visitorMock.VerifyAcceptMethods(
                example,
                visitor => visitor.VisitExampleBeginning(),
                visitor => visitor.VisitExampleEnding()
            );
        }

        [Fact]
        public async Task ExampleElementCallsVisitorMethods()
        {
            const string code = "piece of code";

            var visitorMock = new Mock<IDocumentationVisitor>();
            var example = DocumentationElement.Example(
                new BlockDocumentationElement[]
                {
                    DocumentationElement.Paragraph(Enumerable.Empty<InlineDocumentationElement>()),
                    DocumentationElement.CodeBlock(code),
                    DocumentationElement.UnorderedList(Enumerable.Empty<ListItemDocumentationElement>()),
                    DocumentationElement.OrderedList(Enumerable.Empty<ListItemDocumentationElement>()),
                    DocumentationElement.DefinitionList(Enumerable.Empty<DefinitionListItemDocumentationElement>()),
                    DocumentationElement.Table(Enumerable.Empty<TableColumnDocumentationElement>(), Enumerable.Empty<TableRowDocumentationElement>())
                });

            await visitorMock.VerifyAcceptMethods(
                example,
                visitor => visitor.VisitExampleBeginning(),

                visitor => visitor.VisitParagraphBeginning(),
                visitor => visitor.VisitParagraphEnding(),

                visitor => visitor.VisitCodeBlock(code),

                visitor => visitor.VisitUnorderedListBeginning(),
                visitor => visitor.VisitUnorderedListEnding(),

                visitor => visitor.VisitOrderedListBeginning(),
                visitor => visitor.VisitOrderedListEnding(),

                visitor => visitor.VisitDefinitionListBeginning(),
                visitor => visitor.VisitDefinitionListEnding(),

                visitor => visitor.VisitTableBeginning(),
                visitor => visitor.VisitTableEnding(),

                visitor => visitor.VisitExampleEnding()
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
        public async Task ValueElementCallsBeginningAndEndingVisitorMethods()
        {
            var visitorMock = new Mock<IDocumentationVisitor>();
            var value = DocumentationElement.Value(Enumerable.Empty<BlockDocumentationElement>());

            await visitorMock.VerifyAcceptMethods(
                value,
                visitor => visitor.VisitValueBeginning(),
                visitor => visitor.VisitValueEnding()
            );
        }

        [Fact]
        public async Task ValueElementCallsVisitorMethods()
        {
            const string code = "piece of code";

            var visitorMock = new Mock<IDocumentationVisitor>();
            var value = DocumentationElement.Value(
                new BlockDocumentationElement[]
                {
                    DocumentationElement.Paragraph(Enumerable.Empty<InlineDocumentationElement>()),
                    DocumentationElement.CodeBlock(code),
                    DocumentationElement.UnorderedList(Enumerable.Empty<ListItemDocumentationElement>()),
                    DocumentationElement.OrderedList(Enumerable.Empty<ListItemDocumentationElement>()),
                    DocumentationElement.DefinitionList(Enumerable.Empty<DefinitionListItemDocumentationElement>()),
                    DocumentationElement.Table(Enumerable.Empty<TableColumnDocumentationElement>(), Enumerable.Empty<TableRowDocumentationElement>())
                });

            await visitorMock.VerifyAcceptMethods(
                value,
                visitor => visitor.VisitValueBeginning(),

                visitor => visitor.VisitParagraphBeginning(),
                visitor => visitor.VisitParagraphEnding(),

                visitor => visitor.VisitCodeBlock(code),

                visitor => visitor.VisitUnorderedListBeginning(),
                visitor => visitor.VisitUnorderedListEnding(),

                visitor => visitor.VisitOrderedListBeginning(),
                visitor => visitor.VisitOrderedListEnding(),

                visitor => visitor.VisitDefinitionListBeginning(),
                visitor => visitor.VisitDefinitionListEnding(),

                visitor => visitor.VisitTableBeginning(),
                visitor => visitor.VisitTableEnding(),

                visitor => visitor.VisitValueEnding()
            );
        }
    }
}