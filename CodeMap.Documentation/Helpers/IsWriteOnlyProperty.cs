using CodeMap.DeclarationNodes;

namespace CodeMap.Documentation.Helpers
{
    public class IsWriteOnlyProperty : HandlebarsBooleanHelper<PropertyDeclaration>
    {
        public override string Name
            => nameof(IsWriteOnlyProperty);

        public override bool Apply(PageContext context, PropertyDeclaration propertyDeclaration)
            => (propertyDeclaration.Getter?.AccessModifier ?? AccessModifier.Private) < AccessModifier.Family
            && propertyDeclaration.Setter?.AccessModifier >= AccessModifier.Family;
    }
}