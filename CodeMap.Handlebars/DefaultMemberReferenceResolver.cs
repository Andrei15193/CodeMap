using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Handlebars.Visitors;
using CodeMap.ReferenceData;

namespace CodeMap.Handlebars
{
    public class DefaultMemberReferenceResolver : IMemberReferenceResolver
    {
        private readonly Assembly _library;
        private readonly string _microsoftDocsView;
        private readonly IDictionary<string, string> _cache;

        public DefaultMemberReferenceResolver(Assembly library, string microsoftDocsView)
            => (_library, _microsoftDocsView, _cache) = (library, microsoftDocsView, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));

        public string GetFileName(DeclarationNode declarationNode)
        {
            if (declarationNode is AssemblyDeclaration)
                return "index.html";
            else
            {
                var memberDeclarationFullNameVisitor = new MemberDeclarationFullNameVisitor(excludeParameters: false);
                declarationNode.Accept(memberDeclarationFullNameVisitor);

                var memberDeclarationBaseNameVisitor = new MemberDeclarationFullNameVisitor(excludeParameters: true);
                declarationNode.Accept(memberDeclarationBaseNameVisitor);

                return _GetEntry(memberDeclarationFullNameVisitor.Result, memberDeclarationBaseNameVisitor.Result);
            }
        }

        public string GetUrl(MemberReference memberReference)
        {
            var librarySelectorVisitor = new LibrarySelectorVisitor();
            memberReference.Accept(librarySelectorVisitor);

            if (librarySelectorVisitor.Library == _library)
            {
                var memberReferenceFullNameVisitor = new MemberReferenceFullNameVisitor(excludeParameters: false);
                memberReference.Accept(memberReferenceFullNameVisitor);

                var memberReferenceBaseNameVisitor = new MemberReferenceFullNameVisitor(excludeParameters: true);
                memberReference.Accept(memberReferenceBaseNameVisitor);

                return _GetEntry(memberReferenceFullNameVisitor.Result, memberReferenceBaseNameVisitor.Result);
            }
            else
            {
                var memberReferenceMicrosoftLinkVisitor = new MemberReferenceMicrosoftLinkVisitor(_microsoftDocsView);
                memberReference.Accept(memberReferenceMicrosoftLinkVisitor);
                return memberReferenceMicrosoftLinkVisitor.Result;
            }
        }

        private string _GetEntry(string key, string baseFileName)
        {
            if (_cache.TryGetValue(key, out var fileName))
                return fileName;

            var count = 1;
            fileName = $"{baseFileName}.html";
            while (_cache.Values.Contains(fileName, StringComparer.OrdinalIgnoreCase))
            {
                count++;
                fileName = $"{baseFileName}-{count}.html";
            }
            _cache.Add(key, fileName);
            return fileName;
        }
    }
}