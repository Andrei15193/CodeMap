using System.Linq;
using System.Reflection;
using CodeMap.DeclarationNodes;

namespace CodeMap.Html
{
    /// <summary/>
    public static class AssemblyDeclarationExtensions
    {
    /// <summary/>
        public static string GetInformalVersion(this AssemblyDeclaration assemblyDeclaration)
            => (string)assemblyDeclaration
                .Attributes
                .Single(attribute => attribute.Type.Namespace.Name == "System.Reflection" && attribute.Type.Name == nameof(AssemblyInformationalVersionAttribute))
                .PositionalParameters
                .Single()
                .Value;
    }
}