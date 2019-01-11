using System;
using System.Collections.Generic;
using System.Linq;
using CodeMap.Elements;
using Xunit;

namespace CodeMap.Tests
{
    internal static class MyAssert
    {
        public static TConcrete AssertIs<TConcrete>(this object instance)
        {
            Assert.IsType<TConcrete>(instance);
            return (TConcrete)instance;
        }

        public static TConcrete AssertIs<TConcrete>(this object instance, Action<TConcrete> callback)
        {
            Assert.IsType<TConcrete>(instance);
            var concrete = (TConcrete)instance;
            callback(concrete);
            return concrete;
        }

        public static TInstance AssertTypeReference<TInstance>(this TInstance instance, Func<TypeReferenceDocumentationElement> selector, string @namespace, string name)
        {
            selector()
                .AssertIs<InstanceTypeDocumentationElement>(
                    instanceTypeReference =>
                        instanceTypeReference
                            .AssertEquals(() => instanceTypeReference.Name, name)
                            .AssertEquals(() => instanceTypeReference.Namespace, @namespace)
                );
            return instance;
        }

        public static TInstance AssertTypeReferenceAssembly<TInstance>(this TInstance instance, Func<TypeReferenceDocumentationElement> selector, string name, Version version)
        {
            selector()
                .AssertIs<InstanceTypeDocumentationElement>(
                    instanceTypeReference =>
                        instanceTypeReference.AssertMember(
                            () => instanceTypeReference.Assembly,
                            assembly => assembly
                                .AssertEquals(() => assembly.Name, name)
                                .AssertEquals(() => assembly.Version, version)
                        )
                );
            return instance;
        }

        public static TInstance AssertTypeReferenceAssembly<TInstance>(this TInstance instance, Func<TypeReferenceDocumentationElement> selector, string name, Version version, string culture, string publicKeyToken)
        {
            selector()
                .AssertIs<InstanceTypeDocumentationElement>(
                    instanceTypeReference =>
                        instanceTypeReference.AssertMember(
                            () => instanceTypeReference.Assembly,
                            assembly => assembly
                                .AssertEquals(() => assembly.Name, name)
                                .AssertEquals(() => assembly.Version, version)
                                .AssertEquals(() => assembly.Culture, culture)
                                .AssertEquals(() => assembly.PublicKeyToken, publicKeyToken)
                        )
                );
            return instance;
        }

        public static TInstance AssertMember<TInstance, TMember>(this TInstance instance, Func<TMember> selector, Action<TMember> callback)
        {
            callback(selector());
            return instance;
        }

        public static TInstance AssertCollectionMember<TInstance, TItem>(this TInstance instance, Func<IEnumerable<TItem>> selector, params Action<TItem>[] callbacks)
        {
            var items = selector();
            var itemsList = items as IReadOnlyCollection<TItem> ?? items.ToList();
            Assert.Equal(callbacks.Length, itemsList.Count);
            foreach (var pair in callbacks.Zip(itemsList, (callback, item) => new { Callback = callback, Item = item }))
                pair.Callback(pair.Item);
            return instance;
        }

        public static TInstance AssertEmpty<TInstance, TItem>(this TInstance instance, Func<IEnumerable<TItem>> selector)
        {
            Assert.Empty(selector());
            return instance;
        }

        public static TInstance AssertEquals<TInstance, TValue>(this TInstance instance, Func<TValue> selector, TValue expected)
        {
            Assert.Equal(expected, selector());
            return instance;
        }

        public static TInstance AssertSame<TInstance, TValue>(this TInstance instance, Func<TValue> selector, TValue expected) where TValue : class
        {
            Assert.Same(expected, selector());
            return instance;
        }

        public static TInstance AssertNull<TInstance, TValue>(this TInstance instance, Func<TValue> selector) where TValue : class
        {
            Assert.Null(selector());
            return instance;
        }
    }
}