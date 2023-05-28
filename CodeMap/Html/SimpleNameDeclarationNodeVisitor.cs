using System.Data;
using System.Linq;
using System.Text;
using CodeMap.DeclarationNodes;

namespace CodeMap.Html
{
    internal class SimpleNameDeclarationNodeVisitor : DeclarationNodeVisitor
    {
        public StringBuilder StringBuilder { get; } = new StringBuilder();

        protected internal override void VisitAssembly(AssemblyDeclaration assembly)
        {
            StringBuilder.Append(assembly.Name);
        }

        protected internal override void VisitNamespace(NamespaceDeclaration @namespace)
        {
            if (!(@namespace is GlobalNamespaceDeclaration))
                StringBuilder.Append(@namespace.Name);
        }

        protected internal override void VisitInterface(InterfaceDeclaration @interface)
        {
            if (@interface.DeclaringType is object)
            {
                @interface.DeclaringType.Accept(this);
                StringBuilder.Append('.');
            }

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
            if (@class.DeclaringType is object)
            {
                @class.DeclaringType.Accept(this);
                StringBuilder.Append('.');
            }

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
            if (record.DeclaringType is object)
            {
                record.DeclaringType.Accept(this);
                StringBuilder.Append('.');
            }

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
            if (@struct.DeclaringType is object)
            {
                @struct.DeclaringType.Accept(this);
                StringBuilder.Append('.');
            }

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
            if (@delegate.DeclaringType is object)
            {
                @delegate.DeclaringType.Accept(this);
                StringBuilder.Append('.');
            }

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
        {
            if (@enum.DeclaringType is object)
            {
                @enum.DeclaringType.Accept(this);
                StringBuilder.Append('.');
            }

            StringBuilder.Append(@enum.Name);
        }

        protected internal override void VisitConstant(ConstantDeclaration constant)
        {
            constant.DeclaringType.Accept(this);
            StringBuilder.Append('.');

            StringBuilder.Append(constant.Name);
        }

        protected internal override void VisitField(FieldDeclaration field)
        {
            field.DeclaringType.Accept(this);
            StringBuilder.Append('.');

            StringBuilder.Append(field.Name);
        }

        protected internal override void VisitConstructor(ConstructorDeclaration constructor)
        {
            constructor.DeclaringType.Accept(this);
            StringBuilder.Append('.');

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
        {
            @event.DeclaringType.Accept(this);
            StringBuilder.Append('.');

            StringBuilder.Append(@event.Name);
        }

        protected internal override void VisitProperty(PropertyDeclaration property)
        {
            property.DeclaringType.Accept(this);
            StringBuilder.Append('.');

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
            method.DeclaringType.Accept(this);
            StringBuilder.Append('.');

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