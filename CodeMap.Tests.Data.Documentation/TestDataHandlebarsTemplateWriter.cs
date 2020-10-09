using System;
using System.Collections.Generic;
using CodeMap.Handlebars;

namespace CodeMap.Tests.Data.Documentation
{
    public class TestDataHandlebarsTemplateWriter : HandlebarsTemplateWriter
    {
        public TestDataHandlebarsTemplateWriter(IMemberReferenceResolver memberReferenceResolver)
            : base(memberReferenceResolver)
        {
        }

        protected override IReadOnlyDictionary<string, string> GetPartials()
            => new Dictionary<string, string>(base.GetPartials(), StringComparer.OrdinalIgnoreCase)
            {
                ["Layout"] = ReadFromEmbeddedResource(typeof(TestDataHandlebarsTemplateWriter).Assembly, "CodeMap.Tests.Data.Documentation.Partials.Layout.hbs")
            };
    }
}