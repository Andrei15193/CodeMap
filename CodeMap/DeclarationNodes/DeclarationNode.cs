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
    public abstract class DeclarationNode : IEquatable<MemberReference>, IEquatable<MemberInfo>, IEquatable<Assembly>, IEquatable<AssemblyName>
    {
        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="memberReference"/> are equal.</summary>
        /// <param name="typeDocumentation">The <see cref="TypeDeclaration"/> to compare.</param>
        /// <param name="memberReference">The <see cref="MemberInfo"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(DeclarationNode typeDocumentation, MemberReference memberReference)
            => Equals(typeDocumentation, memberReference);

        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="memberReference"/> are not equal.</summary>
        /// <param name="typeDocumentation">The <see cref="DeclarationNode"/> to compare.</param>
        /// <param name="memberReference">The <see cref="MemberReference"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(DeclarationNode typeDocumentation, MemberReference memberReference)
            => !Equals(typeDocumentation, memberReference);

        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="memberReference"/> are equal.</summary>
        /// <param name="typeDocumentation">The <see cref="TypeDeclaration"/> to compare.</param>
        /// <param name="memberReference">The <see cref="MemberReference"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(MemberReference memberReference, DeclarationNode typeDocumentation)
            => Equals(typeDocumentation, memberReference);

        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="memberReference"/> are not equal.</summary>
        /// <param name="typeDocumentation">The <see cref="DeclarationNode"/> to compare.</param>
        /// <param name="memberReference">The <see cref="MemberReference"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(MemberReference memberReference, DeclarationNode typeDocumentation)
            => !Equals(typeDocumentation, memberReference);

        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="memberInfo"/> are equal.</summary>
        /// <param name="typeDocumentation">The <see cref="TypeDeclaration"/> to compare.</param>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(DeclarationNode typeDocumentation, MemberInfo memberInfo)
            => Equals(typeDocumentation, memberInfo);

        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="memberInfo"/> are not equal.</summary>
        /// <param name="typeDocumentation">The <see cref="DeclarationNode"/> to compare.</param>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(DeclarationNode typeDocumentation, MemberInfo memberInfo)
            => !Equals(typeDocumentation, memberInfo);

        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="memberInfo"/> are equal.</summary>
        /// <param name="typeDocumentation">The <see cref="TypeDeclaration"/> to compare.</param>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(MemberInfo memberInfo, DeclarationNode typeDocumentation)
            => Equals(typeDocumentation, memberInfo);

        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="memberInfo"/> are not equal.</summary>
        /// <param name="typeDocumentation">The <see cref="DeclarationNode"/> to compare.</param>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(MemberInfo memberInfo, DeclarationNode typeDocumentation)
            => !Equals(typeDocumentation, memberInfo);

        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="assembly"/> are equal.</summary>
        /// <param name="typeDocumentation">The <see cref="TypeDeclaration"/> to compare.</param>
        /// <param name="assembly">The <see cref="Assembly"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(DeclarationNode typeDocumentation, Assembly assembly)
            => Equals(typeDocumentation, assembly);

        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="assembly"/> are not equal.</summary>
        /// <param name="typeDocumentation">The <see cref="DeclarationNode"/> to compare.</param>
        /// <param name="assembly">The <see cref="Assembly"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(DeclarationNode typeDocumentation, Assembly assembly)
            => !Equals(typeDocumentation, assembly);

        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="assembly"/> are equal.</summary>
        /// <param name="assembly">The <see cref="Assembly"/> to compare.</param>
        /// <param name="typeDocumentation">The <see cref="TypeDeclaration"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(Assembly assembly, DeclarationNode typeDocumentation)
            => Equals(typeDocumentation, assembly);

        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="assembly"/> are not equal.</summary>
        /// <param name="assembly">The <see cref="Assembly"/> to compare.</param>
        /// <param name="typeDocumentation">The <see cref="DeclarationNode"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(Assembly assembly, DeclarationNode typeDocumentation)
            => !Equals(typeDocumentation, assembly);

        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="assemblyName"/> are equal.</summary>
        /// <param name="typeDocumentation">The <see cref="TypeDeclaration"/> to compare.</param>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(DeclarationNode typeDocumentation, AssemblyName assemblyName)
            => Equals(typeDocumentation, assemblyName);

        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="assemblyName"/> are not equal.</summary>
        /// <param name="typeDocumentation">The <see cref="DeclarationNode"/> to compare.</param>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(DeclarationNode typeDocumentation, AssemblyName assemblyName)
            => !Equals(typeDocumentation, assemblyName);

        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="assemblyName"/> are equal.</summary>
        /// <param name="typeDocumentation">The <see cref="TypeDeclaration"/> to compare.</param>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(AssemblyName assemblyName, DeclarationNode typeDocumentation)
            => Equals(typeDocumentation, assemblyName);

        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="assemblyName"/> are not equal.</summary>
        /// <param name="typeDocumentation">The <see cref="DeclarationNode"/> to compare.</param>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(AssemblyName assemblyName, DeclarationNode typeDocumentation)
            => !Equals(typeDocumentation, assemblyName);

        /// <summary>Converts the provided <paramref name="declarationNode"/> to a <see cref="MemberReference"/>.</summary>
        /// <param name="declarationNode">The <see cref="DeclarationNode"/> to convert.</param>
        public static explicit operator MemberReference(DeclarationNode declarationNode)
            => declarationNode.AsMeberReference();

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


        private readonly MemberReference _memberReference;

        internal DeclarationNode(MemberReference memberReference)
            => _memberReference = memberReference;

        /// <summary>Gets the current instance as a <see cref="MemberReference"/>.</summary>
        /// <returns>Returns the <see cref="MemberReference"/> pointing towards this declaration.</returns>
        /// <remarks>
        /// Member references are designed to contain the necessary minimum information for creating links in documentation files. This
        /// streamlines the generation of links as <see cref="DeclarationNode"/>s themselves can be treated as <see cref="MemberReference"/>s
        /// when generating them.
        /// </remarks>
        public MemberReference AsMeberReference()
            => _memberReference;

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        public abstract void Accept(DeclarationNodeVisitor visitor);

        /// <summary>Determines whether the current <see cref="TypeDeclaration"/> is equal to the provided <paramref name="obj"/>.</summary>
        /// <param name="obj">The <see cref="object"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="TypeDeclaration"/> references the provided <paramref name="obj"/>; <c>false</c> otherwise.</returns>
        /// <remarks>
        /// If the provided <paramref name="obj"/> is a <see cref="Type"/> instance then the comparison is done by comparing members and
        /// determining whether the current instance actually maps to the provided <see cref="Type"/>. Otherwise the equality is determined
        /// by comparing references.
        /// </remarks>
        public sealed override bool Equals(object obj)
            => obj switch
            {
                MemberReference memberReference => Equals(memberReference),
                MemberInfo memberInfo => Equals(memberInfo),
                Assembly assembly => Equals(assembly),
                AssemblyName assemblyName => Equals(assemblyName),
                _ => base.Equals(obj)
            };

        /// <summary>Gets the hash code for the current instance.</summary>
        /// <returns>Returns the hash code for the current instance.</returns>
        public sealed override int GetHashCode()
            => base.GetHashCode();

        /// <summary>Determines whether the current <see cref="DeclarationNode"/> is equal to the provided <paramref name="memberReference"/>.</summary>
        /// <param name="memberReference">The <see cref="MemberReference"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="DeclarationNode"/> references the provided <paramref name="memberReference"/>; <c>false</c> otherwise.</returns>
        public bool Equals(MemberReference memberReference)
            => _memberReference.Equals(memberReference);

        /// <summary>Determines whether the current <see cref="DeclarationNode"/> is equal to the provided <paramref name="memberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="DeclarationNode"/> references the provided <paramref name="memberInfo"/>; <c>false</c> otherwise.</returns>
        public bool Equals(MemberInfo memberInfo)
            => _memberReference.Equals(memberInfo);

        /// <summary>Determines whether the current <see cref="DeclarationNode"/> is equal to the provided <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="Assembly"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="DeclarationNode"/> references the provided <paramref name="assembly"/>; <c>false</c> otherwise.</returns>
        public bool Equals(Assembly assembly)
            => _memberReference.Equals(assembly);

        /// <summary>Determines whether the current <see cref="DeclarationNode"/> is equal to the provided <paramref name="assemblyName"/>.</summary>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="DeclarationNode"/> references the provided <paramref name="assemblyName"/>; <c>false</c> otherwise.</returns>
        public bool Equals(AssemblyName assemblyName)
            => _memberReference.Equals(assemblyName);
    }
}