using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeMap.DocumentationElements
{
    /// <summary>Represents an inline code snippet corresponding to the <c>c</c> XML element.</summary>
    public sealed class InlineCodeDocumentationElement : InlineDocumentationElement
    {
        internal InlineCodeDocumentationElement(string code, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));

            XmlAttributes = xmlAttributes ?? new Dictionary<string, string>();
            if (XmlAttributes.Any(pair => pair.Value == null))
                throw new ArgumentException("Cannot contain 'null' values.", nameof(xmlAttributes));
        }

        /// <summary>The inline code snippet.</summary>
        public string Code { get; }

        /// <summary>The XML attributes specified on the inline code element.</summary>
        public IReadOnlyDictionary<string, string> XmlAttributes { get; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
            => visitor.VisitInlineCode(Code, XmlAttributes);
    }
}