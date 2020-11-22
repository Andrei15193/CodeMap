using System.Collections.Generic;
using CodeMap.DocumentationElements;
using CodeMap.ReferenceData;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented declared member of a type.</summary>
    public abstract class MemberDeclaration : DeclarationNode
    {
        internal MemberDeclaration(MemberReference memberReference)
            : base(memberReference)
        {
        }

        /// <summary>The member name.</summary>
        public string Name { get; internal set; }

        /// <summary>The member access modifier.</summary>
        public AccessModifier AccessModifier { get; internal set; }

        /// <summary>The member declaring type.</summary>
        public TypeDeclaration DeclaringType { get; internal set; }

        /// <summary>The member attributes.</summary>
        public IReadOnlyCollection<AttributeData> Attributes { get; internal set; }

        /// <summary>The member summary.</summary>
        public SummaryDocumentationElement Summary { get; internal set; }

        /// <summary>The member remarks.</summary>
        public RemarksDocumentationElement Remarks { get; internal set; }

        /// <summary>The member examples.</summary>
        public IReadOnlyList<ExampleDocumentationElement> Examples { get; internal set; }

        /// <summary>The related members of the declared member.</summary>
        public IReadOnlyList<MemberReferenceDocumentationElement> RelatedMembers { get; internal set; }
    }
}