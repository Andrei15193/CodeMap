using System;
using CodeMap.DocumentationElements;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public abstract class DocumentationElementTests
    {
        protected sealed class DocumentationVisitorMock<TDocumentationElement> : DocumentationVisitor
            where TDocumentationElement : DocumentationElement
        {
            private readonly TDocumentationElement _expectedDocumentationElement;

            public DocumentationVisitorMock(TDocumentationElement documentationElement)
                => _expectedDocumentationElement = documentationElement;

            public int VisitCount { get; private set; }

            protected override void VisitSummary(SummaryDocumentationElement summary)
                => _InvokeCallback(summary);

            protected override void VisitException(ExceptionDocumentationElement exception)
                => _InvokeCallback(exception);

            protected override void VisitRemarks(RemarksDocumentationElement remarks)
                => _InvokeCallback(remarks);

            protected override void VisitExample(ExampleDocumentationElement example)
                => _InvokeCallback(example);

            protected override void VisitValue(ValueDocumentationElement value)
                => _InvokeCallback(value);

            protected override void VisitParagraph(ParagraphDocumentationElement paragraph)
                => _InvokeCallback(paragraph);

            protected override void VisitCodeBlock(CodeBlockDocumentationElement codeBlock)
                => _InvokeCallback(codeBlock);

            protected override void VisitUnorderedList(UnorderedListDocumentationElement unorderedList)
                => _InvokeCallback(unorderedList);

            protected override void VisitOrderedList(OrderedListDocumentationElement orderedList)
                => _InvokeCallback(orderedList);

            protected override void VisitListItem(ListItemDocumentationElement listItem)
                => _InvokeCallback(listItem);

            protected override void VisitDefinitionList(DefinitionListDocumentationElement definitionList)
                => _InvokeCallback(definitionList);

            protected override void VisitDefinitionListTitle(DefinitionListTitleDocumentationElement definitionListTitle)
                => _InvokeCallback(definitionListTitle);

            protected override void VisitDefinitionListItem(DefinitionListItemDocumentationElement definitionListItem)
                => _InvokeCallback(definitionListItem);

            protected override void VisitDefinitionListItemTerm(DefinitionListItemTermDocumentationElement definitionListItemTerm)
                => _InvokeCallback(definitionListItemTerm);

            protected override void VisitDefinitionListItemDescription(DefinitionListItemDescriptionDocumentationElement definitionListItemDescription)
                => _InvokeCallback(definitionListItemDescription);

            protected override void VisitTable(TableDocumentationElement table)
                => _InvokeCallback(table);

            protected override void VisitTableColumn(TableColumnDocumentationElement tableColumn)
                => _InvokeCallback(tableColumn);

            protected override void VisitTableRow(TableRowDocumentationElement tableRow)
                => _InvokeCallback(tableRow);

            protected override void VisitTableCell(TableCellDocumentationElement tableCell)
                => _InvokeCallback(tableCell);

            protected override void VisitText(TextDocumentationElement text)
                => _InvokeCallback(text);

            protected override void VisitHyperlink(HyperlinkDocumentationElement hyperlink)
                => _InvokeCallback(hyperlink);

            protected override void VisitInlineReference(MemberNameReferenceDocumentationElement memberNameReference)
                => _InvokeCallback(memberNameReference);

            protected override void VisitInlineReference(ReferenceDataDocumentationElement memberInfoReference)
                => _InvokeCallback(memberInfoReference);

            protected override void VisitInlineCode(InlineCodeDocumentationElement inlineCode)
                => _InvokeCallback(inlineCode);

            protected override void VisitParameterReference(ParameterReferenceDocumentationElement parameterReference)
                => _InvokeCallback(parameterReference);

            protected override void VisitGenericParameterReference(GenericParameterReferenceDocumentationElement genericParameterReference)
                => _InvokeCallback(genericParameterReference);

            private void _InvokeCallback<TVisitedDocumentationElement>(TVisitedDocumentationElement actualDeclarationNode)
                where TVisitedDocumentationElement : DocumentationElement
            {
                if (!typeof(TVisitedDocumentationElement).IsAssignableFrom(typeof(TDocumentationElement)))
                    throw new NotImplementedException();

                VisitCount++;
                Assert.Same(_expectedDocumentationElement, actualDeclarationNode);
            }
        }
    }
}