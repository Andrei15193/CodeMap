using System.Linq;
using System.Text;
using CodeMap.ReferenceData;

namespace CodeMap.Html
{
    internal class SimpleNameMemberReferenceVisitor : MemberReferenceVisitor
    {
        public StringBuilder StringBuilder { get; } = new StringBuilder();

        protected internal override void VisitAssembly(AssemblyReference assembly)
            => StringBuilder.Append(assembly.Name);

        protected internal override void VisitNamespace(NamespaceReference @namespace)
            => StringBuilder.Append(@namespace.Name);

        protected internal override void VisitType(TypeReference type)
        {
            if (type is DynamicTypeReference)
                StringBuilder.Append("dynamic");

            else if (type == typeof(void))
                StringBuilder.Append("void");

            else if (type == typeof(object))
                StringBuilder.Append("object");

            else if (type == typeof(bool))
                StringBuilder.Append("bool");

            else if (type == typeof(byte))
                StringBuilder.Append("byte");
            else if (type == typeof(sbyte))
                StringBuilder.Append("sbyte");
            else if (type == typeof(short))
                StringBuilder.Append("short");
            else if (type == typeof(ushort))
                StringBuilder.Append("ushort");
            else if (type == typeof(int))
                StringBuilder.Append("int");
            else if (type == typeof(uint))
                StringBuilder.Append("uint");
            else if (type == typeof(long))
                StringBuilder.Append("long");
            else if (type == typeof(ulong))
                StringBuilder.Append("ulong");

            else if (type == typeof(char))
                StringBuilder.Append("char");
            else if (type == typeof(string))
                StringBuilder.Append("string");

            else if (type == typeof(float))
                StringBuilder.Append("float");
            else if (type == typeof(double))
                StringBuilder.Append("double");
            else if (type == typeof(decimal))
                StringBuilder.Append("decimal");
            else
            {
                if (type.DeclaringType is object)
                    type.DeclaringType.Accept(this);

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
                            StringBuilder.Append(", ");
                        StringBuilder.Append(genericArgument.GetSimpleNameReference());
                    }
                    StringBuilder.Append('>');
                }
            }
        }

        protected internal override void VisitGenericTypeParameter(GenericTypeParameterReference genericTypeParameter)
            => StringBuilder.Append(genericTypeParameter.Name);

        protected internal override void VisitArray(ArrayTypeReference array)
        {
            array.ItemType.Accept(this);
            StringBuilder.Append('[').Append(new string(',', array.Rank - 1)).Append(']');
        }

        protected internal override void VisitByRef(ByRefTypeReference byRef)
        {
            byRef.ReferentType.Accept(this);
            StringBuilder.Append('&');
        }

        protected internal override void VisitPointer(PointerTypeReference pointer)
        {
            pointer.ReferentType.Accept(this);
            StringBuilder.Append('*');
        }

        protected internal override void VisitConstant(ConstantReference constant)
        {
            constant.DeclaringType.Accept(this);
            StringBuilder.Append('.').Append(constant.Name);
        }

        protected internal override void VisitField(FieldReference field)
        {
            field.DeclaringType.Accept(this);
            StringBuilder.Append('.').Append(field.Name);
        }

        protected internal override void VisitConstructor(ConstructorReference constructor)
        {
            constructor.DeclaringType.Accept(this);
            StringBuilder.Append('.');

            StringBuilder.Append(constructor.DeclaringType.Name);
            StringBuilder.Append('(');
            var isFirst = true;
            foreach (var parameterType in constructor.ParameterTypes)
            {
                if (isFirst)
                    isFirst = false;
                else
                    StringBuilder.Append(", ");
                StringBuilder.Append(parameterType.GetSimpleNameReference());
            }
            StringBuilder.Append(')');
        }

        protected internal override void VisitEvent(EventReference @event)
        {
            @event.DeclaringType.Accept(this);
            StringBuilder.Append('.').Append(@event.Name);
        }

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
                        StringBuilder.Append(", ");
                    StringBuilder.Append(parameterType.GetSimpleNameReference());
                }
                StringBuilder.Append(']');
            }
        }

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
                        StringBuilder.Append(", ");
                    StringBuilder.Append(genericArgument.GetSimpleNameReference());
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
                        StringBuilder.Append(", ");
                    StringBuilder.Append(parameterType.GetSimpleNameReference());
                }
                StringBuilder.Append(')');
            }
        }

        protected internal override void VisitGenericMethodParameter(GenericMethodParameterReference genericMethodParameter)
            => StringBuilder.Append(genericMethodParameter.Name);
    }
}