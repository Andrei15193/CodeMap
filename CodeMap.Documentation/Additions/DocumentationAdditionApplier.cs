using System;
using System.Collections.Generic;
using System.Linq;
using CodeMap.DeclarationNodes;

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

            var addition = additions.FirstOrDefault(addition => addition.CanApply(assemblyDeclaration));
            if (addition != null)
            {
                assemblyDeclaration.Summary = addition.GetSummary(assemblyDeclaration) ?? assemblyDeclaration.Summary;
                assemblyDeclaration.Remarks = addition.GetRemarks(assemblyDeclaration) ?? assemblyDeclaration.Remarks;
                assemblyDeclaration.Examples = addition.GetExamples(assemblyDeclaration)?.ToList() ?? assemblyDeclaration.Examples;
                assemblyDeclaration.RelatedMembers = addition.GetRelatedMembers(assemblyDeclaration)?.ToList() ?? assemblyDeclaration.RelatedMembers;
                var namespaceAdditions = (addition.GetNamespaceAdditions(assemblyDeclaration) ?? Enumerable.Empty<NamespaceDocumentationAddition>()).ToList();
                if (namespaceAdditions.Any())
                    foreach (var @namespace in assemblyDeclaration.Namespaces)
                    {
                        var namespaceAddition = namespaceAdditions.FirstOrDefault(addition => addition.CanApply(@namespace));
                        if (namespaceAddition != null)
                        {
                            @namespace.Summary = namespaceAddition.GetSummary(@namespace) ?? @namespace.Summary;
                            @namespace.Remarks = namespaceAddition.GetRemarks(@namespace) ?? @namespace.Remarks;
                            @namespace.Examples = namespaceAddition.GetExamples(@namespace)?.ToList() ?? @namespace.Examples;
                            @namespace.RelatedMembers = namespaceAddition.GetRelatedMembers(@namespace)?.ToList() ?? @namespace.RelatedMembers;
                        }
                    }
            }
            return assemblyDeclaration;
        }

        public static AssemblyDeclaration Apply(this AssemblyDeclaration assemblyDeclaration, params AssemblyDocumentationAddition[] additions)
            => assemblyDeclaration.Apply((IEnumerable<AssemblyDocumentationAddition>)additions);
    }
}