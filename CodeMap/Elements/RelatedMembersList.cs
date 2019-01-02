using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CodeMap.Elements
{
    /// <summary>Represents a list of references corresponding to all <c>seealso</c> XML elements specified on a member.</summary>
    public sealed class RelatedMembersList : IReadOnlyList<MemberReferenceDocumentationElement>
    {
        private readonly IReadOnlyList<MemberReferenceDocumentationElement> _relatedMembers;

        internal RelatedMembersList(IEnumerable<MemberReferenceDocumentationElement> relatedMembers)
        {
            _relatedMembers = relatedMembers as IReadOnlyList<MemberReferenceDocumentationElement>
                ?? relatedMembers?.ToList()
                ?? throw new ArgumentNullException(nameof(relatedMembers));
            if (_relatedMembers.Contains(null))
                throw new ArgumentException("Cannot contain 'null' references.", nameof(relatedMembers));
        }

        /// <summary>Gets the <see cref="MemberReferenceDocumentationElement"/> found at the specified <paramref name="index"/>.</summary>
        /// <param name="index">The index from which to return a member reference.</param>
        /// <returns>Returns the <see cref="MemberReferenceDocumentationElement"/> found at the specified <paramref name="index"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is less than 0 (zero) or greater than or equal to <see cref="Count"/>.</exception>
        public MemberReferenceDocumentationElement this[int index]
            => _relatedMembers[index];

        /// <summary>The number of referred members.</summary>
        public int Count
            => _relatedMembers.Count;

        /// <summary>Gets an <see cref="IEnumerator{T}"/> that iterates through the collection.</summary>
        /// <returns>Returns an enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<MemberReferenceDocumentationElement> GetEnumerator()
            => _relatedMembers.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _relatedMembers.GetEnumerator();
    }
}