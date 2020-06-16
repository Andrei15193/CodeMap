using CodeMap.DeclarationNodes;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeMap.Documentation
{
    public class HtmlWriterDeclarationNodeVisitor : DeclarationNodeVisitor
    {
        private readonly DirectoryInfo _directoryInfo;

        public HtmlWriterDeclarationNodeVisitor(DirectoryInfo directoryInfo)
            => _directoryInfo = directoryInfo;

        protected override void VisitAssembly(AssemblyDeclaration assembly)
        {
            _WriteAssets();
            _WriteHtml("Index.html", new[] { (assembly.Name, "Index.html") }, assembly, contentElement =>
            {
                assembly.Summary.Accept(new HtmlWriterDocumentationVisitor(contentElement, assembly));

                contentElement
                    .AddChild("h2")
                        .AppendText("Namespaces")
                        .ParentNode
                    .AddChild("table").SetClass("table table-hover")
                        .AddChild("thead")
                            .AddChild("tr")
                                .AddChild("th")
                                    .AppendText("Name")
                                    .ParentNode
                                .AddChild("th")
                                    .AppendText("Summary")
                                    .ParentNode
                                .ParentNode
                            .ParentNode
                        .AddChild("tbody")
                            .Aggregate(
                                from @namespace in assembly.Namespaces
                                where @namespace.DeclaredTypes.Any(@type => type.AccessModifier == AccessModifier.Public)
                                orderby @namespace.Name
                                select @namespace,
                                (@namespace, parent) => parent
                                    .AddChild("tr")
                                        .AddChild("td")
                                            .AddChild("a").SetAttribute("href", $"{@namespace.Name}.html")
                                            .AppendText(@namespace.Name)
                                            .ParentNode
                                        .AddChild("td")
                                            .Apply(tableData => @namespace.Summary.Accept(new HtmlWriterDocumentationVisitor(tableData, assembly)))
                            );

                assembly.Remarks.Accept(new HtmlWriterDocumentationVisitor(contentElement, assembly));
            });

            _UpdateVersions(assembly);
            _CopyLatestToSpecificVersion(assembly);
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

        private static string _GetVersion(Version version)
        {
            var prerelease = string.Empty;
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

        private void _WriteAssets()
        {
            foreach (var resource in typeof(Program).Assembly.GetManifestResourceNames())
            {
                var match = Regex.Match(resource, @"Assets\.(?<fileName>\w+\.\w+)$", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    using var fileStream = new FileStream(Path.Combine(_directoryInfo.FullName, match.Groups["fileName"].Value), FileMode.Create, FileAccess.Write);
                    using var assetStream = typeof(Program).Assembly.GetManifestResourceStream(resource);
                    assetStream.CopyTo(fileStream);
                }
            }
        }

        private void _UpdateVersions(AssemblyDeclaration assembly)
        {
            foreach (var htmlFile in _directoryInfo.GetFiles("*.html", SearchOption.AllDirectories))
            {
                var htmlDocument = new HtmlDocument();
                htmlDocument.Load(htmlFile.FullName);
                var otherVersionsHtmlNode = htmlDocument.GetElementbyId("otherVersions");
                otherVersionsHtmlNode.ChildNodes.Clear();

                var otherVersions = _directoryInfo.GetDirectories().Where(subdirectory => subdirectory.Name != _GetVersion(assembly.Version));
                if (otherVersions.Any())
                    otherVersionsHtmlNode
                        .SetClass("btn-group")
                        .SetAttribute("role", "group")
                        .AddChild("button").SetAttribute("id", "otherVersionsButtonGroup").SetAttribute("type", "button").SetClass("btn btn-link dropdown-toggle").SetAttribute("data-toggle", "dropdown").SetAttribute("aria-haspopup", "true").SetAttribute("aria-expanded", "false").AppendText("Other Versions")
                            .ParentNode
                        .AddChild("div").SetClass("dropdown-menu").SetAttribute("aria-labelledby", "otherVersionsButtonGroup")
                            .Aggregate(
                                otherVersions,
                                (otherVersion, parent) => parent
                                    .AddChild("a").SetClass("dropdown-item").SetAttribute("href", otherVersion.Name)
                                        .AppendText(otherVersion.Name)
                            );
                htmlDocument.Save(htmlFile.FullName);
            }
        }

        private void _CopyLatestToSpecificVersion(AssemblyDeclaration assembly)
        {
            var currentVersionDirectory = _directoryInfo.CreateSubdirectory(_GetVersion(assembly.Version));
            foreach (var currentVersionFile in _directoryInfo.GetFiles())
                currentVersionFile.CopyTo(Path.Combine(currentVersionDirectory.FullName, currentVersionFile.Name), true);
        }

        private void _WriteHtml(string fileName, IEnumerable<(string DisplayText, string FileName)> breadcrumbs, AssemblyDeclaration assembly, Action<HtmlNode> callback)
            => _WriteHtml(fileName, null, breadcrumbs, assembly, callback);

        private void _WriteHtml(string fileName, string pageTitle, IEnumerable<(string DisplayText, string FileName)> breadcrumbs, AssemblyDeclaration assembly, Action<HtmlNode> callback)
            => new HtmlDocument()
                .DocumentNode
                .AddDoctype()
                .AddChild("head")
                    .AddChild("meta").SetAttribute("charset", "utf-8")
                        .ParentNode
                    .AddChild("meta").SetAttribute("http-equiv", "Cache-Control").SetAttribute("content", "no-cache, no-store, must-revalidate")
                        .ParentNode
                    .AddChild("meta").SetAttribute("http-equiv", "Pragma").SetAttribute("content", "no-cache")
                        .ParentNode
                    .AddChild("meta").SetAttribute("http-equiv", "Expires").SetAttribute("content", "0")
                        .ParentNode
                    .AddChild("title").AppendText(string.IsNullOrWhiteSpace(pageTitle) ? assembly.Name : $"{assembly.Name} - {pageTitle}")
                        .ParentNode
                    .AddChild("link").SetAttribute("rel", "stylesheet").SetAttribute("href", "https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css").SetAttribute("integrity", "sha384-9aIt2nRpC12Uk9gS9baDl411NQApFmC26EwAOH8WgZl5MYYxFfc+NcPb1dKGj7Sk").SetAttribute("crossorigin", "anonymous")
                        .ParentNode
                    .AddChild("link").SetAttribute("rel", "stylesheet").SetAttribute("href", "style.css")
                        .ParentNode
                    .ParentNode
                .AddChild("body")
                    .AddChild("div")
                        .SetClass("d-flex flex-row align-items-center container px-2 pt-2")
                        .AddChild("h1")
                            .SetClass("flex-grow-1 flex-shrink-1")
                            .AppendText(assembly.Name)
                            .AppendText(" ")
                            .AddChild("small")
                                .AppendText(_GetVersion(assembly.Version))
                                .ParentNode
                            .ParentNode
                        .AddChild("a").SetClass("btn btn-link").SetAttribute("href", "https://github.com/Andrei15193/CodeMap")
                            .AppendText("View on GitHub")
                            .ParentNode
                        .AddChild("a").SetClass("btn btn-link").SetAttribute("href", $"https://www.nuget.org/packages/CodeMap/{_GetVersion(assembly.Version)}")
                            .AppendText("View on NuGet")
                            .ParentNode
                        .AddChild("div").SetAttribute("id", "otherVersions")
                            .ParentNode
                        .ParentNode
                    .AddChild("hr")
                        .ParentNode
                    .AddChild("div").SetClass("d-flex flex-column container px-2 pt-2")
                        .AddChild("nav").SetAttribute("aria-label", "breadcrumb")
                            .AddChild("ol").SetClass("breadcrumb")
                                .Aggregate(
                                    breadcrumbs.Reverse().Skip(1).Reverse(),
                                    (breadcrumb, parent) => parent
                                        .AddChild("li").SetClass("breadcrumb-item").SetAttribute("aria-current", "page")
                                            .AddChild("a").SetAttribute("href", breadcrumb.FileName)
                                                .AppendText(breadcrumb.DisplayText)
                                )
                                .AddChild("li").SetClass("breadcrumb-item active").SetAttribute("aria-current", "page")
                                    .AppendText(breadcrumbs.Last().DisplayText)
                                    .ParentNode
                                .ParentNode
                            .ParentNode
                        .AddChild("div")
                            .Apply(callback)
                            .ParentNode
                        .ParentNode
                    .AddChild("hr")
                        .ParentNode
                    .AddChild("div").SetClass("d-flex flex-column container px-2")
                        .AddChild("p").SetClass("footer")
                            .AddChild("a").SetAttribute("href", $"https://github.com/Andrei15193/CodeMap/releases/tag/{_GetVersion(assembly.Version)}")
                                .AppendText($"{assembly.Name} {_GetVersion(assembly.Version)}")
                                .ParentNode
                            .AppendText(" - ")
                            .AddChild("a").SetAttribute("href", "https://github.com/Andrei15193/CodeMap")
                                .AppendText("View on GitHub")
                                .ParentNode
                            .AppendText(" - ")
                            .AddChild("a").SetAttribute("href", $"https://www.nuget.org/packages/CodeMap/{_GetVersion(assembly.Version)}")
                                .AppendText("View on NuGet")
                                .ParentNode
                            .ParentNode
                        .ParentNode
                    .AddChild("script").SetAttribute("src", "https://code.jquery.com/jquery-3.5.1.slim.min.js").SetAttribute("integrity", "sha384-DfXdz2htPH0lsSSs5nCTpuj/zy4C+OGpamoFVy38MVBnE+IbbVYUew+OrCXaRkfj").SetAttribute("crossorigin", "anonymous")
                        .ParentNode
                    .AddChild("script").SetAttribute("src", "https://cdn.jsdelivr.net/npm/popper.js@1.16.0/dist/umd/popper.min.js").SetAttribute("integrity", "sha384-Q6E9RHvbIyZFJoft+2mJbHaEWldlvI9IOYy5n3zV9zzTtmI3UksdQRVvoxMfooAo").SetAttribute("crossorigin", "anonymous")
                        .ParentNode
                    .AddChild("script").SetAttribute("src", "https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/js/bootstrap.min.js").SetAttribute("integrity", "sha384-OgVRvuATP1z7JjHLkuOU7Xw704+h835Lr+6QL9UvYjZE3Ipu6Tp75j7Bh/kR0JKI").SetAttribute("crossorigin", "anonymous")
                        .OwnerDocument
            .Save(Path.Combine(_directoryInfo.FullName, fileName), Encoding.UTF8);
    }
}