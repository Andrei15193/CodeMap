using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Handlebars;
using CodeMap.Handlebars.Helpers;

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

            var templateWriter = new HandlebarsTemplateWriter(
                "Bootstrap_Jekyll@4.5.0",
                memberReferenceResolver: new MemberReferenceResolver(
                    new Dictionary<Assembly, IMemberReferenceResolver>
                    {
                        { typeof(DeclarationNode).Assembly, new CodeMapMemberReferenceResolver($"../../{_GetSemverVersion(typeof(DeclarationNode).Assembly)}/{CodeMapDirectoryName}/") },
                        { typeof(HandlebarsTemplateWriter).Assembly, new CodeMapMemberReferenceResolver($"../../{_GetSemverVersion(typeof(HandlebarsTemplateWriter).Assembly)}/{CodeMapHandlebarsDirectoryName}/") }
                    },
                    new MicrosoftDocsMemberReferenceResolver("netstandard-2.1")
                )
            );

            var outputDirectoryInfo = new DirectoryInfo(arguments.OutputPath);
            DeclarationNode
                .Create(typeof(DeclarationNode).Assembly)
                .Apply(new DocumentationAdditions.Version1_0.CodeMapAssemblyDocumentationAddition())
                .Accept(new HandlebarsWriterDeclarationNodeVisitor(outputDirectoryInfo, templateWriter)
                {
                    DocumentationTargetDirectory = outputDirectoryInfo
                        .CreateSubdirectory(CodeMapDirectoryName)
                        .CreateSubdirectory(_GetSemverVersion(typeof(DeclarationNode).Assembly))
                });
            DeclarationNode
                .Create(typeof(HandlebarsTemplateWriter).Assembly)
                .Apply(new DocumentationAdditions.Version1_0.CodeMapHandlerbarsAssemblyDocumentationAddition())
                .Accept(new HandlebarsWriterDeclarationNodeVisitor(outputDirectoryInfo, templateWriter)
                {
                    DocumentationTargetDirectory = outputDirectoryInfo
                        .CreateSubdirectory(CodeMapHandlebarsDirectoryName)
                        .CreateSubdirectory(_GetSemverVersion(typeof(HandlebarsTemplateWriter).Assembly))
                });
        }

        private static string _GetSemverVersion(Assembly assembly)
            => Semver.ToSemver(new Version(assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? assembly.GetCustomAttribute<AssemblyVersionAttribute>()?.Version ?? "1.0.0.0"));
    }
}