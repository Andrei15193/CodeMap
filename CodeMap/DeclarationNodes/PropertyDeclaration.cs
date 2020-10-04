using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeMap.DocumentationElements;
using CodeMap.ReferenceData;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented property declared by a type.</summary>
    public sealed class PropertyDeclaration : MemberDeclaration, IEquatable<PropertyInfo>
    {
        internal PropertyDeclaration()
        {
        }

        /// <summary>The property type.</summary>
        public BaseTypeReference Type { get; internal set; }

        /// <summary>Indicates whether the property is static.</summary>
        public bool IsStatic { get; internal set; }

        /// <summary>Indicates whether the property has been marked as virtual.</summary>
        public bool IsVirtual { get; internal set; }

        /// <summary>Indicates whether the property has been marked as abstract.</summary>
        public bool IsAbstract { get; internal set; }

        /// <summary>Indicates whether the property is an override.</summary>
        public bool IsOverride { get; internal set; }

        /// <summary>Indicates whether the property has been marked as sealed.</summary>
        public bool IsSealed { get; internal set; }

        /// <summary>Indicates whether the property hides a member from a base type.</summary>
        public bool IsShadowing { get; internal set; }

        /// <summary>Information about the getter accessor.</summary>
        public PropertyAccessorData Getter { get; internal set; }

        /// <summary>Information about the setter accessor.</summary>
        public PropertyAccessorData Setter { get; internal set; }

        /// <summary>The method parameters.</summary>
        public IReadOnlyList<ParameterData> Parameters { get; internal set; }

        /// <summary>Documentation about the how the value of the property is calculated.</summary>
        public ValueDocumentationElement Value { get; internal set; }

        /// <summary>Documented exceptions that might be thrown when using the property.</summary>
        public IReadOnlyCollection<ExceptionDocumentationElement> Exceptions { get; internal set; }

        /// <summary>Determines whether the current <see cref="PropertyDeclaration"/> is equal to the provided <paramref name="propertyInfo"/>.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="PropertyDeclaration"/> references the provided <paramref name="propertyInfo"/>; <c>false</c> otherwise.</returns>
        public bool Equals(PropertyInfo propertyInfo)
            => propertyInfo != null
            && string.Equals(Name, propertyInfo.Name, StringComparison.OrdinalIgnoreCase)
            && Parameters.Count == propertyInfo.GetIndexParameters().Length
            && Parameters
                .Zip(
                    propertyInfo.GetIndexParameters(),
                    (parameter, propertyInfoParameter) => (
                        ExpectedParameterType: parameter.Type,
                        ActualParameterType: (parameter.IsInputByReference || parameter.IsInputOutputByReference || parameter.IsOutputByReference) && propertyInfoParameter.ParameterType.IsByRef
                            ? propertyInfoParameter.ParameterType.GetElementType()
                            : propertyInfoParameter.ParameterType
                    )
                )
                .All(pair => pair.ExpectedParameterType == pair.ActualParameterType)
            && DeclaringType == propertyInfo.DeclaringType;

        /// <summary>Determines whether the current <see cref="PropertyDeclaration"/> is equal to the provided <paramref name="memberInfo"/>.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="PropertyDeclaration"/> references the provided <paramref name="memberInfo"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(MemberInfo memberInfo)
            => memberInfo is PropertyInfo propertyInfo ? Equals(propertyInfo) : false;

        /// <summary>Determines whether the current <see cref="PropertyDeclaration"/> is equal to the provided <paramref name="obj"/>.</summary>
        /// <param name="obj">The <see cref="object"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="PropertyDeclaration"/> references the provided <paramref name="obj"/>; <c>false</c> otherwise.</returns>
        /// <remarks>
        /// If the provided <paramref name="obj"/> is a <see cref="MemberInfo"/> instance then the comparison is done by comparing members and
        /// determining whether the current instance actually maps to the provided <see cref="MemberInfo"/>. Otherwise the equality is determined
        /// by comparing references.
        /// </remarks>
        public override bool Equals(object obj)
            => obj is PropertyInfo propertyInfo ? Equals(propertyInfo) : base.Equals(obj);

        /// <summary>Calculates the has code for the current <see cref="PropertyDeclaration"/>.</summary>
        /// <returns>Returns a hash code for the current instance.</returns>
        public override int GetHashCode()
            => base.GetHashCode();

        /// <summary>Accepts the provided <paramref name="visitor"/> for traversing the documentation tree.</summary>
        /// <param name="visitor">The <see cref="DeclarationNodeVisitor"/> traversing the documentation tree.</param>
        public override void Accept(DeclarationNodeVisitor visitor)
            => visitor.VisitProperty(this);
    }
}