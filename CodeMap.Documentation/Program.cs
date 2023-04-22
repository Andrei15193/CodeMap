using System;
using System.IO;
using System.Text;
using CodeMap.DeclarationNodes;
using CodeMap.Html;

namespace CodeMap.Documentation
{
    internal static class Program
    {
        private const string CodeMapDirectoryName = "CodeMap";
        private const string CodeMapHandlebarsDirectoryName = "CodeMap.Handlebars";

        internal static void Main(params string[] args)
        {
            var codeMapAssemblyDeclaration = DeclarationNode
                .Create(typeof(DeclarationNode).Assembly)
                .Apply(new DocumentationAdditions.Version1_0.CodeMapAssemblyDocumentationAddition());

            var defaultMemberReferenceResolver = new MicrosoftDocsMemberReferenceResolver("netstandard-2.1", "en-US");

            var outputDirectory = Directory.CreateDirectory(Path.Join(Environment.CurrentDirectory, "TEST Documentation", codeMapAssemblyDeclaration.Version.ToString()));
            using var outputFileStream = new FileStream(Path.Join(outputDirectory.FullName, "index.html"), FileMode.Create, FileAccess.Write, FileShare.Read);
            using var outputFileStreamWriter = new StreamWriter(outputFileStream, Encoding.UTF8);

            var htmlWriterDeclarationNodeVisitor = new HtmlWriterDeclarationNodeVisitor(
                outputFileStreamWriter,
                new MemberReferenceResolver(defaultMemberReferenceResolver)
                {
                    { typeof(DeclarationNode).Assembly, MemberReferenceResolver.Create(memberReference => $"#{memberReference.GetFullNameReference()}") }
                }
            );

            codeMapAssemblyDeclaration.Accept(htmlWriterDeclarationNodeVisitor);
        }
    }
}