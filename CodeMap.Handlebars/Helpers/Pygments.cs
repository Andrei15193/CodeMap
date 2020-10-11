using System.IO;
using System.Linq;
using HandlebarsDotNet;
using PygmentSharp.Core;
using PygmentSharp.Core.Formatting;
using PygmentSharp.Core.Lexing;

namespace CodeMap.Handlebars.Helpers
{
    /// <summary>A helper used to generate a code block having highlighted syntax, this helper is based on <a href="https://github.com/akatakritos/PygmentSharp">PygmentSharp</a>.</summary>
    /// <example>
    /// The following template will generate a code block containing C# highlghed code.
    /// <code language="html">
    /// {{Pygments 'public class MyClass { }' 'c#'}}
    /// </code>
    /// </example>
    public class Pygments : IHandlebarsHelper
    {
        /// <summary>Gets the name of the helper.</summary>
        /// <value>The value of this property is <c>Pygments</c>. It is a constant.</value>
        public string Name
            => nameof(Pygments);

        /// <summary>Writes a code block with highlighting to the provided <paramref name="writer"/>.</summary>
        /// <param name="writer">The <see cref="TextWriter"/> to write the result to.</param>
        /// <param name="context">The context in which this helper is called.</param>
        /// <param name="parameters">The parameter with which this helper has been called.</param>
        /// <remarks>
        /// The first parameter is the code to be highlighted while the second specifies the language for the lexer. If no language is specified or a
        /// lexer is not defined for that language then no highlighting is made. The currently supported languages are as follows:
        /// <list>
        /// <item>C#</item>
        /// <item>XML - Extensible Markup Language</item>
        /// <item>HTML - HyperText Markup Language</item>
        /// <item>CSS - Cascading Style Sheets</item>
        /// <item>JS - JavaScript</item>
        /// <item>TS - TypeScript</item>
        /// <item>Bash - Bourne Again Shell (Linux Bash)</item>
        /// <item>SQL - Structured Query Language</item>
        /// </list>
        /// </remarks>
        public void Apply(TextWriter writer, object context, params object[] parameters)
        {
            var code = parameters.ElementAtOrDefault(0) as string ?? string.Empty;
            var language = parameters.ElementAtOrDefault(1) as string;

            var lexer = language is null ? null : GetLexer(language.ToLowerInvariant());
            if (lexer is null)
            {
                writer.WriteSafeString("<pre><code>");
                writer.Write(code);
                writer.WriteSafeString("</pre></code>");
            }
            else
                writer.WriteSafeString(Pygmentize.Content(code).WithLexer(lexer).WithFormatter(new HtmlFormatter(new HtmlFormatterOptions())).AsString());
        }

        /// <summary>Gets the <see cref="Lexer"/> for the provided <paramref name="language"/>.</summary>
        /// <param name="language">The language for which to get the <see cref="Lexer"/> from.</param>
        /// <returns>Returns the <see cref="Lexer"/> for the provided <paramref name="language"/> or <c>null</c> if there isn't one available.</returns>
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