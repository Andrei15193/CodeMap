using CodeMap.DocumentationElements;
using CodeMap.Tests.Data;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Xunit;

namespace CodeMap.Tests.DocumentationElements
{
    public class CanonicalNameResolverTests
    {
        private static CanonicalNameResolver _CanonicalNameResolver { get; } = new CanonicalNameResolver(
                new[] { typeof(TestAttribute).Assembly }
                    .Concat(
                        typeof(TestAttribute)
                            .Assembly
                            .GetReferencedAssemblies()
                            .Select(Assembly.Load)
                    )
            );

        private static readonly XDocument _xmlDocumentation = XDocument.Parse(
            File.ReadAllText(
                Path.ChangeExtension(typeof(TestAttribute).Assembly.Location, ".xml")
            )
        );

        public const string IndexerParameters =
            "System.Int32," +
            "System.Byte[]," +
            "System.Char[][]," +
            "System.Double[0:,0:]," +
            "CodeMap.Tests.Data.TestClass{System.Int32}.NestedTestClass{System.Byte[],System.Collections.Generic.IEnumerable{System.String}}," +
            "CodeMap.Tests.Data.TestClass{System.Int32}.NestedTestClass{System.Byte[],System.Collections.Generic.IEnumerable{System.String}}[]," +
            "System.Object," +
            "System.Int32*," +
            "System.Byte*[]," +
            "System.Void*," +
            "System.Void**," +
            "System.Void**[]," +
            "`0," +
            "System.String";
        public const string ConstructorParameters =
            "System.Int32," +
            "System.Byte[]," +
            "System.Char[][]," +
            "System.Double[0:,0:]," +
            "System.Int32@," +
            "System.Byte[]@," +
            "System.Char[][]@," +
            "System.Double[0:,0:]@," +
            "System.Int32@," +
            "System.Byte[]@," +
            "System.Char[][]@," +
            "System.Double[0:,0:]@," +
            "CodeMap.Tests.Data.TestClass{System.Int32}.NestedTestClass{System.Byte[],System.Collections.Generic.IEnumerable{System.String}}," +
            "CodeMap.Tests.Data.TestClass{System.Int32}.NestedTestClass{System.Byte[],System.Collections.Generic.IEnumerable{System.String}}[]," +
            "CodeMap.Tests.Data.TestClass{System.Int32}.NestedTestClass{System.Byte[],System.Collections.Generic.IEnumerable{System.String}}@," +
            "CodeMap.Tests.Data.TestClass{System.Int32}.NestedTestClass{System.Byte[],System.Collections.Generic.IEnumerable{System.String}}@," +
            "CodeMap.Tests.Data.TestClass{System.Int32}.NestedTestClass{System.Byte[],System.Collections.Generic.IEnumerable{System.String}}[]@," +
            "CodeMap.Tests.Data.TestClass{System.Int32}.NestedTestClass{System.Byte[],System.Collections.Generic.IEnumerable{System.String}}[]@," +
            "System.Object," +
            "System.Object@," +
            "System.Object@," +
            "`0," +
            "`0@," +
            "`0@," +
            "System.Int32*," +
            "System.Byte*[]," +
            "System.Char*@," +
            "System.Double*@," +
            "System.Decimal*[]@," +
            "System.Int16*[]@," +
            "System.Void*," +
            "System.Void**," +
            "System.Void**@," +
            "System.Void**@," +
            "System.Void**[]," +
            "System.Void**[]@," +
            "System.Void**[]@," +
            "System.String";
        public const string MethodParameters =
            "System.Int32," +
            "System.Byte[]," +
            "System.Char[][]," +
            "System.Double[0:,0:]," +
            "System.Int32@," +
            "System.Byte[]@," +
            "System.Char[][]@," +
            "System.Double[0:,0:]@," +
            "System.Int32@," +
            "System.Byte[]@," +
            "System.Char[][]@," +
            "System.Double[0:,0:]@," +
            "CodeMap.Tests.Data.TestClass{System.Int32}.NestedTestClass{System.Byte[],System.Collections.Generic.IEnumerable{System.String}}," +
            "CodeMap.Tests.Data.TestClass{System.Int32}.NestedTestClass{System.Byte[],System.Collections.Generic.IEnumerable{System.String}}[]," +
            "CodeMap.Tests.Data.TestClass{System.Int32}.NestedTestClass{System.Byte[],System.Collections.Generic.IEnumerable{System.String}}@," +
            "CodeMap.Tests.Data.TestClass{System.Int32}.NestedTestClass{System.Byte[],System.Collections.Generic.IEnumerable{System.String}}@," +
            "CodeMap.Tests.Data.TestClass{System.Int32}.NestedTestClass{System.Byte[],System.Collections.Generic.IEnumerable{System.String}}[]@," +
            "CodeMap.Tests.Data.TestClass{System.Int32}.NestedTestClass{System.Byte[],System.Collections.Generic.IEnumerable{System.String}}[]@," +
            "System.Object," +
            "System.Object@," +
            "System.Object@," +
            "`0," +
            "`0@," +
            "`0@," +
            "System.Int32*," +
            "System.Byte*[]," +
            "System.Char*@," +
            "System.Double*@," +
            "System.Decimal*[]@," +
            "System.Int16*[]@," +
            "System.Void*," +
            "System.Void**," +
            "System.Void**@," +
            "System.Void**@," +
            "System.Void**[]," +
            "System.Void**[]@," +
            "System.Void**[]@," +
            "``0," +
            "``0@," +
            "``0@," +
            "System.String";

        [Theory]
        [InlineData("T:CodeMap.Tests.Data.TestEnum", typeof(TestEnum))]
        [InlineData("T:CodeMap.Tests.Data.TestDelegate`1", typeof(TestDelegate<>))]
        [InlineData("T:CodeMap.Tests.Data.ITestBaseInterface", typeof(ITestBaseInterface))]
        [InlineData("T:CodeMap.Tests.Data.ITestExtendedBaseInterface", typeof(ITestExtendedBaseInterface))]
        [InlineData("T:CodeMap.Tests.Data.ITestInterface`1", typeof(ITestInterface<>))]
        [InlineData("T:CodeMap.Tests.Data.TestBaseClass", typeof(TestBaseClass))]
        [InlineData("T:CodeMap.Tests.Data.TestClass`1", typeof(TestClass<>))]
        [InlineData("T:CodeMap.Tests.Data.TestClass`1.NestedTestEnum", typeof(TestClass<>.NestedTestEnum))]
        [InlineData("T:CodeMap.Tests.Data.TestClass`1.NestedTestDelegate", typeof(TestClass<>.NestedTestDelegate))]
        [InlineData("T:CodeMap.Tests.Data.TestClass`1.INestedTestInterface", typeof(TestClass<>.INestedTestInterface))]
        [InlineData("T:CodeMap.Tests.Data.TestClass`1.NestedTestClass`2", typeof(TestClass<>.NestedTestClass<,>))]
        [InlineData("T:CodeMap.Tests.Data.TestClass`1.NestedTestStruct", typeof(TestClass<>.NestedTestStruct))]
        [InlineData("T:CodeMap.Tests.Data.TestSealedClass", typeof(TestSealedClass))]
        [InlineData("T:CodeMap.Tests.Data.TestStruct`1", typeof(TestStruct<>))]
        [InlineData("T:GlobalTestClass", typeof(GlobalTestClass))]
        [InlineData("T:CodeMap.Tests.Data.ITestGenericParameter`6", typeof(ITestGenericParameter<,,,,,>))]
        [InlineData("T:CodeMap.Tests.Data.TestAttribute", typeof(TestAttribute))]
        public void TypeCanonicalNameResolution(string canonicalName, Type type)
        {
            _AssertResolver(canonicalName, type);
        }

        [Theory]
        [InlineData("M:CodeMap.Tests.Data.TestAttribute.#ctor(System.Object)", typeof(TestAttribute))]
        [InlineData("M:CodeMap.Tests.Data.TestClass`1.#ctor(" + ConstructorParameters + ")", typeof(TestClass<>))]
        public void TestConstructorCanonicalNameResolution(string canonicalName, Type declaringType)
        {
            var constructor = declaringType
                .GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Single();
            _AssertResolver(canonicalName, constructor);
        }

        [Theory]
        [InlineData("M:CodeMap.Tests.Data.TestClass`1.#cctor", typeof(TestClass<>))]
        public void TestStaticConstructorCanonicalNameResolution(string canonicalName, Type declaringType)
        {
            var constructor = declaringType
                .GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Single();
            _AssertResolver(canonicalName, constructor);
        }

        [Theory]
        [InlineData("F:CodeMap.Tests.Data.TestEnum.TestMember1", typeof(TestEnum), "TestMember1")]
        [InlineData("F:CodeMap.Tests.Data.TestEnum.TestMember2", typeof(TestEnum), "TestMember2")]
        [InlineData("F:CodeMap.Tests.Data.TestEnum.TestMember3", typeof(TestEnum), "TestMember3")]

        [InlineData("P:CodeMap.Tests.Data.ITestBaseInterface.InterfaceShadowedTestProperty", typeof(ITestBaseInterface), "InterfaceShadowedTestProperty")]
        [InlineData("M:CodeMap.Tests.Data.ITestBaseInterface.BaseTestMethod", typeof(ITestBaseInterface), "BaseTestMethod")]
        [InlineData("M:CodeMap.Tests.Data.ITestBaseInterface.InterfaceShadowedTestMethod", typeof(ITestBaseInterface), "InterfaceShadowedTestMethod")]

        [InlineData("E:CodeMap.Tests.Data.ITestInterface`1.TestEvent", typeof(ITestInterface<>), "TestEvent")]
        [InlineData("E:CodeMap.Tests.Data.ITestInterface`1.InterfaceShadowedTestEvent", typeof(ITestInterface<>), "InterfaceShadowedTestEvent")]
        [InlineData("P:CodeMap.Tests.Data.ITestInterface`1.TestProperty", typeof(ITestInterface<>), "TestProperty")]
        [InlineData("P:CodeMap.Tests.Data.ITestInterface`1.InterfaceShadowedTestProperty", typeof(ITestInterface<>), "InterfaceShadowedTestProperty")]
        [InlineData("P:CodeMap.Tests.Data.ITestInterface`1.Item(" + IndexerParameters + ")", typeof(ITestInterface<>), "Item")]
        [InlineData("M:CodeMap.Tests.Data.ITestInterface`1.InterfaceShadowedTestMethod", typeof(ITestInterface<>), "InterfaceShadowedTestMethod")]
        [InlineData("M:CodeMap.Tests.Data.ITestInterface`1.TestMethod``1(" + MethodParameters + ")", typeof(ITestInterface<>), "TestMethod")]

        [InlineData("P:CodeMap.Tests.Data.TestAttribute.Value1", typeof(TestAttribute), "Value1")]
        [InlineData("P:CodeMap.Tests.Data.TestAttribute.Value2", typeof(TestAttribute), "Value2")]
        [InlineData("F:CodeMap.Tests.Data.TestAttribute.Value3", typeof(TestAttribute), "Value3")]

        [InlineData("F:CodeMap.Tests.Data.TestBaseClass.ShadowedTestField", typeof(TestBaseClass), "ShadowedTestField")]
        [InlineData("P:CodeMap.Tests.Data.TestBaseClass.StaticTestProperty", typeof(TestBaseClass), "StaticTestProperty")]
        [InlineData("P:CodeMap.Tests.Data.TestBaseClass.ClassShadowedTestProperty", typeof(TestBaseClass), "ClassShadowedTestProperty")]
        [InlineData("P:CodeMap.Tests.Data.TestBaseClass.AbstractTestProperty", typeof(TestBaseClass), "AbstractTestProperty")]
        [InlineData("P:CodeMap.Tests.Data.TestBaseClass.VirtualTestProperty", typeof(TestBaseClass), "VirtualTestProperty")]
        [InlineData("M:CodeMap.Tests.Data.TestBaseClass.StaticTestMethod", typeof(TestBaseClass), "StaticTestMethod")]
        [InlineData("M:CodeMap.Tests.Data.TestBaseClass.ClassShadowedTestMethod", typeof(TestBaseClass), "ClassShadowedTestMethod")]
        [InlineData("M:CodeMap.Tests.Data.TestBaseClass.AbstractTestMethod", typeof(TestBaseClass), "AbstractTestMethod")]
        [InlineData("M:CodeMap.Tests.Data.TestBaseClass.VirtualTestMethod", typeof(TestBaseClass), "VirtualTestMethod")]

        [InlineData("F:CodeMap.Tests.Data.TestClass`1.TestConstant", typeof(TestClass<>), "TestConstant")]
        [InlineData("F:CodeMap.Tests.Data.TestClass`1.TestField", typeof(TestClass<>), "TestField")]
        [InlineData("F:CodeMap.Tests.Data.TestClass`1.ReadonlyTestField", typeof(TestClass<>), "ReadonlyTestField")]
        [InlineData("F:CodeMap.Tests.Data.TestClass`1.StaticTestField", typeof(TestClass<>), "StaticTestField")]
        [InlineData("F:CodeMap.Tests.Data.TestClass`1.ShadowedTestField", typeof(TestClass<>), "ShadowedTestField")]
        [InlineData("E:CodeMap.Tests.Data.TestClass`1.TestEvent", typeof(TestClass<>), "TestEvent")]
        [InlineData("E:CodeMap.Tests.Data.TestClass`1.ClassShadowedTestEvent", typeof(TestClass<>), "ClassShadowedTestEvent")]
        [InlineData("P:CodeMap.Tests.Data.TestClass`1.AbstractTestProperty", typeof(TestClass<>), "AbstractTestProperty")]
        [InlineData("P:CodeMap.Tests.Data.TestClass`1.VirtualTestProperty", typeof(TestClass<>), "VirtualTestProperty")]
        [InlineData("P:CodeMap.Tests.Data.TestClass`1.InterfaceShadowedTestProperty", typeof(TestClass<>), "InterfaceShadowedTestProperty")]
        [InlineData("P:CodeMap.Tests.Data.TestClass`1.Item(" + IndexerParameters + ")", typeof(TestClass<>), "Item")]
        [InlineData("P:CodeMap.Tests.Data.TestClass`1.TestProperty", typeof(TestClass<>), "TestProperty")]
        [InlineData("M:CodeMap.Tests.Data.TestClass`1.ClassShadowedTestMethod", typeof(TestClass<>), "ClassShadowedTestMethod")]
        [InlineData("M:CodeMap.Tests.Data.TestClass`1.AbstractTestMethod", typeof(TestClass<>), "AbstractTestMethod")]
        [InlineData("M:CodeMap.Tests.Data.TestClass`1.VirtualTestMethod", typeof(TestClass<>), "VirtualTestMethod")]
        [InlineData("M:CodeMap.Tests.Data.TestClass`1.TestMethod``1(" + MethodParameters + ")", typeof(TestClass<>), "TestMethod")]

        [InlineData("F:CodeMap.Tests.Data.TestStruct`1.TestConstant", typeof(TestStruct<>), "TestConstant")]
        [InlineData("F:CodeMap.Tests.Data.TestStruct`1.TestField", typeof(TestStruct<>), "TestField")]
        [InlineData("F:CodeMap.Tests.Data.TestStruct`1.ReadonlyTestField", typeof(TestStruct<>), "ReadonlyTestField")]
        [InlineData("F:CodeMap.Tests.Data.TestStruct`1.StaticTestField", typeof(TestStruct<>), "StaticTestField")]
        [InlineData("E:CodeMap.Tests.Data.TestStruct`1.TestEvent", typeof(TestStruct<>), "TestEvent")]
        [InlineData("P:CodeMap.Tests.Data.TestStruct`1.TestProperty", typeof(TestStruct<>), "TestProperty")]
        [InlineData("P:CodeMap.Tests.Data.TestStruct`1.Item(" + IndexerParameters + ")", typeof(TestStruct<>), "Item")]
        [InlineData("M:CodeMap.Tests.Data.TestStruct`1.ToString", typeof(TestStruct<>), "ToString")]
        [InlineData("M:CodeMap.Tests.Data.TestStruct`1.GetHashCode", typeof(TestStruct<>), "GetHashCode")]
        [InlineData("M:CodeMap.Tests.Data.TestStruct`1.TestMethod``1(" + MethodParameters + ")", typeof(TestStruct<>), "TestMethod")]

        [InlineData("E:CodeMap.Tests.Data.TestExplicitClass.CodeMap#Tests#Data#ITestExplicitInterface#TestEvent", typeof(TestExplicitClass), "CodeMap.Tests.Data.ITestExplicitInterface.TestEvent")]
        [InlineData("P:CodeMap.Tests.Data.TestExplicitClass.CodeMap#Tests#Data#ITestExplicitInterface#TestProperty", typeof(TestExplicitClass), "CodeMap.Tests.Data.ITestExplicitInterface.TestProperty")]
        [InlineData("M:CodeMap.Tests.Data.TestExplicitClass.CodeMap#Tests#Data#ITestExplicitInterface#TestMethod", typeof(TestExplicitClass), "CodeMap.Tests.Data.ITestExplicitInterface.TestMethod")]
        public void TestMemberCanonicalNameResolution(string canonicalName, Type declaringType, string memberName)
        {
            var members = declaringType
                .GetMember(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            switch (members.Length)
            {
                case 2:
                    var eventInfo = members.OfType<EventInfo>().Single();
                    var fieldInfo = members.OfType<FieldInfo>().Single();
                    Assert.Equal(eventInfo.EventHandlerType, fieldInfo.FieldType);
                    _AssertResolver(canonicalName, eventInfo);
                    break;

                default:
                    _AssertResolver(canonicalName, members.Single());
                    break;
            }

        }

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

        private static void _AssertResolver(string canonicalName, MemberInfo memberInfo)
        {
            Assert.True(
                _xmlDocumentation
                    .Root
                    .Element("members")
                    .Elements("member")
                    .Any(element => string.Equals(element.Attribute("name")?.Value, canonicalName, StringComparison.OrdinalIgnoreCase)),
                $"Expected {canonicalName} in generated XML documentation file, none found."
            );

            var actualCanonicalName = _CanonicalNameResolver.GetCanonicalNameFrom(memberInfo);
            Assert.Equal(canonicalName, actualCanonicalName);

            var actualMemberInfo = _CanonicalNameResolver.TryFindMemberInfoFor(canonicalName.ToLowerInvariant());
            Assert.Equal(memberInfo, actualMemberInfo);
        }
    }
}