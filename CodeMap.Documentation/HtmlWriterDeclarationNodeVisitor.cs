using CodeMap.DeclarationNodes;
using HtmlAgilityPack;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CodeMap.Documentation
{
    public class HtmlWriterDeclarationNodeVisitor : DeclarationNodeVisitor
    {
        private readonly DirectoryInfo _directoryInfo;

        public HtmlWriterDeclarationNodeVisitor(DirectoryInfo directoryInfo)
            => _directoryInfo = directoryInfo;

        protected override void VisitAssembly(AssemblyDeclaration assembly)
        {
            _WriteAssets();

            _directoryInfo.WritePage("Index.html", assembly);

            foreach (var @namespace in assembly.Namespaces)
                if (@namespace.DeclaredTypes.Any(declaredType => declaredType.AccessModifier == AccessModifier.Public))
                    @namespace.Accept(this);

            _UpdateVersions(assembly);
            _CopyLatestToSpecificVersion(assembly);
        }

        protected override void VisitNamespace(NamespaceDeclaration @namespace)
        {
            _directoryInfo.WritePage($"{@namespace.Name}.html", @namespace);

            foreach (var type in @namespace.DeclaredTypes.Where(declaredType => declaredType.AccessModifier == AccessModifier.Public))
                type.Accept(this);
        }

        protected override void VisitEnum(EnumDeclaration @enum)
            => _directoryInfo.WritePage($"{@enum.GetMemberFullName()}.html", @enum);

        protected override void VisitDelegate(DelegateDeclaration @delegate)
            => throw new NotImplementedException();

        protected override void VisitInterface(InterfaceDeclaration @interface)
            => throw new NotImplementedException();

        protected override void VisitClass(ClassDeclaration @class)
        {
            _directoryInfo.WritePage($"{@class.GetMemberFullName()}.html", @class);

            //foreach (var member in @class.Members.Where(member => member.AccessModifier == AccessModifier.Public))
            //    member.Accept(this);
        }

        protected override void VisitStruct(StructDeclaration @struct)
            => throw new NotImplementedException();

        protected override void VisitConstant(ConstantDeclaration constant)
        {
            throw new NotImplementedException();
        }

        protected override void VisitField(FieldDeclaration field)
        {
            throw new NotImplementedException();
        }

        protected override void VisitConstructor(ConstructorDeclaration constructor)
        {
            throw new NotImplementedException();
        }

        protected override void VisitEvent(EventDeclaration @event)
        {
            throw new NotImplementedException();
        }

        protected override void VisitProperty(PropertyDeclaration property)
        {
            throw new NotImplementedException();
        }

        protected override void VisitMethod(MethodDeclaration method)
        {
            throw new NotImplementedException();
        }

        private void _WriteAssets()
        {
            foreach (var resource in typeof(Program).Assembly.GetManifestResourceNames())
            {
                var match = Regex.Match(resource, @"Assets\.(?<fileName>\w+\.\w+)$", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    using var fileStream = new FileStream(Path.Combine(_directoryInfo.FullName, match.Groups["fileName"].Value), FileMode.Create, FileAccess.Write);
                    using var assetStream = typeof(Program).Assembly.GetManifestResourceStream(resource);
                    assetStream.CopyTo(fileStream);
                }
            }
        }

        private void _CopyLatestToSpecificVersion(AssemblyDeclaration assembly)
        {
            var currentVersionDirectory = _directoryInfo.CreateSubdirectory(assembly.Version.ToSemver());
            foreach (var currentVersionFile in _directoryInfo.GetFiles())
                currentVersionFile.CopyTo(Path.Combine(currentVersionDirectory.FullName, currentVersionFile.Name), true);
        }

        private void _UpdateVersions(AssemblyDeclaration assembly)
        {
            foreach (var htmlFile in _directoryInfo.GetFiles("*.html", SearchOption.AllDirectories))
            {
                var htmlDocument = new HtmlDocument();
                htmlDocument.Load(htmlFile.FullName);
                var otherVersionsHtmlNode = htmlDocument.GetElementbyId("otherVersions");
                otherVersionsHtmlNode.ChildNodes.Clear();

                var otherVersions = _directoryInfo.GetDirectories().Where(subdirectory => subdirectory.Name != assembly.Version.ToSemver());
                if (otherVersions.Any())
                    otherVersionsHtmlNode
                        .SetClass("btn-group")
                        .SetAttribute("role", "group")
                        .AddChild("button").SetAttribute("id", "otherVersionsButtonGroup").SetAttribute("type", "button").SetClass("btn btn-link dropdown-toggle").SetAttribute("data-toggle", "dropdown").SetAttribute("aria-haspopup", "true").SetAttribute("aria-expanded", "false").AppendText("Other Versions")
                            .ParentNode
                        .AddChild("div").SetClass("dropdown-menu").SetAttribute("aria-labelledby", "otherVersionsButtonGroup")
                            .Aggregate(
                                otherVersions,
                                (otherVersionsElement, otherVersion) => otherVersionsElement
                                    .AddChild("a").SetClass("dropdown-item").SetAttribute("href", otherVersion.Name)
                                        .AppendText(otherVersion.Name)
                            );
                htmlDocument.Save(htmlFile.FullName);
            }
        }
    }
}