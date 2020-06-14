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

        /// <summary>Visits a summary element.</summary>
        /// <param name="summary">The <see cref="SummaryDocumentationElement"/> to visit.</param>
        protected internal override void VisitSummary(SummaryDocumentationElement summary)
        {
            _jsonWriter.WritePropertyName("summary");
            _jsonWriter.WriteStartObject();

            _WriteXmlAttributes(summary.XmlAttributes);

            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteStartArray();

            foreach (var element in summary.Content)
                element.Accept(this);

            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a remarks element.</summary>
        /// <param name="remarks">The <see cref="RemarksDocumentationElement"/> to visit.</param>
        protected internal override void VisitRemarks(RemarksDocumentationElement remarks)
        {
            _jsonWriter.WritePropertyName("remarks");
            _jsonWriter.WriteStartObject();

            _WriteXmlAttributes(remarks.XmlAttributes);

            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteStartArray();

            foreach (var element in remarks.Content)
                element.Accept(this);

            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits an example element.</summary>
        /// <param name="example">The <see cref="ExampleDocumentationElement"/> to visit.</param>
        protected internal override void VisitExample(ExampleDocumentationElement example)
        {
            _jsonWriter.WriteStartObject();
            _WriteXmlAttributes(example.XmlAttributes);

            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteStartArray();

            foreach (var element in example.Content)
                element.Accept(this);

            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a value element.</summary>
        /// <param name="value">The <see cref="ValueDocumentationElement"/> to visit.</param>
        protected internal override void VisitValue(ValueDocumentationElement value)
        {
            _jsonWriter.WritePropertyName("value");
            _jsonWriter.WriteStartObject();

            _WriteXmlAttributes(value.XmlAttributes);

            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteStartArray();

            foreach (var element in value.Content)
                element.Accept(this);

            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a paragraph element.</summary>
        /// <param name="paragraph">The <see cref="ParagraphDocumentationElement"/> to visit.</param>
        protected internal override void VisitParagraph(ParagraphDocumentationElement paragraph)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("paragraph");

            _WriteXmlAttributes(paragraph.XmlAttributes);
            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteStartArray();

            foreach (var element in paragraph.Content)
                element.Accept(this);

            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a code block element.</summary>
        /// <param name="codeBlock">The <see cref="CodeBlockDocumentationElement"/> to visit.</param>
        protected internal override void VisitCodeBlock(CodeBlockDocumentationElement codeBlock)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("codeBlock");

            _WriteXmlAttributes(codeBlock.XmlAttributes);
            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteValue(codeBlock.Code);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits an unordered list element.</summary>
        /// <param name="unorderedList">The <see cref="UnorderedListDocumentationElement"/> to visit.</param>
        protected internal override void VisitUnorderedList(UnorderedListDocumentationElement unorderedList)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("unorderedList");

            _WriteXmlAttributes(unorderedList.XmlAttributes);
            _jsonWriter.WritePropertyName("items");
            _jsonWriter.WriteStartArray();

            foreach (var item in unorderedList.Items)
                item.Accept(this);

            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits an ordered list element.</summary>
        /// <param name="orderedList">The <see cref="OrderedListDocumentationElement"/> to visit.</param>
        protected internal override void VisitOrderedList(OrderedListDocumentationElement orderedList)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("orderedList");

            _WriteXmlAttributes(orderedList.XmlAttributes);
            _jsonWriter.WritePropertyName("items");
            _jsonWriter.WriteStartArray();

            foreach (var item in orderedList.Items)
                item.Accept(this);

            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a list item element.</summary>
        /// <param name="listItem">The <see cref="ListItemDocumentationElement"/> to visit.</param>
        protected internal override void VisitListItem(ListItemDocumentationElement listItem)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("listItem");

            _WriteXmlAttributes(listItem.XmlAttributes);
            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteStartArray();

            foreach (var element in listItem.Content)
                element.Accept(this);

            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a definition list element.</summary>
        /// <param name="definitionList">The <see cref="DefinitionListDocumentationElement"/> to visit.</param>
        protected internal override void VisitDefinitionList(DefinitionListDocumentationElement definitionList)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("definitionList");

            _WriteXmlAttributes(definitionList.XmlAttributes);

            definitionList.ListTitle?.Accept(this);

            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();

            _jsonWriter.WritePropertyName("items");
            _jsonWriter.WriteStartArray();

            foreach (var item in definitionList.Items)
                item.Accept(this);

            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a definition list title element.</summary>
        /// <param name="definitionListTitle">The <see cref="DefinitionListTitleDocumentationElement"/> to visit.</param>
        protected internal override void VisitDefinitionListTitle(DefinitionListTitleDocumentationElement definitionListTitle)
        {
            _jsonWriter.WritePropertyName("title");
            _jsonWriter.WriteStartObject();

            _WriteXmlAttributes(definitionListTitle.XmlAttributes);
            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteStartArray();

            foreach (var element in definitionListTitle.Content)
                element.Accept(this);
        }

        /// <summary>Visits a definition list item.</summary>
        /// <param name="definitionListItem">The <see cref="DefinitionListItemDocumentationElement"/> to visit.</param>
        protected internal override void VisitDefinitionListItem(DefinitionListItemDocumentationElement definitionListItem)
        {
            _jsonWriter.WriteStartObject();
            _WriteXmlAttributes(definitionListItem.XmlAttributes);
            definitionListItem.Term.Accept(this);
            definitionListItem.Description.Accept(this);
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a definition list item term.</summary>
        /// <param name="definitionListItemTerm">The <see cref="DefinitionListItemTermDocumentationElement"/> to visit.</param>
        protected internal override void VisitDefinitionListItemTerm(DefinitionListItemTermDocumentationElement definitionListItemTerm)
        {
            _jsonWriter.WritePropertyName("term");
            _jsonWriter.WriteStartObject();

            _WriteXmlAttributes(definitionListItemTerm.XmlAttributes);
            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteStartArray();

            foreach (var element in definitionListItemTerm.Content)
                element.Accept(this);

            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a definition list item description.</summary>
        /// <param name="definitionListItemDescription">The <see cref="DefinitionListItemDescriptionDocumentationElement"/> to visit.</param>
        protected internal override void VisitDefinitionListItemDescription(DefinitionListItemDescriptionDocumentationElement definitionListItemDescription)
        {
            _jsonWriter.WritePropertyName("description");
            _jsonWriter.WriteStartObject();

            _WriteXmlAttributes(definitionListItemDescription.XmlAttributes);
            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteStartArray();

            foreach (var element in definitionListItemDescription.Content)
                element.Accept(this);

            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a table.</summary>
        /// <param name="table">The <see cref="TableDocumentationElement"/> to visit.</param>
        protected internal override void VisitTable(TableDocumentationElement table)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("table");

            _WriteXmlAttributes(table.XmlAttributes);

            _jsonWriter.WritePropertyName("columns");
            _jsonWriter.WriteStartArray();
            foreach (var column in table.Columns)
                column.Accept(this);
            _jsonWriter.WriteEndArray();

            _jsonWriter.WritePropertyName("rows");
            _jsonWriter.WriteStartArray();
            foreach (var row in table.Rows)
                row.Accept(this);
            _jsonWriter.WriteEndArray();

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a table column.</summary>
        /// <param name="tableColumn">The <see cref="TableColumnDocumentationElement"/> to visit.</param>
        protected internal override void VisitTableColumn(TableColumnDocumentationElement tableColumn)
        {
            _jsonWriter.WriteStartObject();
            _WriteXmlAttributes(tableColumn.XmlAttributes);
            _jsonWriter.WritePropertyName("name");
            _jsonWriter.WriteStartArray();

            foreach (var element in tableColumn.Name)
                element.Accept(this);

            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a table row.</summary>
        /// <param name="tableRow">The <see cref="TableRowDocumentationElement"/> to visit.</param>
        protected internal override void VisitTableRow(TableRowDocumentationElement tableRow)
        {
            _jsonWriter.WriteStartObject();
            _WriteXmlAttributes(tableRow.XmlAttributes);
            _jsonWriter.WritePropertyName("cells");
            _jsonWriter.WriteStartArray();

            foreach (var cell in tableRow.Cells)
                cell.Accept(this);

            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a table cell.</summary>
        /// <param name="tableCell">The <see cref="TableCellDocumentationElement"/> to visit.</param>
        protected internal override void VisitTableCell(TableCellDocumentationElement tableCell)
        {
            _jsonWriter.WriteStartObject();
            _WriteXmlAttributes(tableCell.XmlAttributes);
            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteStartArray();

            foreach (var element in tableCell.Content)
                element.Accept(this);

            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits plain text.</summary>
        /// <param name="text">The <see cref="TextDocumentationElement"/> to visit.</param>
        protected internal override void VisitText(TextDocumentationElement text)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("text");

            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteValue(text.Text);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits a hyperlink.</summary>
        /// <param name="hyperlink">The <see cref="HyperlinkDocumentationElement"/> to visit.</param>
        protected internal override void VisitHyperlink(HyperlinkDocumentationElement hyperlink)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("hyperlink");

            _jsonWriter.WritePropertyName("destination");
            _jsonWriter.WriteValue(hyperlink.Destination);

            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteValue(hyperlink.Text);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits an unresolved inline member reference.</summary>
        /// <param name="memberNameReference">The <see cref="MemberNameReferenceDocumentationElement"/> to visit.</param>
        protected internal override void VisitInlineReference(MemberNameReferenceDocumentationElement memberNameReference)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("unresolvedReference");

            _WriteXmlAttributes(memberNameReference.XmlAttributes);

            _jsonWriter.WritePropertyName("canonicalName");
            _jsonWriter.WriteValue(memberNameReference.CanonicalName);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits an inline member reference.</summary>
        /// <param name="memberInfoReference">The <see cref="MemberInfoReferenceDocumentationElement"/> to visit.</param>
        protected internal override void VisitInlineReference(MemberInfoReferenceDocumentationElement memberInfoReference)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("reference");

            _WriteXmlAttributes(memberInfoReference.XmlAttributes);

            _jsonWriter.WritePropertyName("id");
            _jsonWriter.WriteValue(_GetMemberInfoIdFor(memberInfoReference.ReferredMember));

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits an inline code snippet.</summary>
        /// <param name="inlineCode">The <see cref="InlineCodeDocumentationElement"/> to visit.</param>
        protected internal override void VisitInlineCode(InlineCodeDocumentationElement inlineCode)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("code");

            _WriteXmlAttributes(inlineCode.XmlAttributes);

            _jsonWriter.WritePropertyName("content");
            _jsonWriter.WriteValue(inlineCode.Code);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits an inline parameter reference.</summary>
        /// <param name="parameterReference">The <see cref="ParameterReferenceDocumentationElement"/> to visit.</param>
        protected internal override void VisitParameterReference(ParameterReferenceDocumentationElement parameterReference)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("parameterReference");

            _WriteXmlAttributes(parameterReference.XmlAttributes);

            _jsonWriter.WritePropertyName("parameterName");
            _jsonWriter.WriteValue(parameterReference.ParameterName);

            _jsonWriter.WriteEndObject();
        }

        /// <summary>Visits an inline generic parameter reference.</summary>
        /// <param name="genericParameterReference">The <see cref="GenericParameterReferenceDocumentationElement"/> to visit.</param>
        protected internal override void VisitGenericParameterReference(GenericParameterReferenceDocumentationElement genericParameterReference)
        {
            _jsonWriter.WriteStartObject();

            _jsonWriter.WritePropertyName("kind");
            _jsonWriter.WriteValue("genericParameterReference");

            _WriteXmlAttributes(genericParameterReference.XmlAttributes);

            _jsonWriter.WritePropertyName("parameterName");
            _jsonWriter.WriteValue(genericParameterReference.GenericParameterName);

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