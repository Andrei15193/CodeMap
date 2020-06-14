using CodeMap.DeclarationNodes;
using CodeMap.DocumentationElements;
using PygmentSharp.Core;
using PygmentSharp.Core.Formatting;
using PygmentSharp.Core.Lexing;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CodeMap.Documentation
{
    public class HtmlWriterDocumentationVisitor : DocumentationVisitor
    {
        private readonly TextWriter _textWriter;
        private readonly AssemblyDeclaration _library;

        public HtmlWriterDocumentationVisitor(TextWriter textWriter, AssemblyDeclaration library)
            => (_textWriter, _library) = (textWriter, library);

        protected override void VisitSummary(SummaryDocumentationElement summary)
        {
            foreach (var element in summary.Content)
                element.Accept(this);
        }

        protected override void VisitRemarks(RemarksDocumentationElement remarks)
        {
            if (remarks.Content.Any())
            {
                _textWriter.Write("<h2>Remarks</h2>");
                foreach (var element in remarks.Content)
                    element.Accept(this);
            }
        }

        protected override void VisitExample(ExampleDocumentationElement example)
        {
            throw new NotImplementedException();
        }

        protected override void VisitValue(ValueDocumentationElement value)
        {
            throw new NotImplementedException();
        }

        protected override void VisitParagraph(ParagraphDocumentationElement paragraph)
        {
            if (paragraph.XmlAttributes.TryGetValue("section", out var section))
            {
                _textWriter.Write("<h3>");
                _textWriter.Write(section.HtmlEncode());
                _textWriter.Write("</h3>");
            }
            else if (paragraph.XmlAttributes.TryGetValue("subsection", out var subsection))
            {
                _textWriter.Write("<h4>");
                _textWriter.Write(subsection.HtmlEncode());
                _textWriter.Write("</h4>");
            }
            else if (paragraph.XmlAttributes.TryGetValue("subsection-example", out var subsectionExample))
            {
                _textWriter.Write("<h5>");
                _textWriter.Write(subsectionExample.HtmlEncode());
                _textWriter.Write("</h5>");
            }

            _textWriter.Write("<p>");
            foreach (var element in paragraph.Content)
                element.Accept(this);
            _textWriter.Write("</p>");
        }

        protected override void VisitCodeBlock(CodeBlockDocumentationElement codeBlock)
        {
            Lexer lexer = null;
            if (codeBlock.XmlAttributes.TryGetValue("language", out var language))
                switch (language.ToLowerInvariant())
                {
                    case "c#":
                        lexer = new CSharpLexer();
                        break;

                    case "xml":
                        lexer = new HtmlLexer();
                        break;
                }
            if (lexer != null)
                _textWriter.Write(Pygmentize.Content(codeBlock.Code).WithLexer(lexer).WithFormatter(new HtmlFormatter(new HtmlFormatterOptions())).AsString());
            else
                _textWriter.Write($"<pre><code>{codeBlock.Code.HtmlEncode()}</code></pre>");
        }

        protected override void VisitUnorderedList(UnorderedListDocumentationElement unorderedList)
        {
            _textWriter.Write("<ul>");
            foreach (var item in unorderedList.Items)
                item.Accept(this);
            _textWriter.Write("</ul>");
        }

        protected override void VisitOrderedList(OrderedListDocumentationElement orderedList)
        {
            _textWriter.Write("<ol>");
            foreach (var item in orderedList.Items)
                item.Accept(this);
            _textWriter.Write("</ol>");
        }

        protected override void VisitListItem(ListItemDocumentationElement listItem)
        {
            _textWriter.Write("<li>");
            foreach (var element in listItem.Content)
                element.Accept(this);
            _textWriter.Write("</li>");
        }

        protected override void VisitDefinitionList(DefinitionListDocumentationElement definitionList)
        {
            throw new NotImplementedException();
        }

        protected override void VisitDefinitionListTitle(DefinitionListTitleDocumentationElement definitionListTitle)
        {
            throw new NotImplementedException();
        }

        protected override void VisitDefinitionListItem(DefinitionListItemDocumentationElement definitionListItem)
        {
            throw new NotImplementedException();
        }

        protected override void VisitDefinitionListItemTerm(DefinitionListItemTermDocumentationElement definitionListItemTerm)
        {
            throw new NotImplementedException();
        }

        protected override void VisitDefinitionListItemDescription(DefinitionListItemDescriptionDocumentationElement definitionListItemDescription)
        {
            throw new NotImplementedException();
        }

        protected override void VisitTable(TableDocumentationElement table)
        {
            _textWriter.Write("<table class=\"table table-hover\">");
            _textWriter.Write("<thead>");
            _textWriter.Write("<tr>");
            foreach (var column in table.Columns)
                column.Accept(this);
            _textWriter.Write("</tr>");
            _textWriter.Write("</thead>");

            _textWriter.Write("<tbody>");
            foreach (var rows in table.Rows)
                rows.Accept(this);
            _textWriter.Write("</tbody>");
            _textWriter.Write("</table>");
        }

        protected override void VisitTableColumn(TableColumnDocumentationElement tableColumn)
        {
            _textWriter.Write("<th>");
            foreach (var element in tableColumn.Name)
                element.Accept(this);
            _textWriter.Write("</th>");
        }

        protected override void VisitTableRow(TableRowDocumentationElement tableRow)
        {
            _textWriter.Write("<tr>");
            foreach (var cell in tableRow.Cells)
                cell.Accept(this);
            _textWriter.Write("</tr>");
        }

        protected override void VisitTableCell(TableCellDocumentationElement tableCell)
        {
            _textWriter.Write("<td>");
            foreach (var element in tableCell.Content)
                element.Accept(this);
            _textWriter.Write("</td>");
        }

        protected override void VisitText(TextDocumentationElement text)
            => _textWriter.Write(text.Text.HtmlEncode());

        protected override void VisitHyperlink(HyperlinkDocumentationElement hyperlink)
        {
            _textWriter.Write("<a href=\"");
            _textWriter.Write(Uri.EscapeUriString(hyperlink.Destination));
            _textWriter.Write("\">");
            _textWriter.Write(hyperlink.Text.HtmlEncode());
            _textWriter.Write("</a>");
        }

        protected override void VisitInlineReference(MemberInfoReferenceDocumentationElement memberInfoReference)
            => _textWriter.Write($"<a href=\"{Uri.EscapeUriString(_GetMemberLink(memberInfoReference.ReferredMember))}\">{memberInfoReference.ReferredMember.GetMemberName().HtmlEncode()}</a>");

        private string _GetMemberLink(MemberInfo memberInfo)
            => _library == memberInfo.Module.Assembly ? memberInfo.GetMemberFullName() + ".html" : memberInfo.GetMicrosoftDocsLink();

        protected override void VisitInlineCode(InlineCodeDocumentationElement inlineCode)
            => _textWriter.Write($"<code>{inlineCode.Code.HtmlEncode()}</code>");

        protected override void VisitParameterReference(ParameterReferenceDocumentationElement parameterReference)
            => _textWriter.Write($"<em>{parameterReference.ParameterName.HtmlEncode()}</em>");

        protected override void VisitGenericParameterReference(GenericParameterReferenceDocumentationElement genericParameterReference)
            => _textWriter.Write($"<em>{genericParameterReference.GenericParameterName.HtmlEncode()}</em>");
    }
}