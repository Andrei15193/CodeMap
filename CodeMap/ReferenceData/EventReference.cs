using System;
using System.Reflection;

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

        /// <summary>The declaring assembly.</summary>
        public override AssemblyReference Assembly
            => DeclaringType.Assembly;

        /// <summary>Accepts the provided <paramref name="visitor"/> for selecting a concrete instance method.</summary>
        /// <param name="visitor">The <see cref="MemberReferenceVisitor"/> interpreting the reference data.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="visitor"/> is <c>null</c>.</exception>
        public override void Accept(MemberReferenceVisitor visitor)
            => visitor.VisitEvent(this);

        /// <summary>Determines whether the current <see cref="EventReference"/> is equal to the provided <paramref name="memberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="EventReference"/> references the provided <paramref name="memberInfo"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(MemberInfo memberInfo)
            => Equals(memberInfo as EventInfo);

        /// <summary>Determines whether the current <see cref="EventReference"/> is equal to the provided <paramref name="eventInfo"/>.</summary>
        /// <param name="eventInfo">The <see cref="EventInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="EventReference"/> references the provided <paramref name="eventInfo"/>; <c>false</c> otherwise.</returns>
        public bool Equals(EventInfo eventInfo)
            => eventInfo != null
               && Name.Equals(eventInfo.Name, StringComparison.OrdinalIgnoreCase)
               && DeclaringType.Equals(eventInfo.DeclaringType);

        /// <summary>Determines whether the current <see cref="EventReference"/> is equal to the provided <paramref name="assemblyName"/>.</summary>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="EventReference"/> references the provided <paramref name="assemblyName"/>; <c>false</c> otherwise.</returns>
        /// <remarks>This method always returns <c>false</c> because an <see cref="AssemblyName"/> cannot represent an event reference.</remarks>
        public sealed override bool Equals(AssemblyName assemblyName)
            => false;
    }
}