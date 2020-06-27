using CodeMap.DeclarationNodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeMap.Documentation.Additions
{
    public static class DocumentationAdditionApplier
    {
        public static AssemblyDeclaration Apply(this AssemblyDeclaration assemblyDeclaration, IEnumerable<AssemblyDocumentationAddition> additions)
        {
            if (assemblyDeclaration is null)
                throw new ArgumentNullException(nameof(assemblyDeclaration));
            if (additions is null)
                throw new ArgumentNullException(nameof(additions));

            var addition = _GetMatchingAddition(assemblyDeclaration, additions);
            if (addition != null)
            {
                assemblyDeclaration.Summary = addition.Summary ?? assemblyDeclaration.Summary;
                assemblyDeclaration.Remarks = addition.Remarks ?? assemblyDeclaration.Remarks;
                assemblyDeclaration.Examples = addition.Examples ?? assemblyDeclaration.Examples;
                assemblyDeclaration.RelatedMembers = addition.RelatedMembers ?? assemblyDeclaration.RelatedMembers;
                if (addition.NamespaceAdditions != null)
                    foreach (var @namespace in assemblyDeclaration.Namespaces)
                        if (addition.NamespaceAdditions.TryGetValue(@namespace.Name, out var namespaceAddition))
                        {
                            @namespace.Summary = namespaceAddition.Summary ?? @namespace.Summary;
                            @namespace.Remarks = namespaceAddition.Remarks ?? @namespace.Remarks;
                            @namespace.Examples = namespaceAddition.Examples ?? @namespace.Examples;
                            @namespace.RelatedMembers = namespaceAddition.RelatedMembers ?? @namespace.RelatedMembers;
                        }
            }
            return assemblyDeclaration;
        }

        public static AssemblyDeclaration Apply(this AssemblyDeclaration assemblyDeclaration, params AssemblyDocumentationAddition[] additions)
            => assemblyDeclaration.Apply((IEnumerable<AssemblyDocumentationAddition>)additions);

        private static AssemblyDocumentationAddition _GetMatchingAddition(AssemblyDeclaration assemblyDeclaration, IEnumerable<AssemblyDocumentationAddition> additions)
            => additions
                .Where(addition => addition.CanApply(assemblyDeclaration))
                .Concat(additions.Where(addition => addition.CanApply is null))
                .FirstOrDefault();
    }
}