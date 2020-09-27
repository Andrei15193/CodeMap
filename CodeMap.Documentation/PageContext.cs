﻿using System.IO;

namespace CodeMap.Documentation
{
    public class PageContext
    {
        internal PageContext(IMemberFileNameResolver memberFileNameResolver)
            => MemberFileNameResolver = memberFileNameResolver;

        public IMemberFileNameResolver MemberFileNameResolver { get; }

        internal HandlebarsTemplateWriter TemplateWriter { get; set; }

        public void ApplyTemplate(TextWriter textWriter, string templateName, object context)
            => TemplateWriter.Write(textWriter, templateName, context);

        public string ApplyTemplate(string templateName, object context)
            => TemplateWriter.Apply(templateName, context);
    }
}