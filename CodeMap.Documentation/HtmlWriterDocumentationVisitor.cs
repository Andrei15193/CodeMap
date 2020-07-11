using System;
using System.IO;
using CodeMap.DocumentationElements;

namespace CodeMap.Documentation
{
    public class HtmlWriterDocumentationVisitor : DocumentationVisitor
    {
        private readonly TextWriter _writer;
        private readonly PageContext _context;

        public HtmlWriterDocumentationVisitor(TextWriter writer, PageContext context)
            => (_writer, _context) = (writer, context);

        protected override void VisitSummary(SummaryDocumentationElement summary)
            => _writer.WriteTemplate("Summary", _context.WithData(summary));

        protected override void VisitRemarks(RemarksDocumentationElement remarks)
            => _writer.WriteTemplate("Remarks", _context.WithData(remarks));

        protected override void VisitExample(ExampleDocumentationElement example)
            => _writer.WriteTemplate("Example", _context.WithData(example));

        protected override void VisitValue(ValueDocumentationElement value)
            => _writer.WriteTemplate("Value", _context.WithData(value));

        protected override void VisitParagraph(ParagraphDocumentationElement paragraph)
            => _writer.WriteTemplate("Paragraph", _context.WithData(paragraph));

        protected override void VisitCodeBlock(CodeBlockDocumentationElement codeBlock)
            => _writer.WriteTemplate("CodeBlock", _context.WithData(codeBlock));

        protected override void VisitUnorderedList(UnorderedListDocumentationElement unorderedList)
            => _writer.WriteTemplate("UnorderedList", _context.WithData(unorderedList));

        protected override void VisitOrderedList(OrderedListDocumentationElement orderedList)
            => _writer.WriteTemplate("OrderedList", _context.WithData(orderedList));

        protected override void VisitListItem(ListItemDocumentationElement listItem)
            => _writer.WriteTemplate("ListItem", _context.WithData(listItem));

        protected override void VisitDefinitionList(DefinitionListDocumentationElement definitionList)
        {
            throw new NotImplementedException();
        }

        protected override void VisitDefinitionListTitle(DefinitionListTitleDocumentationElement definitionListTitle)
        {
            throw new NotImplementedException();
        }

        protected override void VisitDefinitionListItem(DefinitionListItemDocumentationElement definitionListItem)
        {
            throw new NotImplementedException();
        }

        protected override void VisitDefinitionListItemTerm(DefinitionListItemTermDocumentationElement definitionListItemTerm)
        {
            throw new NotImplementedException();
        }

        protected override void VisitDefinitionListItemDescription(DefinitionListItemDescriptionDocumentationElement definitionListItemDescription)
        {
            throw new NotImplementedException();
        }

        protected override void VisitTable(TableDocumentationElement table)
            => _writer.WriteTemplate("Table", _context.WithData(table));

        protected override void VisitTableColumn(TableColumnDocumentationElement tableColumn)
            => _writer.WriteTemplate("TableColumn", _context.WithData(tableColumn));

        protected override void VisitTableRow(TableRowDocumentationElement tableRow)
            => _writer.WriteTemplate("TableRow", _context.WithData(tableRow));

        protected override void VisitTableCell(TableCellDocumentationElement tableCell)
            => _writer.WriteTemplate("TableCell", _context.WithData(tableCell));

        protected override void VisitText(TextDocumentationElement text)
            => _writer.WriteTemplate("Text", _context.WithData(text));

        protected override void VisitHyperlink(HyperlinkDocumentationElement hyperlink)
            => _writer.WriteTemplate("Hyperlink", _context.WithData(hyperlink));

        protected override void VisitInlineReference(MemberInfoReferenceDocumentationElement memberInfoReference)
            => VisitHyperlink(DocumentationElement.Hyperlink(memberInfoReference.GetMemberUrl(_context.Assembly), memberInfoReference.ReferredMember.GetMemberName()));

        protected override void VisitInlineCode(InlineCodeDocumentationElement inlineCode)
            => _writer.WriteTemplate("Code", _context.WithData(inlineCode));

        protected override void VisitParameterReference(ParameterReferenceDocumentationElement parameterReference)
            => _writer.WriteTemplate("ParameterReference", _context.WithData(parameterReference));

        protected override void VisitGenericParameterReference(GenericParameterReferenceDocumentationElement genericParameterReference)
            => _writer.WriteTemplate("GenericParameterReference", _context.WithData(genericParameterReference));
    }
}