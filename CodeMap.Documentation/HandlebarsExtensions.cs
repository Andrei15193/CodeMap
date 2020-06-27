using CodeMap.DeclarationNodes;
using CodeMap.Documentation.Helpers;
using CodeMap.DocumentationElements;
using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeMap.Documentation
{
    public static class HandlebarsExtensions
    {
        private readonly static IHandlebars _handlebars = Handlebars
            .Create()
            .RegisterHelper(new HasAny())
            .RegisterHelper(new Join())
            .RegisterHelper(new MemberName())
            .RegisterHelper(new MemberUrl())
            .RegisterHelper(new MemberLink())
            .RegisterHelper(new Format())
            .RegisterHelper(new Pygments())
            .RegisterHelper(new HasPublicDefinitions())
            .RegisterHelper(new IsPublicDefinition())
            .RegisterHelper(new DocumentationContent())
            .RegisterHelper(new PageContent())
            .RegisterHelper(new MemberDefinitionsList());
        private readonly static IReadOnlyDictionary<string, Action<TextWriter, object>> _templates = typeof(Program)
            .Assembly
            .GetManifestResourceNames()
            .Select(resource => (Resource: resource, Match: Regex.Match(resource, @"Templates(\.\w+)*\.(?<templateName>\w+)\.hbs$", RegexOptions.IgnoreCase)))
            .Where(item => item.Match.Success)
            .ToDictionary(
                item => item.Match.Groups["templateName"].Value,
                item =>
                {
                    using var assetStream = typeof(Program).Assembly.GetManifestResourceStream(item.Resource);
                    using var streamReader = new StreamReader(assetStream);
                    return _handlebars.Compile(streamReader);
                },
                StringComparer.OrdinalIgnoreCase
            );

        public static IHandlebars RegisterHelper(this IHandlebars handlebars, IHandlebarsHelper handlebarsHelper)
        {
            handlebars.RegisterHelper(handlebarsHelper.Name, handlebarsHelper.Apply);
            return handlebars;
        }

        public static IHandlebars RegisterHelper(this IHandlebars handlebars, IHandlebarsBlockHelper handlebarsBlockHelper)
        {
            handlebars.RegisterHelper(handlebarsBlockHelper.Name, handlebarsBlockHelper.Apply);
            return handlebars;
        }

        public static void WritePage<TDeclarationNode>(this DirectoryInfo directoryInfo, string page, TDeclarationNode declarationNode)
                where TDeclarationNode : DeclarationNode
        {
            using var fileStream = new FileStream(Path.Combine(directoryInfo.FullName, page), FileMode.Create, FileAccess.Write);
            using var streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
            _templates["Page"](streamWriter, new PageContext<TDeclarationNode>(declarationNode));
        }

        public static void WriteTemplate(this TextWriter textWriter, string template, PageContext context)
            => _templates[template](textWriter, context);

        public static void WriteTemplate<TDocumentationElement>(this TextWriter textWriter, string template, TemplateContext<TDocumentationElement> context)
                where TDocumentationElement : DocumentationElement
            => _templates[template](textWriter, context);

        public static void WriteTemplate(TextWriter textWriter, string template, dynamic context)
            => _templates[template](textWriter, context);
    }
}