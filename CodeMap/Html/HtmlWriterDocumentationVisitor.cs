using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CodeMap.DocumentationElements;

namespace CodeMap.Html
{
    /// <summary>
    /// A rudimentary HTML generator for <see cref="DocumentationElement"/>s. This is the most basic way of generating
    /// HTML documentation out of a <see cref="DocumentationElement"/> with customisation options.
    /// </summary>
    /// <seealso cref="HtmlWriterDeclarationNodeVisitor"/>
    public class HtmlWriterDocumentationVisitor : DocumentationVisitor
    {
        private int _exceptionCount = 0;
        private int _exampleCount = 0;

        /// <summary>Initializes a new instance of the <see cref="HtmlWriterDocumentationVisitor"/> class.</summary>
        /// <param name="textWriter">The <see cref="TextWriter"/> to which to write the HTML output.</param>
        /// <param name="memberReferenceResolver">The <see cref="IMemberReferenceResolver"/> used to generate URLs for <see cref="ReferenceData.MemberReference"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="textWriter"/> or <paramref name="memberReferenceResolver"/> are <c>null</c>.</exception>
        public HtmlWriterDocumentationVisitor(TextWriter textWriter, IMemberReferenceResolver memberReferenceResolver)
        {
            TextWriter = textWriter ?? throw new ArgumentNullException(nameof(textWriter));
            MemberReferenceResolver = memberReferenceResolver ?? throw new ArgumentNullException(nameof(memberReferenceResolver));
        }

        /// <summary>The <see cref="TextWriter"/> to which the HTML document is being written to.</summary>
        public TextWriter TextWriter { get; }

        /// <summary>The <see cref="IMemberReferenceResolver"/> used to generate URLs for <see cref="ReferenceData.MemberReference"/>s.</summary>
        public IMemberReferenceResolver MemberReferenceResolver { get; }

        /// <summary>Writes a summary section (<c>section</c>) to the <see cref="TextWriter"/>.</summary>
        /// <param name="summary">The <see cref="SummaryDocumentationElement"/> to write from.</param>
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

        /// <summary>Writes a value section (<c>section</c>) to the <see cref="TextWriter"/>.</summary>
        /// <param name="value">The <see cref="ValueDocumentationElement"/> to write from.</param>
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

        /// <summary>Writes an exception section (<c>section</c>) to the <see cref="TextWriter"/>.</summary>
        /// <param name="exception">The <see cref="ExceptionDocumentationElement"/> to write from.</param>
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

        /// <summary>Writes an example section (<c>section</c>) to the <see cref="TextWriter"/>.</summary>
        /// <param name="example">The <see cref="ExampleDocumentationElement"/> to write from.</param>
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

        /// <summary>Writes a remarks section (<c>section</c>) to the <see cref="TextWriter"/>.</summary>
        /// <param name="remarks">The <see cref="RemarksDocumentationElement"/> to write from.</param>
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

        /// <summary>Writes a paragraph (<c>p</c>) to the <see cref="TextWriter"/>.</summary>
        /// <param name="paragraph">The <see cref="ParagraphDocumentationElement"/> to write from.</param>
        protected internal override void VisitParagraph(ParagraphDocumentationElement paragraph)
        {
            WriteElementOpening("p", paragraph.XmlAttributes);
            foreach (var element in paragraph.Content)
                element.Accept(this);
            WriteElementClosing("p");
        }

        /// <summary>Writes a code block (<c>code</c> wrapped by <c>pre</c>) to the <see cref="TextWriter"/>.</summary>
        /// <param name="codeBlock">The <see cref="CodeBlockDocumentationElement"/> to write from.</param>
        protected internal override void VisitCodeBlock(CodeBlockDocumentationElement codeBlock)
        {
            WriteStartElement("pre");
            WriteElementOpening("code", codeBlock.XmlAttributes);
            WriteSafeHtml(codeBlock.Code);
            WriteElementClosing("code");
            WriteElementClosing("pre");
        }

        /// <summary>Writes an anchor (<c>a</c>) to the <see cref="TextWriter"/> for the provided <paramref name="memberInfoReference"/>.</summary>
        /// <param name="memberInfoReference">The <see cref="ReferenceDataDocumentationElement"/> to write from.</param>
        protected internal override void VisitInlineReference(ReferenceDataDocumentationElement memberInfoReference)
        {
            WriteElementOpening("a", new Dictionary<string, string>(memberInfoReference.XmlAttributes) { { "href", MemberReferenceResolver.GetUrl(memberInfoReference.ReferredMember) } });
            if (memberInfoReference.Content.Any())
                foreach (var element in memberInfoReference.Content)
                    element.Accept(this);
            else
                WriteSafeHtml(memberInfoReference.ReferredMember.GetSimpleNameReference());
            WriteElementClosing("a");
        }

        /// <summary>Writes an anchor (<c>a</c>) to the <see cref="TextWriter"/>.</summary>
        /// <param name="hyperlink">The <see cref="HyperlinkDocumentationElement"/> to write from.</param>
        protected internal override void VisitHyperlink(HyperlinkDocumentationElement hyperlink)
        {
            WriteElementOpening("a", new Dictionary<string, string>(hyperlink.XmlAttributes) { { "href", hyperlink.Destination } });
            if (hyperlink.Content.Any())
                foreach (var element in hyperlink.Content)
                    element.Accept(this);
            else
                WriteSafeHtml(hyperlink.Destination);

            WriteElementClosing("a");
        }

        /// <summary>Writes an inline code snippet (<c>code</c>) to the <see cref="TextWriter"/>.</summary>
        /// <param name="inlineCode">The <see cref="InlineCodeDocumentationElement"/> to write from.</param>
        protected internal override void VisitInlineCode(InlineCodeDocumentationElement inlineCode)
        {
            WriteElementOpening("code", inlineCode.XmlAttributes);
            WriteSafeHtml(inlineCode.Code);
            WriteElementClosing("code");
        }

        /// <summary>Writes a generic parameter reference (parameter name wrapped in <c>code</c> tags) to the <see cref="TextWriter"/>.</summary>
        /// <param name="genericParameterReference">The <see cref="GenericParameterReferenceDocumentationElement"/> to write from.</param>
        protected internal override void VisitGenericParameterReference(GenericParameterReferenceDocumentationElement genericParameterReference)
        {
            WriteElementOpening("pre", genericParameterReference.XmlAttributes);
            WriteSafeHtml(genericParameterReference.GenericParameterName);
            WriteElementClosing("pre");
        }

        /// <summary>Writes a parameter reference (parameter name wrapped in <c>code</c> tags) to the <see cref="TextWriter"/>.</summary>
        /// <param name="parameterReference">The <see cref="ParameterReferenceDocumentationElement"/> to write from.</param>
        protected internal override void VisitParameterReference(ParameterReferenceDocumentationElement parameterReference)
        {
            WriteElementOpening("code", parameterReference.XmlAttributes);
            WriteSafeHtml(parameterReference.ParameterName);
            WriteElementClosing("code");
        }

        /// <summary>Writes an unordered list (<c>ul</c>) to the <see cref="TextWriter"/>.</summary>
        /// <param name="unorderedList">The <see cref="UnorderedListDocumentationElement"/> to write from.</param>
        protected internal override void VisitUnorderedList(UnorderedListDocumentationElement unorderedList)
        {
            WriteElementOpening("ul", unorderedList.XmlAttributes);
            foreach (var item in unorderedList.Items)
                item.Accept(this);
            WriteElementClosing("ul");
        }

        /// <summary>Writes an ordered list (<c>ol</c>) to the <see cref="TextWriter"/>.</summary>
        /// <param name="orderedList">The <see cref="OrderedListDocumentationElement"/> to write from.</param>
        protected internal override void VisitOrderedList(OrderedListDocumentationElement orderedList)
        {
            WriteElementOpening("ol", orderedList.XmlAttributes);
            foreach (var item in orderedList.Items)
                item.Accept(this);
            WriteElementClosing("ol");
        }

        /// <summary>Writes a list item (<c>li</c>) to the <see cref="TextWriter"/>.</summary>
        /// <param name="listItem">The <see cref="ListItemDocumentationElement"/> to write from.</param>
        protected internal override void VisitListItem(ListItemDocumentationElement listItem)
        {
            WriteElementOpening("li", listItem.XmlAttributes);
            foreach (var element in listItem.Content)
                element.Accept(this);
            WriteElementClosing("li");
        }

        /// <summary>Writes a definition list (<c>dl</c>) to the <see cref="TextWriter"/>.</summary>
        /// <param name="definitionList">The <see cref="DefinitionListDocumentationElement"/> to write from.</param>
        protected internal override void VisitDefinitionList(DefinitionListDocumentationElement definitionList)
        {
            definitionList.ListTitle.Accept(this);
            WriteElementOpening("dl", definitionList.XmlAttributes);
            foreach (var item in definitionList.Items)
                item.Accept(this);
            WriteElementClosing("dl");
        }

        /// <summary>Writes a definition list header (<c>h3</c>) to the <see cref="TextWriter"/>.</summary>
        /// <param name="definitionListTitle">The <see cref="DefinitionListTitleDocumentationElement"/> to write from.</param>
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

        /// <summary>Writes a definition list item (<c>dt</c> and <c>dd</c>) to the <see cref="TextWriter"/>.</summary>
        /// <param name="definitionListItem">The <see cref="DefinitionListItemDocumentationElement"/> to write from.</param>
        protected internal override void VisitDefinitionListItem(DefinitionListItemDocumentationElement definitionListItem)
        {
            definitionListItem.Term.Accept(this);
            definitionListItem.Description.Accept(this);
        }

        /// <summary>Writes a definition list item term (<c>dt</c>) to the <see cref="TextWriter"/>.</summary>
        /// <param name="definitionListItemTerm">The <see cref="DefinitionListItemTermDocumentationElement"/> to write from.</param>
        protected internal override void VisitDefinitionListItemTerm(DefinitionListItemTermDocumentationElement definitionListItemTerm)
        {
            WriteElementOpening("dt", definitionListItemTerm.XmlAttributes);
            foreach (var element in definitionListItemTerm.Content)
                element.Accept(this);
            WriteElementClosing("dt");
        }

        /// <summary>Writes a definition list item description (<c>dd</c>) to the <see cref="TextWriter"/>.</summary>
        /// <param name="definitionListItemDescription">The <see cref="DefinitionListItemDescriptionDocumentationElement"/> to write from.</param>
        protected internal override void VisitDefinitionListItemDescription(DefinitionListItemDescriptionDocumentationElement definitionListItemDescription)
        {
            WriteElementOpening("dd", definitionListItemDescription.XmlAttributes);
            foreach (var element in definitionListItemDescription.Content)
                element.Accept(this);
            WriteElementClosing("dd");
        }

        /// <summary>Writes a table (<c>table</c>) to the <see cref="TextWriter"/>.</summary>
        /// <param name="table">The <see cref="TableDocumentationElement"/> to write from.</param>
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

        /// <summary>Writes a table header cell (<c>th</c>) to the <see cref="TextWriter"/>.</summary>
        /// <param name="tableColumn">The <see cref="TableColumnDocumentationElement"/> to write from.</param>
        protected internal override void VisitTableColumn(TableColumnDocumentationElement tableColumn)
        {
            WriteElementOpening("th", tableColumn.XmlAttributes);
            foreach (var element in tableColumn.Name)
                element.Accept(this);
            WriteElementClosing("th");
        }

        /// <summary>Writes a table row (<c>tr</c>) to the <see cref="TextWriter"/>.</summary>
        /// <param name="tableRow">The <see cref="TableRowDocumentationElement"/> to write from.</param>
        protected internal override void VisitTableRow(TableRowDocumentationElement tableRow)
        {
            WriteElementOpening("tr", tableRow.XmlAttributes);
            foreach (var cell in tableRow.Cells)
                cell.Accept(this);
            WriteElementClosing("tr");
        }

        /// <summary>Writes a table cell (<c>td</c>) to the <see cref="TextWriter"/>.</summary>
        /// <param name="tableCell">The <see cref="TableCellDocumentationElement"/> to write from.</param>
        protected internal override void VisitTableCell(TableCellDocumentationElement tableCell)
        {
            WriteElementOpening("td", tableCell.XmlAttributes);
            foreach (var element in tableCell.Content)
                element.Accept(this);
            WriteElementClosing("td");
        }

        /// <summary>Safely writes the text content to the <see cref="TextWriter"/>.</summary>
        /// <param name="text">The <see cref="TextDocumentationElement"/> to write from.</param>
        protected internal override void VisitText(TextDocumentationElement text)
            => WriteSafeHtml(text.Text);

        /// <summary>Writes an opening HTML element.</summary>
        /// <param name="name">The element name, such as <c>p</c> or <c>h1</c>.</param>
        /// <seealso cref="WriteElementOpening(string, IReadOnlyDictionary{string, string})"/>
        protected void WriteStartElement(string name)
            => WriteElementOpening(name, null);

        /// <summary>Writes an opening HTML element.</summary>
        /// <param name="name">The element name, such as <c>p</c> or <c>h1</c>.</param>
        /// <param name="attributes">A set of attributes to set on the element.</param>
        /// <seealso cref="WriteElementOpening(string, IReadOnlyDictionary{string, string})"/>
        protected void WriteElementOpening(string name, IReadOnlyDictionary<string, string> attributes)
        {
            TextWriter.Write($"<{name}");
            if (attributes != null)
                foreach (var xmlAttribute in attributes)
                    TextWriter.Write($" {xmlAttribute.Key}=\"{xmlAttribute.Value}\"");
            TextWriter.Write(">");
        }

        /// <summary>Writes a closing HTML element.</summary>
        /// <param name="name">The element name, such as <c>p</c> or <c>h1</c>.</param>
        /// <seealso cref="WriteElementOpening(string, IReadOnlyDictionary{string, string})"/>
        protected void WriteElementClosing(string name)
            => TextWriter.Write($"</{name}>");


        /// <summary>Writes the provided <paramref name="value"/> as a safe HTML string.</summary>
        /// <param name="value">The text to write.</param>
        /// <remarks>
        /// If the provied <paramref name="value"/> contains HTML reserved characters, they
        /// are escaped.
        /// </remarks>
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