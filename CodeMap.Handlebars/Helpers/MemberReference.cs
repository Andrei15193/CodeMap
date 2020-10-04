using System;
using System.IO;
using System.Linq;
using CodeMap.DeclarationNodes;
using CodeMap.Handlebars.Visitors;
using HandlebarsDotNet;

namespace CodeMap.Handlebars.Helpers
{
    public class MemberReference : IHandlebarsHelper
    {
        private readonly IMemberReferenceResolver _memberReferenceResolver;

        public MemberReference(IMemberReferenceResolver memberReferenceResolver)
            => _memberReferenceResolver = memberReferenceResolver;

        public string Name
            => nameof(MemberReference);

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