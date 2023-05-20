using System.Linq;
using System.Reflection;
using CodeMap.DeclarationNodes;

namespace CodeMap.Html
{
    /// <summary>Helper methods for generating HTML pages.</summary>
    public static class AssemblyDeclarationExtensions
    {
        /// <summary>Gets the informal version from the provided <paramref name="assemblyDeclaration"/>.</summary>
        /// <param name="assemblyDeclaration">The <see cref="AssemblyDeclaration"/> from which to get the informal version.</param>
        /// <returns>Returns the informal version of the provided <paramref name="assemblyDeclaration"/>.</returns>
        public static string GetInformalVersion(this AssemblyDeclaration assemblyDeclaration)
            => (string)assemblyDeclaration
                .Attributes
                .Single(attribute => attribute.Type.Namespace.Name == "System.Reflection" && attribute.Type.Name == nameof(AssemblyInformationalVersionAttribute))
                .PositionalParameters
                .Single()
                .Value;
    }
}