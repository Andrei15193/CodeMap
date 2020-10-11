using System.IO;

namespace CodeMap.Handlebars.Helpers
{
    /// <summary>A concatenation helper, concatenates each provided parameter.</summary>
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
    public class Concat : IHandlebarsHelper
    {
        /// <summary>Gets the name of the helper.</summary>
        /// <value>The value of this property is <c>Concat</c>. It is a constant.</value>
        public string Name
            => nameof(Concat);

        /// <summary>Applies the concatenation by writing each parameter to the provided <paramref name="writer"/>.</summary>
        /// <param name="writer">The <see cref="TextWriter"/> to write the result to.</param>
        /// <param name="context">The context in which this helper is called.</param>
        /// <param name="parameters">The parameter with which this helper has been called.</param>
        public void Apply(TextWriter writer, object context, params object[] parameters)
        {
            foreach (var parameter in parameters)
                writer.Write(parameter);
        }
    }
}