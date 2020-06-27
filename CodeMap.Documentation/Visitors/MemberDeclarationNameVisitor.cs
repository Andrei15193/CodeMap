using CodeMap.DeclarationNodes;
using System.Linq;
using System.Text;

namespace CodeMap.Documentation.Visitors
{
    internal class MemberDeclarationNameVisitor : DeclarationNodeVisitor
    {
        private readonly StringBuilder _nameBuilder = new StringBuilder();

        public string Result
            => _nameBuilder.ToString();

        protected override void VisitAssembly(AssemblyDeclaration assembly)
        {
        }

        protected override void VisitNamespace(NamespaceDeclaration @namespace)
            => _nameBuilder.Append(@namespace.Name);

        protected override void VisitEnum(EnumDeclaration @enum)
            => _nameBuilder.Append(@enum.Name);

        protected override void VisitDelegate(DelegateDeclaration @delegate)
            => _nameBuilder.Append(@delegate.Name);

        protected override void VisitInterface(InterfaceDeclaration @interface)
        {
            _nameBuilder.Append(@interface.Name);
            if (@interface.GenericParameters.Any())
                _nameBuilder
                    .Append('<')
                    .Append(string.Join(", ", @interface.GenericParameters.Select(genericParameter => genericParameter.Name)))
                    .Append('>');
        }

        protected override void VisitClass(ClassDeclaration @class)
        {
            _nameBuilder.Append(@class.Name);
            if (@class.GenericParameters.Any())
                _nameBuilder
                    .Append('<')
                    .Append(string.Join(", ", @class.GenericParameters.Select(genericParameter => genericParameter.Name)))
                    .Append('>');
        }

        protected override void VisitStruct(StructDeclaration @struct)
        {
            _nameBuilder.Append(@struct.Name);
            if (@struct.GenericParameters.Any())
                _nameBuilder
                    .Append('<')
                    .Append(string.Join(", ", @struct.GenericParameters.Select(genericParameter => genericParameter.Name)))
                    .Append('>');
        }

        protected override void VisitConstant(ConstantDeclaration constant)
            => _nameBuilder.Append(constant.Name);

        protected override void VisitField(FieldDeclaration field)
            => _nameBuilder.Append(field.Name);

        protected override void VisitConstructor(ConstructorDeclaration constructor)
            => _nameBuilder
                .Append(constructor.DeclaringType.Name)
                .Append('(')
                .Append(string.Join(", ", constructor.Parameters.Select(parameter => parameter.Type.GetMemberName())))
                .Append(')');

        protected override void VisitEvent(EventDeclaration @event)
            => _nameBuilder.Append(@event.Name);

        protected override void VisitProperty(PropertyDeclaration property)
        {
            _nameBuilder.Append(property.Name);
            if (property.Parameters.Any())
                _nameBuilder
                    .Append('[')
                    .Append(string.Join(", ", property.Parameters.Select(parameter => parameter.Type.GetMemberName())))
                    .Append(']');
        }

        protected override void VisitMethod(MethodDeclaration method)
            => _nameBuilder
                .Append(method.Name)
                .Append('(')
                .Append(string.Join(", ", method.Parameters.Select(parameter => parameter.Type.GetMemberName())))
                .Append(')');
    }
}