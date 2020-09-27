using System;
using System.IO;
using System.Linq;
using CodeMap.DeclarationNodes;

namespace CodeMap.Documentation.Helpers
{
    public class MemberAccessModifier : IHandlebarsHelper
    {
        public string Name
            => nameof(MemberAccessModifier);

        public void Apply(TextWriter writer, object context, params object[] parameters)
        {
            var declarationNode = parameters.DefaultIfEmpty(context).First() as DeclarationNode;
            if (declarationNode is null)
                throw new ArgumentException("Expected a " + nameof(DeclarationNode) + " provided as the first parameter or context.");

            switch (declarationNode)
            {
                case PropertyDeclaration propertyDeclaration:
                    if (propertyDeclaration.Getter != null && propertyDeclaration.Setter != null)
                        if (propertyDeclaration.Getter.AccessModifier == propertyDeclaration.Setter.AccessModifier)
                        {
                            writer.Write(_GetAccessModifierLabel(propertyDeclaration.AccessModifier));
                            writer.Write(" get, set");
                        }
                        else if (propertyDeclaration.Getter.AccessModifier >= AccessModifier.Family && propertyDeclaration.Setter.AccessModifier >= AccessModifier.Family)
                        {
                            writer.Write(_GetAccessModifierLabel(propertyDeclaration.Getter.AccessModifier));
                            writer.Write(" get; ");
                            writer.Write(_GetAccessModifierLabel(propertyDeclaration.Setter.AccessModifier));
                            writer.Write(" set");
                        }
                        else if (propertyDeclaration.Getter.AccessModifier >= AccessModifier.Family)
                        {
                            writer.Write(_GetAccessModifierLabel(propertyDeclaration.Getter.AccessModifier));
                            writer.Write(" get");
                        }
                        else
                        {
                            writer.Write(_GetAccessModifierLabel(propertyDeclaration.Setter.AccessModifier));
                            writer.Write(" set");
                        }
                    else if (propertyDeclaration.Getter != null)
                    {
                        writer.Write(_GetAccessModifierLabel(propertyDeclaration.Getter.AccessModifier));
                        writer.Write(" get");
                    }
                    else
                    {
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
                    throw new ArgumentException($"Unhandled declaration node type: '{declarationNode.GetType().Name}'");
            }
        }

        private string _GetAccessModifierLabel(AccessModifier accessModifier)
            => accessModifier switch
            {
                AccessModifier.Private => "private",
                AccessModifier.FamilyAndAssembly => "private",
                AccessModifier.Assembly => "internal",
                AccessModifier.Family => "protected",
                AccessModifier.FamilyOrAssembly => "protected",
                AccessModifier.Public => "public",
                _ => throw new ArgumentException($"Unhandled access modifier: {accessModifier}.")
            };
    }
}