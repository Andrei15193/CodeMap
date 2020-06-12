using CodeMap.DocumentationElements;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CodeMap.Json
{
    /// <summary>Represents a JSON writer documentation visitor for storing the documentation of an Assembly as JSON.</summary>
    public class JsonWriterDocumentationVisitor : DocumentationVisitor
    {
        private readonly JsonWriter _jsonWriter;

        /// <summary>Initializes a new instance of the <see cref="JsonWriterDocumentationVisitor"/> class.</summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write the documentation to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="jsonWriter"/> is <c>null</c>.</exception>
        public JsonWriterDocumentationVisitor(JsonWriter jsonWriter)
        {
            _jsonWriter = jsonWriter ?? throw new ArgumentNullException(nameof(jsonWriter));
        }

        /// <summary>Visits the beginning of a summary element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>summary</c> element.</param>
        protected internal override void VisitSummaryBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WritePropertyName("summary");
            _jsonWriter.WriteStartObject();

            _WriteXmlAttributes(xmlAttributes);

            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteStartArray();
        }

        /// <summary>Visits the ending of a summary element.</summary>
        protected internal override void VisitSummaryEnding()
        {
            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits the beginning of a remarks element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>remarks</c> element.</param>
        protected internal override void VisitRemarksBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WritePropertyName("remarks");
            _jsonWriter.WriteStartObject();

            _WriteXmlAttributes(xmlAttributes);

            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteStartArray();
        }

        /// <summary>Visits the ending of a remarks element.</summary>
        protected internal override void VisitRemarksEnding()
        {
            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits the beginning of an example element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>example</c> element.</param>
        protected internal override void VisitExampleBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WriteStartObject();
            _WriteXmlAttributes(xmlAttributes);

            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteStartArray();
        }

        /// <summary>Visits the ending of an example element.</summary>
        protected internal override void VisitExampleEnding()
        {
            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits the beginning of a value element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>value</c> element.</param>
        protected internal override void VisitValueBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WritePropertyName("value");
            _jsonWriter.WriteStartObject();

            _WriteXmlAttributes(xmlAttributes);

            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteStartArray();
        }

        /// <summary>Visits the ending of a value element.</summary>
        protected internal override void VisitValueEnding()
        {
            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits the beginning of a paragraph element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>para</c> element.</param>
        protected internal override void VisitParagraphBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("paragraph");

            _WriteXmlAttributes(xmlAttributes);
            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteStartArray();
        }

        /// <summary>Visits the ending of a paragraph element.</summary>
        protected internal override void VisitParagraphEnding()
        {
            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a code block element.</summary>
        /// <param name="code">The text inside the code block.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>code</c> element.</param>
        protected internal override void VisitCodeBlock(string code, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("codeBlock");

            _WriteXmlAttributes(xmlAttributes);
            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteValue(code);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits the beginning of an unordered list element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>list</c> element.</param>
        protected internal override void VisitUnorderedListBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("unorderedList");

            _WriteXmlAttributes(xmlAttributes);
            _jsonWriter.WritePropertyName("items");
            _jsonWriter.WriteStartArray();
        }

        /// <summary>Visits the ending of an unordered list element.</summary>
        protected internal override void VisitUnorderedListEnding()
        {
            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits the beginning of an ordered list element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>list</c> element.</param>
        protected internal override void VisitOrderedListBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("orderedList");

            _WriteXmlAttributes(xmlAttributes);
            _jsonWriter.WritePropertyName("items");
            _jsonWriter.WriteStartArray();
        }

        /// <summary>Visits the ending of an ordered list element.</summary>
        protected internal override void VisitOrderedListEnding()
        {
            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits the beginning of a list item element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>item</c> or <c>description</c> element.</param>
        protected internal override void VisitListItemBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("listItem");

            _WriteXmlAttributes(xmlAttributes);
            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteStartArray();
        }

        /// <summary>Visits the ending of a list item element.</summary>
        protected internal override void VisitListItemEnding()
        {
            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits the beginning of a definition list element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>list</c> element.</param>
        protected internal override void VisitDefinitionListBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("definitionList");

            _WriteXmlAttributes(xmlAttributes);
        }

        /// <summary>Visits the ending of a definition list element.</summary>
        protected internal override void VisitDefinitionListEnding()
        {
            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits the beginning of a definition list title.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>listheader</c> element.</param>
        protected internal override void VisitDefinitionListTitleBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WritePropertyName("title");
            _jsonWriter.WriteStartObject();

            _WriteXmlAttributes(xmlAttributes);
            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteStartArray();
        }

        /// <summary>Visits the ending of a definition list title.</summary>
        protected internal override void VisitDefinitionListTitleEnding()
        {
            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();

            _jsonWriter.WritePropertyName("items");
            _jsonWriter.WriteStartArray();
        }

        /// <summary>Visits the beginning of a definition list item.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>item</c> element.</param>
        protected internal override void VisitDefinitionListItemBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WriteStartObject();
            _WriteXmlAttributes(xmlAttributes);
        }

        /// <summary>Visits the ending of a definition list item.</summary>
        protected internal override void VisitDefinitionListItemEnding()
        {
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits the beginning of a definition list term.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>term</c> element.</param>
        protected internal override void VisitDefinitionTermBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WritePropertyName("term");
            _jsonWriter.WriteStartObject();

            _WriteXmlAttributes(xmlAttributes);
            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteStartArray();
        }

        /// <summary>Visits the ending of a definition list term.</summary>
        protected internal override void VisitDefinitionTermEnding()
        {
            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits the beginning of a definition list term description.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>description</c> element.</param>
        protected internal override void VisitDefinitionTermDescriptionBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WritePropertyName("description");
            _jsonWriter.WriteStartObject();

            _WriteXmlAttributes(xmlAttributes);
            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteStartArray();
        }

        /// <summary>Visits the ending of a definition list term description.</summary>
        protected internal override void VisitDefinitionTermDescriptionEnding()
        {
            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits the beginning of a table.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>list</c> element.</param>
        protected internal override void VisitTableBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("table");

            _WriteXmlAttributes(xmlAttributes);
        }

        /// <summary>Visits the ending of a table.</summary>
        protected internal override void VisitTableEnding()
        {
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits the beginning of a table heading.</summary>
        protected internal override void VisitTableHeadingBeginning()
        {
            _jsonWriter.WritePropertyName("columns");
            _jsonWriter.WriteStartArray();
        }

        /// <summary>Visits the ending of a table heading.</summary>
        protected internal override void VisitTableHeadingEnding()
        {
            _jsonWriter.WriteEndArray();
        }

        /// <summary>Visits the beginning of a table body.</summary>
        protected internal override void VisitTableBodyBeginning()
        {
            _jsonWriter.WritePropertyName("rows");
            _jsonWriter.WriteStartArray();
        }

        /// <summary>Visits the ending of a table body.</summary>
        protected internal override void VisitTableBodyEnding()
        {
            _jsonWriter.WriteEndArray();
        }

        /// <summary>Visits the beginning of a table column.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>term</c> element.</param>
        protected internal override void VisitTableColumnBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WriteStartObject();
            _WriteXmlAttributes(xmlAttributes);
            _jsonWriter.WritePropertyName("name");
            _jsonWriter.WriteStartArray();
        }

        /// <summary>Visits the ending of a table column.</summary>
        protected internal override void VisitTableColumnEnding()
        {
            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits the beginning of a table row.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>item</c> element.</param>
        protected internal override void VisitTableRowBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WriteStartObject();
            _WriteXmlAttributes(xmlAttributes);
            _jsonWriter.WritePropertyName("cells");
            _jsonWriter.WriteStartArray();
        }

        /// <summary>Visits the ending of a table row.</summary>
        protected internal override void VisitTableRowEnding()
        {
            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits the beginning of a table cell.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>description</c> element.</param>
        protected internal override void VisitTableCellBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WriteStartObject();
            _WriteXmlAttributes(xmlAttributes);
            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteStartArray();
        }

        /// <summary>Visits the ending of a table cell.</summary>
        protected internal override void VisitTableCellEnding()
        {
            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits plain text.</summary>
        /// <param name="text">The plain text inside a block element.</param>
        protected internal override void VisitText(string text)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("text");

            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteValue(text);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits an inline member reference.</summary>
        /// <param name="canonicalName">The canonical name of the referred member.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>see</c> or <c>seealso</c> element.</param>
        protected internal override void VisitInlineReference(string canonicalName, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("unresolvedReference");

            _WriteXmlAttributes(xmlAttributes);

            _jsonWriter.WritePropertyName("canonicalName");
            _jsonWriter.WriteValue(canonicalName);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits an inline member reference.</summary>
        /// <param name="referredMember">The referred member.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>see</c> or <c>seealso</c> element.</param>
        protected internal override void VisitInlineReference(MemberInfo referredMember, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("reference");

            _WriteXmlAttributes(xmlAttributes);

            _jsonWriter.WritePropertyName("id");
            _jsonWriter.WriteValue(_GetMemberInfoIdFor(referredMember));

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits an inline code snippet.</summary>
        /// <param name="code">The text inside the inline code.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>c</c> element.</param>
        protected internal override void VisitInlineCode(string code, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("code");

            _WriteXmlAttributes(xmlAttributes);

            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteValue(code);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits an inline parameter reference.</summary>
        /// <param name="parameterName">The name of the referred parameter.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>paramref</c> element.</param>
        protected internal override void VisitParameterReference(string parameterName, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("parameterReference");

            _WriteXmlAttributes(xmlAttributes);

            _jsonWriter.WritePropertyName("parameterName");
            _jsonWriter.WriteValue(parameterName);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits an inline generic parameter reference.</summary>
        /// <param name="genericParameterName">The name of the referred generic parameter.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>typeparamref</c> element.</param>
        protected internal override void VisitGenericParameterReference(string genericParameterName, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("genericParameterReference");

            _WriteXmlAttributes(xmlAttributes);

            _jsonWriter.WritePropertyName("parameterName");
            _jsonWriter.WriteValue(genericParameterName);

            _jsonWriter.WriteEndObject();
        }

        private static string _GetMemberInfoIdFor(MemberInfo memberInfo)
            => _AppendMemberInfoId(new StringBuilder(), memberInfo).ToString();

        private static StringBuilder _AppendMemberInfoId(StringBuilder idBuilder, MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case Type type when (type.IsGenericTypeParameter):
                    return idBuilder.Append('`').Append(type.GenericParameterPosition);

                case Type type when (type.IsGenericMethodParameter):
                    return idBuilder.Append("``").Append(type.GenericParameterPosition);

                case Type type when (type.DeclaringType != null):
                    return _AppendMemberInfoId(idBuilder, type.DeclaringType).Append('.').Append(type.Name);

                case Type type:
                    if (string.IsNullOrWhiteSpace(type.Namespace))
                        return idBuilder.Append(type.Name);
                    else
                        return idBuilder.Append(type.Namespace).Append('.').Append(type.Name);

                case ConstructorInfo constructor:
                    _AppendMemberInfoId(idBuilder, constructor.DeclaringType).Append('.').Append(constructor.Name);
                    return AppendParameters(constructor.GetParameters());

                case PropertyInfo property:
                    _AppendMemberInfoId(idBuilder, property.DeclaringType).Append('.').Append(property.Name);
                    return AppendParameters(property.GetIndexParameters());

                case MethodInfo method:
                    _AppendMemberInfoId(idBuilder, method.DeclaringType).Append('.').Append(method.Name);
                    return AppendParameters(method.GetParameters());

                default:
                    if (memberInfo.DeclaringType != null)
                        return _AppendMemberInfoId(idBuilder, memberInfo.DeclaringType).Append('.').Append(memberInfo.Name);
                    else
                        return new StringBuilder(memberInfo.Name);
            }

            StringBuilder AppendParameters(IEnumerable<ParameterInfo> parameters)
            {
                using (var parameter = parameters.GetEnumerator())
                    if (parameter.MoveNext())
                    {
                        idBuilder.Append('(');
                        _AppendMemberInfoId(idBuilder, parameter.Current.ParameterType);
                        while (parameter.MoveNext())
                        {
                            idBuilder.Append(',');
                            _AppendMemberInfoId(idBuilder, parameter.Current.ParameterType);
                        }
                        idBuilder.Append(')');
                    }
                return idBuilder;
            }
        }

        private void _WriteXmlAttributes(IReadOnlyDictionary<string, string> xmlAttributes)
        {
            _jsonWriter.WritePropertyName("xmlAttributes");
            _jsonWriter.WriteStartObject();
            foreach (var xmlAttribute in xmlAttributes)
            {
                _jsonWriter.WritePropertyName(xmlAttribute.Key);
                _jsonWriter.WriteValue(xmlAttribute.Value);
            }
            _jsonWriter.WriteEndObject();
        }
    }
}