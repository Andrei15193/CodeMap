using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeMap.DocumentationElements;
using CodeMap.ReferenceData;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented method declared by a type.</summary>
    public class MethodDeclaration : MemberDeclaration, IEquatable<MethodInfo>
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
        public IReadOnlyCollection<ExceptionDocumentationElement> Exceptions { get; internal set; }

        /// <summary>Determines whether the current <see cref="MethodDeclaration"/> is equal to the provided <paramref name="methodInfo"/>.</summary>
        /// <param name="methodInfo">The <see cref="MethodInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="MethodDeclaration"/> references the provided <paramref name="methodInfo"/>; <c>false</c> otherwise.</returns>
        public bool Equals(MethodInfo methodInfo)
            => methodInfo != null
            && string.Equals(Name, methodInfo.GetMethodName(), StringComparison.OrdinalIgnoreCase)
            && Parameters.Count == methodInfo.GetParameters().Length
            && Parameters
                .Zip(
                    methodInfo.GetParameters(),
                    (parameter, methodInfoParameter) => (
                        ExpectedParameterType: parameter.Type,
                        ActualParameterType: (parameter.IsInputByReference || parameter.IsInputOutputByReference || parameter.IsOutputByReference) && methodInfoParameter.ParameterType.IsByRef
                            ? methodInfoParameter.ParameterType.GetElementType()
                            : methodInfoParameter.ParameterType
                    )
                )
                .All(pair => pair.ExpectedParameterType.Equals(pair.ActualParameterType))
            && DeclaringType == methodInfo.DeclaringType;

        /// <summary>Determines whether the current <see cref="MethodDeclaration"/> is equal to the provided <paramref name="memberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="MethodDeclaration"/> references the provided <paramref name="memberInfo"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(MemberInfo memberInfo)
            => memberInfo is MethodInfo methodInfo ? Equals(methodInfo) : false;

        /// <summary>Determines whether the current <see cref="MethodDeclaration"/> is equal to the provided <paramref name="obj"/>.</summary>
        /// <param name="obj">The <see cref="object"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="MethodDeclaration"/> references the provided <paramref name="obj"/>; <c>false</c> otherwise.</returns>
        /// <remarks>
        /// If the provided <paramref name="obj"/> is a <see cref="MemberInfo"/> instance then the comparison is done by comparing members and
        /// determining whether the current instance actually maps to the provided <see cref="MemberInfo"/>. Otherwise the equality is determined
        /// by comparing references.
        /// </remarks>
        public override bool Equals(object obj)
            => obj is MethodInfo methodInfo ? Equals(methodInfo) : base.Equals(obj);

        /// <summary>Calculates the has code for the current <see cref="MethodDeclaration"/>.</summary>
        /// <returns>Returns a hash code for the current instance.</returns>
        public override int GetHashCode()
            => base.GetHashCode();

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DeclarationNodeVisitor visitor)
            => visitor.VisitMethod(this);
    }
}