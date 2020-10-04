using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CodeMap.DeclarationNodes
{
    /// <summary>A declaration filter used to select which <see cref="MemberInfo"/>s to convert.</summary>
    public class DeclarationFilter
    {
        /// <summary>Gets a <see cref="DeclarationFilter"/> that maps all non compiler generated <see cref="MemberInfo"/>s.</summary>
        public static DeclarationFilter All
            => new AllDeclarationFilter();

        /// <summary>Determines whether the provided <paramref name="memberInfo"/> should be mapped.</summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> to check.</param>
        /// <returns>Returns <c>true</c> if the provided <paramref name="memberInfo"/> should be mapped; <c>false</c> otherwise.</returns>
        public virtual bool ShouldMap(MemberInfo memberInfo)
            => !Attribute.IsDefined(memberInfo, typeof(CompilerGeneratedAttribute));

        /// <summary>Determines whether the provided <paramref name="type"/> should be mapped.</summary>
        /// <param name="type">The <see cref="Type"/> to check.</param>
        /// <returns>Returns <c>true</c> if the provided <paramref name="type"/> should be mapped; <c>false</c> otherwise.</returns>
        public virtual bool ShouldMap(Type type)
            => ShouldMap(memberInfo: type) && (type.IsPublic || type.IsNestedPublic || type.IsNestedFamily || type.IsNestedFamORAssem);

        /// <summary>Determines whether the provided <paramref name="fieldInfo"/> should be mapped.</summary>
        /// <param name="fieldInfo">The <see cref="FieldInfo"/> to check.</param>
        /// <returns>Returns <c>true</c> if the provided <paramref name="fieldInfo"/> should be mapped; <c>false</c> otherwise.</returns>
        public virtual bool ShouldMap(FieldInfo fieldInfo)
            => ShouldMap(memberInfo: fieldInfo) && (fieldInfo.IsPublic || fieldInfo.IsFamily || fieldInfo.IsFamilyOrAssembly);

        /// <summary>Determines whether the provided <paramref name="constructorInfo"/> should be mapped.</summary>
        /// <param name="constructorInfo">The <see cref="ConstructorInfo"/> to check.</param>
        /// <returns>Returns <c>true</c> if the provided <paramref name="constructorInfo"/> should be mapped; <c>false</c> otherwise.</returns>
        public virtual bool ShouldMap(ConstructorInfo constructorInfo)
            => ShouldMap(memberInfo: constructorInfo) && _IsPublicProtectedOrProtectedInternal(constructorInfo);

        /// <summary>Determines whether the provided <paramref name="eventInfo"/> should be mapped.</summary>
        /// <param name="eventInfo">The <see cref="EventInfo"/> to check.</param>
        /// <returns>Returns <c>true</c> if the provided <paramref name="eventInfo"/> should be mapped; <c>false</c> otherwise.</returns>
        public virtual bool ShouldMap(EventInfo eventInfo)
            => ShouldMap(memberInfo: eventInfo) && (_IsPublicProtectedOrProtectedInternal(eventInfo.AddMethod) || _IsPublicProtectedOrProtectedInternal(eventInfo.RemoveMethod));

        /// <summary>Determines whether the provided <paramref name="propertyInfo"/> should be mapped.</summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> to check.</param>
        /// <returns>Returns <c>true</c> if the provided <paramref name="propertyInfo"/> should be mapped; <c>false</c> otherwise.</returns>
        public virtual bool ShouldMap(PropertyInfo propertyInfo)
            => ShouldMap(memberInfo: propertyInfo) && (_IsPublicProtectedOrProtectedInternal(propertyInfo.GetMethod) || _IsPublicProtectedOrProtectedInternal(propertyInfo.SetMethod));

        /// <summary>Determines whether the provided property accessor should be mapped.</summary>
        /// <param name="methodInfo">The <see cref="MethodInfo"/> representing the property accessor to check.</param>
        /// <returns>Returns <c>true</c> if the provided property accessor should be mapped; <c>false</c> otherwise.</returns>
        public virtual bool ShouldMapPropertyAccessor(MethodInfo methodInfo)
            => _IsPublicProtectedOrProtectedInternal(methodInfo);

        /// <summary>Determines whether the provided <paramref name="methodInfo"/> should be mapped.</summary>
        /// <param name="methodInfo">The <see cref="MethodInfo"/> to check.</param>
        /// <returns>Returns <c>true</c> if the provided <paramref name="methodInfo"/> should be mapped; <c>false</c> otherwise.</returns>
        public virtual bool ShouldMap(MethodInfo methodInfo)
            => ShouldMap(memberInfo: methodInfo) && _IsPublicProtectedOrProtectedInternal(methodInfo);

        private static bool _IsPublicProtectedOrProtectedInternal(MethodBase methodBase)
            => methodBase != null && (methodBase.IsPublic || methodBase.IsFamily || methodBase.IsFamilyOrAssembly);
    }
}