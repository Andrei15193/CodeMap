using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeMap.ReferenceData;

namespace CodeMap.Handlebars.Visitors
{
    internal sealed class MemberReferenceHyperlinkVisitor : MemberReferenceVisitor
    {
        private readonly TextWriter _writer;
        private readonly IMemberReferenceResolver _memberReferenceResolver;
        private readonly Action<TextWriter, string, string> _writeHyperlink;

        public MemberReferenceHyperlinkVisitor(TextWriter writer, IMemberReferenceResolver memberReferenceResolver, Action<TextWriter, string, string> writeHyperlink)
            => (_writer, _memberReferenceResolver, _writeHyperlink) = (writer, memberReferenceResolver, writeHyperlink);

        protected override void VisitAssembly(AssemblyReference assembly)
            => throw new NotImplementedException("Assembly references are not handled.");

        protected override void VisitArray(ArrayTypeReference array)
        {
            array.ItemType.Accept(this);
            _writer.Write('[');
            for (var rankCount = 1; rankCount < array.Rank; rankCount++)
                _writer.Write(',');
            _writer.Write(']');
        }

        protected override void VisitType(TypeReference type)
        {
            _writeHyperlink(_writer, _memberReferenceResolver.GetUrl(type), _GetTypeName(type));
            if (type.GenericArguments.Any())
                _VisitTypes('<', type.GenericArguments, '>');
        }

        protected override void VisitPointer(PointerTypeReference pointer)
        {
            pointer.ReferentType.Accept(this);
            _writer.Write('*');
        }

        protected override void VisitByRef(ByRefTypeReference byRef)
            => byRef.ReferentType.Accept(this);

        protected override void VisitGenericTypeParameter(GenericTypeParameterReference genericTypeParameter)
            => _writer.Write(genericTypeParameter.Name);

        protected override void VisitConstant(ConstantReference constant)
            => _writeHyperlink(_writer, constant.Name, _memberReferenceResolver.GetUrl(constant));

        protected override void VisitField(FieldReference field)
            => _writeHyperlink(_writer, field.Name, _memberReferenceResolver.GetUrl(field));

        protected override void VisitConstructor(ConstructorReference constructor)
        {
            _writeHyperlink(_writer, constructor.DeclaringType.Name, _memberReferenceResolver.GetUrl(constructor));
            _VisitTypes('(', constructor.ParameterTypes, ')');
        }

        protected override void VisitEvent(EventReference @event)
            => _writeHyperlink(_writer, @event.Name, _memberReferenceResolver.GetUrl(@event));

        protected override void VisitProperty(PropertyReference property)
        {
            _writeHyperlink(_writer, property.Name, _memberReferenceResolver.GetUrl(property));
            if (property.ParameterTypes.Any())
                _VisitTypes('[', property.ParameterTypes, ']');
        }

        protected override void VisitMethod(MethodReference method)
        {
            _writeHyperlink(_writer, method.Name, _memberReferenceResolver.GetUrl(method));
            _VisitTypes('(', method.ParameterTypes, ')');
        }

        protected override void VisitGenericMethodParameter(GenericMethodParameterReference genericMethodParameter)
            => _writer.Write(genericMethodParameter.Name);

        private void _VisitTypes(char openingCharacter, IEnumerable<BaseTypeReference> types, char closingCharacter)
        {
            _writer.Write(openingCharacter);
            var isFirst = true;
            foreach (var type in types)
            {
                if (isFirst)
                    isFirst = false;
                else
                    _writer.Write(", ");
                type.Accept(this);
            }
            _writer.Write(closingCharacter);
        }

        private static string _GetTypeName(TypeReference typeReference)
        {
            if (typeReference == typeof(void))
                return "void";
            else if (typeReference == typeof(object))
                return "object";
            else if (typeReference == typeof(bool))
                return "bool";
            else if (typeReference == typeof(byte))
                return "byte";
            else if (typeReference == typeof(sbyte))
                return "sbyte";
            else if (typeReference == typeof(short))
                return "short";
            else if (typeReference == typeof(ushort))
                return "ushort";
            else if (typeReference == typeof(int))
                return "int";
            else if (typeReference == typeof(uint))
                return "uint";
            else if (typeReference == typeof(long))
                return "long";
            else if (typeReference == typeof(float))
                return "float";
            else if (typeReference == typeof(double))
                return "double";
            else if (typeReference == typeof(decimal))
                return "decimal";
            else if (typeReference == typeof(char))
                return "char";
            else if (typeReference == typeof(string))
                return "string";
            else if (typeReference is DynamicTypeReference)
                return "dynamic";
            else
                return typeReference.Name;
        }
    }
}