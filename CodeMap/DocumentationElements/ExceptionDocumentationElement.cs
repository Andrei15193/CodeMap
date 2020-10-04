using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeMap.DocumentationElements
{
    /// <summary>Represents an exception documentation element.</summary>
    public class ExceptionDocumentationElement : DocumentationElement
    {
        internal ExceptionDocumentationElement(MemberReferenceDocumentationElement exception, IEnumerable<BlockDocumentationElement> description, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            Description = description.ToReadOnlyList() ?? throw new ArgumentNullException(nameof(description));
            if (Description.Contains(null))
                throw new ArgumentException("Cannot contain 'null' elements.", nameof(description));

            XmlAttributes = xmlAttributes ?? Extensions.EmptyDictionary<string, string>();
            if (XmlAttributes.Any(pair => pair.Value == null))
                throw new ArgumentException("Cannot contain 'null' values.", nameof(xmlAttributes));
        }

        /// <summary>The exception being thrown.</summary>
        public new MemberReferenceDocumentationElement Exception { get; }

        /// <summary>The description about when the exception is being thrown.</summary>
        public IReadOnlyList<BlockDocumentationElement> Description { get; }

        /// <summary>The XML attributes specified on the generic parameter reference element.</summary>
        public IReadOnlyDictionary<string, string> XmlAttributes { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
            => visitor.VisitException(this);
    }
}