using System.Linq;
using System.Text;
using CodeMap.DeclarationNodes;
using CodeMap.ReferenceData;

namespace CodeMap.Handlebars.Visitors
{
    internal class MemberDeclarationFullNameVisitor : DeclarationNodeVisitor
    {
        private readonly StringBuilder _fullNameBuilder = new StringBuilder();
        private readonly bool _excludeParameters;

        public MemberDeclarationFullNameVisitor(bool excludeParameters)
            => _excludeParameters = excludeParameters;

        public string Result
            => _fullNameBuilder.ToString();

        protected override void VisitAssembly(AssemblyDeclaration assembly)
            => _fullNameBuilder.Append("index");

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
            if (!_excludeParameters && constructor.Parameters.Any())
            {
                _fullNameBuilder.Append('(');
                _fullNameBuilder.Append(string.Join(',', from parameter in constructor.Parameters select _GetMemberFullName(parameter.Type)));
                _fullNameBuilder.Append(')');
            }
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
            if (!_excludeParameters && property.Parameters.Any())
            {
                _fullNameBuilder.Append('[');
                _fullNameBuilder.Append(string.Join(',', from parameter in property.Parameters select _GetMemberFullName(parameter.Type)));
                _fullNameBuilder.Append(']');
            }
        }

        protected override void VisitMethod(MethodDeclaration method)
        {
            _VisitTypeDeclaration(method.DeclaringType);
            _fullNameBuilder.Append('.').Append(method.Name);
            if (method.GenericParameters.Any())
                _fullNameBuilder.Append('`').Append(method.GenericParameters.Count);

            if (!_excludeParameters && method.Parameters.Any())
            {
                _fullNameBuilder.Append('(');
                _fullNameBuilder.Append(string.Join(',', from parameter in method.Parameters select _GetMemberFullName(parameter.Type)));
                _fullNameBuilder.Append(')');
            }
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

        private static string _GetMemberFullName(MemberReference memberReference)
        {
            var visitor = new MemberReferenceFullNameVisitor(false);
            memberReference.Accept(visitor);
            return visitor.Result;
        }
    }
}