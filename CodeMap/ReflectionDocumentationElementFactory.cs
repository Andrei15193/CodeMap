using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeMap.Elements;

namespace CodeMap
{
    /// <summary>
    /// Represents a <see cref="DocumentationElement"/> factory for elements corresponding to reflection objects
    /// (<see cref="Type"/>s, <see cref="MethodInfo"/>s and so on).
    /// </summary>
    public class ReflectionDocumentationElementFactory
    {
        private readonly DocumentationElementCache _referencesCahce = new DocumentationElementCache();
        private readonly MemberDocumentationCollection _membersDocumentation;

        /// <summary>Initializes a new instance of the <see cref="ReflectionDocumentationElementFactory"/> class.</summary>
        public ReflectionDocumentationElementFactory()
            : this(new MemberDocumentationCollection(Enumerable.Empty<MemberDocumentation>()))
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ReflectionDocumentationElementFactory"/> class.</summary>
        /// <param name="membersDocumentation">A collection of <see cref="MemberDocumentation"/> to associate to created <see cref="DocumentationElement"/>s.</param>
        public ReflectionDocumentationElementFactory(MemberDocumentationCollection membersDocumentation)
        {
            _referencesCahce = new DocumentationElementCache();
            _membersDocumentation = membersDocumentation
                ?? throw new ArgumentNullException(nameof(membersDocumentation));
        }

        /// <summary>Creates a <see cref="TypeDocumentationElement"/> from the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> from which to create a <see cref="TypeDocumentationElement"/>.</param>
        /// <returns>Returns a <see cref="TypeDocumentationElement"/> from the provided <paramref name="type"/>.</returns>
        public TypeDocumentationElement Create(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.IsEnum)
                return _CreateEnum(type);
            else if (typeof(Delegate).IsAssignableFrom(type))
                return _CreateDelegate(type);
            else if (type.IsInterface)
                return _CreateInterface(type);
            else if (type.IsClass)
                return _CreateClass(type);
            else if (type.IsValueType)
                return _CreateStruct(type);

            throw new ArgumentException($"Unknown type: '{type.Name}'.", nameof(type));
        }

        private EnumDocumentationElement _CreateEnum(Type enumType)
        {
            var enumDocumentationElement = new EnumDocumentationElement
            {
                Name = enumType.Name,
                AccessModifier = _GetAccessModifierFrom(enumType),
                UnderlyingType = _referencesCahce.GetFor(enumType.GetEnumUnderlyingType(), _CreateTypeReference),
                Attributes = _GetAttributesDataFrom(enumType.CustomAttributes),
            };
            enumDocumentationElement.Members = enumType
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField)
                .Select(
                    field => new ConstantDocumentationElement
                    {
                        Name = field.Name,
                        AccessModifier = _GetAccessModifierFrom(field),
                        Value = field.GetValue(null),
                        Type = _referencesCahce.GetFor(field.FieldType, _CreateTypeReference),
                        Attributes = _GetAttributesDataFrom(field.CustomAttributes),
                        DeclaringType = enumDocumentationElement
                    }
                )
                .OrderBy(constant => constant.Value)
                .AsReadOnlyList();

            return enumDocumentationElement;
        }

        private TypeDocumentationElement _CreateDelegate(Type type)
        {
            throw new NotImplementedException();
        }

        private TypeDocumentationElement _CreateInterface(Type type)
        {
            throw new NotImplementedException();
        }

        private TypeDocumentationElement _CreateClass(Type type)
        {
            throw new NotImplementedException();
        }

        private TypeDocumentationElement _CreateStruct(Type type)
        {
            throw new NotImplementedException();
        }

        private AccessModifier _GetAccessModifierFrom(Type type)
        {
            if (type.IsNested)
                if (type.IsNestedPublic)
                    return AccessModifier.Public;
                else if (type.IsNestedFamily)
                    return AccessModifier.Family;
                else if (type.IsNestedFamORAssem)
                    return AccessModifier.AssemblyOrFamily;
                else if (type.IsNestedFamANDAssem)
                    return AccessModifier.AssemblyAndFamily;
                else if (type.IsNestedAssembly)
                    return AccessModifier.Assembly;
                else if (type.IsNestedPrivate)
                    return AccessModifier.Private;
            if (type.IsPublic)
                return AccessModifier.Public;
            else
                return AccessModifier.Assembly;
        }

        private AccessModifier _GetAccessModifierFrom(FieldInfo field)
        {
            if (field.IsPublic)
                return AccessModifier.Public;
            else if (field.IsFamily)
                return AccessModifier.Family;
            else if (field.IsFamilyOrAssembly)
                return AccessModifier.AssemblyOrFamily;
            else if (field.IsFamilyAndAssembly)
                return AccessModifier.AssemblyAndFamily;
            else if (field.IsAssembly)
                return AccessModifier.Assembly;
            else
                return AccessModifier.Private;
        }

        private TypeReferenceDocumentationElement _CreateTypeReference(Type type)
        {
            return new InstanceTypeDocumentationElement
            {
                Name = type.Name,
                Namespace = type.Namespace,
                GenericArguments = type
                    .GetGenericArguments()
                    .Select(genericArgument => _referencesCahce.GetFor(genericArgument, _CreateTypeReference))
                    .AsReadOnlyList(),
                Assembly = _referencesCahce.GetFor(type.Assembly.GetName(), _CreateAssemblyReference)
            };
        }

        private AssemblyReferenceDocumentationElement _CreateAssemblyReference(AssemblyName assemblyName)
            => new AssemblyReferenceDocumentationElement
            {
                Name = assemblyName.Name,
                Version = assemblyName.Version,
                Culture = assemblyName.CultureName,
                PublicKeyToken = assemblyName.GetPublicKeyToken().ToBase16String()
            };

        private IReadOnlyCollection<AttributeData> _GetAttributesDataFrom(IEnumerable<CustomAttributeData> customAttributes)
            => (
                from customAttribute in customAttributes
                let constructorParameters = customAttribute.Constructor.GetParameters()
                select new AttributeData(
                    _referencesCahce.GetFor(customAttribute.AttributeType, _CreateTypeReference),
                    constructorParameters
                        .Zip(
                            customAttribute.ConstructorArguments,
                            (parameter, argument) =>
                                new AttributeParameterData(
                                    _referencesCahce.GetFor(parameter.ParameterType, _CreateTypeReference),
                                    parameter.Name,
                                    _GetAttributeValue(parameter.ParameterType, argument)
                                )
                        )
                        .ToList(),
                    customAttribute
                        .NamedArguments
                        .Select(
                            namedArgument =>
                            {
                                var parameterType = namedArgument.IsField
                                    ? ((FieldInfo)namedArgument.MemberInfo).FieldType
                                    : ((PropertyInfo)namedArgument.MemberInfo).PropertyType;
                                return new AttributeParameterData(
                                    _referencesCahce.GetFor(parameterType, _CreateTypeReference),
                                    namedArgument.MemberName,
                                    _GetAttributeValue(parameterType, namedArgument.TypedValue.Value)
                                );
                            }
                        )
                        .ToList()
            )
        )
        .ToList();

        private static object _GetAttributeValue(Type type, CustomAttributeTypedArgument argument)
        {
            if (argument.Value == null)
                return null;

            if (type == typeof(object[]))
                return _GetAttributeValue(argument.ArgumentType, (object[])argument.Value);
            if (type == typeof(object))
                return _GetAttributeValue(argument.ArgumentType, argument.Value);
            if (type.IsArray)
                return _GetAttributeValue(type, (object[])argument.Value);

            return _GetAttributeValue(type, argument.Value);
        }

        private static object _GetAttributeValue(Type type, object[] values)
        {
            var result = new object[values.Length];
            for (var index = 0; index < values.Length; index++)
                result[index] = _GetAttributeValue(type, values[index]);
            return result;
        }

        private static object _GetAttributeValue(Type type, object value)
        {
            if (value == null)
                return null;

            if (type.IsEnum)
                return Enum.Parse(type, Convert.ToString(value));

            return value;
        }
    }
}