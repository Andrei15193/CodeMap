using CodeMap.ReferenceData;

namespace CodeMap.Html
{
    /// <summary>Exposes the interface for generating URLs for <see cref="MemberReference"/>s.</summary>
    public interface IMemberReferenceResolver
    {
        /// <summary>Gets the URL (relative or absolute) for the provided <paramref name="memberReference"/>.</summary>
        /// <param name="memberReference">The <see cref="MemberReference"/> for which to get the URL.</param>
        /// <returns>Returns the URL for the provided <paramref name="memberReference"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="memberReference"/> is <c>null</c>.</exception>
        string GetUrl(MemberReference memberReference);
    }
}