using System;

namespace CodeMap.DocumentationElements
{
    /// <summary>Represents a visitor for traversing documentation trees.</summary>
    public abstract class DocumentationVisitor
    {
        /// <summary>Initializes a new instance of the <see cref="DocumentationVisitor"/> class.</summary>
        protected DocumentationVisitor()
        {
        }

        /// <summary>Visits a summary element.</summary>
        /// <param name="summary">The <see cref="SummaryDocumentationElement"/> to visit.</param>
        protected internal abstract void VisitSummary(SummaryDocumentationElement summary);

        /// <summary>Visits a remarks element.</summary>
        /// <param name="remarks">The <see cref="RemarksDocumentationElement"/> to visit.</param>
        protected internal abstract void VisitRemarks(RemarksDocumentationElement remarks);

        /// <summary>Visits an example element.</summary>
        /// <param name="example">The <see cref="ExampleDocumentationElement"/> to visit.</param>
        protected internal abstract void VisitExample(ExampleDocumentationElement example);

        /// <summary>Visits a value element.</summary>
        /// <param name="value">The <see cref="ValueDocumentationElement"/> to visit.</param>
        protected internal abstract void VisitValue(ValueDocumentationElement value);

        /// <summary>Visits a paragraph element.</summary>
        /// <param name="paragraph">The <see cref="ParagraphDocumentationElement"/> to visit.</param>
        protected internal abstract void VisitParagraph(ParagraphDocumentationElement paragraph);

        /// <summary>Visits a code block element.</summary>
        /// <param name="codeBlock">The <see cref="CodeBlockDocumentationElement"/> to visit.</param>
        protected internal abstract void VisitCodeBlock(CodeBlockDocumentationElement codeBlock);

        /// <summary>Visits an unordered list element.</summary>
        /// <param name="unorderedList">The <see cref="UnorderedListDocumentationElement"/> to visit.</param>
        protected internal abstract void VisitUnorderedList(UnorderedListDocumentationElement unorderedList);

        /// <summary>Visits an ordered list element.</summary>
        /// <param name="orderedList">The <see cref="OrderedListDocumentationElement"/> to visit.</param>
        protected internal abstract void VisitOrderedList(OrderedListDocumentationElement orderedList);

        /// <summary>Visits a list item element.</summary>
        /// <param name="listItem">The <see cref="ListItemDocumentationElement"/> to visit.</param>
        protected internal abstract void VisitListItem(ListItemDocumentationElement listItem);

        /// <summary>Visits a definition list element.</summary>
        /// <param name="definitionList">The <see cref="DefinitionListDocumentationElement"/> to visit.</param>
        protected internal abstract void VisitDefinitionList(DefinitionListDocumentationElement definitionList);

        /// <summary>Visits a definition list title element.</summary>
        /// <param name="definitionListTitle">The <see cref="DefinitionListTitleDocumentationElement"/> to visit.</param>
        protected internal abstract void VisitDefinitionListTitle(DefinitionListTitleDocumentationElement definitionListTitle);

        /// <summary>Visits a definition list item.</summary>
        /// <param name="definitionListItem">The <see cref="DefinitionListItemDocumentationElement"/> to visit.</param>
        protected internal abstract void VisitDefinitionListItem(DefinitionListItemDocumentationElement definitionListItem);

        /// <summary>Visits a definition list item term.</summary>
        /// <param name="definitionListItemTerm">The <see cref="DefinitionListItemTermDocumentationElement"/> to visit.</param>
        protected internal abstract void VisitDefinitionListItemTerm(DefinitionListItemTermDocumentationElement definitionListItemTerm);

        /// <summary>Visits a definition list item description.</summary>
        /// <param name="definitionListItemDescription">The <see cref="DefinitionListItemDescriptionDocumentationElement"/> to visit.</param>
        protected internal abstract void VisitDefinitionListItemDescription(DefinitionListItemDescriptionDocumentationElement definitionListItemDescription);

        /// <summary>Visits a table.</summary>
        /// <param name="table">The <see cref="TableDocumentationElement"/> to visit.</param>
        protected internal abstract void VisitTable(TableDocumentationElement table);

        /// <summary>Visits a table column.</summary>
        /// <param name="tableColumn">The <see cref="TableColumnDocumentationElement"/> to visit.</param>
        protected internal abstract void VisitTableColumn(TableColumnDocumentationElement tableColumn);

        /// <summary>Visits a table row.</summary>
        /// <param name="tableRow">The <see cref="TableRowDocumentationElement"/> to visit.</param>
        protected internal abstract void VisitTableRow(TableRowDocumentationElement tableRow);

        /// <summary>Visits a table cell.</summary>
        /// <param name="tableCell">The <see cref="TableCellDocumentationElement"/> to visit.</param>
        protected internal abstract void VisitTableCell(TableCellDocumentationElement tableCell);

        /// <summary>Visits plain text.</summary>
        /// <param name="text">The <see cref="TextDocumentationElement"/> to visit.</param>
        protected internal abstract void VisitText(TextDocumentationElement text);

        /// <summary>Visits an unresolved inline member reference.</summary>
        /// <param name="memberNameReference">The <see cref="MemberNameReferenceDocumentationElement"/> to visit.</param>
        protected internal virtual void VisitInlineReference(MemberNameReferenceDocumentationElement memberNameReference)
            => throw new InvalidOperationException($"Could not find member from '{memberNameReference.CanonicalName}' canonical name. Override VisitInlineReference(MemberNameReferenceDocumentationElement) to ignore this error.");

        /// <summary>Visits an inline member reference.</summary>
        /// <param name="memberInfoReference">The <see cref="MemberInfoReferenceDocumentationElement"/> to visit.</param>
        protected internal abstract void VisitInlineReference(MemberInfoReferenceDocumentationElement memberInfoReference);

        /// <summary>Visits an inline code snippet.</summary>
        /// <param name="inlineCode">The <see cref="InlineCodeDocumentationElement"/> to visit.</param>
        protected internal abstract void VisitInlineCode(InlineCodeDocumentationElement inlineCode);

        /// <summary>Visits an inline parameter reference.</summary>
        /// <param name="parameterReference">The <see cref="ParameterReferenceDocumentationElement"/> to visit.</param>
        protected internal abstract void VisitParameterReference(ParameterReferenceDocumentationElement parameterReference);

        /// <summary>Visits an inline generic parameter reference.</summary>
        /// <param name="genericParameterReference">The <see cref="GenericParameterReferenceDocumentationElement"/> to visit.</param>
        protected internal abstract void VisitGenericParameterReference(GenericParameterReferenceDocumentationElement genericParameterReference);
    }
}