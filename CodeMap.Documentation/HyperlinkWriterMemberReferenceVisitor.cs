using System;
using System.IO;
using System.Linq;
using System.Text;
using CodeMap.Html;
using CodeMap.ReferenceData;

namespace CodeMap.Documentation
{
    internal class HyperlinkWriterMemberReferenceVisitor : MemberReferenceVisitor
    {
        public HyperlinkWriterMemberReferenceVisitor(TextWriter textWriter, IMemberReferenceResolver memberReferenceResolver)
        {
            TextWriter = textWriter ?? throw new ArgumentNullException(nameof(textWriter));
            MemberReferenceResolver = memberReferenceResolver ?? throw new ArgumentNullException(nameof(memberReferenceResolver));
        }

        public TextWriter TextWriter { get; }

        public IMemberReferenceResolver MemberReferenceResolver { get; }

        protected override void VisitAssembly(AssemblyReference assembly)
        {
        }

        protected override void VisitNamespace(NamespaceReference @namespace)
        {
        }

        protected override void VisitType(TypeReference type)
        {
            if (type.DeclaringType is not null)
            {
                type.DeclaringType.Accept(this);
                WriteSafeHtml(".");
            }

            TextWriter.Write("<a href=\"");
            WriteSafeHtml(MemberReferenceResolver.GetUrl(type));
            TextWriter.Write("\">");
            WriteSafeHtml(GetTypeName(type));
            TextWriter.Write("</a>");

            if (type.GenericArguments.Any())
            {
                WriteSafeHtml("<");
                var isFirst = true;
                foreach (var genericArgument in type.GenericArguments)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        WriteSafeHtml(", ");
                    genericArgument.Accept(this);
                }
                WriteSafeHtml(">");
            }
        }

        protected virtual string GetTypeName(TypeReference type)
        {
            if (type == typeof(void))
                return "void";

            if (type == typeof(char))
                return "char";
            if (type == typeof(string))
                return "string";

            if (type == typeof(byte))
                return "byte";
            if (type == typeof(sbyte))
                return "sbyte";
            if (type == typeof(short))
                return "short";
            if (type == typeof(ushort))
                return "ushort";
            if (type == typeof(int))
                return "int";
            if (type == typeof(uint))
                return "uint";
            if (type == typeof(long))
                return "long";
            if (type == typeof(ulong))
                return "ulong";
                
            if (type == typeof(float))
                return "float";
            if (type == typeof(double))
                return "double";
            if (type == typeof(decimal))
                return "decimal";

            return type.Name;
        }

        protected override void VisitGenericTypeParameter(GenericTypeParameterReference genericTypeParameter)
        {
            TextWriter.Write("<code>");
            WriteSafeHtml(genericTypeParameter.Name);
            TextWriter.Write("</code>");
        }

        protected override void VisitArray(ArrayTypeReference array)
        {
            array.ItemType.Accept(this);
            WriteSafeHtml("[");
            WriteSafeHtml(new string(',', array.Rank - 1));
            WriteSafeHtml("]");
        }

        protected override void VisitByRef(ByRefTypeReference byRef)
            => byRef.ReferentType.Accept(this);

        protected override void VisitPointer(PointerTypeReference pointer)
        {
            pointer.ReferentType.Accept(this);
            WriteSafeHtml("*");
        }

        protected override void VisitConstant(ConstantReference constant)
        => throw new NotImplementedException();

        protected override void VisitField(FieldReference field)
        => throw new NotImplementedException();

        protected override void VisitConstructor(ConstructorReference constructor)
        => throw new NotImplementedException();

        protected override void VisitEvent(EventReference @event)
        => throw new NotImplementedException();

        protected override void VisitProperty(PropertyReference property)
        => throw new NotImplementedException();

        protected override void VisitMethod(MethodReference method)
        => throw new NotImplementedException();

        protected override void VisitGenericMethodParameter(GenericMethodParameterReference genericMethodParameter)
        => throw new NotImplementedException();

        protected void WriteSafeHtml(string value)
        {
            var htmlSafeValue = value;
            if (value.Any(@char => @char == '<' || @char == '>' || @char == '&' || @char == '\'' || @char == '"' || char.IsControl(@char)))
                htmlSafeValue = value
                    .Aggregate(
                        new StringBuilder(),
                        (stringBuilder, @char) =>
                        {
                            switch (@char)
                            {
                                case '<':
                                    return stringBuilder.Append("&lt;");

                                case '>':
                                    return stringBuilder.Append("&gt;");

                                case '&':
                                    return stringBuilder.Append("&amp;");

                                case '"':
                                    return stringBuilder.Append("&quot");

                                default:
                                    if (@char == '\'' || char.IsControl(@char))
                                        return stringBuilder.Append("&#x").Append(((short)@char).ToString("x2")).Append(';');
                                    else
                                        return stringBuilder.Append(@char);
                            }
                        }
                    )
                    .ToString();

            TextWriter.Write(htmlSafeValue);
        }
    }
}