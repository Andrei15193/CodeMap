using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a documented delegate declaration.</summary>
    public class DelegateDocumentationElement : TypeDocumentationElement
    {
        internal DelegateDocumentationElement()
        {
        }

        /// <summary>The delegate generic parameters.</summary>
        public IReadOnlyList<TypeGenericParameterDocumentationElement> GenericParameters { get; internal set; }

        /// <summary>The delegate parameters.</summary>
        public IReadOnlyList<ParameterDocumentationElement> Parameters { get; internal set; }

        /// <summary>The delegate documented return value.</summary>
        public ReturnsDocumentationElement Return { get; internal set; }

        /// <summary>The delegate documented exceptions.</summary>
        public IReadOnlyCollection<ExceptionDocumentationElement> Exceptions { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
        {
            throw new NotImplementedException();
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>Determines whether the current <see cref="DelegateDocumentationElement"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="DelegateDocumentationElement"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
            => base.Equals(type)
            && GenericParameters.Count == type.GetGenericArguments().Length;
    }
}