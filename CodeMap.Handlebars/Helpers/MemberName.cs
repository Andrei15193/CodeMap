using System;
using System.IO;
using System.Linq;
using CodeMap.DeclarationNodes;
using CodeMap.ReferenceData;

namespace CodeMap.Handlebars.Helpers
{
    public class MemberName : IHandlebarsHelper
    {
        public string Name
            => nameof(MemberName);

        public void Apply(TextWriter writer, object context, params object[] parameters)
        {
            var parameter = parameters.DefaultIfEmpty(context).First();
            switch (parameter)
            {
                case TypeDeclaration typeDeclaration:
                    writer.Write(typeDeclaration.GetMemberName());
                    break;

                case MemberDeclaration memberDeclaration:
                    writer.Write(memberDeclaration.GetMemberName());
                    break;

                case MemberReference memberReference:
                    writer.Write(memberReference.GetMemberName());
                    break;

                case NamespaceDeclaration @namespace:
                    writer.Write(@namespace.Name);
                    break;

                default:
                    throw new ArgumentException($"Unhandled parameter type: '{parameter.GetType().Name}'");
            }
        }
    }
}