using System;
using System.IO;
using CodeMap.DeclarationNodes;
using CodeMap.Documentation;
using CodeMap.Handlebars;
using CodeMap.Tests.Data.Documentation.DocumentationAdditions;

namespace CodeMap.Tests.Data.Documentation
{
    internal static class Program
    {
        internal static void Main(params string[] args)
        {
            var arguments = Arguments.GetFrom(args);
            if (arguments.OutputPath == null)
                throw new ArgumentException("Expected -OutputPath", nameof(args));
            if (arguments.TargetSubdirectory == null)
                throw new ArgumentException("Expected -TargetSubdirectory", nameof(args));

            var library = typeof(GlobalTestClass).Assembly;
            var documentation = DeclarationNode.Create(library, DeclarationFilter.All).Apply(new TestDataAssemblyDocumentationAddition());

            var outputDirectory = new DirectoryInfo(arguments.OutputPath);
            outputDirectory.Create();

            var targetDirectory = outputDirectory.CreateSubdirectory(arguments.TargetSubdirectory);

            var memberFileNameResolver = new DefaultMemberReferenceResolver(library, "netcore-3.1");
            var templateWriter = new TestDataHandlebarsTemplateWriter(memberFileNameResolver);

            documentation.Accept(new FileTemplateWriterDeclarationNodeVisitor(targetDirectory, memberFileNameResolver, templateWriter));
        }
    }
}