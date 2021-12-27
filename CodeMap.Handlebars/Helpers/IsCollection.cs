using System.Collections;
using System.Linq;
using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.PathStructure;

namespace CodeMap.Handlebars.Helpers
{
    /// <summary>A helper used to determine whether a value is a collection.</summary>
    /// <example>
    /// The following template will generate a paragraph if the provided values is a collection.
    /// <code language="html">
    /// {{#if (IsCollection this)}}&lt;p&gt;A collection&lt;/p&gt;{{/if}}
    /// </code>
    /// </example>
    public class IsCollection : IHelperDescriptor<HelperOptions>
    {
        /// <summary>Gets the name of the helper.</summary>
        /// <value>The value of this property is <c>IsCollection</c>.</value>
        public PathInfo Name
            => nameof(IsCollection);

        /// <summary>Returns <c>true</c> to the provided <paramref name="options"/> if the first argument (when provided) is an <see cref="IEnumerable"/>. When there are no arguments provided then the current <paramref name="context"/> is checked whether it is an <see cref="IEnumerable"/>.</summary>
        /// <param name="options">The helper options.</param>
        /// <param name="context">The context in which this helper is called.</param>
        /// <param name="arguments">The arguments with which this helper has been called.</param>
        /// <returns>Returns <c>true</c> if the first argument (when missing, the context) is an <see cref="IEnumerable"/>; otherwise <c>false</c>.</returns>
        public object Invoke(in HelperOptions options, in Context context, in Arguments arguments)
            => arguments.DefaultIfEmpty(context).First() is IEnumerable;

        /// <summary>Writes <c>true</c> to the provided <paramref name="output"/> if the first argument (when provided) is an <see cref="IEnumerable"/>. When there are no arguments provided then the current <paramref name="context"/> is checked whether it is an <see cref="IEnumerable"/>.</summary>
        /// <param name="output">The <see cref="EncodedTextWriter"/> to write the result to.</param>
        /// <param name="options">The helper options.</param>
        /// <param name="context">The context in which this helper is called.</param>
        /// <param name="arguments">The arguments with which this helper has been called.</param>
        public void Invoke(in EncodedTextWriter output, in HelperOptions options, in Context context, in Arguments arguments)
        {
            if (arguments.DefaultIfEmpty(context).First() is IEnumerable)
                output.Write(true);
        }
    }
}