using System;
using System.IO;
using System.Linq;

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
    /// The following template will generate the Semver expression for the given parameter.
    /// <code language="html">
    /// &lt;p&gt;{{Semver version}}&lt;/p&gt;
    /// </code>
    /// If the current context exposes a <c>version</c> property that is a <see cref="Version"/> equal to <c>1.0.0.0</c>, the output will be as follows:
    /// <code language="html">
    /// &lt;p&gt;1.0.0&lt;/p&gt;
    /// </code>
    /// </example>
    public class Semver : IHandlebarsHelper
    {
        /// <summary>Gets the name of the helper.</summary>
        /// <value>The value of this property is <c>Semver</c>. It is a constant.</value>
        public string Name
            => nameof(Semver);

        /// <summary>Writes a <a href="https://semver.org/">Semver</a> expression for the provided first parameter or context.</summary>
        /// <param name="writer">The <see cref="TextWriter"/> to write the result to.</param>
        /// <param name="context">The context in which this helper is called.</param>
        /// <param name="parameters">The parameter with which this helper has been called.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the first parameter is not a <see cref="Version"/> or when not provided and the given <paramref name="context"/> is not a <see cref="Version"/>.
        /// </exception>
        public void Apply(TextWriter writer, object context, params object[] parameters)
        {
            var version = parameters.DefaultIfEmpty(context).First() as Version;
            if (version is null)
                throw new ArgumentException("Expected a " + nameof(Version) + " provided as the first parameter or context.");

            writer.Write(version.ToSemver());
        }
    }
}