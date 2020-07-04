﻿using System;
using System.IO;
using CodeMap.DeclarationNodes;

namespace CodeMap.Documentation.Helpers
{
    public class IsReadOnlyProperty : IHandlebarsHelper
    {
        public string Name
            => nameof(IsReadOnlyProperty);

        public void Apply(TextWriter writer, dynamic context, params object[] parameters)
        {
            switch (parameters[0])
            {
                case PropertyDeclaration propertyDeclaration:
                    if (propertyDeclaration.Getter?.AccessModifier >= AccessModifier.Family && (propertyDeclaration.Setter?.AccessModifier ?? AccessModifier.Private) < AccessModifier.Family)
                        writer.Write(true);
                    break;

                default:
                    throw new ArgumentException($"Unhandled parameter type: '{parameters[0].GetType().Name}'");
            }
        }
    }
}