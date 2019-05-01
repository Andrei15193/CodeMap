using CodeMap.ReferenceData;
using CodeMap.Tests.Data;
using Moq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CodeMap.Tests.ReferenceData
{
    public class MemberReferenceFactoryTests
    {
        private MemberReferenceFactory _Factory { get; }

        private Mock<ITestMemberReferenceVisitor> _VisitorMock { get; }

        private MemberReferenceVisitor _Visitor { get; }

        public MemberReferenceFactoryTests()
        {
            _Factory = new MemberReferenceFactory();
            _VisitorMock = new Mock<ITestMemberReferenceVisitor>();
            _Visitor = new MemberReferenceVisitorAdapter(_VisitorMock.Object);
        }

        [Fact]
        public async Task CreateFromSimpleType_ReturnsSpecificTypeReference()
        {
            TypeReference typeReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitType(It.IsNotNull<TypeReference>()))
                .Callback((TypeReference actualTypeReference) => typeReference = actualTypeReference);

            await _Factory.Create(typeof(int)).AcceptAsync(_Visitor);

            Assert.Equal("Int32", typeReference.Name);
            Assert.Equal("System", typeReference.Namespace);
            Assert.Empty(typeReference.GenericArguments);
            Assert.Null(typeReference.DeclaringType);
            Assert.True(typeof(int).Assembly == typeReference.Assembly);
        }

        [Fact]
        public async Task TypeReference_IsEqualToInitialType()
        {
            TypeReference typeReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitType(It.IsNotNull<TypeReference>()))
                .Callback((TypeReference actualTypeReference) => typeReference = actualTypeReference);
            var type = typeof(TestClass<int>.NestedTestClass<IEnumerable<string>, IDictionary<long, decimal>>);

            await _Factory.Create(type).AcceptAsync(_Visitor);

            Assert.True(typeReference == type);
            Assert.True(type == typeReference);
            Assert.True(typeReference != type.GetGenericTypeDefinition());
            Assert.True(type.GetGenericTypeDefinition() != typeReference);

            Assert.True(typeReference.DeclaringType == typeof(TestClass<int>));
            Assert.True(typeof(TestClass<int>) == typeReference.DeclaringType);
            Assert.True(typeReference.DeclaringType != typeof(TestClass<>));
            Assert.True(typeof(TestClass<>) != typeReference.DeclaringType);
        }

        [Fact]
        public async Task TypeReference_IsEqualToInitialGenericTypeDefinition()
        {
            TypeReference typeReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitType(It.IsNotNull<TypeReference>()))
                .Callback((TypeReference actualTypeReference) => typeReference = actualTypeReference);
            var type = typeof(TestClass<int>.NestedTestClass<IEnumerable<string>, IDictionary<long, decimal>>);
            var genericTypeDefinition = type.GetGenericTypeDefinition();

            await _Factory.Create(genericTypeDefinition).AcceptAsync(_Visitor);

            Assert.True(typeReference == genericTypeDefinition);
            Assert.True(genericTypeDefinition == typeReference);
            Assert.True(typeReference != type);
            Assert.True(type != typeReference);

            Assert.True(typeReference.DeclaringType == typeof(TestClass<>));
            Assert.True(typeof(TestClass<>) == typeReference.DeclaringType);
            Assert.True(typeReference.DeclaringType != typeof(TestClass<int>));
            Assert.True(typeof(TestClass<int>) != typeReference.DeclaringType);
        }

        [Fact]
        public async Task CreateFromVoidType_ReturnsVoidTypeReference()
        {
            TypeReference typeReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitType(It.IsNotNull<TypeReference>()))
                .Callback((TypeReference actualTypeReference) => typeReference = actualTypeReference);

            await _Factory.Create(typeof(void)).AcceptAsync(_Visitor);

            Assert.IsType<VoidTypeReference>(typeReference);
            Assert.True(typeReference == typeof(void));
            Assert.True(typeReference != typeof(int));
        }

        [Fact]
        public async Task CreateDynamicTypeReference()
        {
            TypeReference typeReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitType(It.IsNotNull<TypeReference>()))
                .Callback((TypeReference actualTypeReference) => typeReference = actualTypeReference);

            await _Factory.CreateDynamic().AcceptAsync(_Visitor);

            Assert.IsType<DynamicTypeReference>(typeReference);
            Assert.True(typeReference == typeof(object));
            Assert.True(typeReference == typeof(IDynamicMetaObjectProvider));
            Assert.True(typeReference == typeof(DynamicObject));
            Assert.True(typeReference != typeof(int));
        }

        [Fact]
        public async Task CreateFromGenericTypeParameter_ReturnsGenericTypeParameterReference()
        {
            GenericTypeParameterReference genericParameterReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitGenericTypeParameter(It.IsNotNull<GenericTypeParameterReference>()))
                .Callback((GenericTypeParameterReference actualGenericParameterReference) => genericParameterReference = actualGenericParameterReference);

            await _Factory.Create(typeof(IEnumerable<>).GetGenericArguments().Single()).AcceptAsync(_Visitor);

            Assert.Equal("T", genericParameterReference.Name);
            Assert.True(genericParameterReference.DeclaringType == typeof(IEnumerable<>));
        }

        [Fact]
        public void CreateFromAssembly_ReturnsAssemblyReference()
        {
            var assemblyReference = _Factory.Create(typeof(GlobalTestClass).Assembly);

            Assert.Equal("CodeMap.Tests.Data", assemblyReference.Name);
            Assert.Equal(new Version(1, 2, 3, 4), assemblyReference.Version);
            Assert.Empty(assemblyReference.Culture);
            Assert.Empty(assemblyReference.PublicKeyToken);
        }

        [Fact]
        public void CreateFromAssemblyName_ReturnsAssemblyReference()
        {
            var assemblyReference = _Factory.Create(typeof(GlobalTestClass).Assembly.GetName());

            Assert.Equal("CodeMap.Tests.Data", assemblyReference.Name);
            Assert.Equal(new Version(1, 2, 3, 4), assemblyReference.Version);
            Assert.Empty(assemblyReference.Culture);
            Assert.Empty(assemblyReference.PublicKeyToken);
        }

        [Fact]
        public void AssemblyReference_IsEqualToInitialAssembly()
        {
            var assembly = typeof(GlobalTestClass).Assembly;
            var otherAssembly = typeof(MemberReferenceFactoryTests).Assembly;
            var assemblyReference = _Factory.Create(assembly);

            Assert.True(assemblyReference == assembly);
            Assert.True(assembly == assemblyReference);
            Assert.True(assemblyReference != otherAssembly);
            Assert.True(otherAssembly != assemblyReference);
        }

        [Fact]
        public void AssemblyReference_IsEqualToInitialAssemblyName()
        {
            var assemblyName = typeof(GlobalTestClass).Assembly.GetName();
            var otherAssemblyName = typeof(MemberReferenceFactoryTests).Assembly.GetName();
            var assemblyReference = _Factory.Create(assemblyName);

            Assert.True(assemblyReference == assemblyName);
            Assert.True(assemblyName == assemblyReference);
            Assert.True(assemblyReference != otherAssemblyName);
            Assert.True(otherAssemblyName != assemblyReference);
        }

        [Fact]
        public void CreateFromNullMemberInfo_ThrowsExceptions()
        {
            var exception = Assert.Throws<ArgumentNullException>("memberInfo", () => _Factory.Create(memberInfo: null));
            Assert.Equal(new ArgumentNullException("memberInfo").Message, exception.Message);
        }

        [Fact]
        public void CreateFromNullAssembly_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("assembly", () => _Factory.Create(assembly: null));
            Assert.Equal(new ArgumentNullException("assembly").Message, exception.Message);
        }

        [Fact]
        public void CreateFromNullAssemblyName_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>("assemblyName", () => _Factory.Create(assemblyName: null));
            Assert.Equal(new ArgumentNullException("assemblyName").Message, exception.Message);
        }
    }
}