﻿using System;
using System.IO;
using CodeMap.DeclarationNodes;

namespace CodeMap.Documentation.Helpers
{
    public class IsPublicDefinition : IHandlebarsHelper
    {
        public string Name
            => nameof(IsPublicDefinition);

        public void Apply(TextWriter writer, dynamic context, params object[] parameters)
        {
            switch (parameters[0])
            {
                case MemberDeclaration memberDeclaration:
                    if (memberDeclaration.AccessModifier >= AccessModifier.Family)
                        writer.Write(true);
                    break;

                case TypeDeclaration typeDeclaration:
                    if (typeDeclaration.AccessModifier >= AccessModifier.Family)
                        writer.Write(true);
                    break;

                default:
                    throw new ArgumentException($"Unhandled parameter type: '{parameters[0].GetType().Name}'");
            }
        }
    }
}