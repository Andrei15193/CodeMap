using System.Linq;
using System.Text;
using CodeMap.DeclarationNodes;

namespace CodeMap.Html
{
    internal class SimpleNameDeclarationNodeVisitor : DeclarationNodeVisitor
    {
        public StringBuilder StringBuilder { get; } = new StringBuilder();

        protected internal override void VisitAssembly(AssemblyDeclaration assembly)
            => StringBuilder.Append(assembly.Name);

        protected internal override void VisitNamespace(NamespaceDeclaration @namespace)
        {
            if (!(@namespace is GlobalNamespaceDeclaration))
                StringBuilder.Append(@namespace.Name);
        }

        protected internal override void VisitInterface(InterfaceDeclaration @interface)
        {
            StringBuilder.Append(@interface.Name);

            if (@interface.GenericParameters.Any())
            {
                StringBuilder.Append('<');
                var isFirst = true;
                foreach (var genericParameter in @interface.GenericParameters)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        StringBuilder.Append(", ");
                    StringBuilder.Append(genericParameter.Name);
                }
                StringBuilder.Append('>');
            }
        }

        protected internal override void VisitClass(ClassDeclaration @class)
        {
            StringBuilder.Append(@class.Name);
            if (@class.GenericParameters.Any())
            {
                StringBuilder.Append('<');
                var isFirst = true;
                foreach (var genericParameter in @class.GenericParameters)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        StringBuilder.Append(", ");
                    StringBuilder.Append(genericParameter.Name);
                }
                StringBuilder.Append('>');
            }
        }

        protected internal override void VisitRecord(RecordDeclaration record)
        {
            StringBuilder.Append(record.Name);
            if (record.GenericParameters.Any())
            {
                StringBuilder.Append('<');
                var isFirst = true;
                foreach (var genericParameter in record.GenericParameters)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        StringBuilder.Append(", ");
                    StringBuilder.Append(genericParameter.Name);
                }
                StringBuilder.Append('>');
            }
        }

        protected internal override void VisitStruct(StructDeclaration @struct)
        {
            StringBuilder.Append(@struct.Name);
            if (@struct.GenericParameters.Any())
            {
                StringBuilder.Append('<');
                var isFirst = true;
                foreach (var genericParameter in @struct.GenericParameters)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        StringBuilder.Append(", ");
                    StringBuilder.Append(genericParameter.Name);
                }
                StringBuilder.Append('>');
            }
        }

        protected internal override void VisitDelegate(DelegateDeclaration @delegate)
        {
            StringBuilder.Append(@delegate.Name);
            if (@delegate.GenericParameters.Any())
            {
                StringBuilder.Append('<');
                var isFirst = true;
                foreach (var genericParameter in @delegate.GenericParameters)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        StringBuilder.Append(", ");
                    StringBuilder.Append(genericParameter.Name);
                }
                StringBuilder.Append('>');
            }

            {
                StringBuilder.Append('(');
                var isFirst = true;
                foreach (var parameter in @delegate.Parameters)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        StringBuilder.Append(", ");
                    StringBuilder.Append(parameter.Type.GetSimpleNameReference());
                }
                StringBuilder.Append(')');
            }
        }

        protected internal override void VisitEnum(EnumDeclaration @enum)
            => StringBuilder.Append(@enum.Name);

        protected internal override void VisitConstant(ConstantDeclaration constant)
            => StringBuilder.Append(constant.Name);

        protected internal override void VisitField(FieldDeclaration field)
            => StringBuilder.Append(field.Name);

        protected internal override void VisitConstructor(ConstructorDeclaration constructor)
        {
            StringBuilder.Append(constructor.Name);
            StringBuilder.Append('(');
            var isFirst = true;
            foreach (var parameter in constructor.Parameters)
            {
                if (isFirst)
                    isFirst = false;
                else
                    StringBuilder.Append(", ");
                StringBuilder.Append(parameter.Type.GetSimpleNameReference());
            }
            StringBuilder.Append(')');
        }

        protected internal override void VisitEvent(EventDeclaration @event)
            => StringBuilder.Append(@event.Name);

        protected internal override void VisitProperty(PropertyDeclaration property)
        {
            StringBuilder.Append(property.Name);
            if (property.Parameters.Any())
            {
                StringBuilder.Append('[');
                var isFirst = true;
                foreach (var parameter in property.Parameters)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        StringBuilder.Append(", ");
                    StringBuilder.Append(parameter.Type.GetSimpleNameReference());
                }
                StringBuilder.Append(']');
            }
        }

        protected internal override void VisitMethod(MethodDeclaration method)
        {
            StringBuilder.Append(method.Name);
            if (method.GenericParameters.Any())
            {
                StringBuilder.Append('<');
                var isFirst = true;
                foreach (var genericParameter in method.GenericParameters)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        StringBuilder.Append(", ");
                    StringBuilder.Append(genericParameter.Name);
                }
                StringBuilder.Append('>');
            }

            {
                StringBuilder.Append('(');
                var isFirst = true;
                foreach (var parameter in method.Parameters)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        StringBuilder.Append(", ");
                    StringBuilder.Append(parameter.Type.GetSimpleNameReference());
                }
                StringBuilder.Append(')');
            }
        }
    }
}