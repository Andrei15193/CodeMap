using System.IO;

namespace CodeMap.Documentation.Helpers
{
    public class RelatedMembersList : HandlebarsContextualHelper<object>
    {
        public override string Name
            => nameof(RelatedMembersList);

        public override void Apply(TextWriter writer, PageContext context, object parameter)
            => HandlebarsExtensions.WriteTemplate(writer, Name, context.WithData(parameter));
    }
}