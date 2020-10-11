using System.IO;

namespace CodeMap.Handlebars.Helpers
{
    /// <summary>Represents an inline Handlebars helper.</summary>
    /// <remarks>Helper names should be globally unique.</remarks>
    public interface IHandlebarsHelper
    {
        /// <summary>Gets the name of the helper.</summary>
        string Name { get; }

        /// <summary>Applies the helper in the provided <paramref name="context"/> with the given <paramref name="parameters"/>.</summary>
        /// <param name="writer">The <see cref="TextWriter"/> to which to write the result.</param>
        /// <param name="context">The context in which the helper has been called.</param>
        /// <param name="parameters">The parameters with which the helper was called.</param>
        /// <exception cref="System.ArgumentException">Thrown when any of the provided parameters are invaid.</exception>
        void Apply(TextWriter writer, object context, params object[] parameters);
    }
}