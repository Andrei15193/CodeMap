using System.Text;
using CodeMap.DeclarationNodes;

namespace CodeMap.Html
{
    internal class HtmlPageTitleDeclarationNodeVisitor : DeclarationNodeVisitor
    {
        public HtmlPageTitleDeclarationNodeVisitor()
            : this(null)
        {
        }

        public HtmlPageTitleDeclarationNodeVisitor(StringBuilder stringBuilder)
            => TitleStringBuilder = stringBuilder ?? new StringBuilder();

        public StringBuilder TitleStringBuilder { get; }

        protected internal override void VisitAssembly(AssemblyDeclaration assembly)
            => TitleStringBuilder
                .Append(assembly.Name)
                .Append('@')
                .Append(assembly.Version)
                .Append(" - Home");
                
        protected internal override void VisitNamespace(NamespaceDeclaration @namespace)
            => TitleStringBuilder
                .Append(@namespace.Assembly.Name)
                .Append('@')
                .Append(@namespace.Assembly.Version)
                .Append(" - ")
                .Append(@namespace.Name)
                .Append(" Namespace");

        protected internal override void VisitEnum(EnumDeclaration @enum)
            => TitleStringBuilder
                .Append(@enum.Assembly.Name)
                .Append('@')
                .Append(@enum.Assembly.Version)
                .Append(" - ")
                .Append(@enum.GetSimpleNameReference())
                .Append(" Enum");

        protected internal override void VisitDelegate(DelegateDeclaration @delegate)
            => TitleStringBuilder
                .Append(@delegate.Assembly.Name)
                .Append('@')
                .Append(@delegate.Assembly.Version)
                .Append(" - ")
                .Append(@delegate.GetSimpleNameReference())
                .Append(" Delegate");

        protected internal override void VisitInterface(InterfaceDeclaration @interface)
            => TitleStringBuilder
                .Append(@interface.Assembly.Name)
                .Append('@')
                .Append(@interface.Assembly.Version)
                .Append(" - ")
                .Append(@interface.GetSimpleNameReference())
                .Append(" Interface");

        protected internal override void VisitClass(ClassDeclaration @class)
            => TitleStringBuilder
                .Append(@class.Assembly.Name)
                .Append('@')
                .Append(@class.Assembly.Version)
                .Append(" - ")
                .Append(@class.GetSimpleNameReference())
                .Append(" Class");

        protected internal override void VisitRecord(RecordDeclaration record)
            => TitleStringBuilder
                .Append(record.Assembly.Name)
                .Append('@')
                .Append(record.Assembly.Version)
                .Append(" - ")
                .Append(record.GetSimpleNameReference())
                .Append(" Record");

        protected internal override void VisitStruct(StructDeclaration @struct)
            => TitleStringBuilder
                .Append(@struct.Assembly.Name)
                .Append('@')
                .Append(@struct.Assembly.Version)
                .Append(" - ")
                .Append(@struct.GetSimpleNameReference())
                .Append(" Struct");

        protected internal override void VisitConstant(ConstantDeclaration constant)
            => TitleStringBuilder
                .Append(constant.DeclaringType.Assembly.Name)
                .Append('@')
                .Append(constant.DeclaringType.Assembly.Version)
                .Append(" - ")
                .Append(constant.GetSimpleNameReference())
                .Append(" Constant");

        protected internal override void VisitField(FieldDeclaration field)
            => TitleStringBuilder
                .Append(field.DeclaringType.Assembly.Name)
                .Append('@')
                .Append(field.DeclaringType.Assembly.Version)
                .Append(" - ")
                .Append(field.GetSimpleNameReference())
                .Append(" Field");

        protected internal override void VisitConstructor(ConstructorDeclaration constructor)
            => TitleStringBuilder
                .Append(constructor.DeclaringType.Assembly.Name)
                .Append('@')
                .Append(constructor.DeclaringType.Assembly.Version)
                .Append(" - ")
                .Append(constructor.GetSimpleNameReference())
                .Append(" Constructor");

        protected internal override void VisitEvent(EventDeclaration @event)
            => TitleStringBuilder
                .Append(@event.DeclaringType.Assembly.Name)
                .Append('@')
                .Append(@event.DeclaringType.Assembly.Version)
                .Append(" - ")
                .Append(@event.GetSimpleNameReference())
                .Append(" Event");

        protected internal override void VisitProperty(PropertyDeclaration property)
            => TitleStringBuilder
                .Append(property.DeclaringType.Assembly.Name)
                .Append('@')
                .Append(property.DeclaringType.Assembly.Version)
                .Append(" - ")
                .Append(property.GetSimpleNameReference())
                .Append(" Property");

        protected internal override void VisitMethod(MethodDeclaration method)
            => TitleStringBuilder
                .Append(method.DeclaringType.Assembly.Name)
                .Append('@')
                .Append(method.DeclaringType.Assembly.Version)
                .Append(" - ")
                .Append(method.GetSimpleNameReference())
                .Append(" Property");
    }
}