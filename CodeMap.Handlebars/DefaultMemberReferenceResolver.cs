using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Handlebars.Visitors;
using CodeMap.ReferenceData;

namespace CodeMap.Handlebars
{
    /// <summary>A default implementation for resolving member references.</summary>
    public class DefaultMemberReferenceResolver : IMemberReferenceResolver
    {
        private readonly Assembly _library;
        private readonly string _microsoftDocsView;
        private readonly IDictionary<string, string> _cache;

        /// <summary>Initializes a new instance of the <see cref="DefaultMemberReferenceResolver"/> class.</summary>
        /// <param name="library">The documented library, used to determine whether a local reference should be made or a reference to MS docs should be made instead.</param>
        /// <param name="microsoftDocsView">The view query string parameter when generating MS docs links, this corresponds to the target version.</param>
        public DefaultMemberReferenceResolver(Assembly library, string microsoftDocsView)
            => (_library, _microsoftDocsView, _cache) = (library, microsoftDocsView, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));

        /// <summary>Gets the HTML file name for the provided <paramref name="declarationNode"/>.</summary>
        /// <param name="declarationNode">The <see cref="DeclarationNode"/> for which to generate a file name.</param>
        /// <returns>Returns the HTML file name where the documentation for the provided <paramref name="declarationNode"/> should be saved.</returns>
        public string GetFileName(DeclarationNode declarationNode)
        {
            var memberDeclarationFullNameVisitor = new MemberDeclarationFullNameVisitor(excludeParameters: false);
            declarationNode.Accept(memberDeclarationFullNameVisitor);

            var memberDeclarationBaseNameVisitor = new MemberDeclarationFullNameVisitor(excludeParameters: true);
            declarationNode.Accept(memberDeclarationBaseNameVisitor);

            return _GetEntry(memberDeclarationFullNameVisitor.Result, memberDeclarationBaseNameVisitor.Result);
        }

        /// <summary>Gets the URL for the provided <paramref name="memberReference"/>.</summary>
        /// <param name="memberReference">The <see cref="MemberReference"/> for which to generate the URL.</param>
        /// <returns>Returns the URL for the provided <see cref="MemberReference"/>. If it points to a member of the documented library then an URL for that page is returned; otherwise an MS doc reference is created.</returns>
        public string GetUrl(MemberReference memberReference)
        {
            if (memberReference.Assembly == _library)
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