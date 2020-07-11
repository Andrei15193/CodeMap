using System.Collections.Generic;
using System.IO;
using CodeMap.DocumentationElements;

namespace CodeMap.Documentation.Helpers
{
    public class RelatedMembersList : HandlebarsContextualHelper<IEnumerable<MemberReferenceDocumentationElement>>
    {
        public override string Name
            => nameof(RelatedMembersList);

        public override void Apply(TextWriter writer, PageContext context, IEnumerable<MemberReferenceDocumentationElement> memberReferences)
            => HandlebarsExtensions.WriteTemplate(writer, Name, context.WithData(memberReferences));
    }
}