using System.IO;

namespace CodeMap.Documentation.Helpers
{
    public class TypeReferenceLink : HandlebarsContextualHelper<object>
    {
        public override string Name
            => nameof(TypeReferenceLink);

        public override void Apply(TextWriter writer, PageContext context, object parameter)
            => HandlebarsExtensions.WriteTemplate(writer, Name, context.WithData(parameter));
    }
}