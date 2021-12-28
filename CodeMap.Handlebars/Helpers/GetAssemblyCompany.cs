using System;
using System.Linq;
using System.Reflection;
using CodeMap.DeclarationNodes;
using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.PathStructure;

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
    public class GetAssemblyCompany : IHelperDescriptor<HelperOptions>
    {
        /// <summary>Gets the name of the helper.</summary>
        /// <value>The value of this property is <c>GetAssemblyCompany</c>.</value>
        public PathInfo Name
            => nameof(GetAssemblyCompany);

        /// <summary>Gets the assembly company attribute value from the first argument or current context.</summary>
        /// <param name="options">The helper options.</param>
        /// <param name="context">The context in which this helper is called.</param>
        /// <param name="arguments">The arguments with which this helper has been called.</param>
        /// <returns>Returns the assembly company attribute value from the first argument or current context.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the first argument is not an <see cref="AssemblyDeclaration"/> or when not provided and the given <paramref name="context"/> is not an <see cref="AssemblyDeclaration"/>.
        /// </exception>
        public object Invoke(in HelperOptions options, in Context context, in Arguments arguments)
        {
            var assemblyDeclaraction = arguments.ElementAtOrDefault(0) as AssemblyDeclaration ?? throw new ArgumentException("Expected an " + nameof(AssemblyDeclaration) + " provided as the first argument or context.");

            var companyName = assemblyDeclaraction
                .Attributes
                .Where(attribute => attribute.Type == typeof(AssemblyCompanyAttribute))
                .Select(attribute => (string)attribute.PositionalParameters[0].Value)
                .FirstOrDefault();
            return companyName;
        }

        /// <summary>Writes the assembly company attribute value from the first argument or current context to the provided <paramref name="output"/>.</summary>
        /// <param name="output">The <see cref="EncodedTextWriter"/> to write the result to.</param>
        /// <param name="options">The helper options.</param>
        /// <param name="context">The context in which this helper is called.</param>
        /// <param name="arguments">The arguments with which this helper has been called.</param>
        /// <returns>Returns the assembly company attribute value from the first argument or current context.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the first argument is not an <see cref="AssemblyDeclaration"/> or when not provided and the given <paramref name="context"/> is not an <see cref="AssemblyDeclaration"/>.
        /// </exception>
        public void Invoke(in EncodedTextWriter output, in HelperOptions options, in Context context, in Arguments arguments)
            => output.WriteSafeString(Invoke(options, context, arguments));
    }
}