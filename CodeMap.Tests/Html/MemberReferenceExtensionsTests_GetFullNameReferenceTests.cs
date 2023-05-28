using CodeMap.Html;
using CodeMap.ReferenceData;
using CodeMap.Tests.Data;
using System.Linq;
using System.Reflection;
using Xunit;

namespace CodeMap.Tests.Html
{
    public class MemberReferenceExtensionsTests_GetFullNameReferenceTests
    {
        private readonly MemberReferenceFactory _memberReference = new();

        [Fact]
        public void GettingFullNameReferenceForAssemblyReferenceReturnsName()
        {
            var assemblyReference = _memberReference.Create(typeof(GlobalTestClass).Assembly);

            var fullNameReference = assemblyReference.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForNamespaceReferenceReturnsName()
        {
            var namespaceReference = _memberReference.CreateNamespace("CodeMap.Tests.Data", typeof(GlobalTestClass).Assembly);

            var fullNameReference = namespaceReference.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForGlobalTypeReferenceReturnsName()
        {
            var typeReference = _memberReference.Create(typeof(GlobalTestClass));

            var fullNameReference = typeReference.GetFullNameReference();

            Assert.Equal("GlobalTestClass", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForTypeReferenceReturnsNamespaceNameAndOwnName()
        {
            var typeReference = _memberReference.Create(typeof(TestBaseClass));

            var fullNameReference = typeReference.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestBaseClass", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForGenericTypeReferenceReturnsNamespaceNameOwnNameAndGenericParameterNames()
        {
            var typeReference = _memberReference.Create(typeof(ITestGenericParameter<,,,,,,>));

            var fullNameReference = typeReference.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.ITestGenericParameter<TParam1,TParam2,TParam3,TParam4,TParam5,TParam6,TParam7>", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForConstructedGenericTypeReferenceReturnsNamespaceNameOwnNameAndGenericArgumentNames()
        {
            var typeReference = _memberReference.Create(typeof(TestClass<int>));

            var fullNameReference = typeReference.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestClass<System.Int32>", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForDelegateReferenceReturnsNamespaceNameAndOwnName()
        {
            var typeReference = _memberReference.Create(typeof(TestDelegate<>));

            var fullNameReference = typeReference.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestDelegate<TParam>", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForConstantReferenceReturnsNamespaceNameDeclaringTypeNameAndOwnName()
        {
            var typeReference = _memberReference.Create(typeof(TestClass<>).GetField("ClassShadowedTestConstant"));

            var fullNameReference = typeReference.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestClass<TParam>.ClassShadowedTestConstant", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForFieldReferenceReturnsNamespaceNameDeclaringTypeNameAndOwnName()
        {
            var typeReference = _memberReference.Create(typeof(TestClass<>).GetField("ShadowedTestField", BindingFlags.Instance | BindingFlags.NonPublic));

            var fullNameReference = typeReference.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestClass<TParam>.ShadowedTestField", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForConstructorReferenceReturnsNamespaceNameDeclaringTypeNameOwnNameAndParameterTypeNames()
        {
            var typeReference = _memberReference.Create(typeof(TestClass<>).GetConstructors().Single());

            var fullNameReference = typeReference.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestClass<TParam>.TestClass(System.Int32)", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForEventReferenceReturnsNamespaceNameDeclaringTypeNameAndOwnName()
        {
            var typeReference = _memberReference.Create(typeof(TestClass<>).GetEvent("ClassShadowedTestEvent"));

            var fullNameReference = typeReference.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestClass<TParam>.ClassShadowedTestEvent", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForPropertyReferenceReturnsNamespaceNameDeclaringTypeNameAndOwnName()
        {
            var typeReference = _memberReference.Create(typeof(TestClass<>).GetProperty("VirtualTestProperty"));

            var fullNameReference = typeReference.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestClass<TParam>.VirtualTestProperty", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForIndexPropertyReferenceReturnsNamespaceNameDeclaringTypeNameOwnNameAndParameterTypeNames()
        {
            var typeReference = _memberReference.Create(typeof(TestClass<>).GetProperty("Item"));

            var fullNameReference = typeReference.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestClass<TParam>.Item[System.Int32]", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForMethodReferenceReturnsNamespaceNameDeclaringTypeNameOwnNameAndParameterTypeNames()
        {
            var typeReference = _memberReference.Create(typeof(TestClass<>).GetMethod("TestMethod22"));

            var fullNameReference = typeReference.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestClass<TParam>.TestMethod22(TParam)", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForGenericMethodReferenceReturnsNamespaceNameDeclaringTypeNameGenericParameterNamesOwnNameAndParameterTypeNames()
        {
            var typeReference = _memberReference.Create(typeof(TestClass<>).GetMethod("TestMethod39"));

            var fullNameReference = typeReference.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestClass<TParam>.TestMethod39<TMethodParam>(TMethodParam&)", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForConstructedGenericMethodReferenceReturnsNamespaceNameDeclaringTypeNameGenericArgumentNamesOwnNameAndParameterTypeNames()
        {
            var typeReference = _memberReference.Create(typeof(TestClass<>).GetMethod("TestMethod39").MakeGenericMethod(typeof(string)));

            var fullNameReference = typeReference.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestClass<TParam>.TestMethod39<System.String>(System.String&)", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForNestedTypeReferenceReturnsNamespaceNameDeclaringTypeNameAndOwnName()
        {
            var typeReference = _memberReference.Create(typeof(TestClass<>.NestedTestEnum));

            var fullNameReference = typeReference.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestClass<TParam>.NestedTestEnum", fullNameReference);
        }
    }
}