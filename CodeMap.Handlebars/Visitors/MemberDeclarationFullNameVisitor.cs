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
        {
            _VisitTypeDeclaration(@delegate);
            if (@delegate.GenericParameters.Any())
                _fullNameBuilder.Append('`').Append(@delegate.GenericParameters.Count);
        }

        protected override void VisitInterface(InterfaceDeclaration @interface)
        {
            _VisitTypeDeclaration(@interface);
            if (@interface.GenericParameters.Any())
                _fullNameBuilder.Append('`').Append(@interface.GenericParameters.Count);
        }

        protected override void VisitClass(ClassDeclaration @class)
        {
            _VisitTypeDeclaration(@class);
            if (@class.GenericParameters.Any())
                _fullNameBuilder.Append('`').Append(@class.GenericParameters.Count);
        }

        protected override void VisitStruct(StructDeclaration @struct)
        {
            _VisitTypeDeclaration(@struct);
            if (@struct.GenericParameters.Any())
                _fullNameBuilder.Append('`').Append(@struct.GenericParameters.Count);
        }

        protected override void VisitConstant(ConstantDeclaration constant)
        {
            constant.DeclaringType.Accept(this);
            _fullNameBuilder.Append('.').Append(constant.Name);
        }

        protected override void VisitField(FieldDeclaration field)
        {
            field.DeclaringType.Accept(this);
            _fullNameBuilder.Append('.').Append(field.Name);
        }

        protected override void VisitConstructor(ConstructorDeclaration constructor)
        {
            constructor.DeclaringType.Accept(this);
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
            @event.DeclaringType.Accept(this);
            _fullNameBuilder.Append('.').Append(@event.Name);
        }

        protected override void VisitProperty(PropertyDeclaration property)
        {
            property.DeclaringType.Accept(this);
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
            method.DeclaringType.Accept(this);
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
            if (!(typeDeclaration.Namespace is GlobalNamespaceDeclaration))
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