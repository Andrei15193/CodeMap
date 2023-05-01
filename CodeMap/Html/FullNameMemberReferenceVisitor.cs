using System.Linq;
using System.Text;
using CodeMap.ReferenceData;

namespace CodeMap.Html
{
    /// <summary/>
    public class FullNameMemberReferenceVisitor : MemberReferenceVisitor
    {
        /// <summary/>
        public FullNameMemberReferenceVisitor()
            : this(null)
        {
        }

        /// <summary/>
        public FullNameMemberReferenceVisitor(StringBuilder stringBuilder)
            => StringBuilder ??= new StringBuilder();

        /// <summary/>
        public StringBuilder StringBuilder { get; set; }

        /// <summary/>
        protected internal override void VisitAssembly(AssemblyReference assembly)
        {
        }

        /// <summary/>
        protected internal override void VisitNamespace(NamespaceReference @namespace)
        {
            @namespace.Assembly.Accept(this);
            if (!string.IsNullOrWhiteSpace(@namespace.Name))
            {
                if (StringBuilder.Length > 0)
                    StringBuilder.Append('.');
                StringBuilder.Append(@namespace.Name);
            }
        }

        /// <summary/>
        protected internal override void VisitType(TypeReference type)
        {
            if (type.DeclaringType != (TypeReference)null)
                type.DeclaringType.Accept(this);
            else
                type.Namespace.Accept(this);

            if (StringBuilder.Length > 0)
                StringBuilder.Append('.');
            StringBuilder.Append(type.Name);
            if (type.GenericArguments.Any())
            {
                StringBuilder.Append('<');
                var isFirst = true;
                foreach (var genericArgument in type.GenericArguments)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        StringBuilder.Append(',');

                    var memberReferenceVisitor = new FullNameMemberReferenceVisitor();
                    genericArgument.Accept(memberReferenceVisitor);
                    StringBuilder.Append(memberReferenceVisitor.StringBuilder);
                }
                StringBuilder.Append('>');
            }
        }

        /// <summary/>
        protected internal override void VisitGenericTypeParameter(GenericTypeParameterReference genericTypeParameter)
            => StringBuilder.Append(genericTypeParameter.Name);

        /// <summary/>
        protected internal override void VisitArray(ArrayTypeReference array)
        {
            array.ItemType.Accept(this);
            StringBuilder.Append('[').Append(new string(',', array.Rank - 1)).Append(']');
        }

        /// <summary/>
        protected internal override void VisitByRef(ByRefTypeReference byRef)
        {
            byRef.ReferentType.Accept(this);
            StringBuilder.Append('&');
        }

        /// <summary/>
        protected internal override void VisitPointer(PointerTypeReference pointer)
        {
            pointer.ReferentType.Accept(this);
            StringBuilder.Append('*');
        }

        /// <summary/>
        protected internal override void VisitConstant(ConstantReference constant)
        {
            constant.DeclaringType.Accept(this);
            StringBuilder.Append('.').Append(constant.Name);
        }

        /// <summary/>
        protected internal override void VisitField(FieldReference field)
        {
            field.DeclaringType.Accept(this);
            StringBuilder.Append('.').Append(field.Name);
        }

        /// <summary/>
        protected internal override void VisitConstructor(ConstructorReference constructor)
        {
            constructor.DeclaringType.Accept(this);
            StringBuilder.Append('.').Append(constructor.DeclaringType.Name);
        }

        /// <summary/>
        protected internal override void VisitEvent(EventReference @event)
        {
            @event.DeclaringType.Accept(this);
            StringBuilder.Append('.').Append(@event.Name);
        }

        /// <summary/>
        protected internal override void VisitProperty(PropertyReference property)
        {
            property.DeclaringType.Accept(this);
            StringBuilder.Append('.').Append(property.Name);

            if (property.ParameterTypes.Any())
            {
                StringBuilder.Append('[');
                var isFirst = true;
                foreach (var parameterType in property.ParameterTypes)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        StringBuilder.Append(',');
                    StringBuilder.Append(parameterType.GetFullNameReference());
                }
                StringBuilder.Append(']');
            }
        }

        /// <summary/>
        protected internal override void VisitMethod(MethodReference method)
        {
            method.DeclaringType.Accept(this);
            StringBuilder.Append('.').Append(method.Name);
            if (method.GenericArguments.Any())
            {
                StringBuilder.Append('<');
                var isFirst = true;
                foreach (var genericArgument in method.GenericArguments)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        StringBuilder.Append(',');
                    var memberReferenceVisitor = new FullNameMemberReferenceVisitor();
                    genericArgument.Accept(memberReferenceVisitor);
                    StringBuilder.Append(memberReferenceVisitor.StringBuilder);
                }
                StringBuilder.Append('>');
            }

            if (method.ParameterTypes.Any())
            {
                StringBuilder.Append('(');
                var isFirst = true;
                foreach (var parameterType in method.ParameterTypes)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        StringBuilder.Append(',');
                    StringBuilder.Append(parameterType.GetFullNameReference());
                }
                StringBuilder.Append(',');
                StringBuilder.Append(')');
            }
        }

        /// <summary/>
        protected internal override void VisitGenericMethodParameter(GenericMethodParameterReference genericMethodParameter)
            => StringBuilder.Append(genericMethodParameter.Name);
    }
}