using CodeMap.DocumentationElements;

namespace CodeMap.Tests.DocumentationElements
{
    public class DocumentationVisitorAdapter : DocumentationVisitor
    {
        private readonly IDocumentationVisitor _documentationVisitor;

        public DocumentationVisitorAdapter(IDocumentationVisitor documentationVisitor)
            => _documentationVisitor = documentationVisitor;

        protected override void VisitCodeBlock(CodeBlockDocumentationElement codeBlock)
            => _documentationVisitor.VisitCodeBlock(codeBlock);

        protected override void VisitDefinitionList(DefinitionListDocumentationElement definitionList)
            => _documentationVisitor.VisitDefinitionList(definitionList);

        protected override void VisitDefinitionListTitle(DefinitionListTitleDocumentationElement definitionListTitle)
            => _documentationVisitor.VisitDefinitionListTitle(definitionListTitle);

        protected override void VisitDefinitionListItem(DefinitionListItemDocumentationElement definitionListItem)
            => _documentationVisitor.VisitDefinitionListItem(definitionListItem);

        protected override void VisitDefinitionListItemTerm(DefinitionListItemTermDocumentationElement definitionListItemTerm)
            => _documentationVisitor.VisitDefinitionListItemTerm(definitionListItemTerm);

        protected override void VisitDefinitionListItemDescription(DefinitionListItemDescriptionDocumentationElement definitionListItemDescription)
            => _documentationVisitor.VisitDefinitionListItemDescription(definitionListItemDescription);

        protected override void VisitExample(ExampleDocumentationElement example)
            => _documentationVisitor.VisitExample(example);

        protected override void VisitGenericParameterReference(GenericParameterReferenceDocumentationElement genericParameterReference)
            => _documentationVisitor.VisitGenericParameterReference(genericParameterReference);

        protected override void VisitInlineCode(InlineCodeDocumentationElement inlineCode)
            => _documentationVisitor.VisitInlineCode(inlineCode);

        protected override void VisitInlineReference(MemberInfoReferenceDocumentationElement memberInfoReference)
            => _documentationVisitor.VisitInlineReference(memberInfoReference);

        protected override void VisitListItem(ListItemDocumentationElement listItem)
            => _documentationVisitor.VisitListItem(listItem);

        protected override void VisitOrderedList(OrderedListDocumentationElement orderedList)
            => _documentationVisitor.VisitOrderedList(orderedList);

        protected override void VisitParagraph(ParagraphDocumentationElement paragraph)
            => _documentationVisitor.VisitParagraph(paragraph);

        protected override void VisitParameterReference(ParameterReferenceDocumentationElement parameterReference)
            => _documentationVisitor.VisitParameterReference(parameterReference);

        protected override void VisitRemarks(RemarksDocumentationElement remarks)
            => _documentationVisitor.VisitRemarks(remarks);

        protected override void VisitSummary(SummaryDocumentationElement summary)
            => _documentationVisitor.VisitSummary(summary);

        protected override void VisitTable(TableDocumentationElement table)
            => _documentationVisitor.VisitTable(table);

        protected override void VisitTableCell(TableCellDocumentationElement tableCell)
            => _documentationVisitor.VisitTableCell(tableCell);

        protected override void VisitTableColumn(TableColumnDocumentationElement tableColumn)
            => _documentationVisitor.VisitTableColumn(tableColumn);

        protected override void VisitTableRow(TableRowDocumentationElement tableRow)
            => _documentationVisitor.VisitTableRow(tableRow);

        protected override void VisitText(TextDocumentationElement text)
            => _documentationVisitor.VisitText(text);

        protected override void VisitUnorderedList(UnorderedListDocumentationElement unorderedList)
            => _documentationVisitor.VisitUnorderedList(unorderedList);

        protected override void VisitValue(ValueDocumentationElement value)
            => _documentationVisitor.VisitValue(value);
    }
}