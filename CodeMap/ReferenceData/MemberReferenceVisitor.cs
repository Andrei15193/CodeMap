using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a visitor for <see cref="MemberReference"/> instances.</summary>
    public abstract class MemberReferenceVisitor
    {
        /// <summary>Visits the given <paramref name="typeReference"/>.</summary>
        /// <param name="typeReference">The <see cref="TypeReference"/> to visit.</param>
        protected internal abstract void VisitTypeReference(TypeReference typeReference);

        /// <summary>Asynchronously visits the given <paramref name="typeReference"/>.</summary>
        /// <param name="typeReference">The <see cref="TypeReference"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitTypeReferenceAsync(TypeReference typeReference, CancellationToken cancellationToken)
        {
            try
            {
                VisitTypeReference(typeReference);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }
    }
}