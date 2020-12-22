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
        internal static void Main(params string[] args)
        {
            var arguments = Arguments.GetFrom(args);
            if (string.IsNullOrWhiteSpace(arguments.OutputPath))
                throw new ArgumentException("Expected -OutputPath", nameof(args));
            if (string.IsNullOrWhiteSpace(arguments.TargetSubdirectory))
                throw new ArgumentException("Expected -TargetSubdirectory", nameof(args));

            var outputDirectory = Directory.CreateDirectory(arguments.OutputPath);
            var testDataDirectory = outputDirectory.CreateSubdirectory(arguments.TargetSubdirectory);

            var themes = new[]
            {
                "Bootstrap",
                "Bootstrap_Jekyll"
            };

            using (var indexFileStream = new FileStream(Path.Combine(testDataDirectory.FullName, "index.html"), FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var indexStreamWriter = new StreamWriter(indexFileStream))
            {
                var templateWriter = new HandlebarsTemplateWriter("Bootstrap_Jekyll", new CodeMapMemberReferenceResolver());
                templateWriter.Write(indexStreamWriter, "Index", themes);
                templateWriter.Assets?.CopyToRecursively(testDataDirectory);
            }

            foreach (var theme in themes)
            {
                var templateWriter = new HandlebarsTemplateWriter(
                    theme,
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
                    .Accept(new HandlebarsWriterDeclarationNodeVisitor(testDataDirectory.CreateSubdirectory(theme), templateWriter));

                if (templateWriter.Extras is object)
                    foreach (var extrasSubdirectory in templateWriter.Extras.Subdirectories)
                        extrasSubdirectory.CopyTo(outputDirectory.CreateSubdirectory(extrasSubdirectory.Name).CreateSubdirectory(theme));
            }
        }
    }
}