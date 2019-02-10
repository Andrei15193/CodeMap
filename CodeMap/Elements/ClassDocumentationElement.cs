using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a documented class declaration.</summary>
    public sealed class ClassDocumentationElement : TypeDocumentationElement
    {
        internal ClassDocumentationElement()
        {
        }

        /// <summary>Indicates whether the class is abstract.</summary>
        public bool IsAbstract { get; internal set; }

        /// <summary>Indicates whether the class is sealed.</summary>
        public bool IsSealed { get; internal set; }

        /// <summary>Indicates whether the class is static.</summary>
        public bool IsStatic { get; internal set; }

        /// <summary>The class generic parameters.</summary>
        public IReadOnlyList<TypeGenericParameterData> GenericParameters { get; internal set; }

        /// <summary>The base type.</summary>
        public TypeReferenceData BaseClass { get; internal set; }

        /// <summary>The implemented interfaces.</summary>
        public IReadOnlyCollection<TypeReferenceData> ImplementedInterfaces { get; internal set; }

        /// <summary>The declared constants.</summary>
        public IReadOnlyCollection<ConstantDocumentationElement> Constants { get; internal set; }

        /// <summary>The declared fields.</summary>
        public IReadOnlyCollection<FieldDocumentationElement> Fields { get; internal set; }

        /// <summary>The declared constructors.</summary>
        public IReadOnlyCollection<ConstructorDocumentationElement> Constructors { get; internal set; }

        /// <summary>The declared events.</summary>
        public IReadOnlyCollection<EventDocumentationElement> Events { get; internal set; }

        /// <summary>The declared properties.</summary>
        public IReadOnlyCollection<PropertyDocumentationElement> Properties { get; internal set; }

        /// <summary>The declared method.</summary>
        public IReadOnlyCollection<MethodDocumentationElement> Methods { get; internal set; }

        /// <summary>The declared nested enums.</summary>
        public IReadOnlyCollection<EnumDocumentationElement> NestedEnums { get; internal set; }

        /// <summary>The declared nested delegates.</summary>
        public IReadOnlyCollection<DelegateDocumentationElement> NestedDelegates { get; internal set; }

        /// <summary>The declared nested interfaces.</summary>
        public IReadOnlyCollection<InterfaceDocumentationElement> NestedInterfaces { get; internal set; }

        /// <summary>The declared nested classes.</summary>
        public IReadOnlyCollection<ClassDocumentationElement> NestedClasses { get; internal set; }

        /// <summary>The declared nested structs.</summary>
        public IReadOnlyCollection<StructDocumentationElement> NestedStructs { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
        {
            throw new NotImplementedException();
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>Determines whether the current <see cref="ClassDocumentationElement"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="ClassDocumentationElement"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
            => base.Equals(type)
            && GenericParameters.Count == (type.GetGenericArguments().Length - (type.DeclaringType?.GetGenericArguments().Length ?? 0));
    }
}