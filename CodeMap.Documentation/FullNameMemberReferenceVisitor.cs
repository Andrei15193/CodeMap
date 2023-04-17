using System.Linq;
using System.Text;
using CodeMap.ReferenceData;

namespace CodeMap.Documentation
{
    public class FullNameMemberReferenceVisitor : MemberReferenceVisitor
    {
        public FullNameMemberReferenceVisitor()
            : this(null)
        {
        }

        public FullNameMemberReferenceVisitor(StringBuilder stringBuilder)
            => StringBuilder ??= new StringBuilder();

        public StringBuilder StringBuilder { get; set; }

        protected override void VisitAssembly(AssemblyReference assembly)
        {
        }

        protected override void VisitNamespace(NamespaceReference @namespace)
        {
            @namespace.Assembly.Accept(this);
            if (!string.IsNullOrWhiteSpace(@namespace.Name))
            {
                if (StringBuilder.Length > 0)
                    StringBuilder.Append('.');
                StringBuilder.Append(@namespace.Name);
            }
        }

        protected override void VisitType(TypeReference type)
        {
            if (type.DeclaringType is not null)
                type.DeclaringType.Accept(this);
            else
                type.Namespace.Accept(this);

            if (StringBuilder.Length > 0)
                StringBuilder.Append('.');
            StringBuilder.Append(type.Name);
            if (type.GenericArguments.Any())
            {
                StringBuilder.Append('[');
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
                StringBuilder.Append(']');
            }
        }

        protected override void VisitGenericTypeParameter(GenericTypeParameterReference genericTypeParameter)
            => StringBuilder.Append(genericTypeParameter.Name);

        protected override void VisitArray(ArrayTypeReference array)
        {
            array.ItemType.Accept(this);
            StringBuilder.Append('[').Append(new string(',', array.Rank - 1)).Append(']');
        }

        protected override void VisitByRef(ByRefTypeReference byRef)
        {
            byRef.ReferentType.Accept(this);
            StringBuilder.Append('&');
        }

        protected override void VisitPointer(PointerTypeReference pointer)
        {
            pointer.ReferentType.Accept(this);
            StringBuilder.Append('*');
        }

        protected override void VisitConstant(ConstantReference constant)
        {
            constant.DeclaringType.Accept(this);
            StringBuilder.Append('.').Append(constant.Name);
        }

        protected override void VisitField(FieldReference field)
        {
            field.DeclaringType.Accept(this);
            StringBuilder.Append('.').Append(field.Name);
        }

        protected override void VisitConstructor(ConstructorReference constructor)
        {
            constructor.DeclaringType.Accept(this);
            StringBuilder.Append('.').Append(constructor.DeclaringType.Name);
        }

        protected override void VisitEvent(EventReference @event)
        {
            @event.DeclaringType.Accept(this);
            StringBuilder.Append('.').Append(@event.Name);
        }

        protected override void VisitProperty(PropertyReference property)
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
                    parameterType.Accept(this);
                }
                StringBuilder.Append(',');
                StringBuilder.Append(']');
            }
        }

        protected override void VisitMethod(MethodReference method)
        {
            method.DeclaringType.Accept(this);
            StringBuilder.Append('.').Append(method.Name);
            if (method.GenericArguments.Any())
            {
                StringBuilder.Append('[');
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
                StringBuilder.Append(']');
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
                    parameterType.Accept(this);
                }
                StringBuilder.Append(',');
                StringBuilder.Append(')');
            }
        }

        protected override void VisitGenericMethodParameter(GenericMethodParameterReference genericMethodParameter)
            => StringBuilder.Append(genericMethodParameter.Name);
    }
}