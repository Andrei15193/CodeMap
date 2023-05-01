using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeMap.DocumentationElements;
using CodeMap.Html;

namespace CodeMap.Documentation
{
    public class CodeMapHtmlWriterDocumentationVisitor : HtmlWriterDocumentationVisitor
    {
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
    }
}