using System.IO;
using System.Linq;
using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.IO;
using HandlebarsDotNet.PathStructure;
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
    public class Pygments : IHelperDescriptor<HelperOptions>
    {
        /// <summary>Gets the name of the helper.</summary>
        /// <value>The value of this property is <c>Pygments</c>.</value>
        public PathInfo Name
            => nameof(Pygments);

        /// <summary>Gets a code block with highlighting.</summary>
        /// <param name="options">The helper options.</param>
        /// <param name="context">The context in which this helper is called.</param>
        /// <param name="arguments">The arguments with which this helper has been called.</param>
        /// <returns>Returns a code block with highlighting to the provided.</returns>
        /// <remarks>
        /// The first argument is the code to be highlighted while the second specifies the language for the lexer. If no language is specified or a
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
        public object Invoke(in HelperOptions options, in Context context, in Arguments arguments)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var output = new EncodedTextWriter(stringWriter, new HtmlEncoder(), new DefaultFormatterProvider()))
                    Invoke(output, options, context, arguments);
                stringWriter.Flush();
                return stringWriter.ToString();
            }
        }

        /// <summary>Writes a code block with highlighting to the provided <paramref name="output"/>.</summary>
        /// <param name="output">The <see cref="EncodedTextWriter"/> to write the result to.</param>
        /// <param name="options">The helper options.</param>
        /// <param name="context">The context in which this helper is called.</param>
        /// <param name="arguments">The arguments with which this helper has been called.</param>
        /// <remarks>
        /// The first argument is the code to be highlighted while the second specifies the language for the lexer. If no language is specified or a
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
        public void Invoke(in EncodedTextWriter output, in HelperOptions options, in Context context, in Arguments arguments)
        {
            var code = arguments.ElementAtOrDefault(0) as string ?? string.Empty;
            var language = arguments.ElementAtOrDefault(1) as string;

            var lexer = language is null ? null : GetLexer(language.ToLowerInvariant());
            if (lexer is null)
            {
                output.WriteSafeString("<pre><code>");
                output.Write(code);
                output.WriteSafeString("</pre></code>");
            }
            else
                output.Write(Pygmentize.Content(code).WithLexer(lexer).WithFormatter(new HtmlFormatter(new HtmlFormatterOptions())).AsString());
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