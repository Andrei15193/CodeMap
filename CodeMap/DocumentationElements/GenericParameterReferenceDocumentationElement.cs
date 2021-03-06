﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeMap.DocumentationElements
{
    /// <summary>Represents a generic parameter reference corresponding to the <c>typeparamref</c> XML element.</summary>
    public sealed class GenericParameterReferenceDocumentationElement : InlineDocumentationElement
    {
        internal GenericParameterReferenceDocumentationElement(string genericParameterName, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            GenericParameterName = genericParameterName ?? throw new ArgumentNullException(nameof(genericParameterName));

            XmlAttributes = xmlAttributes ?? Extensions.EmptyDictionary<string, string>();
            if (XmlAttributes.Any(pair => pair.Value is null))
                throw new ArgumentException("Cannot contain 'null' values.", nameof(xmlAttributes));
        }

        /// <summary>The name of the referred generic parameter.</summary>
        public string GenericParameterName { get; }

        /// <summary>The XML attributes specified on the generic parameter reference element.</summary>
        public IReadOnlyDictionary<string, string> XmlAttributes { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
            => visitor.VisitGenericParameterReference(this);
    }
}