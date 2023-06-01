using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeMap.DocumentationElements;
using CodeMap.ReferenceData;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented assembly.</summary>
    public class AssemblyDeclaration : DeclarationNode, IEquatable<Assembly>, IEquatable<AssemblyName>
    {
        internal AssemblyDeclaration(AssemblyReference assemblyReference)
            : base(assemblyReference)
        {
        }

        /// <summary>The assembly name.</summary>
        public string Name { get; internal set; }

        /// <summary>The assembly version.</summary>
        public Version Version { get; internal set; }

        /// <summary>The assembly culture, if it is a satelite one.</summary>
        public string Culture { get; internal set; }

        /// <summary>The assembly public key token, if it is a signed one.</summary>
        public string PublicKeyToken { get; internal set; }

        /// <summary>The assemblies that the current one depends on.</summary>
        public IReadOnlyCollection<AssemblyReference> Dependencies { get; internal set; }

        /// <summary>The assembly attributes.</summary>
        public IReadOnlyCollection<AttributeData> Attributes { get; internal set; }

        /// <summary>The declared namespaces.</summary>
        public IReadOnlyCollection<NamespaceDeclaration> Namespaces { get; internal set; }

        /// <summary>The assembly summary.</summary>
        public SummaryDocumentationElement Summary { get; internal set; }

        /// <summary>The assembly remarks.</summary>
        public RemarksDocumentationElement Remarks { get; internal set; }

        /// <summary>The assembly examples.</summary>
        public IReadOnlyList<ExampleDocumentationElement> Examples { get; internal set; }

        /// <summary>The assembly related members.</summary>
        public IReadOnlyList<ReferenceDocumentationElement> RelatedMembers { get; internal set; }

        /// <summary>Applies the first applicable <see cref="AssemblyDocumentationAddition"/> from the provided <paramref name="additions"/>.</summary>
        /// <param name="additions">The <see cref="AssemblyDocumentationAddition"/>s to look through.</param>
        /// <returns>Returns the current <see cref="AssemblyDeclaration"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="additions"/> is <c>null</c>.</exception>
        /// <remarks>
        /// It is possible to have multiple <see cref="AssemblyDocumentationAddition"/>s when working with large libraries. There may
        /// be one <see cref="AssemblyDocumentationAddition"/> for each major version. By providing them as a list it is easier to
        /// maintain because each <see cref="AssemblyDocumentationAddition"/> has its own predicate which in turn will determine which
        /// addition should be used specifically for the current <see cref="AssemblyDeclaration"/>.
        /// </remarks>
        public AssemblyDeclaration Apply(IEnumerable<AssemblyDocumentationAddition> additions)
        {
            if (additions is null)
                throw new ArgumentNullException(nameof(additions));

            var addition = additions.FirstOrDefault(addition => addition.CanApply(this));
            if (addition is object)
            {
                Summary = addition.GetSummary(this) ?? Summary;
                Remarks = addition.GetRemarks(this) ?? Remarks;
                Examples = addition.GetExamples(this).ToReadOnlyList() ?? Examples;
                RelatedMembers = addition.GetRelatedMembers(this).ToReadOnlyList() ?? RelatedMembers;
                var namespaceAdditions = addition.GetNamespaceAdditions(this).ToReadOnlyList();
                if (namespaceAdditions is object && namespaceAdditions.Count > 0)
                    foreach (var @namespace in Namespaces)
                        @namespace.Apply(namespaceAdditions);
            }

            return this;
        }

        /// <summary>Applies the first applicable <see cref="AssemblyDocumentationAddition"/> from the provided <paramref name="additions"/>.</summary>
        /// <param name="additions">The <see cref="AssemblyDocumentationAddition"/>s to look through.</param>
        /// <returns>Returns the current <see cref="AssemblyDeclaration"/> instance.</returns>
        /// <remarks>
        /// It is possible to have multiple <see cref="AssemblyDocumentationAddition"/>s when working with large libraries. There may
        /// be one <see cref="AssemblyDocumentationAddition"/> for each major version. By providing them as a list it is easier to
        /// maintain because each <see cref="AssemblyDocumentationAddition"/> has its own predicate which in turn will determine which
        /// addition should be used specifically for the current <see cref="AssemblyDeclaration"/>.
        /// </remarks>
        public AssemblyDeclaration Apply(params AssemblyDocumentationAddition[] additions)
            => Apply((IEnumerable<AssemblyDocumentationAddition>)additions);

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DeclarationNodeVisitor visitor)
            => visitor.VisitAssembly(this);
    }
}