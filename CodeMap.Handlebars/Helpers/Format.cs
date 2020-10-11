using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CodeMap.Handlebars.Helpers
{
    /// <summary>A format helper, useful for applying different formats to a value.</summary>
    /// <example>
    /// The following template will generate a paragraph containing the formatted value.
    /// <code language="html">
    /// &lt;p&gt;{{Format this '0.00'}}&lt;/p&gt;
    /// </code>
    /// If the current context is a number, say <c>123</c> the output would be:
    /// <code language="html">
    /// &lt;p&gt;123.00&lt;/p&gt;
    /// </code>
    /// </example>
    public class Format : IHandlebarsHelper
    {
        /// <summary>Gets the name of the helper.</summary>
        /// <value>The value of this property is <c>Format</c>. It is a constant.</value>
        public string Name
            => nameof(Format);

        /// <summary>Formats the first parameter which is expected to be an <see cref="IFormattable"/> by applying the format string provided through the second parameter which is expected to be a <see cref="string"/>.</summary>
        /// <param name="writer">The <see cref="TextWriter"/> to write the result to.</param>
        /// <param name="context">The context in which this helper is called.</param>
        /// <param name="parameters">The parameter with which this helper has been called.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the first parameter does not implement <see cref="IFormattable"/> or when the second parameter is not a <see cref="string"/>.
        /// </exception>
        public void Apply(TextWriter writer, object context, params object[] parameters)
        {
            var formattable = parameters.ElementAtOrDefault(0) as IFormattable ?? throw new ArgumentException("Expected a " + nameof(IFormattable) + " as the first parameter.");
            var format = parameters.ElementAtOrDefault(1) as string ?? throw new ArgumentException("Expected a format string as the second parameter.");
            writer.Write(formattable.ToString(format, CultureInfo.InvariantCulture));
        }
    }
}