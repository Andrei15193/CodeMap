﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using CodeMap.Handlebars.Helpers;
using EmbeddedResourceBrowser;
using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;

namespace CodeMap.Handlebars
{
    /// <summary>A <a href="https://github.com/Handlebars-Net/Handlebars.Net">Handlebars.NET</a> based template writer providing the default templates, partials and helpers.</summary>
    /// <remarks>
    /// <para>
    /// The Handlebars resources are loaded using the <see cref="EmbeddedDirectory.Merge(IEnumerable{Assembly})"/> method from the provided assemblies, the
    /// default assemblies from where resources are <c>CodeMap.Handlebars</c> and the entry assembly (<see cref="Assembly.GetEntryAssembly"/>). This should
    /// suffice for most cases as the intended way of using this library is to define a .NET Core Console application that generates the documenation and
    /// add new or modified handlebars templates and assets as embedded resources in the console application assembly.
    /// </para>
    /// <para>
    /// Embedded resources are loaded from the themes directories where each theme is formed out of 3 base directories: <c>Assets</c>, made available through
    /// the <see cref="Assets"/> property, <c>Partials</c> and <c>Templates</c>. Assets are provided as an <see cref="EmbeddedDirectory"/> in order to allow
    /// copying them in various ways and different directories. On the other hand, the handlebars templates are loaded automatically from their respective
    /// directories after their structures have been flattened.
    /// </para>
    /// <para>
    /// Overriding templates or assets is rather easy, simply define an embedded resource respecting the directory structure.
    /// </para>
    /// <code>
    /// MyDocumentationGeneratorApp/
    ///     Themes/
    ///         Bootstrap/
    ///             4.5.0/
    ///                 This is a version specific override. Only files for this version will be overridden, all other versions are not affected.
    ///                 Assets/
    ///                     favicon.ico (adds a favicon asset that will be copied over along side all other default resources)
    ///                 Partials/
    ///                     Layout.hbs (overrides the layout partial, this can be a copy of the default layout to which one can add extra tags, such as one for the favicon)
    ///                 Templates/
    ///                     Class.hbs (this will override the default handlebars template for class declarations)
    ///                     Index.hbs (this will add a new template that can be used besides the declaration node templates, useful for generating home pages)
    ///
    ///             This is a theme specific override. Only files for this theme will be overridden, all versions are affected, but all other themes are not affected.
    ///             Assets/
    ///                 favicon.ico (adds a favicon asset that will be copied over along side all other default resources)
    ///             Partials/
    ///                 Layout.hbs (overrides the layout partial, this can be a copy of the default layout to which one can add extra tags, such as one for the favicon)
    ///             Templates/
    ///                 Class.hbs (this will override the default handlebars template for class declarations)
    ///                 Index.hbs (this will add a new template that can be used besides the declaration node templates, useful for generating home pages)
    ///
    ///        This is a global override. All themes are affected by these changes.
    ///        Assets/
    ///            favicon.ico (adds a favicon asset that will be copied over along side all other default resources)
    ///        Partials/
    ///            Layout.hbs (overrides the layout partial, this can be a copy of the default layout to which one can add extra tags, such as one for the favicon)
    ///        Templates/
    ///            Class.hbs (this will override the default handlebars template for class declarations)
    ///            Index.hbs (this will add a new template that can be used besides the declaration node templates, useful for generating home pages)
    /// </code>
    /// <para>
    /// In case there are multiple overrides then they are picked from specific to global meaning that if there is a Bootstrap override and a global override
    /// for the same template file, when generating documentation using the Bootstrap theme then the specific override is used, for all other themes the
    /// global one is used. This applies to version specific vs theme overrides, the version speific will be picked when using that version, otherwise the
    /// theme one will be picked.
    /// </para>
    /// <para>
    /// To view all default templates simply browse them on the repository page, generally when making changes to them it is a good option to start by
    /// copying the default one and modify it. At the same time one can create a new layout from the ground up using different theming libraries.
    /// </para>
    /// <para>
    /// Generally, the library aims to provide as much out of the box that is possible. Generating project documentation should be really easy and quick
    /// to configure.
    /// </para>
    /// </remarks>
    public class HandlebarsTemplateWriter
    {
        private readonly Regex _themeSpecificationRegex = new Regex(@"^(?<name>[^@]*)(?<version>@((?<major>\d+)(\.(?<minor>\d+)(\.(?<patch>\d+))?)?)|latest)?$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
        private readonly Regex _versionDirectoryNameRegex = new Regex(@"^_\d+$", RegexOptions.IgnoreCase);
        private readonly IMemberReferenceResolver _memberReferenceResolver;
        private readonly EmbeddedDirectory _baseThemeDirectory;
        private readonly EmbeddedDirectory _versionedThemeDirectory;
        private readonly Lazy<IReadOnlyDictionary<string, HandlebarsTemplate<TextWriter, object, object>>> _templates;

        /// <summary>Initializes a new instance of the <see cref="HandlebarsTemplateWriter"/> class.</summary>
        /// <param name="theme">The theme name to apply. This can be accompanied by the version of the theme (e.g.: <c>Bootstrap@4</c>, <c>Bootstrap@4.5</c>, or <c>Bootstrap@4.5.0</c>). To use the latest version either specify <c>@latest</c> or no version at all.</param>
        /// <param name="memberReferenceResolver">The <see cref="IMemberReferenceResolver"/> used by the <see cref="MemberReference"/> helper.</param>
        /// <param name="assemblies">The <see cref="Assembly"/> objects from where to load assets and Handlebars templates and partials.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="theme" /> is <c>null</c>, empty or white space or when a theme with the provided name (and version) could not be found.
        /// </exception>
        public HandlebarsTemplateWriter(string theme, IMemberReferenceResolver memberReferenceResolver, IEnumerable<Assembly> assemblies)
        {
            if (string.IsNullOrWhiteSpace(theme))
                throw new ArgumentException("Cannot be null, empty or white space.", nameof(theme));

            var resources = EmbeddedDirectory.Merge(assemblies);
            if (!resources.Subdirectories.TryGetValue("Themes", out var themesDirectory))
                throw new ArgumentException("Themes directory does not contain any embedded resources.");

            var match = _themeSpecificationRegex.Match(theme);
            var themeName = match.Groups["name"].Value;
            var themeVersion = match.Groups["version"].Value;
            if (!themesDirectory.Subdirectories.TryGetValue(themeName, out _baseThemeDirectory))
                throw new ArgumentException($"'{themeName}' directory does not contain any embedded resources.");

            _versionedThemeDirectory = (
                from majorVersionSubdirectory in _baseThemeDirectory.Subdirectories
                where string.IsNullOrWhiteSpace(match.Groups["major"].Value) || Regex.IsMatch(majorVersionSubdirectory.Name, $"^_{match.Groups["major"].Value}$")
                from minorVersionSubdirectory in majorVersionSubdirectory.Subdirectories
                where string.IsNullOrWhiteSpace(match.Groups["minor"].Value) || Regex.IsMatch(minorVersionSubdirectory.Name, $"^_{match.Groups["minor"].Value}$")
                from patchVersionSubdirectory in minorVersionSubdirectory.Subdirectories
                where string.IsNullOrWhiteSpace(match.Groups["patch"].Value) || Regex.IsMatch(patchVersionSubdirectory.Name, $"^_{match.Groups["patch"].Value}$")
                orderby new Version(int.Parse(majorVersionSubdirectory.Name.Substring(1), NumberStyles.None, CultureInfo.InvariantCulture), int.Parse(minorVersionSubdirectory.Name.Substring(1), NumberStyles.None, CultureInfo.InvariantCulture), int.Parse(patchVersionSubdirectory.Name.Substring(1), NumberStyles.None, CultureInfo.InvariantCulture)) descending
                select patchVersionSubdirectory
            ).FirstOrDefault();

            if (_versionedThemeDirectory is null)
                throw new ArgumentException("'{themeVersion}' theme vesrion directory does not contain any embedded resources.");

            _memberReferenceResolver = memberReferenceResolver;
            _templates = new Lazy<IReadOnlyDictionary<string, HandlebarsTemplate<TextWriter, object, object>>>(_GetTemplates);
        }

        /// <summary>Initializes a new instance of the <see cref="HandlebarsTemplateWriter"/> class.</summary>
        /// <param name="theme">The theme name to apply. This can be accompanied by the version of the theme (e.g.: <c>Bootstrap@4</c>, <c>Bootstrap@4.5</c>, or <c>Bootstrap@4.5.0</c>). To use the latest version either specify <c>@latest</c> or no version at all.</param>
        /// <param name="memberReferenceResolver">The <see cref="IMemberReferenceResolver"/> used by the <see cref="MemberReference"/> helper.</param>
        /// <param name="assemblies">The <see cref="Assembly"/> objects from where to load assets and Handlebars templates and partials.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="theme" /> is <c>null</c>, empty or white space or when a theme with the provided name (and version) could not be found.
        /// </exception>
        public HandlebarsTemplateWriter(string theme, IMemberReferenceResolver memberReferenceResolver, params Assembly[] assemblies)
            : this(theme, memberReferenceResolver, (IEnumerable<Assembly>)assemblies)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="HandlebarsTemplateWriter"/> class.</summary>
        /// <param name="theme">The theme name to apply. This can be accompanied by the version of the theme (e.g.: <c>Bootstrap@4</c>, <c>Bootstrap@4.5</c>, or <c>Bootstrap@4.5.0</c>). To use the latest version either specify <c>@latest</c> or no version at all.</param>
        /// <param name="memberReferenceResolver">The <see cref="IMemberReferenceResolver"/> used by the <see cref="MemberReference"/> helper.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="theme" /> is <c>null</c>, empty or white space or when a theme with the provided name (and version) could not be found.
        /// </exception>
        public HandlebarsTemplateWriter(string theme, IMemberReferenceResolver memberReferenceResolver)
            : this(theme, memberReferenceResolver, typeof(HandlebarsTemplateWriter).Assembly, Assembly.GetEntryAssembly())
        {
        }

        /// <summary>Gets the <see cref="EmbeddedFile"/>s containing theme assets.</summary>
        public IEnumerable<EmbeddedFile> Assets
            => _GetEmbeddedFilesFromResourceDirectory("assets");

        /// <summary>Gets the <see cref="EmbeddedFile"/>s containing theme extras.</summary>
        public IEnumerable<EmbeddedFile> Extras
            => _GetEmbeddedFilesFromResourceDirectory("extras");

        /// <summary>Writes the provided <paramref name="templateName"/> with in the given <paramref name="context"/> to the provided <paramref name="textWriter"/>.</summary>
        /// <param name="textWriter">The <see cref="TextWriter"/> to which to write the template.</param>
        /// <param name="templateName">The name of the template to apply.</param>
        /// <param name="context">The context in which to apply the template.</param>
        public void Write(TextWriter textWriter, string templateName, object context)
            => _templates.Value[templateName](textWriter, context);

        /// <summary>Applies the template with the given <paramref name="templateName"/> in the given <paramref name="context"/>.</summary>
        /// <param name="templateName">The template to apply.</param>
        /// <param name="context">The context in which to apply the template.</param>
        /// <returns>Returns the applied template.</returns>
        public string Apply(string templateName, object context)
        {
            var result = new StringBuilder();
            using (var stringWriter = new StringWriter(result))
                Write(stringWriter, templateName, context);
            return result.ToString();
        }

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
        protected virtual IEnumerable<IHelperDescriptor<HelperOptions>> GetHelpers(IMemberReferenceResolver memberReferenceResolver)
        {
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
        protected virtual IEnumerable<IHelperDescriptor<BlockHelperOptions>> GetBlockHelpers(IMemberReferenceResolver memberReferenceResolver)
            => Enumerable.Empty<IHelperDescriptor<BlockHelperOptions>>();

        private IEnumerable<EmbeddedDirectory> _EmbeddedResourceDirectories
        {
            get
            {
                var embeddedDirectory = _versionedThemeDirectory;
                do
                {
                    yield return embeddedDirectory;
                    embeddedDirectory = embeddedDirectory.ParentDirectory;
                } while (!embeddedDirectory.Name.Equals("Themes", StringComparison.OrdinalIgnoreCase));
                yield return embeddedDirectory;
            }
        }

        private IEnumerable<EmbeddedFile> _GetEmbeddedFilesFromResourceDirectory(string directoryName)
            => _EmbeddedResourceDirectories
                .SelectMany(embeddedResourceDirectory => embeddedResourceDirectory.Subdirectories.TryGetValue(directoryName, out var embeddedDirectory) ? embeddedDirectory.GetAllFiles() : Enumerable.Empty<EmbeddedFile>())
                .GroupBy(file => file.Name, (name, files) => files.First(), StringComparer.OrdinalIgnoreCase);

        private IReadOnlyDictionary<string, HandlebarsTemplate<TextWriter, object, object>> _GetTemplates()
        {
            var handlerbars = HandlebarsDotNet.Handlebars.Create();

            var helpers = (GetHelpers(_memberReferenceResolver) ?? Enumerable.Empty<IHelperDescriptor>())
                .Concat(GetBlockHelpers(_memberReferenceResolver) ?? Enumerable.Empty<IHelperDescriptor>())
                .GroupBy(helper => helper.Name.ToString(), (helperName, matchedHelpers) => matchedHelpers.Last(), StringComparer.OrdinalIgnoreCase);

            foreach (var helper in helpers)
                switch (helper)
                {
                    case IHelperDescriptor<HelperOptions> inlineHelper:
                        handlerbars.RegisterHelper(inlineHelper);
                        break;

                    case IHelperDescriptor<BlockHelperOptions> blockHelper:
                        handlerbars.RegisterHelper(blockHelper);
                        break;
                }

            foreach (var partial in _GetEmbeddedFilesFromResourceDirectory("partials").Where(template => template.Extension.Equals(".hbs", StringComparison.OrdinalIgnoreCase)))
                using (var partialStream = partial.OpenRead())
                using (var partialStreamReader = new StreamReader(partialStream))
                    handlerbars.RegisterTemplate(_GetTemplateName(partial), partialStreamReader.ReadToEnd());

            return _GetEmbeddedFilesFromResourceDirectory("templates")
                .Where(template => template.Extension.Equals(".hbs", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(
                    _GetTemplateName,
                    template =>
                    {
                        using var templateStream = template.OpenRead();
                        using var templateStreamReader = new StreamReader(templateStream);
                        return handlerbars.Compile(templateStreamReader);
                    },
                    StringComparer.OrdinalIgnoreCase
                );
        }

        private static string _GetTemplateName(EmbeddedFile embeddedFile)
            => embeddedFile.Name.Substring(0, embeddedFile.Name.Length - embeddedFile.Extension.Length);
    }
}