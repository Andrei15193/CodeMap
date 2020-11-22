using System.Collections.Generic;
using CodeMap.DocumentationElements;
using CodeMap.ReferenceData;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented delegate declaration.</summary>
    public class DelegateDeclaration : TypeDeclaration
    {
        internal DelegateDeclaration(TypeReference typeReference)
            : base(typeReference)
        {
        }

        /// <summary>The delegate generic parameters.</summary>
        public IReadOnlyList<GenericTypeParameterData> GenericParameters { get; internal set; }

        /// <summary>The delegate parameters.</summary>
        public IReadOnlyList<ParameterData> Parameters { get; internal set; }

        /// <summary>The documented delegate return value.</summary>
        public MethodReturnData Return { get; internal set; }

        /// <summary>The delegate documented exceptions.</summary>
        public IReadOnlyList<ExceptionDocumentationElement> Exceptions { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DeclarationNodeVisitor visitor)
            => visitor.VisitDelegate(this);
    }
}