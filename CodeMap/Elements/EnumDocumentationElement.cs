﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a documented enum declaration.</summary>
    public class EnumDocumentationElement : TypeDocumentationElement
    {
        internal EnumDocumentationElement()
        {
        }

        /// <summary>The underlying type of the enum members.</summary>
        public TypeReferenceData UnderlyingType { get; internal set; }

        /// <summary>The enum members.</summary>
        public IReadOnlyList<ConstantDocumentationElement> Members { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
        {
            visitor.VisitEnum(this);
            foreach (var member in Members)
                member.Accept(visitor);
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
        {
            await visitor.VisitEnumAsync(this, cancellationToken);
            foreach (var member in Members)
                await member.AcceptAsync(visitor, cancellationToken);
        }
    }
}