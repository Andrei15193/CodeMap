using System.IO;
using System.Text.RegularExpressions;
using CodeMap.DeclarationNodes;
using CodeMap.Handlebars;

namespace CodeMap.Documentation
{
    public class FileTemplateWriterDeclarationNodeVisitor : TemplateWriterDeclarationNodeVisitor
    {
        public FileTemplateWriterDeclarationNodeVisitor(DirectoryInfo targetDirectory, IMemberFileNameResolver memberFileNameResolver, TemplateWriter templateWriter)
            : base(templateWriter)
            => (DirectoryInfo, MemberFileNameResolver) = (targetDirectory, memberFileNameResolver);

        protected DirectoryInfo DirectoryInfo { get; }

        protected IMemberFileNameResolver MemberFileNameResolver { get; }

        protected override TextWriter GetTextWriter(DeclarationNode declarationNode)
            => new StreamWriter(new FileStream(Path.Combine(DirectoryInfo.FullName, MemberFileNameResolver.GetFileName(declarationNode)), FileMode.Create, FileAccess.Write));

        protected override void VisitAssembly(AssemblyDeclaration assembly)
        {
            _ClearFolder();

            _WriteAssets();

            base.VisitAssembly(assembly);
        }

        private void _ClearFolder()
        {
            if (DirectoryInfo.Exists)
                DirectoryInfo.Delete(true);
            DirectoryInfo.Create();
        }

        private void _WriteAssets()
        {
            foreach (var resource in typeof(Program).Assembly.GetManifestResourceNames())
            {
                var match = Regex.Match(resource, @"Assets\.(?<fileName>\w+\.\w+)$", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    using var fileStream = new FileStream(Path.Combine(DirectoryInfo.FullName, match.Groups["fileName"].Value), FileMode.Create, FileAccess.Write);
                    using var assetStream = typeof(Program).Assembly.GetManifestResourceStream(resource);
                    assetStream.CopyTo(fileStream);
                }
            }
        }
    }
}