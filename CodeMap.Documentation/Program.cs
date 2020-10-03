using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CodeMap.DeclarationNodes;
using CodeMap.DocumentationElements;
using CodeMap.Handlebars;
using HtmlAgilityPack;

namespace CodeMap.Documentation
{
    internal static class Program
    {
        internal static void Main(string[] args)
        {
            var arguments = Arguments.GetFrom(args);
            if (arguments.OutputPath == null)
                throw new ArgumentException("Expected -OutputPath", nameof(args));

            var library = typeof(DocumentationElement).Assembly;
            var documentation = DeclarationNode
                .Create(library)
                .Apply(new DocumentationAdditions.Version1_0.CodeMapAssemblyDocumentationAddition());

            var outputDirectory = new DirectoryInfo(arguments.OutputPath);
            outputDirectory.Create();

            var hasExplicitTargetDirectory = !string.IsNullOrWhiteSpace(arguments.TargetSubdirectory);
            var targetDirectory = outputDirectory.CreateSubdirectory(hasExplicitTargetDirectory ? arguments.TargetSubdirectory : documentation.Version.ToSemver());

            var memberFileNameResolver = new DefaultMemberReferenceResolver(library);
            var templateWriter = new HandlebarsTemplateWriter(memberFileNameResolver);

            documentation.Accept(new FileTemplateWriterDeclarationNodeVisitor(targetDirectory, memberFileNameResolver, templateWriter));

            if (!hasExplicitTargetDirectory)
                _UpdateFiles(outputDirectory, templateWriter);
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

        private static void _UpdateFiles(DirectoryInfo outputDirectoryInfo, TemplateWriter templateWriter)
        {
            var directories = outputDirectoryInfo
                .GetDirectories()
                .Where(directory => Regex.IsMatch(directory.Name, @"^\d+\.\d+\.\d+(-(alpha|beta|rc)\d+)?$", RegexOptions.IgnoreCase))
                .OrderBy(directory => directory.Name, Comparer<string>.Create(_VersionComparison))
                .ToList();

            Parallel.ForEach(
                directories.Last().EnumerateFiles(),
                latestVersionFile => latestVersionFile.CopyTo(Path.Combine(outputDirectoryInfo.FullName, latestVersionFile.Name), true)
            );

            var versions = directories.Select(directory => directory.Name);
            Parallel.ForEach(
                Enumerable
                    .Repeat(outputDirectoryInfo, 1)
                    .Concat(directories)
                    .SelectMany(direcotry => direcotry.EnumerateFiles("*.html", SearchOption.TopDirectoryOnly)),
                htmlFile =>
                {
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.Load(htmlFile.FullName);

                    var otherVersionsHtmlNode = htmlDocument.GetElementbyId("otherVersions");
                    otherVersionsHtmlNode.ParentNode.ReplaceChild(
                        htmlDocument.CreateTextNode(templateWriter.Apply(
                            "OtherVersions",
                            new
                            {
                                IsLatestSelected = IsLatest(htmlFile),
                                PathToLatest = IsLatest(htmlFile) ? "index.html" : "../index.html",
                                HasOtherVersions = versions.Skip(1).Any(),
                                Versions = from version in versions.AsEnumerable().Reverse()
                                           select new
                                           {
                                               IsSelected = IsSelectedVersion(htmlFile, version),
                                               Label = version,
                                               Path = IsLatest(htmlFile) ? $"{version}/index.Html" : $"../{version}/index.Html"
                                           }
                            })),
                        otherVersionsHtmlNode
                    );

                    var deprecationNoticeHtmlNode = htmlDocument.GetElementbyId("deprecationNotice");
                    deprecationNoticeHtmlNode.ParentNode.ReplaceChild(
                        htmlDocument.CreateTextNode(templateWriter.Apply(
                            "DeprecationNotice",
                            new
                            {
                                IsLatest = IsLatest(htmlFile) || IsSelectedVersion(htmlFile, versions.Last()),
                                PathToLatest = "../index.html"
                            })),
                        deprecationNoticeHtmlNode
                    );

                    htmlDocument.Save(htmlFile.FullName);
                }
            );

            bool IsLatest(FileInfo htmlFile)
                => string.Equals(htmlFile.DirectoryName, outputDirectoryInfo.FullName, StringComparison.OrdinalIgnoreCase);

            bool IsSelectedVersion(FileInfo htmlFile, string version)
                => string.Equals(version, htmlFile.Directory.Name, StringComparison.OrdinalIgnoreCase);
        }

        private class Arguments
        {
            public static Arguments GetFrom(IEnumerable<string> args)
            {
                var result = new Arguments();
                string name = null;
                foreach (var arg in args)
                    if (arg.StartsWith('-'))
                        name = arg.Substring(1);
                    else if (name != null)
                    {
                        var property = typeof(Arguments).GetRuntimeProperty(name);
                        if (property != null)
                            property.SetValue(result, arg);
                        name = null;
                    }

                return result;
            }

            public string OutputPath { get; set; }

            public string TargetSubdirectory { get; set; }
        }
    }
}