using System;
using System.Collections.Generic;
using System.Linq;
using CodeMap.DeclarationNodes;

namespace CodeMap.Documentation.Helpers
{
    public class HasPublicDefinitions : HandlebarsBooleanHelper<IEnumerable<DeclarationNode>>
    {
        public override string Name
            => nameof(HasPublicDefinitions);

        public override bool Apply(PageContext context, IEnumerable<DeclarationNode> parameter)
            => parameter switch
            {
                IEnumerable<MemberDeclaration> memberDeclarations => memberDeclarations.Any(memberDeclaration => memberDeclaration.AccessModifier >= AccessModifier.Family),
                IEnumerable<TypeDeclaration> typeDeclarations => typeDeclarations.Any(typeDeclaration => typeDeclaration.AccessModifier >= AccessModifier.Family),
                _ => throw new ArgumentException($"Unhandled parameter type: '{parameter.GetType().Name}'")
            };
    }
}