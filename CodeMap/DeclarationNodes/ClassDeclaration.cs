using CodeMap.ReferenceData;
using System;
using System.Collections.Generic;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented class declaration.</summary>
    public sealed class ClassDeclaration : TypeDeclaration
    {
        internal ClassDeclaration()
        {
        }

        /// <summary>Indicates whether the class is abstract.</summary>
        public bool IsAbstract { get; internal set; }

        /// <summary>Indicates whether the class is sealed.</summary>
        public bool IsSealed { get; internal set; }

        /// <summary>Indicates whether the class is static.</summary>
        public bool IsStatic { get; internal set; }

        /// <summary>The class generic parameters.</summary>
        public IReadOnlyList<GenericTypeParameterData> GenericParameters { get; internal set; }

        /// <summary>The base type.</summary>
        public TypeReference BaseClass { get; internal set; }

        /// <summary>The implemented interfaces.</summary>
        public IReadOnlyCollection<TypeReference> ImplementedInterfaces { get; internal set; }

        /// <summary>The declared members.</summary>
        public IReadOnlyCollection<MemberDeclaration> Members { get; internal set; }

        /// <summary>The declared constants.</summary>
        public IReadOnlyCollection<ConstantDeclaration> Constants { get; internal set; }

        /// <summary>The declared fields.</summary>
        public IReadOnlyCollection<FieldDeclaration> Fields { get; internal set; }

        /// <summary>The declared constructors.</summary>
        public IReadOnlyCollection<ConstructorDeclaration> Constructors { get; internal set; }

        /// <summary>The declared events.</summary>
        public IReadOnlyCollection<EventDeclaration> Events { get; internal set; }

        /// <summary>The declared properties.</summary>
        public IReadOnlyCollection<PropertyDeclaration> Properties { get; internal set; }

        /// <summary>The declared method.</summary>
        public IReadOnlyCollection<MethodDeclaration> Methods { get; internal set; }

        /// <summary>The declared nested types.</summary>
        public IReadOnlyCollection<TypeDeclaration> NestedTypes { get; internal set; }

        /// <summary>The declared nested enums.</summary>
        public IReadOnlyCollection<EnumDeclaration> NestedEnums { get; internal set; }

        /// <summary>The declared nested delegates.</summary>
        public IReadOnlyCollection<DelegateDeclaration> NestedDelegates { get; internal set; }

        /// <summary>The declared nested interfaces.</summary>
        public IReadOnlyCollection<InterfaceDeclaration> NestedInterfaces { get; internal set; }

        /// <summary>The declared nested classes.</summary>
        public IReadOnlyCollection<ClassDeclaration> NestedClasses { get; internal set; }

        /// <summary>The declared nested structs.</summary>
        public IReadOnlyCollection<StructDeclaration> NestedStructs { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DeclarationNodeVisitor visitor)
            => visitor.VisitClass(this);

        /// <summary>Determines whether the current <see cref="ClassDeclaration"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="ClassDeclaration"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
            => base.Equals(type)
            && GenericParameters.Count == (type.GetGenericArguments().Length - (type.DeclaringType?.GetGenericArguments().Length ?? 0));
    }
}