using System;
using System.IO;
using CodeMap.DeclarationNodes;

namespace CodeMap.Documentation.Helpers
{
    public class MemberAccessModifier : HandlebarsContextualHelper<DeclarationNode>
    {
        public override string Name
            => nameof(MemberAccessModifier);

        public override void Apply(TextWriter writer, PageContext context, DeclarationNode parameter)
        {
            switch (parameter)
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
                    throw new ArgumentException($"Unhandled parameter type: '{parameter.GetType().Name}'");
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