using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using CodeMap.DeclarationNodes;
using CodeMap.Documentation.Visitors;
using CodeMap.DocumentationElements;
using CodeMap.ReferenceData;

namespace CodeMap.Documentation
{
    internal static class Extensions
    {
        public static string ToSemver(this Version version)
        {
            var prerelease = string.Empty;
            if (version.Build > 0)
                switch (version.Build / 1000)
                {
                    case 1:
                        prerelease = "-alpha" + version.Build % 1000;
                        break;

                    case 2:
                        prerelease = "-beta" + version.Build % 1000;
                        break;

                    case 3:
                        prerelease = "-rc" + version.Build % 1000;
                        break;
                }

            return $"{version.Major}.{version.Minor}.{version.Revision}{prerelease}";
        }

        public static string CollapseIndentation(this string value)
        {
            var trimmedValue = value.Trim('\r', '\n');
            var match = Regex.Match(trimmedValue, @"^\s+", RegexOptions.Multiline);
            if (match.Success)
            {
                var indentation = match.Value.Length;
                do
                {
                    indentation = Math.Min(indentation, match.Value.Length);
                    match = match.NextMatch();
                } while (match.Success);
                var collapsedText = Regex.Replace(trimmedValue, @$"^\s{{{indentation}}}", string.Empty, RegexOptions.Multiline);
                return collapsedText;
            }
            else
                return trimmedValue;
        }

        public static string GetMemberName(this TypeDeclaration typeDeclaration)
        {
            var visitor = new MemberDeclarationNameVisitor();
            typeDeclaration.Accept(visitor);
            return visitor.Result;
        }

        public static string GetMemberName(this MemberDeclaration memberDeclaration)
        {
            var visitor = new MemberDeclarationNameVisitor();
            memberDeclaration.Accept(visitor);
            return visitor.Result;
        }

        public static string GetMemberName(this MemberReference memberReference)
        {
            if (memberReference == typeof(object))
                return "object";
            if (memberReference == typeof(bool))
                return "bool";
            if (memberReference == typeof(byte))
                return "byte";
            if (memberReference == typeof(sbyte))
                return "sbyte";
            if (memberReference == typeof(short))
                return "short";
            if (memberReference == typeof(ushort))
                return "ushort";
            if (memberReference == typeof(int))
                return "int";
            if (memberReference == typeof(uint))
                return "uint";
            if (memberReference == typeof(long))
                return "long";
            if (memberReference == typeof(float))
                return "float";
            if (memberReference == typeof(double))
                return "double";
            if (memberReference == typeof(decimal))
                return "decimal";
            if (memberReference == typeof(char))
                return "char";
            if (memberReference == typeof(string))
                return "string";
            if (memberReference is DynamicTypeReference)
                return "dynamic";

            var visitor = new MemberReferenceNameVisitor();
            memberReference.Accept(visitor);
            return visitor.Result;
        }

        public static string GetMemberFullName(this MemberInfo member)
        {
            var memberFullNameBuilder = new StringBuilder();
            switch (member)
            {
                case FieldInfo field:
                    memberFullNameBuilder.Append(GetMemberFullName(field.DeclaringType)).Append(field.Name);
                    break;

                case EventInfo @event:
                    memberFullNameBuilder.Append(GetMemberFullName(@event.DeclaringType)).Append(@event.Name);
                    break;

                case PropertyInfo property:
                    memberFullNameBuilder.Append(GetMemberFullName(property.DeclaringType)).Append(property.Name);
                    var indexParameters = property.GetIndexParameters();
                    if (indexParameters.Length > 0)
                    {
                        var isFirst = true;
                        memberFullNameBuilder.Append('[');
                        foreach (var indexParameter in indexParameters)
                        {
                            if (isFirst)
                                isFirst = false;
                            else
                                memberFullNameBuilder.Append(',');
                            memberFullNameBuilder.Append(GetMemberFullName(indexParameter.ParameterType));
                        }
                        memberFullNameBuilder.Append(']');
                    }
                    break;

                case MethodInfo method:
                    memberFullNameBuilder.Append(GetMemberFullName(method.DeclaringType)).Append(method.Name);
                    var methodParameters = method.GetParameters();
                    if (methodParameters.Length > 0)
                    {
                        var isFirst = true;
                        memberFullNameBuilder.Append('(');
                        foreach (var methodParameter in methodParameters)
                        {
                            if (isFirst)
                                isFirst = false;
                            else
                                memberFullNameBuilder.Append(',');
                            memberFullNameBuilder.Append(GetMemberFullName(methodParameter.ParameterType));
                        }
                        memberFullNameBuilder.Append(')');
                    }
                    break;

                case ConstructorInfo constructor:
                    memberFullNameBuilder.Append(GetMemberFullName(constructor.DeclaringType)).Append(constructor.DeclaringType.Name);
                    var constructorParameters = constructor.GetParameters();
                    if (constructorParameters.Length > 0)
                    {
                        var isFirst = true;
                        memberFullNameBuilder.Append('(');
                        foreach (var constructorParameter in constructorParameters)
                        {
                            if (isFirst)
                                isFirst = false;
                            else
                                memberFullNameBuilder.Append(',');
                            memberFullNameBuilder.Append(GetMemberFullName(constructorParameter.ParameterType));
                        }
                        memberFullNameBuilder.Append(')');
                    }
                    break;

                case Type type:
                    memberFullNameBuilder.Append(type.Name);
                    while (type.DeclaringType != null)
                    {
                        memberFullNameBuilder.Insert(0, '.').Insert(0, type.DeclaringType.Name);
                        type = type.DeclaringType;
                    }
                    if (!string.IsNullOrWhiteSpace(type.Namespace))
                        memberFullNameBuilder.Insert(0, '.').Insert(0, type.Namespace);
                    break;
            }
            return memberFullNameBuilder.ToString();
        }

        public static string GetMemberName(this MemberInfo member)
        {
            var memberNameBuilder = new StringBuilder();
            switch (member)
            {
                case FieldInfo field:
                    memberNameBuilder.Append(field.Name);
                    break;

                case EventInfo @event:
                    memberNameBuilder.Append(@event.Name);
                    break;

                case PropertyInfo property:
                    memberNameBuilder.Append(property.Name);
                    var indexParameters = property.GetIndexParameters();
                    if (indexParameters.Length > 0)
                    {
                        var isFirst = true;
                        memberNameBuilder.Append('[');
                        foreach (var indexParameter in indexParameters)
                        {
                            if (isFirst)
                                isFirst = false;
                            else
                                memberNameBuilder.Append(',');
                            memberNameBuilder.Append(GetMemberFullName(indexParameter.ParameterType));
                        }
                        memberNameBuilder.Append(']');
                    }
                    break;

                case MethodInfo methodInfo:
                    memberNameBuilder.Append(methodInfo.Name);
                    var methodParameters = methodInfo.GetParameters();
                    if (methodParameters.Length > 0)
                    {
                        var isFirst = true;
                        memberNameBuilder.Append('(');
                        foreach (var methodParameter in methodParameters)
                        {
                            if (isFirst)
                                isFirst = false;
                            else
                                memberNameBuilder.Append(',');
                            memberNameBuilder.Append(GetMemberFullName(methodParameter.ParameterType));
                        }
                        memberNameBuilder.Append(')');
                    }
                    break;

                case ConstructorInfo constructor:
                    memberNameBuilder.Append(constructor.DeclaringType.Name);
                    var constructorParameters = constructor.GetParameters();
                    if (constructorParameters.Length > 0)
                    {
                        var isFirst = true;
                        memberNameBuilder.Append('(');
                        foreach (var constructorParameter in constructorParameters)
                        {
                            if (isFirst)
                                isFirst = false;
                            else
                                memberNameBuilder.Append(',');
                            memberNameBuilder.Append(GetMemberFullName(constructorParameter.ParameterType));
                        }
                        memberNameBuilder.Append(')');
                    }
                    break;

                case Type type:
                    memberNameBuilder.Append(type.Name);
                    break;
            }
            return memberNameBuilder.ToString();
        }

        public static string GetMemberUrl(this MemberInfoReferenceDocumentationElement memberInfoReference, AssemblyDeclaration library)
            => memberInfoReference.ReferredMember.GetMemberUrl(library);

        public static string GetMemberUrl(this MemberInfo memberInfo, AssemblyDeclaration library)
            => library == memberInfo.Module.Assembly ? memberInfo.GetMemberFullName() + ".html" : memberInfo.GetMicrosoftDocsLink();

        public static string GetMemberUrl(this MemberInfo memberInfo, Assembly library)
            => library == memberInfo.Module.Assembly ? memberInfo.GetMemberFullName() + ".html" : memberInfo.GetMicrosoftDocsLink();

        public static string GetMicrosoftDocsLink(this MemberInfo member)
        {
            var microsoftDocsLinkBuilder = new StringBuilder("https://docs.microsoft.com/dotnet/api/");

            if (member is Type)
                AppendTypePath(member as Type);
            else
                AppendTypePath(member.DeclaringType)
                    .Append('.')
                    .Append(member.Name.ToLowerInvariant());
            microsoftDocsLinkBuilder.Append("?view=netcore-3.1");

            return microsoftDocsLinkBuilder.ToString();

            StringBuilder AppendTypePath(Type type)
            {
                var typeStack = new Stack<Type>();

                while (type != null)
                {
                    typeStack.Push(type);
                    type = type.DeclaringType;
                }

                microsoftDocsLinkBuilder.Append(typeStack.Peek().Namespace);
                while (typeStack.Any())
                    microsoftDocsLinkBuilder.Append('.').Append(typeStack.Pop().Name.Replace('`', '-'));

                return microsoftDocsLinkBuilder;
            }
        }
    }
}