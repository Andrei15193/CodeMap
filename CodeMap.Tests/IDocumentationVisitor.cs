using System.Reflection;
using CodeMap.DeclarationNodes;

namespace CodeMap.Tests
{
    public interface IDocumentationVisitor
    {
        void VisitAssembly(AssemblyDeclaration assembly);

        void VisitNamespace(NamespaceDeclaration @namespace);

        void VisitEnum(EnumDeclaration @enum);

        void VisitDelegate(DelegateDeclaration @delegate);

        void VisitInterface(InterfaceDeclaration @interface);

        void VisitClass(ClassDeclaration @class);

        void VisitStruct(StructDeclaration @struct);

        void VisitConstant(ConstantDeclaration constant);

        void VisitField(FieldDeclaration field);

        void VisitConstructor(ConstructorDeclaration constructor);

        void VisitEvent(EventDeclaration @event);

        void VisitProperty(PropertyDeclaration property);

        void VisitMethod(MethodDeclaration method);

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