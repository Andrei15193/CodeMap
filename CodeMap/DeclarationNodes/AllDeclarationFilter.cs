using System;
using System.Reflection;

namespace CodeMap.DeclarationNodes
{
    internal class AllDeclarationFilter : DeclarationFilter
    {
        public override bool ShouldMap(Type type)
            => ShouldMap(memberInfo: type);

        public override bool ShouldMap(FieldInfo fieldInfo)
            => ShouldMap(memberInfo: fieldInfo);

        public override bool ShouldMap(ConstructorInfo constructorInfo)
            => ShouldMap(memberInfo: constructorInfo);

        public override bool ShouldMap(EventInfo eventInfo)
            => ShouldMap(memberInfo: eventInfo);

        public override bool ShouldMap(PropertyInfo propertyInfo)
            => ShouldMap(memberInfo: propertyInfo);

        public override bool ShouldMapPropertyAccessor(MethodInfo methodInfo)
            => true;

        public override bool ShouldMap(MethodInfo methodInfo)
            => ShouldMap(memberInfo: methodInfo);
    }
}