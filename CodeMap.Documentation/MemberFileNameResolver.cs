using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Documentation.Visitors;
using CodeMap.ReferenceData;

namespace CodeMap.Documentation
{
    public class MemberFileNameResolver
    {
        private readonly Assembly _library;
        private readonly IDictionary<string, string> _cache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public MemberFileNameResolver(Assembly library)
            => _library = library;

        public string GetFileName(DeclarationNode declatationNode)
            => _GetEntry(_GetKey(declatationNode), _GetBaseName(declatationNode));

        public string GetFileName(MemberReference memberReference)
            => memberReference is TypeReference typeReference && typeReference.Assembly == _library
            ? _GetEntry(_GetKey(memberReference), _GetBaseName(memberReference))
            : _GetMicrosoftDocsLink(memberReference);

        private string _GetKey(MemberReference memberReference)
        {
            var memberReferenceFullNameVisitor = new MemberReferenceFullNameVisitor(false);
            memberReference.Accept(memberReferenceFullNameVisitor);
            return memberReferenceFullNameVisitor.Result;
        }

        private string _GetKey(DeclarationNode delcarationNode)
        {
            var memberDeclarationFullNameVisitor = new MemberDeclarationFullNameVisitor(false);
            delcarationNode.Accept(memberDeclarationFullNameVisitor);
            return memberDeclarationFullNameVisitor.Result;
        }

        private string _GetBaseName(MemberReference memberReference)
        {
            var memberReferenceFullNameVisitor = new MemberReferenceFullNameVisitor(true);
            memberReference.Accept(memberReferenceFullNameVisitor);
            return memberReferenceFullNameVisitor.Result;
        }

        private string _GetBaseName(DeclarationNode declarationNode)
        {
            if (declarationNode is AssemblyDeclaration)
                return "Index";

            var memberDeclarationFullNameVisitor = new MemberDeclarationFullNameVisitor(true);
            declarationNode.Accept(memberDeclarationFullNameVisitor);
            return memberDeclarationFullNameVisitor.Result;
        }

        private static string _GetMicrosoftDocsLink(MemberReference memberReference)
        {
            var visitor = new MemberReferenceMicrosoftLinkVisitor();
            memberReference.Accept(visitor);
            return visitor.Result;
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