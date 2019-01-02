using System.Reflection;
using CodeMap.Elements;

namespace CodeMap.ElementsTests
{
    public interface IDocumentationVisitor
    {
        void VisitSummaryBeginning();

        void VisitSummaryEnding();

        void VisitReturnsBeginning(TypeReferenceData returnType);

        void VisitReturnsEnding();

        void VisitRemarksBeginning();

        void VisitRemarksEnding();

        void VisitExampleBeginning();

        void VisitExampleEnding();

        void VisitValueBeginning();

        void VisitValueEnding();

        void VisitParagraphBeginning();

        void VisitCodeBlock(string code);

        void VisitRelatedMembersListBeginning();

        void VisitRelatedMembersListEnding();

        void VisitRelatedMember(MemberReferenceDocumentationElement referredMember);

        void VisitParagraphEnding();

        void VisitUnorderedListBeginning();

        void VisitUnorderedListEnding();

        void VisitOrderedListBeginning();

        void VisitOrderedListEnding();

        void VisitListItemBeginning();

        void VisitListItemEnding();

        void VisitDefinitionListBeginning();

        void VisitDefinitionListEnding();

        void VisitDefinitionListTitleBeginning();

        void VisitDefinitionListTitleEnding();

        void VisitDefinitionListItemBeginning();

        void VisitDefinitionListItemEnding();

        void VisitDefinitionTermBeginning();

        void VisitDefinitionTermEnding();

        void VisitDefinitionTermDescriptionBeginning();

        void VisitDefinitionTermDescriptionEnding();

        void VisitTableBeginning();

        void VisitTableEnding();

        void VisitTableHeadingBeginning();

        void VisitTableHeadingEnding();

        void VisitTableColumnBeginning();

        void VisitTableColumnEnding();

        void VisitTableBodyBeginning();

        void VisitTableBodyEnding();

        void VisitTableRowBeginning();

        void VisitTableRowEnding();

        void VisitTableCellBeginning();

        void VisitTableCellEnding();

        void VisitText(string text);

        void VisitInlineCode(string code);

        void VisitInlineReference(MemberInfo referredMember);

        void VisitParameterReference(string parameterName);

        void VisitGenericParameterReference(string genericParameterName);
    }
}