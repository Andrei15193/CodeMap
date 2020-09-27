using System.IO;
using System.Text;

namespace CodeMap.Documentation
{
    public abstract class TemplateWriter
    {
        public abstract void Write(TextWriter writer, string templateName, object context);

        public string Apply(string templateName, object context)
        {
            var result = new StringBuilder();
            using (var stringWriter = new StringWriter(result))
                Write(stringWriter, templateName, context);
            return result.ToString();
        }
    }
}