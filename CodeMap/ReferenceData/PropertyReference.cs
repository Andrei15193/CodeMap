using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents a property reference.</summary>
    public sealed class PropertyReference : MemberReference, IEquatable<PropertyInfo>
    {
        internal PropertyReference()
        {
        }

        /// <summary>The property name.</summary>
        public string Name { get; internal set; }

        /// <summary>The property declaring type.</summary>
        public TypeReference DeclaringType { get; internal set; }

        /// <summary>The property parameter types.</summary>
        public IReadOnlyList<BaseTypeReference> ParameterTypes { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override void Accept(MemberReferenceVisitor visitor)
            => visitor.VisitProperty(this);

        /// <summary>Asynchronously accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override Task AcceptAsync(MemberReferenceVisitor visitor, CancellationToken cancellationToken)
            => visitor.VisitPropertyAsync(this, cancellationToken);

        /// <summary>Determines whether the current <see cref="PropertyReference"/> is equal to the provided <paramref name="memberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="PropertyReference"/> references the provided <paramref name="memberInfo"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(MemberInfo memberInfo)
            => memberInfo is PropertyInfo propertyInfo && Equals(propertyInfo);

        /// <summary>Determines whether the current <see cref="PropertyReference"/> is equal to the provided <paramref name="propertyInfo"/>.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="PropertyReference"/> references the provided <paramref name="propertyInfo"/>; <c>false</c> otherwise.</returns>
        public bool Equals(PropertyInfo propertyInfo)
        {
            throw new NotImplementedException();
        }
    }
}