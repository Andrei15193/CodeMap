using System.Reflection;
using CodeMap.Elements;

namespace CodeMap.Tests
{
    public interface IDocumentationVisitor
    {
        void VisitAssembly(AssemblyDocumentationElement assembly);

        void VisitNamespace(NamespaceDocumentationElement @namespace);

        void VisitEnum(EnumDocumentationElement @enum);

        void VisitDelegate(DelegateDocumentationElement @delegate);

        void VisitInterface(InterfaceDocumentationElement @interface);

        void VisitClass(ClassDocumentationElement @class);

        void VisitStruct(StructDocumentationElement @struct);

        void VisitConstant(ConstantDocumentationElement constant);

        void VisitField(FieldDocumentationElement field);

        void VisitConstructor(ConstructorDocumentationElement constructor);

        void VisitEvent(EventDocumentationElement @event);

        void VisitProperty(PropertyDocumentationElement property);

        void VisitMethod(MethodDocumentationElement method);

        void VisitSummaryBeginning();

        void VisitSummaryEnding();

        void VisitRemarksBeginning();

        void VisitRemarksEnding();

        void VisitExampleBeginning();

        void VisitExampleEnding();

        void VisitValueBeginning();

        void VisitValueEnding();

        void VisitParagraphBeginning();

        void VisitCodeBlock(string code);

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