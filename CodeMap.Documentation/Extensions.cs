using System;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeMap.Documentation
{
    internal static class Extensions
    {
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

        public static string GetMicrosoftDocsLink(this MemberInfo member)
        {
            var microsoftDocsLinkBuilder = new StringBuilder((member.DeclaringType != null ? GetMemberFullName(member.DeclaringType) : GetMemberFullName(member)).ToLowerInvariant())
                .Append("?view=netcore-3.1#");

            switch (member)
            {
                case FieldInfo _:
                case EventInfo _:
                    microsoftDocsLinkBuilder.Append(GetMemberFullName(member.DeclaringType)).Replace(".", "_").Replace("`", "__").Append('_').Append(member.Name);
                    break;

                case PropertyInfo property:
                    microsoftDocsLinkBuilder.Append(GetMemberFullName(property.DeclaringType).Replace(".", "_").Replace("`", "__")).Append('_').Append(property.Name);
                    var indexParameters = property.GetIndexParameters();
                    if (indexParameters.Length > 0)
                    {
                        var isFirst = true;
                        microsoftDocsLinkBuilder.Append('_');
                        foreach (var indexParameter in indexParameters)
                        {
                            if (isFirst)
                                isFirst = false;
                            else
                                microsoftDocsLinkBuilder.Append('_');
                            microsoftDocsLinkBuilder.Append(GetMemberFullName(indexParameter.ParameterType));
                        }
                        microsoftDocsLinkBuilder.Append('_');
                    }
                    break;

                case MethodInfo method:
                    microsoftDocsLinkBuilder.Append(GetMemberFullName(method.DeclaringType).Replace(".", "_").Replace("`", "__")).Append('_').Append(method.Name);
                    var methodParameters = method.GetParameters();
                    if (methodParameters.Length > 0)
                    {
                        var isFirst = true;
                        microsoftDocsLinkBuilder.Append('_');
                        foreach (var methodParameter in methodParameters)
                        {
                            if (isFirst)
                                isFirst = false;
                            else
                                microsoftDocsLinkBuilder.Append('_');
                            microsoftDocsLinkBuilder.Append(GetMemberFullName(methodParameter.ParameterType));
                        }
                        microsoftDocsLinkBuilder.Append('_');
                    }
                    break;

                case ConstructorInfo constructor:
                    microsoftDocsLinkBuilder.Append(GetMemberFullName(constructor.DeclaringType).Replace(".", "_").Replace("`", "__")).Append('_').Append("ctor");
                    var constructorParameters = constructor.GetParameters();
                    if (constructorParameters.Length > 0)
                    {
                        var isFirst = true;
                        microsoftDocsLinkBuilder.Append('_');
                        foreach (var constructorParameter in constructorParameters)
                        {
                            if (isFirst)
                                isFirst = false;
                            else
                                microsoftDocsLinkBuilder.Append('_');
                            microsoftDocsLinkBuilder.Append(GetMemberFullName(constructorParameter.ParameterType));
                        }
                        microsoftDocsLinkBuilder.Append('_');
                    }
                    break;

                case Type type:
                    microsoftDocsLinkBuilder.Append(type.Name);
                    while (type.DeclaringType != null)
                    {
                        microsoftDocsLinkBuilder.Insert(0, '_').Insert(0, type.DeclaringType.Name);
                        type = type.DeclaringType;
                    }
                    if (!string.IsNullOrWhiteSpace(type.Namespace))
                        microsoftDocsLinkBuilder.Insert(0, '_').Insert(0, type.Namespace);
                    break;
            }
            return microsoftDocsLinkBuilder.Insert(0, "https://docs.microsoft.com/dotnet/api/").ToString();
        }
    }
}