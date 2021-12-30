using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.PathStructure;

namespace CodeMap.Handlebars.Helpers
{
    /// <summary>A concatenation helper, concatenates each provided argument.</summary>
    /// <example>
    /// The following template will generate a paragraph containing the concatenated values.
    /// <code language="html">
    /// &lt;p&gt;{{Concat 'Is this your ' this ' value?}}&lt;/p&gt;
    /// </code>
    /// If the current context is a number, say <c>5</c> the output would be:
    /// <code language="html">
    /// &lt;p&gt;Is this your 5 value?&lt;/p&gt;
    /// </code>
    /// </example>
    public class Concat : IHelperDescriptor<HelperOptions>
    {
        /// <summary>Gets the name of the helper.</summary>
        /// <value>The value of this property is <c>Concat</c>.</value>
        public PathInfo Name
            => nameof(Concat);

        /// <summary>Returns the concatenated <paramref name="arguments"/>.</summary>
        /// <param name="options">The helper options.</param>
        /// <param name="context">The context in which this helper is called.</param>
        /// <param name="arguments">The arguments with which this helper has been called.</param>
        /// <returns>Returns the concatenated <paramref name="arguments"/>.</returns>
        public object Invoke(in HelperOptions options, in Context context, in Arguments arguments)
            => string.Join(string.Empty, arguments.AsEnumerable());

        /// <summary>Writes the concatenated <paramref name="arguments"/> to the provided <paramref name="output"/>.</summary>
        /// <param name="output">The <see cref="EncodedTextWriter"/> to write the result to.</param>
        /// <param name="options">The helper options.</param>
        /// <param name="context">The context in which this helper is called.</param>
        /// <param name="arguments">The arguments with which this helper has been called.</param>
        public void Invoke(in EncodedTextWriter output, in HelperOptions options, in Context context, in Arguments arguments)
        {
            foreach (var argument in arguments)
                output.WriteSafeString(argument ?? string.Empty);
        }
    }
}