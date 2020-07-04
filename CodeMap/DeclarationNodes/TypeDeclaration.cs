using System;
using System.Collections.Generic;
using CodeMap.DocumentationElements;
using CodeMap.ReferenceData;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented type.</summary>
    public abstract class TypeDeclaration : DeclarationNode, IEquatable<Type>, IEquatable<TypeReference>
    {
        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="type"/> are equal.</summary>
        /// <param name="typeDocumentation">The <see cref="TypeDeclaration"/> to compare.</param>
        /// <param name="type">The <see cref="Type"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(TypeDeclaration typeDocumentation, Type type)
            => Equals(typeDocumentation, type);

        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="type"/> are not equal.</summary>
        /// <param name="typeDocumentation">The <see cref="TypeDeclaration"/> to compare.</param>
        /// <param name="type">The <see cref="Type"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(TypeDeclaration typeDocumentation, Type type)
            => !Equals(typeDocumentation, type);

        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="type"/> are equal.</summary>
        /// <param name="type">The <see cref="Type"/> to compare.</param>
        /// <param name="typeDocumentation">The <see cref="TypeDeclaration"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(Type type, TypeDeclaration typeDocumentation)
            => Equals(typeDocumentation, type);

        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="type"/> are not equal.</summary>
        /// <param name="type">The <see cref="Type"/> to compare.</param>
        /// <param name="typeDocumentation">The <see cref="TypeDeclaration"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(Type type, TypeDeclaration typeDocumentation)
            => !Equals(typeDocumentation, type);

        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="typeReference"/> are equal.</summary>
        /// <param name="typeDocumentation">The <see cref="TypeDeclaration"/> to compare.</param>
        /// <param name="typeReference">The <see cref="TypeReference"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(TypeDeclaration typeDocumentation, TypeReference typeReference)
            => Equals(typeDocumentation, typeReference);

        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="typeReference"/> are not equal.</summary>
        /// <param name="typeDocumentation">The <see cref="TypeDeclaration"/> to compare.</param>
        /// <param name="typeReference">The <see cref="TypeReference"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(TypeDeclaration typeDocumentation, TypeReference typeReference)
            => !Equals(typeDocumentation, typeReference);

        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="typeReference"/> are equal.</summary>
        /// <param name="typeReference">The <see cref="TypeReference"/> to compare.</param>
        /// <param name="typeDocumentation">The <see cref="TypeDeclaration"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(TypeReference typeReference, TypeDeclaration typeDocumentation)
            => Equals(typeDocumentation, typeReference);

        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="typeReference"/> are not equal.</summary>
        /// <param name="typeReference">The <see cref="TypeReference"/> to compare.</param>
        /// <param name="typeDocumentation">The <see cref="TypeDeclaration"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(TypeReference typeReference, TypeDeclaration typeDocumentation)
            => !Equals(typeDocumentation, typeReference);

        internal TypeDeclaration()
        {
        }

        /// <summary>The type name.</summary>
        public string Name { get; internal set; }

        /// <summary>The declaring namespace.</summary>
        public NamespaceDeclaration Namespace { get; internal set; }

        /// <summary>The declaring assembly.</summary>
        public AssemblyDeclaration Assembly
            => Namespace?.Assembly;

        /// <summary>The delcaring type.</summary>
        public TypeDeclaration DeclaringType { get; internal set; }

        /// <summary>The type access modifier.</summary>
        public AccessModifier AccessModifier { get; internal set; }

        /// <summary>The attributes decorating the type.</summary>
        public IReadOnlyCollection<AttributeData> Attributes { get; internal set; }

        /// <summary>The type summary.</summary>
        public SummaryDocumentationElement Summary { get; internal set; }

        /// <summary>The type remarks.</summary>
        public RemarksDocumentationElement Remarks { get; internal set; }

        /// <summary>The type examples.</summary>
        public IReadOnlyList<ExampleDocumentationElement> Examples { get; internal set; }

        /// <summary>The type related members.</summary>
        public IReadOnlyList<MemberReferenceDocumentationElement> RelatedMembers { get; internal set; }

        /// <summary>Determines whether the current <see cref="TypeDeclaration"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="TypeDeclaration"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public virtual bool Equals(Type type)
        {
            if (type == null || type.IsPointer || type.IsArray || type.IsByRef || type.IsGenericParameter || type.IsConstructedGenericType)
                return false;

            var backTickIndex = type.Name.LastIndexOf('`');
            return
                Namespace is GlobalNamespaceDeclaration
                    ? string.IsNullOrWhiteSpace(type.Namespace)
                    : string.Equals(Namespace.Name, type.Namespace, StringComparison.OrdinalIgnoreCase)
                && string.Equals(Name, (backTickIndex >= 0 ? type.Name.Substring(0, backTickIndex) : type.Name), StringComparison.OrdinalIgnoreCase)
                && DeclaringType == type.DeclaringType
                && Assembly == type.Assembly;
        }

        /// <summary>Determines whether the current <see cref="TypeDeclaration"/> is equal to the provided <paramref name="typeReference"/>.</summary>
        /// <param name="typeReference">The <see cref="TypeReference"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="TypeDeclaration"/> references the provided <paramref name="typeReference"/>; <c>false</c> otherwise.</returns>
        public virtual bool Equals(TypeReference typeReference)
            => typeReference != null
            && Namespace is GlobalNamespaceDeclaration
                ? string.IsNullOrWhiteSpace(typeReference.Namespace)
                : string.Equals(Namespace.Name, typeReference.Namespace, StringComparison.OrdinalIgnoreCase)
            && string.Equals(Name, typeReference.Name, StringComparison.OrdinalIgnoreCase)
            && DeclaringType == typeReference.DeclaringType
            && Assembly == typeReference.Assembly;

        /// <summary>Determines whether the current <see cref="TypeDeclaration"/> is equal to the provided <paramref name="obj"/>.</summary>
        /// <param name="obj">The <see cref="object"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="TypeDeclaration"/> references the provided <paramref name="obj"/>; <c>false</c> otherwise.</returns>
        /// <remarks>
        /// If the provided <paramref name="obj"/> is a <see cref="Type"/> instance then the comparison is done by comparing members and
        /// determining whether the current instance actually maps to the provided <see cref="Type"/>. Otherwise the equality is determined
        /// by comparing references.
        /// </remarks>
        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case Type type:
                    return Equals(type);

                case TypeReference typeReference:
                    return Equals(typeReference);

                default:
                    return base.Equals(obj);
            }
        }

        /// <summary>Computes the hash code for the current instance.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
            => new
            {
                Namespace = Namespace.Name.ToLowerInvariant(),
                Name = Name.ToLowerInvariant(),
                AssemblyHashCode = Assembly.GetHashCode()
            }.GetHashCode();
    }
}