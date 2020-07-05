using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Documentation.Additions;
using CodeMap.DocumentationElements;

namespace CodeMap.Documentation
{
    internal static class Program
    {
        internal static void Main(string[] args)
        {
            var arguments = Arguments.GetFrom(args);
            if (arguments.OutputPath == null)
                throw new ArgumentException("Expected -OutputPath", nameof(args));

            var documentation = DeclarationNode
                .Create(typeof(DocumentationElement).Assembly)
                .Apply(new AssemblyDocumentationAddition_1_0());

            var outputDirectory = new DirectoryInfo(arguments.OutputPath);
            outputDirectory.Create();

            documentation.Accept(new HtmlWriterDeclarationNodeVisitor(outputDirectory, new MemberFileNameProvider()));
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
        }
    }
}