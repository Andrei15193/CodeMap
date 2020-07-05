using System.IO;
using HandlebarsDotNet;
using PygmentSharp.Core;
using PygmentSharp.Core.Formatting;
using PygmentSharp.Core.Lexing;

namespace CodeMap.Documentation.Helpers
{
    public class Pygments : HandlebarsContextualHelper<string, string>
    {
        public override string Name
            => nameof(Pygments);

        public override void Apply(TextWriter writer, PageContext context, string code, string language)
        {
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