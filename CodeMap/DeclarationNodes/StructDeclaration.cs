﻿using System.Collections.Generic;
using CodeMap.ReferenceData;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented struct declaration.</summary>
    public sealed class StructDeclaration : TypeDeclaration
    {
        internal StructDeclaration(TypeReference typeReference)
            : base(typeReference)
        {
        }

        /// <summary>The struct generic parameters.</summary>
        public IReadOnlyList<GenericTypeParameterData> GenericParameters { get; internal set; }

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

        /// <summary>The declared nested records.</summary>
        public IReadOnlyCollection<RecordDeclaration> NestedRecords { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DeclarationNodeVisitor visitor)
            => visitor.VisitStruct(this);
    }
}