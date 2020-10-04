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

            var lexer = GetLexer(language?.ToLowerInvariant());
            if (lexer is null)
            {
                writer.WriteSafeString("<pre><code>");
                writer.Write(code);
                writer.WriteSafeString("</pre></code>");
            }
            else
                writer.WriteSafeString(Pygmentize.Content(code).WithLexer(lexer).WithFormatter(new HtmlFormatter(new HtmlFormatterOptions())).AsString());
        }

        protected virtual Lexer GetLexer(string language)
            => language switch
            {
                "c#" => new CSharpLexer(),
                "xml" => new HtmlLexer(),
                "html" => new HtmlLexer(),
                "css" => new CssLexer(),
                "js" => new JavascriptLexer(),
                "ts" => new TypescriptLexer(),
                "bash" => new BashLexer(),
                "sql" => new SqlLexer(),
                _ => null,
            };
    }
}