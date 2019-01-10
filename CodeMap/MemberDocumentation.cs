using System;
using System.Collections.Generic;
using System.Linq;
using CodeMap.Elements;

namespace CodeMap
{
    /// <summary>Represents a documentation entry for a specific member in the XML documentation.</summary>
    public sealed class MemberDocumentation
    {
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
        /// <param name="relatedMembersList">The related members list.</param>
        public MemberDocumentation(
            string canonicalName,
            SummaryDocumentationElement summary,
            ILookup<string, BlockDocumentationElement> genericParameters,
            ILookup<string, BlockDocumentationElement> parameters,
            IEnumerable<BlockDocumentationElement> returns,
            ILookup<string, BlockDocumentationElement> exceptions,
            RemarksDocumentationElement remarks,
            IEnumerable<ExampleDocumentationElement> examples,
            ValueDocumentationElement value,
            RelatedMembersList relatedMembersList)
        {
            CanonicalName = canonicalName ?? throw new ArgumentNullException(nameof(canonicalName));
            Summary = summary;
            GenericParameters = genericParameters.OrEmpty();
            Parameters = parameters.OrEmpty();
            Returns = returns.AsReadOnlyListOrEmpty();
            Exceptions = exceptions.OrEmpty();
            Remarks = remarks;
            Examples = examples.AsReadOnlyListOrEmpty();
            Value = value;
            RelatedMembersList = relatedMembersList ?? new RelatedMembersList(Enumerable.Empty<MemberReferenceDocumentationElement>());
        }

        /// <summary>The canonical name of the documented member.</summary>
        public string CanonicalName { get; }

        /// <summary>The summary section.</summary>
        public SummaryDocumentationElement Summary { get; }

        /// <summary>The generic parameters documentation.</summary>
        public ILookup<string, BlockDocumentationElement> GenericParameters { get; }

        /// <summary>The parameters documentation.</summary>
        public ILookup<string, BlockDocumentationElement> Parameters { get; }

        /// <summary>The returns section.</summary>
        public IReadOnlyList<BlockDocumentationElement> Returns { get; }

        /// <summary>The exceptions documentation.</summary>
        public ILookup<string, BlockDocumentationElement> Exceptions { get; }

        /// <summary>The remarks section.</summary>
        public RemarksDocumentationElement Remarks { get; }

        /// <summary>The examples sections.</summary>
        public IReadOnlyList<ExampleDocumentationElement> Examples { get; }

        /// <summary>The value section.</summary>
        public ValueDocumentationElement Value { get; }

        /// <summary>The related members list.</summary>
        public RelatedMembersList RelatedMembersList { get; }
    }
}