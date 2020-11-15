using System.IO;
using CodeMap.DeclarationNodes;
using EmbeddedResourceBrowser;

namespace CodeMap.Handlebars
{
    /// <summary>Represents a <a href="https://github.com/Handlebars-Net/Handlebars.Net">Handlebars.NET</a> based writer for generating documentation.</summary>
    public class HandlebarsWriterDeclarationNodeVisitor : DeclarationNodeVisitor
    {
        private readonly IMemberReferenceResolver _memberReferenceResolver;
        private readonly DirectoryInfo _directoryInfo;
        private readonly HandlebarsTemplateWriter _handlebarsTemplateWriter;

        /// <summary>Initialzies a new instance of the <see cref="HandlebarsWriterDeclarationNodeVisitor"/> class.</summary>
        /// <param name="directoryInfo">The directory to write the documentation files to.</param>
        /// <param name="memberReferenceResolver">The <see cref="IMemberReferenceResolver"/> used to generate documentation file names.</param>
        /// <param name="handlebarsTemplateWriter">The <see cref="HandlebarsTemplateWriter"/> used for generating documentation files.</param>
        public HandlebarsWriterDeclarationNodeVisitor(DirectoryInfo directoryInfo, IMemberReferenceResolver memberReferenceResolver, HandlebarsTemplateWriter handlebarsTemplateWriter)
        {
            _memberReferenceResolver = memberReferenceResolver;
            _directoryInfo = directoryInfo;
            _handlebarsTemplateWriter = handlebarsTemplateWriter;
        }

        /// <summary>Visits an <see cref="AssemblyDeclaration"/>.</summary>
        /// <param name="assembly">The <see cref="AssemblyDeclaration"/> to visit.</param>
        protected override void VisitAssembly(AssemblyDeclaration assembly)
        {
            _handlebarsTemplateWriter.Assets.CopyTo(_directoryInfo);

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
            using var fileStream = new FileStream(Path.Combine(_directoryInfo.FullName, _memberReferenceResolver.GetFileName(declarationNode)), FileMode.Create, FileAccess.Write, FileShare.Read);
            using var fileStreamWriter = new StreamWriter(fileStream);
            _handlebarsTemplateWriter.Write(fileStreamWriter, templateName, declarationNode);
        }
    }
}