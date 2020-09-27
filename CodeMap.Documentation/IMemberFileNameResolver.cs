using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.ReferenceData;

namespace CodeMap.Documentation
{
    public interface IMemberFileNameResolver
    {
        string GetFileName(DeclarationNode declatationNode);
        string GetUrl(MemberReference memberReference);
        string GetUrl(MemberInfo memberInfo);
    }
}