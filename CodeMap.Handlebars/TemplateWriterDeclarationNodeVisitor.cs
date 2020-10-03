using System.IO;
using CodeMap.DeclarationNodes;

namespace CodeMap.Handlebars
{
    public abstract class TemplateWriterDeclarationNodeVisitor : DeclarationNodeVisitor
    {
        public TemplateWriterDeclarationNodeVisitor(TemplateWriter templateWriter)
            => TemplateWriter = templateWriter;

        protected TemplateWriter TemplateWriter { get; }

        protected abstract TextWriter GetTextWriter(DeclarationNode declarationNode);

        protected override void VisitAssembly(AssemblyDeclaration assembly)
        {
            _ApplyTempalte(DocumentationTemplateNames.Assembly, assembly);

            foreach (var @namespace in assembly.Namespaces)
                @namespace.Accept(this);
        }

        protected override void VisitNamespace(NamespaceDeclaration @namespace)
        {
            _ApplyTempalte(DocumentationTemplateNames.Namespace, @namespace);

            foreach (var type in @namespace.DeclaredTypes)
                type.Accept(this);
        }

        protected override void VisitEnum(EnumDeclaration @enum)
            => _ApplyTempalte(DocumentationTemplateNames.Enum, @enum);

        protected override void VisitDelegate(DelegateDeclaration @delegate)
            => _ApplyTempalte(DocumentationTemplateNames.Delegate, @delegate);

        protected override void VisitInterface(InterfaceDeclaration @interface)
        {
            _ApplyTempalte(DocumentationTemplateNames.Interface, @interface);

            foreach (var member in @interface.Members)
                member.Accept(this);
        }

        protected override void VisitClass(ClassDeclaration @class)
        {
            _ApplyTempalte(DocumentationTemplateNames.Class, @class);

            foreach (var member in @class.Members)
                member.Accept(this);
        }

        protected override void VisitStruct(StructDeclaration @struct)
        {
            _ApplyTempalte(DocumentationTemplateNames.Struct, @struct);

            foreach (var member in @struct.Members)
                member.Accept(this);
        }

        protected override void VisitConstant(ConstantDeclaration constant)
            => _ApplyTempalte(DocumentationTemplateNames.Constant, constant);

        protected override void VisitField(FieldDeclaration field)
            => _ApplyTempalte(DocumentationTemplateNames.Field, field);

        protected override void VisitConstructor(ConstructorDeclaration constructor)
            => _ApplyTempalte(DocumentationTemplateNames.Constructor, constructor);

        protected override void VisitEvent(EventDeclaration @event)
            => _ApplyTempalte(DocumentationTemplateNames.Event, @event);

        protected override void VisitProperty(PropertyDeclaration property)
            => _ApplyTempalte(DocumentationTemplateNames.Property, property);

        protected override void VisitMethod(MethodDeclaration method)
            => _ApplyTempalte(DocumentationTemplateNames.Method, method);

        private void _ApplyTempalte(string templateName, DeclarationNode declarationNode)
        {
            using var textWriter = GetTextWriter(declarationNode);
            TemplateWriter.Write(textWriter, templateName, declarationNode);
        }
    }
}