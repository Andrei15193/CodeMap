using System.IO;
using CodeMap.DeclarationNodes;

namespace CodeMap.Handlebars
{
    /// <summary>A base <see cref="DeclarationNodeVisitor"/> implementation based on a <see cref="Handlebars.TemplateWriter"/>.</summary>
    /// <remarks>This implementation visits every declaration and applies the respective template using the <see cref="DeclarationNode"/> as its context.</remarks>
    /// <seealso cref="DocumentationTemplateNames"/>
    public abstract class TemplateWriterDeclarationNodeVisitor : DeclarationNodeVisitor
    {
        /// <summary>Initializes a new instance of the <see cref="TemplateWriterDeclarationNodeVisitor"/> class.</summary>
        /// <param name="templateWriter">The <see cref="Handlebars.TemplateWriter"/> to use for applying <see cref="DeclarationNode"/> templates.</param>
        protected TemplateWriterDeclarationNodeVisitor(TemplateWriter templateWriter)
            => TemplateWriter = templateWriter;

        /// <summary>The <see cref="Handlebars.TemplateWriter"/> used to apply templates for each visited <see cref="DeclarationNode"/>.</summary>
        protected TemplateWriter TemplateWriter { get; }

        /// <summary>Gets a <see cref="TextWriter"/> where the applied template is written to.</summary>
        /// <param name="declarationNode">The <see cref="DeclarationNode"/> for which to get the <see cref="TextWriter"/>.</param>
        /// <returns>Returns a <see cref="TextWriter"/> for the provided <paramref name="declarationNode"/> where the applied template is written to.</returns>
        protected abstract TextWriter GetTextWriter(DeclarationNode declarationNode);

        /// <summary>Visits an <see cref="AssemblyDeclaration"/>.</summary>
        /// <param name="assembly">The <see cref="AssemblyDeclaration"/> to visit.</param>
        protected override void VisitAssembly(AssemblyDeclaration assembly)
        {
            _ApplyTempalte(DocumentationTemplateNames.Assembly, assembly);

            foreach (var @namespace in assembly.Namespaces)
                @namespace.Accept(this);
        }

        /// <summary>Visits a <see cref="NamespaceDeclaration"/>.</summary>
        /// <param name="namespace">The <see cref="NamespaceDeclaration"/> to visit.</param>
        protected override void VisitNamespace(NamespaceDeclaration @namespace)
        {
            _ApplyTempalte(DocumentationTemplateNames.Namespace, @namespace);

            foreach (var type in @namespace.DeclaredTypes)
                type.Accept(this);
        }

        /// <summary>Visits an <see cref="EnumDeclaration"/>.</summary>
        /// <param name="enum">The <see cref="EnumDeclaration"/> to visit.</param>
        protected override void VisitEnum(EnumDeclaration @enum)
            => _ApplyTempalte(DocumentationTemplateNames.Enum, @enum);

        /// <summary>Visits a <see cref="DelegateDeclaration"/>.</summary>
        /// <param name="delegate">The <see cref="DelegateDeclaration"/> to visit.</param>
        protected override void VisitDelegate(DelegateDeclaration @delegate)
            => _ApplyTempalte(DocumentationTemplateNames.Delegate, @delegate);

        /// <summary>Visits an <see cref="InterfaceDeclaration"/>.</summary>
        /// <param name="interface">The <see cref="InterfaceDeclaration"/> to visit.</param>
        protected override void VisitInterface(InterfaceDeclaration @interface)
        {
            _ApplyTempalte(DocumentationTemplateNames.Interface, @interface);

            foreach (var member in @interface.Members)
                member.Accept(this);
        }

        /// <summary>Visits a <see cref="ClassDeclaration"/>.</summary>
        /// <param name="class">The <see cref="ClassDeclaration"/> to visit.</param>
        protected override void VisitClass(ClassDeclaration @class)
        {
            _ApplyTempalte(DocumentationTemplateNames.Class, @class);

            foreach (var member in @class.Members)
                member.Accept(this);
        }

        /// <summary>Visits a <see cref="StructDeclaration"/>.</summary>
        /// <param name="struct">The <see cref="StructDeclaration"/> to visit.</param>
        protected override void VisitStruct(StructDeclaration @struct)
        {
            _ApplyTempalte(DocumentationTemplateNames.Struct, @struct);

            foreach (var member in @struct.Members)
                member.Accept(this);
        }

        /// <summary>Visits a <see cref="ConstantDeclaration"/>.</summary>
        /// <param name="constant">The <see cref="ConstantDeclaration"/> to visit.</param>
        protected override void VisitConstant(ConstantDeclaration constant)
            => _ApplyTempalte(DocumentationTemplateNames.Constant, constant);

        /// <summary>Visits a <see cref="FieldDeclaration"/>.</summary>
        /// <param name="field">The <see cref="FieldDeclaration"/> to visit.</param>
        protected override void VisitField(FieldDeclaration field)
            => _ApplyTempalte(DocumentationTemplateNames.Field, field);

        /// <summary>Visits a <see cref="ConstructorDeclaration"/>.</summary>
        /// <param name="constructor">The <see cref="ConstructorDeclaration"/> to visit.</param>
        protected override void VisitConstructor(ConstructorDeclaration constructor)
            => _ApplyTempalte(DocumentationTemplateNames.Constructor, constructor);

        /// <summary>Visits a <see cref="EventDeclaration"/>.</summary>
        /// <param name="event">The <see cref="EventDeclaration"/> to visit.</param>
        protected override void VisitEvent(EventDeclaration @event)
            => _ApplyTempalte(DocumentationTemplateNames.Event, @event);

        /// <summary>Visits a <see cref="PropertyDeclaration"/>.</summary>
        /// <param name="property">The <see cref="PropertyDeclaration"/> to visit.</param>
        protected override void VisitProperty(PropertyDeclaration property)
            => _ApplyTempalte(DocumentationTemplateNames.Property, property);

        /// <summary>Visits a <see cref="MethodDeclaration"/>.</summary>
        /// <param name="method">The <see cref="MethodDeclaration"/> to visit.</param>
        protected override void VisitMethod(MethodDeclaration method)
            => _ApplyTempalte(DocumentationTemplateNames.Method, method);

        private void _ApplyTempalte(string templateName, DeclarationNode declarationNode)
        {
            using var textWriter = GetTextWriter(declarationNode);
            TemplateWriter.Write(textWriter, templateName, declarationNode);
        }
    }
}