using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeMap.DocumentationElements;
using CodeMap.Html;
using PygmentSharp.Core;
using PygmentSharp.Core.Formatting;
using PygmentSharp.Core.Lexing;

namespace CodeMap.Documentation
{
    public class CodeMapHtmlWriterDocumentationVisitor : HtmlWriterDocumentationVisitor
    {
        private static readonly IReadOnlyDictionary<string, Func<Lexer>> _lexers = new Dictionary<string, Func<Lexer>>(StringComparer.OrdinalIgnoreCase)
        {
            { "c#", () => new CSharpLexer() },
            { "csharp", () => new CSharpLexer() },
            { "html", () => new HtmlLexer() },
            { "xml", () => new XmlLexer() },
            { "xhtml", () => new XmlLexer() },
            { "bash", () => new PygmentSharp.Core.Lexing.BashLexer() },
            { "ps", () => new PygmentSharp.Core.Lexing.BashLexer() },
            { "pwsh", () => new PygmentSharp.Core.Lexing.BashLexer() },
            { "powershell", () => new PygmentSharp.Core.Lexing.BashLexer() },
            { "css", () => new PygmentSharp.Core.Lexing.CssLexer() },
            { "js", () => new PygmentSharp.Core.Lexing.JavascriptLexer() },
            { "javascript", () => new PygmentSharp.Core.Lexing.JavascriptLexer() },
            { "sql", () => new PygmentSharp.Core.Lexing.SqlLexer() }
        };

        public CodeMapHtmlWriterDocumentationVisitor(TextWriter textWriter, IMemberReferenceResolver memberReferenceResolver)
            : base(textWriter, memberReferenceResolver)
        {
        }

        protected override void VisitParagraph(ParagraphDocumentationElement paragraph)
        {
            if (paragraph.XmlAttributes.TryGetValue("section", out var sectionTitle))
            {
                TextWriter.Write("<h3>");
                WriteSafeHtml(sectionTitle);
                TextWriter.Write("</h3>");
            }
            if (paragraph.XmlAttributes.TryGetValue("subsection", out var subsectionTitle))
            {
                TextWriter.Write("<h4>");
                WriteSafeHtml(subsectionTitle);
                TextWriter.Write("</h4>");
            }
            if (paragraph.XmlAttributes.TryGetValue("subsectionExample", out var subsectionExampleTitle))
            {
                TextWriter.Write("<h5>");
                WriteSafeHtml(subsectionExampleTitle);
                TextWriter.Write("</h5>");
            }

            base.VisitParagraph(DocumentationElement.Paragraph(
                paragraph.Content,
                new Dictionary<string, string>(paragraph.XmlAttributes.Where(xmlAttribute => xmlAttribute.Key != "section" && xmlAttribute.Key != "subsection" && xmlAttribute.Key != "subsectionExample")))
            );
        }

        protected override void VisitCodeBlock(CodeBlockDocumentationElement codeBlock)
        {
            if ((codeBlock.XmlAttributes.TryGetValue("language", out var language) || codeBlock.XmlAttributes.TryGetValue("lang", out language)) && _lexers.TryGetValue(language, out var lexerFactory))
            {
                var lexer = lexerFactory();
                var formattedCode = Pygmentize
                    .Content(codeBlock.Code)
                    .WithLexer(lexer)
                    .WithFormatter(new HtmlFormatter(new HtmlFormatterOptions()))
                    .ToHtml()
                    .AsString();
                TextWriter.Write(formattedCode);
            }
            else
                base.VisitCodeBlock(codeBlock);
        }
    }
}