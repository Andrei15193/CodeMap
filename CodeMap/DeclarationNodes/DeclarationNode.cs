using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.DeclarationNodes
{
    /// <summary>A documentation element that is part of the documentation tree for an <see cref="System.Reflection.Assembly"/> and associated XML documentation.</summary>
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
                MemberDocumentationCollection membersDocumentation;
                using (var xmlDocumentationReader = xmlDocumentationFileInfo.OpenText())
                    membersDocumentation = Task.Run(() => new XmlDocumentationReader().ReadAsync(xmlDocumentationReader)).Result;

                return Create(assembly, membersDocumentation);
            }
            else
                return Create(assembly, new MemberDocumentationCollection(Enumerable.Empty<MemberDocumentation>()));
        }

        /// <summary>Creates an <see cref="AssemblyDeclaration"/> from the provided <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="Assembly"/> from which to create a <see cref="AssemblyDeclaration"/>.</param>
        /// <param name="membersDocumentation">
        /// A <see cref="MemberDocumentationCollection"/> containing written documentation to associated to
        /// <see cref="DeclarationNode"/>s representing assembly member declarations.
        /// </param>
        /// <returns>Returns an <see cref="AssemblyDeclaration"/> from the provided <paramref name="assembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> or <paramref name="membersDocumentation"/> are <c>null</c>.</exception>
        public static AssemblyDeclaration Create(Assembly assembly, MemberDocumentationCollection membersDocumentation)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            if (membersDocumentation == null)
                throw new ArgumentNullException(nameof(membersDocumentation));

            return new DeclarationNodeFactory(
                    new CanonicalNameResolver(new[] { assembly }.Concat(assembly.GetReferencedAssemblies().Select(Assembly.Load))),
                    membersDocumentation
                )
                .Create(assembly);
        }

        /// <summary>Creates an <see cref="AssemblyDeclaration"/> from the provided <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="Assembly"/> from which to create a <see cref="AssemblyDeclaration"/>.</param>
        /// <returns>Returns an <see cref="AssemblyDeclaration"/> from the provided <paramref name="assembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is <c>null</c>.</exception>
        /// <remarks>The associated XML documentation file is searched in the folder from where the assembly was loaded,
        /// if one is found then it is used, otherwise no written documentation is added to the result.</remarks>
        public static Task<AssemblyDeclaration> CreateAsync(Assembly assembly)
            => CreateAsync(assembly, CancellationToken.None);

        /// <summary>Creates an <see cref="AssemblyDeclaration"/> from the provided <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="Assembly"/> from which to create a <see cref="AssemblyDeclaration"/>.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns an <see cref="AssemblyDeclaration"/> from the provided <paramref name="assembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is <c>null</c>.</exception>
        /// <remarks>The associated XML documentation file is searched in the folder from where the assembly was loaded,
        /// if one is found then it is used, otherwise no written documentation is added to the result.</remarks>
        public static async Task<AssemblyDeclaration> CreateAsync(Assembly assembly, CancellationToken cancellationToken)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var xmlDocumentationFileInfo = new FileInfo(Path.ChangeExtension(assembly.Location, ".xml"));
            if (xmlDocumentationFileInfo.Exists)
                using (var xmlDocumentationReader = xmlDocumentationFileInfo.OpenText())
                    return await CreateAsync(assembly, xmlDocumentationReader, cancellationToken).ConfigureAwait(false);
            else
                return Create(assembly);
        }

        /// <summary>Creates an <see cref="AssemblyDeclaration"/> from the provided <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="Assembly"/> from which to create a <see cref="AssemblyDeclaration"/>.</param>
        /// <param name="xmlDocumentationReader">A <see cref="TextReader"/> for reading the associated XML documentation file.</param>
        /// <returns>Returns an <see cref="AssemblyDeclaration"/> from the provided <paramref name="assembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> or <paramref name="xmlDocumentationReader"/> are <c>null</c>.</exception>
        public static Task<AssemblyDeclaration> CreateAsync(Assembly assembly, TextReader xmlDocumentationReader)
            => CreateAsync(assembly, xmlDocumentationReader);

        /// <summary>Creates an <see cref="AssemblyDeclaration"/> from the provided <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="Assembly"/> from which to create a <see cref="AssemblyDeclaration"/>.</param>
        /// <param name="xmlDocumentationReader">A <see cref="TextReader"/> for reading the associated XML documentation file.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns an <see cref="AssemblyDeclaration"/> from the provided <paramref name="assembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> or <paramref name="xmlDocumentationReader"/> are <c>null</c>.</exception>
        public static async Task<AssemblyDeclaration> CreateAsync(Assembly assembly, TextReader xmlDocumentationReader, CancellationToken cancellationToken)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            if (xmlDocumentationReader == null)
                throw new ArgumentNullException(nameof(xmlDocumentationReader));

            return Create(
                assembly,
                await new XmlDocumentationReader().ReadAsync(xmlDocumentationReader, cancellationToken).ConfigureAwait(false)
            );
        }

        /// <summary>Creates a <see cref="TypeDeclaration"/> from the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> from which to create a <see cref="TypeDeclaration"/>.</param>
        /// <returns>Returns a <see cref="TypeDeclaration"/> from the provided <paramref name="type"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is <c>null</c>.</exception>
        /// <remarks>
        /// This method creates the entire <see cref="AssemblyDeclaration"/> and returns the <see cref="TypeDeclaration"/>
        /// for the provided <paramref name="type"/>.
        /// </remarks>
        public static TypeDeclaration Create(Type type)
            => Create(type, new MemberDocumentationCollection(Enumerable.Empty<MemberDocumentation>()));

        /// <summary>Creates a <see cref="TypeDeclaration"/> from the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> from which to create a <see cref="TypeDeclaration"/>.</param>
        /// <param name="membersDocumentation">
        /// A <see cref="MemberDocumentationCollection"/> containing written documentation to associated to
        /// <see cref="DeclarationNode"/>s representing assembly member declarations.
        /// </param>
        /// <returns>Returns a <see cref="TypeDeclaration"/> from the provided <paramref name="type"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> or <paramref name="membersDocumentation"/> are <c>null</c>.</exception>
        /// <remarks>
        /// This method creates the entire <see cref="AssemblyDeclaration"/> and returns the <see cref="TypeDeclaration"/>
        /// for the provided <paramref name="type"/>.
        /// </remarks>
        public static TypeDeclaration Create(Type type, MemberDocumentationCollection membersDocumentation)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (membersDocumentation == null)
                throw new ArgumentNullException(nameof(membersDocumentation));

            return Create(type.Assembly, membersDocumentation)
                .Namespaces
                .Where(@namespace => string.Equals(@namespace.Name, type.Namespace, StringComparison.OrdinalIgnoreCase))
                .SelectMany(
                    @namespace => @namespace
                        .Enums
                        .AsEnumerable<TypeDeclaration>()
                        .Concat(@namespace.Delegates)
                        .Concat(@namespace.Interfaces)
                        .Concat(@namespace.Classes.SelectMany(_GetWithNested))
                        .Concat(@namespace.Structs.SelectMany(_GetWithNested))
                )
                .FirstOrDefault(typeDocumentationElement => typeDocumentationElement == type);
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

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task AcceptAsync(DeclarationNodeVisitor visitor)
            => AcceptAsync(visitor, CancellationToken.None);

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public abstract Task AcceptAsync(DeclarationNodeVisitor visitor, CancellationToken cancellationToken);
    }
}