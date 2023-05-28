using CodeMap.DeclarationNodes;
using CodeMap.Html;
using System.Linq;
using Xunit;

namespace CodeMap.Tests.Html
{
    public class DeclarationNodeExtensions_GetSimpleNameReferenceTests
    {
        [Fact]
        public void GettingSimpleNameReferenceForAssemblyDeclarationReturnsName()
        {
            var assemblyDeclaration = DeclarationNode.Create(typeof(GlobalTestClass).Assembly);

            var simpleNameReference = assemblyDeclaration.GetSimpleNameReference();

            Assert.Equal("CodeMap.Tests.Data", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForNamespaceDeclarationReturnsName()
        {
            var namespaceDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly)
                .Namespaces
                .Single(@namespace => @namespace.Name == "CodeMap.Tests.Data");

            var simpleNameReference = namespaceDeclaration.GetSimpleNameReference();

            Assert.Equal("CodeMap.Tests.Data", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForGlobalNamespaceDeclarationReturnsEmptyString()
        {
            var namespaceDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly)
                .Namespaces
                .OfType<GlobalNamespaceDeclaration>()
                .Single();

            var simpleNameReference = namespaceDeclaration.GetSimpleNameReference();

            Assert.Empty(simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForTypeDeclarationReturnsName()
        {
            var typeDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly)
                .Namespaces
                .Single(@namespace => @namespace.Name == "CodeMap.Tests.Data")
                .DeclaredTypes
                .Single(type => type.Name == "TestEnum");

            var simpleNameReference = typeDeclaration.GetSimpleNameReference();

            Assert.Equal("TestEnum", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForGenericInterfaceDeclarationReturnsNameAndGenericParameterNames()
        {
            var typeDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly)
                .Namespaces
                .Single(@namespace => @namespace.Name == "CodeMap.Tests.Data")
                .DeclaredTypes
                .Single(type => type.Name == "ITestGenericParameter");

            var simpleNameReference = typeDeclaration.GetSimpleNameReference();

            Assert.Equal("ITestGenericParameter<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForGenericClassDeclarationReturnsNameAndGenericParameterNames()
        {
            var typeDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly)
                .Namespaces
                .Single(@namespace => @namespace.Name == "CodeMap.Tests.Data")
                .DeclaredTypes
                .Single(type => type.Name == "TestClass");

            var simpleNameReference = typeDeclaration.GetSimpleNameReference();

            Assert.Equal("TestClass<TParam>", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForGenericRecordDeclarationReturnsNameAndGenericParameterNames()
        {
            var typeDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly)
                .Namespaces
                .Single(@namespace => @namespace.Name == "CodeMap.Tests.Data")
                .DeclaredTypes
                .Single(type => type.Name == "TestRecord");

            var simpleNameReference = typeDeclaration.GetSimpleNameReference();

            Assert.Equal("TestRecord<TParam>", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForGenericStructDeclarationReturnsNameAndGenericParameterNames()
        {
            var typeDeclaration = DeclarationNode
                .Create(typeof(GlobalTestClass).Assembly)
                .Namespaces
                .Single(@namespace => @namespace.Name == "CodeMap.Tests.Data")
                .DeclaredTypes
                .Single(type => type.Name == "TestStruct");

            var simpleNameReference = typeDeclaration.GetSimpleNameReference();

            Assert.Equal("TestStruct<TParam>", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForConstantDeclarationReturnsDeclaringTypeNameAndOwnName()
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

            var simpleNameReference = constantDeclaration.GetSimpleNameReference();

            Assert.Equal("TestClass<TParam>.ClassShadowedTestConstant", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForFieldDeclarationReturnsDeclaringTypeNameAndOwnName()
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

            var simpleNameReference = fieldDeclaration.GetSimpleNameReference();

            Assert.Equal("TestClass<TParam>.StaticTestField", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForConstructorDeclarationReturnsDeclaringTypeNameOwnNameAndParameterTypeNames()
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

            var simpleNameReference = constructorDeclaration.GetSimpleNameReference();

            Assert.Equal("TestClass<TParam>.TestClass(int)", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForEventDeclarationReturnsDeclaringTypeNameAndOwnName()
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

            var simpleNameReference = eventDeclaration.GetSimpleNameReference();

            Assert.Equal("TestClass<TParam>.AbstractTestEvent", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForPropertyDeclarationReturnsDeclaringTypeNameAndOwnName()
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

            var simpleNameReference = propertyDeclaration.GetSimpleNameReference();

            Assert.Equal("TestClass<TParam>.InterfaceShadowedTestProperty", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForIndexPropertyDeclarationReturnsDeclaringTypeNameOwnNameAndParameterTypeNames()
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

            var simpleNameReference = propertyDeclaration.GetSimpleNameReference();

            Assert.Equal("TestClass<TParam>.Item[int]", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForMethodDeclarationReturnsDeclaringTypeNameOwnNameAndParameterTypeNames()
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

            var simpleNameReference = methodDeclaration.GetSimpleNameReference();

            Assert.Equal("TestClass<TParam>.TestMethod(int, string)", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForGenericMethodDeclarationReturnsDeclaringTypeNameOwnNameGenericParameterNamesAndParameterTypeNames()
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

            var simpleNameReference = methodDeclaration.GetSimpleNameReference();

            Assert.Equal("TestClass<TParam>.TestMethod38<TMethodParam>(TMethodParam)", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForNestedEnumReturnsDeclaringTypeNameAndOwnName()
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

            var simpleNameReference = typeDeclaration.GetSimpleNameReference();

            Assert.Equal("TestClass<TParam>.NestedTestEnum", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForNestedDelegateReturnsDeclaringTypeNameOwnNameAndParameterTypeNames()
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

            var simpleNameReference = typeDeclaration.GetSimpleNameReference();

            Assert.Equal("TestClass<TParam>.NestedTestDelegate()", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForNestedInterfaceReturnsDeclaringTypeNameAndOwnName()
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

            var simpleNameReference = typeDeclaration.GetSimpleNameReference();

            Assert.Equal("TestClass<TParam>.INestedTestInterface", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForNestedInterfaceReturnsDeclaringTypeNameOwnNameAndGenericParameters()
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

            var simpleNameReference = typeDeclaration.GetSimpleNameReference();

            Assert.Equal("TestClass<TParam>.NestedTestClass<TParam2, TParam3>", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForNestedRecordReturnsDeclaringTypeNameAndOwnName()
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

            var simpleNameReference = typeDeclaration.GetSimpleNameReference();

            Assert.Equal("TestClass<TParam>.NestedTestRecord", simpleNameReference);
        }

        [Fact]
        public void GettingSimpleNameReferenceForNestedStructReturnsDeclaringTypeNameAndOwnName()
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

            var simpleNameReference = typeDeclaration.GetSimpleNameReference();

            Assert.Equal("TestClass<TParam>.NestedTestStruct", simpleNameReference);
        }
    }
}