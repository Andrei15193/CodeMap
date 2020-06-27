using CodeMap.DeclarationNodes;
using System;
using System.Collections.Generic;

namespace CodeMap.Documentation
{
    public class PageContext
    {
        public PageContext(DeclarationNode declarationNode)
            => DeclarationNode = declarationNode;

        public DeclarationNode DeclarationNode { get; }

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
                _ => throw new ArgumentException($"DeclarationNode type not handled: '{DeclarationNode.GetType().Name}'.")
            };

        private IEnumerable<PageBreadcrumb> _GetBreadcrumbs(AssemblyDeclaration assembly)
        {
            yield return new PageBreadcrumb(assembly.Name);
        }

        private IEnumerable<PageBreadcrumb> _GetBreadcrumbs(NamespaceDeclaration @namespace)
        {
            yield return new PageBreadcrumb(@namespace.Assembly.Name, "Index.html");
            yield return new PageBreadcrumb(@namespace.Name);
        }

        private IEnumerable<PageBreadcrumb> _GetBreadcrumbs(TypeDeclaration typeDeclaration)
        {
            var typeDeclarations = new Stack<TypeDeclaration>();
            while (typeDeclaration != null)
            {
                typeDeclarations.Push(typeDeclaration);
                typeDeclaration = typeDeclaration.DeclaringType;
            }

            yield return new PageBreadcrumb(typeDeclarations.Peek().Assembly.Name, "Index.html");
            yield return new PageBreadcrumb(typeDeclarations.Peek().Namespace.Name, $"{typeDeclarations.Peek().Namespace.Name}.html");
            while (typeDeclarations.Count > 1)
            {
                var currentTypeDeclaration = typeDeclarations.Pop();
                yield return new PageBreadcrumb(currentTypeDeclaration.Name, $"{currentTypeDeclaration.GetMemberFullName()}.html");
            }
            yield return new PageBreadcrumb(typeDeclarations.Peek().Name);
        }
    }

    public class PageContext<TDeclarationNode> : PageContext
        where TDeclarationNode : DeclarationNode
    {
        public PageContext(TDeclarationNode declarationNode)
            : base(declarationNode)
        {
        }

        public new TDeclarationNode DeclarationNode
            => (TDeclarationNode)base.DeclarationNode;
    }
}