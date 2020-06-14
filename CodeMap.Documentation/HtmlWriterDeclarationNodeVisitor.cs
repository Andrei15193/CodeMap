using CodeMap.DeclarationNodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CodeMap.Documentation
{
    public class HtmlWriterDeclarationNodeVisitor : DeclarationNodeVisitor
    {
        private readonly DirectoryInfo _directoryInfo;

        public HtmlWriterDeclarationNodeVisitor(DirectoryInfo directoryInfo)
            => _directoryInfo = directoryInfo;

        protected override void VisitAssembly(AssemblyDeclaration assembly)
        {
            _WriteCss("Style.css");

            _WriteHtml(
                "Index.html",
                new[] { assembly.Name },
                assembly,
                textWriter =>
                {
                    assembly.Summary.Accept(new HtmlWriterDocumentationVisitor(textWriter, assembly));

                    textWriter.Write("<h2>Namespaces</h2>");
                    textWriter.Write("<table class=\"table table-hover\">");
                    textWriter.Write("<thead>");
                    textWriter.Write("<tr>");
                    textWriter.Write("<th>Name</th>");
                    textWriter.Write("<th>Summary</th>");
                    textWriter.Write("</tr>");
                    textWriter.Write("</thead>");

                    textWriter.Write("<tbody>");
                    foreach (var @namespace in from @namespace in assembly.Namespaces
                                               where @namespace.DeclaredTypes.Any(@type => type.AccessModifier == AccessModifier.Public)
                                               orderby @namespace.Name
                                               select @namespace)
                    {
                        textWriter.Write("<tr>");
                        textWriter.Write($"<td><a href=\"{@namespace.Name}.html\">{@namespace.Name}</a></td>");
                        textWriter.Write("<td>");
                        @namespace.Summary.Accept(new HtmlWriterDocumentationVisitor(textWriter, assembly));
                        textWriter.Write("</td>");
                        textWriter.Write("</tr>");
                    }
                    textWriter.Write("</tbody>");
                    textWriter.Write("</table>");

                    assembly.Remarks.Accept(new HtmlWriterDocumentationVisitor(textWriter, assembly));
                }
            );

            var currentVersionDirectory = _directoryInfo.CreateSubdirectory(_GetVersion(assembly.Version));
            foreach (var currentVersionFile in _directoryInfo.GetFiles())
                currentVersionFile.CopyTo(Path.Combine(currentVersionDirectory.FullName, currentVersionFile.Name), true);
        }

        private static string _GetVersion(Version version)
        {
            var prerelease = "";

            if (version.Build > 0)
                switch (version.Build / 1000)
                {
                    case 1:
                        prerelease = "-alpha" + version.Build % 1000;
                        break;

                    case 2:
                        prerelease = "-beta" + version.Build % 1000;
                        break;

                    case 3:
                        prerelease = "-rc" + version.Build % 1000;
                        break;
                }

            return $"{version.Major}.{version.Minor}.{version.Revision}{prerelease}";
        }

        protected override void VisitClass(ClassDeclaration @class)
        {
            throw new NotImplementedException();
        }

        protected override void VisitConstant(ConstantDeclaration constant)
        {
            throw new NotImplementedException();
        }

        protected override void VisitConstructor(ConstructorDeclaration constructor)
        {
            throw new NotImplementedException();
        }

        protected override void VisitDelegate(DelegateDeclaration @delegate)
        {
            throw new NotImplementedException();
        }

        protected override void VisitEnum(EnumDeclaration @enum)
        {
            throw new NotImplementedException();
        }

        protected override void VisitEvent(EventDeclaration @event)
        {
            throw new NotImplementedException();
        }

        protected override void VisitField(FieldDeclaration field)
        {
            throw new NotImplementedException();
        }

        protected override void VisitInterface(InterfaceDeclaration @interface)
        {
            throw new NotImplementedException();
        }

        protected override void VisitMethod(MethodDeclaration method)
        {
            throw new NotImplementedException();
        }

        protected override void VisitNamespace(NamespaceDeclaration @namespace)
        {
            throw new NotImplementedException();
        }

        protected override void VisitProperty(PropertyDeclaration property)
        {
            throw new NotImplementedException();
        }

        protected override void VisitStruct(StructDeclaration @struct)
        {
            throw new NotImplementedException();
        }

        private void _WriteCss(string fileName)
        {
            using var fileStream = new FileStream(Path.Combine(_directoryInfo.FullName, fileName), FileMode.Create, FileAccess.Write);
            using var textWriter = new StreamWriter(fileStream, Encoding.UTF8);

            textWriter.Write("p, th, td, li { text-align: justify; } ");
            textWriter.Write("table.table > thead > tr > th p:last-child, table.table > tbody > tr > td p:last-child { margin-bottom: 0; } ");
            textWriter.Write("pre { padding: 2px 7px; background-color: #F8F8F8; } ");
            textWriter.Write("p.footer { text-align: center; } ");

            textWriter.Write("/* Pygments theme: Colorful */ ");
            textWriter.Write(".highlight .hll { background-color: #ffffcc } ");
            textWriter.Write(".highlight  { background: #ffffff; } ");
            textWriter.Write(".highlight .c { color: #888888 } /* Comment */ ");
            textWriter.Write(".highlight .err { color: #FF0000; background-color: #FFAAAA } /* Error */ ");
            textWriter.Write(".highlight .k { color: #008800; font-weight: bold } /* Keyword */ ");
            textWriter.Write(".highlight .o { color: #333333 } /* Operator */ ");
            textWriter.Write(".highlight .ch { color: #888888 } /* Comment.Hashbang */ ");
            textWriter.Write(".highlight .cm { color: #888888 } /* Comment.Multiline */ ");
            textWriter.Write(".highlight .cp { color: #557799 } /* Comment.Preproc */ ");
            textWriter.Write(".highlight .cpf { color: #888888 } /* Comment.PreprocFile */ ");
            textWriter.Write(".highlight .c1 { color: #888888 } /* Comment.Single */ ");
            textWriter.Write(".highlight .cs { color: #cc0000; font-weight: bold } /* Comment.Special */ ");
            textWriter.Write(".highlight .gd { color: #A00000 } /* Generic.Deleted */ ");
            textWriter.Write(".highlight .ge { font-style: italic } /* Generic.Emph */ ");
            textWriter.Write(".highlight .gr { color: #FF0000 } /* Generic.Error */ ");
            textWriter.Write(".highlight .gh { color: #000080; font-weight: bold } /* Generic.Heading */ ");
            textWriter.Write(".highlight .gi { color: #00A000 } /* Generic.Inserted */ ");
            textWriter.Write(".highlight .go { color: #888888 } /* Generic.Output */ ");
            textWriter.Write(".highlight .gp { color: #c65d09; font-weight: bold } /* Generic.Prompt */ ");
            textWriter.Write(".highlight .gs { font-weight: bold } /* Generic.Strong */ ");
            textWriter.Write(".highlight .gu { color: #800080; font-weight: bold } /* Generic.Subheading */ ");
            textWriter.Write(".highlight .gt { color: #0044DD } /* Generic.Traceback */ ");
            textWriter.Write(".highlight .kc { color: #008800; font-weight: bold } /* Keyword.Constant */ ");
            textWriter.Write(".highlight .kd { color: #008800; font-weight: bold } /* Keyword.Declaration */ ");
            textWriter.Write(".highlight .kn { color: #008800; font-weight: bold } /* Keyword.Namespace */ ");
            textWriter.Write(".highlight .kp { color: #003388; font-weight: bold } /* Keyword.Pseudo */ ");
            textWriter.Write(".highlight .kr { color: #008800; font-weight: bold } /* Keyword.Reserved */ ");
            textWriter.Write(".highlight .kt { color: #333399; font-weight: bold } /* Keyword.Type */ ");
            textWriter.Write(".highlight .m { color: #6600EE; font-weight: bold } /* Literal.Number */ ");
            textWriter.Write(".highlight .s { background-color: #fff0f0 } /* Literal.String */ ");
            textWriter.Write(".highlight .na { color: #0000CC } /* Name.Attribute */ ");
            textWriter.Write(".highlight .nb { color: #007020 } /* Name.Builtin */ ");
            textWriter.Write(".highlight .nc { color: #BB0066; font-weight: bold } /* Name.Class */ ");
            textWriter.Write(".highlight .no { color: #003366; font-weight: bold } /* Name.Constant */ ");
            textWriter.Write(".highlight .nd { color: #555555; font-weight: bold } /* Name.Decorator */ ");
            textWriter.Write(".highlight .ni { color: #880000; font-weight: bold } /* Name.Entity */ ");
            textWriter.Write(".highlight .ne { color: #FF0000; font-weight: bold } /* Name.Exception */ ");
            textWriter.Write(".highlight .nf { color: #0066BB; font-weight: bold } /* Name.Function */ ");
            textWriter.Write(".highlight .nl { color: #997700; font-weight: bold } /* Name.Label */ ");
            textWriter.Write(".highlight .nn { color: #0e84b5; font-weight: bold } /* Name.Namespace */ ");
            textWriter.Write(".highlight .nt { color: #007700 } /* Name.Tag */ ");
            textWriter.Write(".highlight .nv { color: #996633 } /* Name.Variable */ ");
            textWriter.Write(".highlight .ow { color: #000000; font-weight: bold } /* Operator.Word */ ");
            textWriter.Write(".highlight .w { color: #bbbbbb } /* Text.Whitespace */ ");
            textWriter.Write(".highlight .mb { color: #6600EE; font-weight: bold } /* Literal.Number.Bin */ ");
            textWriter.Write(".highlight .mf { color: #6600EE; font-weight: bold } /* Literal.Number.Float */ ");
            textWriter.Write(".highlight .mh { color: #005588; font-weight: bold } /* Literal.Number.Hex */ ");
            textWriter.Write(".highlight .mi { color: #0000DD; font-weight: bold } /* Literal.Number.Integer */ ");
            textWriter.Write(".highlight .mo { color: #4400EE; font-weight: bold } /* Literal.Number.Oct */ ");
            textWriter.Write(".highlight .sa { background-color: #fff0f0 } /* Literal.String.Affix */ ");
            textWriter.Write(".highlight .sb { background-color: #fff0f0 } /* Literal.String.Backtick */ ");
            textWriter.Write(".highlight .sc { color: #0044DD } /* Literal.String.Char */ ");
            textWriter.Write(".highlight .dl { background-color: #fff0f0 } /* Literal.String.Delimiter */ ");
            textWriter.Write(".highlight .sd { color: #DD4422 } /* Literal.String.Doc */ ");
            textWriter.Write(".highlight .s2 { background-color: #fff0f0 } /* Literal.String.Double */ ");
            textWriter.Write(".highlight .se { color: #666666; font-weight: bold; background-color: #fff0f0 } /* Literal.String.Escape */ ");
            textWriter.Write(".highlight .sh { background-color: #fff0f0 } /* Literal.String.Heredoc */ ");
            textWriter.Write(".highlight .si { background-color: #eeeeee } /* Literal.String.Interpol */ ");
            textWriter.Write(".highlight .sx { color: #DD2200; background-color: #fff0f0 } /* Literal.String.Other */ ");
            textWriter.Write(".highlight .sr { color: #000000; background-color: #fff0ff } /* Literal.String.Regex */ ");
            textWriter.Write(".highlight .s1 { background-color: #fff0f0 } /* Literal.String.Single */ ");
            textWriter.Write(".highlight .ss { color: #AA6600 } /* Literal.String.Symbol */ ");
            textWriter.Write(".highlight .bp { color: #007020 } /* Name.Builtin.Pseudo */ ");
            textWriter.Write(".highlight .fm { color: #0066BB; font-weight: bold } /* Name.Function.Magic */ ");
            textWriter.Write(".highlight .vc { color: #336699 } /* Name.Variable.Class */ ");
            textWriter.Write(".highlight .vg { color: #dd7700; font-weight: bold } /* Name.Variable.Global */ ");
            textWriter.Write(".highlight .vi { color: #3333BB } /* Name.Variable.Instance */ ");
            textWriter.Write(".highlight .vm { color: #996633 } /* Name.Variable.Magic */ ");
            textWriter.Write(".highlight .il { color: #0000DD; font-weight: bold } /* Literal.Number.Integer.Long */ ");
        }

        private void _WriteHtml(string fileName, IEnumerable<string> breadcrumbs, AssemblyDeclaration assembly, Action<TextWriter> callback)
            => _WriteHtml(fileName, null, breadcrumbs, assembly, callback);

        private void _WriteHtml(string fileName, string pageTitle, IEnumerable<string> breadcrumbs, AssemblyDeclaration assembly, Action<TextWriter> callback)
        {
            using var fileStream = new FileStream(Path.Combine(_directoryInfo.FullName, fileName), FileMode.Create, FileAccess.Write);
            using var textWriter = new StreamWriter(fileStream, Encoding.UTF8);

            textWriter.Write("<!DOCTYPE html>");
            textWriter.Write("<head>");
            textWriter.Write("<meta charset=\"utf-8\">");
            textWriter.Write("<meta http-equiv=\"Cache-Control\" content=\"no-cache, no-store, must-revalidate\">");
            textWriter.Write("<meta http-equiv=\"Pragma\" content=\"no-cache\">");
            textWriter.Write("<meta http-equiv=\"Expires\" content=\"0\">");
            textWriter.Write("<link rel=\"stylesheet\" href=\"https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css\" integrity=\"sha384-9aIt2nRpC12Uk9gS9baDl411NQApFmC26EwAOH8WgZl5MYYxFfc+NcPb1dKGj7Sk\" crossorigin=\"anonymous\">");
            textWriter.Write("<link rel=\"stylesheet\" href=\"style.css\">");
            textWriter.Write("<link rel=\"stylesheet\" href=\"https://raw.githubusercontent.com/jwarby/pygments-css/master/zenburn.css\">");
            textWriter.Write("<title>");
            textWriter.Write(assembly.Name);
            if (!string.IsNullOrWhiteSpace(pageTitle))
            {
                textWriter.Write(" - ");
                textWriter.Write(pageTitle);
            }
            textWriter.Write("</title>");
            textWriter.Write("</head>");

            textWriter.Write("<body>");

            textWriter.Write("<div class=\"d-flex flex-row align-items-center container px-2 pt-2\">");
            textWriter.Write($"<h1 class=\"flex-grow-1 flex-shrink-1\">{assembly.Name} <small>({_GetVersion(assembly.Version)})</small></h1>");

            textWriter.Write("<a class=\"btn btn-link \" href=\"https://github.com/Andrei15193/CodeMap\">View on GitHub</a>");
            textWriter.Write($"<a class=\"btn btn-link \" href=\"https://www.nuget.org/packages/CodeMap/{_GetVersion(assembly.Version)}\">View on NuGet</a>");

            var olderVersions = _directoryInfo.GetDirectories().Where(subdirectory => subdirectory.Name != _GetVersion(assembly.Version));
            if (olderVersions.Any())
            {
                textWriter.Write("<div class=\"btn-group\" role=\"group\">");
                textWriter.Write("<button id=\"headerButtonGroup\" type=\"button\" class=\"btn btn-link dropdown-toggle\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\">");
                textWriter.Write("Older Versions");
                textWriter.Write("</button>");
                textWriter.Write("<div class=\"dropdown-menu\" aria-labelledby=\"headerButtonGroup\">");
                foreach (var olderVersion in olderVersions)
                    textWriter.Write($"<a class=\"dropdown-item\" href=\"{olderVersion.Name}/Index.html\">{olderVersion.Name}</a>");
                textWriter.Write("</div>");
                textWriter.Write("</div>");
            }

            textWriter.Write("</div>");

            textWriter.Write("<hr>");

            textWriter.Write("<div class=\"d-flex flex-column container px-2 pt-2\">");
            textWriter.Write("<nav aria-label=\"breadcrumb\">");
            textWriter.Write("<ol class=\"breadcrumb\">");
            foreach (var breadcrumb in breadcrumbs.Reverse().Skip(1).Reverse())
            {
                textWriter.Write($"<li class=\"breadcrumb-item active\" aria-current=\"page\">");
                textWriter.Write(breadcrumb);
                textWriter.Write("</li>");
            }
            textWriter.Write($"<li class=\"breadcrumb-item\" aria-current=\"page\">");
            textWriter.Write(breadcrumbs.Last());
            textWriter.Write("</li>");
            textWriter.Write("</ol>");
            textWriter.Write("</nav>");

            callback(textWriter);

            textWriter.Write("</div>");

            textWriter.Write("<hr>");

            textWriter.Write("<div class=\"d-flex flex-column container px-2\">");
            textWriter.Write("<p class=\"footer\">");
            textWriter.Write($"<a href=\"https://github.com/Andrei15193/CodeMap/releases/tag/{_GetVersion(assembly.Version)}\">{assembly.Name} {_GetVersion(assembly.Version)}</a>");
            textWriter.Write(" - <a href=\"https://github.com/Andrei15193/CodeMap\">View on GitHub</a>");
            textWriter.Write($" - <a href=\"https://www.nuget.org/packages/CodeMap/{_GetVersion(assembly.Version)}\">View on NuGet</a>");
            var copyrightAttribute = assembly.Attributes.FirstOrDefault(attribute => attribute.Type == typeof(AssemblyCopyrightAttribute));
            if (copyrightAttribute != null)
            {
                var copyrightValue = copyrightAttribute.PositionalParameters.Single().Value;
                textWriter.Write("<br>&copy; ");
                textWriter.Write(DateTime.UtcNow.Year);
                textWriter.Write(" ");
                textWriter.Write(copyrightValue);
            }
            textWriter.Write("</p>");
            textWriter.Write("</div>");

            textWriter.Write("<script src=\"https://code.jquery.com/jquery-3.5.1.slim.min.js\" integrity=\"sha384-DfXdz2htPH0lsSSs5nCTpuj/zy4C+OGpamoFVy38MVBnE+IbbVYUew+OrCXaRkfj\" crossorigin=\"anonymous\"></script>");
            textWriter.Write("<script src=\"https://cdn.jsdelivr.net/npm/popper.js@1.16.0/dist/umd/popper.min.js\" integrity=\"sha384-Q6E9RHvbIyZFJoft+2mJbHaEWldlvI9IOYy5n3zV9zzTtmI3UksdQRVvoxMfooAo\" crossorigin=\"anonymous\"></script>");
            textWriter.Write("<script src=\"https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/js/bootstrap.min.js\" integrity=\"sha384-OgVRvuATP1z7JjHLkuOU7Xw704+h835Lr+6QL9UvYjZE3Ipu6Tp75j7Bh/kR0JKI\" crossorigin=\"anonymous\"></script>");
            textWriter.Write("</body>");
        }
    }
}