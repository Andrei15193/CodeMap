using System;
using System.Linq;

namespace CodeMap.DeclarationNodes
{
    /// <summary>Represents a documented method generic parameter.</summary>
    public sealed class GenericMethodParameterData : GenericParameterData
    {
        internal GenericMethodParameterData()
        {
        }

        /// <summary>The method declaring the generic parameter.</summary>
        public MethodDeclaration DeclaringMethod { get; internal set; }

        /// <summary>Determines whether the current <see cref="GenericMethodParameterData"/> is equal to the provided <paramref name="type"/>.</summary>
        /// <param name="type">The <see cref="Type"/> to compare to.</param>
        /// <returns>Returns <c>true</c> if the current <see cref="GenericMethodParameterData"/> references the provided <paramref name="type"/>; <c>false</c> otherwise.</returns>
        public override bool Equals(Type type)
        {
            if (type is null || !type.IsGenericMethodParameter)
                return false;

            return string.Equals(Name, type.Name, StringComparison.OrdinalIgnoreCase)
                && Position == type.GenericParameterPosition
                && DeclaringMethod.DeclaringType == type.DeclaringMethod.DeclaringType
                && string.Equals(DeclaringMethod.Name, type.DeclaringMethod.Name, StringComparison.OrdinalIgnoreCase)
                && DeclaringMethod.GenericParameters.Count == type.DeclaringMethod.GetGenericArguments().Length
                && DeclaringMethod
                    .Parameters
                    .Zip(
                        type.DeclaringMethod.GetParameters(),
                        (parameterData, parameterInfo) => new
                        {
                            ParameterData = parameterData,
                            ParameterInfo = parameterInfo
                        }
                    )
                    .All(
                        pair =>
                        {
                            var unwrappedType = _UnwrapTypeByRef(pair.ParameterInfo.ParameterType);
                            return unwrappedType == type || pair.ParameterData.Type == unwrappedType;
                        }
                    );
        }

        private Type _UnwrapTypeByRef(Type type)
        {
            var referentType = type;
            while (referentType.IsByRef)
                referentType = referentType.GetElementType();
            return referentType;
        }
    }
}