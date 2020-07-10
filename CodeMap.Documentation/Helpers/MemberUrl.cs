using System;
using System.IO;
using CodeMap.DeclarationNodes;
using CodeMap.ReferenceData;

namespace CodeMap.Documentation.Helpers
{
    public class MemberUrl : HandlebarsContextualHelper<object>
    {
        public override string Name
            => nameof(MemberUrl);

        public override void Apply(TextWriter writer, PageContext context, object parameter)
        {
            switch (parameter)
            {
                case DeclarationNode declarationNode:
                    writer.Write(context.MemberFileNameResolver.GetFileName(declarationNode));
                    break;

                case ArrayTypeReference arrayTypeReference:
                    Apply(writer, context, arrayTypeReference.ItemType);
                    break;

                case MemberReference memberReference:
                    writer.Write(context.MemberFileNameResolver.GetFileName(memberReference));
                    break;

                default:
                    throw new ArgumentException($"Unhandled parameter type: '{parameter.GetType().Name}'");
            }
        }
    }
}