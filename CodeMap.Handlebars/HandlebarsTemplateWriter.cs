using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CodeMap.Handlebars.Helpers;

namespace CodeMap.Handlebars
{
    /// <summary>A Handlebars.NET <see cref="TemplateWriter"/> providing the default templates, partials and helpers.</summary>
    public class HandlebarsTemplateWriter : TemplateWriter
    {
        private readonly IReadOnlyDictionary<string, Action<TextWriter, object>> _templates;

        /// <summary>Initializes a new instance of the <see cref="HandlebarsTemplateWriter"/> class.</summary>
        /// <param name="memberReferenceResolver">The <see cref="IMemberReferenceResolver"/> used by the <see cref="MemberReference"/> helper.</param>
        public HandlebarsTemplateWriter(IMemberReferenceResolver memberReferenceResolver)
            => _templates = _GetTemplates(memberReferenceResolver);

        /// <summary>Writes the provided <paramref name="templateName"/> with in the given <paramref name="context"/> to the provided <paramref name="textWriter"/>.</summary>
        /// <param name="textWriter">The <see cref="TextWriter"/> to which to write the template.</param>
        /// <param name="templateName">The name of the template to apply.</param>
        /// <param name="context">The context in which to apply the template.</param>
        public sealed override void Write(TextWriter textWriter, string templateName, object context)
            => _templates[templateName](textWriter, context);

        /// <summary>Gets a collection of inline helpers that are available in each template.</summary>
        /// <param name="memberReferenceResolver">The <see cref="IMemberReferenceResolver"/> used to resolve <see cref="ReferenceData.MemberReference"/>s.</param>
        /// <returns>Returns a collection of inline helpers to use in templates.</returns>
        /// <remarks>
        /// If there are multiple helpers that have the same name, only the latest will be registered in the Handlebars engine.
        /// This enables easy overriding of helpers as such:
        /// <code language="c#">
        /// public class MyCustomHandlebarsTemplateWriter : HandlebarsTemplateWriter
        /// {
        ///     public MyCustomHandlebarsTemplateWriter(IMemberReferenceResolver memberReferenceResolver)
        ///         : base(memberReferenceResolver)
        ///     {
        ///     }
        ///
        ///     protected override IEnumerable&lt;IHandlebarsHelper&gt; GetHelpers(IMemberReferenceResolver memberReferenceResolver)
        ///     {
        ///         foreach (var helper in base.GetHelpers(memberReferenceResolver))
        ///             yield return helper;
        ///
        ///         yield return new MyCustomSemver();
        ///     }
        /// }
        /// </code>
        /// If <c>MyCustomSemver</c> has the same helper name as <see cref="Semver"/> then <c>MyCustomSemver</c> will override the <see cref="Semver"/> helper.
        /// </remarks>
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

        /// <summary>Gets a collection of block helpers that are available in each template.</summary>
        /// <param name="memberReferenceResolver">The <see cref="IMemberReferenceResolver"/> used to resolve <see cref="ReferenceData.MemberReference"/>s.</param>
        /// <returns>Returns a collection of block helpers to use in templates.</returns>
        /// <remarks>If there are multiple helpers that have the same name, only the latest will be registered in the Handlebars engine.</remarks>
        protected virtual IEnumerable<IHandlebarsBlockHelper> GetBlockHelpers(IMemberReferenceResolver memberReferenceResolver)
            => Enumerable.Empty<IHandlebarsBlockHelper>();

        /// <summary>Gets the Handlebars partials.</summary>
        /// <returns>Returns a <see cref="IReadOnlyDictionary{TKey, TValue}"/> containing the Handlebars partials.</returns>
        /// <remarks>
        /// Overring partials or providing new ones can be done by using collection initializers as such:
        /// <code language="c#">
        /// public class MyCustomHandlebarsTemplateWriter : HandlebarsTemplateWriter
        /// {
        ///     public MyCustomHandlebarsTemplateWriter(IMemberReferenceResolver memberReferenceResolver)
        ///         : base(memberReferenceResolver)
        ///     {
        ///     }
        /// 
        ///     protected override IReadOnlyDictionary&lt;string, string&gt; GetPartials()
        ///         => new Dictionary&lt;string, string&gt;(base.GetPartials(), StringComparer.OrdinalIgnoreCase)
        ///         {
        ///             ["Class"] = ReadFromEmbeddedResource(typeof(MyCustomHandlebarsTemplateWriter).Assembly, "MyCustomHandlebarsTemplateWriter.CustomLayoutPartial.hbs")
        ///         };
        /// }
        /// </code>
        /// </remarks>
        protected virtual IReadOnlyDictionary<string, string> GetPartials()
            => typeof(HandlebarsTemplateWriter)
                .Assembly
                .GetManifestResourceNames()
                .Select(resource => (Resource: resource, Match: Regex.Match(resource, @"Partials(\.\w+)*\.(?<templateName>\w+)\.hbs$", RegexOptions.IgnoreCase)))
                .Where(item => item.Match.Success)
                .ToDictionary(
                    item => item.Match.Groups["templateName"].Value,
                    item => ReadFromEmbeddedResource(typeof(HandlebarsTemplateWriter).Assembly, item.Resource),
                    StringComparer.OrdinalIgnoreCase
                );

        /// <summary>Gets the Handlebars templates.</summary>
        /// <returns>Returns a <see cref="IReadOnlyDictionary{TKey, TValue}"/> containing the Handlebars templates.</returns>
        /// <remarks>
        /// Overring templates or providing new ones can be done by using collection initializers as such:
        /// <code language="c#">
        /// public class MyCustomHandlebarsTemplateWriter : HandlebarsTemplateWriter
        /// {
        ///     public MyCustomHandlebarsTemplateWriter(IMemberReferenceResolver memberReferenceResolver)
        ///         : base(memberReferenceResolver)
        ///     {
        ///     }
        /// 
        ///     protected override IReadOnlyDictionary&lt;string, string&gt; GetTemplates()
        ///         => new Dictionary&lt;string, string&gt;(base.GetTemplates(), StringComparer.OrdinalIgnoreCase)
        ///         {
        ///             ["Class"] = ReadFromEmbeddedResource(typeof(MyCustomHandlebarsTemplateWriter).Assembly, "MyCustomHandlebarsTemplateWriter.CustomClassTemplate.hbs")
        ///         };
        /// }
        /// </code>
        /// </remarks>
        protected virtual IReadOnlyDictionary<string, string> GetTemplates()
            => typeof(HandlebarsTemplateWriter)
                .Assembly
                .GetManifestResourceNames()
                .Select(resource => (Resource: resource, Match: Regex.Match(resource, @"Templates(\.\w+)*\.(?<templateName>\w+)\.hbs$", RegexOptions.IgnoreCase)))
                .Where(item => item.Match.Success)
                .ToDictionary(
                    item => item.Match.Groups["templateName"].Value,
                    item => ReadFromEmbeddedResource(typeof(HandlebarsTemplateWriter).Assembly, item.Resource),
                    StringComparer.OrdinalIgnoreCase
                );

        private IReadOnlyDictionary<string, Action<TextWriter, object>> _GetTemplates(IMemberReferenceResolver memberReferenceResolver)
        {
            var handlebars = HandlebarsDotNet.Handlebars.Create();
            foreach (var helper in _GetLatest(GetHelpers(memberReferenceResolver) ?? Enumerable.Empty<IHandlebarsHelper>(), helper => helper.Name, helper => helper))
                handlebars.RegisterHelper(helper.Key, helper.Value.Apply);

            foreach (var blockHelper in _GetLatest(GetBlockHelpers(memberReferenceResolver) ?? Enumerable.Empty<IHandlebarsBlockHelper>(), blockHelper => blockHelper.Name, blockHelper => blockHelper))
                handlebars.RegisterHelper(blockHelper.Key, blockHelper.Value.Apply);

            foreach (var (viewName, viewTemplate) in _GetLatest(GetPartials() ?? Enumerable.Empty<KeyValuePair<string, string>>(), pair => pair.Key, pair => pair.Value))
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

        /// <summary>Reads an embedded resource as a string, useful for loading handlebar templates from embedded resources.</summary>
        /// <param name="assembly">The <see cref="Assembly"/> from which to read the resource.</param>
        /// <param name="embeddedResourceName">The resource name to read.</param>
        /// <returns>Returns the textual content of the embedded resource.</returns>
        protected static string ReadFromEmbeddedResource(Assembly assembly, string embeddedResourceName)
        {
            using var streamReader = new StreamReader(assembly.GetManifestResourceStream(embeddedResourceName));
            return streamReader.ReadToEnd();
        }

        private static IEnumerable<(string Key, TValue Value)> _GetLatest<TItem, TValue>(IEnumerable<TItem> items, Func<TItem, string> keySelector, Func<TItem, TValue> valueSelector)
            => items.GroupBy(keySelector, valueSelector, (key, values) => (key, values.Last()), StringComparer.OrdinalIgnoreCase);
    }
}