using System;
using System.Collections.Generic;
using System.Reflection;
using CodeMap.DocumentationElements;
using CodeMap.ReferenceData;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented assembly.</summary>
    public class AssemblyDeclaration : DeclarationNode, IEquatable<Assembly>, IEquatable<AssemblyName>, IEquatable<AssemblyReference>
    {
        /// <summary>Determines whether the provided <paramref name="assemblyDocumentationElement"/> and <paramref name="assembly"/> are equal.</summary>
        /// <param name="assemblyDocumentationElement">The <see cref="AssemblyDeclaration"/> to compare.</param>
        /// <param name="assembly">The <see cref="Assembly"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(AssemblyDeclaration assemblyDocumentationElement, Assembly assembly)
            => Equals(assemblyDocumentationElement, assembly);

        /// <summary>Determines whether the provided <paramref name="assemblyDocumentationElement"/> and <paramref name="assembly"/> are not equal.</summary>
        /// <param name="assemblyDocumentationElement">The <see cref="AssemblyDeclaration"/> to compare.</param>
        /// <param name="assembly">The <see cref="Assembly"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(AssemblyDeclaration assemblyDocumentationElement, Assembly assembly)
            => !Equals(assemblyDocumentationElement, assembly);

        /// <summary>Determines whether the provided <paramref name="assemblyDocumentationElement"/> and <paramref name="assembly"/> are equal.</summary>
        /// <param name="assembly">The <see cref="Assembly"/> to compare.</param>
        /// <param name="assemblyDocumentationElement">The <see cref="AssemblyDeclaration"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(Assembly assembly, AssemblyDeclaration assemblyDocumentationElement)
            => Equals(assemblyDocumentationElement, assembly);

        /// <summary>Determines whether the provided <paramref name="assemblyDocumentationElement"/> and <paramref name="assembly"/> are equal.</summary>
        /// <param name="assembly">The <see cref="Assembly"/> to compare.</param>
        /// <param name="assemblyDocumentationElement">The <see cref="AssemblyDeclaration"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(Assembly assembly, AssemblyDeclaration assemblyDocumentationElement)
            => !Equals(assemblyDocumentationElement, assembly);

        /// <summary>Determines whether the provided <paramref name="assemblyDocumentationElement"/> and <paramref name="assemblyName"/> are equal.</summary>
        /// <param name="assemblyDocumentationElement">The <see cref="AssemblyDeclaration"/> to compare.</param>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(AssemblyDeclaration assemblyDocumentationElement, AssemblyName assemblyName)
            => Equals(assemblyDocumentationElement, assemblyName);

        /// <summary>Determines whether the provided <paramref name="assemblyDocumentationElement"/> and <paramref name="assemblyName"/> are not equal.</summary>
        /// <param name="assemblyDocumentationElement">The <see cref="AssemblyDeclaration"/> to compare.</param>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(AssemblyDeclaration assemblyDocumentationElement, AssemblyName assemblyName)
            => !Equals(assemblyDocumentationElement, assemblyName);

        /// <summary>Determines whether the provided <paramref name="assemblyDocumentationElement"/> and <paramref name="assemblyName"/> are equal.</summary>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> to compare.</param>
        /// <param name="assemblyDocumentationElement">The <see cref="AssemblyDeclaration"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(AssemblyName assemblyName, AssemblyDeclaration assemblyDocumentationElement)
            => Equals(assemblyDocumentationElement, assemblyName);

        /// <summary>Determines whether the provided <paramref name="assemblyDocumentationElement"/> and <paramref name="assemblyName"/> are equal.</summary>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> to compare.</param>
        /// <param name="assemblyDocumentationElement">The <see cref="AssemblyDeclaration"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(AssemblyName assemblyName, AssemblyDeclaration assemblyDocumentationElement)
            => !Equals(assemblyDocumentationElement, assemblyName);

        /// <summary>Determines whether the provided <paramref name="assemblyDocumentationElement"/> and <paramref name="assemblyReference"/> are equal.</summary>
        /// <param name="assemblyDocumentationElement">The <see cref="AssemblyDeclaration"/> to compare.</param>
        /// <param name="assemblyReference">The <see cref="AssemblyReference"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(AssemblyDeclaration assemblyDocumentationElement, AssemblyReference assemblyReference)
            => Equals(assemblyDocumentationElement, assemblyReference);

        /// <summary>Determines whether the provided <paramref name="assemblyDocumentationElement"/> and <paramref name="assemblyReference"/> are not equal.</summary>
        /// <param name="assemblyDocumentationElement">The <see cref="AssemblyDeclaration"/> to compare.</param>
        /// <param name="assemblyReference">The <see cref="AssemblyReference"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(AssemblyDeclaration assemblyDocumentationElement, AssemblyReference assemblyReference)
            => !Equals(assemblyDocumentationElement, assemblyReference);

        /// <summary>Determines whether the provided <paramref name="assemblyDocumentationElement"/> and <paramref name="assemblyReference"/> are equal.</summary>
        /// <param name="assemblyReference">The <see cref="AssemblyReference"/> to compare.</param>
        /// <param name="assemblyDocumentationElement">The <see cref="AssemblyDeclaration"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(AssemblyReference assemblyReference, AssemblyDeclaration assemblyDocumentationElement)
            => Equals(assemblyDocumentationElement, assemblyReference);

        /// <summary>Determines whether the provided <paramref name="assemblyDocumentationElement"/> and <paramref name="assemblyReference"/> are equal.</summary>
        /// <param name="assemblyReference">The <see cref="AssemblyReference"/> to compare.</param>
        /// <param name="assemblyDocumentationElement">The <see cref="AssemblyDeclaration"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(AssemblyReference assemblyReference, AssemblyDeclaration assemblyDocumentationElement)
            => !Equals(assemblyDocumentationElement, assemblyReference);

        internal AssemblyDeclaration()
        {
        }

        /// <summary>The assembly name.</summary>
        public string Name { get; internal set; }

        /// <summary>The assembly version.</summary>
        public Version Version { get; internal set; }

        /// <summary>The assembly culture, if it is a satelite one.</summary>
        public string Culture { get; internal set; }

        /// <summary>The assembly public key token, if it is a signed one.</summary>
        public string PublicKeyToken { get; internal set; }

        /// <summary>The assemblies that the current one depends on.</summary>
        public IReadOnlyCollection<AssemblyReference> Dependencies { get; internal set; }

        /// <summary>The assembly attributes.</summary>
        public IReadOnlyCollection<AttributeData> Attributes { get; internal set; }

        /// <summary>The declared namespaces.</summary>
        public IReadOnlyCollection<NamespaceDeclaration> Namespaces { get; internal set; }

        /// <summary>The assembly summary.</summary>
        public SummaryDocumentationElement Summary { get; set; }

        /// <summary>The assembly remarks.</summary>
        public RemarksDocumentationElement Remarks { get; set; }

        /// <summary>The assembly examples.</summary>
        public IReadOnlyList<ExampleDocumentationElement> Examples { get; set; }

        /// <summary>The assembly related members.</summary>
        public IReadOnlyList<MemberReferenceDocumentationElement> RelatedMembers { get; set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DeclarationNodeVisitor visitor)
            => visitor.VisitAssembly(this);

        /// <summary>Determines whether the current <see cref="AssemblyDeclaration"/> is equal to the provided <paramref name="assembly"/>.</summary>
        /// <param name="assembly">The <see cref="Assembly"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="AssemblyDeclaration"/> references the provided <paramref name="assembly"/>; <c>false</c> otherwise.</returns>
        public bool Equals(Assembly assembly)
            => Equals(assembly?.GetName());

        /// <summary>Determines whether the current <see cref="AssemblyDeclaration"/> is equal to the provided <paramref name="assemblyName"/>.</summary>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="AssemblyDeclaration"/> references the provided <paramref name="assemblyName"/>; <c>false</c> otherwise.</returns>
        public bool Equals(AssemblyName assemblyName)
            => assemblyName != null
            && string.Equals(Name, assemblyName.Name, StringComparison.OrdinalIgnoreCase)
            && Version == assemblyName.Version
            && string.Equals(Culture, assemblyName.CultureName, StringComparison.OrdinalIgnoreCase)
            && string.Equals(PublicKeyToken, assemblyName.GetPublicKeyToken().ToBase16String(), StringComparison.OrdinalIgnoreCase);

        /// <summary>Determines whether the current <see cref="AssemblyDeclaration"/> is equal to the provided <paramref name="assemblyReference"/>.</summary>
        /// <param name="assemblyReference">The <see cref="AssemblyReference"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="AssemblyDeclaration"/> references the provided <paramref name="assemblyReference"/>; <c>false</c> otherwise.</returns>
        public bool Equals(AssemblyReference assemblyReference)
            => assemblyReference != null
            && string.Equals(Name, assemblyReference.Name, StringComparison.OrdinalIgnoreCase)
            && Version == assemblyReference.Version
            && string.Equals(Culture, assemblyReference.Culture, StringComparison.OrdinalIgnoreCase)
            && string.Equals(PublicKeyToken, assemblyReference.PublicKeyToken, StringComparison.OrdinalIgnoreCase);

        /// <summary>Determines whether the current <see cref="AssemblyDeclaration"/> is equal to the provided <paramref name="obj"/>.</summary>
        /// <param name="obj">The <see cref="object"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="AssemblyDeclaration"/> references the provided <paramref name="obj"/>; <c>false</c> otherwise.</returns>
        /// <remarks>
        /// If the provided <paramref name="obj"/> is an <see cref="Assembly"/>, <see cref="AssemblyName"/>, <see cref="AssemblyReference"/>
        /// instance then the comparison is done by comparing members and determining whether the current instance actually maps to the provided
        /// <see cref="Assembly"/>, <see cref="AssemblyName"/> or <see cref="AssemblyReference"/>. Otherwise the equality is determined by comparing
        /// references.
        /// </remarks>
        public override bool Equals(object obj)
        {
            if (obj is Assembly assembly)
                return Equals(assembly);
            else if (obj is AssemblyName assemblyName)
                return Equals(assemblyName);
            else if (obj is AssemblyReference assemblyReference)
                return Equals(assemblyReference);
            else
                return base.Equals(obj);
        }

        /// <summary>Computes the hash code for the current instance.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
            => new
            {
                Name = Name.ToLowerInvariant(),
                Version = $"{Version.Major}.{Version.Minor}.{Version.Build}.{Version.Revision}",
                Culture = Culture.ToLowerInvariant(),
                PublicKeyToken = PublicKeyToken.ToLowerInvariant()
            }.GetHashCode();
    }
}