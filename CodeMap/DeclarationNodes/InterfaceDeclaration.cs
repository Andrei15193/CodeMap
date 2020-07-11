using System;
using System.Collections.Generic;
using CodeMap.ReferenceData;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented interface declaration.</summary>
    public sealed class InterfaceDeclaration : TypeDeclaration
    {
        internal InterfaceDeclaration()
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

        /// <summary>Determines whether the current <see cref="InterfaceDeclaration"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="InterfaceDeclaration"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
            => base.Equals(type)
            && GenericParameters.Count == (type.GetGenericArguments().Length - (type.DeclaringType?.GetGenericArguments().Length ?? 0));
    }
}