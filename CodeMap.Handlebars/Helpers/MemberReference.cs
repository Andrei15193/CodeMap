using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Handlebars.Visitors;
using CodeMap.ReferenceData;
using HandlebarsDotNet;

namespace CodeMap.Handlebars.Helpers
{
    public class MemberReference : IHandlebarsHelper
    {
        private readonly IMemberReferenceResolver _memberReferenceResolver;

        public MemberReference(IMemberReferenceResolver memberReferenceResolver)
            => _memberReferenceResolver = memberReferenceResolver;

        public string Name
            => nameof(MemberReference);

        public void Apply(TextWriter writer, object context, params object[] parameters)
        {
            var parameter = parameters.DefaultIfEmpty(context).First();
            switch (parameter)
            {
                case DeclarationNode declarationNode:
                    var memberDeclarationNameVisitor = new MemberDeclarationNameVisitor();
                    declarationNode.Accept(memberDeclarationNameVisitor);
                    WriteHyperlink(writer, _memberReferenceResolver.GetFileName(declarationNode), memberDeclarationNameVisitor.Result);
                    break;

                case ReferenceData.MemberReference memberReference:
                    memberReference.Accept(new MemberReferenceHyperlinkVisitor(writer, _memberReferenceResolver, WriteHyperlink));
                    break;

                case MemberInfo memberInfo:
                    _WriteMemberInfoHyperlink(writer, memberInfo);
                    break;

                default:
                    throw new ArgumentException($"Unhandled parameter type: '{parameter.GetType().Name}'");
            }
        }

        protected virtual void WriteHyperlink(TextWriter textWriter, string url, string content)
        {
            textWriter.WriteSafeString("<a href=\"");
            textWriter.Write(url);
            textWriter.WriteSafeString("\">");
            textWriter.Write(content);
            textWriter.WriteSafeString("</a>");
        }

        private void _WriteMemberInfoHyperlink(TextWriter writer, MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case Type type when type.IsArray:
                    _WriteMemberInfoHyperlink(writer, type.GetElementType());
                    writer.Write('[');
                    for (var rankCount = 1; rankCount < type.GetArrayRank(); rankCount++)
                        writer.Write(',');
                    writer.Write(']');
                    break;

                case Type type when type.IsPointer:
                    _WriteMemberInfoHyperlink(writer, type.GetElementType());
                    writer.Write('*');
                    break;

                case Type type when type.IsByRef:
                    _WriteMemberInfoHyperlink(writer, type.GetElementType());
                    break;

                case Type type:
                    WriteHyperlink(writer, _memberReferenceResolver.GetUrl(type), _GetMemberName(type));
                    if (type.GenericTypeArguments.Any())
                        _WriteTypesHyperlinks(writer, '<', type.GenericTypeArguments, '>');
                    break;

                case ConstructorInfo constructorInfo:
                    WriteHyperlink(writer, _memberReferenceResolver.GetUrl(constructorInfo), _GetMemberName(constructorInfo.DeclaringType.Name));
                    _WriteTypesHyperlinks(writer, '(', constructorInfo.GetParameters().Select(parameter => parameter.ParameterType), ')');
                    break;

                case MethodInfo methodInfo:
                    WriteHyperlink(writer, _memberReferenceResolver.GetUrl(methodInfo), _GetMemberName(methodInfo.Name));
                    _WriteTypesHyperlinks(writer, '(', methodInfo.GetParameters().Select(parameter => parameter.ParameterType), ')');
                    break;

                case PropertyInfo propertyInfo:
                    WriteHyperlink(writer, _memberReferenceResolver.GetUrl(propertyInfo), _GetMemberName(propertyInfo.Name));
                    if (propertyInfo.GetIndexParameters().Any())
                        _WriteTypesHyperlinks(writer, '[', propertyInfo.GetIndexParameters().Select(parameter => parameter.ParameterType), ']');
                    break;

                default:
                    WriteHyperlink(writer, _memberReferenceResolver.GetUrl(memberInfo), _GetMemberName(memberInfo.Name));
                    break;
            }
        }

        private void _WriteTypesHyperlinks(TextWriter writer, char openingCharacter, IEnumerable<Type> types, char closingCharacters)
        {
            writer.Write(openingCharacter);
            var isFirst = true;
            foreach (var type in types)
            {
                if (isFirst)
                    isFirst = false;
                else
                    writer.Write(", ");
                _WriteMemberInfoHyperlink(writer, type);
            }
            writer.Write(closingCharacters);
        }

        private void _WriteParameterTypesHyperlinks(TextWriter writer, char openingCharacter, IEnumerable<ParameterInfo> parameters, char closingCharacters)
        {
            writer.Write(openingCharacter);
            var isFirst = true;
            foreach (var parameter in parameters)
            {
                if (isFirst)
                    isFirst = false;
                else
                    writer.Write(", ");
                _WriteMemberInfoHyperlink(writer, parameter.ParameterType);
            }
            writer.Write(closingCharacters);
        }

        private static string _GetMemberName(string typeName)
        {
            var backtickIndex = typeName.IndexOf('`');
            return backtickIndex >= 0 ? typeName.Substring(0, backtickIndex) : typeName;
        }

        private static string _GetMemberName(Type type)
        {
            if (type == typeof(void))
                return "void";
            else if (type == typeof(object))
                return "object";
            else if (type == typeof(bool))
                return "bool";
            else if (type == typeof(byte))
                return "byte";
            else if (type == typeof(sbyte))
                return "sbyte";
            else if (type == typeof(short))
                return "short";
            else if (type == typeof(ushort))
                return "ushort";
            else if (type == typeof(int))
                return "int";
            else if (type == typeof(uint))
                return "uint";
            else if (type == typeof(long))
                return "long";
            else if (type == typeof(float))
                return "float";
            else if (type == typeof(double))
                return "double";
            else if (type == typeof(decimal))
                return "decimal";
            else if (type == typeof(char))
                return "char";
            else if (type == typeof(string))
                return "string";
            else
                return _GetMemberName(type.Name);
        }

        private sealed class MemberReferenceHyperlinkVisitor : MemberReferenceVisitor
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
}