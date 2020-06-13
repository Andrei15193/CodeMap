using CodeMap.DocumentationElements;

namespace CodeMap.Tests.DocumentationElements
{
    public interface IDocumentationVisitor
    {
        void VisitSummary(SummaryDocumentationElement summary);

        void VisitRemarks(RemarksDocumentationElement remarks);

        void VisitExample(ExampleDocumentationElement example);

        void VisitValue(ValueDocumentationElement value);

        void VisitParagraph(ParagraphDocumentationElement paragraph);

        void VisitCodeBlock(CodeBlockDocumentationElement codeBlock);

        void VisitUnorderedList(UnorderedListDocumentationElement unorderedList);

        void VisitOrderedList(OrderedListDocumentationElement orderedList);

        void VisitListItem(ListItemDocumentationElement listItem);

        void VisitDefinitionList(DefinitionListDocumentationElement definitionList);

        void VisitDefinitionListTitle(DefinitionListTitleDocumentationElement definitionListTitle);

        void VisitDefinitionListItem(DefinitionListItemDocumentationElement definitionListItem);

        void VisitDefinitionListItemTerm(DefinitionListItemTermDocumentationElement definitionListItemTerm);

        void VisitDefinitionListItemDescription(DefinitionListItemDescriptionDocumentationElement definitionListItemDescription);

        void VisitTable(TableDocumentationElement table);

        void VisitTableColumn(TableColumnDocumentationElement tableColumn);

        void VisitTableRow(TableRowDocumentationElement tableRow);

        void VisitTableCell(TableCellDocumentationElement tableCell);

        void VisitText(TextDocumentationElement text);

        void VisitInlineReference(MemberNameReferenceDocumentationElement memberNameReference);

        void VisitInlineReference(MemberInfoReferenceDocumentationElement memberInfoReference);

        void VisitInlineCode(InlineCodeDocumentationElement inlineCode);

        void VisitParameterReference(ParameterReferenceDocumentationElement parameterReference);

        void VisitGenericParameterReference(GenericParameterReferenceDocumentationElement genericParameterReference);
    }
}