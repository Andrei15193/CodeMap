using System.IO;
using HandlebarsDotNet;
using PygmentSharp.Core;
using PygmentSharp.Core.Formatting;
using PygmentSharp.Core.Lexing;

namespace CodeMap.Documentation.Helpers
{
    public class Pygments : IHandlebarsHelper
    {
        public string Name
            => nameof(Pygments);

        public void Apply(TextWriter writer, dynamic context, params object[] parameters)
        {
            var code = (string)parameters[0];
            var language = HandlebarsUtils.IsUndefinedBindingResult(parameters[1]) ? null : (string)parameters[1];

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