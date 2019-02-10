using System.Reflection;
using CodeMap.Elements;

namespace CodeMap.ElementsTests
{
    public class DocumentationVisitorAdapter : DocumentationVisitor
    {
        private readonly IDocumentationVisitor _documentationVisitor;

        public DocumentationVisitorAdapter(IDocumentationVisitor documentationVisitor)
        {
            _documentationVisitor = documentationVisitor;
        }

        protected override void VisitSummaryBeginning()
        {
            _documentationVisitor.VisitSummaryBeginning();
        }

        protected override void VisitSummaryEnding()
        {
            _documentationVisitor.VisitSummaryEnding();
        }

        protected override void VisitReturnsBeginning(TypeReferenceDocumentationElement returnType)
        {
            _documentationVisitor.VisitReturnsBeginning(returnType);
        }

        protected override void VisitReturnsEnding()
        {
            _documentationVisitor.VisitReturnsEnding();
        }

        protected override void VisitRemarksBeginning()
        {
            _documentationVisitor.VisitRemarksBeginning();
        }

        protected override void VisitRemarksEnding()
        {
            _documentationVisitor.VisitRemarksEnding();
        }

        protected override void VisitExampleBeginning()
        {
            _documentationVisitor.VisitExampleBeginning();
        }

        protected override void VisitExampleEnding()
        {
            _documentationVisitor.VisitExampleEnding();
        }

        protected override void VisitValueBeginning()
        {
            _documentationVisitor.VisitValueBeginning();
        }

        protected override void VisitValueEnding()
        {
            _documentationVisitor.VisitValueEnding();
        }

        protected override void VisitParagraphBeginning()
        {
            _documentationVisitor.VisitParagraphBeginning();
        }

        protected override void VisitCodeBlock(string code)
        {
            _documentationVisitor.VisitCodeBlock(code);
        }

        protected override void VisitRelatedMembersListBeginning()
        {
            _documentationVisitor.VisitRelatedMembersListBeginning();
        }

        protected override void VisitRelatedMembersListEnding()
        {
            _documentationVisitor.VisitRelatedMembersListEnding();
        }

        protected override void VisitRelatedMember(MemberReferenceDocumentationElement relatedMember)
        {
            _documentationVisitor.VisitRelatedMember(relatedMember);
        }

        protected override void VisitParagraphEnding()
        {
            _documentationVisitor.VisitParagraphEnding();
        }

        protected override void VisitUnorderedListBeginning()
        {
            _documentationVisitor.VisitUnorderedListBeginning();
        }

        protected override void VisitUnorderedListEnding()
        {
            _documentationVisitor.VisitUnorderedListEnding();
        }

        protected override void VisitOrderedListBeginning()
        {
            _documentationVisitor.VisitOrderedListBeginning();
        }

        protected override void VisitOrderedListEnding()
        {
            _documentationVisitor.VisitOrderedListEnding();
        }

        protected override void VisitListItemBeginning()
        {
            _documentationVisitor.VisitListItemBeginning();
        }

        protected override void VisitListItemEnding()
        {
            _documentationVisitor.VisitListItemEnding();
        }

        protected override void VisitDefinitionListBeginning()
        {
            _documentationVisitor.VisitDefinitionListBeginning();
        }

        protected override void VisitDefinitionListEnding()
        {
            _documentationVisitor.VisitDefinitionListEnding();
        }

        protected override void VisitDefinitionListTitleBeginning()
        {
            _documentationVisitor.VisitDefinitionListTitleBeginning();
        }

        protected override void VisitDefinitionListTitleEnding()
        {
            _documentationVisitor.VisitDefinitionListTitleEnding();
        }

        protected override void VisitDefinitionListItemBeginning()
        {
            _documentationVisitor.VisitDefinitionListItemBeginning();
        }

        protected override void VisitDefinitionListItemEnding()
        {
            _documentationVisitor.VisitDefinitionListItemEnding();
        }

        protected override void VisitDefinitionTermBeginning()
        {
            _documentationVisitor.VisitDefinitionTermBeginning();
        }

        protected override void VisitDefinitionTermEnding()
        {
            _documentationVisitor.VisitDefinitionTermEnding();
        }

        protected override void VisitDefinitionTermDescriptionBeginning()
        {
            _documentationVisitor.VisitDefinitionTermDescriptionBeginning();
        }

        protected override void VisitDefinitionTermDescriptionEnding()
        {
            _documentationVisitor.VisitDefinitionTermDescriptionEnding();
        }

        protected override void VisitTableBeginning()
        {
            _documentationVisitor.VisitTableBeginning();
        }

        protected override void VisitTableEnding()
        {
            _documentationVisitor.VisitTableEnding();
        }

        protected override void VisitTableHeadingBeginning()
        {
            _documentationVisitor.VisitTableHeadingBeginning();
        }

        protected override void VisitTableHeadingEnding()
        {
            _documentationVisitor.VisitTableHeadingEnding();
        }

        protected override void VisitTableColumnBeginning()
        {
            _documentationVisitor.VisitTableColumnBeginning();
        }

        protected override void VisitTableColumnEnding()
        {
            _documentationVisitor.VisitTableColumnEnding();
        }

        protected override void VisitTableBodyBeginning()
        {
            _documentationVisitor.VisitTableBodyBeginning();
        }

        protected override void VisitTableBodyEnding()
        {
            _documentationVisitor.VisitTableBodyEnding();
        }

        protected override void VisitTableRowBeginning()
        {
            _documentationVisitor.VisitTableRowBeginning();
        }

        protected override void VisitTableRowEnding()
        {
            _documentationVisitor.VisitTableRowEnding();
        }

        protected override void VisitTableCellBeginning()
        {
            _documentationVisitor.VisitTableCellBeginning();
        }

        protected override void VisitTableCellEnding()
        {
            _documentationVisitor.VisitTableCellEnding();
        }

        protected override void VisitText(string text)
        {
            _documentationVisitor.VisitText(text);
        }

        protected override void VisitInlineCode(string code)
        {
            _documentationVisitor.VisitInlineCode(code);
        }

        protected override void VisitInlineReference(MemberInfo referredMember)
        {
            _documentationVisitor.VisitInlineReference(referredMember);
        }

        protected override void VisitParameterReference(string parameterName)
        {
            _documentationVisitor.VisitParameterReference(parameterName);
        }

        protected override void VisitGenericParameterReference(string genericParameterName)
        {
            _documentationVisitor.VisitGenericParameterReference(genericParameterName);
        }
    }
}