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

        public static TInstance AssertTypeReference<TInstance>(this TInstance instance, string @namespace, string name)
            where TInstance : TypeReferenceDocumentationElement
        {
            instance
                .AssertIs<InstanceTypeDocumentationElement>(
                    instanceTypeReference =>
                        instanceTypeReference
                            .AssertEqual(() => instanceTypeReference.Name, name)
                            .AssertEqual(() => instanceTypeReference.Namespace, @namespace)
                );
            return instance;
        }

        public static TInstance AssertTypeReference<TInstance>(this TInstance instance, Func<TypeReferenceDocumentationElement> selector, string @namespace, string name)
        {
            selector().AssertTypeReference(@namespace, name);
            return instance;
        }

        public static TInstance AssertGenericArguments<TInstance>(this TInstance instance, params Action<TypeReferenceDocumentationElement>[] callbacks)
        {
            instance.AssertIs<InstanceTypeDocumentationElement>(
                instanceTypeReference => instanceTypeReference.AssertCollectionMember(() => instanceTypeReference.GenericArguments, callbacks)
            );
            return instance;
        }

        public static TInstance AssertTypeReferenceAssembly<TInstance>(this TInstance instance, string name, Version version)
            where TInstance : TypeReferenceDocumentationElement
        {
            instance
                .AssertIs<InstanceTypeDocumentationElement>(
                    instanceTypeReference =>
                        instanceTypeReference.AssertMember(
                            () => instanceTypeReference.Assembly,
                            assembly => assembly
                                .AssertEqual(() => assembly.Name, name)
                                .AssertEqual(() => assembly.Version, version)
                        )
                );
            return instance;
        }

        public static TInstance AssertTypeReferenceAssembly<TInstance>(this TInstance instance, string name, Version version, string culture, string publicKeyToken)
            where TInstance : TypeReferenceDocumentationElement
        {
            instance
                .AssertIs<InstanceTypeDocumentationElement>(
                    instanceTypeReference =>
                        instanceTypeReference.AssertMember(
                            () => instanceTypeReference.Assembly,
                            assembly => assembly
                                .AssertEqual(() => assembly.Name, name)
                                .AssertEqual(() => assembly.Version, version)
                                .AssertEqual(() => assembly.Culture, culture)
                                .AssertEqual(() => assembly.PublicKeyToken, publicKeyToken)
                        )
                );
            return instance;
        }

        public static TInstance AssertTypeReferenceAssembly<TInstance>(this TInstance instance, Func<TypeReferenceDocumentationElement> selector, string name, Version version)
        {
            selector().AssertTypeReferenceAssembly(name, version);
            return instance;
        }

        public static TInstance AssertTypeReferenceAssembly<TInstance>(this TInstance instance, Func<TypeReferenceDocumentationElement> selector, string name, Version version, string culture, string publicKeyToken)
        {
            selector().AssertTypeReferenceAssembly(name, version, culture, publicKeyToken);
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

        public static TInstance AssertEqual<TInstance, TValue>(this TInstance instance, Func<TValue> selector, TValue expected)
        {
            Assert.Equal(expected, selector());
            return instance;
        }

        public static TInstance AssertTrue<TInstance>(this TInstance instance, Func<bool> selector)
        {
            Assert.True(selector());
            return instance;
        }

        public static TInstance AssertFalse<TInstance>(this TInstance instance, Func<bool> selector)
        {
            Assert.False(selector());
            return instance;
        }

        public static TInstance AssertSame<TInstance>(this TInstance instance, TInstance expected)
            where TInstance : class
        {
            Assert.Same(expected, instance);
            return instance;
        }

        public static TInstance AssertSame<TInstance, TValue>(this TInstance instance, Func<TValue> selector, TValue expected) where TValue : class
        {
            Assert.Same(expected, selector());
            return instance;
        }

        public static TInstance AssertSameItems<TInstance, TItem>(this TInstance instance, Func<IEnumerable<TItem>> selector, IEnumerable<TItem> expected) where TItem : class
        {
            var actual = selector();
            var expectedItems = expected as IReadOnlyCollection<TItem> ?? expected.ToList();
            var actualItems = actual as IReadOnlyCollection<TItem> ?? actual.ToList();
            Assert.Equal(expectedItems.Count, actualItems.Count);
            foreach (var pair in expectedItems.Zip(actualItems, (expectedItem, actualItem) => new { ExpectedItem = expectedItem, ActualItem = actualItem }))
                Assert.Same(pair.ExpectedItem, pair.ActualItem);

            return instance;
        }

        public static TInstance AssertNull<TInstance, TValue>(this TInstance instance, Func<TValue> selector) where TValue : class
        {
            Assert.Null(selector());
            return instance;
        }
    }
}