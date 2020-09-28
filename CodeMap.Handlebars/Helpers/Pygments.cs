using System.IO;
using System.Linq;
using HandlebarsDotNet;
using PygmentSharp.Core;
using PygmentSharp.Core.Formatting;
using PygmentSharp.Core.Lexing;

namespace CodeMap.Handlebars.Helpers
{
    public class Pygments : IHandlebarsHelper
    {
        public string Name
            => nameof(Pygments);

        public void Apply(TextWriter writer, object context, params object[] parameters)
        {
            string code = parameters.ElementAtOrDefault(0) as string ?? string.Empty;
            string language = parameters.ElementAtOrDefault(1) as string;
            Lexer lexer = null;
            switch (language?.ToLowerInvariant())
            {
                case "c#":
                    lexer = new CSharpLexer();
                    break;

                case "xml":
                    lexer = new HtmlLexer();
                    break;
            }
            if (lexer != null)
                writer.WriteSafeString(Pygmentize.Content(code).WithLexer(lexer).WithFormatter(new HtmlFormatter(new HtmlFormatterOptions())).AsString());
            else
            {
                writer.WriteSafeString("<pre><code>");
                writer.Write(code);
                writer.WriteSafeString("</pre></code>");
            }
        }
    }
}