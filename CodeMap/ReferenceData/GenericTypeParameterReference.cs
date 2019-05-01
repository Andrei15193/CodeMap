using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a generic type parameter reference.</summary>
    public sealed class GenericTypeParameterReference : GenericParameterReference
    {
        internal GenericTypeParameterReference()
        {
        }

        /// <summary>The type declaring the generic parameter.</summary>
        public TypeReference DeclaringType { get; internal set; }

        /// <summary>Asynchronously accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        public override void Accept(MemberReferenceVisitor visitor)
        {
            if (visitor == null)
                throw new ArgumentNullException(nameof(visitor));

            visitor.VisitGenericTypeParameter(this);
        }

        /// <summary>Asynchronously accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task AcceptAsync(MemberReferenceVisitor visitor, CancellationToken cancellationToken)
        {
            if (visitor == null)
                return Task.FromException(new ArgumentNullException(nameof(visitor)));

            return visitor.VisitGenericTypeParameterAsync(this, cancellationToken);
        }

        /// <summary>Determines whether the current <see cref="GenericTypeParameterReference"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="GenericTypeParameterReference"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
            => base.Equals(type)
            && DeclaringType == type.DeclaringType;
    }
}