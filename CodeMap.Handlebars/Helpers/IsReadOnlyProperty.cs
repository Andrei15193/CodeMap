using System;
using System.IO;
using System.Linq;
using CodeMap.DeclarationNodes;

namespace CodeMap.Handlebars.Helpers
{
    public class IsReadOnlyProperty : IHandlebarsHelper
    {
        public string Name
            => nameof(IsReadOnlyProperty);

        public void Apply(TextWriter writer, object context, params object[] parameters)
        {
            var propertyDeclaration = parameters.DefaultIfEmpty(context).First() as PropertyDeclaration;
            if (propertyDeclaration is null)
                throw new ArgumentException("Expected a " + nameof(PropertyDeclaration) + " provided as the first parameter or context.");

            if (propertyDeclaration.Getter?.AccessModifier >= AccessModifier.Family && (propertyDeclaration.Setter?.AccessModifier ?? AccessModifier.Private) < AccessModifier.Family)
                writer.Write(true);
        }
    }
}