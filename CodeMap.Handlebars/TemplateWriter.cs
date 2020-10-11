using System.IO;
using System.Text;

namespace CodeMap.Handlebars
{
    /// <summary>Represents a base class for <see cref="TemplateWriter"/>s.</summary>
    public abstract class TemplateWriter
    {
        /// <summary>Applies the template with the given <paramref name="templateName"/> in the given <paramref name="context"/> and writes it to the provided <paramref name="writer"/>.</summary>
        /// <param name="writer">The <see cref="TextWriter"/> to which to apply the template to.</param>
        /// <param name="templateName">The template name to apply.</param>
        /// <param name="context">The context in which to apply the template.</param>
        public abstract void Write(TextWriter writer, string templateName, object context);

        /// <summary>Applies the template with the given <paramref name="templateName"/> in the given <paramref name="context"/>.</summary>
        /// <param name="templateName">The template to apply.</param>
        /// <param name="context">The context in which to apply the template.</param>
        /// <returns>Returns the applied template.</returns>
        public string Apply(string templateName, object context)
        {
            var result = new StringBuilder();
            using (var stringWriter = new StringWriter(result))
                Write(stringWriter, templateName, context);
            return result.ToString();
        }
    }
}