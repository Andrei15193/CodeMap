using System;
using System.Collections.Generic;
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
    /// default assemblies are <c>CodeMap.Handlebars</c> and the entry assembly (<see cref="Assembly.GetEntryAssembly"/>). This should suffice for most cases
    /// as the intended way of using this library is to define a .NET Core Console application that generates the documenation and add new or modified
    /// handlebars templates and assets as embedded resources in the console application assembly.
    /// </para>
    /// <para>
    /// Embedded resources are loaded from the themes directories where each theme is formed out of 3 base directories: <c>Static</c>, made available through
    /// the <see cref="StaticFilesDirectories"/> property, <c>Partials</c> and <c>Templates</c>. Assets are provided as an <see cref="EmbeddedDirectory"/> in
    /// order to allow copying them in various ways and different directories. On the other hand, the handlebars templates are loaded automatically from their
    /// respective directories after their structures have been flattened.
    /// </para>
    /// <para>
    /// Themes are grouped into categories, each category provides a number of features and is aimed to be used in a certain way. For instance, the GitHub
    /// Pages category is aimed to generate documentation websites that are to be deployed using <a href="https://pages.github.com/">GitHub Pages</a>
    /// leveraging <a href="https://jekyllrb.com/">Jekyll</a> features. On the other hand, the Simple category contains themes that generate documentation
    /// pages for a single version, these themes are aimed to get started with the library and get quick results while the GitHub Pages one offers more
    /// features, but it takes longer to get started.
    /// </para>
    /// <para>
    /// Each category contains a number of themes where each is versioned. This is to offer legacy support for older documentation sites, or for sites
    /// that at some point will become legacy. All files in specific versions have the version either in their name or in the relative path for the theme.
    /// For instance, the layout for the Bootstrap theme version 4.5.0 is named <c>Bootstrap@4.5.0.html</c>, while the includes for this theme are under
    /// the <c>Bootstrap/4.5.0/</c> directory structure. This allows for multiple themes and versions of the same theme to be used at the same time.
    /// This is generally applicable when a library hosts multiple versions of its documentation, for legacy reasons as libraries usually have multiple
    /// versions that are supported at the same time, at the very least when a new version is released.
    /// </para>
    /// <para>
    /// Themes are embedded resources in the .NET assembly in order to simplify their usage. If the assembly is loaded, then all themes are available,
    /// no files are missing. The structure is simple, the root embedded directory for themes is <c>Themes</c>, then there is the theme category, then
    /// the theme itself. At all levels there are three directory names that are reserved, <c>Templates</c>, <c>Partials</c> and <c>Static</c>. The
    /// embedded resources are inherited, meaning that a file under the <c>Partials</c> directory which itself is directly under the <c>Themes</c>
    /// directory is available across all themes. Similarly there can be theme category specific files, this works well with the GitHub Pages category
    /// as includes that are useful across all themes aimed to be hosted on <a href="https://pages.github.com/">GitHub Pages</a> can be defined in one place.
    /// </para>
    /// <para>
    /// Customizing themes is done rather easy. The embedded resources are merged using the <see cref="EmbeddedDirectory.Merge(IEnumerable{Assembly})"/>
    /// method and from that result the theme is loaded when creating an instance of this class. This is one of the reasons why only the
    /// <c>CodeMap.Handlebars</c> assembly and the calling assembly are used by default, to customize a theme one only needs to follow the same directory
    /// structure and provide the replacements or additions.
    /// </para>
    /// <code>
    /// MyDocumentationGeneratorApp/
    ///     Themes/
    ///     |-- GitHub Pages/
    ///     |   |-- Bootstrap@4.5.0/
    ///     |   |   |   This is a theme override/addition. Only files for this theme will be overridden.
    ///     |   |   |-- Static/
    ///     |   |   |   |-- favicon.ico (adds a favicon asset that will be copied over along side all other default resources)
    ///     |   |   |-- Partials/
    ///     |   |   |   |-- Breadcrumbs.hbs (overrides the breadcrumbs partial, this can be a copy of the default breadcrumbs to which one can add extra tags)
    ///     |   |   |-- Templates/
    ///     |   |   |   |-- Class.hbs (this will override the default handlebars template for class declarations)
    ///     |   |   |   |-- Index.hbs (this will add a new template that can be used besides the declaration node templates, useful for generating home pages)
    ///     |   |
    ///     |   |-- This is a theme category override/addition. All themes in this category are affected by these changes.
    ///     |   |-- Static/
    ///     |   |   |-- favicon.ico (adds a favicon asset that will be copied over along side all other default resources)
    ///     |   |-- Partials/
    ///     |   |   |-- Breadcrumbs.hbs (overrides the breadcrumbs partial, this can be a copy of the default breadcrumbs to which one can add extra tags)
    ///     |   |-- Templates/
    ///     |   |   |-- Class.hbs (this will override the default handlebars template for class declarations)
    ///     |   |   |-- Index.hbs (this will add a new template that can be used besides the declaration node templates, useful for generating home pages)
    ///     |
    ///     |   This is a global override/addition. All themes are affected by these changes.
    ///     |-- Static/
    ///     |   |-- favicon.ico (adds a favicon asset that will be copied over along side all other default resources)
    ///     |-- Partials/
    ///     |   |-- Breadcrumbs.hbs (overrides the breadcrumbs partial, this can be a copy of the default breadcrumbs to which one can add extra tags)
    ///     |-- Templates/
    ///     |   |-- Class.hbs (this will override the default handlebars template for class declarations)
    ///     |   |-- Index.hbs (this will add a new template that can be used besides the declaration node templates, useful for generating home pages)
    /// </code>
    /// <para>
    /// In case there are multiple overrides then they are picked from specific to global meaning that if there is a Bootstrap override and a global override
    /// for the same template file, when generating documentation using the Bootstrap theme then the specific override is used, for all other themes the
    /// global one is used.
    /// </para>
    /// <para>
    /// To view all default templates simply browse them on the repository page, generally when making changes to them it is a good option to start by
    /// copying the default one and modifying it. At the same time one can create a new layout from the ground up using different theming libraries.
    /// </para>
    /// <para>
    /// The library aims to provide as much out of the box that as possible. Generating project documentation should be really be easy to configure.
    /// </para>
    /// </remarks>
    public class HandlebarsTemplateWriter
    {
        /// <summary>Gets the names of reserved directory names.</summary>
        /// <value>The collection contains the following values: <c>Partials</c>, <c>Templates</c>, and <c>Static</c>></value>
        public static IReadOnlyList<string> ReservedDirectoryNames { get; } = new[] { "Static", "Partials", "Templates" };

        private readonly Regex _themeSpecificationRegex = new Regex(@"^(?<category>[^/]+)/(?<name>[^@]+)@(?<version>\d+\.\d+\.\d+)$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
        private readonly IMemberReferenceResolver _memberReferenceResolver;
        private readonly EmbeddedDirectory _themesDirectory;
        private readonly EmbeddedDirectory _themeCategoryDirectory;
        private readonly EmbeddedDirectory _themeDirectory;
        private readonly Lazy<IReadOnlyDictionary<string, HandlebarsTemplate<TextWriter, object, object>>> _templates;

        /// <summary>Initializes a new instance of the <see cref="HandlebarsTemplateWriter"/> class.</summary>
        /// <param name="theme">The theme name to apply using the following specification <c>theme-category/theme-name@theme version</c> (e.g.: <c>Simple/Bootstrap@4.5.0</c>).</param>
        /// <param name="memberReferenceResolver">The <see cref="IMemberReferenceResolver"/> used by the <see cref="MemberReference"/> helper.</param>
        /// <param name="assemblies">The <see cref="Assembly"/> objects from where to load assets and Handlebars templates and partials.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="theme" /> is <c>null</c>, empty or white space or when a theme could not be found.
        /// </exception>
        public HandlebarsTemplateWriter(string theme, IMemberReferenceResolver memberReferenceResolver, IEnumerable<Assembly> assemblies)
        {
            if (string.IsNullOrWhiteSpace(theme))
                throw new ArgumentException("Cannot be null, empty or white space.", nameof(theme));

            var resources = EmbeddedDirectory.Merge(assemblies);
            if (!resources.Subdirectories.TryGetValue("Themes", out var themesDirectory))
                throw new ArgumentException("Themes directory does not contain any embedded resources.");

            _themesDirectory = themesDirectory;
            var match = _themeSpecificationRegex.Match(theme);
            if (!match.Success)
                throw new ArgumentException("The theme name is not in the expected format (theme-category/theme-name@theme-version).", nameof(theme));

            var themeCategory = match.Groups["category"].Value;
            var themeName = match.Groups["name"].Value;
            var themeVersion = match.Groups["version"].Value;

            if (ReservedDirectoryNames.Contains(themeCategory, StringComparer.OrdinalIgnoreCase))
                throw new ArgumentNullException($"'{themeCategory}' theme category name is reserved.", nameof(theme));

            if (ReservedDirectoryNames.Contains(themeName, StringComparer.OrdinalIgnoreCase))
                throw new ArgumentNullException($"'{themeName}' theme name is reserved.", nameof(theme));

            if (!_themesDirectory.Subdirectories.TryGetValue(themeCategory, out _themeCategoryDirectory))
                throw new ArgumentException($"'{themeCategory}' theme category was not found, directory does not contain any embedded resources.", nameof(theme));

            if (!_themeCategoryDirectory.Subdirectories.TryGetValue($"{themeName}@{themeVersion}", out _themeDirectory))
                if (_themeCategoryDirectory.Subdirectories.All(subdirectory => !subdirectory.Name.StartsWith(themeName, StringComparison.OrdinalIgnoreCase)))
                    throw new ArgumentException($"'{themeName}' theme was not found, directory does not contain any embedded resources.", nameof(theme));
                else
                    throw new ArgumentException($"'{themeVersion}' theme version was not found, directory does not contain any embedded resources.", nameof(theme));

            _memberReferenceResolver = memberReferenceResolver;
            _templates = new Lazy<IReadOnlyDictionary<string, HandlebarsTemplate<TextWriter, object, object>>>(_GetTemplates);
        }

        /// <summary>Initializes a new instance of the <see cref="HandlebarsTemplateWriter"/> class.</summary>
        /// <param name="theme">The theme name to apply using the following specification <c>theme-category/theme-name@theme version</c> (e.g.: <c>Simple/Bootstrap@4.5.0</c>).</param>
        /// <param name="memberReferenceResolver">The <see cref="IMemberReferenceResolver"/> used by the <see cref="MemberReference"/> helper.</param>
        /// <param name="assemblies">The <see cref="Assembly"/> objects from where to load assets and Handlebars templates and partials.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="theme" /> is <c>null</c>, empty or white space or when a theme could not be found.
        /// </exception>
        public HandlebarsTemplateWriter(string theme, IMemberReferenceResolver memberReferenceResolver, params Assembly[] assemblies)
            : this(theme, memberReferenceResolver, (IEnumerable<Assembly>)assemblies)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="HandlebarsTemplateWriter"/> class.</summary>
        /// <param name="theme">The theme name to apply using the following specification <c>theme-category/theme-name@theme version</c> (e.g.: <c>Simple/Bootstrap@4.5.0</c>).</param>
        /// <param name="memberReferenceResolver">The <see cref="IMemberReferenceResolver"/> used by the <see cref="MemberReference"/> helper.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="theme" /> is <c>null</c>, empty or white space or when a theme could not be found.
        /// </exception>
        public HandlebarsTemplateWriter(string theme, IMemberReferenceResolver memberReferenceResolver)
            : this(theme, memberReferenceResolver, typeof(HandlebarsTemplateWriter).Assembly, Assembly.GetEntryAssembly())
        {
        }

        /// <summary>Gets the <see cref="EmbeddedDirectory"/> instances for static files from the version specific to the global ones.</summary>
        /// <value>
        /// Static files can be specified at any level for a theme. They can be specific only to a version (e.g.: the stylesheet would be a
        /// version specific static file), specific to a theme (e.g.: if a theme has the same color theme throughout the versions then the
        /// Pygments stylesheet can be placed at the theme level since it is the same for all), specific to a theme category (e.g.: includes
        /// that are used by all GitHub Pages themes can be placed here), or at the global level (e.g.: helpers can be placed here, or theme
        /// invariant files such as the license file).
        /// </value>
        /// <remarks>
        /// Copying files from these directories can be done using the <see cref="FileSystemInfoExtensions.CopyTo(EmbeddedDirectory, DirectoryInfo)"/>
        /// method on the reverse order of the provided static <see cref="EmbeddedDirectory"/> items. The most specific ones should replace the most
        /// general ones in case of files having the same name.
        /// <code lang="c#">
        /// foreach (var staticFilesDirectory in templateWriter.StaticFilesDirectories.Reverse())
        ///     // CopyTo is an extension method
        ///     staticFilesDirectory.CopyTo(new DirectoryInfo(outputDirectoryPath));
        /// </code>
        /// Alternatively, a <see cref="HashSet{T}"/> can be used to keep track of which files have already been copied and simply skip them
        /// when they are found in a more general <see cref="EmbeddedDirectory"/>. This will optimize disk operations as only the necessary
        /// files will be copied.
        /// <code lang="c#">
        /// var copiedFiles = new HashSet&lt;string&lt;(StringComparer.OrdinalIgnoreCase);
        /// foreach (var staticFilesDirectory in templateWriter.StaticFilesDirectories)
        ///     foreach (var staticFile in staticFilesDirectory.GetAllFiles())
        ///     {
        ///         var currentDirectory = staticFile.ParentDirectory;
        ///         var filePathBuilder = new StringBuilder(staticFile.Name);
        ///         while (currentDirectory != staticFilesDirectory)
        ///         {
        ///             filePathBuilder
        ///                 .Insert(0, Path.DirectorySeparatorChar)
        ///                 .Insert(0, currentDirectory.Name);
        ///             currentDirectory = currentDirectory.ParentDirectory;
        ///         }
        ///         filePathBuilder
        ///             .Insert(0, Path.DirectorySeparatorChar)
        ///             .Insert(0, outputDirectoryPath);
        ///         var outputFileInfo = new FileInfo(filePathBuilder.ToString());
        ///         if (copiedFiles.Add(outputFileInfo.FullName))
        ///             using (var staticFileStream = staticFile.OpenRead())
        ///             using (var outputFileStream = outputFileInfo.Open(FileMode.Create, FileAccess.Write, FileShare.Read))
        ///                 staticFileStream.CopyTo(outputFileStream);
        ///     }
        /// </code>
        /// </remarks>
        public IEnumerable<EmbeddedDirectory> StaticFilesDirectories
        {
            get
            {
                foreach (var embeddedResourceDirectory in _EmbeddedResourceDirectories)
                    if (embeddedResourceDirectory.Subdirectories.TryGetValue("Static", out var resourceDirectory))
                        yield return resourceDirectory;
            }
        }

        /// <summary>Writes the provided <paramref name="templateName"/> with in the given <paramref name="context"/> to the provided <paramref name="textWriter"/>.</summary>
        /// <param name="textWriter">The <see cref="TextWriter"/> to which to write the template.</param>
        /// <param name="templateName">The name of the template to apply.</param>
        /// <param name="context">The context in which to apply the template.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="templateName"/> is not defined.</exception>
        public void Write(TextWriter textWriter, string templateName, object context)
            => Write(textWriter, templateName, context, null);

        /// <summary>Writes the provided <paramref name="templateName"/> with in the given <paramref name="context"/> to the provided <paramref name="textWriter"/>.</summary>
        /// <param name="textWriter">The <see cref="TextWriter"/> to which to write the template.</param>
        /// <param name="templateName">The name of the template to apply.</param>
        /// <param name="context">The context in which to apply the template.</param>
        /// <param name="data">An object containing data, this can be used for different data overrides.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="templateName"/> is not defined.</exception>
        public void Write(TextWriter textWriter, string templateName, object context, object data)
        {
            if (!_templates.Value.TryGetValue(templateName, out var templateWriter))
                throw new ArgumentException($"The \"{templateName}\" template does not exist.", nameof(templateName));

            templateWriter(textWriter, context, data);
        }

        /// <summary>Applies the template with the given <paramref name="templateName"/> in the given <paramref name="context"/>.</summary>
        /// <param name="templateName">The template to apply.</param>
        /// <param name="context">The context in which to apply the template.</param>
        /// <returns>Returns the applied template.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="templateName"/> is not defined.</exception>
        public string Apply(string templateName, object context)
            => Apply(templateName, context, null);

        /// <summary>Applies the template with the given <paramref name="templateName"/> in the given <paramref name="context"/>.</summary>
        /// <param name="templateName">The template to apply.</param>
        /// <param name="context">The context in which to apply the template.</param>
        /// <param name="data">An object containing data, this can be used for different data overrides.</param>
        /// <returns>Returns the applied template.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="templateName"/> is not defined.</exception>
        public string Apply(string templateName, object context, object data)
        {
            if (!_templates.Value.TryGetValue(templateName, out var templateWriter))
                throw new ArgumentException($"The \"{templateName}\" template does not exist.", nameof(templateName));

            var result = new StringBuilder();
            using (var stringWriter = new StringWriter(result))
                Write(stringWriter, templateName, context, data);
            return result.ToString();
        }

        /// <summary>Gets a collection of helpers that are available in each template.</summary>
        /// <param name="memberReferenceResolver">The <see cref="IMemberReferenceResolver"/> used to resolve <see cref="ReferenceData.MemberReference"/>s.</param>
        /// <returns>Returns a collection of helpers to use in templates.</returns>
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
        protected virtual IEnumerable<IHelperDescriptor> GetHelpers(IMemberReferenceResolver memberReferenceResolver)
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

        private IEnumerable<EmbeddedDirectory> _EmbeddedResourceDirectories
        {
            get
            {
                yield return _themeDirectory;
                yield return _themeCategoryDirectory;
                yield return _themesDirectory;
            }
        }

        private IReadOnlyDictionary<string, HandlebarsTemplate<TextWriter, object, object>> _GetTemplates()
        {
            var handlerbars = HandlebarsDotNet.Handlebars.Create();

            var helpers = GetHelpers(_memberReferenceResolver)
                ?.GroupBy(helper => helper.Name.ToString(), (helperName, matchedHelpers) => matchedHelpers.Last(), StringComparer.OrdinalIgnoreCase)
                ?? Enumerable.Empty<IHelperDescriptor>();

            foreach (var helper in helpers)
                switch (helper)
                {
                    case IHelperDescriptor<HelperOptions> inlineHelper:
                        handlerbars.RegisterHelper(inlineHelper);
                        break;

                    case IHelperDescriptor<BlockHelperOptions> blockHelper:
                        handlerbars.RegisterHelper(blockHelper);
                        break;

                    default:
                        throw new InvalidOperationException($"Unknown type for \"{helper.Name}\" helper.");
                }

            foreach (var partial in _GetEmbeddedFilesFromResourceDirectory("Partials").Where(template => template.Extension.Equals(".hbs", StringComparison.OrdinalIgnoreCase)))
                using (var partialStream = partial.OpenRead())
                using (var partialStreamReader = new StreamReader(partialStream))
                    handlerbars.RegisterTemplate(_GetTemplateName(partial), partialStreamReader.ReadToEnd());

            return _GetEmbeddedFilesFromResourceDirectory("Templates")
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

        private IEnumerable<EmbeddedFile> _GetEmbeddedFilesFromResourceDirectory(string directoryName)
            => _EmbeddedResourceDirectories
                .SelectMany(embeddedResourceDirectory => embeddedResourceDirectory.Subdirectories.TryGetValue(directoryName, out var embeddedDirectory) ? embeddedDirectory.GetAllFiles() : Enumerable.Empty<EmbeddedFile>())
                .GroupBy(file => file.Name, (name, files) => files.First(), StringComparer.OrdinalIgnoreCase);

        private static string _GetTemplateName(EmbeddedFile embeddedFile)
            => embeddedFile.Name.Substring(0, embeddedFile.Name.Length - embeddedFile.Extension.Length);
    }
}