using System;
using System.IO;
using System.Linq;
using CodeMap.DeclarationNodes;
using CodeMap.Handlebars.Visitors;
using HandlebarsDotNet;

namespace CodeMap.Handlebars.Helpers
{
    /// <summary>A helper used to generate hyperlinks for a <see cref="DeclarationNode"/> or a <see cref="ReferenceData.MemberReference"/>.</summary>
    /// <example>
    /// The following template will generate a paragraph containing a hyperlink to the given parameter.
    /// <code language="html">
    /// &lt;p&gt;{{MemberReference declaringType}}&lt;/p&gt;
    /// </code>
    /// If the current context exposes a <c>declaringType</c> property that is a <see cref="ClassDeclaration"/> named <c>CustomType</c>, the output will be as follows:
    /// <code language="html">
    /// &lt;p&gt;&lt;a href=&quot;CustomType.html&quot;&gt;CustomType&lt;/a&gt;&lt;/p&gt;
    /// </code>
    /// </example>
    public class MemberReference : IHandlebarsHelper
    {
        private readonly IMemberReferenceResolver _memberReferenceResolver;

        /// <summary>Initializes a new instance of the <see cref="MemberReference"/> class.</summary>
        /// <param name="memberReferenceResolver">The <see cref="IMemberReferenceResolver"/> to use for resolving <see cref="DeclarationNode"/>s and <see cref="ReferenceData.MemberReference"/>s.</param>
        public MemberReference(IMemberReferenceResolver memberReferenceResolver)
            => _memberReferenceResolver = memberReferenceResolver;

        /// <summary>Gets the name of the helper.</summary>
        /// <value>The value of this property is <c>MemberReference</c>. It is a constant.</value>
        public string Name
            => nameof(MemberReference);

        /// <summary>Writes a hyperlink (or multiple in case of generic types) for the provided first parameter or context.</summary>
        /// <param name="writer">The <see cref="TextWriter"/> to write the result to.</param>
        /// <param name="context">The context in which this helper is called.</param>
        /// <param name="parameters">The parameter with which this helper has been called.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the first parameter is not a <see cref="DeclarationNode"/> nor a <see cref="ReferenceData.MemberReference"/>; or when not provided and the given <paramref name="context"/> is not a <see cref="DeclarationNode"/> nor a <see cref="ReferenceData.MemberReference"/>.
        /// </exception>
        public void Apply(TextWriter writer, object context, params object[] parameters)
        {
            var parameter = parameters.DefaultIfEmpty(context).First();
            switch (parameter)
            {
                case DeclarationNode declarationNode:
                    var memberDeclarationNameVisitor = new MemberDeclarationNameVisitor();
                    declarationNode.Accept(memberDeclarationNameVisitor);
                    WriteHyperlink(writer, _memberReferenceResolver.GetFileName(declarationNode), memberDeclarationNameVisitor.Result);
                    break;

                case ReferenceData.MemberReference memberReference:
                    memberReference.Accept(new MemberReferenceHyperlinkVisitor(writer, _memberReferenceResolver, WriteHyperlink));
                    break;

                default:
                    throw new ArgumentException($"Unhandled parameter type: '{parameter.GetType().Name}'");
            }
        }

        /// <summary>Writes a hyperlink to the provided <paramref name="textWriter"/>.</summary>
        /// <param name="textWriter">The <see cref="TextWriter"/> to write the hyperlink to.</param>
        /// <param name="url">The URL of the hyperlink.</param>
        /// <param name="content">The context (text) of the hyperlink.</param>
        protected virtual void WriteHyperlink(TextWriter textWriter, string url, string content)
        {
            textWriter.WriteSafeString("<a href=\"");
            textWriter.Write(url);
            textWriter.WriteSafeString("\">");
            textWriter.Write(content);
            textWriter.WriteSafeString("</a>");
        }
    }
}