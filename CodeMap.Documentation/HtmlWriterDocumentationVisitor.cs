using System;
using System.IO;
using System.Linq;
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
            => _writer.WriteTemplate("Summary", new TemplateContext<SummaryDocumentationElement>(_context, summary));

        protected override void VisitRemarks(RemarksDocumentationElement remarks)
        {
            if (remarks.Content.Any())
                _writer.WriteTemplate("Remarks", new TemplateContext<RemarksDocumentationElement>(_context, remarks));
        }

        protected override void VisitExample(ExampleDocumentationElement example)
        {
            if (example.Content.Any())
                _writer.WriteTemplate("Example", new TemplateContext<ExampleDocumentationElement>(_context, example));
        }

        protected override void VisitValue(ValueDocumentationElement value)
        {
            if (value.Content.Any())
                _writer.WriteTemplate("Value", new TemplateContext<ValueDocumentationElement>(_context, value));
        }

        protected override void VisitParagraph(ParagraphDocumentationElement paragraph)
            => _writer.WriteTemplate("Paragraph", new TemplateContext<ParagraphDocumentationElement>(_context, paragraph));

        protected override void VisitCodeBlock(CodeBlockDocumentationElement codeBlock)
            => _writer.WriteTemplate("CodeBlock", new TemplateContext<CodeBlockDocumentationElement>(_context, codeBlock));

        protected override void VisitUnorderedList(UnorderedListDocumentationElement unorderedList)
            => _writer.WriteTemplate("UnorderedList", new TemplateContext<UnorderedListDocumentationElement>(_context, unorderedList));

        protected override void VisitOrderedList(OrderedListDocumentationElement orderedList)
            => _writer.WriteTemplate("OrderedList", new TemplateContext<OrderedListDocumentationElement>(_context, orderedList));

        protected override void VisitListItem(ListItemDocumentationElement listItem)
            => _writer.WriteTemplate("ListItem", new TemplateContext<ListItemDocumentationElement>(_context, listItem));

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
            => _writer.WriteTemplate("Table", new TemplateContext<TableDocumentationElement>(_context, table));

        protected override void VisitTableColumn(TableColumnDocumentationElement tableColumn)
            => _writer.WriteTemplate("TableColumn", new TemplateContext<TableColumnDocumentationElement>(_context, tableColumn));

        protected override void VisitTableRow(TableRowDocumentationElement tableRow)
            => _writer.WriteTemplate("TableRow", new TemplateContext<TableRowDocumentationElement>(_context, tableRow));

        protected override void VisitTableCell(TableCellDocumentationElement tableCell)
            => _writer.WriteTemplate("TableCell", new TemplateContext<TableCellDocumentationElement>(_context, tableCell));

        protected override void VisitText(TextDocumentationElement text)
            => _writer.WriteTemplate("Text", new TemplateContext<TextDocumentationElement>(_context, text));

        protected override void VisitHyperlink(HyperlinkDocumentationElement hyperlink)
            => _writer.WriteTemplate("Hyperlink", new TemplateContext<HyperlinkDocumentationElement>(_context, hyperlink));

        protected override void VisitInlineReference(MemberInfoReferenceDocumentationElement memberInfoReference)
            => VisitHyperlink(DocumentationElement.Hyperlink(memberInfoReference.GetMemberUrl(_context.Assembly), memberInfoReference.ReferredMember.GetMemberName()));

        protected override void VisitInlineCode(InlineCodeDocumentationElement inlineCode)
            => _writer.WriteTemplate("Code", new TemplateContext<InlineCodeDocumentationElement>(_context, inlineCode));

        protected override void VisitParameterReference(ParameterReferenceDocumentationElement parameterReference)
            => _writer.WriteTemplate("ParameterReference", new TemplateContext<ParameterReferenceDocumentationElement>(_context, parameterReference));

        protected override void VisitGenericParameterReference(GenericParameterReferenceDocumentationElement genericParameterReference)
            => _writer.WriteTemplate("GenericParameterReference", new TemplateContext<GenericParameterReferenceDocumentationElement>(_context, genericParameterReference));
    }
}