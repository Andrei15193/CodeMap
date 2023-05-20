using System.Linq;
using System.Text;
using CodeMap.DeclarationNodes;

namespace CodeMap.Html
{
    internal class FullNameDeclarationNodeVisitor : DeclarationNodeVisitor
    {
        public FullNameDeclarationNodeVisitor()
            : this(null)
        {
        }

        public FullNameDeclarationNodeVisitor(StringBuilder stringBuilder)
            => StringBuilder = stringBuilder ?? new StringBuilder();

        public StringBuilder StringBuilder { get; set; }

        protected internal override void VisitAssembly(AssemblyDeclaration assembly)
        {
        }

        protected internal override void VisitNamespace(NamespaceDeclaration @namespace)
        {
            @namespace.Assembly.Accept(this);
            if (!(@namespace is GlobalNamespaceDeclaration))
            {
                if (StringBuilder.Length > 0)
                    StringBuilder.Append('.');
                StringBuilder.Append(@namespace.Name);
            }
        }

        protected internal override void VisitEnum(EnumDeclaration @enum)
        {
            if (@enum.DeclaringType != (DeclarationNode)null)
                @enum.DeclaringType.Accept(this);
            else
                @enum.Namespace.Accept(this);

            if (StringBuilder.Length > 0)
                StringBuilder.Append('.');
            StringBuilder.Append(@enum.Name);
        }

        protected internal override void VisitDelegate(DelegateDeclaration @delegate)
        {
            if (@delegate.DeclaringType != (DeclarationNode)null)
                @delegate.DeclaringType.Accept(this);
            else
                @delegate.Namespace.Accept(this);

            if (StringBuilder.Length > 0)
                StringBuilder.Append('.');
            StringBuilder.Append(@delegate.Name);
            if (@delegate.GenericParameters.Any())
            {
                StringBuilder.Append('[');
                var isFirst = true;
                foreach (var genericParameter in @delegate.GenericParameters)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        StringBuilder.Append(',');
                    StringBuilder.Append(genericParameter.Name);
                }
                StringBuilder.Append(']');
            }

            {
                StringBuilder.Append('(');
                var isFirst = true;
                foreach (var parameter in @delegate.Parameters)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        StringBuilder.Append(',');
                    StringBuilder.Append(parameter.Type.GetFullNameReference());
                }
                StringBuilder.Append(')');
            }
        }

        protected internal override void VisitInterface(InterfaceDeclaration @interface)
        {
            if (@interface.DeclaringType != (DeclarationNode)null)
                @interface.DeclaringType.Accept(this);
            else
                @interface.Namespace.Accept(this);

            if (StringBuilder.Length > 0)
                StringBuilder.Append('.');
            StringBuilder.Append(@interface.Name);
            if (@interface.GenericParameters.Any())
            {
                StringBuilder.Append('[');
                var isFirst = true;
                foreach (var genericParameter in @interface.GenericParameters)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        StringBuilder.Append(',');
                    StringBuilder.Append(genericParameter.Name);
                }
                StringBuilder.Append(']');
            }
        }

        protected internal override void VisitClass(ClassDeclaration @class)
        {
            if (@class.DeclaringType != (DeclarationNode)null)
                @class.DeclaringType.Accept(this);
            else
                @class.Namespace.Accept(this);

            if (StringBuilder.Length > 0)
                StringBuilder.Append('.');
            StringBuilder.Append(@class.Name);
            if (@class.GenericParameters.Any())
            {
                StringBuilder.Append('[');
                var isFirst = true;
                foreach (var genericParameter in @class.GenericParameters)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        StringBuilder.Append(',');
                    StringBuilder.Append(genericParameter.Name);
                }
                StringBuilder.Append(']');
            }
        }

        protected internal override void VisitRecord(RecordDeclaration record)
        {
            if (record.DeclaringType != (DeclarationNode)null)
                record.DeclaringType.Accept(this);
            else
                record.Namespace.Accept(this);

            if (StringBuilder.Length > 0)
                StringBuilder.Append('.');
            StringBuilder.Append(record.Name);
            if (record.GenericParameters.Any())
            {
                StringBuilder.Append('[');
                var isFirst = true;
                foreach (var genericParameter in record.GenericParameters)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        StringBuilder.Append(',');
                    StringBuilder.Append(genericParameter.Name);
                }
                StringBuilder.Append(']');
            }
        }

        protected internal override void VisitStruct(StructDeclaration @struct)
        {
            if (@struct.DeclaringType != (DeclarationNode)null)
                @struct.DeclaringType.Accept(this);
            else
                @struct.Namespace.Accept(this);

            if (StringBuilder.Length > 0)
                StringBuilder.Append('.');
            StringBuilder.Append(@struct.Name);
            if (@struct.GenericParameters.Any())
            {
                StringBuilder.Append('[');
                var isFirst = true;
                foreach (var genericParameter in @struct.GenericParameters)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        StringBuilder.Append(',');
                    StringBuilder.Append(genericParameter.Name);
                }
                StringBuilder.Append(']');
            }
        }

        protected internal override void VisitConstant(ConstantDeclaration constant)
        {
            constant.DeclaringType.Accept(this);
            StringBuilder.Append('.').Append(constant.Name);
        }

        protected internal override void VisitField(FieldDeclaration field)
        {
            field.DeclaringType.Accept(this);
            StringBuilder.Append('.').Append(field.Name);
        }

        protected internal override void VisitConstructor(ConstructorDeclaration constructor)
        {
            constructor.DeclaringType.Accept(this);
            StringBuilder.Append('.').Append(constructor.Name);

            StringBuilder.Append('(');
            var isFirst = true;
            foreach (var parameter in constructor.Parameters)
            {
                if (isFirst)
                    isFirst = false;
                else
                    StringBuilder.Append(',');
                StringBuilder.Append(parameter.Type.GetFullNameReference());
            }
            StringBuilder.Append(')');
        }

        protected internal override void VisitEvent(EventDeclaration @event)
        {
            @event.DeclaringType.Accept(this);
            StringBuilder.Append('.').Append(@event.Name);
        }

        protected internal override void VisitProperty(PropertyDeclaration property)
        {
            property.DeclaringType.Accept(this);
            StringBuilder.Append('.').Append(property.Name);

            if (property.Parameters.Any())
            {
                StringBuilder.Append('[');
                var isFirst = true;
                foreach (var parameter in property.Parameters)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        StringBuilder.Append(',');
                    StringBuilder.Append(parameter.Type.GetFullNameReference());
                }
                StringBuilder.Append(']');
            }
        }

        protected internal override void VisitMethod(MethodDeclaration method)
        {
            method.DeclaringType.Accept(this);
            StringBuilder.Append('.').Append(method.Name);

            if (method.GenericParameters.Any())
                StringBuilder.Append("``").Append(method.GenericParameters.Count);

            StringBuilder.Append('(');
            var isFirst = true;
            foreach (var parameter in method.Parameters)
            {
                if (isFirst)
                    isFirst = false;
                else
                    StringBuilder.Append(',');
                StringBuilder.Append(parameter.Type.GetFullNameReference());
            }
            StringBuilder.Append(')');
        }
    }
}