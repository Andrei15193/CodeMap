using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CodeMap.DeclarationNodes;
using CodeMap.Handlebars;
using EmbeddedResourceBrowser;

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
                "Bootstrap_Jekyll",
                memberReferenceResolver: new MemberReferenceResolver(
                    new Dictionary<Assembly, IMemberReferenceResolver>
                    {
                        { typeof(DeclarationNode).Assembly, new CodeMapMemberReferenceResolver($"../../{arguments.TargetSubdirectory}/{CodeMapDirectoryName}/") },
                        { typeof(HandlebarsTemplateWriter).Assembly, new CodeMapMemberReferenceResolver($"../../{arguments.TargetSubdirectory}/{CodeMapHandlebarsDirectoryName}/") },
                        { typeof(EmbeddedDirectory).Assembly, new CodeMapMemberReferenceResolver("https://andrei15193.github.io/EmbeddedResourceBrowser/") }
                    },
                    new MicrosoftDocsMemberReferenceResolver("netstandard-2.1")
                )
            );

            var codeMapAssemblyDeclaration = DeclarationNode
                .Create(typeof(DeclarationNode).Assembly)
                .Apply(new DocumentationAdditions.Version1_0.CodeMapAssemblyDocumentationAddition());

            codeMapAssemblyDeclaration
                .Accept(new HandlebarsWriterDeclarationNodeVisitor(_GetTargetDirectory(arguments, CodeMapDirectoryName), templateWriter));
            DeclarationNode
                .Create(typeof(HandlebarsTemplateWriter).Assembly)
                .Apply(new DocumentationAdditions.Version1_0.CodeMapHandlerbarsAssemblyDocumentationAddition())
                .Accept(new HandlebarsWriterDeclarationNodeVisitor(_GetTargetDirectory(arguments, CodeMapHandlebarsDirectoryName), templateWriter));

            templateWriter.Extras?.CopyToRecursively(Directory.CreateDirectory(arguments.OutputPath));
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

        private static IEnumerable<string> _GetVersions(DirectoryInfo outputDirectory)
        {
            return outputDirectory
                .GetDirectories()
                .Where(_IsVersionDirectory)
                .OrderBy(directory => directory.Name, Comparer<string>.Create(_VersionComparison))
                .Select(directory => directory.Name)
                .ToList();

            static bool _IsVersionDirectory(DirectoryInfo directory)
               => Regex.IsMatch(directory.Name, @"^\d+\.\d+\.\d+(-(alpha|beta|rc)\d+)?$", RegexOptions.IgnoreCase);

            static int _VersionComparison(string left, string right)
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
        }
    }
}