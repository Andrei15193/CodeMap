using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeMap.DocumentationElements
{
    /// <summary>Represents a lookup collection for <see cref="MemberDocumentation"/> objects.</summary>
    /// <remarks>
    /// <para>
    /// The lookup is done by searching for the canonical name of a <see cref="MemberInfo"/>, if a case sensitive (best match) is found
    /// then the <see cref="MemberDocumentation"/> of that member is returned. Otherwsie the first case insensitive match is returned.
    /// </para>
    /// </remarks>
    public class MemberDocumentationCollection : IReadOnlyCollection<MemberDocumentation>
    {
        private readonly ILookup<string, MemberDocumentation> _membersDocumentation;

        /// <summary>Initializes a new instance of the <see cref="MemberDocumentationCollection"/> class.</summary>
        /// <param name="membersDocumentation">A collection of <see cref="MemberDocumentation"/> objects to initialize the collection with.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="membersDocumentation"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="membersDocumentation"/> contains <c>null</c> items.</exception>
        public MemberDocumentationCollection(IEnumerable<MemberDocumentation> membersDocumentation)
        {
            _membersDocumentation = membersDocumentation
                ?.ToLookup(
                    memberDocumentation => (
                            memberDocumentation ?? throw new ArgumentException("Cannot contain 'null' member documentation items.", nameof(membersDocumentation))
                        )
                        .CanonicalName,
                    StringComparer.OrdinalIgnoreCase
                )
                ?? throw new ArgumentNullException(nameof(membersDocumentation));
        }

        /// <summary>Attempts to find a <see cref="MemberDocumentation"/> for the provided <paramref name="canonicalName"/>.</summary>
        /// <param name="canonicalName">The canonical name to search the related documentation for.</param>
        /// <param name="result">The related <see cref="MemberDocumentation"/> when found; <c>null</c> otherwise.</param>
        /// <returns>Returns <c>true</c> if a match was found; <c>false</c> otherwise.</returns>
        public bool TryFind(string canonicalName, out MemberDocumentation result)
        {
            result = null;
            if (!_membersDocumentation.Contains(canonicalName))
                return false;

            var membersDocumentation = _membersDocumentation[canonicalName];
            var foundBestMatch = false;
            using (var memberDocumentation = membersDocumentation.GetEnumerator())
            {
                if (memberDocumentation.MoveNext())
                    result = memberDocumentation.Current;

                do
                    if (canonicalName.Equals(memberDocumentation.Current.CanonicalName, StringComparison.Ordinal))
                    {
                        result = memberDocumentation.Current;
                        foundBestMatch = true;
                    }
                while (memberDocumentation.MoveNext() && !foundBestMatch);
            }

            return result != null;
        }

        /// <summary>Gets the total number of <see cref="MemberDocumentation"/> items in the collection.</summary>
        public int Count
            => _membersDocumentation.Sum(Enumerable.Count);

        /// <summary>Creates an enumerator that iterates through the collection.</summary>
        /// <returns>Returns an enumerator that iterates through the collection.</returns>
        public IEnumerator<MemberDocumentation> GetEnumerator()
            => _membersDocumentation.SelectMany(Enumerable.AsEnumerable).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}