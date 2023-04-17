using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CodeMap.DocumentationElements;
using CodeMap.Html;
using CodeMap.ReferenceData;

namespace CodeMap.Documentation
{
    public class HtmlWriterDocumentationVisitor : DocumentationVisitor
    {
        private int _exceptionCount = 0;
        private int _exampleCount = 0;

        public HtmlWriterDocumentationVisitor(TextWriter textWriter, IMemberReferenceResolver memberReferenceResolver)
        {
            TextWriter = textWriter ?? throw new ArgumentNullException(nameof(textWriter));
            MemberReferenceResolver = memberReferenceResolver ?? throw new ArgumentNullException(nameof(memberReferenceResolver));
        }

        public TextWriter TextWriter { get; }

        public IMemberReferenceResolver MemberReferenceResolver { get; }

        protected override void VisitSummary(SummaryDocumentationElement summary)
        {
            if (summary.Content.Any())
            {
                WriteElementOpening("section", new Dictionary<string, string>(summary.XmlAttributes) { { "id", "summary" } });
                foreach (var element in summary.Content)
                    element.Accept(this);
                WriteElementClosing("section");
            }
        }

        protected override void VisitValue(ValueDocumentationElement value)
        {
            if (value.Content.Any())
            {
                WriteElementOpening("section", new Dictionary<string, string>(value.XmlAttributes) { { "id", "value" } });

                WriteStartElement("h2");
                WriteSafeHtml("Value");
                WriteElementClosing("h2");
                foreach (var element in value.Content)
                    element.Accept(this);

                WriteElementClosing("section");
            }
        }

        protected override void VisitException(ExceptionDocumentationElement exception)
        {
            _exceptionCount++;
            WriteElementOpening("section", new Dictionary<string, string>(exception.XmlAttributes) { { "id", $"exception-{_exceptionCount}" } });

            WriteStartElement("h2");
            WriteSafeHtml("Exception: ");
            exception.Exception.Accept(this);
            WriteElementClosing("h2");

            foreach (var element in exception.Description)
                element.Accept(this);

            WriteElementClosing("section");
        }

        protected override void VisitExample(ExampleDocumentationElement example)
        {
            if (example.Content.Any())
            {
                _exampleCount++;
                WriteElementOpening("section", new Dictionary<string, string>(example.XmlAttributes) { { "id", $"example-{_exampleCount}" } });

                WriteStartElement("h2");
                WriteSafeHtml("Example");
                WriteElementClosing("h2");
                foreach (var element in example.Content)
                    element.Accept(this);

                WriteElementClosing("section");
            }
        }

        protected override void VisitRemarks(RemarksDocumentationElement remarks)
        {
            if (remarks.Content.Any())
            {
                WriteElementOpening("section", new Dictionary<string, string>(remarks.XmlAttributes) { { "id", "remarks" } });

                WriteStartElement("h2");
                WriteSafeHtml("Remarks");
                WriteElementClosing("h2");
                foreach (var element in remarks.Content)
                    element.Accept(this);

                WriteElementClosing("section");
            }
        }

        protected override void VisitParagraph(ParagraphDocumentationElement paragraph)
        {
            WriteElementOpening("p", paragraph.XmlAttributes);
            foreach (var element in paragraph.Content)
                element.Accept(this);
            WriteElementClosing("p");
        }

        protected override void VisitCodeBlock(CodeBlockDocumentationElement codeBlock)
        {
            WriteElementOpening("code", codeBlock.XmlAttributes);
            WriteStartElement("pre");
            WriteSafeHtml(codeBlock.Code);
            WriteElementClosing("pre");
            WriteElementClosing("code");
        }

        protected override void VisitInlineReference(ReferenceDataDocumentationElement memberInfoReference)
        {
            WriteElementOpening("a", new Dictionary<string, string>(memberInfoReference.XmlAttributes) { { "href", MemberReferenceResolver.GetUrl(memberInfoReference.ReferredMember) } });
            WriteSafeHtml(memberInfoReference.ReferredMember.GetSimpleNameReference());
            WriteElementClosing("a");
        }

        protected override void VisitHyperlink(HyperlinkDocumentationElement hyperlink)
        {
            WriteElementOpening("a", new Dictionary<string, string>(hyperlink.XmlAttributes) { { "href", hyperlink.Destination } });
            WriteSafeHtml(hyperlink.Text);
            WriteElementClosing("a");
        }

        protected override void VisitInlineCode(InlineCodeDocumentationElement inlineCode)
        {
            WriteElementOpening("code", inlineCode.XmlAttributes);
            WriteSafeHtml(inlineCode.Code);
            WriteElementClosing("code");
        }

        protected override void VisitGenericParameterReference(GenericParameterReferenceDocumentationElement genericParameterReference)
        {
            WriteElementOpening("pre", genericParameterReference.XmlAttributes);
            WriteSafeHtml(genericParameterReference.GenericParameterName);
            WriteElementClosing("pre");
        }

        protected override void VisitParameterReference(ParameterReferenceDocumentationElement parameterReference)
        {
            WriteElementOpening("code", parameterReference.XmlAttributes);
            WriteSafeHtml(parameterReference.ParameterName);
            WriteElementClosing("code");
        }

        protected override void VisitUnorderedList(UnorderedListDocumentationElement unorderedList)
        {
            WriteElementOpening("ul", unorderedList.XmlAttributes);
            foreach (var item in unorderedList.Items)
                item.Accept(this);
            WriteElementClosing("ul");
        }

        protected override void VisitOrderedList(OrderedListDocumentationElement orderedList)
        {
            WriteElementOpening("ol", orderedList.XmlAttributes);
            foreach (var item in orderedList.Items)
                item.Accept(this);
            WriteElementClosing("ol");
        }

        protected override void VisitListItem(ListItemDocumentationElement listItem)
        {
            WriteElementOpening("li", listItem.XmlAttributes);
            foreach (var element in listItem.Content)
                element.Accept(this);
            WriteElementClosing("li");
        }

        protected override void VisitDefinitionList(DefinitionListDocumentationElement definitionList)
        {
            definitionList.ListTitle.Accept(this);
            WriteElementOpening("dl", definitionList.XmlAttributes);
            foreach (var item in definitionList.Items)
                item.Accept(this);
            WriteElementClosing("dl");
        }

        protected override void VisitDefinitionListTitle(DefinitionListTitleDocumentationElement definitionListTitle)
        {
            if (definitionListTitle.Content.Any())
            {
                WriteElementOpening("h3", definitionListTitle.XmlAttributes);
                foreach (var element in definitionListTitle.Content)
                    element.Accept(this);
                WriteElementClosing("h3");
            }
        }

        protected override void VisitDefinitionListItem(DefinitionListItemDocumentationElement definitionListItem)
        {
            definitionListItem.Term.Accept(this);
            definitionListItem.Description.Accept(this);
        }

        protected override void VisitDefinitionListItemTerm(DefinitionListItemTermDocumentationElement definitionListItemTerm)
        {
            WriteElementOpening("dt", definitionListItemTerm.XmlAttributes);
            foreach (var element in definitionListItemTerm.Content)
                element.Accept(this);
            WriteElementClosing("dt");
        }

        protected override void VisitDefinitionListItemDescription(DefinitionListItemDescriptionDocumentationElement definitionListItemDescription)
        {
            WriteElementOpening("dd", definitionListItemDescription.XmlAttributes);
            foreach (var element in definitionListItemDescription.Content)
                element.Accept(this);
            WriteElementClosing("dd");
        }

        protected override void VisitTable(TableDocumentationElement table)
        {
            WriteElementOpening("table", table.XmlAttributes);
            WriteStartElement("thead");
            WriteStartElement("tr");
            foreach (var column in table.Columns)
                column.Accept(this);
            WriteElementClosing("tr");
            WriteElementClosing("thead");
            WriteElementClosing("table");
        }


        protected override void VisitTableColumn(TableColumnDocumentationElement tableColumn)
        {
            WriteElementOpening("th", tableColumn.XmlAttributes);
            foreach (var element in tableColumn.Name)
                element.Accept(this);
            WriteElementClosing("th");
        }

        protected override void VisitTableRow(TableRowDocumentationElement tableRow)
        {
            WriteElementOpening("tr", tableRow.XmlAttributes);
            foreach (var cell in tableRow.Cells)
                cell.Accept(this);
            WriteElementClosing("tr");
        }
        protected override void VisitTableCell(TableCellDocumentationElement tableCell)
        {
            WriteElementOpening("tr", tableCell.XmlAttributes);
            foreach (var element in tableCell.Content)
                element.Accept(this);
            WriteElementClosing("tr");
        }

        protected override void VisitText(TextDocumentationElement text)
            => WriteSafeHtml(text.Text);

        protected void WriteStartElement(string name)
            => WriteElementOpening(name, null);

        protected void WriteElementOpening(string name, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            TextWriter.Write($"<{name}");
            if (xmlAttributes is not null)
                foreach (var xmlAttribute in xmlAttributes)
                    TextWriter.Write($" {xmlAttribute.Key}=\"{xmlAttribute.Value}\"");
            TextWriter.Write(">");
        }

        protected void WriteElementClosing(string name)
            => TextWriter.Write($"</{name}>");

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