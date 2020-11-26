using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CodeMap.DeclarationNodes;
using CodeMap.Handlebars;
using EmbeddedResourceBrowser;
using HtmlAgilityPack;

namespace CodeMap.Documentation
{
    internal static class Program
    {
        private const string CodeMapDirectoryName = "CodeMap";
        private const string CodeMapHandlebarsDirectoryName = "CodeMap.Handlebars";

        internal static void Main(params string[] args)
        {
            var arguments = Arguments.GetFrom(args);
            if (string.IsNullOrWhiteSpace(arguments.OutputPath))
                throw new ArgumentException("Expected -OutputPath", nameof(args));
            if (string.IsNullOrWhiteSpace(arguments.TargetSubdirectory))
                throw new ArgumentException("Expected -TargetSubdirectory", nameof(args));

            var templateWriter = new HandlebarsTemplateWriter(
                new MemberReferenceResolver(
                    new Dictionary<Assembly, IMemberReferenceResolver>
                    {
                        { typeof(DeclarationNode).Assembly, new CodeMapMemberReferenceResolver($"../../{arguments.TargetSubdirectory}/{CodeMapDirectoryName}/") },
                        { typeof(HandlebarsTemplateWriter).Assembly, new CodeMapMemberReferenceResolver($"../../{arguments.TargetSubdirectory}/{CodeMapHandlebarsDirectoryName}/") }
                    },
                    new MicrosoftDocsMemberReferenceResolver("netstandard-2.1")
                )
            );

            var codeMapAssemblyDeclaration = DeclarationNode
                .Create(typeof(DeclarationNode).Assembly)
                .Apply(new DocumentationAdditions.Version1_0.CodeMapAssemblyDocumentationAddition());

            WriteHomePage(arguments, templateWriter, codeMapAssemblyDeclaration);
            codeMapAssemblyDeclaration
                .Accept(new HandlebarsWriterDeclarationNodeVisitor(_GetTargetDirectory(arguments, CodeMapDirectoryName), templateWriter));
            DeclarationNode
                .Create(typeof(HandlebarsTemplateWriter).Assembly)
                .Apply(new DocumentationAdditions.Version1_0.CodeMapHandlerbarsAssemblyDocumentationAddition())
                .Accept(new HandlebarsWriterDeclarationNodeVisitor(_GetTargetDirectory(arguments, CodeMapHandlebarsDirectoryName), templateWriter));

            _UpdateFiles(arguments, templateWriter);
        }

        private static void WriteHomePage(Arguments arguments, HandlebarsTemplateWriter templateWriter, AssemblyDeclaration codeMapAssemblyDeclaration)
        {
            var outputDirectory = new DirectoryInfo(arguments.OutputPath);
            outputDirectory.Create();

            using (var indexFileStreamWriter = new StreamWriter(new FileStream(Path.Combine(outputDirectory.FullName, "index.html"), FileMode.Create, FileAccess.Write, FileShare.Read)))
                templateWriter.Write(indexFileStreamWriter, "Home", codeMapAssemblyDeclaration);

            templateWriter.Assets.CopyTo(outputDirectory);
        }

        private static DirectoryInfo _GetTargetDirectory(Arguments arguments, string documentationDirectoryName)
        {
            var targetDirectory = new DirectoryInfo(arguments.OutputPath)
                .CreateSubdirectory(arguments.TargetSubdirectory)
                .CreateSubdirectory(documentationDirectoryName);

            targetDirectory.Delete(true);
            targetDirectory.Create();

            return targetDirectory;
        }

        private static void _UpdateFiles(Arguments arguments, HandlebarsTemplateWriter templateWriter)
        {
            var outputDirectory = new DirectoryInfo(arguments.OutputPath);
            var versions = _GetVersions(outputDirectory);

            var directories = outputDirectory
                .GetDirectories()
                .SelectMany(versionDirectory => versionDirectory.GetDirectories())
                .Concat(Enumerable.Repeat(outputDirectory, 1));

            Parallel.ForEach(directories, _WriteAssets);

            Parallel.ForEach(
                directories.SelectMany(direcotry => direcotry.EnumerateFiles("*.html", SearchOption.TopDirectoryOnly)),
                htmlFile =>
                {
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.Load(htmlFile.FullName);

                    var navigationHtmlNode = htmlDocument.GetElementbyId("navigation");
                    if (navigationHtmlNode != null)
                        navigationHtmlNode.ParentNode.ReplaceChild(
                            htmlDocument.CreateTextNode(ApplyNavigation(htmlFile, versions)),
                            navigationHtmlNode
                        );

                    var deprecationNoticeHtmlNode = htmlDocument.GetElementbyId("deprecationNotice");
                    if (deprecationNoticeHtmlNode != null)
                        if (_IsDocumentationSelected(CodeMapDirectoryName, htmlFile))
                            deprecationNoticeHtmlNode.ParentNode.ReplaceChild(
                                htmlDocument.CreateTextNode(ApplyDeprecationNotice(CodeMapDirectoryName, htmlFile, versions.LastOrDefault())),
                                deprecationNoticeHtmlNode
                            );
                        else if (_IsDocumentationSelected(CodeMapHandlebarsDirectoryName, htmlFile))
                            deprecationNoticeHtmlNode.ParentNode.ReplaceChild(
                                htmlDocument.CreateTextNode(ApplyDeprecationNotice(CodeMapHandlebarsDirectoryName, htmlFile, versions.LastOrDefault())),
                                deprecationNoticeHtmlNode
                            );

                    htmlDocument.Save(htmlFile.FullName);
                }
            );

            string ApplyNavigation(FileInfo htmlFile, IEnumerable<string> versions)
            {
                var isHomePageSelected = string.Equals(htmlFile.Directory.FullName, outputDirectory.FullName, StringComparison.OrdinalIgnoreCase) && string.Equals("index.html", htmlFile.Name);
                return templateWriter.Apply(
                    "Navigation",
                    new
                    {
                        Home = new
                        {
                            IsSelected = isHomePageSelected,
                            Path = isHomePageSelected ? "index.html" : "../../index.html",
                            Label = "Home"
                        },
                        CodeMapVersions = from codeMapVersion in versions.Reverse()
                                          select new
                                          {
                                              IsSelected = _IsSelectedVersion(CodeMapDirectoryName, htmlFile, codeMapVersion),
                                              Label = codeMapVersion,
                                              Path = isHomePageSelected
                                                  ? $"{codeMapVersion}/{CodeMapDirectoryName}/index.html"
                                                  : $"../../{codeMapVersion}/{CodeMapDirectoryName}/index.html"
                                          },
                        IsCodeMapSelected = _IsDocumentationSelected(CodeMapDirectoryName, htmlFile),
                        CodeMapHandlebarsVersions = from codeMapHandlebarsVersion in versions.Reverse()
                                                    select new
                                                    {
                                                        IsSelected = _IsSelectedVersion(CodeMapHandlebarsDirectoryName, htmlFile, codeMapHandlebarsVersion),
                                                        Label = codeMapHandlebarsVersion,
                                                        Path = isHomePageSelected
                                                            ? $"{codeMapHandlebarsVersion}/{CodeMapHandlebarsDirectoryName}/index.html"
                                                            : $"../../{codeMapHandlebarsVersion}/{CodeMapHandlebarsDirectoryName}/index.html"
                                                    },
                        IsCodeMapHandlebarsSelected = _IsDocumentationSelected(CodeMapHandlebarsDirectoryName, htmlFile)
                    });
            }

            string ApplyDeprecationNotice(string documentationDirectoryName, FileInfo htmlFile, string latestVersion)
                => templateWriter.Apply(
                    "DeprecationNotice",
                    new
                    {
                        IsLatest = latestVersion is null || _IsSelectedVersion(documentationDirectoryName, htmlFile, latestVersion),
                        PathToLatest = $"../../{latestVersion}/{documentationDirectoryName}/index.html"
                    });
        }

        private static IEnumerable<string> _GetVersions(DirectoryInfo outputDirectory)
            => outputDirectory
                .GetDirectories()
                .Where(_IsVersionDirectory)
                .OrderBy(directory => directory.Name, Comparer<string>.Create(_VersionComparison))
                .Select(directory => directory.Name)
                .ToList();

        private static bool _IsVersionDirectory(DirectoryInfo directory)
            => Regex.IsMatch(directory.Name, @"^\d+\.\d+\.\d+(-(alpha|beta|rc)\d+)?$", RegexOptions.IgnoreCase);

        private static void _WriteAssets(DirectoryInfo directory)
        {
            foreach (var resource in typeof(Program).Assembly.GetManifestResourceNames())
            {
                var match = Regex.Match(resource, @"Assets\.(?<fileName>\w+\.\w+)$", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    using var fileStream = new FileStream(Path.Combine(directory.FullName, match.Groups["fileName"].Value), FileMode.Create, FileAccess.Write);
                    using var assetStream = typeof(Program).Assembly.GetManifestResourceStream(resource);
                    assetStream.CopyTo(fileStream);
                }
            }
        }

        private static int _VersionComparison(string left, string right)
        {
            var leftBaseVersion = left.Split(new[] { '-' }, 2).First();
            var rightBaseVersion = right.Split(new[] { '-' }, 2).First();
            var baseComparation = string.Compare(leftBaseVersion, rightBaseVersion, StringComparison.OrdinalIgnoreCase);

            if (baseComparation == 0)
                if (left == leftBaseVersion)
                    return 1;
                else if (right == rightBaseVersion)
                    return -1;
                else
                    return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
            else
                return baseComparation;
        }

        private static bool _IsSelectedVersion(string documentationDirectoryName, FileInfo htmlFile, string version)
            => string.Equals(version, htmlFile.Directory.Parent.Name, StringComparison.OrdinalIgnoreCase)
            && _IsDocumentationSelected(documentationDirectoryName, htmlFile);

        private static bool _IsDocumentationSelected(string documentationDirectoryName, FileInfo htmlFile)
            => string.Equals(documentationDirectoryName, htmlFile.Directory.Name, StringComparison.OrdinalIgnoreCase);
    }
}