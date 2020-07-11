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
        private readonly string _targetFolder;
        private readonly MemberFileNameResolver _memberFileNameResolver;

        public HtmlWriterDeclarationNodeVisitor(DirectoryInfo directoryInfo, string targetFolder, MemberFileNameResolver memberFileNameResolver)
            => (_directoryInfo, _targetFolder, _memberFileNameResolver) = (directoryInfo, targetFolder, memberFileNameResolver);

        protected override void VisitAssembly(AssemblyDeclaration assembly)
        {
            _ClearFolders(assembly);

            _WriteAssets(assembly);
            _GetTargetSubdirectory(assembly)
                .WritePage("index.html", new PageContext(_memberFileNameResolver, assembly));

            foreach (var @namespace in assembly.Namespaces)
                if (@namespace.DeclaredTypes.Any(declaredType => declaredType.AccessModifier >= AccessModifier.Family))
                    @namespace.Accept(this);

            if (string.IsNullOrWhiteSpace(_targetFolder))
                _UpdateFiles();
        }

        protected override void VisitNamespace(NamespaceDeclaration @namespace)
        {
            _GetTargetSubdirectory(@namespace.Assembly)
                .WritePage(_memberFileNameResolver.GetFileName(@namespace), new PageContext(_memberFileNameResolver, @namespace));

            foreach (var type in @namespace.DeclaredTypes.Where(declaredType => declaredType.AccessModifier >= AccessModifier.Family))
                type.Accept(this);
        }

        protected override void VisitEnum(EnumDeclaration @enum)
            => _GetTargetSubdirectory(@enum.Assembly)
                .WritePage(_memberFileNameResolver.GetFileName(@enum), new PageContext(_memberFileNameResolver, @enum));

        protected override void VisitDelegate(DelegateDeclaration @delegate)
            => throw new NotImplementedException();

        protected override void VisitInterface(InterfaceDeclaration @interface)
            => throw new NotImplementedException();

        protected override void VisitClass(ClassDeclaration @class)
        {
            _GetTargetSubdirectory(@class.Assembly)
                .WritePage(_memberFileNameResolver.GetFileName(@class), new PageContext(_memberFileNameResolver, @class));

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
            => _GetTargetSubdirectory(constructor.DeclaringType.Assembly)
                .WritePage(_memberFileNameResolver.GetFileName(constructor), new PageContext(_memberFileNameResolver, constructor));

        protected override void VisitEvent(EventDeclaration @event)
        {
            throw new NotImplementedException();
        }

        protected override void VisitProperty(PropertyDeclaration property)
            => _GetTargetSubdirectory(property.DeclaringType.Assembly)
                .WritePage(_memberFileNameResolver.GetFileName(property), new PageContext(_memberFileNameResolver, property));

        protected override void VisitMethod(MethodDeclaration method)
            => _GetTargetSubdirectory(method.DeclaringType.Assembly)
                .WritePage(_memberFileNameResolver.GetFileName(method), new PageContext(_memberFileNameResolver, method));

        private void _ClearFolders(AssemblyDeclaration assembly)
        {
            if (_directoryInfo.Exists)
            {
                if (string.IsNullOrWhiteSpace(_targetFolder))
                    foreach (var file in _directoryInfo.GetFiles())
                        file.Delete();

                var targetSubdirectory = _GetTargetSubdirectory(assembly);
                targetSubdirectory.Delete(true);
                targetSubdirectory.Create();
            }
        }

        private DirectoryInfo _GetTargetSubdirectory(AssemblyDeclaration assembly)
            => _directoryInfo.CreateSubdirectory(string.IsNullOrWhiteSpace(_targetFolder) ? assembly.Version.ToSemver() : _targetFolder);

        private void _WriteAssets(AssemblyDeclaration assembly)
        {
            foreach (var resource in typeof(Program).Assembly.GetManifestResourceNames())
            {
                var match = Regex.Match(resource, @"Assets\.(?<fileName>\w+\.\w+)$", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    using var fileStream = new FileStream(Path.Combine(_GetTargetSubdirectory(assembly).FullName, match.Groups["fileName"].Value), FileMode.Create, FileAccess.Write);
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

        private void _UpdateFiles()
        {
            var directories = _directoryInfo
                .GetDirectories()
                .Where(directory => Regex.IsMatch(directory.Name, @"^\d+\.\d+\.\d+(-(alpha|beta|rc)\d+)?$", RegexOptions.IgnoreCase))
                .OrderBy(directory => directory.Name, Comparer<string>.Create(_VersionComparison))
                .ToList();

            Parallel.ForEach(
                directories.Last().EnumerateFiles(),
                latestVersionFile => latestVersionFile.CopyTo(Path.Combine(_directoryInfo.FullName, latestVersionFile.Name), true)
            );

            var versions = directories.Select(directory => directory.Name);
            Parallel.ForEach(
                Enumerable
                    .Repeat(_directoryInfo, 1)
                    .Concat(directories)
                    .SelectMany(direcotry => direcotry.EnumerateFiles("*.html", SearchOption.TopDirectoryOnly)),
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
                                    PathToLatest = IsLatest(htmlFile) ? "index.html" : "../index.html",
                                    HasOtherVersions = versions.Skip(1).Any(),
                                    Versions = from version in versions.AsEnumerable().Reverse()
                                               select new
                                               {
                                                   IsSelected = IsSelectedVersion(htmlFile, version),
                                                   Label = version,
                                                   Path = IsLatest(htmlFile) ? $"{version}/index.Html" : $"../{version}/index.Html"
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
                                    PathToLatest = "../index.html"
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