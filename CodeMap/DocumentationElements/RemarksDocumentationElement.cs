﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeMap.DocumentationElements
{
    /// <summary>Represents a remarks section corresponding to the <c>remarks</c> XML element.</summary>
    public sealed class RemarksDocumentationElement : DocumentationElement
    {
        internal RemarksDocumentationElement(IEnumerable<BlockDocumentationElement> content, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            Content = content.ToReadOnlyList() ?? throw new ArgumentNullException(nameof(content));
            if (Content.Contains(null))
                throw new ArgumentException("Cannot contain 'null' elements.", nameof(content));

            XmlAttributes = xmlAttributes ?? Extensions.EmptyDictionary<string, string>();
            if (XmlAttributes.Any(pair => pair.Value is null))
                throw new ArgumentException("Cannot contain 'null' values.", nameof(xmlAttributes));
        }

        /// <summary>The content of the remarks section.</summary>
        public IReadOnlyList<BlockDocumentationElement> Content { get; }

        /// <summary>The additional XML attributes on the remarks element.</summary>
        public IReadOnlyDictionary<string, string> XmlAttributes { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
            => visitor.VisitRemarks(this);
    }
}