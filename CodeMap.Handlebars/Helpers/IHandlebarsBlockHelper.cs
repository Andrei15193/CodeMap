using System.IO;
using HandlebarsDotNet;

namespace CodeMap.Handlebars.Helpers
{
    /// <summary>Represents a black Handlebars helper.</summary>
    /// <remarks>Helper names should be globally unique.</remarks>
    public interface IHandlebarsBlockHelper
    {
        /// <summary>Gets the name of the helper.</summary>
        string Name { get; }

        /// <summary>Applies the helper in the provided <paramref name="context"/> with the given <paramref name="parameters"/>.</summary>
        /// <param name="writer">The <see cref="TextWriter"/> to which to write the result.</param>
        /// <param name="options">The options with which to apply the block helper.</param>
        /// <param name="context">The context in which the helper has been called.</param>
        /// <param name="parameters">The parameters with which the helper was called.</param>
        void Apply(TextWriter writer, HelperOptions options, object context, params object[] parameters);
    }
}