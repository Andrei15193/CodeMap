using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CodeMap.DeclarationNodes;
using CodeMap.Handlebars;
using HtmlAgilityPack;

namespace CodeMap.Documentation
{
    internal static class Program
    {
        internal static void Main(params string[] args)
        {
            var arguments = Arguments.GetFrom(args);
            if (arguments.OutputPath == null)
                throw new ArgumentException("Expected -OutputPath", nameof(args));

            WriteHomePage(arguments);
            WriteDocumentation(arguments, typeof(DeclarationNode).Assembly, new DocumentationAdditions.Version1_0.CodeMapAssemblyDocumentationAddition());
            WriteDocumentation(arguments, typeof(HandlebarsTemplateWriter).Assembly, new DocumentationAdditions.Version1_0.CodeMapHandlerbarsAssemblyDocumentationAddition());

            _UpdateFiles(arguments);
        }

        private static void WriteHomePage(Arguments arguments)
        {
            var outputDirectory = new DirectoryInfo(arguments.OutputPath);
            outputDirectory.Create();

            var codeMapAssembly = typeof(DeclarationNode).Assembly;
            var codeMapAssemblyDeclaration = DeclarationNode.Create(codeMapAssembly);
            var templateWriter = new CodeMapHandlebarsTemplateWriter(new DefaultMemberReferenceResolver(codeMapAssembly, "netstandard-2.1"));

            using (var indexFileStreamWriter = new StreamWriter(new FileStream(Path.Combine(outputDirectory.FullName, "index.html"), FileMode.Create, FileAccess.Write, FileShare.Read)))
                templateWriter.Write(indexFileStreamWriter, "Home", codeMapAssemblyDeclaration);
        }

        private static void WriteDocumentation(Arguments arguments, Assembly assembly, params AssemblyDocumentationAddition[] assemblyDocumentationAdditions)
        {
            var assemblyDeclaration = DeclarationNode
                .Create(assembly)
                .Apply(assemblyDocumentationAdditions);

            var memberFileNameResolver = new DefaultMemberReferenceResolver(assembly, "netstandard-2.1");
            var templateWriter = new CodeMapHandlebarsTemplateWriter(memberFileNameResolver);

            var documentationDirectory = new DirectoryInfo(arguments.OutputPath).CreateSubdirectory(assemblyDeclaration.Name);
            var targetDirectory = documentationDirectory.CreateSubdirectory(!string.IsNullOrWhiteSpace(arguments.TargetSubdirectory) ? arguments.TargetSubdirectory : assemblyDeclaration.Version.ToSemver());

            targetDirectory.Delete(true);
            targetDirectory.Create();

            assemblyDeclaration.Accept(new FileTemplateWriterDeclarationNodeVisitor(targetDirectory, memberFileNameResolver, templateWriter));
        }

        private static void _UpdateFiles(Arguments arguments)
        {
            var codeMapAssembly = typeof(DeclarationNode).Assembly;
            var codeMapAssemblyDeclaration = DeclarationNode.Create(codeMapAssembly);
            var templateWriter = new CodeMapHandlebarsTemplateWriter(new DefaultMemberReferenceResolver(codeMapAssembly, "netstandard-2.1"));

            var outputDirectory = new DirectoryInfo(arguments.OutputPath);
            var codeMapDirectory = outputDirectory.CreateSubdirectory("CodeMap");
            var codeMapHandlebarsDirectory = outputDirectory.CreateSubdirectory("CodeMap.Handlebars");

            var directories = codeMapDirectory
                .GetDirectories()
                .Concat(codeMapHandlebarsDirectory.GetDirectories())
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
                            htmlDocument.CreateTextNode(ApplyNavigation(htmlFile, codeMapDirectory, codeMapHandlebarsDirectory)),
                            navigationHtmlNode
                        );

                    var deprecationNoticeHtmlNode = htmlDocument.GetElementbyId("deprecationNotice");
                    if (deprecationNoticeHtmlNode != null)
                        if (_IsDocumentationSelected(codeMapDirectory, htmlFile))
                            deprecationNoticeHtmlNode.ParentNode.ReplaceChild(
                                htmlDocument.CreateTextNode(ApplyDeprecationNotice(codeMapDirectory, htmlFile, _GetVersions(codeMapDirectory).LastOrDefault())),
                                deprecationNoticeHtmlNode
                            );
                        else if (_IsDocumentationSelected(codeMapHandlebarsDirectory, htmlFile))
                            deprecationNoticeHtmlNode.ParentNode.ReplaceChild(
                                htmlDocument.CreateTextNode(ApplyDeprecationNotice(codeMapHandlebarsDirectory, htmlFile, _GetVersions(codeMapHandlebarsDirectory).LastOrDefault())),
                                deprecationNoticeHtmlNode
                            );

                    htmlDocument.Save(htmlFile.FullName);
                }
            );

            string ApplyNavigation(FileInfo htmlFile, DirectoryInfo codeMapDirectory, DirectoryInfo codeMapHandlebarsDirectory)
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
                        CodeMapVersions = from codeMapVersion in _GetVersions(codeMapDirectory).Reverse()
                                          select new
                                          {
                                              IsSelected = _IsSelectedVersion(codeMapDirectory, htmlFile, codeMapVersion),
                                              Label = codeMapVersion,
                                              Path = isHomePageSelected
                                                  ? $"{codeMapDirectory.Name}/{codeMapVersion}/index.html"
                                                  : $"../../{codeMapDirectory.Name}/{codeMapVersion}/index.html"
                                          },
                        IsCodeMapSelected = _IsDocumentationSelected(codeMapDirectory, htmlFile),
                        CodeMapHandlebarsVersions = from codeMapHandlebarsVersion in _GetVersions(codeMapHandlebarsDirectory).Reverse()
                                                    select new
                                                    {
                                                        IsSelected = _IsSelectedVersion(codeMapHandlebarsDirectory, htmlFile, codeMapHandlebarsVersion),
                                                        Label = codeMapHandlebarsVersion,
                                                        Path = isHomePageSelected
                                                            ? $"{codeMapHandlebarsDirectory.Name}/{codeMapHandlebarsVersion}/index.html"
                                                            : $"../../{codeMapHandlebarsDirectory.Name}/{codeMapHandlebarsVersion}/index.html"
                                                    },
                        IsCodeMapHandlebarsSelected = _IsDocumentationSelected(codeMapHandlebarsDirectory, htmlFile)
                    });
            }

            string ApplyDeprecationNotice(DirectoryInfo documentationDirectory, FileInfo htmlFile, string latestVersion)
                => templateWriter.Apply(
                    "DeprecationNotice",
                    new
                    {
                        IsLatest = latestVersion is null || _IsSelectedVersion(documentationDirectory, htmlFile, latestVersion),
                        PathToLatest = $"../{latestVersion}/index.html"
                    });
        }

        private static IEnumerable<string> _GetVersions(DirectoryInfo documentationDirectory)
            => documentationDirectory
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

        private static bool _IsSelectedVersion(DirectoryInfo documentationFolder, FileInfo htmlFile, string version)
            => string.Equals(version, htmlFile.Directory.Name, StringComparison.OrdinalIgnoreCase)
            && _IsDocumentationSelected(documentationFolder, htmlFile);

        private static bool _IsDocumentationSelected(DirectoryInfo documentationFolder, FileInfo htmlFile)
            => string.Equals(documentationFolder.FullName, htmlFile.Directory.Parent.FullName, StringComparison.OrdinalIgnoreCase);
    }
}