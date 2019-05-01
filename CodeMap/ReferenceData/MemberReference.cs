#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents an Assembly member reference.</summary>
    public abstract class MemberReference : IEquatable<MemberInfo>
    {
        /// <summary>Determines whether the provided <paramref name="memberReference"/> and <paramref name="memberInfo"/> are equal.</summary>
        /// <param name="memberReference">The <see cref="MemberReference"/> to compare.</param>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(MemberReference memberReference, MemberInfo memberInfo)
            => Equals(memberReference, memberInfo);

        /// <summary>Determines whether the provided <paramref name="memberReference"/> and <paramref name="memberInfo"/> are not equal.</summary>
        /// <param name="memberReference">The <see cref="MemberReference"/> to compare.</param>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(MemberReference memberReference, MemberInfo memberInfo)
            => !Equals(memberReference, memberInfo);

        /// <summary>Determines whether the provided <paramref name="memberReference"/> and <paramref name="memberInfo"/> are equal.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare.</param>
        /// <param name="memberReference">The <see cref="MemberReference"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(MemberInfo memberInfo, MemberReference memberReference)
            => Equals(memberReference, memberInfo);

        /// <summary>Determines whether the provided <paramref name="memberReference"/> and <paramref name="memberInfo"/> are not equal.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare.</param>
        /// <param name="memberReference">The <see cref="MemberReference"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(MemberInfo memberInfo, MemberReference memberReference)
            => !Equals(memberReference, memberInfo);

        internal MemberReference()
        {
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public abstract void Accept(MemberReferenceVisitor visitor);

        /// <summary>Asynchronously accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public Task AcceptAsync(MemberReferenceVisitor visitor)
            => AcceptAsync(visitor, CancellationToken.None);

        /// <summary>Asynchronously accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public abstract Task AcceptAsync(MemberReferenceVisitor visitor, CancellationToken cancellationToken);

        /// <summary>Determines whether the current <see cref="MemberReference"/> is equal to the provided <paramref name="obj"/>.</summary>
        /// <param name="obj">The <see cref="object"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="MemberReference"/> references the provided <paramref name="obj"/>; <c>false</c> otherwise.</returns>
        /// <remarks>
        /// If the provided <paramref name="obj"/> is a <see cref="MemberInfo"/> instance then the comparison is done by comparing members and
        /// determining whether the current instance actually maps to the provided <see cref="MemberInfo"/>. Otherwise the equality is determined
        /// by comparing references.
        /// </remarks>
        public sealed override bool Equals(object obj)
            => obj != null
            && obj is MemberInfo memberInfo
                ? Equals(memberInfo)
                : base.Equals(obj);

        /// <summary>Determines whether the current <see cref="MemberReference"/> is equal to the provided <paramref name="memberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="MemberInfo"/> references the provided <paramref name="memberInfo"/>; <c>false</c> otherwise.</returns>
        public abstract bool Equals(MemberInfo memberInfo);
    }
}