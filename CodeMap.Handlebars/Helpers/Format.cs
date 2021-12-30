using System;
using System.Globalization;
using System.Linq;
using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.PathStructure;

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
    public class Format : IHelperDescriptor<HelperOptions>
    {
        /// <summary>Gets the name of the helper.</summary>
        /// <value>The value of this property is <c>Format</c>.</value>
        public PathInfo Name
            => nameof(Format);

        /// <summary>Formats the first argument which is expected to be an <see cref="IFormattable"/> by applying the format string provided through the second argument which is expected to be a <see cref="string"/>.</summary>
        /// <param name="options">The helper options.</param>
        /// <param name="context">The context in which this helper is called.</param>
        /// <param name="arguments">The arguments with which this helper has been called.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the first argument does not implement <see cref="IFormattable"/> or when the second argument is not a <see cref="string"/>.
        /// </exception>
        public object Invoke(in HelperOptions options, in Context context, in Arguments arguments)
        {
            var formattable = arguments.ElementAtOrDefault(0) as IFormattable ?? throw new ArgumentException("Expected an " + nameof(IFormattable) + " as the first argument.");
            var format = arguments.ElementAtOrDefault(1) as string ?? throw new ArgumentException("Expected a format string as the second argument.");
            return formattable.ToString(format, CultureInfo.InvariantCulture);
        }

        /// <summary>Formats the first argument which is expected to be an <see cref="IFormattable"/> by applying the format string provided through the second argument which is expected to be a <see cref="string"/>.</summary>
        /// <param name="output">The <see cref="EncodedTextWriter"/> to write the result to.</param>
        /// <param name="options">The helper options.</param>
        /// <param name="context">The context in which this helper is called.</param>
        /// <param name="arguments">The arguments with which this helper has been called.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the first argument does not implement <see cref="IFormattable"/> or when the second argument is not a <see cref="string"/>.
        /// </exception>
        public void Invoke(in EncodedTextWriter output, in HelperOptions options, in Context context, in Arguments arguments)
            => output.WriteSafeString(Invoke(options, context, arguments) ?? string.Empty);
    }
}