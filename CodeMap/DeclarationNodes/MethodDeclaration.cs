using System;
using System.Collections.Generic;
using System.Linq;
using CodeMap.DocumentationElements;
using CodeMap.ReferenceData;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented method declared by a type.</summary>
    public class MethodDeclaration : MemberDeclaration, IEquatable<MethodReference>
    {
        internal MethodDeclaration()
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
        public IReadOnlyCollection<ExceptionData> Exceptions { get; internal set; }

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DeclarationNodeVisitor visitor)
            => visitor.VisitMethod(this);

        /// <summary>Determines whether the current <see cref="MethodDeclaration"/> is equal to the provided <paramref name="memberReference"/>.</summary>
        /// <param name="memberReference">The <see cref="MemberReference"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="MethodDeclaration"/> references the provided <paramref name="memberReference"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(MemberReference memberReference)
            => memberReference is MethodReference methodReference
            && Equals(methodReference);

        /// <summary>Determines whether the current <see cref="MethodDeclaration"/> is equal to the provided <paramref name="methodReference"/>.</summary>
        /// <param name="methodReference">The <see cref="MethodReference"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="MethodDeclaration"/> references the provided <paramref name="methodReference"/>; <c>false</c> otherwise.</returns>
        public bool Equals(MethodReference methodReference)
            => methodReference != null
            && string.Equals(Name, methodReference.Name, StringComparison.OrdinalIgnoreCase)
            && DeclaringType == methodReference.DeclaringType
            && Parameters.Select(parameter => parameter.Type).SequenceEqual(methodReference.ParameterTypes);
    }
}