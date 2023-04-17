using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

            var fileWriterDeclarationNodeVisitor = FileWriterDeclarationNodeVisitor.Create(
                declarationNode =>
                {
                    var declarationDirectoryPath = declarationNode switch
                    {
                        AssemblyDeclaration assembly => assembly.Version.ToString(),
                        NamespaceDeclaration @namespace => Path.Join(@namespace.Assembly.Version.ToString(), @namespace.GetFullNameReference()),
                        TypeDeclaration type => Path.Join(type.Assembly.Version.ToString(), type.GetFullNameReference()),
                        MemberDeclaration member => Path.Join(member.DeclaringType.Assembly.Version.ToString(), member.GetFullNameReference()),
                        _ => throw new NotImplementedException()
                    };

                    var outputDirectory = Directory.CreateDirectory(Path.Join(Environment.CurrentDirectory, "TEST Documentation", declarationDirectoryPath));

                    return new StreamWriter(
                        new FileStream(
                            Path.Join(outputDirectory.FullName, "index.html"),
                            FileMode.Create,
                            FileAccess.Write,
                            FileShare.Read
                        ),
                        Encoding.UTF8,
                        leaveOpen: false
                    );
                },
                (declarationNode, declarationTextWriter) => new HtmlWriterDeclarationNodeVisitor(
                    declarationTextWriter,
                    new MemberReferenceResolver(defaultMemberReferenceResolver)
                    {
                        { typeof(DeclarationNode).Assembly, MemberReferenceResolver.Create(memberReference => declarationNode is AssemblyDeclaration ? $"./{memberReference.GetFullNameReference()}/" : $"../{memberReference.GetFullNameReference()}/") }
                    }
                )
            );

            codeMapAssemblyDeclaration.Accept(fileWriterDeclarationNodeVisitor);

            // var arguments = Arguments.GetFrom(args);
            // if (string.IsNullOrWhiteSpace(arguments.OutputPath))
            //     throw new ArgumentException("Expected -OutputPath", nameof(args));
            // if (string.IsNullOrWhiteSpace(arguments.TargetSubdirectory))
            //     throw new ArgumentException("Expected -TargetSubdirectory", nameof(args));

            // var templateWriter = new HandlebarsTemplateWriter(
            //     "Bootstrap_Jekyll",
            //     memberReferenceResolver: new MemberReferenceResolver(
            //         new Dictionary<Assembly, IMemberReferenceResolver>
            //         {
            //             { typeof(DeclarationNode).Assembly, new CodeMapMemberReferenceResolver($"../../{arguments.TargetSubdirectory}/{CodeMapDirectoryName}/") },
            //             { typeof(HandlebarsTemplateWriter).Assembly, new CodeMapMemberReferenceResolver($"../../{arguments.TargetSubdirectory}/{CodeMapHandlebarsDirectoryName}/") },
            //             { typeof(EmbeddedDirectory).Assembly, new CodeMapMemberReferenceResolver("https://andrei15193.github.io/EmbeddedResourceBrowser/") }
            //         },
            //         new MicrosoftDocsMemberReferenceResolver("netstandard-2.1")
            //     )
            // );

            // var codeMapAssemblyDeclaration = DeclarationNode
            //     .Create(typeof(DeclarationNode).Assembly)
            //     .Apply(new DocumentationAdditions.Version1_0.CodeMapAssemblyDocumentationAddition());

            // codeMapAssemblyDeclaration
            //     .Accept(new HandlebarsWriterDeclarationNodeVisitor(_GetTargetDirectory(arguments, CodeMapDirectoryName), templateWriter));
            // DeclarationNode
            //     .Create(typeof(HandlebarsTemplateWriter).Assembly)
            //     .Apply(new DocumentationAdditions.Version1_0.CodeMapHandlerbarsAssemblyDocumentationAddition())
            //     .Accept(new HandlebarsWriterDeclarationNodeVisitor(_GetTargetDirectory(arguments, CodeMapHandlebarsDirectoryName), templateWriter));

            // foreach (var extraFile in templateWriter.Extras)
            // {
            //     var nameBuilder = new StringBuilder();
            //     var embeddedDirectory = extraFile.ParentDirectory;
            //     while (embeddedDirectory is object && !embeddedDirectory.Name.Equals("extras", StringComparison.OrdinalIgnoreCase))
            //     {
            //         nameBuilder.Insert(0, Path.DirectorySeparatorChar);
            //         nameBuilder.Insert(0, embeddedDirectory.Name);
            //         embeddedDirectory = embeddedDirectory.ParentDirectory;
            //     }
            //     Directory.CreateDirectory(Path.Combine(arguments.OutputPath, nameBuilder.ToString()));
            //     nameBuilder.Append(extraFile.Name);

            //     using (var outputFileStream = new FileStream(Path.Combine(arguments.OutputPath, nameBuilder.ToString()), FileMode.Create, FileAccess.Write, FileShare.Read))
            //     using (var extraFileStream = extraFile.OpenRead())
            //         extraFileStream.CopyTo(outputFileStream);
            // }
        }

        private static DirectoryInfo _GetTargetDirectory(Arguments arguments, string documentationDirectoryName)
        {
            var targetDirectory = new DirectoryInfo(arguments.OutputPath)
                .CreateSubdirectory(arguments.TargetSubdirectory)
                .CreateSubdirectory(documentationDirectoryName);

            targetDirectory.Delete(true);
            targetDirectory.Create();

            return targetDirectory;
        }

        private static IEnumerable<string> _GetVersions(DirectoryInfo outputDirectory)
        {
            return outputDirectory
                .GetDirectories()
                .Where(_IsVersionDirectory)
                .OrderBy(directory => directory.Name, Comparer<string>.Create(_VersionComparison))
                .Select(directory => directory.Name)
                .ToList();

            static bool _IsVersionDirectory(DirectoryInfo directory)
               => Regex.IsMatch(directory.Name, @"^\d+\.\d+\.\d+(-(alpha|beta|rc)\d+)?$", RegexOptions.IgnoreCase);

            static int _VersionComparison(string left, string right)
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
        }
    }
}