using System;
using System.Collections.Generic;
using System.Linq;
using CodeMap.DeclarationNodes;

namespace CodeMap.Documentation
{
    public class PageContext
    {
        public PageContext(MemberFileNameResolver memberFileNameResolver, DeclarationNode declarationNode)
            => (MemberFileNameResolver, DeclarationNode) = (memberFileNameResolver, declarationNode);

        protected PageContext(PageContext pageContext)
            => (MemberFileNameResolver, DeclarationNode) = (pageContext.MemberFileNameResolver, pageContext.DeclarationNode);

        public DeclarationNode DeclarationNode { get; }

        public MemberFileNameResolver MemberFileNameResolver { get; }

        public string Title => DeclarationNode switch
        {
            AssemblyDeclaration assembly => assembly.Name,
            NamespaceDeclaration @namespace => $"{Assembly.Name} - {@namespace.Name}",
            TypeDeclaration type => $"{Assembly.Name} - {type.Name}",
            MemberDeclaration member => $"{Assembly.Name} - {member.Name}",
            _ => throw new ArgumentException($"Declaration node type '{DeclarationNode.GetType().Name}' is not treated.", nameof(DeclarationNode))
        };

        public string AssemblyName => Assembly.Name;

        public string AssemblyVersion => Assembly.Version.ToSemver();

        public AssemblyDeclaration Assembly => DeclarationNode switch
        {
            AssemblyDeclaration assembly => assembly,
            NamespaceDeclaration @namespace => @namespace.Assembly,
            TypeDeclaration type => type.Assembly,
            MemberDeclaration member => member.DeclaringType.Assembly,
            _ => throw new ArgumentException($"Declaration node type '{DeclarationNode.GetType().Name}' is not treated.", nameof(DeclarationNode))
        };

        public IEnumerable<PageBreadcrumb> Breadcrumbs
            => DeclarationNode switch
            {
                AssemblyDeclaration assembly => _GetBreadcrumbs(assembly),
                NamespaceDeclaration @namespace => _GetBreadcrumbs(@namespace),
                TypeDeclaration type => _GetBreadcrumbs(type),
                MemberDeclaration memberDeclaration => _GetBreadcrumbs(memberDeclaration),
                _ => throw new ArgumentException($"DeclarationNode type not handled: '{DeclarationNode.GetType().Name}'.")
            };

        public PageContextWithData<TData> WithData<TData>(TData data)
            => new PageContextWithData<TData>(this, data);

        private IEnumerable<PageBreadcrumb> _GetBreadcrumbs(AssemblyDeclaration assembly)
        {
            yield return new PageBreadcrumb(assembly.Name);
        }

        private IEnumerable<PageBreadcrumb> _GetBreadcrumbs(NamespaceDeclaration @namespace)
        {
            yield return new PageBreadcrumb(@namespace.Assembly.Name, MemberFileNameResolver.GetFileName(@namespace.Assembly));
            yield return new PageBreadcrumb(@namespace.Name);
        }

        private IEnumerable<PageBreadcrumb> _GetBreadcrumbs(TypeDeclaration typeDeclaration)
        {
            var typeDeclarations = new Stack<TypeDeclaration>();
            while (!(typeDeclaration is null))
            {
                typeDeclarations.Push(typeDeclaration);
                typeDeclaration = typeDeclaration.DeclaringType;
            }

            yield return new PageBreadcrumb(typeDeclarations.Peek().Assembly.Name, MemberFileNameResolver.GetFileName(typeDeclarations.Peek().Assembly));
            yield return new PageBreadcrumb(typeDeclarations.Peek().Namespace.Name, MemberFileNameResolver.GetFileName(typeDeclarations.Peek().Namespace));
            while (typeDeclarations.Count > 1)
            {
                var currentTypeDeclaration = typeDeclarations.Pop();
                yield return new PageBreadcrumb(currentTypeDeclaration.Name, MemberFileNameResolver.GetFileName(currentTypeDeclaration));
            }
            yield return new PageBreadcrumb(typeDeclarations.Peek().Name);
        }

        private IEnumerable<PageBreadcrumb> _GetBreadcrumbs(MemberDeclaration memberDeclaration)
        {
            var typeDeclarations = new Stack<TypeDeclaration>();
            var typeDeclaration = memberDeclaration.DeclaringType;
            while (!(typeDeclaration is null))
            {
                typeDeclarations.Push(typeDeclaration);
                typeDeclaration = typeDeclaration.DeclaringType;
            }

            yield return new PageBreadcrumb(typeDeclarations.Peek().Assembly.Name, MemberFileNameResolver.GetFileName(typeDeclarations.Peek().Assembly));
            yield return new PageBreadcrumb(typeDeclarations.Peek().Namespace.Name, MemberFileNameResolver.GetFileName(typeDeclarations.Peek().Namespace));
            while (typeDeclarations.Any())
            {
                var currentTypeDeclaration = typeDeclarations.Pop();
                yield return new PageBreadcrumb(currentTypeDeclaration.Name, MemberFileNameResolver.GetFileName(currentTypeDeclaration));
            }
            yield return new PageBreadcrumb(memberDeclaration.GetMemberName());
        }
    }
}