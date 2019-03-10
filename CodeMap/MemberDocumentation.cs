using CodeMap.Elements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeMap
{
    /// <summary>Represents a documentation entry for a specific member in the XML documentation.</summary>
    public sealed class MemberDocumentation
    {
        /// <summary>Initializes a new instance of the <see cref="MemberDocumentation"/> class.</summary>
        /// <param name="canonicalName">The canonical name of the documented member.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="canonicalName"/> is <c>null</c>.</exception>
        public MemberDocumentation(string canonicalName)
            : this(canonicalName, null, null, null, null, null, null, null, null, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="MemberDocumentation"/> class.</summary>
        /// <param name="canonicalName">The canonical name of the documented member.</param>
        /// <param name="summary">The summary section.</param>
        /// <param name="genericParameters">The generic parameters documentation.</param>
        /// <param name="parameters">The parameters documentation.</param>
        /// <param name="returns">The returns section.</param>
        /// <param name="exceptions">The exceptions documentation.</param>
        /// <param name="remarks">The remarks section.</param>
        /// <param name="examples">The examples sections.</param>
        /// <param name="value">The value section.</param>
        /// <param name="relatedMembers">The related members.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="canonicalName"/> is <c>null</c>.</exception>
        public MemberDocumentation(
            string canonicalName,
            SummaryDocumentationElement summary,
            IReadOnlyDictionary<string, BlockDescriptionDocumentationElement> genericParameters,
            IReadOnlyDictionary<string, BlockDescriptionDocumentationElement> parameters,
            BlockDescriptionDocumentationElement returns,
            IReadOnlyDictionary<string, BlockDescriptionDocumentationElement> exceptions,
            RemarksDocumentationElement remarks,
            IEnumerable<ExampleDocumentationElement> examples,
            ValueDocumentationElement value,
            IEnumerable<MemberReferenceDocumentationElement> relatedMembers)
        {
            CanonicalName = canonicalName ?? throw new ArgumentNullException(nameof(canonicalName));
            Summary = summary ?? DocumentationElement.Summary(Enumerable.Empty<BlockDocumentationElement>());
            GenericParameters = genericParameters.OrEmpty();
            Parameters = parameters.OrEmpty();
            Returns = returns ?? DocumentationElement.BlockDescription(Enumerable.Empty<BlockDocumentationElement>());
            Exceptions = exceptions.OrEmpty();
            Remarks = remarks ?? DocumentationElement.Remarks(Enumerable.Empty<BlockDocumentationElement>());
            Examples = examples.AsReadOnlyListOrEmpty();
            Value = value ?? DocumentationElement.Value(Enumerable.Empty<BlockDocumentationElement>());
            RelatedMembers = relatedMembers.AsReadOnlyListOrEmpty();
        }

        /// <summary>The canonical name of the documented member.</summary>
        public string CanonicalName { get; }

        /// <summary>The summary section.</summary>
        public SummaryDocumentationElement Summary { get; }

        /// <summary>The generic parameters documentation.</summary>
        public IReadOnlyDictionary<string, BlockDescriptionDocumentationElement> GenericParameters { get; }

        /// <summary>The parameters documentation.</summary>
        public IReadOnlyDictionary<string, BlockDescriptionDocumentationElement> Parameters { get; }

        /// <summary>The returns section.</summary>
        public BlockDescriptionDocumentationElement Returns { get; }

        /// <summary>The exceptions documentation.</summary>
        public IReadOnlyDictionary<string, BlockDescriptionDocumentationElement> Exceptions { get; }

        /// <summary>The remarks section.</summary>
        public RemarksDocumentationElement Remarks { get; }

        /// <summary>The examples sections.</summary>
        public IReadOnlyList<ExampleDocumentationElement> Examples { get; }

        /// <summary>The value section.</summary>
        public ValueDocumentationElement Value { get; }

        /// <summary>The related members list.</summary>
        public IReadOnlyList<MemberReferenceDocumentationElement> RelatedMembers { get; }
    }
}