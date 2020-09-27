using System;
using System.IO;
using System.Linq;
using CodeMap.DeclarationNodes;
using CodeMap.ReferenceData;

namespace CodeMap.Documentation.Helpers
{
    public class MemberUrl : IHandlebarsHelper
    {
        private readonly PageContext _pageContext;

        public MemberUrl(PageContext pageContext)
            => _pageContext = pageContext;

        public string Name
            => nameof(MemberUrl);

        public void Apply(TextWriter writer, object context, params object[] parameters)
        {
            var parameter = parameters.DefaultIfEmpty(context).First();
            switch (parameter)
            {
                case DeclarationNode declarationNode:
                    writer.Write(_pageContext.MemberFileNameResolver.GetFileName(declarationNode));
                    break;

                case ArrayTypeReference arrayTypeReference:
                    Apply(writer, _pageContext, arrayTypeReference.ItemType);
                    break;

                case MemberReference memberReference:
                    writer.Write(_pageContext.MemberFileNameResolver.GetUrl(memberReference));
                    break;

                default:
                    throw new ArgumentException($"Unhandled parameter type: '{parameter.GetType().Name}'");
            }
        }
    }
}