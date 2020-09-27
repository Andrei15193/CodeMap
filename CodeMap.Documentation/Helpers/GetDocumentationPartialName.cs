using System;
using System.IO;
using System.Linq;
using CodeMap.DocumentationElements;

namespace CodeMap.Documentation.Helpers
{
    public class GetDocumentationPartialName : IHandlebarsHelper
    {
        public string Name
            => nameof(GetDocumentationPartialName);

        public void Apply(TextWriter writer, object context, params object[] parameters)
        {
            var documentationElement = parameters.DefaultIfEmpty(context).First() as DocumentationElement;
            if (documentationElement is null)
                throw new ArgumentException("Expected a " + nameof(DocumentationElement) + " provided as the first parameter or context.");

            var visitor = new PartialNameSelector();
            documentationElement.Accept(visitor);
            writer.Write(visitor.PartialName);
        }

        private sealed class PartialNameSelector : DocumentationVisitor
        {
            public string PartialName { get; private set; }

            protected override void VisitSummary(SummaryDocumentationElement summary)
                => PartialName = DocumentationPartialNames.Summary;

            protected override void VisitRemarks(RemarksDocumentationElement remarks)
                => PartialName = DocumentationPartialNames.Remarks;

            protected override void VisitExample(ExampleDocumentationElement example)
                => PartialName = DocumentationPartialNames.Example;

            protected override void VisitValue(ValueDocumentationElement value)
                => PartialName = DocumentationPartialNames.Value;

            protected override void VisitParagraph(ParagraphDocumentationElement paragraph)
                => PartialName = DocumentationPartialNames.Paragraph;

            protected override void VisitCodeBlock(CodeBlockDocumentationElement codeBlock)
                => PartialName = DocumentationPartialNames.CodeBlock;

            protected override void VisitUnorderedList(UnorderedListDocumentationElement unorderedList)
                => PartialName = DocumentationPartialNames.UnorderedList;

            protected override void VisitOrderedList(OrderedListDocumentationElement orderedList)
                => PartialName = DocumentationPartialNames.OrderedList;

            protected override void VisitListItem(ListItemDocumentationElement listItem)
                => PartialName = DocumentationPartialNames.ListItem;

            protected override void VisitDefinitionList(DefinitionListDocumentationElement definitionList)
                => PartialName = DocumentationPartialNames.DefinitionList;

            protected override void VisitDefinitionListTitle(DefinitionListTitleDocumentationElement definitionListTitle)
                => PartialName = DocumentationPartialNames.DefinitionListTitle;

            protected override void VisitDefinitionListItem(DefinitionListItemDocumentationElement definitionListItem)
                => PartialName = DocumentationPartialNames.DefinitionListItem;

            protected override void VisitDefinitionListItemTerm(DefinitionListItemTermDocumentationElement definitionListItemTerm)
                => PartialName = DocumentationPartialNames.DefinitionListItemTerm;

            protected override void VisitDefinitionListItemDescription(DefinitionListItemDescriptionDocumentationElement definitionListItemDescription)
                => PartialName = DocumentationPartialNames.DefinitionListItemDescription;

            protected override void VisitTable(TableDocumentationElement table)
                => PartialName = DocumentationPartialNames.Table;

            protected override void VisitTableColumn(TableColumnDocumentationElement tableColumn)
                => PartialName = DocumentationPartialNames.TableColumn;

            protected override void VisitTableRow(TableRowDocumentationElement tableRow)
                => PartialName = DocumentationPartialNames.TableRow;

            protected override void VisitTableCell(TableCellDocumentationElement tableCell)
                => PartialName = DocumentationPartialNames.TableCell;

            protected override void VisitText(TextDocumentationElement text)
                => PartialName = DocumentationPartialNames.Text;

            protected override void VisitHyperlink(HyperlinkDocumentationElement hyperlink)
                => PartialName = DocumentationPartialNames.Hyperlink;

            protected override void VisitInlineCode(InlineCodeDocumentationElement inlineCode)
                => PartialName = DocumentationPartialNames.Code;

            protected override void VisitInlineReference(MemberInfoReferenceDocumentationElement memberInfoReference)
                => PartialName = DocumentationPartialNames.Hyperlink;

            protected override void VisitGenericParameterReference(GenericParameterReferenceDocumentationElement genericParameterReference)
                => PartialName = DocumentationPartialNames.GenericParameterReference;

            protected override void VisitParameterReference(ParameterReferenceDocumentationElement parameterReference)
                => PartialName = DocumentationPartialNames.ParameterReference;
        }
    }
}