﻿using CodeMap.DocumentationElements;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented constructor declared by a type.</summary>
    public class ConstructorDeclaration : MemberDeclaration
    {
        internal ConstructorDeclaration()
        {
        }

        /// <summary>The constructor parameters.</summary>
        public IReadOnlyList<ParameterData> Parameters { get; internal set; }

        /// <summary>Documented exceptions that might be thrown when calling the constructor.</summary>
        public IReadOnlyCollection<ExceptionData> Exceptions { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DeclarationNodeVisitor visitor)
            => visitor.VisitConstructor(this);

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task AcceptAsync(DeclarationNodeVisitor visitor, CancellationToken cancellationToken)
            => visitor.VisitConstructorAsync(this, cancellationToken);
    }
}