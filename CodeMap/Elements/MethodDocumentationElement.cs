using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap.Elements
{
    /// <summary>Represents a documented method declared by a type.</summary>
    public class MethodDocumentationElement : MemberDocumentationElement
    {
        internal MethodDocumentationElement()
        {
        }

        /// <summary>Indicates whether the method is static.</summary>
        public bool IsStatic { get; internal set; }

        /// <summary>Indicates whether the method has been marked as virtual.</summary>
        public bool IsVirtual { get; internal set; }

        /// <summary>Indicates whether the method has been marked as abstract.</summary>
        public bool IsAbstract { get; internal set; }

        /// <summary>Indicates whether the method is an override.</summary>
        public bool IsOverride { get; internal set; }

        /// <summary>Indicates whether the method has been marked as sealed.</summary>
        public bool IsSealed { get; internal set; }

        /// <summary>Indicates whether the method hides a member from a base type.</summary>
        public bool IsShadowing { get; internal set; }

        /// <summary>The method generic parameters.</summary>
        public IReadOnlyList<MethodGenericParameterTypeData> GenericParameters { get; internal set; }

        /// <summary>The method parameters.</summary>
        public IReadOnlyList<ParameterData> Parameters { get; internal set; }

        /// <summary>The documented method return value.</summary>
        public ReturnsDocumentationElement Return { get; internal set; }

        /// <summary>Documented exceptions that might be thrown when calling the method.</summary>
        public IReadOnlyCollection<ExceptionData> Exceptions { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DocumentationVisitor visitor)
            => visitor.VisitMethod(this);

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree asynchronously.</summary>
        /// <param name="visitor">The <see cref="DocumentationVisitor"/> traversing the documentation tree.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task AcceptAsync(DocumentationVisitor visitor, CancellationToken cancellationToken)
            => visitor.VisitMethodAsync(this, cancellationToken);
    }
}