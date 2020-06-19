using CodeMap.DeclarationNodes;
using CodeMap.DocumentationElements;
using HtmlAgilityPack;
using PygmentSharp.Core;
using PygmentSharp.Core.Formatting;
using PygmentSharp.Core.Lexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeMap.Documentation
{
    public class HtmlWriterDocumentationVisitor : DocumentationVisitor
    {
        private readonly Stack<HtmlNode> _htmlNodes;
        private readonly AssemblyDeclaration _library;

        public HtmlWriterDocumentationVisitor(HtmlNode htmlNode, AssemblyDeclaration library)
            => (_htmlNodes, _library) = (new Stack<HtmlNode>(new[] { htmlNode }), library);

        protected override void VisitSummary(SummaryDocumentationElement summary)
        {
            foreach (var element in summary.Content)
                element.Accept(this);
        }

        protected override void VisitRemarks(RemarksDocumentationElement remarks)
        {
            if (remarks.Content.Any())
            {
                _htmlNodes.Peek().AddChild("h3").AppendText("Remarks");
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
                _htmlNodes.Peek().AddChild("h4").AppendText(section);
            else if (paragraph.XmlAttributes.TryGetValue("subsection", out var subsection))
                _htmlNodes.Peek().AddChild("h5").AppendText(subsection);
            else if (paragraph.XmlAttributes.TryGetValue("subsection-example", out var subsectionExample))
                _htmlNodes.Peek().AddChild("h6").AppendText(subsectionExample);

            _htmlNodes.Push(_htmlNodes.Peek().AddChild("p"));
            foreach (var element in paragraph.Content)
                element.Accept(this);
            _htmlNodes.Pop();
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
                _htmlNodes.Peek().AppendChild(_htmlNodes.Peek().OwnerDocument.CreateComment(Pygmentize.Content(codeBlock.Code).WithLexer(lexer).WithFormatter(new HtmlFormatter(new HtmlFormatterOptions())).AsString()));
            else
                _htmlNodes.Peek().AddChild("pre").AddChild("code").AppendText(codeBlock.Code);
        }

        protected override void VisitUnorderedList(UnorderedListDocumentationElement unorderedList)
        {
            _htmlNodes.Push(_htmlNodes.Peek().AddChild("ul"));
            foreach (var item in unorderedList.Items)
                item.Accept(this);
            _htmlNodes.Pop();
        }

        protected override void VisitOrderedList(OrderedListDocumentationElement orderedList)
        {
            _htmlNodes.Push(_htmlNodes.Peek().AddChild("ol"));
            foreach (var item in orderedList.Items)
                item.Accept(this);
            _htmlNodes.Pop();
        }

        protected override void VisitListItem(ListItemDocumentationElement listItem)
        {
            _htmlNodes.Push(_htmlNodes.Peek().AddChild("li"));
            foreach (var element in listItem.Content)
                element.Accept(this);
            _htmlNodes.Pop();
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
            _htmlNodes.Push(_htmlNodes.Peek().AddChild("table").SetClass("table table-hover"));

            _htmlNodes.Push(_htmlNodes.Peek().AddChild("thead").AddChild("tr"));
            foreach (var column in table.Columns)
                column.Accept(this);
            _htmlNodes.Pop();

            _htmlNodes.Push(_htmlNodes.Peek().AddChild("tbody"));
            foreach (var rows in table.Rows)
                rows.Accept(this);
            _htmlNodes.Pop();

            _htmlNodes.Pop();
        }

        protected override void VisitTableColumn(TableColumnDocumentationElement tableColumn)
        {
            _htmlNodes.Push(_htmlNodes.Peek().AddChild("th"));
            foreach (var element in tableColumn.Name)
                element.Accept(this);
            _htmlNodes.Pop();
        }

        protected override void VisitTableRow(TableRowDocumentationElement tableRow)
        {
            _htmlNodes.Push(_htmlNodes.Peek().AddChild("tr"));
            foreach (var cell in tableRow.Cells)
                cell.Accept(this);
            _htmlNodes.Pop();
        }

        protected override void VisitTableCell(TableCellDocumentationElement tableCell)
        {
            _htmlNodes.Push(_htmlNodes.Peek().AddChild("td"));
            foreach (var element in tableCell.Content)
                element.Accept(this);
            _htmlNodes.Pop();
        }

        protected override void VisitText(TextDocumentationElement text)
            => _htmlNodes.Peek().AppendText(text.Text);

        protected override void VisitHyperlink(HyperlinkDocumentationElement hyperlink)
            => _htmlNodes.Peek().AddChild("a").SetAttribute("href", hyperlink.Destination).AppendText(hyperlink.Text);

        protected override void VisitInlineReference(MemberInfoReferenceDocumentationElement memberInfoReference)
            => _htmlNodes.Peek().AddChild("a").SetAttribute("href", _GetMemberLink(memberInfoReference.ReferredMember)).AppendText(memberInfoReference.ReferredMember.GetMemberName());

        private string _GetMemberLink(MemberInfo memberInfo)
            => _library == memberInfo.Module.Assembly ? memberInfo.GetMemberFullName() + ".html" : memberInfo.GetMicrosoftDocsLink();

        protected override void VisitInlineCode(InlineCodeDocumentationElement inlineCode)
            => _htmlNodes.Peek().AddChild("code").AppendText(inlineCode.Code);

        protected override void VisitParameterReference(ParameterReferenceDocumentationElement parameterReference)
            => _htmlNodes.Peek().AddChild("em").AppendText(parameterReference.ParameterName);

        protected override void VisitGenericParameterReference(GenericParameterReferenceDocumentationElement genericParameterReference)
            => _htmlNodes.Peek().AddChild("em").AppendText(genericParameterReference.GenericParameterName);
    }
}