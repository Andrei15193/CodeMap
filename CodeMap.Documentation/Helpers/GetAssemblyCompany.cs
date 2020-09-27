using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CodeMap.DeclarationNodes;

namespace CodeMap.Documentation.Helpers
{
    public class GetAssemblyCompany : IHandlebarsHelper
    {
        public string Name
            => nameof(GetAssemblyCompany);

        public void Apply(TextWriter writer, object context, params object[] parameters)
        {
            var assemblyDeclaraction = parameters.DefaultIfEmpty(context).First() as AssemblyDeclaration;
            if (assemblyDeclaraction is null)
                throw new ArgumentException("Expected a " + nameof(AssemblyDeclaration) + " provided as the first parameter or context.");

            var companyName = assemblyDeclaraction
                .Attributes
                .Where(attribute => attribute.Type == typeof(AssemblyCompanyAttribute))
                .Select(attribute => (string)attribute.PositionalParameters[0].Value)
                .FirstOrDefault();
            writer.Write(companyName);
        }
    }
}