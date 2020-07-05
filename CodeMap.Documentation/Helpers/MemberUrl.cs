using System;
using System.IO;
using CodeMap.DeclarationNodes;
using CodeMap.ReferenceData;

namespace CodeMap.Documentation.Helpers
{
    public class MemberUrl : HandlebarsContextualHelper<object>
    {
        public override string Name
            => nameof(MemberUrl);

        public override void Apply(TextWriter writer, PageContext context, object parameter)
        {
            switch (parameter)
            {
                case MemberDeclaration memberDeclaration:
                    writer.Write(context.MemberFileNameProvider.GetFileName(memberDeclaration));
                    writer.Write(".html");
                    break;

                case TypeDeclaration typeDeclaration:
                    writer.Write(context.MemberFileNameProvider.GetFileName(typeDeclaration));
                    writer.Write(".html");
                    break;

                case NamespaceDeclaration namespaceDeclaration:
                    writer.Write(namespaceDeclaration.Name);
                    writer.Write(".html");
                    break;

                case AssemblyDeclaration _:
                    writer.Write("Index.html");
                    break;

                case TypeReference typeReference:
                    if (typeReference.Assembly == typeof(DeclarationNode).Assembly.GetName())
                    {
                        writer.Write(context.MemberFileNameProvider.GetFileName(typeReference));
                        writer.Write(".html");
                    }
                    else
                        writer.Write(typeReference.GetMicrosoftDocsLink());
                    break;

                case ArrayTypeReference arrayTypeReference:
                    Apply(writer, context, arrayTypeReference.ItemType);
                    break;

                default:
                    throw new ArgumentException($"Unhandled parameter type: '{parameter.GetType().Name}'");
            }
        }
    }
}