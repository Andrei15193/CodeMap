using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
            if (string.IsNullOrWhiteSpace(arguments.OutputPath))
                throw new ArgumentException("Expected -OutputPath", nameof(args));
            if (string.IsNullOrWhiteSpace(arguments.TargetSubdirectory))
                throw new ArgumentException("Expected -TargetSubdirectory", nameof(args));

            DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly, DeclarationFilter.All)
                .Apply(new TestDataAssemblyDocumentationAddition())
                .Accept(
                    new HandlebarsWriterDeclarationNodeVisitor(
                        Directory.CreateDirectory(arguments.OutputPath).CreateSubdirectory(arguments.TargetSubdirectory),
                        new HandlebarsTemplateWriter(
                            new MemberReferenceResolver(
                                new Dictionary<Assembly, IMemberReferenceResolver>
                                {
                                    { typeof(GlobalTestClass).Assembly, new CodeMapMemberReferenceResolver() }
                                },
                                new MicrosoftDocsMemberReferenceResolver("netcore-3.1")
                            )
                        )
                    )
                );
        }
    }
}