using CodeMap.ReferenceData;
using CodeMap.Tests.Data;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
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

            await _Factory.Create(_GetGenericTypeParameter()).AcceptAsync(_Visitor);

            Assert.Equal("T", genericParameterReference.Name);
            Assert.True(genericParameterReference.DeclaringType == typeof(IEnumerable<>));
            Assert.True(genericParameterReference == _GetGenericTypeParameter());

            Type _GetGenericTypeParameter()
                => typeof(IEnumerable<>).GetGenericArguments().Single();
        }

        [Fact]
        public async Task CreateFromGenericMethodParameter_ReturnsGenericMethodParameterReference()
        {
            GenericMethodParameterReference genericParameterReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitGenericMethodParameter(It.IsNotNull<GenericMethodParameterReference>()))
                .Callback((GenericMethodParameterReference actualGenericParameterReference) => genericParameterReference = actualGenericParameterReference);

            await _Factory.Create(_GetGenericMethodParameter()).AcceptAsync(_Visitor);

            Assert.Equal("T", genericParameterReference.Name);
            Assert.True(genericParameterReference.DeclaringMethod == _GetMethodInfo());
            Assert.True(genericParameterReference == _GetGenericMethodParameter());

            MethodInfo _GetMethodInfo()
                => typeof(string)
                    .GetMethods()
                    .Single(
                        methodInfo => methodInfo.Name == "Join"
                            && methodInfo.IsGenericMethod
                            && methodInfo.GetParameters().First().ParameterType == typeof(string)
                    );

            Type _GetGenericMethodParameter()
                => _GetMethodInfo()
                    .GetGenericArguments()
                    .Single();
        }

        [Fact]
        public async Task CreateFromConstant_ReturnsConstantReference()
        {
            ConstantReference constantReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitConstant(It.IsNotNull<ConstantReference>()))
                .Callback((ConstantReference actualConstantReference) => constantReference = actualConstantReference);

            await _Factory.Create(_GetFieldInfo()).AcceptAsync(_Visitor);

            Assert.Equal("MaxValue", constantReference.Name);
            Assert.Equal(int.MaxValue, constantReference.Value);
            Assert.True(constantReference.DeclaringType == typeof(int));
            Assert.True(constantReference == _GetFieldInfo());

            FieldInfo _GetFieldInfo()
                => typeof(int).GetField(nameof(int.MaxValue));
        }

        [Fact]
        public async Task CreateFromField_ReturnsFieldReference()
        {
            FieldReference fieldReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitField(It.IsNotNull<FieldReference>()))
                .Callback((FieldReference actualFieldReference) => fieldReference = actualFieldReference);

            await _Factory.Create(_GetFieldInfo()).AcceptAsync(_Visitor);

            Assert.Equal("Empty", fieldReference.Name);
            Assert.True(fieldReference.DeclaringType == typeof(string));
            Assert.True(fieldReference == _GetFieldInfo());

            FieldInfo _GetFieldInfo()
                => typeof(string).GetField(nameof(string.Empty));
        }

        [Fact]
        public async Task CreateFromConstructor_ReturnsConstructorReference()
        {
            ConstructorReference constructorReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitConstructor(It.IsNotNull<ConstructorReference>()))
                .Callback((ConstructorReference actualConstructorReference) => constructorReference = actualConstructorReference);

            await _Factory.Create(_GetConstructorInfo()).AcceptAsync(_Visitor);

            Assert.True(constructorReference.DeclaringType == typeof(string));
            Assert.True(constructorReference
                .ParameterTypes
                .AsEnumerable<object>()
                .SequenceEqual(new[] { typeof(char), typeof(int) })
            );
            Assert.True(constructorReference == _GetConstructorInfo());

            ConstructorInfo _GetConstructorInfo()
                => typeof(string).GetConstructor(new[] { typeof(char), typeof(int) });
        }

        [Fact]
        public async Task CreateFromEvent_ReturnsEventReference()
        {
            EventReference eventReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitEvent(It.IsNotNull<EventReference>()))
                .Callback((EventReference actualEventReference) => eventReference = actualEventReference);

            await _Factory.Create(_GetEventInfo()).AcceptAsync(_Visitor);

            Assert.Equal("PropertyChanged", eventReference.Name);
            Assert.True(eventReference.DeclaringType == typeof(INotifyPropertyChanged));
            Assert.True(eventReference == _GetEventInfo());

            EventInfo _GetEventInfo()
                => typeof(INotifyPropertyChanged).GetEvent(nameof(INotifyPropertyChanged.PropertyChanged));
        }

        [Fact]
        public async Task CreateFromProperty_ReturnsPropertyReference()
        {
            PropertyReference propertyReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitProperty(It.IsNotNull<PropertyReference>()))
                .Callback((PropertyReference actualPropertyReference) => propertyReference = actualPropertyReference);

            await _Factory.Create(_GetPropertyInfo()).AcceptAsync(_Visitor);

            Assert.Equal("Item", propertyReference.Name);
            Assert.True(propertyReference.DeclaringType == typeof(IDictionary<string, string>));
            Assert.True(propertyReference
                .ParameterTypes
                .AsEnumerable<object>()
                .SequenceEqual(new[] { typeof(string) })
            );
            Assert.True(propertyReference == _GetPropertyInfo());

            PropertyInfo _GetPropertyInfo()
                => typeof(IDictionary<string, string>).GetDefaultMembers().OfType<PropertyInfo>().Single();
        }

        [Fact]
        public async Task CreateFromMethod_ReturnsMethodReference()
        {
            MethodReference methodReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitMethod(It.IsNotNull<MethodReference>()))
                .Callback((MethodReference actualMethodReference) => methodReference = actualMethodReference);

            await _Factory.Create(_GetMethodInfo()).AcceptAsync(_Visitor);

            Assert.Equal("Join", methodReference.Name);
            Assert.True(methodReference
                .GenericArguments
                .AsEnumerable<object>()
                .SequenceEqual(new[] { typeof(int) })
            );
            Assert.True(methodReference.DeclaringType == typeof(string));
            Assert.True(methodReference
                .ParameterTypes
                .AsEnumerable<object>()
                .SequenceEqual(new[] { typeof(string), typeof(IEnumerable<int>) })
            );
            Assert.True(methodReference == _GetMethodInfo());
            Assert.True(methodReference != _GetMethodInfo().GetGenericMethodDefinition());

            MethodInfo _GetMethodInfo()
                => typeof(string)
                    .GetMethods()
                    .Single(
                        methodInfo => methodInfo.Name == "Join"
                            && methodInfo.IsGenericMethod
                            && methodInfo.GetParameters().First().ParameterType == typeof(string)
                    )
                    .MakeGenericMethod(typeof(int));
        }

        [Fact]
        public async Task CreateFromGenericMethodDefinition_ReturnsMethodReference()
        {
            MethodReference methodReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitMethod(It.IsNotNull<MethodReference>()))
                .Callback((MethodReference actualMethodReference) => methodReference = actualMethodReference);

            await _Factory.Create(_GetMethodInfo()).AcceptAsync(_Visitor);

            Assert.Equal("Join", methodReference.Name);
            Assert.True(methodReference
                .GenericArguments
                .AsEnumerable<object>()
                .SequenceEqual(new[] { _GetGenericArgument() })
            );
            Assert.True(methodReference.DeclaringType == typeof(string));
            Assert.True(methodReference
                .ParameterTypes
                .AsEnumerable<object>()
                .SequenceEqual(new[] { typeof(string), typeof(IEnumerable<>).MakeGenericType(_GetGenericArgument()) })
            );
            Assert.True(methodReference == _GetMethodInfo());
            Assert.True(methodReference != _GetMethodInfo().MakeGenericMethod(typeof(int)));

            MethodInfo _GetMethodInfo()
                => typeof(string)
                    .GetMethods()
                    .Single(
                        methodInfo => methodInfo.Name == "Join"
                            && methodInfo.IsGenericMethod
                            && methodInfo.GetParameters().First().ParameterType == typeof(string)
                    );

            Type _GetGenericArgument()
                => _GetMethodInfo().GetGenericArguments().Single();
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
        public void CreateFromNullType_ThrowsExceptions()
        {
            var exception = Assert.Throws<ArgumentNullException>("type", () => _Factory.Create(type: null));
            Assert.Equal(new ArgumentNullException("type").Message, exception.Message);
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