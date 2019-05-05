using CodeMap.ReferenceData;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a documented interface declaration.</summary>
    public sealed class InterfaceDocumentationElement : TypeDocumentationElement
    {
        internal InterfaceDocumentationElement()
        {
        }

        /// <summary>The declared members.</summary>
        public IReadOnlyCollection<MemberDocumentationElement> Members { get; internal set; }

        /// <summary>The interface generic parameters.</summary>
        public IReadOnlyList<TypeGenericParameterData> GenericParameters { get; internal set; }

        /// <summary>The base interfaces.</summary>
        public IReadOnlyCollection<TypeReference> BaseInterfaces { get; internal set; }

        /// <summary>The declared events.</summary>
        public IReadOnlyCollection<EventDocumentationElement> Events { get; internal set; }

        /// <summary>The declared properties.</summary>
        public IReadOnlyCollection<PropertyDocumentationElement> Properties { get; internal set; }

        /// <summary>The declared method.</summary>
        public IReadOnlyCollection<MethodDocumentationElement> Methods { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
            => visitor.VisitInterface(this);

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
            => visitor.VisitInterfaceAsync(this, cancellationToken);

        /// <summary>Determines whether the current <see cref="InterfaceDocumentationElement"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="InterfaceDocumentationElement"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
            => base.Equals(type)
            && GenericParameters.Count == (type.GetGenericArguments().Length - (type.DeclaringType?.GetGenericArguments().Length ?? 0));
    }
}