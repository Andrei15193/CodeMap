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
        private readonly IReadOnlyDictionary<string, Action<TextWriter, object>> _templates;

        public HandlebarsTemplateWriter(IMemberReferenceResolver memberReferenceResolver)
            => _templates = _GetTemplates(memberReferenceResolver);

        public sealed override void Write(TextWriter textWriter, string templateName, object context)
            => _templates[templateName](textWriter, context);

        protected virtual IEnumerable<IHandlebarsHelper> GetHelpers(IMemberReferenceResolver memberReferenceResolver)
        {
            yield return new IsCollection();
            yield return new Concat();
            yield return new Format();
            yield return new Semver();
            yield return new Pygments();
            yield return new GetAssemblyCompany();
            yield return new MemberReference(memberReferenceResolver);
            yield return new GetDocumentationPartialName();
            yield return new MemberAccessModifier();
        }

        protected virtual IEnumerable<IHandlebarsBlockHelper> GetBlockHelpers(IMemberReferenceResolver memberReferenceResolver)
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

        private IReadOnlyDictionary<string, Action<TextWriter, object>> _GetTemplates(IMemberReferenceResolver memberReferenceResolver)
        {
            var handlebars = HandlebarsDotNet.Handlebars.Create();
            foreach (var helper in _GetLatest(GetHelpers(memberReferenceResolver) ?? Enumerable.Empty<IHandlebarsHelper>(), helper => helper.Name, helper => helper))
                handlebars.RegisterHelper(helper.Key, helper.Value.Apply);

            foreach (var blockHelper in _GetLatest(GetBlockHelpers(memberReferenceResolver) ?? Enumerable.Empty<IHandlebarsBlockHelper>(), blockHelper => blockHelper.Name, blockHelper => blockHelper))
                handlebars.RegisterHelper(blockHelper.Key, blockHelper.Value.Apply);

            foreach (var (viewName, viewTemplate) in _GetLatest(GetViews() ?? Enumerable.Empty<KeyValuePair<string, string>>(), pair => pair.Key, pair => pair.Value))
                using (var viewTextReader = new StringReader(viewTemplate))
                    handlebars.RegisterTemplate(viewName, handlebars.Compile(viewTextReader));

            return _GetLatest(GetTemplates() ?? Enumerable.Empty<KeyValuePair<string, string>>(), pair => pair.Key, pair => pair.Value)
                .ToDictionary(
                    pair => pair.Key,
                    pair =>
                    {
                        var templateTextReader = new StringReader(pair.Value);
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

        private static IEnumerable<(string Key, TValue Value)> _GetLatest<TItem, TValue>(IEnumerable<TItem> items, Func<TItem, string> keySelector, Func<TItem, TValue> valueSelector)
            => items.GroupBy(keySelector, valueSelector, (key, values) => (key, values.Last()), StringComparer.OrdinalIgnoreCase);
    }
}