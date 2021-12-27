using System;
using System.Collections.Generic;
using System.Linq;
using CodeMap.ReferenceData;
using HandlebarsDotNet;

namespace CodeMap.Handlebars.Visitors
{
    internal sealed class MemberReferenceHyperlinkVisitor : MemberReferenceVisitor
    {
        private readonly EncodedTextWriter _writer;
        private readonly IMemberReferenceResolver _memberReferenceResolver;
        private readonly Action<EncodedTextWriter, string, string> _writeHyperlink;

        public MemberReferenceHyperlinkVisitor(EncodedTextWriter writer, IMemberReferenceResolver memberReferenceResolver, Action<EncodedTextWriter, string, string> writeHyperlink)
            => (_writer, _memberReferenceResolver, _writeHyperlink) = (writer, memberReferenceResolver, writeHyperlink);

        protected override void VisitAssembly(AssemblyReference assembly)
            => _writeHyperlink(_writer, _memberReferenceResolver.GetUrl(assembly), assembly.Name);

        protected override void VisitNamespace(NamespaceReference @namespace)
            => _writeHyperlink(_writer, _memberReferenceResolver.GetUrl(@namespace), string.IsNullOrWhiteSpace(@namespace.Name) ? "global-namespace" : @namespace.Name);

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

        protected override void VisitArray(ArrayTypeReference array)
        {
            array.ItemType.Accept(this);
            _writer.Write('[');
            for (var rankCount = 1; rankCount < array.Rank; rankCount++)
                _writer.Write(',');
            _writer.Write(']');
        }

        protected override void VisitGenericTypeParameter(GenericTypeParameterReference genericTypeParameter)
            => _writer.Write(genericTypeParameter.Name);

        protected override void VisitConstant(ConstantReference constant)
            => _writeHyperlink(_writer, _memberReferenceResolver.GetUrl(constant), constant.Name);

        protected override void VisitField(FieldReference field)
            => _writeHyperlink(_writer, _memberReferenceResolver.GetUrl(field), field.Name);

        protected override void VisitConstructor(ConstructorReference constructor)
        {
            _writeHyperlink(_writer, _memberReferenceResolver.GetUrl(constructor), constructor.DeclaringType.Name);
            _VisitTypes('(', constructor.ParameterTypes, ')');
        }

        protected override void VisitEvent(EventReference @event)
            => _writeHyperlink(_writer, _memberReferenceResolver.GetUrl(@event), @event.Name);

        protected override void VisitProperty(PropertyReference property)
        {
            _writeHyperlink(_writer, _memberReferenceResolver.GetUrl(property), property.Name);
            if (property.ParameterTypes.Any())
                _VisitTypes('[', property.ParameterTypes, ']');
        }

        protected override void VisitMethod(MethodReference method)
        {
            _writeHyperlink(_writer, _memberReferenceResolver.GetUrl(method), method.Name);
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