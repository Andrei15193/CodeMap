using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CodeMap.Handlebars.Helpers;

namespace CodeMap.Handlebars
{
    public class HandlebarsTemplateWriter : TemplateWriter
    {
        private readonly PageContext _pageContext;
        private readonly Lazy<IReadOnlyDictionary<string, Action<TextWriter, object>>> _templates;

        public HandlebarsTemplateWriter(PageContext pageContext)
            => (_pageContext, _templates) = (pageContext, new Lazy<IReadOnlyDictionary<string, Action<TextWriter, object>>>(_GetTemplates));

        public sealed override void Write(TextWriter textWriter, string templateName, object context)
            => _templates.Value[templateName](textWriter, context);

        protected virtual IEnumerable<IHandlebarsHelper> GetHelpers()
        {
            yield return new IsCollection();
            yield return new Concat();
            yield return new Semver();
            yield return new GetAssemblyCompany();
            yield return new GetDocumentationPartialName();
            yield return new MemberName();
            yield return new SimpleMemberName();
            yield return new MemberUrl(_pageContext);
            yield return new MemberLink(this);
            yield return new MemberAccessModifier();
            yield return new TypeReferenceLink(this);
            yield return new ArrayRank();
            yield return new Format();
            yield return new Pygments();
            yield return new IsReadOnlyProperty();
            yield return new IsReadWriteProperty();
            yield return new IsWriteOnlyProperty();
            yield return new MemberDeclarationsList(this);
        }

        protected virtual IEnumerable<IHandlebarsBlockHelper> GetBlockHelpers()
            => Enumerable.Empty<IHandlebarsBlockHelper>();

        protected virtual IReadOnlyDictionary<string, string> GetViews()
            => typeof(HandlebarsTemplateWriter)
                .Assembly
                .GetManifestResourceNames()
                .Select(resource => (Resource: resource, Match: Regex.Match(resource, @"Partials(\.\w+)*\.(?<templateName>\w+)\.hbs$", RegexOptions.IgnoreCase)))
                .Where(item => item.Match.Success)
                .ToDictionary(
                    item => item.Match.Groups["templateName"].Value,
                    item => _ReadFromEmbeddedResource(typeof(HandlebarsTemplateWriter).Assembly, item.Resource),
                    StringComparer.OrdinalIgnoreCase
                );

        protected virtual IReadOnlyDictionary<string, string> GetTemplates()
            => typeof(HandlebarsTemplateWriter)
                .Assembly
                .GetManifestResourceNames()
                .Select(resource => (Resource: resource, Match: Regex.Match(resource, @"Templates(\.\w+)*\.(?<templateName>\w+)\.hbs$", RegexOptions.IgnoreCase)))
                .Where(item => item.Match.Success)
                .ToDictionary(
                    item => item.Match.Groups["templateName"].Value,
                    item => _ReadFromEmbeddedResource(typeof(HandlebarsTemplateWriter).Assembly, item.Resource),
                    StringComparer.OrdinalIgnoreCase
                );

        private IReadOnlyDictionary<string, Action<TextWriter, object>> _GetTemplates()
        {

            var handlebars = HandlebarsDotNet.Handlebars.Create();
            foreach (var helper in GetHelpers() ?? Enumerable.Empty<IHandlebarsHelper>())
                handlebars.RegisterHelper(helper.Name, helper.Apply);

            foreach (var blockHelper in GetBlockHelpers() ?? Enumerable.Empty<IHandlebarsBlockHelper>())
                handlebars.RegisterHelper(blockHelper.Name, blockHelper.Apply);

            foreach (var (viewName, viewTemplate) in _GetLatestProviders(GetViews() ?? Enumerable.Empty<KeyValuePair<string, string>>()))
                using (var viewTextReader = new StringReader(viewTemplate))
                    handlebars.RegisterTemplate(viewName, handlebars.Compile(viewTextReader));

            return _GetLatestProviders(GetTemplates() ?? Enumerable.Empty<KeyValuePair<string, string>>())
                .ToDictionary(
                    pair => pair.Name,
                    pair =>
                    {
                        var templateTextReader = new StringReader(pair.Template);
                        return handlebars.Compile(templateTextReader);
                    },
                    StringComparer.OrdinalIgnoreCase
                );
        }

        private static string _ReadFromEmbeddedResource(Assembly assembly, string embeddedResourceName)
        {
            using var streamReader = new StreamReader(assembly.GetManifestResourceStream(embeddedResourceName));
            return streamReader.ReadToEnd();
        }

        private static IEnumerable<(string Name, string Template)> _GetLatestProviders(IEnumerable<KeyValuePair<string, string>> providers)
            => providers.GroupBy(pair => pair.Key, pair => pair.Value, (viewName, viewProviders) => (viewName, viewProviders.Last()), StringComparer.OrdinalIgnoreCase);
    }
}