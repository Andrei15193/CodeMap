using System.Collections.Generic;
using CodeMap.DocumentationElements;
using CodeMap.ReferenceData;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented type.</summary>
    public abstract class TypeDeclaration : DeclarationNode
    {
        internal TypeDeclaration(TypeReference typeReference)
            : base(typeReference)
        {
        }

        /// <summary>The type name.</summary>
        public string Name { get; internal set; }

        /// <summary>The declaring namespace.</summary>
        public NamespaceDeclaration Namespace { get; internal set; }

        /// <summary>The declaring assembly.</summary>
        public AssemblyDeclaration Assembly
            => Namespace?.Assembly;

        /// <summary>The delcaring type.</summary>
        public TypeDeclaration DeclaringType { get; internal set; }

        /// <summary>The type access modifier.</summary>
        public AccessModifier AccessModifier { get; internal set; }

        /// <summary>The attributes decorating the type.</summary>
        public IReadOnlyCollection<AttributeData> Attributes { get; internal set; }

        /// <summary>The type summary.</summary>
        public SummaryDocumentationElement Summary { get; internal set; }

        /// <summary>The type remarks.</summary>
        public RemarksDocumentationElement Remarks { get; internal set; }

        /// <summary>The type examples.</summary>
        public IReadOnlyList<ExampleDocumentationElement> Examples { get; internal set; }

        /// <summary>The type related members.</summary>
        public IReadOnlyList<MemberReferenceDocumentationElement> RelatedMembers { get; internal set; }
    }
}