using CodeMap.Elements;
using System.Collections.Generic;
using System.Reflection;

namespace CodeMap.Tests
{
    public class DocumentationVisitorAdapter : DocumentationVisitor
    {
        private readonly IDocumentationVisitor _documentationVisitor;

        public DocumentationVisitorAdapter(IDocumentationVisitor documentationVisitor)
        {
            _documentationVisitor = documentationVisitor;
        }

        protected override void VisitAssembly(AssemblyDocumentationElement assembly)
        {
            _documentationVisitor.VisitAssembly(assembly);
        }

        protected override void VisitNamespace(NamespaceDocumentationElement @namespace)
        {
            _documentationVisitor.VisitNamespace(@namespace);
        }

        protected override void VisitEnum(EnumDocumentationElement @enum)
        {
            _documentationVisitor.VisitEnum(@enum);
        }

        protected override void VisitDelegate(DelegateDocumentationElement @delegate)
        {
            _documentationVisitor.VisitDelegate(@delegate);
        }

        protected override void VisitInterface(InterfaceDocumentationElement @interface)
        {
            _documentationVisitor.VisitInterface(@interface);
        }

        protected override void VisitClass(ClassDocumentationElement @class)
        {
            _documentationVisitor.VisitClass(@class);
        }

        protected override void VisitStruct(StructDocumentationElement @struct)
        {
            _documentationVisitor.VisitStruct(@struct);
        }

        protected override void VisitConstant(ConstantDocumentationElement constant)
        {
            _documentationVisitor.VisitConstant(constant);
        }

        protected override void VisitField(FieldDocumentationElement field)
        {
            _documentationVisitor.VisitField(field);
        }

        protected override void VisitConstructor(ConstructorDocumentationElement constructor)
        {
            _documentationVisitor.VisitConstructor(constructor);
        }

        protected override void VisitEvent(EventDocumentationElement @event)
        {
            _documentationVisitor.VisitEvent(@event);
        }

        protected override void VisitProperty(PropertyDocumentationElement property)
        {
            _documentationVisitor.VisitProperty(property);
        }

        protected override void VisitMethod(MethodDocumentationElement method)
        {
            _documentationVisitor.VisitMethod(method);
        }

        protected override void VisitSummaryBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _documentationVisitor.VisitSummaryBeginning();
        }

        protected override void VisitSummaryEnding()
        {
            _documentationVisitor.VisitSummaryEnding();
        }

        protected override void VisitRemarksBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _documentationVisitor.VisitRemarksBeginning();
        }

        protected override void VisitRemarksEnding()
        {
            _documentationVisitor.VisitRemarksEnding();
        }

        protected override void VisitExampleBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _documentationVisitor.VisitExampleBeginning();
        }

        protected override void VisitExampleEnding()
        {
            _documentationVisitor.VisitExampleEnding();
        }

        protected override void VisitValueBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _documentationVisitor.VisitValueBeginning();
        }

        protected override void VisitValueEnding()
        {
            _documentationVisitor.VisitValueEnding();
        }

        protected override void VisitParagraphBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _documentationVisitor.VisitParagraphBeginning();
        }

        protected override void VisitCodeBlock(string code, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _documentationVisitor.VisitCodeBlock(code);
        }

        protected override void VisitParagraphEnding()
        {
            _documentationVisitor.VisitParagraphEnding();
        }

        protected override void VisitUnorderedListBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _documentationVisitor.VisitUnorderedListBeginning();
        }

        protected override void VisitUnorderedListEnding()
        {
            _documentationVisitor.VisitUnorderedListEnding();
        }

        protected override void VisitOrderedListBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _documentationVisitor.VisitOrderedListBeginning();
        }

        protected override void VisitOrderedListEnding()
        {
            _documentationVisitor.VisitOrderedListEnding();
        }

        protected override void VisitListItemBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _documentationVisitor.VisitListItemBeginning();
        }

        protected override void VisitListItemEnding()
        {
            _documentationVisitor.VisitListItemEnding();
        }

        protected override void VisitDefinitionListBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _documentationVisitor.VisitDefinitionListBeginning();
        }

        protected override void VisitDefinitionListEnding()
        {
            _documentationVisitor.VisitDefinitionListEnding();
        }

        protected override void VisitDefinitionListTitleBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _documentationVisitor.VisitDefinitionListTitleBeginning();
        }

        protected override void VisitDefinitionListTitleEnding()
        {
            _documentationVisitor.VisitDefinitionListTitleEnding();
        }

        protected override void VisitDefinitionListItemBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _documentationVisitor.VisitDefinitionListItemBeginning();
        }

        protected override void VisitDefinitionListItemEnding()
        {
            _documentationVisitor.VisitDefinitionListItemEnding();
        }

        protected override void VisitDefinitionTermBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _documentationVisitor.VisitDefinitionTermBeginning();
        }

        protected override void VisitDefinitionTermEnding()
        {
            _documentationVisitor.VisitDefinitionTermEnding();
        }

        protected override void VisitDefinitionTermDescriptionBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _documentationVisitor.VisitDefinitionTermDescriptionBeginning();
        }

        protected override void VisitDefinitionTermDescriptionEnding()
        {
            _documentationVisitor.VisitDefinitionTermDescriptionEnding();
        }

        protected override void VisitTableBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
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

        protected override void VisitTableColumnBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
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

        protected override void VisitTableRowBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _documentationVisitor.VisitTableRowBeginning();
        }

        protected override void VisitTableRowEnding()
        {
            _documentationVisitor.VisitTableRowEnding();
        }

        protected override void VisitTableCellBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
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

        protected override void VisitInlineCode(string code, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _documentationVisitor.VisitInlineCode(code);
        }

        protected override void VisitInlineReference(MemberInfo referredMember, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _documentationVisitor.VisitInlineReference(referredMember);
        }

        protected override void VisitParameterReference(string parameterName, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _documentationVisitor.VisitParameterReference(parameterName);
        }

        protected override void VisitGenericParameterReference(string genericParameterName, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _documentationVisitor.VisitGenericParameterReference(genericParameterName);
        }
    }
}