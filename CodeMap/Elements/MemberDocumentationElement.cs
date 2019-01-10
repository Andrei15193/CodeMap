using System.Collections.Generic;

namespace CodeMap.Elements
{
    /// <summary>Represents a documented declared member of a type.</summary>
    public abstract class MemberDocumentationElement : DocumentationElement
    {
        internal MemberDocumentationElement()
        {
        }

        /// <summary>The member name.</summary>
        public string Name { get; internal set; }

        /// <summary>The member access modifier.</summary>
        public AccessModifier AccessModifier { get; internal set; }

        /// <summary>The member declaring type.</summary>
        public TypeDocumentationElement DeclaringType { get; internal set; }

        /// <summary>The member attributes.</summary>
        public IReadOnlyCollection<AttributeData> Attributes { get; internal set; }

        /// <summary>The member summary.</summary>
        new public SummaryDocumentationElement Summary { get; internal set; }

        /// <summary>The member remarks.</summary>
        new public RemarksDocumentationElement Remarks { get; internal set; }

        /// <summary>The member examples.</summary>
        public IReadOnlyList<ExampleDocumentationElement> Examples { get; internal set; }

        /// <summary>The related members of the declared member.</summary>
        public RelatedMembersList RelatedMembers { get; }
    }
}