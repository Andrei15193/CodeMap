using System.Linq;
using System.Text;
using CodeMap.DeclarationNodes;

namespace CodeMap.Documentation.Visitors
{
    internal class MemberDeclarationFullNameVisitor : DeclarationNodeVisitor
    {
        private readonly StringBuilder _fullNameBuilder = new StringBuilder();

        public string Result
            => _fullNameBuilder.ToString();

        protected override void VisitAssembly(AssemblyDeclaration assembly)
        {
        }

        protected override void VisitNamespace(NamespaceDeclaration @namespace)
            => _fullNameBuilder.Append(@namespace.Name);

        protected override void VisitEnum(EnumDeclaration @enum)
            => _VisitTypeDeclaration(@enum);

        protected override void VisitDelegate(DelegateDeclaration @delegate)
            => _VisitTypeDeclaration(@delegate);

        protected override void VisitInterface(InterfaceDeclaration @interface)
            => _VisitTypeDeclaration(@interface);

        protected override void VisitClass(ClassDeclaration @class)
            => _VisitTypeDeclaration(@class);

        protected override void VisitStruct(StructDeclaration @struct)
            => _VisitTypeDeclaration(@struct);

        protected override void VisitConstant(ConstantDeclaration constant)
        {
            _VisitTypeDeclaration(constant.DeclaringType);
            _fullNameBuilder.Append('.').Append(constant.Name);
        }

        protected override void VisitField(FieldDeclaration field)
        {
            _VisitTypeDeclaration(field.DeclaringType);
            _fullNameBuilder.Append('.').Append(field.Name);
        }

        protected override void VisitConstructor(ConstructorDeclaration constructor)
        {
            _VisitTypeDeclaration(constructor.DeclaringType);
            _fullNameBuilder.Append('.').Append(constructor.DeclaringType.Name);
            if (constructor.Parameters.Any())
                _fullNameBuilder.Append('-').Append(constructor.Parameters.Count);
        }

        protected override void VisitEvent(EventDeclaration @event)
        {
            _VisitTypeDeclaration(@event.DeclaringType);
            _fullNameBuilder.Append('.').Append(@event.Name);
        }

        protected override void VisitProperty(PropertyDeclaration property)
        {
            _VisitTypeDeclaration(property.DeclaringType);
            _fullNameBuilder.Append('.').Append(property.Name);
            if (property.Parameters.Any())
                _fullNameBuilder.Append('-').Append(property.Parameters.Count);
        }

        protected override void VisitMethod(MethodDeclaration method)
        {
            _VisitTypeDeclaration(method.DeclaringType);
            _fullNameBuilder.Append('.').Append(method.Name);
            if (method.GenericParameters.Any())
                _fullNameBuilder.Append('`').Append(method.GenericParameters.Count);

            if (method.Parameters.Any())
                _fullNameBuilder.Append('-').Append(method.Parameters.Count);
        }

        private void _VisitTypeDeclaration(TypeDeclaration typeDeclaration)
        {
            if (!(typeDeclaration.DeclaringType is null))
            {
                typeDeclaration.DeclaringType.Accept(this);
                _fullNameBuilder.Append('.');
            }
            else if (!(typeDeclaration.Namespace is GlobalNamespaceDeclaration))
            {
                typeDeclaration.Namespace.Accept(this);
                _fullNameBuilder.Append('.');
            }

            _fullNameBuilder.Append(typeDeclaration.Name);
        }
    }
}