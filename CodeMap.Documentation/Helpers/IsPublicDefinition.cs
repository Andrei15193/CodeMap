using System;
using CodeMap.DeclarationNodes;

namespace CodeMap.Documentation.Helpers
{
    public class IsPublicDefinition : HandlebarsBooleanHelper<DeclarationNode>
    {
        public override string Name
            => nameof(IsPublicDefinition);

        public override bool Apply(PageContext context, DeclarationNode parameter)
            => parameter switch
            {
                MemberDeclaration memberDeclaration => memberDeclaration.AccessModifier >= AccessModifier.Family,
                TypeDeclaration typeDeclaration => typeDeclaration.AccessModifier >= AccessModifier.Family,
                _ => throw new ArgumentException($"Unhandled parameter type: '{parameter.GetType().Name}'")
            };
    }
}