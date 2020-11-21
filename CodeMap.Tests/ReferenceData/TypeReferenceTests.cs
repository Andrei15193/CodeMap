using System;
using System.Collections.Generic;
using CodeMap.ReferenceData;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.ReferenceData
{
    public class TypeReferenceTests : MemberReferenceTests
    {
        [Fact]
        public void CreateFromNullTypeThrowsExceptions()
        {
            var exception = Assert.Throws<ArgumentNullException>("type", () => Factory.Create(type: null));
            Assert.Equal(new ArgumentNullException("type").Message, exception.Message);
        }

        [Fact]
        public void CreateFromNullMemberInfoThrowsExceptions()
        {
            var exception = Assert.Throws<ArgumentNullException>("memberInfo", () => Factory.Create(memberInfo: null));
            Assert.Equal(new ArgumentNullException("memberInfo").Message, exception.Message);
        }

        [Fact]
        public void CreateReferenceFromSimpleType()
        {
            var typeReference = (TypeReference)Factory.Create(typeof(int));
            var visitor = new MemberReferenceVisitorMock<TypeReference>(typeReference);

            Assert.Equal("Int32", typeReference.Name);
            Assert.Equal("System", typeReference.Namespace);
            Assert.Empty(typeReference.GenericArguments);
            Assert.Null(typeReference.DeclaringType);
            Assert.True(typeReference.Assembly == typeof(int).Assembly.GetName());

            typeReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);
        }

        [Fact]
        public void CreateReferenceFromConstructedGenericType()
        {
            var type = typeof(TestClass<int>.NestedTestClass<IEnumerable<string>, IDictionary<long, decimal>>);
            var typeReference = (TypeReference)Factory.Create(type);
            var visitor = new MemberReferenceVisitorMock<TypeReference>(typeReference);

            Assert.True(typeReference == type);
            Assert.True(type == typeReference);
            Assert.True(typeReference != type.GetGenericTypeDefinition());
            Assert.True(type.GetGenericTypeDefinition() != typeReference);

            Assert.True(typeReference.DeclaringType == typeof(TestClass<int>));
            Assert.True(typeof(TestClass<int>) == typeReference.DeclaringType);
            Assert.True(typeReference.DeclaringType != typeof(TestClass<>));
            Assert.True(typeof(TestClass<>) != typeReference.DeclaringType);

            typeReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);
        }

        [Fact]
        public void CreateReferenceFromGenericDefinitionType()
        {
            var type = typeof(TestClass<int>.NestedTestClass<IEnumerable<string>, IDictionary<long, decimal>>);
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            var typeReference = (TypeReference)Factory.Create(genericTypeDefinition);
            var visitor = new MemberReferenceVisitorMock<TypeReference>(typeReference);

            Assert.True(typeReference == genericTypeDefinition);
            Assert.True(genericTypeDefinition == typeReference);
            Assert.True(typeReference != type);
            Assert.True(type != typeReference);

            Assert.True(typeReference.DeclaringType == typeof(TestClass<>));
            Assert.True(typeof(TestClass<>) == typeReference.DeclaringType);
            Assert.True(typeReference.DeclaringType != typeof(TestClass<int>));
            Assert.True(typeof(TestClass<int>) != typeReference.DeclaringType);

            typeReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);
        }
    }
}