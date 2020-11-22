using System.Collections.Generic;
using CodeMap.ReferenceData;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented interface declaration.</summary>
    public sealed class InterfaceDeclaration : TypeDeclaration
    {
        internal InterfaceDeclaration(TypeReference typeReference)
            : base(typeReference)
        {
        }

        /// <summary>The declared members.</summary>
        public IReadOnlyCollection<MemberDeclaration> Members { get; internal set; }

        /// <summary>The interface generic parameters.</summary>
        public IReadOnlyList<GenericTypeParameterData> GenericParameters { get; internal set; }

        /// <summary>The base interfaces.</summary>
        public IReadOnlyCollection<TypeReference> BaseInterfaces { get; internal set; }

        /// <summary>The declared events.</summary>
        public IReadOnlyCollection<EventDeclaration> Events { get; internal set; }

        /// <summary>The declared properties.</summary>
        public IReadOnlyCollection<PropertyDeclaration> Properties { get; internal set; }

        /// <summary>The declared method.</summary>
        public IReadOnlyCollection<MethodDeclaration> Methods { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DeclarationNodeVisitor visitor)
            => visitor.VisitInterface(this);
    }
}