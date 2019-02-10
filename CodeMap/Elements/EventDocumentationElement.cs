﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a documented event declared by a type.</summary>
    public sealed class EventDocumentationElement : MemberDocumentationElement
    {
        internal EventDocumentationElement()
        {
        }

        /// <summary>The event type.</summary>
        public TypeReferenceData Type { get; internal set; }

        /// <summary>Indicates whether the event is static.</summary>
        public bool IsStatic { get; internal set; }

        /// <summary>Indicates whether the event has been marked as virtual.</summary>
        public bool IsVirtual { get; internal set; }

        /// <summary>Indicates whether the event has been marked as abstract.</summary>
        public bool IsAbstract { get; internal set; }

        /// <summary>Indicates whether the event is an override.</summary>
        public bool IsOverride { get; internal set; }

        /// <summary>Indicates whether the event has been marked as sealed.</summary>
        public bool IsSealed { get; internal set; }

        /// <summary>Indicates whether the event hides a member from a base type.</summary>
        public bool IsShadowing { get; internal set; }

        /// <summary>Information about the adder accessor.</summary>
        public EventAccessorData Adder { get; internal set; }

        /// <summary>Information about the remover accessor.</summary>
        public EventAccessorData Remover { get; internal set; }

        /// <summary>Documented exceptions that might be thrown by subscribers.</summary>
        public IReadOnlyCollection<ExceptionDocumentationElement> Exceptions { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
            => visitor.VisitEvent(this);

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
            => visitor.VisitEventAsync(this, cancellationToken);
    }
}