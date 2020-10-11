using System.Collections;
using System.IO;
using System.Linq;

namespace CodeMap.Handlebars.Helpers
{
    /// <summary>A helper used to determine whether a value is a collection.</summary>
    /// <example>
    /// The following template will generate a paragraph if the provided values is a collection.
    /// <code language="html">
    /// {{#if (IsCollection this)}}&lt;p&gt;A collection&lt;/p&gt;{{/if}}
    /// </code>
    /// </example>
    public class IsCollection : IHandlebarsHelper
    {
        /// <summary>Gets the name of the helper.</summary>
        /// <value>The value of this property is <c>IsCollection</c>. It is a constant.</value>
        public string Name
            => nameof(IsCollection);

        /// <summary>Writes <c>true</c> to the provided <paramref name="writer"/> if the first parameter (when provided) is an <see cref="IEnumerable"/>. When there are no parameters provided then the current <paramref name="context"/> is checked whether it is an <see cref="IEnumerable"/>.</summary>
        /// <param name="writer">The <see cref="TextWriter"/> to write the result to.</param>
        /// <param name="context">The context in which this helper is called.</param>
        /// <param name="parameters">The parameter with which this helper has been called.</param>
        public void Apply(TextWriter writer, object context, params object[] parameters)
        {
            if (parameters.DefaultIfEmpty(context).First() is IEnumerable)
                writer.Write(true);
        }
    }
}