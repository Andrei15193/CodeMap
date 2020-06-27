using CodeMap.DeclarationNodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeMap.Documentation.Helpers
{
    public class HasPublicDefinitions : IHandlebarsHelper
    {
        public string Name
            => nameof(HasPublicDefinitions);

        public void Apply(TextWriter writer, dynamic context, params object[] parameters)
        {
            switch (parameters[0])
            {
                case IEnumerable<MemberDeclaration> memberDeclarations:
                    if (memberDeclarations.Any(memberDeclaration => memberDeclaration.AccessModifier == AccessModifier.Public))
                        writer.Write(true);
                    break;

                case IEnumerable<TypeDeclaration> typeDeclarations:
                    if (typeDeclarations.Any(typeDeclaration => typeDeclaration.AccessModifier == AccessModifier.Public))
                        writer.Write(true);
                    break;

                default:
                    throw new ArgumentException($"Unhandled parameter type: '{parameters[0].GetType().Name}'");
            }
        }
    }
}