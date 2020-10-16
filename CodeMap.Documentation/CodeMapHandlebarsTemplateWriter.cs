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

        protected override IReadOnlyDictionary<string, string> GetTemplates()
            => new Dictionary<string, string>(base.GetTemplates(), StringComparer.OrdinalIgnoreCase)
            {
                { "Home", ReadFromEmbeddedResource(typeof(CodeMapHandlebarsTemplateWriter).Assembly, "CodeMap.Documentation.Templates.Home.hbs") },
                { "Navigation", ReadFromEmbeddedResource(typeof(CodeMapHandlebarsTemplateWriter).Assembly, "CodeMap.Documentation.Templates.Navigation.hbs") },
                { "DeprecationNotice", ReadFromEmbeddedResource(typeof(CodeMapHandlebarsTemplateWriter).Assembly, "CodeMap.Documentation.Templates.DeprecationNotice.hbs") }
            };
    }
}