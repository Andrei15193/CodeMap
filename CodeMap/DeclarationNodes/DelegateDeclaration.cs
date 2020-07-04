using System;
using System.Collections.Generic;
using CodeMap.DocumentationElements;
using CodeMap.ReferenceData;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented delegate declaration.</summary>
    public class DelegateDeclaration : TypeDeclaration
    {
        internal DelegateDeclaration()
        {
        }

        /// <summary>The delegate generic parameters.</summary>
        public IReadOnlyList<GenericTypeParameterData> GenericParameters { get; internal set; }

        /// <summary>The delegate parameters.</summary>
        public IReadOnlyList<ParameterData> Parameters { get; internal set; }

        /// <summary>The documented delegate return value.</summary>
        public MethodReturnData Return { get; internal set; }

        /// <summary>The delegate documented exceptions.</summary>
        public IReadOnlyCollection<ExceptionData> Exceptions { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DeclarationNodeVisitor visitor)
            => visitor.VisitDelegate(this);

        /// <summary>Determines whether the current <see cref="DelegateDeclaration"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="DelegateDeclaration"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
            => base.Equals(type)
            && GenericParameters.Count == (type.GetGenericArguments().Length - (type.DeclaringType?.GetGenericArguments().Length ?? 0));

        /// <summary>Determines whether the current <see cref="DelegateDeclaration"/> is equal to the provided <paramref name="typeReference"/>.</summary>
        /// <param name="typeReference">The <see cref="TypeReference"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="DelegateDeclaration"/> references the provided <paramref name="typeReference"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(TypeReference typeReference)
            => base.Equals(typeReference)
            && GenericParameters.Count == typeReference.GenericArguments.Count;
    }
}