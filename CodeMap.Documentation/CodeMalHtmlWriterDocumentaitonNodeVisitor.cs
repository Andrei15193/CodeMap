using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeMap.DeclarationNodes;
using CodeMap.DocumentationElements;
using CodeMap.Html;

namespace CodeMap.Documentation
{
    public class CodeMalHtmlWriterDocumentaitonNodeVisitor : HtmlWriterDeclarationNodeVisitor
    {
        public CodeMalHtmlWriterDocumentaitonNodeVisitor(TextWriter textWriter, IMemberReferenceResolver memberReferenceResolver)
            : base(textWriter, memberReferenceResolver)
        {
        }

        protected override DocumentationVisitor CreateDocumentationVisitor()
            => new CodeMapHtmlWriterDocumentationVisitor(TextWriter, MemberReferenceResolver);

        protected override void WriteOtherHtmlAttributes(DeclarationNode declarationNode)
        {
            TextWriter.Write(" class=\"h-100\"");
        }

        protected override void WriteOtherBodyAttributes(DeclarationNode declarationNode)
        {
            TextWriter.Write(" class=\"px-3 h-100 d-flex flex-column\"");
        }

        protected override void WriteOtherSectionAttributes(DeclarationNode declarationNode)
        {
            TextWriter.Write(" class=\"mt-2 flex-fill\"");
        }

        protected override void WriteNavigation(DeclarationNode declarationNode)
        {
            TextWriter.Write("<nav class=\"badge bg-light p-2 w-100 mb-2\">");
            TextWriter.Write("<ol class=\"breadcrumb m-0\">");
            WriteDeclarationItems(declarationNode);
            TextWriter.Write("</ol>");
            TextWriter.Write("</nav>");
        }

        protected override void WriteNavigationItem(DeclarationNode declarationNode)
        {
            TextWriter.Write("<li class=\"breadcrumb-item\">");
            base.WriteNavigationItem(declarationNode);
            TextWriter.Write("</li>");
        }

        protected override void WriteNavigationActiveItem(DeclarationNode declarationNode)
        {
            TextWriter.Write("<li class=\"breadcrumb-item active\">");
            base.WriteNavigationActiveItem(declarationNode);
            TextWriter.Write("</li>");
        }

        protected override void WriteNamespacesList(IEnumerable<NamespaceDeclaration> namespaces)
        {
            if (namespaces.Any())
            {
                TextWriter.Write("<section data-sectionId=\"namespaces\">");

                TextWriter.Write("<table class=\"table table-hover caption-top\">");

                TextWriter.Write("<thead>");
                TextWriter.Write("<tr>");
                TextWriter.Write("<th>");
                WriteSafeHtml("Namespace");
                TextWriter.Write("</th>");
                TextWriter.Write("<th>");
                WriteSafeHtml("Description");
                TextWriter.Write("</th>");
                TextWriter.Write("</tr>");
                TextWriter.Write("</thead>");

                TextWriter.Write("<tbody>");
                foreach (var @namespace in namespaces)
                {
                    TextWriter.Write("<tr>");
                    TextWriter.Write("<td>");
                    TextWriter.Write("<a href=\"#");
                    TextWriter.Write(Uri.EscapeDataString(@namespace.GetFullNameReference()));
                    TextWriter.Write("\">");
                    WriteSafeHtml(@namespace.GetSimpleNameReference());
                    TextWriter.Write("</a>");
                    TextWriter.Write("</td>");
                    TextWriter.Write("<td>");
                    WriteFirstSummaryParagraph(@namespace.Summary);
                    TextWriter.Write("</td>");
                    TextWriter.Write("</tr>");
                }
                TextWriter.Write("</tbody>");

                TextWriter.Write("</table>");

                TextWriter.Write("</section>");
            }
        }

        protected override void WriteTypesList(IEnumerable<TypeDeclaration> types, string title, string sectionId)
        {
            if (types.Any())
            {
                TextWriter.Write("<section data-sectionId=\"");
                WriteSafeHtml(sectionId);
                TextWriter.Write("\">");

                TextWriter.Write("<table class=\"table table-hover caption-top\">");

                TextWriter.Write("<caption>");
                WriteSafeHtml(title);
                TextWriter.Write("</caption>");

                TextWriter.Write("<thead>");
                TextWriter.Write("<tr>");
                TextWriter.Write("<th>");
                WriteSafeHtml("Name");
                TextWriter.Write("</th>");
                TextWriter.Write("<th>");
                WriteSafeHtml("Description");
                TextWriter.Write("</th>");
                TextWriter.Write("</tr>");
                TextWriter.Write("</thead>");

                TextWriter.Write("<tbody>");
                foreach (var type in types)
                {
                    TextWriter.Write("<tr>");
                    TextWriter.Write("<td>");
                    TextWriter.Write("<a href=\"#");
                    TextWriter.Write(Uri.EscapeDataString(type.GetFullNameReference()));
                    TextWriter.Write("\">");
                    WriteSafeHtml(type.GetSimpleNameReference());
                    TextWriter.Write("</a>");
                    TextWriter.Write("</td>");
                    TextWriter.Write("<td>");
                    WriteFirstSummaryParagraph(type.Summary);
                    TextWriter.Write("</td>");
                    TextWriter.Write("</tr>");
                }
                TextWriter.Write("</tbody>");

                TextWriter.Write("</table>");

                TextWriter.Write("</section>");
            }
        }

        protected override void WriteMembersList(IEnumerable<MemberDeclaration> members, string title, string sectionId, bool hideAccessModifier)
        {
            if (members.Any())
            {
                TextWriter.Write("<section data-sectionId=\"");
                WriteSafeHtml(sectionId);
                TextWriter.Write("\">");

                TextWriter.Write("<table class=\"table table-hover caption-top\">");

                TextWriter.Write("<caption>");
                WriteSafeHtml(title);
                TextWriter.Write("</caption>");

                TextWriter.Write("<thead>");
                TextWriter.Write("<tr>");
                TextWriter.Write("<th>");
                WriteSafeHtml("Name");
                TextWriter.Write("</th>");
                if (!hideAccessModifier)
                {
                    TextWriter.Write("<th>");
                    WriteSafeHtml("Access");
                    TextWriter.Write("</th>");
                }
                TextWriter.Write("<th>");
                WriteSafeHtml("Description");
                TextWriter.Write("</th>");
                TextWriter.Write("</tr>");
                TextWriter.Write("</thead>");

                TextWriter.Write("<tbody>");
                foreach (var member in members)
                {
                    TextWriter.Write("<tr>");
                    TextWriter.Write("<td>");
                    TextWriter.Write("<a href=\"#");
                    TextWriter.Write(Uri.EscapeDataString(member.GetFullNameReference()));
                    TextWriter.Write("\">");
                    WriteSafeHtml(member.GetSimpleNameReference());
                    TextWriter.Write("</a>");
                    TextWriter.Write("</td>");
                    if (!hideAccessModifier)
                    {
                        TextWriter.Write("<td><code>");
                        WriteAccessModifier(member.AccessModifier);
                        TextWriter.Write("</code></td>");
                    }
                    TextWriter.Write("<td>");
                    WriteFirstSummaryParagraph(member.Summary);
                    TextWriter.Write("</td>");
                    TextWriter.Write("</tr>");
                }
                TextWriter.Write("</tbody>");

                TextWriter.Write("</table>");

                TextWriter.Write("</section>");
            }
        }

        protected override void WriteEnumMembersList(IEnumerable<ConstantDeclaration> members)
        {
            if (members.Any())
            {
                TextWriter.Write("<section data-sectionId=\"members\">");

                TextWriter.Write("<table class=\"table table-hover caption-top\">");

                TextWriter.Write("<caption>");
                WriteSafeHtml("Members");
                TextWriter.Write("</caption>");

                TextWriter.Write("<thead>");
                TextWriter.Write("<tr>");
                TextWriter.Write("<th>");
                WriteSafeHtml("Name");
                TextWriter.Write("</th>");
                TextWriter.Write("<th>");
                WriteSafeHtml("Value");
                TextWriter.Write("</th>");
                TextWriter.Write("<th>");
                WriteSafeHtml("Description");
                TextWriter.Write("</th>");
                TextWriter.Write("</tr>");
                TextWriter.Write("</thead>");

                TextWriter.Write("<tbody>");
                foreach (var member in members)
                {
                    TextWriter.Write("<tr>");
                    TextWriter.Write("<td><code>");
                    WriteSafeHtml(member.GetSimpleNameReference());
                    TextWriter.Write("</code></td>");
                    TextWriter.Write("<td><code>");
                    WriteValue(member.Value);
                    TextWriter.Write("</code></td>");
                    TextWriter.Write("<td>");
                    WriteFirstSummaryParagraph(member.Summary);
                    TextWriter.Write("</td>");
                    TextWriter.Write("</tr>");
                }
                TextWriter.Write("</tbody>");

                TextWriter.Write("</table>");

                TextWriter.Write("</section>");
            }
        }

        protected override void WriteHtmlEnding(DeclarationNode declarationNode)
        {
            TextWriter.Write("<footer class=\"mt-2 py-2 border-top text-center\">");
            WriteSafeHtml("CodeMap - GitHub - NuGet");
            TextWriter.Write("</footer>");
            base.WriteHtmlEnding(declarationNode);
        }

        protected override void WriteOtherHtmlHeadTags(DeclarationNode declarationNode)
            => TextWriter.Write("<link href=\"https://cdn.jsdelivr.net/npm/bootstrap@5.2.3/dist/css/bootstrap.min.css\" rel=\"stylesheet\" integrity=\"sha384-rbsA2VBKQhggwzxH7pPCaAqO46MgnOM80zW1RWuH61DGLwZJEdK2Kadq2F9CUG65\" crossorigin=\"anonymous\">");

        protected override void WriteOtherHtmlBodyTags(DeclarationNode declarationNode)
            => TextWriter.Write("<script src=\"https://cdn.jsdelivr.net/npm/bootstrap@5.2.3/dist/js/bootstrap.bundle.min.js\" integrity=\"sha384-kenU1KFdBIe4zVF0s0G1M5b4hcpxyD9F7jL+jjXkk+Q2h455rYXK/7HAuoJl+0I4\" crossorigin=\"anonymous\"></script>");
    }
}