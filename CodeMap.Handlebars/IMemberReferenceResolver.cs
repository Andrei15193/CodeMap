using CodeMap.DeclarationNodes;
using CodeMap.ReferenceData;

namespace CodeMap.Handlebars
{
    public interface IMemberReferenceResolver
    {
        string GetFileName(DeclarationNode declarationNode);
        string GetUrl(MemberReference memberReference);
    }
}