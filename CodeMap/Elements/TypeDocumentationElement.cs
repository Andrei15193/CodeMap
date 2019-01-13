using System.Collections.Generic;

namespace CodeMap.Elements
{
    /// <summary>Represents a documented type.</summary>
    public abstract class TypeDocumentationElement : DocumentationElement
    {
        internal TypeDocumentationElement()
        {
        }

        /// <summary>The type name.</summary>
        public string Name { get; internal set; }

        /// <summary>The delcaring type, if any.</summary>
        public TypeDocumentationElement DeclaringType { get; internal set; }

        /// <summary>The type access modifier.</summary>
        public AccessModifier AccessModifier { get; internal set; }

        /// <summary>The attributes decorating the type.</summary>
        public IReadOnlyCollection<AttributeData> Attributes { get; internal set; }

        /// <summary>The type summary, if any.</summary>
        new public SummaryDocumentationElement Summary { get; internal set; }

        /// <summary>The type remarks, if any.</summary>
        new public RemarksDocumentationElement Remarks { get; internal set; }

        /// <summary>The type examples.</summary>
        public IReadOnlyList<ExampleDocumentationElement> Examples { get; internal set; }

        /// <summary>The type related members.</summary>
        public IReadOnlyList<MemberReferenceDocumentationElement> RelatedMembers { get; internal set; }
    }
}