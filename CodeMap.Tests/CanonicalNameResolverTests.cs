using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace CodeMap.Tests
{
    [CLSCompliant(false)]
    public class CanonicalNameResolverTests
    {
        private static CanonicalNameResolver _CanonicalNameResolver { get; } = new CanonicalNameResolver(
                new[] { typeof(CanonicalNameResolverTests).Assembly }
                    .Concat(
                        typeof(CanonicalNameResolverTests)
                            .Assembly
                            .GetReferencedAssemblies()
                            .Select(Assembly.Load)
                    )
            );

        [Fact]
        public void TryingToGetCanonicalNameFromNullThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => _CanonicalNameResolver.GetCanonicalNameFrom(null));

            Assert.Equal(new ArgumentNullException("memberInfo").Message, exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        [InlineData("\r")]
        [InlineData("\n")]
        public void TryingToGetMemberInfoFromEmptyOrWhiteSpaceCanonicalNameThrowsException(string canonicalName)
        {
            var exception = Assert.Throws<ArgumentException>(() => _CanonicalNameResolver.TryFindMemberInfoFor(canonicalName));

            Assert.Equal(new ArgumentException("Cannot be 'null', empty or white space.", "canonicalName").Message, exception.Message);
        }

        [Theory]
        [InlineData("t")]
        [InlineData("te")]
        public void TryingToGetMemberInfoFromCanonicalNameWithAtMostTwoCharactersThrowsException(string canonicalName)
        {
            var exception = Assert.Throws<ArgumentException>(() => _CanonicalNameResolver.TryFindMemberInfoFor(canonicalName));

            Assert.Equal(new ArgumentException($"The canonical name must be at least three characters long, '{canonicalName}' given.", "canonicalName").Message, exception.Message);
        }

        [Theory]
        [InlineData("missing_member_type_character_identifier")]
        [InlineData("T.wrong_separator")]
        public void TryingToGetMemberInfoFromIllFormedCanonicalNameThrowsException(string canonicalName)
        {
            var exception = Assert.Throws<ArgumentException>(() => _CanonicalNameResolver.TryFindMemberInfoFor(canonicalName));

            Assert.Equal(
                new ArgumentException(
                    $"The canonical name must be in the form '<member_type_identifier_character>:<canonical_name_identifier>' (e.g.: T:SomeNamespace.SomeClass), '{canonicalName}' given.",
                    "canonicalName"
                ).Message,
                exception.Message
            );
        }

        [Fact]
        public void TryingToGetMemberInfoFromUnknownMemberTypeThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() => _CanonicalNameResolver.TryFindMemberInfoFor("K:wrong_member_type"));

            Assert.Equal(
                new ArgumentException(
                    "Cannot find member type for 'K:wrong_member_type' canonical name (T, F, E, P or M must be the first character in the canonical name).",
                    "canonicalName"
                ).Message,
                exception.Message
            );
        }

        [Fact]
        public void TryingToCreateResolverWithNullAssemblyCollectionThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("searchAssemblies", () => new CanonicalNameResolver(null));

            Assert.Equal(new ArgumentNullException("searchAssemblies").Message, exception.Message);
        }

        [Fact]
        public void TryingToCreateResolverWithAssemblyCollectionContainingNullThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>("searchAssemblies", () => new CanonicalNameResolver(new Assembly[] { null }));

            Assert.Equal(new ArgumentException("Cannot contain 'null' assemblies.", "searchAssemblies").Message, exception.Message);
        }

        [Fact]
        public void GettingCanonicalNameForTestEnumWorks()
        {
            _AssertResolver(
                "T:CodeMap.Tests.TestEnum",
                typeof(TestEnum)
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestEnumWorks()
        {
            _AssertResolver(
                "T:CodeMap.Tests.TestClass.NestedTestEnum",
                typeof(TestClass).GetNestedType("NestedTestEnum", BindingFlags.NonPublic | BindingFlags.Public)
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestEnumMemberWorks()
        {
            _AssertResolver(
                "F:CodeMap.Tests.TestEnum.TestMember1",
                typeof(TestEnum)
                    .GetField(
                        "TestMember1",
                        BindingFlags.Public | BindingFlags.Static
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestEnumMemberWorks()
        {
            _AssertResolver(
                "F:CodeMap.Tests.TestClass.NestedTestEnum.TestMember1",
                typeof(TestClass)
                    .GetNestedType("NestedTestEnum", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetField(
                        "TestMember1",
                        BindingFlags.Public | BindingFlags.Static
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestDelegateWorks()
        {
            _AssertResolver(
                "T:CodeMap.Tests.TestDelegate",
                typeof(TestDelegate)
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestDelegateWorks()
        {
            _AssertResolver(
                "T:CodeMap.Tests.TestClass.NestedTestDelegate",
                typeof(TestClass).GetNestedType("NestedTestDelegate", BindingFlags.NonPublic | BindingFlags.Public)
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestDelegateWithOneGenericParameterWorks()
        {
            _AssertResolver(
                "T:CodeMap.Tests.TestDelegate`1",
                typeof(TestDelegate<>)
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestDelegateWithOneGenericParameterWorks()
        {
            _AssertResolver(
                "T:CodeMap.Tests.TestClass.NestedTestDelegate`1",
                typeof(TestClass).GetNestedType("NestedTestDelegate`1", BindingFlags.NonPublic | BindingFlags.Public)
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestDelegateInsideTestClassWithOneGenericParameterWorks()
        {
            _AssertResolver(
                "T:CodeMap.Tests.TestClass`1.NestedTestDelegate`1",
                typeof(TestClass<>).GetNestedType("NestedTestDelegate`1", BindingFlags.NonPublic | BindingFlags.Public)
            );
        }

        [Fact]
        public void GettingCanonicalNameForITestInterfaceWorks()
        {
            _AssertResolver(
                "T:CodeMap.Tests.ITestInterface",
                typeof(ITestInterface)
            );
        }

        [Fact]
        public void GettingCanonicalNameForITestInterfaceEventWorks()
        {
            _AssertResolver(
                "E:CodeMap.Tests.ITestInterface.TestEvent",
                typeof(ITestInterface)
                    .GetEvent(
                        "TestEvent",
                        BindingFlags.Public | BindingFlags.Instance
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForINestedTestInterfaceEventWorks()
        {
            _AssertResolver(
                "E:CodeMap.Tests.TestClass.INestedTestInterface.TestEvent",
                typeof(TestClass)
                    .GetNestedType("INestedTestInterface", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetEvent(
                        "TestEvent",
                        BindingFlags.Public | BindingFlags.Instance
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForITestInterfacePropertyWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.ITestInterface.TestProperty",
                typeof(ITestInterface)
                    .GetProperty(
                        "TestProperty",
                        BindingFlags.Public | BindingFlags.Instance
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForINestedTestInterfacePropertyWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass.INestedTestInterface.TestProperty",
                typeof(TestClass)
                    .GetNestedType("INestedTestInterface", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetProperty(
                        "TestProperty",
                        BindingFlags.Public | BindingFlags.Instance
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForITestInterfaceIndexerWithOneParameterWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.ITestInterface.Item(System.String)",
                typeof(ITestInterface)
                    .GetProperty(
                        "Item",
                        BindingFlags.Public | BindingFlags.Instance,
                        Type.DefaultBinder,
                        null,
                        new[] { typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForINestedTestInterfaceIndexerWithOneParameterWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass.INestedTestInterface.Item(System.String)",
                typeof(TestClass)
                    .GetNestedType("INestedTestInterface", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetProperty(
                        "Item",
                        BindingFlags.Public | BindingFlags.Instance,
                        Type.DefaultBinder,
                        null,
                        new[] { typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForITestInterfaceIndexerWithTwoParametersWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.ITestInterface.Item(System.String,System.Int32)",
                typeof(ITestInterface)
                    .GetProperty(
                        "Item",
                        BindingFlags.Public | BindingFlags.Instance,
                        Type.DefaultBinder,
                        null,
                        new[] { typeof(string), typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForINestedTestInterfaceIndexerWithTwoParametersWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass.INestedTestInterface.Item(System.String,System.Int32)",
                typeof(TestClass)
                    .GetNestedType("INestedTestInterface", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetProperty(
                        "Item",
                        BindingFlags.Public | BindingFlags.Instance,
                        Type.DefaultBinder,
                        null,
                        new[] { typeof(string), typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForITestInterfaceMethodWithNoParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.ITestInterface.TestMethod",
                typeof(ITestInterface)
                    .GetMethod(
                        "TestMethod",
                        BindingFlags.Public | BindingFlags.Instance,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForINestedTestInterfaceMethodWithNoParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.INestedTestInterface.TestMethod",
                typeof(TestClass)
                    .GetNestedType("INestedTestInterface", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "TestMethod",
                        BindingFlags.Public | BindingFlags.Instance,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForITestInterfaceMethodWithOneParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.ITestInterface.TestMethod(System.Int32)",
                typeof(ITestInterface)
                    .GetMethod(
                        "TestMethod",
                        BindingFlags.Public | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForINestedTestInterfaceMethodWithOneParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.INestedTestInterface.TestMethod(System.Int32)",
                typeof(TestClass)
                    .GetNestedType("INestedTestInterface", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "TestMethod",
                        BindingFlags.Public | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForITestInterfaceMethodWithTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.ITestInterface.TestMethod(System.Int32,System.String)",
                typeof(ITestInterface)
                    .GetMethod(
                        "TestMethod",
                        BindingFlags.Public | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(int), typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForINestedTestInterfaceMethodWithTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.INestedTestInterface.TestMethod(System.Int32,System.String)",
                typeof(TestClass)
                    .GetNestedType("INestedTestInterface", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "TestMethod",
                        BindingFlags.Public | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(int), typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassWorks()
        {
            _AssertResolver(
                "T:CodeMap.Tests.TestClass",
                typeof(TestClass)
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWorks()
        {
            _AssertResolver(
                "T:CodeMap.Tests.TestClass.NestedTestClass",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass", BindingFlags.NonPublic | BindingFlags.Public)
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassFieldWorks()
        {
            _AssertResolver(
                "F:CodeMap.Tests.TestClass._testField",
                typeof(TestClass)
                    .GetField(
                        "_testField",
                        BindingFlags.NonPublic | BindingFlags.Instance
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassFieldWorks()
        {
            _AssertResolver(
                "F:CodeMap.Tests.TestClass.NestedTestClass._testField",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetField(
                        "_testField",
                        BindingFlags.NonPublic | BindingFlags.Instance
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassConstructorWithNoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.#ctor",
                typeof(TestClass)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassConstructorWithNoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass.#ctor",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassConstructorWithOneParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.#ctor(System.Double)",
                typeof(TestClass)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(double) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassConstructorWithOneParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass.#ctor(System.Double)",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(double) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassConstructorWithTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.#ctor(System.Double,System.Int32)",
                typeof(TestClass)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(double), typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassConstructorWithTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass.#ctor(System.Double,System.Int32)",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(double), typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassStaticConstructorWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.#cctor",
                typeof(TestClass)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassStaticConstructorWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass.#cctor",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassEventWorks()
        {
            _AssertResolver(
                "E:CodeMap.Tests.TestClass.TestEvent",
                typeof(TestClass)
                    .GetEvent(
                        "TestEvent",
                        BindingFlags.NonPublic | BindingFlags.Instance
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassEventWorks()
        {
            _AssertResolver(
                "E:CodeMap.Tests.TestClass.NestedTestClass.TestEvent",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetEvent(
                        "TestEvent",
                        BindingFlags.NonPublic | BindingFlags.Instance
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassStaticEventWorks()
        {
            _AssertResolver(
                "E:CodeMap.Tests.TestClass.StaticTestEvent",
                typeof(TestClass)
                    .GetEvent(
                        "StaticTestEvent",
                        BindingFlags.NonPublic | BindingFlags.Static
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassStaticEventWorks()
        {
            _AssertResolver(
                "E:CodeMap.Tests.TestClass.NestedTestClass.StaticTestEvent",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetEvent(
                        "StaticTestEvent",
                        BindingFlags.NonPublic | BindingFlags.Static
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassPropertyWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass.TestProperty",
                typeof(TestClass)
                    .GetProperty(
                        "TestProperty",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        null,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassPropertyWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass.NestedTestClass.TestProperty",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetProperty(
                        "TestProperty",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        null,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassStaticPropertyWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass.StaticTestProperty",
                typeof(TestClass)
                    .GetProperty(
                        "StaticTestProperty",
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        null,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassStaticPropertyWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass.NestedTestClass.StaticTestProperty",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetProperty(
                        "StaticTestProperty",
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        null,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassIndexerWithOneParameterWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass.Item(System.String)",
                typeof(TestClass)
                    .GetProperty(
                        "Item",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        null,
                        new[] { typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassIndexerWithOneParameterWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass.NestedTestClass.Item(System.String)",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetProperty(
                        "Item",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        null,
                        new[] { typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassIndexerWithTwoParametersWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass.Item(System.String,System.Int32)",
                typeof(TestClass)
                    .GetProperty(
                        "Item",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        null,
                        new[] { typeof(string), typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassIndexerWithTwoParametersWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass.NestedTestClass.Item(System.String,System.Int32)",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetProperty(
                        "Item",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        null,
                        new[] { typeof(string), typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassMethodWithNoParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.TestMethod",
                typeof(TestClass)
                    .GetMethod(
                        "TestMethod",
                        0,
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassMethodWithNoParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass.TestMethod",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "TestMethod",
                        0,
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassMethodWithOneParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.TestMethod(System.Int32)",
                typeof(TestClass)
                    .GetMethod(
                        "TestMethod",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassMethodWithOneParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass.TestMethod(System.Int32)",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "TestMethod",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassMethodWithTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.TestMethod(System.Int32,System.String)",
                typeof(TestClass)
                    .GetMethod(
                        "TestMethod",
                        0,
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(int), typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassMethodWithTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass.TestMethod(System.Int32,System.String)",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "TestMethod",
                        0,
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(int), typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassMethodWithOneGenericParameterAndTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.TestMethod``1(System.Int32,System.String)",
                typeof(TestClass)
                    .GetMethod(
                        "TestMethod",
                        1,
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(int), typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassMethodWithOneGenericParameterAndTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass.TestMethod``1(System.Int32,System.String)",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "TestMethod",
                        1,
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(int), typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassMethodWithTwoGenericParametersAndNoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.TestMethod``2",
                typeof(TestClass)
                    .GetMethod(
                        "TestMethod",
                        2,
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassMethodWithTwoGenericParametersAndNoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass.TestMethod``2",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "TestMethod",
                        2,
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassStaticMethodWithNoParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.StaticTestMethod",
                typeof(TestClass)
                    .GetMethod(
                        "StaticTestMethod",
                        0,
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassStaticMethodWithNoParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass.StaticTestMethod",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "StaticTestMethod",
                        0,
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassStaticMethodWithOneParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.StaticTestMethod(System.Int32)",
                typeof(TestClass)
                    .GetMethod(
                        "StaticTestMethod",
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        new[] { typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassStaticMethodWithOneParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass.StaticTestMethod(System.Int32)",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "StaticTestMethod",
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        new[] { typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassStaticMethodWithTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.StaticTestMethod(System.Int32,System.String)",
                typeof(TestClass)
                    .GetMethod(
                        "StaticTestMethod",
                        0,
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        new[] { typeof(int), typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassStaticMethodWithTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass.StaticTestMethod(System.Int32,System.String)",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "StaticTestMethod",
                        0,
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        new[] { typeof(int), typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassStaticMethodWithOneGenericParameterAndTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.StaticTestMethod``1(System.Int32,System.String)",
                typeof(TestClass)
                    .GetMethod(
                        "StaticTestMethod",
                        1,
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        new[] { typeof(int), typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassStaticMethodWithOneGenericParameterAndTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass.StaticTestMethod``1(System.Int32,System.String)",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "StaticTestMethod",
                        1,
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        new[] { typeof(int), typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassStaticMethodWithTwoGenericParametersAndNoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.StaticTestMethod``2",
                typeof(TestClass)
                    .GetMethod(
                        "StaticTestMethod",
                        2,
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassStaticMethodWithTwoGenericParametersAndNoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass.StaticTestMethod``2",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "StaticTestMethod",
                        2,
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassWithOneGenericParameterWorks()
        {
            _AssertResolver(
                "T:CodeMap.Tests.TestClass`1",
                typeof(TestClass<>)
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterWorks()
        {
            _AssertResolver(
                "T:CodeMap.Tests.TestClass.NestedTestClass`1",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterInsideTestClassWithOneGenericParameterWorks()
        {
            _AssertResolver(
                "T:CodeMap.Tests.TestClass`1.NestedTestClass`1",
                typeof(TestClass<>)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassWithOneGenericParameterFieldWorks()
        {
            _AssertResolver(
                "F:CodeMap.Tests.TestClass`1._testField",
                typeof(TestClass<>)
                    .GetField(
                        "_testField",
                        BindingFlags.NonPublic | BindingFlags.Instance
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterFieldWorks()
        {
            _AssertResolver(
                "F:CodeMap.Tests.TestClass.NestedTestClass`1._testField",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetField(
                        "_testField",
                        BindingFlags.NonPublic | BindingFlags.Instance
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterInsideTestClassWithOneGenericParameterFieldWorks()
        {
            _AssertResolver(
                "F:CodeMap.Tests.TestClass`1.NestedTestClass`1._testField",
                typeof(TestClass<>)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetField(
                        "_testField",
                        BindingFlags.NonPublic | BindingFlags.Instance
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassWithOneGenericParameterConstructorWithNoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.#ctor",
                typeof(TestClass<>)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterConstructorWithNoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass`1.#ctor",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterInsideTestClassWithOneGenericParameterConstructorWithNoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.NestedTestClass`1.#ctor",
                typeof(TestClass<>)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassWithOneGenericParameterConstructorWithOneParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.#ctor(System.Double)",
                typeof(TestClass<>)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(double) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterConstructorWithOneParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass`1.#ctor(System.Double)",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(double) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterInsideTestClassWithOneGenericParameterConstructorWithOneParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.NestedTestClass`1.#ctor(System.Double)",
                typeof(TestClass<>)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(double) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassWithOneGenericParameterConstructorWithTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.#ctor(System.Double,System.Int32)",
                typeof(TestClass<>)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(double), typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterConstructorWithTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass`1.#ctor(System.Double,System.Int32)",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(double), typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterInsideTestClassWithOneGenericParameterConstructorWithTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.NestedTestClass`1.#ctor(System.Double,System.Int32)",
                typeof(TestClass<>)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(double), typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassWithOneGenericParameterStaticConstructorWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.#cctor",
                typeof(TestClass<>)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterStaticConstructorWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass`1.#cctor",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterInsideTestClassWithOneGenericParameterStaticConstructorWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.NestedTestClass`1.#cctor",
                typeof(TestClass<>)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassWithOneGenericParameterEventWorks()
        {
            _AssertResolver(
                "E:CodeMap.Tests.TestClass`1.TestEvent",
                typeof(TestClass<>)
                    .GetEvent(
                        "TestEvent",
                        BindingFlags.NonPublic | BindingFlags.Instance
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterEventWorks()
        {
            _AssertResolver(
                "E:CodeMap.Tests.TestClass.NestedTestClass`1.TestEvent",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetEvent(
                        "TestEvent",
                        BindingFlags.NonPublic | BindingFlags.Instance
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterInsideTestClassWithOneGenericParameterEventWorks()
        {
            _AssertResolver(
                "E:CodeMap.Tests.TestClass`1.NestedTestClass`1.TestEvent",
                typeof(TestClass<>)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetEvent(
                        "TestEvent",
                        BindingFlags.NonPublic | BindingFlags.Instance
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassWithOneGenericParameterStaticEventWorks()
        {
            _AssertResolver(
                "E:CodeMap.Tests.TestClass`1.StaticTestEvent",
                typeof(TestClass<>)
                    .GetEvent(
                        "StaticTestEvent",
                        BindingFlags.NonPublic | BindingFlags.Static
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterStaticEventWorks()
        {
            _AssertResolver(
                "E:CodeMap.Tests.TestClass.NestedTestClass`1.StaticTestEvent",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetEvent(
                        "StaticTestEvent",
                        BindingFlags.NonPublic | BindingFlags.Static
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterInsideTestClassWithOneGenericParameterStaticEventWorks()
        {
            _AssertResolver(
                "E:CodeMap.Tests.TestClass`1.NestedTestClass`1.StaticTestEvent",
                typeof(TestClass<>)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetEvent(
                        "StaticTestEvent",
                        BindingFlags.NonPublic | BindingFlags.Static
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassWithOneGenericParameterPropertyWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass`1.TestProperty",
                typeof(TestClass<>)
                    .GetProperty(
                        "TestProperty",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        null,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterPropertyWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass.NestedTestClass`1.TestProperty",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetProperty(
                        "TestProperty",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        null,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterInsideTestClassWithOneGenericParameterPropertyWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass`1.NestedTestClass`1.TestProperty",
                typeof(TestClass<>)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetProperty(
                        "TestProperty",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        null,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassWithOneGenericParameterStaticPropertyWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass`1.StaticTestProperty",
                typeof(TestClass<>)
                    .GetProperty(
                        "StaticTestProperty",
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        null,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterStaticPropertyWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass.NestedTestClass`1.StaticTestProperty",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetProperty(
                        "StaticTestProperty",
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        null,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterInsideTestClassWithOneGenericParameterStaticPropertyWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass`1.NestedTestClass`1.StaticTestProperty",
                typeof(TestClass<>)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetProperty(
                        "StaticTestProperty",
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        null,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassWithOneGenericParameterIndexerWithOneParameterWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass`1.Item(System.String)",
                typeof(TestClass<>)
                    .GetProperty(
                        "Item",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        null,
                        new[] { typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterIndexerWithOneParameterWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass.NestedTestClass`1.Item(System.String)",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetProperty(
                        "Item",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        null,
                        new[] { typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterInsideTestClassWithOneGenericParameterIndexerWithOneParameterWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass`1.NestedTestClass`1.Item(System.String)",
                typeof(TestClass<>)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetProperty(
                        "Item",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        null,
                        new[] { typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassWithOneGenericParameterIndexerWithTwoParametersWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass`1.Item(System.String,System.Int32)",
                typeof(TestClass<>)
                    .GetProperty(
                        "Item",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        null,
                        new[] { typeof(string), typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterIndexerWithTwoParametersWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass.NestedTestClass`1.Item(System.String,System.Int32)",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetProperty(
                        "Item",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        null,
                        new[] { typeof(string), typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterInsideTestClassWithOneGenericParameterIndexerWithTwoParametersWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass`1.NestedTestClass`1.Item(System.String,System.Int32)",
                typeof(TestClass<>)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetProperty(
                        "Item",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        null,
                        new[] { typeof(string), typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassWithOneGenericParameterMethodWithNoParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.TestMethod",
                typeof(TestClass<>)
                    .GetMethod(
                        "TestMethod",
                        0,
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterMethodWithNoParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass`1.TestMethod",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "TestMethod",
                        0,
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterInsideTestClassWithOneGenericParameterMethodWithNoParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.NestedTestClass`1.TestMethod",
                typeof(TestClass<>)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "TestMethod",
                        0,
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassWithOneGenericParameterMethodWithOneParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.TestMethod(System.Int32)",
                typeof(TestClass<>)
                    .GetMethod(
                        "TestMethod",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterMethodWithOneParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass`1.TestMethod(System.Int32)",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "TestMethod",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterInsideTestClassWithOneGenericParameterMethodWithOneParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.NestedTestClass`1.TestMethod(System.Int32)",
                typeof(TestClass<>)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "TestMethod",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassWithOneGenericParameterMethodWithTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.TestMethod(System.Int32,System.String)",
                typeof(TestClass<>)
                    .GetMethod(
                        "TestMethod",
                        0,
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(int), typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterMethodWithTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass`1.TestMethod(System.Int32,System.String)",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "TestMethod",
                        0,
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(int), typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterInsideTestClassWithOneGenericParameterMethodWithTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.NestedTestClass`1.TestMethod(System.Int32,System.String)",
                typeof(TestClass<>)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "TestMethod",
                        0,
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(int), typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassWithOneGenericParameterMethodWithOneGenericParameterAndTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.TestMethod``1(System.Int32,System.String)",
                typeof(TestClass<>)
                    .GetMethod(
                        "TestMethod",
                        1,
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(int), typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterMethodWithOneGenericParameterAndTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass`1.TestMethod``1(System.Int32,System.String)",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "TestMethod",
                        1,
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(int), typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterInsideTestClassWithOneGenericParameterMethodWithOneGenericParameterAndTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.NestedTestClass`1.TestMethod``1(System.Int32,System.String)",
                typeof(TestClass<>)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "TestMethod",
                        1,
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(int), typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassWithOneGenericParameterMethodWithTwoGenericParametersAndNoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.TestMethod``2",
                typeof(TestClass<>)
                    .GetMethod(
                        "TestMethod",
                        2,
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterMethodWithTwoGenericParametersAndNoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass`1.TestMethod``2",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "TestMethod",
                        2,
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterInsideTestClassWithOneGenericParameterMethodWithTwoGenericParametersAndNoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.NestedTestClass`1.TestMethod``2",
                typeof(TestClass<>)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "TestMethod",
                        2,
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassWithOneGenericParameterStaticMethodWithNoParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.StaticTestMethod",
                typeof(TestClass<>)
                    .GetMethod(
                        "StaticTestMethod",
                        0,
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterStaticMethodWithNoParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass`1.StaticTestMethod",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "StaticTestMethod",
                        0,
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterInsideTestClassWithOneGenericParameterStaticMethodWithNoParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.NestedTestClass`1.StaticTestMethod",
                typeof(TestClass<>)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "StaticTestMethod",
                        0,
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassWithOneGenericParameterStaticMethodWithOneParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.StaticTestMethod(System.Int32)",
                typeof(TestClass<>)
                    .GetMethod(
                        "StaticTestMethod",
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        new[] { typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterStaticMethodWithOneParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass`1.StaticTestMethod(System.Int32)",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "StaticTestMethod",
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        new[] { typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterInsideTestClassWithOneGenericParameterStaticMethodWithOneParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.NestedTestClass`1.StaticTestMethod(System.Int32)",
                typeof(TestClass<>)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "StaticTestMethod",
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        new[] { typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassWithOneGenericParameterStaticMethodWithTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.StaticTestMethod(System.Int32,System.String)",
                typeof(TestClass<>)
                    .GetMethod(
                        "StaticTestMethod",
                        0,
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        new[] { typeof(int), typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterStaticMethodWithTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass`1.StaticTestMethod(System.Int32,System.String)",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "StaticTestMethod",
                        0,
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        new[] { typeof(int), typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericInsideTestClassWithOneGenericParameterParameterStaticMethodWithTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.NestedTestClass`1.StaticTestMethod(System.Int32,System.String)",
                typeof(TestClass<>)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "StaticTestMethod",
                        0,
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        new[] { typeof(int), typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassWithOneGenericParameterStaticMethodWithOneGenericParameterAndTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.StaticTestMethod``1(System.Int32,System.String)",
                typeof(TestClass<>)
                    .GetMethod(
                        "StaticTestMethod",
                        1,
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        new[] { typeof(int), typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterStaticMethodWithOneGenericParameterAndTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass`1.StaticTestMethod``1(System.Int32,System.String)",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "StaticTestMethod",
                        1,
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        new[] { typeof(int), typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterInsideTestClassWithOneGenericParameterStaticMethodWithOneGenericParameterAndTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.NestedTestClass`1.StaticTestMethod``1(System.Int32,System.String)",
                typeof(TestClass<>)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "StaticTestMethod",
                        1,
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        new[] { typeof(int), typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassWithOneGenericParameterStaticMethodWithTwoGenericParametersAndNoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.StaticTestMethod``2",
                typeof(TestClass<>)
                    .GetMethod(
                        "StaticTestMethod",
                        2,
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterStaticMethodWithTwoGenericParametersAndNoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestClass`1.StaticTestMethod``2",
                typeof(TestClass)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "StaticTestMethod",
                        2,
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestClassWithOneGenericParameterInsideTestClassWithOneGenericParameterStaticMethodWithTwoGenericParametersAndNoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.NestedTestClass`1.StaticTestMethod``2",
                typeof(TestClass<>)
                    .GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "StaticTestMethod",
                        2,
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestStructWorks()
        {
            _AssertResolver(
                "T:CodeMap.Tests.TestStruct",
                typeof(TestStruct)
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestStructWorks()
        {
            _AssertResolver(
                "T:CodeMap.Tests.TestClass.NestedTestStruct",
                typeof(TestClass).GetNestedType("NestedTestStruct", BindingFlags.NonPublic | BindingFlags.Public)
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestStructFieldWorks()
        {
            _AssertResolver(
                "F:CodeMap.Tests.TestStruct._testField",
                typeof(TestStruct)
                    .GetField(
                        "_testField",
                        BindingFlags.NonPublic | BindingFlags.Instance
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestStructFieldWorks()
        {
            _AssertResolver(
                "F:CodeMap.Tests.TestClass.NestedTestStruct._testField",
                typeof(TestClass)
                    .GetNestedType("NestedTestStruct", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetField(
                        "_testField",
                        BindingFlags.NonPublic | BindingFlags.Instance
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestStructConstructorWithOneParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestStruct.#ctor(System.Double)",
                typeof(TestStruct)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(double) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestStructConstructorWithOneParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestStruct.#ctor(System.Double)",
                typeof(TestClass)
                    .GetNestedType("NestedTestStruct", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(double) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestStructConstructorWithTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestStruct.#ctor(System.Double,System.Int32)",
                typeof(TestStruct)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(double), typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestStructConstructorWithTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestStruct.#ctor(System.Double,System.Int32)",
                typeof(TestClass)
                    .GetNestedType("NestedTestStruct", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(double), typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestStructStaticConstructorWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestStruct.#cctor",
                typeof(TestStruct)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestStructStaticConstructorWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestStruct.#cctor",
                typeof(TestClass)
                    .GetNestedType("NestedTestStruct", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestStructEventWorks()
        {
            _AssertResolver(
                "E:CodeMap.Tests.TestStruct.TestEvent",
                typeof(TestStruct)
                    .GetEvent(
                        "TestEvent",
                        BindingFlags.NonPublic | BindingFlags.Instance
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestStructEventWorks()
        {
            _AssertResolver(
                "E:CodeMap.Tests.TestClass.NestedTestStruct.TestEvent",
                typeof(TestClass)
                    .GetNestedType("NestedTestStruct", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetEvent(
                        "TestEvent",
                        BindingFlags.NonPublic | BindingFlags.Instance
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestStructStaticEventWorks()
        {
            _AssertResolver(
                "E:CodeMap.Tests.TestStruct.StaticTestEvent",
                typeof(TestStruct)
                    .GetEvent(
                        "StaticTestEvent",
                        BindingFlags.NonPublic | BindingFlags.Static
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestStructStaticEventWorks()
        {
            _AssertResolver(
                "E:CodeMap.Tests.TestClass.NestedTestStruct.StaticTestEvent",
                typeof(TestClass)
                    .GetNestedType("NestedTestStruct", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetEvent(
                        "StaticTestEvent",
                        BindingFlags.NonPublic | BindingFlags.Static
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestStructPropertyWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestStruct.TestProperty",
                typeof(TestStruct)
                    .GetProperty(
                        "TestProperty",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        null,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestStructPropertyWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass.NestedTestStruct.TestProperty",
                typeof(TestClass)
                    .GetNestedType("NestedTestStruct", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetProperty(
                        "TestProperty",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        null,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestStructStaticPropertyWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestStruct.StaticTestProperty",
                typeof(TestStruct)
                    .GetProperty(
                        "StaticTestProperty",
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        null,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestStructStaticPropertyWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass.NestedTestStruct.StaticTestProperty",
                typeof(TestClass)
                    .GetNestedType("NestedTestStruct", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetProperty(
                        "StaticTestProperty",
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        null,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestStructIndexerWithOneParameterWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestStruct.Item(System.String)",
                typeof(TestStruct)
                    .GetProperty(
                        "Item",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        null,
                        new[] { typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestStructIndexerWithOneParameterWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass.NestedTestStruct.Item(System.String)",
                typeof(TestClass)
                    .GetNestedType("NestedTestStruct", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetProperty(
                        "Item",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        null,
                        new[] { typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestStructIndexerWithTwoParametersWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestStruct.Item(System.String,System.Int32)",
                typeof(TestStruct)
                    .GetProperty(
                        "Item",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        null,
                        new[] { typeof(string), typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestStructIndexerWithTwoParametersWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass.NestedTestStruct.Item(System.String,System.Int32)",
                typeof(TestClass)
                    .GetNestedType("NestedTestStruct", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetProperty(
                        "Item",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        null,
                        new[] { typeof(string), typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestStructMethodWithNoParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestStruct.TestMethod",
                typeof(TestStruct)
                    .GetMethod(
                        "TestMethod",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestStructMethodWithNoParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestStruct.TestMethod",
                typeof(TestClass)
                    .GetNestedType("NestedTestStruct", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "TestMethod",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestStructMethodWithOneParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestStruct.TestMethod(System.Int32)",
                typeof(TestStruct)
                    .GetMethod(
                        "TestMethod",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestStructMethodWithOneParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestStruct.TestMethod(System.Int32)",
                typeof(TestClass)
                    .GetNestedType("NestedTestStruct", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "TestMethod",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestStructMethodWithTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestStruct.TestMethod(System.Int32,System.String)",
                typeof(TestStruct)
                    .GetMethod(
                        "TestMethod",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(int), typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedStructClassMethodWithTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestStruct.TestMethod(System.Int32,System.String)",
                typeof(TestClass)
                    .GetNestedType("NestedTestStruct", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "TestMethod",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(int), typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestStructStaticMethodWithNoParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestStruct.StaticTestMethod",
                typeof(TestStruct)
                    .GetMethod(
                        "StaticTestMethod",
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestStructStaticMethodWithNoParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestStruct.StaticTestMethod",
                typeof(TestClass)
                    .GetNestedType("NestedTestStruct", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "StaticTestMethod",
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        Type.EmptyTypes,
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestStructStaticMethodWithOneParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestStruct.StaticTestMethod(System.Int32)",
                typeof(TestStruct)
                    .GetMethod(
                        "StaticTestMethod",
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        new[] { typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestStructStaticMethodWithOneParameterWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestStruct.StaticTestMethod(System.Int32)",
                typeof(TestClass)
                    .GetNestedType("NestedTestStruct", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "StaticTestMethod",
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        new[] { typeof(int) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestStructStaticMethodWithTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestStruct.StaticTestMethod(System.Int32,System.String)",
                typeof(TestStruct)
                    .GetMethod(
                        "StaticTestMethod",
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        new[] { typeof(int), typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTestStructStaticMethodWithTwoParametersWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.NestedTestStruct.StaticTestMethod(System.Int32,System.String)",
                typeof(TestClass)
                    .GetNestedType("NestedTestStruct", BindingFlags.NonPublic | BindingFlags.Public)
                    .GetMethod(
                        "StaticTestMethod",
                        BindingFlags.NonPublic | BindingFlags.Static,
                        Type.DefaultBinder,
                        new[] { typeof(int), typeof(string) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTypeInGlobalNamespaceWorks()
        {
            _AssertResolver(
                "T:GlobalTestClass",
                typeof(GlobalTestClass)
            );
        }

        [Fact]
        public void GettingCanonicalNameForNestedTypeInGlobalNamespaceWorks()
        {
            _AssertResolver(
                "T:GlobalTestClass.NestedTestClass",
                typeof(GlobalTestClass).GetNestedType("NestedTestClass", BindingFlags.NonPublic | BindingFlags.Public)
            );
        }

        [Fact]
        public void GettingCanonicalNameForIndexerUsingGenericParametersFromTypeWorks()
        {
            _AssertResolver(
                "P:CodeMap.Tests.TestClass`1.Item(`0)",
                typeof(TestClass<>)
                    .GetProperty(
                        "Item",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        null,
                        new[] { typeof(TestClass<>).GetGenericArguments()[0] },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForGenericMethodUsingGenericParametersFromMethodWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass.TestMethod``1(``0)",
                typeof(TestClass)
                    .GetMethod(
                        "TestMethod",
                        1,
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { Type.MakeGenericMethodParameter(0) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForGenericMethodUsingGenericParametersFromMethodAndTypeWorks()
        {
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.TestMethod``1(`0,``0)",
                typeof(TestClass<>)
                    .GetMethod(
                        "TestMethod",
                        1,
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(TestClass<>).GetGenericArguments()[0], Type.MakeGenericMethodParameter(0) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForGenericMethodUsingGenericParametersFromMethodTypeAndNestedTypeWorks()
        {
            var nestedType = typeof(TestClass<>).GetNestedType("NestedTestClass`1", BindingFlags.NonPublic | BindingFlags.Public);
            var nestedTypeGenericArguments = nestedType.GetGenericArguments();
            _AssertResolver(
                "M:CodeMap.Tests.TestClass`1.NestedTestClass`1.TestMethod``1(`0,`1,``0)",
                nestedType
                    .GetMethod(
                        "TestMethod",
                        1,
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[]
                        {
                            nestedTypeGenericArguments[0],
                            nestedTypeGenericArguments[1],
                            Type.MakeGenericMethodParameter(0)
                        },
                        null
                    )
            );
        }

        [Theory]
        [InlineData("F:TestField")]
        [InlineData("M:#ctor")]
        [InlineData("E:TestEvent")]
        [InlineData("P:TestProperty")]
        [InlineData("T:TestType")]
        [InlineData("M:TestMethod")]
        public void TryingToGetMemberInfoFromCanonicalNameThatDoesNotResolveToOneReturnsNull(string canonicalName)
        {
            var memberInfo = _CanonicalNameResolver.TryFindMemberInfoFor(canonicalName);
            Assert.Null(memberInfo);
        }

        [Fact]
        public void GettingCanonicalNameForTestClassTestMethodWithParameterBeingGenericParameter()
        {
            _AssertResolver(
                "M:CodeMap.Tests.Test.TestMethod(System.Collections.Generic.IReadOnlyDictionary{System.Int32,System.Collections.Generic.IEnumerable{System.String}},System.Collections.Generic.IEnumerable{System.Double})",
                typeof(Test)
                    .GetMethod(
                        "TestMethod",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(IReadOnlyDictionary<int, IEnumerable<string>>), typeof(IEnumerable<double>) },
                        null
                    )
            );
        }

        [Fact]
        public void GettingCanonicalNameForTestClassTestMethodWithByRefParameter()
        {
            _AssertResolver(
                "M:CodeMap.Tests.Test.TestMethod(System.Int32@,System.String@)",
                typeof(Test)
                    .GetMethod(
                        "TestMethod",
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        Type.DefaultBinder,
                        new[] { typeof(int).MakeByRefType(), typeof(string).MakeByRefType() },
                        null
                    )
            );
        }

        private static void _AssertResolver(string canonicalName, MemberInfo memberInfo)
        {
            var actualCanonicalName = _CanonicalNameResolver.GetCanonicalNameFrom(memberInfo);
            Assert.Equal(canonicalName, actualCanonicalName);

            var actualMemberInfo = _CanonicalNameResolver.TryFindMemberInfoFor(canonicalName.ToLowerInvariant());
            Assert.Equal(memberInfo, actualMemberInfo);
        }
    }
}