using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Documentation;
using CodeMap.Handlebars;
using CodeMap.Tests.Data.Documentation.DocumentationAdditions;
using EmbeddedResourceBrowser;

namespace CodeMap.Tests.Data.Documentation
{
    internal static class Program
    {
        private static readonly IEnumerable<ThemeInfo> _themes = new[]
        {
                new ThemeInfo
                {
                    Name = "Bootstrap",
                    Description = "Generate a documentation website using the Bootstrap framework and deploy it directly. This is the easiest way to get started with CodeMap.",
                    Banner = "Bootstrap.png"
                },
                new ThemeInfo
                {
                    Name = "Bootstrap_Jekyll",
                    Description = "A Bootstrap based theme that depends on Jekyll to actually generate the final website. Allows more flexibility as the documentation is generated in HTML pages that can be further customized through Jekyll. This theme is useful if you want to have a blog for your project site or generate the navigation bar to support previous versions of your library.",
                    Banner = "Bootstrap_Jekyll.png"
                }
            };

        internal static void Main(params string[] args)
        {
            var arguments = Arguments.GetFrom(args);
            if (string.IsNullOrWhiteSpace(arguments.OutputPath))
                throw new ArgumentException("Expected -OutputPath", nameof(args));
            if (string.IsNullOrWhiteSpace(arguments.TargetSubdirectory))
                throw new ArgumentException("Expected -TargetSubdirectory", nameof(args));

            var outputDirectory = Directory.CreateDirectory(arguments.OutputPath);
            var testDataDirectory = outputDirectory.CreateSubdirectory(arguments.TargetSubdirectory);

            using (var indexFileStream = new FileStream(Path.Combine(testDataDirectory.FullName, "index.html"), FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var indexStreamWriter = new StreamWriter(indexFileStream))
            {
                var templateWriter = new HandlebarsTemplateWriter("Bootstrap_Jekyll", new CodeMapMemberReferenceResolver());
                templateWriter.Write(indexStreamWriter, "Index", _themes);
                templateWriter.Assets?.CopyToRecursively(testDataDirectory);
            }

            foreach (var theme in _themes)
            {
                var templateWriter = new HandlebarsTemplateWriter(
                    theme.Name,
                    new MemberReferenceResolver(
                        new Dictionary<Assembly, IMemberReferenceResolver>
                        {
                            { typeof(GlobalTestClass).Assembly, new CodeMapMemberReferenceResolver() }
                        },
                        new MicrosoftDocsMemberReferenceResolver("netcore-3.1")
                    )
                );
                DeclarationNode
                    .Create(typeof(GlobalTestClass).Assembly, DeclarationFilter.All)
                    .Apply(new TestDataAssemblyDocumentationAddition())
                    .Accept(new HandlebarsWriterDeclarationNodeVisitor(testDataDirectory.CreateSubdirectory(theme.Name), templateWriter));

                if (templateWriter.Extras is object)
                    foreach (var extrasSubdirectory in templateWriter.Extras.Subdirectories)
                        extrasSubdirectory.CopyTo(outputDirectory.CreateSubdirectory(extrasSubdirectory.Name).CreateSubdirectory(theme.Name));
            }
        }
    }
}