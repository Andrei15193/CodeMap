using CodeMap.Html;
using CodeMap.ReferenceData;
using CodeMap.Tests.Data;
using System.Linq;
using System.Reflection;
using Xunit;

namespace CodeMap.Tests.Html
{
    public class MemberReferenceExtensionsTests_GetSimpleNameReference
    {
        private readonly MemberReferenceFactory _memberReference = new();

        [Fact]
        public void GettingSimpleNameReferenceForAssemblyReferenceReturnsName()
        {
            var assemblyReference = _memberReference.Create(typeof(GlobalTestClass).Assembly);

            var simpleNameReference = assemblyReference.GetSimpleNameReference();

            Assert.Equal("CodeMap.Tests.Data", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForNamespaceReferenceReturnsName()
        {
            var namespaceReference = _memberReference.CreateNamespace("CodeMap.Tests.Data", typeof(GlobalTestClass).Assembly);

            var simpleNameReference = namespaceReference.GetSimpleNameReference();

            Assert.Equal("CodeMap.Tests.Data", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForTypeReferenceReturnsName()
        {
            var typeReference = _memberReference.Create(typeof(GlobalTestClass));

            var simpleNameReference = typeReference.GetSimpleNameReference();

            Assert.Equal("GlobalTestClass", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForGenericTypeReferenceReturnsNameAndGenericParameterNames()
        {
            var typeReference = _memberReference.Create(typeof(ITestGenericParameter<,,,,,,>));

            var simpleNameReference = typeReference.GetSimpleNameReference();

            Assert.Equal("ITestGenericParameter<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForConstructedGenericTypeReferenceReturnsNameAndGenericArgumentNames()
        {
            var typeReference = _memberReference.Create(typeof(TestClass<int>));

            var simpleNameReference = typeReference.GetSimpleNameReference();

            Assert.Equal("TestClass<int>", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForDelegateReferenceReturnsName()
        {
            var typeReference = _memberReference.Create(typeof(TestDelegate<>));

            var simpleNameReference = typeReference.GetSimpleNameReference();

            Assert.Equal("TestDelegate<TParam>", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForConstantReferenceReturnsDeclaringTypeNameAndOwnName()
        {
            var typeReference = _memberReference.Create(typeof(TestClass<>).GetField("ClassShadowedTestConstant"));

            var simpleNameReference = typeReference.GetSimpleNameReference();

            Assert.Equal("TestClass<TParam>.ClassShadowedTestConstant", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForFieldReferenceReturnsDeclaringTypeNameAndOwnName()
        {
            var typeReference = _memberReference.Create(typeof(TestClass<>).GetField("ShadowedTestField", BindingFlags.Instance | BindingFlags.NonPublic));

            var simpleNameReference = typeReference.GetSimpleNameReference();

            Assert.Equal("TestClass<TParam>.ShadowedTestField", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForConstructorReferenceReturnsDeclaringTypeNameOwnNameAndParameterTypeNames()
        {
            var typeReference = _memberReference.Create(typeof(TestClass<>).GetConstructors().Single());

            var simpleNameReference = typeReference.GetSimpleNameReference();

            Assert.Equal("TestClass<TParam>.TestClass(int)", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForEventReferenceReturnsDeclaringTypeNameAndOwnName()
        {
            var typeReference = _memberReference.Create(typeof(TestClass<>).GetEvent("ClassShadowedTestEvent"));

            var simpleNameReference = typeReference.GetSimpleNameReference();

            Assert.Equal("TestClass<TParam>.ClassShadowedTestEvent", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForPropertyReferenceReturnsDeclaringTypeNameAndOwnName()
        {
            var typeReference = _memberReference.Create(typeof(TestClass<>).GetProperty("VirtualTestProperty"));

            var simpleNameReference = typeReference.GetSimpleNameReference();

            Assert.Equal("TestClass<TParam>.VirtualTestProperty", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForIndexPropertyReferenceReturnsDeclaringTypeNameOwnNameAndParameterTypeNames()
        {
            var typeReference = _memberReference.Create(typeof(TestClass<>).GetProperty("Item"));

            var simpleNameReference = typeReference.GetSimpleNameReference();

            Assert.Equal("TestClass<TParam>.Item[int]", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForMethodReferenceReturnsDeclaringTypeNameOwnNameAndParameterTypeNames()
        {
            var typeReference = _memberReference.Create(typeof(TestClass<>).GetMethod("TestMethod22"));

            var simpleNameReference = typeReference.GetSimpleNameReference();

            Assert.Equal("TestClass<TParam>.TestMethod22(TParam)", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForGenericMethodReferenceReturnsDeclaringTypeNameGenericParameterNamesOwnNameAndParameterTypeNames()
        {
            var typeReference = _memberReference.Create(typeof(TestClass<>).GetMethod("TestMethod39"));

            var simpleNameReference = typeReference.GetSimpleNameReference();

            Assert.Equal("TestClass<TParam>.TestMethod39<TMethodParam>(TMethodParam&)", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForConstructedGenericMethodReferenceReturnsDeclaringTypeNameGenericArgumentNamesOwnNameAndParameterTypeNames()
        {
            var typeReference = _memberReference.Create(typeof(TestClass<>).GetMethod("TestMethod39").MakeGenericMethod(typeof(string)));

            var simpleNameReference = typeReference.GetSimpleNameReference();

            Assert.Equal("TestClass<TParam>.TestMethod39<string>(string&)", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForNestedTypeReferenceReturnsDeclaringTypeNameAndOwnName()
        {
            var typeReference = _memberReference.Create(typeof(TestClass<>.NestedTestEnum));

            var simpleNameReference = typeReference.GetSimpleNameReference();

            Assert.Equal("TestClass<TParam>.NestedTestEnum", simpleNameReference);
        }
    }
}