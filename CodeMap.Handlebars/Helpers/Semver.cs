using System;
using System.Linq;
using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.PathStructure;

namespace CodeMap.Handlebars.Helpers
{
    /// <summary>A helper used to convert a <see cref="Version"/> into a <a href="https://semver.org/">Semver</a> expression.</summary>
    /// <remarks>
    /// The <a href="https://semver.org/">Semver</a> mapping takes into account all 4 parts of a <see cref="Version"/> object.
    /// <list type="table">
    /// <listheader>
    /// <description>Part</description>
    /// <description>Description</description>
    /// </listheader>
    /// <item><description><see cref="Version.Major"/></description><description>Major</description></item>
    /// <item><description><see cref="Version.Minor"/></description><description>Minor</description></item>
    /// <item><description><see cref="Version.Revision"/></description><description>Patch</description></item>
    /// <item>
    /// <description><see cref="Version.Build"/></description>
    /// <description>This is translated into Prerelease if the value is part of the following intervals:
    /// <list>
    /// <item>Between <c>1000</c> and <c>1999</c>: <c>alpha</c> release (from <c>000</c> to <c>999</c>)</item>
    /// <item>Between <c>2000</c> and <c>2999</c>: <c>beta</c> release (from <c>000</c> to <c>999</c>)</item>
    /// <item>Between <c>3000</c> and <c>3999</c>: <c>rc</c> release (from <c>000</c> to <c>999</c>)</item>
    /// </list>
    /// Basically a 4-digit code where the first one indicates the type of prerelease
    /// (<c>1</c> - <c>alpha</c>, <c>2</c> - <c>beta</c>, <c>3</c> - <c>rc</c>) where the remaining 3 digits is the
    /// number of the prerelease (<c>2001</c> means <c>beta1</c>)
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <example>
    /// The following template will generate the Semver expression for the given argument.
    /// <code language="html">
    /// &lt;p&gt;{{Semver version}}&lt;/p&gt;
    /// </code>
    /// If the current context exposes a <c>version</c> property that is a <see cref="Version"/> equal to <c>1.0.0.0</c>, the output will be as follows:
    /// <code language="html">
    /// &lt;p&gt;1.0.0&lt;/p&gt;
    /// </code>
    /// </example>
    public class Semver : IHelperDescriptor<HelperOptions>
    {
        /// <summary>Gets the name of the helper.</summary>
        /// <value>The value of this property is <c>Semver</c>.</value>
        public PathInfo Name
            => nameof(Semver);

        /// <summary>Gets a <a href="https://semver.org/">Semver</a> expression for the provided first argument or context.</summary>
        /// <param name="options">The helper options.</param>
        /// <param name="context">The context in which this helper is called.</param>
        /// <param name="arguments">The arguments with which this helper has been called.</param>
        /// <returns>Returns a <a href="https://semver.org/">Semver</a> expression for the provided first argument or context.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the first argument is not a <see cref="Version"/> or when not provided and the given <paramref name="context"/> is not a <see cref="Version"/>.
        /// </exception>
        public object Invoke(in HelperOptions options, in Context context, in Arguments arguments)
        {
            var version = arguments.ElementAtOrDefault(0) as Version ?? throw new ArgumentException("Expected a " + nameof(Version) + " provided as the first argument or context.");
            return ToSemver(version);
        }

        /// <summary>Writes a <a href="https://semver.org/">Semver</a> expression for the provided first argument or context to the provided <paramref name="output"/>.</summary>
        /// <param name="output">The <see cref="EncodedTextWriter"/> to write the result to.</param>
        /// <param name="options">The helper options.</param>
        /// <param name="context">The context in which this helper is called.</param>
        /// <param name="arguments">The arguments with which this helper has been called.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the first argument is not a <see cref="Version"/> or when not provided and the given <paramref name="context"/> is not a <see cref="Version"/>.
        /// </exception>
        public void Invoke(in EncodedTextWriter output, in HelperOptions options, in Context context, in Arguments arguments)
            => output.WriteSafeString(Invoke(options, context, arguments));

        /// <summary>Gets the Semver expression for the givem <paramref name="version"/>.</summary>
        /// <param name="version">The <see cref="Version"/> to convert to a Semver expression.</param>
        /// <returns>Returns the Semver expression for the given <paramref name="version"/>.</returns>
        /// <remarks>
        /// See the remarks on the declarying type (<see cref="Semver"/>) for more information about the default mapping between
        /// a <see cref="Version"/> and a Semver expression.
        /// </remarks>
        public static string ToSemver(Version version)
        {
            var prerelease = string.Empty;
            if (version.Build > 0)
                switch (version.Build / 1000)
                {
                    case 1:
                        prerelease = "-alpha" + version.Build % 1000;
                        break;

                    case 2:
                        prerelease = "-beta" + version.Build % 1000;
                        break;

                    case 3:
                        prerelease = "-rc" + version.Build % 1000;
                        break;
                }

            return $"{version.Major}.{version.Minor}.{version.Revision}{prerelease}";
        }
    }
}