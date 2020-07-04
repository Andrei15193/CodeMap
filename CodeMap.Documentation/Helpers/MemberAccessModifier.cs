using System;
using System.IO;
using CodeMap.DeclarationNodes;

namespace CodeMap.Documentation.Helpers
{
    public class MemberAccessModifier : IHandlebarsHelper
    {
        public string Name
            => nameof(MemberAccessModifier);

        public void Apply(TextWriter writer, dynamic context, params object[] parameters)
        {
            switch (parameters[0])
            {
                case PropertyDeclaration propertyDeclaration:
                    writer.Write(_GetAccessModifierLabel(propertyDeclaration.Getter.AccessModifier));
                    writer.Write(" get");
                    if (propertyDeclaration.Setter?.AccessModifier >= AccessModifier.Family)
                    {
                        writer.Write("; ");
                        writer.Write(_GetAccessModifierLabel(propertyDeclaration.Setter.AccessModifier));
                        writer.Write(" set");
                    }
                    break;

                case MemberDeclaration memberDeclaration:
                    writer.Write(_GetAccessModifierLabel(memberDeclaration.AccessModifier));
                    break;

                case TypeDeclaration typeDeclaration:
                    writer.Write(_GetAccessModifierLabel(typeDeclaration.AccessModifier));
                    break;

                default:
                    throw new ArgumentException($"Unhandled parameter type: '{parameters[0].GetType().Name}'");
            }
        }

        private string _GetAccessModifierLabel(AccessModifier accessModifier)
            => accessModifier switch
            {
                AccessModifier.FamilyOrAssembly => "protected",
                AccessModifier.Family => "protected",
                AccessModifier.Public => "public",
                _ => throw new ArgumentException($"Unhandled access modifier: {accessModifier}.")
            };
    }
}