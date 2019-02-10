#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
using System;
using System.Collections.Generic;

namespace CodeMap.Elements
{
    /// <summary>Represents a documented type.</summary>
    public abstract class TypeDocumentationElement : DocumentationElement, IEquatable<Type>
    {
        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="type"/> are equal.</summary>
        /// <param name="typeDocumentation">The <see cref="TypeDocumentationElement"/> to compare.</param>
        /// <param name="type">The <see cref="Type"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(TypeDocumentationElement typeDocumentation, Type type)
            => Equals(typeDocumentation, type);

        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="type"/> are not equal.</summary>
        /// <param name="typeDocumentation">The <see cref="TypeDocumentationElement"/> to compare.</param>
        /// <param name="type">The <see cref="Type"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(TypeDocumentationElement typeDocumentation, Type type)
            => !Equals(typeDocumentation, type);

        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="type"/> are equal.</summary>
        /// <param name="type">The <see cref="Type"/> to compare.</param>
        /// <param name="typeDocumentation">The <see cref="TypeDocumentationElement"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(Type type, TypeDocumentationElement typeDocumentation)
            => Equals(typeDocumentation, type);

        /// <summary>Determines whether the provided <paramref name="typeDocumentation"/> and <paramref name="type"/> are not equal.</summary>
        /// <param name="type">The <see cref="Type"/> to compare.</param>
        /// <param name="typeDocumentation">The <see cref="TypeDocumentationElement"/> to compare.</param>
        /// <returns>Returns <c>true</c> if the two provided instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(Type type, TypeDocumentationElement typeDocumentation)
            => !Equals(typeDocumentation, type);

        internal TypeDocumentationElement()
        {
        }

        /// <summary>The type name.</summary>
        public string Name { get; internal set; }

        /// <summary>The declaring namespace.</summary>
        public NamespaceDocumentationElement Namespace { get; internal set; }

        /// <summary>The declaring assembly.</summary>
        public AssemblyDocumentationElement Assembly
            => Namespace?.Assembly;

        /// <summary>The delcaring type.</summary>
        public TypeDocumentationElement DeclaringType { get; internal set; }

        /// <summary>The type access modifier.</summary>
        public AccessModifier AccessModifier { get; internal set; }

        /// <summary>The attributes decorating the type.</summary>
        public IReadOnlyCollection<AttributeData> Attributes { get; internal set; }

        /// <summary>The type summary.</summary>
        new public SummaryDocumentationElement Summary { get; internal set; }

        /// <summary>The type remarks.</summary>
        new public RemarksDocumentationElement Remarks { get; internal set; }

        /// <summary>The type examples.</summary>
        public IReadOnlyList<ExampleDocumentationElement> Examples { get; internal set; }

        /// <summary>The type related members.</summary>
        public IReadOnlyList<MemberReferenceDocumentationElement> RelatedMembers { get; internal set; }

        /// <summary>Determines whether the current <see cref="TypeDocumentationElement"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="TypeDocumentationElement"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public virtual bool Equals(Type type)
        {
            if (type == null || type.IsPointer || type.IsArray || type.IsByRef || type.IsGenericParameter || type.IsConstructedGenericType)
                return false;

            var backTickIndex = type.Name.LastIndexOf('`');
            return
                Namespace is GlobalNamespaceDocumentationElement
                    ? string.IsNullOrWhiteSpace(type.Namespace)
                    : string.Equals(Namespace.Name, type.Namespace, StringComparison.OrdinalIgnoreCase)
                && string.Equals(Name, (backTickIndex >= 0 ? type.Name.Substring(0, backTickIndex) : type.Name), StringComparison.OrdinalIgnoreCase)
                && Assembly == type.Assembly;
        }

        /// <summary>Determines whether the current <see cref="TypeDocumentationElement"/> is equal to the provided <paramref name="obj"/>.</summary>
        /// <param name="obj">The <see cref="object"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="TypeDocumentationElement"/> references the provided <paramref name="obj"/>; <c>false</c> otherwise.</returns>
        /// <remarks>
        /// If the provided <paramref name="obj"/> is a <see cref="Type"/> instance then the comparison is done by comparing members and
        /// determining whether the current instance actually maps to the provided <see cref="Type"/>. Otherwise the equality is determined
        /// by comparing references.
        /// </remarks>
        public override bool Equals(object obj)
        {
            if (obj is Type type)
                return Equals(type);
            else
                return base.Equals(obj);
        }
    }
}