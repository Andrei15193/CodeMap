using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CodeMap.DocumentationElements;
using CodeMap.ReferenceData;

namespace CodeMap.DeclarationNodes
{
    /// <summary>A documentation element that is part of the documentation tree for an <see cref="Assembly"/> and associated XML documentation.</summary>
    public abstract class DeclarationNode
    {
        /// <summary>Creates an <see cref="AssemblyDeclaration"/> from the provided <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="Assembly"/> from which to create a <see cref="AssemblyDeclaration"/>.</param>
        /// <returns>Returns an <see cref="AssemblyDeclaration"/> from the provided <paramref name="assembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is <c>null</c>.</exception>
        public static AssemblyDeclaration Create(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var xmlDocumentationFileInfo = new FileInfo(Path.ChangeExtension(assembly.Location, ".xml"));
            if (xmlDocumentationFileInfo.Exists)
            {
                var canonicalNameResolver = new CanonicalNameResolver(_LoadAssemblies(assembly));

                MemberDocumentationCollection membersDocumentation;
                using (var xmlDocumentationReader = xmlDocumentationFileInfo.OpenText())
                    membersDocumentation = new XmlDocumentationReader(new MemberReferenceFactory(), canonicalNameResolver).Read(xmlDocumentationReader);

                return new DeclarationNodeFactory(canonicalNameResolver, membersDocumentation, new DeclarationFilter()).Create(assembly);
            }
            else
                return Create(assembly, new MemberDocumentationCollection(Enumerable.Empty<MemberDocumentation>()), new DeclarationFilter());
        }

        /// <summary>Creates an <see cref="AssemblyDeclaration"/> from the provided <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="Assembly"/> from which to create a <see cref="AssemblyDeclaration"/>.</param>
        /// <param name="declarationFilter">A <see cref="DeclarationFilter"/> used to select which <see cref="MemberInfo"/>s will be mapped to <see cref="DeclarationNode"/>s.</param>
        /// <returns>Returns an <see cref="AssemblyDeclaration"/> from the provided <paramref name="assembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is <c>null</c>.</exception>
        public static AssemblyDeclaration Create(Assembly assembly, DeclarationFilter declarationFilter)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var xmlDocumentationFileInfo = new FileInfo(Path.ChangeExtension(assembly.Location, ".xml"));
            if (xmlDocumentationFileInfo.Exists)
            {
                var canonicalNameResolver = new CanonicalNameResolver(_LoadAssemblies(assembly));

                MemberDocumentationCollection membersDocumentation;
                using (var xmlDocumentationReader = xmlDocumentationFileInfo.OpenText())
                    membersDocumentation = new XmlDocumentationReader(new MemberReferenceFactory(), canonicalNameResolver).Read(xmlDocumentationReader);

                return new DeclarationNodeFactory(canonicalNameResolver, membersDocumentation, declarationFilter).Create(assembly);
            }
            else
                return Create(assembly, new MemberDocumentationCollection(Enumerable.Empty<MemberDocumentation>()), declarationFilter);
        }

        /// <summary>Creates an <see cref="AssemblyDeclaration"/> from the provided <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="Assembly"/> from which to create a <see cref="AssemblyDeclaration"/>.</param>
        /// <param name="membersDocumentation">
        /// A <see cref="MemberDocumentationCollection"/> containing written documentation to associated to
        /// <see cref="DeclarationNode"/>s representing assembly member declarations.
        /// </param>
        /// <param name="declarationFilter">A <see cref="DeclarationFilter"/> used to select which <see cref="MemberInfo"/>s will be mapped to <see cref="DeclarationNode"/>s.</param>
        /// <returns>Returns an <see cref="AssemblyDeclaration"/> from the provided <paramref name="assembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> or <paramref name="membersDocumentation"/> are <c>null</c>.</exception>
        public static AssemblyDeclaration Create(Assembly assembly, MemberDocumentationCollection membersDocumentation, DeclarationFilter declarationFilter)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            if (membersDocumentation == null)
                throw new ArgumentNullException(nameof(membersDocumentation));

            return new DeclarationNodeFactory(
                new CanonicalNameResolver(_LoadAssemblies(assembly)),
                membersDocumentation,
                declarationFilter
            ).Create(assembly);
        }

        private static IEnumerable<Assembly> _LoadAssemblies(Assembly assembly)
        {
            var assemblies = new HashSet<Assembly>();
            var assembliesToVisit = new Queue<Assembly>();
            assembliesToVisit.Enqueue(assembly);
            do
            {
                var currentAssembly = assembliesToVisit.Dequeue();
                if (assemblies.Add(currentAssembly))
                    foreach (var referencedAssembly in currentAssembly.GetReferencedAssemblies())
                        assembliesToVisit.Enqueue(Assembly.Load(referencedAssembly));
            } while (assembliesToVisit.Count > 0);
            return assemblies;
        }

        private static IEnumerable<TypeDeclaration> _GetWithNested(ClassDeclaration classDocumentationElement)
            => new[] { classDocumentationElement }
                .Concat(classDocumentationElement
                    .NestedEnums
                    .AsEnumerable<TypeDeclaration>()
                    .Concat(classDocumentationElement.NestedDelegates)
                    .Concat(classDocumentationElement.NestedInterfaces)
                    .Concat(classDocumentationElement.NestedClasses.SelectMany(_GetWithNested))
                    .Concat(classDocumentationElement.NestedStructs.SelectMany(_GetWithNested))
                );

        private static IEnumerable<TypeDeclaration> _GetWithNested(StructDeclaration structDocumentationElement)
            => new[] { structDocumentationElement }
                .Concat(structDocumentationElement
                    .NestedEnums
                    .AsEnumerable<TypeDeclaration>()
                    .Concat(structDocumentationElement.NestedDelegates)
                    .Concat(structDocumentationElement.NestedInterfaces)
                    .Concat(structDocumentationElement.NestedClasses.SelectMany(_GetWithNested))
                    .Concat(structDocumentationElement.NestedStructs.SelectMany(_GetWithNested))
                );

        internal DeclarationNode()
        {
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        public abstract void Accept(DeclarationNodeVisitor visitor);
    }
}