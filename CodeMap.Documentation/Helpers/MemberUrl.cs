using System;
using System.IO;
using CodeMap.DeclarationNodes;
using CodeMap.ReferenceData;

namespace CodeMap.Documentation.Helpers
{
    public class MemberUrl : IHandlebarsHelper
    {
        private readonly MemberFileNameProvider _memberFileNameProvider;

        public MemberUrl(MemberFileNameProvider memberFileNameProvider)
            => _memberFileNameProvider = memberFileNameProvider;

        public string Name
            => nameof(MemberUrl);

        public void Apply(TextWriter writer, dynamic context, params object[] parameters)
        {
            if (parameters[0] is MemberDeclaration memberDeclaration)
            {
                writer.Write(_memberFileNameProvider.GetFileName(memberDeclaration));
                writer.Write(".html");
            }
            else if (parameters[0] is TypeDeclaration typeDeclaration)
            {
                writer.Write(_memberFileNameProvider.GetFileName(typeDeclaration));
                writer.Write(".html");
            }
            else if (parameters[0] is NamespaceDeclaration namespaceDeclaration)
            {
                writer.Write(namespaceDeclaration.Name);
                writer.Write(".html");
            }
            else if (parameters[0] is AssemblyDeclaration _)
                writer.Write("Index.html");
            else if (parameters[0] is TypeReference typeReference)
            {
                if (typeReference.Assembly == typeof(DeclarationNode).Assembly.GetName())
                {
                    writer.Write(_memberFileNameProvider.GetFileName(typeReference));
                    writer.Write(".html");
                }
                else
                    writer.Write(typeReference.GetMicrosoftDocsLink());
            }
            else if (parameters[0] is ArrayTypeReference arrayTypeReference)
                Apply(writer, context, arrayTypeReference.ItemType);
            else
                throw new ArgumentException($"Unhandled parameter type: '{parameters[0].GetType().Name}'");
        }
    }
}