using CodeMap.DeclarationNodes;

namespace CodeMap.Documentation.Helpers
{
    public class IsReadOnlyProperty : HandlebarsBooleanHelper<PropertyDeclaration>
    {
        public override string Name
            => nameof(IsReadOnlyProperty);

        public override bool Apply(PageContext context, PropertyDeclaration propertyDeclaration)
            => propertyDeclaration.Getter?.AccessModifier >= AccessModifier.Family
            && (propertyDeclaration.Setter?.AccessModifier ?? AccessModifier.Private) < AccessModifier.Family;
    }
}