using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CodeMap.DeclarationNodes;

namespace CodeMap.Handlebars.Helpers
{
    /// <summary>A helper used to get the company name from an <see cref="AssemblyDeclaration"/>.</summary>
    /// <example>
    /// The following template will generate a paragraph containing the company name.
    /// <code language="html">
    /// &lt;p&gt;{{GetAssemblyCompany}}&lt;/p&gt;
    /// </code>
    /// If the current context is an <see cref="AssemblyDeclaration"/> with the company name set to <c>MyAwesomeCompany</c>, the output would be:
    /// <code language="html">
    /// &lt;p&gt;MyAwesomeCompany&lt;/p&gt;
    /// </code>
    /// </example>
    public class GetAssemblyCompany : IHandlebarsHelper
    {
        /// <summary>Gets the name of the helper.</summary>
        /// <value>The value of this property is <c>GetAssemblyCompany</c>. It is a constant.</value>
        public string Name
            => nameof(GetAssemblyCompany);

        /// <summary>Gets the assembly company attribute value from the first parameter or current context.</summary>
        /// <param name="writer">The <see cref="TextWriter"/> to write the result to.</param>
        /// <param name="context">The context in which this helper is called.</param>
        /// <param name="parameters">The parameter with which this helper has been called.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the first parameter is not an <see cref="AssemblyDeclaration"/> or when not provided and the given <paramref name="context"/> is not an <see cref="AssemblyDeclaration"/>.
        /// </exception>
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