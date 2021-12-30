using System;
using System.IO;
using System.Linq;
using System.Text;
using CodeMap.DeclarationNodes;
using CodeMap.Handlebars.Visitors;
using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.IO;
using HandlebarsDotNet.PathStructure;

namespace CodeMap.Handlebars.Helpers
{
    /// <summary>A helper used to generate hyperlinks for a <see cref="DeclarationNode"/> or a <see cref="ReferenceData.MemberReference"/>.</summary>
    /// <example>
    /// The following template will generate a paragraph containing a hyperlink to the given argument.
    /// <code language="html">
    /// &lt;p&gt;{{MemberReference declaringType}}&lt;/p&gt;
    /// </code>
    /// If the current context exposes a <c>declaringType</c> property that is a <see cref="ClassDeclaration"/> named <c>CustomType</c>, the output will be as follows:
    /// <code language="html">
    /// &lt;p&gt;&lt;a href=&quot;CustomType.html&quot;&gt;CustomType&lt;/a&gt;&lt;/p&gt;
    /// </code>
    /// </example>
    public class MemberReference : IHelperDescriptor<HelperOptions>
    {
        private readonly IMemberReferenceResolver _memberReferenceResolver;

        /// <summary>Initializes a new instance of the <see cref="MemberReference"/> class.</summary>
        /// <param name="memberReferenceResolver">The <see cref="IMemberReferenceResolver"/> to use for resolving <see cref="DeclarationNode"/>s and <see cref="ReferenceData.MemberReference"/>s.</param>
        public MemberReference(IMemberReferenceResolver memberReferenceResolver)
            => _memberReferenceResolver = memberReferenceResolver;

        /// <summary>Gets the name of the helper.</summary>
        /// <value>The value of this property is <c>MemberReference</c>.</value>
        public PathInfo Name
            => nameof(MemberReference);

        /// <summary>Gets a hyperlink (or multiple in case of generic types) for the provided first argument or context.</summary>
        /// <param name="options">The helper options.</param>
        /// <param name="context">The context in which this helper is called.</param>
        /// <param name="arguments">The arguments with which this helper has been called.</param>
        /// <returns>Returns a hyperlink (or multiple in case of generic types) for the provided first argument or context.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the first argument is not a <see cref="DeclarationNode"/> nor a <see cref="ReferenceData.MemberReference"/>; or when not provided and the given <paramref name="context"/> is not a <see cref="DeclarationNode"/> nor a <see cref="ReferenceData.MemberReference"/>.
        /// </exception>
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

        /// <summary>Writes a hyperlink (or multiple in case of generic types) for the provided first argument or context to the provided <paramref name="output"/>.</summary>
        /// <param name="output">The <see cref="EncodedTextWriter"/> to write the result to.</param>
        /// <param name="options">The helper options.</param>
        /// <param name="context">The context in which this helper is called.</param>
        /// <param name="arguments">The arguments with which this helper has been called.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the first argument is not a <see cref="DeclarationNode"/> nor a <see cref="ReferenceData.MemberReference"/>; or when not provided and the given <paramref name="context"/> is not a <see cref="DeclarationNode"/> nor a <see cref="ReferenceData.MemberReference"/>.
        /// </exception>
        public void Invoke(in EncodedTextWriter output, in HelperOptions options, in Context context, in Arguments arguments)
        {
            var argument = arguments.DefaultIfEmpty(context.Value).First();

            switch (argument)
            {
                case DeclarationNode declarationNode:
                    var nameBuilder = new StringBuilder();
                    declarationNode.AsMeberReference().Accept(new MemberReferenceNameBuilderVisitor(nameBuilder));
                    WriteHyperlink(output, _memberReferenceResolver.GetUrl(declarationNode.AsMeberReference()), nameBuilder.ToString());
                    break;

                case ReferenceData.MemberReference memberReference:
                    memberReference.Accept(new MemberReferenceHyperlinkVisitor(output, _memberReferenceResolver, WriteHyperlink));
                    break;

                default:
                    throw new ArgumentException($"Unhandled argument type: '{argument.GetType().Name}'");
            }
        }

        /// <summary>Writes a hyperlink to the provided <paramref name="output"/>.</summary>
        /// <param name="output">The <see cref="EncodedTextWriter"/> to write the hyperlink to.</param>
        /// <param name="url">The URL of the hyperlink.</param>
        /// <param name="content">The content (text) of the hyperlink.</param>
        protected virtual void WriteHyperlink(EncodedTextWriter output, string url, string content)
        {
            output.Write("<a href=\"", encode: false);
            output.WriteSafeString(url);
            output.Write("\">", encode: false);
            output.WriteSafeString(content);
            output.Write("</a>", encode: false);
        }
    }
}