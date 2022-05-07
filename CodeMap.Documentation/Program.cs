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

            var themesEmbeddedDirectory = new EmbeddedDirectory(typeof(HandlebarsTemplateWriter).Assembly).Subdirectories["Themes"];

            var keyThemeDirectoriesToVisit = new Queue<ThemeEmbeddedDirectoryData>();
            keyThemeDirectoriesToVisit.Enqueue(new ThemeEmbeddedDirectoryData(
                $"https://github.com/Andrei15193/CodeMap/blob/{codeMapHandlebarsVersion}/CodeMap.Handlebars/Themes",
                outputDirectoryInfo.CreateSubdirectory("CodeMap.Handlebars").CreateSubdirectory(codeMapHandlebarsVersion).CreateSubdirectory("Themes"),
                outputDirectoryInfo.CreateSubdirectory("_includes").CreateSubdirectory("CodeMap.Handlebars").CreateSubdirectory(codeMapHandlebarsVersion).CreateSubdirectory("Themes"),
                themesEmbeddedDirectory,
                Enumerable.Empty<ThemeEmbeddedDirectoryData>()
            ));

            do
            {
                var keyThemeEmbeddedDirectoryData = keyThemeDirectoriesToVisit.Dequeue();
                foreach (var keyThemeEmbeddedSubdirectoryData in keyThemeEmbeddedDirectoryData.SubdirectoriesData)
                    if (!HandlebarsTemplateWriter.ReservedDirectoryNames.Contains(keyThemeEmbeddedSubdirectoryData.Name, StringComparer.OrdinalIgnoreCase))
                        keyThemeDirectoriesToVisit.Enqueue(keyThemeEmbeddedSubdirectoryData);

                using (var includeFileStream = new FileStream(Path.Combine(keyThemeEmbeddedDirectoryData.OutputIncludeDirectory.FullName, "files.html"), FileMode.Create, FileAccess.Write, FileShare.Read))
                using (var includeStreamWriter = new StreamWriter(includeFileStream))
                {
                    var themeEmbeddedDirectoriesData = Enumerable.Repeat(keyThemeEmbeddedDirectoryData, 1).Concat(keyThemeEmbeddedDirectoryData.Parents);
                    if (themeEmbeddedDirectoriesData.Any(themeEmbeddedDirectoryData => HandlebarsTemplateWriter.ReservedDirectoryNames.Any(themeEmbeddedDirectoryData.EmbeddedDirectory.Subdirectories.ContainsKey)))
                    {
                        if (keyThemeEmbeddedDirectoryData.EmbeddedDirectory == themesEmbeddedDirectory)
                            includeStreamWriter.WriteLine("<h3 id=\"global-files\">Global Files</h3>");
                        else
                            includeStreamWriter.WriteLine("<h3 id=\"files\">Files</h3>");

                        includeStreamWriter.WriteLine("<ul>");
                        _WriteFileListInclude(includeStreamWriter, themeEmbeddedDirectoriesData);
                        includeStreamWriter.WriteLine("</ul>");
                    }
                }

                foreach (var embeddedFile in keyThemeEmbeddedDirectoryData.EmbeddedDirectory.Files)
                    using (var embeddedFileStream = embeddedFile.OpenRead())
                    using (var outputFileStream = new FileStream(Path.Combine(keyThemeEmbeddedDirectoryData.OutputPageDirectory.FullName, embeddedFile.Name), FileMode.Create, FileAccess.Write, FileShare.Read))
                        embeddedFileStream.CopyTo(outputFileStream);
            } while (keyThemeDirectoriesToVisit.Count > 0);
        }

        private static void _WriteFileListInclude(TextWriter includeStreamWriter, IEnumerable<ThemeEmbeddedDirectoryData> keyThemeEmbeddedDirectoriesData)
        {
            foreach (var reservedDirectoryName in HandlebarsTemplateWriter.ReservedDirectoryNames)
                Write(
                    from keyThemeEmbeddedDirectoryData in keyThemeEmbeddedDirectoriesData
                    from subdirectoryData in keyThemeEmbeddedDirectoryData.SubdirectoriesData
                    where reservedDirectoryName == subdirectoryData.Name
                    select subdirectoryData
                );

            void Write(IEnumerable<ThemeEmbeddedDirectoryData> themeEmbeddedDirectoriesData, int level = 1)
            {
                if (themeEmbeddedDirectoriesData.Any())
                {
                    includeStreamWriter.WriteLine(new string(' ', 4 * level) + $"<li>" + themeEmbeddedDirectoriesData.Select(themeEmbeddedDirectoryData => themeEmbeddedDirectoryData.Name).First());

                    includeStreamWriter.WriteLine(new string(' ', 4 * (level + 1)) + "<ul>");
                    var groupedThemeEmbeddedSubdirectoriesData =
                        from themeEmbeddedDirectoryData in themeEmbeddedDirectoriesData
                        from themeEmbeddedSubdirectoryData in themeEmbeddedDirectoryData.SubdirectoriesData
                        group themeEmbeddedSubdirectoryData by themeEmbeddedSubdirectoryData.Name.ToLowerInvariant();

                    if (groupedThemeEmbeddedSubdirectoriesData.Any())
                        foreach (var embeddedSubdirectories in groupedThemeEmbeddedSubdirectoriesData)
                            Write(embeddedSubdirectories, level + 2);

                    var files =
                        from themeEmbeddedDirectoryData in themeEmbeddedDirectoriesData
                        from embeddedFile in themeEmbeddedDirectoryData.EmbeddedDirectory.Files
                        group (BaseUrl: $"{themeEmbeddedDirectoryData.GitHubBaseUrl}/{embeddedFile.Name}", EmbeddedFile: embeddedFile, IsInherited: themeEmbeddedDirectoryData.IsInherited) by embeddedFile.Name.ToLowerInvariant() into groupedFiles
                        orderby groupedFiles.First().EmbeddedFile.Name
                        select groupedFiles.First();
                    foreach (var (baseUrl, embeddedFile, isInherited) in files)
                    {
                        includeStreamWriter.Write(new string(' ', 4 * (level + 2)) + $"<li><a href=\"{baseUrl}\">{embeddedFile.Name}</a>");
                        if (isInherited)
                            includeStreamWriter.Write(" <small>(inherited)</small>");
                        includeStreamWriter.WriteLine("</li>");
                    }
                    includeStreamWriter.WriteLine(new string(' ', 4 * (level + 1)) + "</ul>");

                    includeStreamWriter.WriteLine(new string(' ', 4 * level) + "</li>");
                }
            }
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

        private record ThemeEmbeddedDirectoryData(string GitHubBaseUrl, DirectoryInfo OutputPageDirectory, DirectoryInfo OutputIncludeDirectory, EmbeddedDirectory EmbeddedDirectory, IEnumerable<ThemeEmbeddedDirectoryData> Parents)
        {
            public string Name
                => EmbeddedDirectory.Name;

            public bool IsInherited { get; init; }

            public IEnumerable<ThemeEmbeddedDirectoryData> SubdirectoriesData
            {
                get
                {
                    foreach (var subdirectory in EmbeddedDirectory.Subdirectories)
                        yield return GetSubdirectory(subdirectory.Name);
                }
            }

            public ThemeEmbeddedDirectoryData GetSubdirectory(string subdirectoryName)
                => new ThemeEmbeddedDirectoryData(
                    $"{GitHubBaseUrl}/{subdirectoryName}",
                    OutputPageDirectory.CreateSubdirectory(subdirectoryName),
                    OutputIncludeDirectory.CreateSubdirectory(subdirectoryName.Replace(' ', '-').Replace('+', '_')),
                    EmbeddedDirectory.Subdirectories[subdirectoryName],
                    Enumerable.Repeat(this, 1).Concat(Parents).Select(parent => parent with { IsInherited = true })
                )
                {
                    IsInherited = this.IsInherited
                };
        }
    }
}