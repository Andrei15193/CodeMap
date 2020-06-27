using CodeMap.DeclarationNodes;
using CodeMap.ReferenceData;
using System;
using System.IO;

namespace CodeMap.Documentation.Helpers
{
    public class MemberUrl : IHandlebarsHelper
    {
        public string Name
            => nameof(MemberUrl);

        public void Apply(TextWriter writer, dynamic context, params object[] parameters)
        {
            if (parameters[0] is MemberDeclaration memberDeclaration)
            {
                writer.Write(memberDeclaration.GetMemberFullName());
                writer.Write(".html");
            }
            else if (parameters[0] is TypeDeclaration typeDeclaration)
            {
                writer.Write(typeDeclaration.GetMemberFullName());
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
                    writer.Write(typeReference.GetMemberFullName());
                    writer.Write(".html");
                }
                else
                    writer.Write(typeReference.GetMicrosoftDocsLink());
            }
            else
                throw new ArgumentException($"Unhandled parameter type: '{parameters[0].GetType().Name}'");
        }
    }
}