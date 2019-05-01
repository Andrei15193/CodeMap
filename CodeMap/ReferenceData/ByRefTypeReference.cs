using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents the type of a value passed by ref.</summary>
    public sealed class ByRefTypeReference : BaseTypeReference
    {
        internal ByRefTypeReference()
        {
        }

        /// <summary>The value type passed by ref.</summary>
        public BaseTypeReference ReferentType { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override void Accept(MemberReferenceVisitor visitor)
            => visitor.VisitByRef(this);

        /// <summary>Asynchronously accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override Task AcceptAsync(MemberReferenceVisitor visitor, CancellationToken cancellationToken)
            => visitor.VisitByRefAsync(this, cancellationToken);

        /// <summary>Determines whether the current <see cref="ByRefTypeReference"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="ByRefTypeReference"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
            => type != null
            && type.IsByRef
            && ReferentType == type.GetElementType();
    }
}