using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CodeMap.DocumentationElements;

namespace CodeMap.Html
{
    /// <summary/>
    public class HtmlWriterDocumentationVisitor : DocumentationVisitor
    {
        private int _exceptionCount = 0;
        private int _exampleCount = 0;

        /// <summary/>
        public HtmlWriterDocumentationVisitor(TextWriter textWriter, IMemberReferenceResolver memberReferenceResolver)
        {
            TextWriter = textWriter ?? throw new ArgumentNullException(nameof(textWriter));
            MemberReferenceResolver = memberReferenceResolver ?? throw new ArgumentNullException(nameof(memberReferenceResolver));
        }

        /// <summary/>
        public TextWriter TextWriter { get; }

        /// <summary/>
        public IMemberReferenceResolver MemberReferenceResolver { get; }

        /// <summary/>
        protected internal override void VisitSummary(SummaryDocumentationElement summary)
        {
            if (summary.Content.Any())
            {
                WriteElementOpening("section", new Dictionary<string, string>(summary.XmlAttributes) { { "data-sectionId", "summary" } });
                foreach (var element in summary.Content)
                    element.Accept(this);
                WriteElementClosing("section");
            }
        }

        /// <summary/>
        protected internal override void VisitValue(ValueDocumentationElement value)
        {
            if (value.Content.Any())
            {
                WriteElementOpening("section", new Dictionary<string, string>(value.XmlAttributes) { { "data-sectionId", "value" } });

                WriteStartElement("h2");
                WriteSafeHtml("Value");
                WriteElementClosing("h2");
                foreach (var element in value.Content)
                    element.Accept(this);

                WriteElementClosing("section");
            }
        }

        /// <summary/>
        protected internal override void VisitException(ExceptionDocumentationElement exception)
        {
            _exceptionCount++;
            WriteElementOpening("section", new Dictionary<string, string>(exception.XmlAttributes) { { "data-sectionId", $"exception-{_exceptionCount}" } });

            WriteStartElement("h2");
            WriteSafeHtml("Exception: ");
            exception.Exception.Accept(this);
            WriteElementClosing("h2");

            foreach (var element in exception.Description)
                element.Accept(this);

            WriteElementClosing("section");
        }

        /// <summary/>
        protected internal override void VisitExample(ExampleDocumentationElement example)
        {
            if (example.Content.Any())
            {
                _exampleCount++;
                WriteElementOpening("section", new Dictionary<string, string>(example.XmlAttributes) { { "data-sectionId", $"example-{_exampleCount}" } });

                WriteStartElement("h2");
                WriteSafeHtml("Example");
                WriteElementClosing("h2");
                foreach (var element in example.Content)
                    element.Accept(this);

                WriteElementClosing("section");
            }
        }

        /// <summary/>
        protected internal override void VisitRemarks(RemarksDocumentationElement remarks)
        {
            if (remarks.Content.Any())
            {
                WriteElementOpening("section", new Dictionary<string, string>(remarks.XmlAttributes) { { "data-sectionId", "remarks" } });

                WriteStartElement("h2");
                WriteSafeHtml("Remarks");
                WriteElementClosing("h2");
                foreach (var element in remarks.Content)
                    element.Accept(this);

                WriteElementClosing("section");
            }
        }

        /// <summary/>
        protected internal override void VisitParagraph(ParagraphDocumentationElement paragraph)
        {
            WriteElementOpening("p", paragraph.XmlAttributes);
            foreach (var element in paragraph.Content)
                element.Accept(this);
            WriteElementClosing("p");
        }

        /// <summary/>
        protected internal override void VisitCodeBlock(CodeBlockDocumentationElement codeBlock)
        {
            WriteStartElement("pre");
            WriteElementOpening("code", codeBlock.XmlAttributes);
            WriteSafeHtml(codeBlock.Code);
            WriteElementClosing("code");
            WriteElementClosing("pre");
        }

        /// <summary/>
        protected internal override void VisitInlineReference(ReferenceDataDocumentationElement memberInfoReference)
        {
            WriteElementOpening("a", new Dictionary<string, string>(memberInfoReference.XmlAttributes) { { "href", MemberReferenceResolver.GetUrl(memberInfoReference.ReferredMember) } });
            WriteSafeHtml(memberInfoReference.ReferredMember.GetSimpleNameReference());
            WriteElementClosing("a");
        }

        /// <summary/>
        protected internal override void VisitHyperlink(HyperlinkDocumentationElement hyperlink)
        {
            WriteElementOpening("a", new Dictionary<string, string>(hyperlink.XmlAttributes) { { "href", hyperlink.Destination } });
            WriteSafeHtml(hyperlink.Text);
            WriteElementClosing("a");
        }

        /// <summary/>
        protected internal override void VisitInlineCode(InlineCodeDocumentationElement inlineCode)
        {
            WriteElementOpening("code", inlineCode.XmlAttributes);
            WriteSafeHtml(inlineCode.Code);
            WriteElementClosing("code");
        }

        /// <summary/>
        protected internal override void VisitGenericParameterReference(GenericParameterReferenceDocumentationElement genericParameterReference)
        {
            WriteElementOpening("pre", genericParameterReference.XmlAttributes);
            WriteSafeHtml(genericParameterReference.GenericParameterName);
            WriteElementClosing("pre");
        }

        /// <summary/>
        protected internal override void VisitParameterReference(ParameterReferenceDocumentationElement parameterReference)
        {
            WriteElementOpening("code", parameterReference.XmlAttributes);
            WriteSafeHtml(parameterReference.ParameterName);
            WriteElementClosing("code");
        }

        /// <summary/>
        protected internal override void VisitUnorderedList(UnorderedListDocumentationElement unorderedList)
        {
            WriteElementOpening("ul", unorderedList.XmlAttributes);
            foreach (var item in unorderedList.Items)
                item.Accept(this);
            WriteElementClosing("ul");
        }

        /// <summary/>
        protected internal override void VisitOrderedList(OrderedListDocumentationElement orderedList)
        {
            WriteElementOpening("ol", orderedList.XmlAttributes);
            foreach (var item in orderedList.Items)
                item.Accept(this);
            WriteElementClosing("ol");
        }

        /// <summary/>
        protected internal override void VisitListItem(ListItemDocumentationElement listItem)
        {
            WriteElementOpening("li", listItem.XmlAttributes);
            foreach (var element in listItem.Content)
                element.Accept(this);
            WriteElementClosing("li");
        }

        /// <summary/>
        protected internal override void VisitDefinitionList(DefinitionListDocumentationElement definitionList)
        {
            definitionList.ListTitle.Accept(this);
            WriteElementOpening("dl", definitionList.XmlAttributes);
            foreach (var item in definitionList.Items)
                item.Accept(this);
            WriteElementClosing("dl");
        }

        /// <summary/>
        protected internal override void VisitDefinitionListTitle(DefinitionListTitleDocumentationElement definitionListTitle)
        {
            if (definitionListTitle.Content.Any())
            {
                WriteElementOpening("h3", definitionListTitle.XmlAttributes);
                foreach (var element in definitionListTitle.Content)
                    element.Accept(this);
                WriteElementClosing("h3");
            }
        }

        /// <summary/>
        protected internal override void VisitDefinitionListItem(DefinitionListItemDocumentationElement definitionListItem)
        {
            definitionListItem.Term.Accept(this);
            definitionListItem.Description.Accept(this);
        }

        /// <summary/>
        protected internal override void VisitDefinitionListItemTerm(DefinitionListItemTermDocumentationElement definitionListItemTerm)
        {
            WriteElementOpening("dt", definitionListItemTerm.XmlAttributes);
            foreach (var element in definitionListItemTerm.Content)
                element.Accept(this);
            WriteElementClosing("dt");
        }

        /// <summary/>
        protected internal override void VisitDefinitionListItemDescription(DefinitionListItemDescriptionDocumentationElement definitionListItemDescription)
        {
            WriteElementOpening("dd", definitionListItemDescription.XmlAttributes);
            foreach (var element in definitionListItemDescription.Content)
                element.Accept(this);
            WriteElementClosing("dd");
        }

        /// <summary/>
        protected internal override void VisitTable(TableDocumentationElement table)
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

        /// <summary/>
        protected internal override void VisitTableColumn(TableColumnDocumentationElement tableColumn)
        {
            WriteElementOpening("th", tableColumn.XmlAttributes);
            foreach (var element in tableColumn.Name)
                element.Accept(this);
            WriteElementClosing("th");
        }

        /// <summary/>
        protected internal override void VisitTableRow(TableRowDocumentationElement tableRow)
        {
            WriteElementOpening("tr", tableRow.XmlAttributes);
            foreach (var cell in tableRow.Cells)
                cell.Accept(this);
            WriteElementClosing("tr");
        }

        /// <summary/>
        protected internal override void VisitTableCell(TableCellDocumentationElement tableCell)
        {
            WriteElementOpening("tr", tableCell.XmlAttributes);
            foreach (var element in tableCell.Content)
                element.Accept(this);
            WriteElementClosing("tr");
        }

        /// <summary/>
        protected internal override void VisitText(TextDocumentationElement text)
            => WriteSafeHtml(text.Text);

        /// <summary/>
        protected void WriteStartElement(string name)
            => WriteElementOpening(name, null);

        /// <summary/>
        protected void WriteElementOpening(string name, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            TextWriter.Write($"<{name}");
            if (xmlAttributes != null)
                foreach (var xmlAttribute in xmlAttributes)
                    TextWriter.Write($" {xmlAttribute.Key}=\"{xmlAttribute.Value}\"");
            TextWriter.Write(">");
        }

        /// <summary/>
        protected void WriteElementClosing(string name)
            => TextWriter.Write($"</{name}>");

        /// <summary/>
        protected void WriteSafeHtml(string value)
        {
            var htmlSafeValue = value;
            if (value.Any(@char => @char == '<' || @char == '>' || @char == '&' || @char == '\'' || @char == '"' || (char.IsControl(@char) && !char.IsWhiteSpace(@char))))
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
                                    return stringBuilder.Append("&quot;");

                                default:
                                    if (@char == '\'' || (char.IsControl(@char) && !char.IsWhiteSpace(@char)))
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