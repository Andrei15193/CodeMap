using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CodeMap.DeclarationNodes;
using HtmlAgilityPack;

namespace CodeMap.Documentation
{
    public class HtmlWriterDeclarationNodeVisitor : DeclarationNodeVisitor
    {
        private readonly DirectoryInfo _directoryInfo;
        private readonly MemberFileNameProvider _memberFileNameProvider;

        public HtmlWriterDeclarationNodeVisitor(DirectoryInfo directoryInfo, MemberFileNameProvider memberFileNameProvider)
            => (_directoryInfo, _memberFileNameProvider) = (directoryInfo, memberFileNameProvider);

        protected override void VisitAssembly(AssemblyDeclaration assembly)
        {
            _ClearFolders(assembly);

            _WriteAssets(assembly);
            _directoryInfo
                .CreateSubdirectory(assembly.Version.ToSemver())
                .WritePage("Index.html", new PageContext(_memberFileNameProvider, assembly));

            foreach (var @namespace in assembly.Namespaces)
                if (@namespace.DeclaredTypes.Any(declaredType => declaredType.AccessModifier >= AccessModifier.Family))
                    @namespace.Accept(this);

            _UpdateFiles(assembly);
        }

        protected override void VisitNamespace(NamespaceDeclaration @namespace)
        {
            _directoryInfo
                .CreateSubdirectory(@namespace.Assembly.Version.ToSemver())
                .WritePage($"{_memberFileNameProvider.GetFileName(@namespace)}.html", new PageContext(_memberFileNameProvider, @namespace));

            foreach (var type in @namespace.DeclaredTypes.Where(declaredType => declaredType.AccessModifier >= AccessModifier.Family))
                type.Accept(this);
        }

        protected override void VisitEnum(EnumDeclaration @enum)
            => _directoryInfo
                .CreateSubdirectory(@enum.Assembly.Version.ToSemver())
                .WritePage($"{_memberFileNameProvider.GetFileName(@enum)}.html", new PageContext(_memberFileNameProvider, @enum));

        protected override void VisitDelegate(DelegateDeclaration @delegate)
            => throw new NotImplementedException();

        protected override void VisitInterface(InterfaceDeclaration @interface)
            => throw new NotImplementedException();

        protected override void VisitClass(ClassDeclaration @class)
        {
            _directoryInfo
                .CreateSubdirectory(@class.Assembly.Version.ToSemver())
                .WritePage($"{_memberFileNameProvider.GetFileName(@class)}.html", new PageContext(_memberFileNameProvider, @class));

            foreach (var member in @class.Members.Where(member => member.AccessModifier >= AccessModifier.Family))
                member.Accept(this);
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
            => _directoryInfo
                .CreateSubdirectory(constructor.DeclaringType.Assembly.Version.ToSemver())
                .WritePage($"{_memberFileNameProvider.GetFileName(constructor)}.html", new PageContext(_memberFileNameProvider, constructor));

        protected override void VisitEvent(EventDeclaration @event)
        {
            throw new NotImplementedException();
        }

        protected override void VisitProperty(PropertyDeclaration property)
            => _directoryInfo
                .CreateSubdirectory(property.DeclaringType.Assembly.Version.ToSemver())
                .WritePage($"{_memberFileNameProvider.GetFileName(property)}.html", new PageContext(_memberFileNameProvider, property));

        protected override void VisitMethod(MethodDeclaration method)
            => _directoryInfo
                .CreateSubdirectory(method.DeclaringType.Assembly.Version.ToSemver())
                .WritePage($"{_memberFileNameProvider.GetFileName(method)}.html", new PageContext(_memberFileNameProvider, method));

        private void _ClearFolders(AssemblyDeclaration assembly)
        {
            if (_directoryInfo.Exists)
            {
                foreach (var file in _directoryInfo.GetFiles())
                    file.Delete();

                var versionSubdirectory = _directoryInfo.CreateSubdirectory(assembly.Version.ToSemver());
                versionSubdirectory.Delete(true);
                versionSubdirectory.Create();
            }
        }

        private void _WriteAssets(AssemblyDeclaration assembly)
        {
            foreach (var resource in typeof(Program).Assembly.GetManifestResourceNames())
            {
                var match = Regex.Match(resource, @"Assets\.(?<fileName>\w+\.\w+)$", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    using var fileStream = new FileStream(Path.Combine(_directoryInfo.FullName, assembly.Version.ToSemver(), match.Groups["fileName"].Value), FileMode.Create, FileAccess.Write);
                    using var assetStream = typeof(Program).Assembly.GetManifestResourceStream(resource);
                    assetStream.CopyTo(fileStream);
                }
            }
        }

        private static int _VersionComparison(string left, string right)
        {
            var leftBaseVersion = left.Split(new[] { '-' }, 2).First();
            var rightBaseVersion = right.Split(new[] { '-' }, 2).First();
            var baseComparation = string.Compare(leftBaseVersion, rightBaseVersion, StringComparison.OrdinalIgnoreCase);

            if (baseComparation == 0)
                if (left == leftBaseVersion)
                    return 1;
                else if (right == rightBaseVersion)
                    return -1;
                else
                    return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
            else
                return baseComparation;
        }

        private void _UpdateFiles(AssemblyDeclaration assembly)
        {
            var directories = _directoryInfo.GetDirectories().OrderBy(directory => directory.Name, Comparer<string>.Create(_VersionComparison)).ToList();

            Parallel.ForEach(
                directories.Last().EnumerateFiles(),
                latestVersionFile => latestVersionFile.CopyTo(Path.Combine(_directoryInfo.FullName, latestVersionFile.Name), true)
            );

            var versions = directories.Select(directory => directory.Name);
            Parallel.ForEach(
                _directoryInfo.EnumerateFiles("*.html", SearchOption.AllDirectories),
                htmlFile =>
                {
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.Load(htmlFile.FullName);

                    var otherVersionsHtmlNode = htmlDocument.GetElementbyId("otherVersions");
                    otherVersionsHtmlNode.ParentNode.ReplaceChild(
                        htmlDocument.CreateTextNode(
                            HandlebarsExtensions.ApplyTemplate(
                                "OtherVersions",
                                new
                                {
                                    IsLatestSelected = IsLatest(htmlFile),
                                    PathToLatest = IsLatest(htmlFile) ? "Index.html" : "../Index.html",
                                    HasOtherVersions = versions.Skip(1).Any(),
                                    Versions = from version in versions.AsEnumerable().Reverse()
                                               select new
                                               {
                                                   IsSelected = IsSelectedVersion(htmlFile, version),
                                                   Label = version,
                                                   Path = IsLatest(htmlFile) ? $"{version}/Index.Html" : $"../{version}/Index.Html"
                                               }
                                })
                        ),
                        otherVersionsHtmlNode
                    );

                    var deprecationNoticeHtmlNode = htmlDocument.GetElementbyId("deprecationNotice");
                    deprecationNoticeHtmlNode.ParentNode.ReplaceChild(
                        htmlDocument.CreateTextNode(
                            HandlebarsExtensions.ApplyTemplate(
                                "DeprecationNotice",
                                new
                                {
                                    IsLatest = IsLatest(htmlFile) || IsSelectedVersion(htmlFile, versions.Last()),
                                    PathToLatest = "../Index.html"
                                })
                        ),
                        deprecationNoticeHtmlNode
                    );

                    htmlDocument.Save(htmlFile.FullName);
                }
            );

            bool IsLatest(FileInfo htmlFile)
                => string.Equals(htmlFile.DirectoryName, _directoryInfo.FullName, StringComparison.OrdinalIgnoreCase);

            bool IsSelectedVersion(FileInfo htmlFile, string version)
                => string.Equals(version, htmlFile.Directory.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}