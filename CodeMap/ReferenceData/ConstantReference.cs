using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a reference to a constant field.</summary>
    public sealed class ConstantReference : MemberReference, IEquatable<FieldInfo>
    {
        internal ConstantReference()
        {
        }

        /// <summary>The name of the constant.</summary>
        public string Name { get; internal set; }

        /// <summary>The value of the constant.</summary>
        public object Value { get; internal set; }

        /// <summary>The type declaring the constant.</summary>
        public TypeReference DeclaringType { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override void Accept(MemberReferenceVisitor visitor)
        {
            if (visitor == null)
                throw new ArgumentNullException(nameof(visitor));

            visitor.VisitConstant(this);
        }

        /// <summary>Asynchronously accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override Task AcceptAsync(MemberReferenceVisitor visitor, CancellationToken cancellationToken)
        {
            if (visitor == null)
                return Task.FromException(new ArgumentNullException(nameof(visitor)));

            return visitor.VisitConstantAsync(this, cancellationToken);
        }

        /// <summary>Determines whether the current <see cref="ConstantReference"/> is equal to the provided <paramref name="memberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="ConstantReference"/> references the provided <paramref name="memberInfo"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(MemberInfo memberInfo)
            => memberInfo is FieldInfo fieldInfo && Equals(fieldInfo);

        /// <summary>Determines whether the current <see cref="ConstantReference"/> is equal to the provided <paramref name="fieldInfo"/>.</summary>
        /// <param name="fieldInfo">The <see cref="FieldInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="ConstantReference"/> references the provided <paramref name="fieldInfo"/>; <c>false</c> otherwise.</returns>
        public bool Equals(FieldInfo fieldInfo)
            => fieldInfo != null
            && fieldInfo.IsLiteral
            && string.Equals(fieldInfo.Name, Name, StringComparison.OrdinalIgnoreCase);
    }
}