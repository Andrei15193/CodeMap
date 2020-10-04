using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeMap.DeclarationNodes;
using CodeMap.ReferenceData;

namespace CodeMap.Handlebars.Visitors
{
    internal class MemberDeclarationNameVisitor : DeclarationNodeVisitor
    {
        private readonly StringBuilder _nameBuilder = new StringBuilder();

        public string Result
            => _nameBuilder.ToString();

        protected override void VisitAssembly(AssemblyDeclaration assembly)
            => _nameBuilder.Append(assembly.Name);

        protected override void VisitNamespace(NamespaceDeclaration @namespace)
            => _nameBuilder.Append(@namespace.Name);

        protected override void VisitEnum(EnumDeclaration @enum)
            => _nameBuilder.Append(@enum.Name);

        protected override void VisitDelegate(DelegateDeclaration @delegate)
        {
            _nameBuilder.Append(@delegate.Name);
            _AppendGenericParameters(@delegate.GenericParameters);
        }

        protected override void VisitInterface(InterfaceDeclaration @interface)
        {
            _nameBuilder.Append(@interface.Name);
            _AppendGenericParameters(@interface.GenericParameters);
        }

        protected override void VisitClass(ClassDeclaration @class)
        {
            _nameBuilder.Append(@class.Name);
            _AppendGenericParameters(@class.GenericParameters);
        }

        protected override void VisitStruct(StructDeclaration @struct)
        {
            _nameBuilder.Append(@struct.Name);
            _AppendGenericParameters(@struct.GenericParameters);
        }

        protected override void VisitConstant(ConstantDeclaration constant)
            => _nameBuilder.Append(constant.Name);

        protected override void VisitField(FieldDeclaration field)
            => _nameBuilder.Append(field.Name);

        protected override void VisitConstructor(ConstructorDeclaration constructor)
        {
            _nameBuilder.Append(constructor.DeclaringType.Name);
            _AppendReferencedMembers('(', constructor.Parameters.Select(parameter => parameter.Type), ')');
        }

        protected override void VisitEvent(EventDeclaration @event)
            => _nameBuilder.Append(@event.Name);

        protected override void VisitProperty(PropertyDeclaration property)
        {
            _nameBuilder.Append(property.Name);
            if (property.Parameters.Any())
                _AppendReferencedMembers('[', property.Parameters.Select(parameter => parameter.Type), ']');
        }

        protected override void VisitMethod(MethodDeclaration method)
        {
            _nameBuilder.Append(method.Name);
            _AppendReferencedMembers('(', method.Parameters.Select(parameter => parameter.Type), ')');
        }

        private void _AppendGenericParameters(IEnumerable<GenericTypeParameterData> genericTypeParameters)
        {
            if (genericTypeParameters.Any())
            {
                _nameBuilder.Append('<');
                var isFirst = true;
                foreach (var genericParameter in genericTypeParameters)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        _nameBuilder.Append(", ");
                    _nameBuilder.Append(genericParameter.Name);
                }
                _nameBuilder.Append('>');
            }
        }

        private void _AppendReferencedMembers(char openingCharacter, IEnumerable<MemberReference> memberReferences, char closingCharacter)
        {
            var visitor = new MemberReferenceNameBuilderVisitor(_nameBuilder);

            _nameBuilder.Append(openingCharacter);
            var isFirst = true;
            foreach (var memberReference in memberReferences)
            {
                if (isFirst)
                    isFirst = false;
                else
                    _nameBuilder.Append(", ");
                memberReference.Accept(visitor);
            }
            _nameBuilder.Append(closingCharacter);
        }
    }
}