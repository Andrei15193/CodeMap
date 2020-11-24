using CodeMap.DeclarationNodes;
using CodeMap.ReferenceData;

namespace CodeMap.Handlebars
{
    /// <summary>Exposes the interface of an object that resolves <see cref="DeclarationNode"/>s and <see cref="MemberReference"/>s.</summary>
    public interface IMemberReferenceResolver
    {
        /// <summary>Gets the URL (relative or absolute) for the given <paramref name="memberReference"/>.</summary>
        /// <param name="memberReference">The <see cref="MemberReference"/> for which to get the URL.</param>
        /// <returns>Returns the URL for the given <paramref name="memberReference"/>.</returns>
        string GetUrl(MemberReference memberReference);
    }
}