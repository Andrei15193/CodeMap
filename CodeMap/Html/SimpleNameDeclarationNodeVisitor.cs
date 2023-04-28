using System.Linq;
using System.Text;
using CodeMap.DeclarationNodes;

namespace CodeMap.Html
{
    /// <summary/>
    public class SimpleNameDeclarationNodeVisitor : DeclarationNodeVisitor
    {
        /// <summary/>
        public SimpleNameDeclarationNodeVisitor()
            : this(null)
        {
        }

        /// <summary/>
        public SimpleNameDeclarationNodeVisitor(StringBuilder stringBuilder)
            => StringBuilder ??= new StringBuilder();

        /// <summary/>
        public StringBuilder StringBuilder { get; set; }

        /// <summary/>
        protected internal override void VisitAssembly(AssemblyDeclaration assembly)
        {
            if (StringBuilder.Length > 0)
                StringBuilder.Append('.');
            StringBuilder.Append(assembly.Name);
        }

        /// <summary/>
        protected internal override void VisitNamespace(NamespaceDeclaration @namespace)
        {
            if (!(@namespace is GlobalNamespaceDeclaration))
            {
                if (StringBuilder.Length > 0)
                    StringBuilder.Append('.');
                StringBuilder.Append(@namespace.Name);
            }
        }

        /// <summary/>
        protected internal override void VisitInterface(InterfaceDeclaration @interface)
        {
            if (StringBuilder.Length > 0)
                StringBuilder.Append('.');
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

        /// <summary/>
        protected internal override void VisitClass(ClassDeclaration @class)
        {
            if (StringBuilder.Length > 0)
                StringBuilder.Append('.');
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

        /// <summary/>
        protected internal override void VisitRecord(RecordDeclaration record)
        {
            if (StringBuilder.Length > 0)
                StringBuilder.Append('.');
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

        /// <summary/>
        protected internal override void VisitStruct(StructDeclaration @struct)
        {
            if (StringBuilder.Length > 0)
                StringBuilder.Append('.');
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

        /// <summary/>
        protected internal override void VisitDelegate(DelegateDeclaration @delegate)
        {
            if (StringBuilder.Length > 0)
                StringBuilder.Append('.');
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

        /// <summary/>
        protected internal override void VisitEnum(EnumDeclaration @enum)
        {
            if (StringBuilder.Length > 0)
                StringBuilder.Append('.');
            StringBuilder.Append(@enum.Name);
        }

        /// <summary/>
        protected internal override void VisitConstant(ConstantDeclaration constant)
        {
            if (StringBuilder.Length > 0)
                StringBuilder.Append('.');
            StringBuilder.Append(constant.Name);
        }

        /// <summary/>
        protected internal override void VisitField(FieldDeclaration field)
        {
            if (StringBuilder.Length > 0)
                StringBuilder.Append('.');
            StringBuilder.Append(field.Name);
        }

        /// <summary/>
        protected internal override void VisitConstructor(ConstructorDeclaration constructor)
        {
            if (StringBuilder.Length > 0)
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

        /// <summary/>
        protected internal override void VisitEvent(EventDeclaration @event)
        {
            if (StringBuilder.Length > 0)
                StringBuilder.Append('.');
            StringBuilder.Append(@event.Name);
        }

        /// <summary/>
        protected internal override void VisitProperty(PropertyDeclaration property)
        {
            if (StringBuilder.Length > 0)
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

        /// <summary/>
        protected internal override void VisitMethod(MethodDeclaration method)
        {
            if (StringBuilder.Length > 0)
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