using System.Collections.Generic;
using CodeMap.DocumentationElements;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented constructor declared by a type.</summary>
    public class ConstructorDeclaration : MemberDeclaration
    {
        internal ConstructorDeclaration()
        {
        }

        /// <summary>The constructor parameters.</summary>
        public IReadOnlyList<ParameterData> Parameters { get; internal set; }

        /// <summary>Documented exceptions that might be thrown when calling the constructor.</summary>
        public IReadOnlyCollection<ExceptionData> Exceptions { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DeclarationNodeVisitor visitor)
            => visitor.VisitConstructor(this);
    }
}