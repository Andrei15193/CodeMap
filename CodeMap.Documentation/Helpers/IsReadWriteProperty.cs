using CodeMap.DeclarationNodes;

namespace CodeMap.Documentation.Helpers
{
    public class IsReadWriteProperty : HandlebarsBooleanHelper<PropertyDeclaration>
    {
        public override string Name
            => nameof(IsReadWriteProperty);

        public override bool Apply(PageContext context, PropertyDeclaration propertyDeclaration)
            => propertyDeclaration.Getter?.AccessModifier >= AccessModifier.Family
            && propertyDeclaration.Setter?.AccessModifier >= AccessModifier.Family;
    }
}