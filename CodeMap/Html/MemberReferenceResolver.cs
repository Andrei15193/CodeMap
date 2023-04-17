using CodeMap.ReferenceData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeMap.Html
{
    /// <summary>
    /// A member reference resolver composed of multiple <see cref="IMemberReferenceResolver"/>s for generating different URLs based on
    /// the library towards which the reference was made.
    /// </summary>
    public class MemberReferenceResolver : IMemberReferenceResolver, IEnumerable<IMemberReferenceResolver>
    {
        /// <summary>Creates an <see cref="IMemberReferenceResolver"/> based on the provided <paramref name="urlResolver"/> callback.</summary>
        /// <param name="urlResolver">A callback that resolves the URL for a given <see cref="MemberReference"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="urlResolver"/> is <c>null</c>.</exception>
        public static IMemberReferenceResolver Create(Func<MemberReference, string> urlResolver)
            => new CallbackMemberReferenceResolver(urlResolver ?? throw new ArgumentNullException(nameof(urlResolver)));

        private readonly IMemberReferenceResolver _defaultMemberReferenceResolver;
        private readonly ICollection<Func<MemberReference, IMemberReferenceResolver>> _memberReferenceResolverFactories = new List<Func<MemberReference, IMemberReferenceResolver>>();

        /// <summary>Initializes a new instance of the <see cref="MemberReferenceResolver"/> class.</summary>
        /// <param name="defaultMemberReferenceResolver">A default <see cref="IMemberReferenceResolver"/> to use in case no <see cref="IMemberReferenceResolver"/> can be found otherwise.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="defaultMemberReferenceResolver"/> is <c>null</c>.</exception>
        public MemberReferenceResolver(IMemberReferenceResolver defaultMemberReferenceResolver)
            => _defaultMemberReferenceResolver = defaultMemberReferenceResolver ?? throw new ArgumentNullException(nameof(defaultMemberReferenceResolver));

        /// <summary>Adds the provided <paramref name="memberReferenceResolver"/> as an <see cref="IMemberReferenceResolver"/> when resolving references to members belonging to the provided <paramref name="assembly"/>.</summary>
        /// <param name="assembly">An assembly for which to resolve <see cref="MemberReference"/>s using the provided <paramref name="memberReferenceResolver"/>.</param>
        /// <param name="memberReferenceResolver">The <see cref="IMemberReferenceResolver"/> to use when resolving URLs for members of the provided <paramref name="assembly"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> or <paramref name="memberReferenceResolver"/> are <c>null</c>.</exception>
        public void Add(Assembly assembly, IMemberReferenceResolver memberReferenceResolver)
        {
            if (assembly is null)
                throw new ArgumentNullException(nameof(assembly));
            if (memberReferenceResolver is null)
                throw new ArgumentNullException(nameof(memberReferenceResolver));

            _memberReferenceResolverFactories.Add(memberReference => memberReference.Assembly == assembly ? memberReferenceResolver : null);
        }

        /// <summary>Adds the provided <paramref name="memberReferenceResolverFactory"/> for lookups when trying to resolve an URL.</summary>
        /// <param name="memberReferenceResolverFactory">A callback that will create a specific <see cref="IMemberReferenceResolver"/> for a <see cref="MemberReference"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="memberReferenceResolverFactory"/> is <c>null</c>.</exception>
        public void Add(Func<MemberReference, IMemberReferenceResolver> memberReferenceResolverFactory)
            => _memberReferenceResolverFactories.Add(memberReferenceResolverFactory ?? throw new ArgumentNullException(nameof(memberReferenceResolverFactory)));


        /// <summary>Gets the URL (relative or absolute) for the given <paramref name="memberReference"/>.</summary>
        /// <param name="memberReference">The <see cref="MemberReference"/> for which to get the URL.</param>
        /// <returns>Returns the URL for the given <paramref name="memberReference"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="memberReference"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when a URL could not be generated.</exception>
        public string GetUrl(MemberReference memberReference)
        {
            if (memberReference is null)
                throw new ArgumentNullException(nameof(memberReference));

            var memberReferenceResolver = _memberReferenceResolverFactories
                .Select(memberReferenceResolverFactory => memberReferenceResolverFactory(memberReference))
                .Where(memberReferenceResolver => memberReferenceResolver != null)
                .DefaultIfEmpty(_defaultMemberReferenceResolver)
                .First();

            var url = memberReferenceResolver.GetUrl(memberReference);
            if (url is null)
                throw new InvalidOperationException();
            else
                return url;
        }

        IEnumerator<IMemberReferenceResolver> IEnumerable<IMemberReferenceResolver>.GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield break;
        }

        private sealed class CallbackMemberReferenceResolver : IMemberReferenceResolver
        {
            private readonly Func<MemberReference, string> _urlResolver;

            public CallbackMemberReferenceResolver(Func<MemberReference, string> urlResolver)
                => _urlResolver = urlResolver;

            public string GetUrl(MemberReference memberReference)
                => _urlResolver(memberReference);
        }
    }
}