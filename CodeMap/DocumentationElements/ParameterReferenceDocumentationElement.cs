using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeMap.DocumentationElements
{
    /// <summary>Represents a parameter reference corresponding to the <c>paramref</c> XML element.</summary>
    public sealed class ParameterReferenceDocumentationElement : InlineDocumentationElement
    {
        internal ParameterReferenceDocumentationElement(string parameterName, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));

            XmlAttributes = xmlAttributes ?? new Dictionary<string, string>();
            if (XmlAttributes.Any(pair => pair.Value == null))
                throw new ArgumentException("Cannot contain 'null' values.", nameof(xmlAttributes));
        }

        /// <summary>The name of the parameter.</summary>
        public string ParameterName { get; }

        /// <summary>The XML attributes specified on the parameter reference element.</summary>
        public IReadOnlyDictionary<string, string> XmlAttributes { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
            => visitor.VisitParameterReference(this);
    }
}