using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeMap.ReferenceData;

namespace CodeMap.Handlebars
{
    /// <summary>
    /// A member reference resolver composed of multiple <see cref="IMemberReferenceResolver"/>s for generating different URLs based on
    /// the library towards which the reference was made.
    /// </summary>
    public class MemberReferenceResolver : IMemberReferenceResolver
    {
        private readonly IEnumerable<KeyValuePair<Assembly, IMemberReferenceResolver>> _memberReferenceResolvers;
        private readonly IMemberReferenceResolver _defaultMemberReferenceResolver;

        /// <summary>Initializes a new instance of the <see cref="MemberReferenceResolver"/> class.</summary>
        /// <param name="memberReferenceResolvers">A collection of <see cref="Assembly"/>s with associated <see cref="IMemberReferenceResolver"/>s used for determining which specfic <see cref="IMemberReferenceResolver"/>.</param>
        /// <param name="defaultMemberReferenceResolver">A default <see cref="IMemberReferenceResolver"/> to use in case no <see cref="IMemberReferenceResolver"/> matches an <see cref="Assembly"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="memberReferenceResolvers"/> is <c>null</c> or contains <c>null</c> values.</exception>
        public MemberReferenceResolver(IReadOnlyDictionary<Assembly, IMemberReferenceResolver> memberReferenceResolvers, IMemberReferenceResolver defaultMemberReferenceResolver)
        {
            if (memberReferenceResolvers is null || memberReferenceResolvers.Values.Contains(null))
                throw new ArgumentException("Cannot be 'null' or contain 'null' values.");

            _memberReferenceResolvers = memberReferenceResolvers;
            _defaultMemberReferenceResolver = defaultMemberReferenceResolver;
        }


        /// <summary>Initializes a new instance of the <see cref="MemberReferenceResolver"/> class.</summary>
        /// <param name="memberReferenceResolvers">A collection of <see cref="Assembly"/>s with associated <see cref="IMemberReferenceResolver"/>s used for determining which specfic <see cref="IMemberReferenceResolver"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="memberReferenceResolvers"/> is <c>null</c> or contains <c>null</c> values.</exception>
        public MemberReferenceResolver(IReadOnlyDictionary<Assembly, IMemberReferenceResolver> memberReferenceResolvers)
            : this(memberReferenceResolvers, null)
        {
        }

        /// <summary>Gets the URL (relative or absolute) for the given <paramref name="memberReference"/>.</summary>
        /// <param name="memberReference">The <see cref="MemberReference"/> for which to get the URL.</param>
        /// <returns>Returns the URL for the given <paramref name="memberReference"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="memberReference"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no suitable <see cref="IMemberReferenceResolver"/> was found for the provided <paramref name="memberReference"/>.</exception>
        public string GetUrl(MemberReference memberReference)
        {
            if (memberReference is null)
                throw new ArgumentNullException(nameof(memberReference));

            var matchedMemberReference = _memberReferenceResolvers
                .Where(pair => pair.Key == memberReference.Assembly)
                .Select(pair => pair.Value)
                .DefaultIfEmpty(_defaultMemberReferenceResolver)
                .FirstOrDefault();

            return matchedMemberReference is object ? matchedMemberReference.GetUrl(memberReference) : throw new InvalidOperationException("No matching IMemberReferenceResolver was found for the provided memberReference.");
        }
    }
}