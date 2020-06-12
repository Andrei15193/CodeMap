using CodeMap.ReferenceData;
using CodeMap.Tests.Data;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
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
        public void CreateFromSimpleType_ReturnsSpecificTypeReference()
        {
            TypeReference typeReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitType(It.IsNotNull<TypeReference>()))
                .Callback((TypeReference actualTypeReference) => typeReference = actualTypeReference);

            _Factory.Create(typeof(int)).Accept(_Visitor);

            Assert.Equal("Int32", typeReference.Name);
            Assert.Equal("System", typeReference.Namespace);
            Assert.Empty(typeReference.GenericArguments);
            Assert.Null(typeReference.DeclaringType);
            Assert.True(typeReference.Assembly == typeof(int).Assembly.GetName());
        }

        [Fact]
        public void TypeReference_IsEqualToInitialType()
        {
            TypeReference typeReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitType(It.IsNotNull<TypeReference>()))
                .Callback((TypeReference actualTypeReference) => typeReference = actualTypeReference);
            var type = typeof(TestClass<int>.NestedTestClass<IEnumerable<string>, IDictionary<long, decimal>>);

            _Factory.Create(type).Accept(_Visitor);

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
        public void TypeReference_IsEqualToInitialGenericTypeDefinition()
        {
            TypeReference typeReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitType(It.IsNotNull<TypeReference>()))
                .Callback((TypeReference actualTypeReference) => typeReference = actualTypeReference);
            var type = typeof(TestClass<int>.NestedTestClass<IEnumerable<string>, IDictionary<long, decimal>>);
            var genericTypeDefinition = type.GetGenericTypeDefinition();

            _Factory.Create(genericTypeDefinition).Accept(_Visitor);

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
        public void CreateFromVoidType_ReturnsVoidTypeReference()
        {
            TypeReference typeReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitType(It.IsNotNull<TypeReference>()))
                .Callback((TypeReference actualTypeReference) => typeReference = actualTypeReference);

            _Factory.Create(typeof(void)).Accept(_Visitor);

            Assert.IsType<VoidTypeReference>(typeReference);
            Assert.True(typeReference == typeof(void));
            Assert.True(typeReference != typeof(int));
        }

        [Fact]
        public void CreateDynamicTypeReference()
        {
            TypeReference typeReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitType(It.IsNotNull<TypeReference>()))
                .Callback((TypeReference actualTypeReference) => typeReference = actualTypeReference);

            _Factory.CreateDynamic().Accept(_Visitor);

            Assert.IsType<DynamicTypeReference>(typeReference);
            Assert.True(typeReference == typeof(object));
            Assert.True(typeReference == typeof(IDynamicMetaObjectProvider));
            Assert.True(typeReference == typeof(DynamicObject));
            Assert.True(typeReference != typeof(int));
        }

        [Fact]
        public void CreateFromArray_ReturnsArrayTypeReference()
        {
            ArrayTypeReference arrayTypeReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitArray(It.IsNotNull<ArrayTypeReference>()))
                .Callback((ArrayTypeReference actualArrayTypeReference) => arrayTypeReference = actualArrayTypeReference);

            _Factory.Create(typeof(decimal[][,])).Accept(_Visitor);

            Assert.Equal(1, arrayTypeReference.Rank);
            var itemArrayTypeReference = Assert.IsType<ArrayTypeReference>(arrayTypeReference.ItemType);
            Assert.Equal(2, itemArrayTypeReference.Rank);
            Assert.True(itemArrayTypeReference.ItemType == typeof(decimal));
            Assert.True(arrayTypeReference == typeof(decimal[][,]));
            Assert.True(arrayTypeReference != typeof(decimal[,][]));
        }

        [Fact]
        public void CreateFromPointerType_ReturnsPointerTypeReference()
        {
            PointerTypeReference pointerTypeReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitPointer(It.IsNotNull<PointerTypeReference>()))
                .Callback((PointerTypeReference actualPointerTypeReference) => pointerTypeReference = actualPointerTypeReference);

            _Factory.Create(typeof(int**)).Accept(_Visitor);

            var referentPointerTypeReference = Assert.IsType<PointerTypeReference>(pointerTypeReference.ReferentType);
            Assert.True(referentPointerTypeReference.ReferentType == typeof(int));
            Assert.True(referentPointerTypeReference == typeof(int*));
            Assert.True(referentPointerTypeReference != typeof(int));
            Assert.True(pointerTypeReference == typeof(int**));
            Assert.True(pointerTypeReference != typeof(int*));
        }

        [Fact]
        public void CreateFromByRefType_ReturnsByRefTypeReference()
        {
            ByRefTypeReference byRefTypeReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitByRef(It.IsNotNull<ByRefTypeReference>()))
                .Callback((ByRefTypeReference actualByRefTypeReference) => byRefTypeReference = actualByRefTypeReference);

            _Factory.Create(typeof(int).MakeByRefType()).Accept(_Visitor);

            Assert.True(byRefTypeReference.ReferentType == typeof(int));
            Assert.True(byRefTypeReference == typeof(int).MakeByRefType());
            Assert.True(byRefTypeReference != typeof(int));
        }

        [Fact]
        public void CreateFromGenericTypeParameter_ReturnsGenericTypeParameterReference()
        {
            GenericTypeParameterReference genericParameterReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitGenericTypeParameter(It.IsNotNull<GenericTypeParameterReference>()))
                .Callback((GenericTypeParameterReference actualGenericParameterReference) => genericParameterReference = actualGenericParameterReference);

            _Factory.Create(_GetGenericTypeParameter()).Accept(_Visitor);

            Assert.Equal("T", genericParameterReference.Name);
            Assert.True(genericParameterReference.DeclaringType == typeof(IEnumerable<>));
            Assert.True(genericParameterReference == _GetGenericTypeParameter());

            Type _GetGenericTypeParameter()
                => typeof(IEnumerable<>).GetGenericArguments().Single();
        }

        [Fact]
        public void CreateFromGenericMethodParameter_ReturnsGenericMethodParameterReference()
        {
            GenericMethodParameterReference genericParameterReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitGenericMethodParameter(It.IsNotNull<GenericMethodParameterReference>()))
                .Callback((GenericMethodParameterReference actualGenericParameterReference) => genericParameterReference = actualGenericParameterReference);

            _Factory.Create(_GetGenericMethodParameter()).Accept(_Visitor);

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
        public void CreateFromConstant_ReturnsConstantReference()
        {
            ConstantReference constantReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitConstant(It.IsNotNull<ConstantReference>()))
                .Callback((ConstantReference actualConstantReference) => constantReference = actualConstantReference);

            _Factory.Create(_GetFieldInfo()).Accept(_Visitor);

            Assert.Equal("MaxValue", constantReference.Name);
            Assert.Equal(int.MaxValue, constantReference.Value);
            Assert.True(constantReference.DeclaringType == typeof(int));
            Assert.True(constantReference == _GetFieldInfo());

            FieldInfo _GetFieldInfo()
                => typeof(int).GetField(nameof(int.MaxValue));
        }

        [Fact]
        public void CreateFromField_ReturnsFieldReference()
        {
            FieldReference fieldReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitField(It.IsNotNull<FieldReference>()))
                .Callback((FieldReference actualFieldReference) => fieldReference = actualFieldReference);

            _Factory.Create(_GetFieldInfo()).Accept(_Visitor);

            Assert.Equal("Empty", fieldReference.Name);
            Assert.True(fieldReference.DeclaringType == typeof(string));
            Assert.True(fieldReference == _GetFieldInfo());

            FieldInfo _GetFieldInfo()
                => typeof(string).GetField(nameof(string.Empty));
        }

        [Fact]
        public void CreateFromConstructor_ReturnsConstructorReference()
        {
            ConstructorReference constructorReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitConstructor(It.IsNotNull<ConstructorReference>()))
                .Callback((ConstructorReference actualConstructorReference) => constructorReference = actualConstructorReference);

            _Factory.Create(_GetConstructorInfo()).Accept(_Visitor);

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
        public void CreateFromEvent_ReturnsEventReference()
        {
            EventReference eventReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitEvent(It.IsNotNull<EventReference>()))
                .Callback((EventReference actualEventReference) => eventReference = actualEventReference);

            _Factory.Create(_GetEventInfo()).Accept(_Visitor);

            Assert.Equal("PropertyChanged", eventReference.Name);
            Assert.True(eventReference.DeclaringType == typeof(INotifyPropertyChanged));
            Assert.True(eventReference == _GetEventInfo());

            EventInfo _GetEventInfo()
                => typeof(INotifyPropertyChanged).GetEvent(nameof(INotifyPropertyChanged.PropertyChanged));
        }

        [Fact]
        public void CreateFromProperty_ReturnsPropertyReference()
        {
            PropertyReference propertyReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitProperty(It.IsNotNull<PropertyReference>()))
                .Callback((PropertyReference actualPropertyReference) => propertyReference = actualPropertyReference);

            _Factory.Create(_GetPropertyInfo()).Accept(_Visitor);

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
        public void CreateFromMethod_ReturnsMethodReference()
        {
            MethodReference methodReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitMethod(It.IsNotNull<MethodReference>()))
                .Callback((MethodReference actualMethodReference) => methodReference = actualMethodReference);

            _Factory.Create(_GetMethodInfo()).Accept(_Visitor);

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
        public void CreateFromGenericMethodDefinition_ReturnsMethodReference()
        {
            MethodReference methodReference = null;
            _VisitorMock
                .Setup(visitor => visitor.VisitMethod(It.IsNotNull<MethodReference>()))
                .Callback((MethodReference actualMethodReference) => methodReference = actualMethodReference);

            _Factory.Create(_GetMethodInfo()).Accept(_Visitor);

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
            AssemblyReference assemblyReference = null;

            _VisitorMock
                .Setup(visitor => visitor.VisitAssembly(It.IsNotNull<AssemblyReference>()))
                .Callback((AssemblyReference actualAssemblyReference) => assemblyReference = actualAssemblyReference);

            _Factory.Create(typeof(TestClass<>).Assembly).Accept(_Visitor);

            Assert.Equal("CodeMap.Tests.Data", assemblyReference.Name);
            Assert.Equal(new Version(1, 2, 3, 4), assemblyReference.Version);
            Assert.Empty(assemblyReference.Culture);
            Assert.Empty(assemblyReference.PublicKeyToken);
        }

        [Fact]
        public void CreateFromAssemblyName_ReturnsAssemblyReference()
        {
            AssemblyReference assemblyReference = null;

            _VisitorMock
                .Setup(visitor => visitor.VisitAssembly(It.IsNotNull<AssemblyReference>()))
                .Callback((AssemblyReference actualAssemblyReference) => assemblyReference = actualAssemblyReference);

            _Factory.Create(typeof(TestClass<>).Assembly.GetName()).Accept(_Visitor);

            Assert.Equal("CodeMap.Tests.Data", assemblyReference.Name);
            Assert.Equal(new Version(1, 2, 3, 4), assemblyReference.Version);
            Assert.Empty(assemblyReference.Culture);
            Assert.Empty(assemblyReference.PublicKeyToken);
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