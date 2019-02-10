using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a documented struct declaration.</summary>
    public sealed class StructDocumentationElement : TypeDocumentationElement
    {
        internal StructDocumentationElement()
        {
        }

        /// <summary>The struct generic parameters.</summary>
        public IReadOnlyList<TypeGenericParameterData> GenericParameters { get; internal set; }

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
            visitor.VisitStruct(this);
            foreach (var constant in Constants)
                constant.Accept(visitor);
            foreach (var field in Fields)
                field.Accept(visitor);
            foreach (var constructor in Constructors)
                constructor.Accept(visitor);
            foreach (var @event in Events)
                @event.Accept(visitor);
            foreach (var property in Properties)
                property.Accept(visitor);
            foreach (var methods in Methods)
                methods.Accept(visitor);

            foreach (var nestedEnum in NestedEnums)
                nestedEnum.Accept(visitor);
            foreach (var nestedDelegate in NestedDelegates)
                nestedDelegate.Accept(visitor);
            foreach (var nestedInterface in NestedInterfaces)
                nestedInterface.Accept(visitor);
            foreach (var nestedClass in NestedClasses)
                nestedClass.Accept(visitor);
            foreach (var nestedStruct in NestedStructs)
                nestedStruct.Accept(visitor);
        }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
        {
            await visitor.VisitStructAsync(this, cancellationToken).ConfigureAwait(false);
            foreach (var constant in Constants)
                await constant.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
            foreach (var field in Fields)
                await field.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
            foreach (var constructor in Constructors)
                await constructor.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
            foreach (var @event in Events)
                await @event.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
            foreach (var property in Properties)
                await property.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
            foreach (var methods in Methods)
                await methods.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);

            foreach (var nestedEnum in NestedEnums)
                await nestedEnum.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
            foreach (var nestedDelegate in NestedDelegates)
                await nestedDelegate.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
            foreach (var nestedInterface in NestedInterfaces)
                await nestedInterface.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
            foreach (var nestedClass in NestedClasses)
                await nestedClass.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
            foreach (var nestedStruct in NestedStructs)
                await nestedStruct.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Determines whether the current <see cref="StructDocumentationElement"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="StructDocumentationElement"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
            => base.Equals(type)
            && GenericParameters.Count == (type.GetGenericArguments().Length - (type.DeclaringType?.GetGenericArguments().Length ?? 0));
    }
}