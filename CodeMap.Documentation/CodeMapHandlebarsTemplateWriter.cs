using System;
using System.Collections.Generic;
using CodeMap.Handlebars;

namespace CodeMap.Documentation
{
    public class CodeMapHandlebarsTemplateWriter : HandlebarsTemplateWriter
    {
        public CodeMapHandlebarsTemplateWriter(IMemberReferenceResolver memberReferenceResolver)
            : base(memberReferenceResolver)
        {
        }

        protected override IReadOnlyDictionary<string, string> GetPartials()
            => new Dictionary<string, string>(base.GetPartials(), StringComparer.OrdinalIgnoreCase)
            {
                ["Layout"] = ReadFromEmbeddedResource(typeof(CodeMapHandlebarsTemplateWriter).Assembly, "CodeMap.Documentation.Partials.Layout.hbs")
            };
    }
}