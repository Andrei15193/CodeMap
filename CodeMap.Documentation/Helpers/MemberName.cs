using System;
using System.IO;
using CodeMap.DeclarationNodes;
using CodeMap.ReferenceData;

namespace CodeMap.Documentation.Helpers
{
    public class MemberName : IHandlebarsHelper
    {
        public string Name
            => nameof(MemberName);

        public void Apply(TextWriter writer, dynamic context, params object[] parameters)
        {
            if (parameters[0] is TypeDeclaration typeDeclaration)
                writer.Write(typeDeclaration.GetMemberName());
            else if (parameters[0] is MemberDeclaration memberDeclaration)
                writer.Write(memberDeclaration.GetMemberName());
            else if (parameters[0] is MemberReference memberReference)
                writer.Write(memberReference.GetMemberName());
            else if (parameters[0] is NamespaceDeclaration @namespace)
                writer.Write(@namespace.Name);
            else
                throw new ArgumentException($"Unhandled parameter type: '{parameters[0].GetType().Name}'");
        }
    }
}
