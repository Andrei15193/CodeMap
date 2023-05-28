using CodeMap.DeclarationNodes;
using CodeMap.Html;
using System.Linq;
using Xunit;

namespace CodeMap.Tests.Html
{
    public class DeclarationNodeExtensions_GetFullNameReferenceTests
    {
        [Fact]
        public void GettingFullNameReferenceForAssemblyDeclarationReturnsName()
        {
            var assemblyDeclaration = DeclarationNode.Create(typeof(GlobalTestClass).Assembly);

            var fullNameReference = assemblyDeclaration.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForNamespaceDeclarationReturnsName()
        {
            var namespaceDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly)
                .Namespaces
                .Single(@namespace => @namespace.Name == "CodeMap.Tests.Data");

            var fullNameReference = namespaceDeclaration.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForGlobalNamespaceDeclarationReturnsEmptyName()
        {
            var namespaceDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly)
                .Namespaces
                .OfType<GlobalNamespaceDeclaration>()
                .Single();

            var fullNameReference = namespaceDeclaration.GetFullNameReference();

            Assert.Empty(fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForGlobalTypeDeclarationReturnsName()
        {
            var typeDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly)
                .Namespaces
                .OfType<GlobalNamespaceDeclaration>()
                .Single()
                .DeclaredTypes
                .Single();

            var fullNameReference = typeDeclaration.GetFullNameReference();

            Assert.Equal("GlobalTestClass", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForTypeDeclarationReturnsNamespaceNameAndOwnName()
        {
            var typeDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly)
                .Namespaces
                .Single(@namespace => @namespace.Name == "CodeMap.Tests.Data")
                .DeclaredTypes
                .Single(type => type.Name == "TestEnum");

            var fullNameReference = typeDeclaration.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestEnum", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForGenericInterfaceDeclarationReturnsNamespaceNameOwnNameAndGenericParameterNames()
        {
            var typeDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly)
                .Namespaces
                .Single(@namespace => @namespace.Name == "CodeMap.Tests.Data")
                .DeclaredTypes
                .Single(type => type.Name == "ITestGenericParameter");

            var fullNameReference = typeDeclaration.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.ITestGenericParameter<TParam1,TParam2,TParam3,TParam4,TParam5,TParam6,TParam7>", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForGenericClassDeclarationReturnsNamespaceNameOwnNameAndGenericParameterNames()
        {
            var typeDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly)
                .Namespaces
                .Single(@namespace => @namespace.Name == "CodeMap.Tests.Data")
                .DeclaredTypes
                .Single(type => type.Name == "TestClass");

            var fullNameReference = typeDeclaration.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestClass<TParam>", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForGenericRecordDeclarationReturnsNamespaceNameOwnNameAndGenericParameterNames()
        {
            var typeDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly)
                .Namespaces
                .Single(@namespace => @namespace.Name == "CodeMap.Tests.Data")
                .DeclaredTypes
                .Single(type => type.Name == "TestRecord");

            var fullNameReference = typeDeclaration.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestRecord<TParam>", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForGenericStructDeclarationReturnsNamespaceNameOwnNameAndGenericParameterNames()
        {
            var typeDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly)
                .Namespaces
                .Single(@namespace => @namespace.Name == "CodeMap.Tests.Data")
                .DeclaredTypes
                .Single(type => type.Name == "TestStruct");

            var fullNameReference = typeDeclaration.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestStruct<TParam>", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForConstantDeclarationReturnsNamespaceNameDeclaringTypeNameAndOwnName()
        {
            var constantDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly)
                .Namespaces
                .Single(@namespace => @namespace.Name == "CodeMap.Tests.Data")
                .DeclaredTypes
                .OfType<ClassDeclaration>()
                .Single(type => type.Name == "TestClass")
                .Constants
                .Single(constant => constant.Name == "ClassShadowedTestConstant");

            var fullNameReference = constantDeclaration.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestClass<TParam>.ClassShadowedTestConstant", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForFieldDeclarationReturnsNamespaceNameDeclaringTypeNameAndOwnName()
        {
            var fieldDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly, DeclarationFilter.All)
                .Namespaces
                .Single(@namespace => @namespace.Name == "CodeMap.Tests.Data")
                .DeclaredTypes
                .OfType<ClassDeclaration>()
                .Single(type => type.Name == "TestClass")
                .Fields
                .Single(field => field.Name == "StaticTestField");

            var fullNameReference = fieldDeclaration.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestClass<TParam>.StaticTestField", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForConstructorDeclarationReturnsNamespaceNameDeclaringTypeNameOwnNameAndParameterTypeNames()
        {
            var constructorDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly, DeclarationFilter.All)
                .Namespaces
                .Single(@namespace => @namespace.Name == "CodeMap.Tests.Data")
                .DeclaredTypes
                .OfType<ClassDeclaration>()
                .Single(type => type.Name == "TestClass")
                .Constructors
                .Single();

            var fullNameReference = constructorDeclaration.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestClass<TParam>.TestClass(System.Int32)", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForEventDeclarationReturnsNamespaceNameDeclaringTypeNameAndOwnName()
        {
            var eventDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly, DeclarationFilter.All)
                .Namespaces
                .Single(@namespace => @namespace.Name == "CodeMap.Tests.Data")
                .DeclaredTypes
                .OfType<ClassDeclaration>()
                .Single(type => type.Name == "TestClass")
                .Events
                .Single(@event => @event.Name == "AbstractTestEvent");

            var fullNameReference = eventDeclaration.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestClass<TParam>.AbstractTestEvent", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForPropertyDeclarationReturnsNamespaceNameDeclaringTypeNameAndOwnName()
        {
            var propertyDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly, DeclarationFilter.All)
                .Namespaces
                .Single(@namespace => @namespace.Name == "CodeMap.Tests.Data")
                .DeclaredTypes
                .OfType<ClassDeclaration>()
                .Single(type => type.Name == "TestClass")
                .Properties
                .Single(property => property.Name == "InterfaceShadowedTestProperty");

            var fullNameReference = propertyDeclaration.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestClass<TParam>.InterfaceShadowedTestProperty", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForIndexPropertyDeclarationReturnsNamespaceNameDeclaringTypeNameOwnNameAndParameterTypeNames()
        {
            var propertyDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly, DeclarationFilter.All)
                .Namespaces
                .Single(@namespace => @namespace.Name == "CodeMap.Tests.Data")
                .DeclaredTypes
                .OfType<ClassDeclaration>()
                .Single(type => type.Name == "TestClass")
                .Properties
                .Single(property => property.Name == "Item");

            var fullNameReference = propertyDeclaration.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestClass<TParam>.Item[System.Int32]", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForMethodDeclarationReturnsNamespaceNameDeclaringTypeNameOwnNameAndParameterTypeNames()
        {
            var methodDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly, DeclarationFilter.All)
                .Namespaces
                .Single(@namespace => @namespace.Name == "CodeMap.Tests.Data")
                .DeclaredTypes
                .OfType<ClassDeclaration>()
                .Single(type => type.Name == "TestClass")
                .Methods
                .Single(method => method.Name == "TestMethod");

            var fullNameReference = methodDeclaration.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestClass<TParam>.TestMethod(System.Int32,System.String)", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForGenericMethodDeclarationReturnsNamespaceNameDeclaringTypeNameOwnNameGenericParameterNamesAndParameterTypeNames()
        {
            var methodDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly, DeclarationFilter.All)
                .Namespaces
                .Single(@namespace => @namespace.Name == "CodeMap.Tests.Data")
                .DeclaredTypes
                .OfType<ClassDeclaration>()
                .Single(type => type.Name == "TestClass")
                .Methods
                .Single(method => method.Name == "TestMethod38");

            var fullNameReference = methodDeclaration.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestClass<TParam>.TestMethod38<TMethodParam>(TMethodParam)", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForNestedEnumReturnsNamespaceNameDeclaringTypeNameAndOwnName()
        {
            var typeDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly, DeclarationFilter.All)
                .Namespaces
                .Single(@namespace => @namespace.Name == "CodeMap.Tests.Data")
                .DeclaredTypes
                .OfType<ClassDeclaration>()
                .Single(type => type.Name == "TestClass")
                .NestedTypes
                .Single(type => type.Name == "NestedTestEnum");

            var fullNameReference = typeDeclaration.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestClass<TParam>.NestedTestEnum", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForNestedDelegateReturnsNamespaceNameDeclaringTypeNameOwnNameAndParameterTypeNames()
        {
            var typeDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly, DeclarationFilter.All)
                .Namespaces
                .Single(@namespace => @namespace.Name == "CodeMap.Tests.Data")
                .DeclaredTypes
                .OfType<ClassDeclaration>()
                .Single(type => type.Name == "TestClass")
                .NestedTypes
                .Single(type => type.Name == "NestedTestDelegate");

            var fullNameReference = typeDeclaration.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestClass<TParam>.NestedTestDelegate()", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForNestedInterfaceReturnsNamespaceNameDeclaringTypeNameAndOwnName()
        {
            var typeDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly, DeclarationFilter.All)
                .Namespaces
                .Single(@namespace => @namespace.Name == "CodeMap.Tests.Data")
                .DeclaredTypes
                .OfType<ClassDeclaration>()
                .Single(type => type.Name == "TestClass")
                .NestedTypes
                .Single(type => type.Name == "INestedTestInterface");

            var fullNameReference = typeDeclaration.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestClass<TParam>.INestedTestInterface", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForNestedInterfaceReturnsNamespaceNameDeclaringTypeNameOwnNameAndGenericParameters()
        {
            var typeDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly, DeclarationFilter.All)
                .Namespaces
                .Single(@namespace => @namespace.Name == "CodeMap.Tests.Data")
                .DeclaredTypes
                .OfType<ClassDeclaration>()
                .Single(type => type.Name == "TestClass")
                .NestedTypes
                .Single(type => type.Name == "NestedTestClass");

            var fullNameReference = typeDeclaration.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestClass<TParam>.NestedTestClass<TParam2,TParam3>", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForNestedRecordReturnsNamespaceNameDeclaringTypeNameAndOwnName()
        {
            var typeDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly, DeclarationFilter.All)
                .Namespaces
                .Single(@namespace => @namespace.Name == "CodeMap.Tests.Data")
                .DeclaredTypes
                .OfType<ClassDeclaration>()
                .Single(type => type.Name == "TestClass")
                .NestedTypes
                .Single(type => type.Name == "NestedTestRecord");

            var fullNameReference = typeDeclaration.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestClass<TParam>.NestedTestRecord", fullNameReference);
        }

        [Fact]
        public void GettingFullNameReferenceForNestedStructReturnsNamespaceNameDeclaringTypeNameAndOwnName()
        {
            var typeDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly, DeclarationFilter.All)
                .Namespaces
                .Single(@namespace => @namespace.Name == "CodeMap.Tests.Data")
                .DeclaredTypes
                .OfType<ClassDeclaration>()
                .Single(type => type.Name == "TestClass")
                .NestedTypes
                .Single(type => type.Name == "NestedTestStruct");

            var fullNameReference = typeDeclaration.GetFullNameReference();

            Assert.Equal("CodeMap.Tests.Data.TestClass<TParam>.NestedTestStruct", fullNameReference);
        }
    }
}