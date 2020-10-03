using System.IO;
using System.Text.RegularExpressions;
using CodeMap.DeclarationNodes;
using CodeMap.Handlebars;

namespace CodeMap.Documentation
{
    public class FileTemplateWriterDeclarationNodeVisitor : TemplateWriterDeclarationNodeVisitor
    {
        private readonly DirectoryInfo _targetDirecotry;
        private readonly IMemberReferenceResolver _memberFileNameResolver;

        public FileTemplateWriterDeclarationNodeVisitor(DirectoryInfo targetDirectory, IMemberReferenceResolver memberFileNameResolver, TemplateWriter templateWriter)
            : base(templateWriter)
            => (_targetDirecotry, _memberFileNameResolver) = (targetDirectory, memberFileNameResolver);

        protected override TextWriter GetTextWriter(DeclarationNode declarationNode)
            => new StreamWriter(new FileStream(Path.Combine(_targetDirecotry.FullName, _memberFileNameResolver.GetFileName(declarationNode)), FileMode.Create, FileAccess.Write, FileShare.Read));

        protected override void VisitAssembly(AssemblyDeclaration assembly)
        {
            _ClearFolder();

            _WriteAssets();

            base.VisitAssembly(assembly);
        }

        private void _ClearFolder()
        {
            if (_targetDirecotry.Exists)
                _targetDirecotry.Delete(true);
            _targetDirecotry.Create();
        }

        private void _WriteAssets()
        {
            foreach (var resource in typeof(Program).Assembly.GetManifestResourceNames())
            {
                var match = Regex.Match(resource, @"Assets\.(?<fileName>\w+\.\w+)$", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    using var fileStream = new FileStream(Path.Combine(_targetDirecotry.FullName, match.Groups["fileName"].Value), FileMode.Create, FileAccess.Write);
                    using var assetStream = typeof(Program).Assembly.GetManifestResourceStream(resource);
                    assetStream.CopyTo(fileStream);
                }
            }
        }
    }
}