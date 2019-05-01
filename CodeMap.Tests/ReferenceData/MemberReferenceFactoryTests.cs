﻿using CodeMap.ReferenceData;
using CodeMap.Tests.Data;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public async Task CreateFromArray_ReturnsArrayTypeReference()
        {
            ArrayTypeReference arrayTypeReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitArray(It.IsNotNull<ArrayTypeReference>()))
                .Callback((ArrayTypeReference actualArrayTypeReference) => arrayTypeReference = actualArrayTypeReference);

            await _Factory.Create(typeof(decimal[][,])).AcceptAsync(_Visitor);

            Assert.Equal(1, arrayTypeReference.Rank);
            var itemArrayTypeReference = Assert.IsType<ArrayTypeReference>(arrayTypeReference.ItemType);
            Assert.Equal(2, itemArrayTypeReference.Rank);
            Assert.True(itemArrayTypeReference.ItemType == typeof(decimal));
            Assert.True(arrayTypeReference == typeof(decimal[][,]));
            Assert.True(arrayTypeReference != typeof(decimal[,][]));
        }

        [Fact]
        public async Task CreateFromPointerType_ReturnsPointerTypeReference()
        {
            PointerTypeReference pointerTypeReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitPointer(It.IsNotNull<PointerTypeReference>()))
                .Callback((PointerTypeReference actualPointerTypeReference) => pointerTypeReference = actualPointerTypeReference);

            await _Factory.Create(typeof(int**)).AcceptAsync(_Visitor);

            var referentPointerTypeReference = Assert.IsType<PointerTypeReference>(pointerTypeReference.ReferentType);
            Assert.True(referentPointerTypeReference.ReferentType == typeof(int));
            Assert.True(referentPointerTypeReference == typeof(int*));
            Assert.True(referentPointerTypeReference != typeof(int));
            Assert.True(pointerTypeReference == typeof(int**));
            Assert.True(pointerTypeReference != typeof(int*));
        }

        [Fact]
        public async Task CreateFromByRefType_ReturnsByRefTypeReference()
        {
            ByRefTypeReference byRefTypeReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitByRef(It.IsNotNull<ByRefTypeReference>()))
                .Callback((ByRefTypeReference actualByRefTypeReference) => byRefTypeReference = actualByRefTypeReference);

            await _Factory.Create(typeof(int).MakeByRefType()).AcceptAsync(_Visitor);

            Assert.True(byRefTypeReference.ReferentType == typeof(int));
            Assert.True(byRefTypeReference == typeof(int).MakeByRefType());
            Assert.True(byRefTypeReference != typeof(int));
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
        public async Task CreateFromConstant_ReturnsConstantReference()
        {
            ConstantReference constantReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitConstant(It.IsNotNull<ConstantReference>()))
                .Callback((ConstantReference actualConstantReference) => constantReference = actualConstantReference);

            await _Factory.Create(typeof(int).GetField(nameof(int.MaxValue))).AcceptAsync(_Visitor);

            Assert.Equal("MaxValue", constantReference.Name);
            Assert.Equal(int.MaxValue, constantReference.Value);
            Assert.True(constantReference.DeclaringType == typeof(int));
        }

        [Fact]
        public async Task CreateFromField_ReturnsFieldReference()
        {
            FieldReference fieldReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitField(It.IsNotNull<FieldReference>()))
                .Callback((FieldReference actualFieldReference) => fieldReference = actualFieldReference);

            await _Factory.Create(typeof(string).GetField(nameof(string.Empty))).AcceptAsync(_Visitor);

            Assert.Equal("Empty", fieldReference.Name);
            Assert.True(fieldReference.DeclaringType == typeof(string));
        }

        [Fact]
        public async Task CreateFromConstructor_ReturnsConstructorReference()
        {
            ConstructorReference constructorReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitConstructor(It.IsNotNull<ConstructorReference>()))
                .Callback((ConstructorReference actualConstructorReference) => constructorReference = actualConstructorReference);

            await _Factory.Create(typeof(string).GetConstructor(new[] { typeof(char), typeof(int) })).AcceptAsync(_Visitor);

            Assert.True(constructorReference.DeclaringType == typeof(string));
            Assert.True(constructorReference
                .ParameterTypes
                .Zip(
                    new[] { typeof(char), typeof(int) },
                    (typeReference, type) => (TypeReference: typeReference, Type: type)
                )
                .All(pair => pair.TypeReference == pair.Type)
            );
        }

        [Fact]
        public async Task CreateFromEvent_ReturnsEventReference()
        {
            EventReference eventReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitEvent(It.IsNotNull<EventReference>()))
                .Callback((EventReference actualEventReference) => eventReference = actualEventReference);

            await _Factory.Create(typeof(INotifyPropertyChanged).GetEvent(nameof(INotifyPropertyChanged.PropertyChanged))).AcceptAsync(_Visitor);

            Assert.Equal("PropertyChanged", eventReference.Name);
            Assert.True(eventReference.DeclaringType == typeof(INotifyPropertyChanged));
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