using System;
using System.IO;
using System.Text;
using CodeMap.DeclarationNodes;
using CodeMap.Html;

namespace CodeMap.Documentation
{
    internal static class Program
    {
        internal static void Main(params string[] args)
        {
            var arguments = Arguments.GetFrom(args);

            var codeMapAssemblyDeclaration = DeclarationNode
                .Create(typeof(DeclarationNode).Assembly)
                .Apply(new DocumentationAdditions.Version1_0.CodeMapAssemblyDocumentationAddition());

            var defaultMemberReferenceResolver = new MicrosoftDocsMemberReferenceResolver("netstandard-2.1", "en-US");

            var outputFileInfo = new FileInfo(arguments.OutputFilePath);
            outputFileInfo.Directory.Create();
            using var outputFileStream = new FileStream(outputFileInfo.FullName, FileMode.Create, FileAccess.Write, FileShare.Read);
            using var outputFileStreamWriter = new StreamWriter(outputFileStream, Encoding.UTF8);

            var htmlWriterDeclarationNodeVisitor = new CodeMalHtmlWriterDocumentaitonNodeVisitor(
                outputFileStreamWriter,
                new MemberReferenceResolver(defaultMemberReferenceResolver)
                {
                    { typeof(DeclarationNode).Assembly, MemberReferenceResolver.Create(memberReference => $"#{Uri.EscapeDataString(memberReference.GetFullNameReference())}") }
                }
            );

            codeMapAssemblyDeclaration.Accept(htmlWriterDeclarationNodeVisitor);
        }
    }
}