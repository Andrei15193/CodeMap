using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
            if (string.IsNullOrWhiteSpace(arguments.Version))
                arguments.Version = null;

            var codeMapAssemblyDeclaration = DeclarationNode
                .Create(typeof(DeclarationNode).Assembly)
                .Apply(new DocumentationAdditions.Version1_0.CodeMapAssemblyDocumentationAddition());
            var codeMapVersion = arguments.Version ?? _ToSemver(codeMapAssemblyDeclaration.Version);

            var codeMapHandlebarsDeclaration = DeclarationNode
                .Create(typeof(HandlebarsTemplateWriter).Assembly)
                .Apply(new DocumentationAdditions.Version1_0.CodeMapHandlerbarsAssemblyDocumentationAddition());
            var codeMapHandlebarsVersion = arguments.Version ?? _ToSemver(codeMapHandlebarsDeclaration.Version);

            var templateWriter = new HandlebarsTemplateWriter(
                "GitHub Pages/Bootstrap@4.5.0",
                memberReferenceResolver: new MemberReferenceResolver(
                    new Dictionary<Assembly, IMemberReferenceResolver>
                    {
                        { typeof(DeclarationNode).Assembly, new CodeMapMemberReferenceResolver($"../../{CodeMapDirectoryName}/{codeMapVersion}/") },
                        { typeof(HandlebarsTemplateWriter).Assembly, new CodeMapMemberReferenceResolver($"../../{CodeMapHandlebarsDirectoryName}/{codeMapHandlebarsVersion}/") }
                    },
                    new MicrosoftDocsMemberReferenceResolver("netstandard-2.1")
                )
            );

            var outputDirectoryInfo = new DirectoryInfo(arguments.OutputPath);
            codeMapAssemblyDeclaration
                .Accept(new HandlebarsWriterDeclarationNodeVisitor(outputDirectoryInfo, templateWriter, new[] { "CodeMap", "1.0.0" })
                {
                    DocumentationTargetDirectory = outputDirectoryInfo
                        .CreateSubdirectory(CodeMapDirectoryName)
                        .CreateSubdirectory(codeMapVersion)
                });
            codeMapHandlebarsDeclaration
                .Accept(new HandlebarsWriterDeclarationNodeVisitor(outputDirectoryInfo, templateWriter, new[] { "CodeMap.Handlebars", "1.0.0" })
                {
                    DocumentationTargetDirectory = outputDirectoryInfo
                        .CreateSubdirectory(CodeMapHandlebarsDirectoryName)
                        .CreateSubdirectory(codeMapHandlebarsVersion)
                });

            var toVisit = new Queue<(DirectoryInfo OutputDirectory, EmbeddedDirectory EmbeddedDirectory)>(
                Enumerable.Repeat((outputDirectoryInfo.CreateSubdirectory("Themes"), templateWriter.ThemesDirectory), 1)
            );
            do
            {
                var (outputDirectory, embeddedDirectory) = toVisit.Dequeue();
                foreach (var embeddedSubdirectory in embeddedDirectory.Subdirectories)
                    if (!HandlebarsTemplateWriter.ReservedDirectoryNames.Contains(embeddedSubdirectory.Name, StringComparer.OrdinalIgnoreCase))
                        toVisit.Enqueue((outputDirectory.CreateSubdirectory(embeddedSubdirectory.Name), embeddedSubdirectory));

                foreach (var embeddedFile in embeddedDirectory.Files)
                {
                    using (var embeddedFileStream = embeddedFile.OpenRead())
                    using (var outputFileStream = new FileStream(Path.Combine(outputDirectory.FullName, embeddedFile.Name), FileMode.Create, FileAccess.Write, FileShare.Read))
                        embeddedFileStream.CopyTo(outputFileStream);
                }
            } while (toVisit.Count > 0);
        }

        private static string _ToSemver(Version version)
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
    }
}