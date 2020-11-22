using System.Collections.Generic;
using CodeMap.DocumentationElements;
using CodeMap.ReferenceData;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented method declared by a type.</summary>
    public class MethodDeclaration : MemberDeclaration
    {
        internal MethodDeclaration(MethodReference methodReference)
            : base(methodReference)
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
        public IReadOnlyList<GenericMethodParameterData> GenericParameters { get; internal set; }

        /// <summary>The method parameters.</summary>
        public IReadOnlyList<ParameterData> Parameters { get; internal set; }

        /// <summary>The documented method return value.</summary>
        public MethodReturnData Return { get; internal set; }

        /// <summary>Documented exceptions that might be thrown when calling the method.</summary>
        public IReadOnlyCollection<ExceptionDocumentationElement> Exceptions { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DeclarationNodeVisitor visitor)
            => visitor.VisitMethod(this);
    }
}