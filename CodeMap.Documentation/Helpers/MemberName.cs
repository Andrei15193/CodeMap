using System;
using System.IO;
using CodeMap.DeclarationNodes;
using CodeMap.ReferenceData;

namespace CodeMap.Documentation.Helpers
{
    public class MemberName : HandlebarsContextualHelper<object>
    {
        public override string Name
            => nameof(MemberName);

        public override void Apply(TextWriter writer, PageContext context, object parameter)
        {
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