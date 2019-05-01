using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.ReferenceData
{
    /// <summary>Represents an event reference.</summary>
    public sealed class EventReference : MemberReference, IEquatable<EventInfo>
    {
        internal EventReference()
        {
        }

        /// <summary>The event name.</summary>
        public string Name { get; internal set; }

        /// <summary>The event declaring type.</summary>
        public TypeReference DeclaringType { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override void Accept(MemberReferenceVisitor visitor)
            => visitor.VisitEvent(this);

        /// <summary>Asynchronously accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override Task AcceptAsync(MemberReferenceVisitor visitor, CancellationToken cancellationToken)
            => visitor.VisitEventAsync(this, cancellationToken);

        /// <summary>Determines whether the current <see cref="EventReference"/> is equal to the provided <paramref name="memberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="EventReference"/> references the provided <paramref name="memberInfo"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(MemberInfo memberInfo)
            => memberInfo is EventInfo eventInfo && Equals(eventInfo);

        /// <summary>Determines whether the current <see cref="EventReference"/> is equal to the provided <paramref name="eventInfo"/>.</summary>
        /// <param name="eventInfo">The <see cref="MemberInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="EventReference"/> references the provided <paramref name="eventInfo"/>; <c>false</c> otherwise.</returns>
        public bool Equals(EventInfo eventInfo)
            => eventInfo != null
            && string.Equals(Name, eventInfo.Name, StringComparison.OrdinalIgnoreCase)
            && DeclaringType == eventInfo.DeclaringType;
    }
}