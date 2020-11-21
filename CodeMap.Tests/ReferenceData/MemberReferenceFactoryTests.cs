using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using CodeMap.ReferenceData;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.ReferenceData
{
    public class MemberReferenceFactoryTests
    {
        private MemberReferenceFactory _Factory { get; } = new MemberReferenceFactory();

        [Fact]
        public void CreateFromSimpleType_ReturnsSpecificTypeReference()
        {
            var typeReference = (TypeReference)_Factory.Create(typeof(int));
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
        public void TypeReference_IsEqualToInitialType()
        {
            var type = typeof(TestClass<int>.NestedTestClass<IEnumerable<string>, IDictionary<long, decimal>>);
            var typeReference = (TypeReference)_Factory.Create(type);
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
        public void TypeReference_IsEqualToInitialGenericTypeDefinition()
        {
            var type = typeof(TestClass<int>.NestedTestClass<IEnumerable<string>, IDictionary<long, decimal>>);
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            var typeReference = (TypeReference)_Factory.Create(genericTypeDefinition);
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

        [Fact]
        public void CreateFromVoidType_ReturnsVoidTypeReference()
        {
            var typeReference = (VoidTypeReference)_Factory.Create(typeof(void));
            var visitor = new MemberReferenceVisitorMock<TypeReference>(typeReference);

            Assert.IsType<VoidTypeReference>(typeReference);
            Assert.True(typeReference == typeof(void));
            Assert.True(typeReference != typeof(int));

            typeReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);
        }

        [Fact]
        public void CreateDynamicTypeReference()
        {
            var typeReference = _Factory.CreateDynamic();
            var visitor = new MemberReferenceVisitorMock<TypeReference>(typeReference);

            Assert.IsType<DynamicTypeReference>(typeReference);
            Assert.True(typeReference == typeof(object));
            Assert.True(typeReference == typeof(IDynamicMetaObjectProvider));
            Assert.True(typeReference == typeof(DynamicObject));
            Assert.True(typeReference != typeof(int));

            typeReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);
        }

        [Fact]
        public void CreateFromArray_ReturnsArrayTypeReference()
        {
            var arrayTypeReference = (ArrayTypeReference)_Factory.Create(typeof(decimal[][,]));
            var visitor = new MemberReferenceVisitorMock<ArrayTypeReference>(arrayTypeReference);

            Assert.Equal(1, arrayTypeReference.Rank);
            var itemArrayTypeReference = Assert.IsType<ArrayTypeReference>(arrayTypeReference.ItemType);
            Assert.Equal(2, itemArrayTypeReference.Rank);
            Assert.True(itemArrayTypeReference.ItemType == typeof(decimal));
            Assert.True(arrayTypeReference == typeof(decimal[][,]));
            Assert.True(arrayTypeReference != typeof(decimal[,][]));

            arrayTypeReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);
        }

        [Fact]
        public void CreateFromPointerType_ReturnsPointerTypeReference()
        {
            var pointerTypeReference = (PointerTypeReference)_Factory.Create(typeof(int**));
            var visitor = new MemberReferenceVisitorMock<PointerTypeReference>(pointerTypeReference);

            var referentPointerTypeReference = Assert.IsType<PointerTypeReference>(pointerTypeReference.ReferentType);
            Assert.True(referentPointerTypeReference.ReferentType == typeof(int));
            Assert.True(referentPointerTypeReference == typeof(int*));
            Assert.True(referentPointerTypeReference != typeof(int));
            Assert.True(pointerTypeReference == typeof(int**));
            Assert.True(pointerTypeReference != typeof(int*));

            pointerTypeReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);
        }

        [Fact]
        public void CreateFromByRefType_ReturnsByRefTypeReference()
        {
            var byRefTypeReference = (ByRefTypeReference)_Factory.Create(typeof(int).MakeByRefType());
            var visitor = new MemberReferenceVisitorMock<ByRefTypeReference>(byRefTypeReference);

            Assert.True(byRefTypeReference.ReferentType == typeof(int));
            Assert.True(byRefTypeReference == typeof(int).MakeByRefType());
            Assert.True(byRefTypeReference != typeof(int));

            byRefTypeReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);
        }

        [Fact]
        public void CreateFromGenericTypeParameter_ReturnsGenericTypeParameterReference()
        {
            var genericParameterReference = (GenericTypeParameterReference)_Factory.Create(_GetGenericTypeParameter());
            var visitor = new MemberReferenceVisitorMock<GenericTypeParameterReference>(genericParameterReference);

            Assert.Equal("T", genericParameterReference.Name);
            Assert.Equal(0, genericParameterReference.Position);
            Assert.True(genericParameterReference.DeclaringType == typeof(IEnumerable<>));
            Assert.True(genericParameterReference == _GetGenericTypeParameter());

            genericParameterReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);

            Type _GetGenericTypeParameter()
                => typeof(IEnumerable<>).GetGenericArguments().Single();
        }

        [Fact]
        public void CreateFromGenericMethodParameter_ReturnsGenericMethodParameterReference()
        {
            var genericParameterReference = (GenericMethodParameterReference)_Factory.Create(_GetGenericMethodParameter());
            var visitor = new MemberReferenceVisitorMock<GenericMethodParameterReference>(genericParameterReference);

            Assert.Equal("T", genericParameterReference.Name);
            Assert.Equal(0, genericParameterReference.Position);
            Assert.True(genericParameterReference.DeclaringMethod == _GetMethodInfo());
            Assert.True(genericParameterReference == _GetGenericMethodParameter());

            genericParameterReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);

            Type _GetGenericMethodParameter()
                => _GetMethodInfo()
                    .GetGenericArguments()
                    .Single();

            MethodInfo _GetMethodInfo()
                => typeof(string)
                    .GetMethods()
                    .Single(
                        methodInfo => methodInfo.Name == "Join"
                            && methodInfo.IsGenericMethod
                            && methodInfo.GetParameters().First().ParameterType == typeof(string)
                    );
        }

        [Fact]
        public void CreateFromConstant_ReturnsConstantReference()
        {
            var constantReference = (ConstantReference)_Factory.Create(_GetFieldInfo());
            var visitor = new MemberReferenceVisitorMock<ConstantReference>(constantReference);

            Assert.Equal("MaxValue", constantReference.Name);
            Assert.Equal(int.MaxValue, constantReference.Value);
            Assert.True(constantReference.DeclaringType == typeof(int));
            Assert.True(constantReference == _GetFieldInfo());

            constantReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);

            FieldInfo _GetFieldInfo()
                => typeof(int).GetField(nameof(int.MaxValue));
        }

        [Fact]
        public void CreateFromField_ReturnsFieldReference()
        {
            var fieldReference = (FieldReference)_Factory.Create(_GetFieldInfo());
            var visitor = new MemberReferenceVisitorMock<FieldReference>(fieldReference);

            Assert.Equal("Empty", fieldReference.Name);
            Assert.True(fieldReference.DeclaringType == typeof(string));
            Assert.True(fieldReference == _GetFieldInfo());

            fieldReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);

            FieldInfo _GetFieldInfo()
                => typeof(string).GetField(nameof(string.Empty));
        }

        [Fact]
        public void CreateFromConstructor_ReturnsConstructorReference()
        {
            var constructorReference = (ConstructorReference)_Factory.Create(_GetConstructorInfo());
            var visitor = new MemberReferenceVisitorMock<ConstructorReference>(constructorReference);

            Assert.True(constructorReference.DeclaringType == typeof(string));
            Assert.True(constructorReference
                .ParameterTypes
                .AsEnumerable<object>()
                .SequenceEqual(new[] { typeof(char), typeof(int) })
            );
            Assert.True(constructorReference == _GetConstructorInfo());

            constructorReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);

            ConstructorInfo _GetConstructorInfo()
                => typeof(string).GetConstructor(new[] { typeof(char), typeof(int) });
        }

        [Fact]
        public void CreateFromEvent_ReturnsEventReference()
        {
            var eventReference = (EventReference)_Factory.Create(_GetEventInfo());
            var visitor = new MemberReferenceVisitorMock<EventReference>(eventReference);

            Assert.Equal("PropertyChanged", eventReference.Name);
            Assert.True(eventReference.DeclaringType == typeof(INotifyPropertyChanged));
            Assert.True(eventReference == _GetEventInfo());

            eventReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);

            EventInfo _GetEventInfo()
                => typeof(INotifyPropertyChanged).GetEvent(nameof(INotifyPropertyChanged.PropertyChanged));
        }

        [Fact]
        public void CreateFromProperty_ReturnsPropertyReference()
        {
            var propertyReference = (PropertyReference)_Factory.Create(_GetPropertyInfo());
            var visitor = new MemberReferenceVisitorMock<PropertyReference>(propertyReference);

            Assert.Equal("Item", propertyReference.Name);
            Assert.True(propertyReference.DeclaringType == typeof(IDictionary<string, string>));
            Assert.True(propertyReference
                .ParameterTypes
                .AsEnumerable<object>()
                .SequenceEqual(new[] { typeof(string) })
            );
            Assert.True(propertyReference == _GetPropertyInfo());

            propertyReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);

            PropertyInfo _GetPropertyInfo()
                => typeof(IDictionary<string, string>).GetDefaultMembers().OfType<PropertyInfo>().Single();
        }

        [Fact]
        public void CreateFromMethod_ReturnsMethodReference()
        {
            var methodReference = (MethodReference)_Factory.Create(_GetMethodInfo());
            var visitor = new MemberReferenceVisitorMock<MethodReference>(methodReference);

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

            methodReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);

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
            var methodReference = (MethodReference)_Factory.Create(_GetMethodInfo());
            var visitor = new MemberReferenceVisitorMock<MethodReference>(methodReference);

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

            methodReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);

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
            var assemblyReference = _Factory.Create(typeof(TestClass<>).Assembly);
            var visitor = new MemberReferenceVisitorMock<AssemblyReference>(assemblyReference);

            Assert.Equal("CodeMap.Tests.Data", assemblyReference.Name);
            Assert.Equal(new Version(1, 2, 3, 4), assemblyReference.Version);
            Assert.Empty(assemblyReference.Culture);
#if DEBUG
            Assert.Empty(assemblyReference.PublicKeyToken);
#else
            Assert.Equal("4919ac5af74d53e8", assemblyReference.PublicKeyToken);
#endif

            assemblyReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);
        }

        [Fact]
        public void CreateFromAssemblyName_ReturnsAssemblyReference()
        {
            var assemblyReference = _Factory.Create(typeof(TestClass<>).Assembly.GetName());
            var visitor = new MemberReferenceVisitorMock<AssemblyReference>(assemblyReference);

            Assert.Equal("CodeMap.Tests.Data", assemblyReference.Name);
            Assert.Equal(new Version(1, 2, 3, 4), assemblyReference.Version);
            Assert.Empty(assemblyReference.Culture);
#if DEBUG
            Assert.Empty(assemblyReference.PublicKeyToken);
#else
            Assert.Equal("4919ac5af74d53e8", assemblyReference.PublicKeyToken);
#endif

            assemblyReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);
        }

        [Fact]
        public void AssemblyReference_IsEqualToInitialAssemblyName()
        {
            var assemblyName = typeof(GlobalTestClass).Assembly.GetName();
            var otherAssemblyName = typeof(MemberReferenceFactoryTests).Assembly.GetName();

            var assemblyReference = _Factory.Create(assemblyName);
            var visitor = new MemberReferenceVisitorMock<AssemblyReference>(assemblyReference);

            Assert.True(assemblyReference == assemblyName);
            Assert.True(assemblyName == assemblyReference);
            Assert.True(assemblyReference != otherAssemblyName);
            Assert.True(otherAssemblyName != assemblyReference);

            assemblyReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);
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


        protected sealed class MemberReferenceVisitorMock<TMemberReference> : MemberReferenceVisitor
            where TMemberReference : MemberReference
        {
            private readonly TMemberReference _expectedMemberReference;

            public MemberReferenceVisitorMock(TMemberReference memberReference)
                => _expectedMemberReference = memberReference;

            public int VisitCount { get; private set; }

            protected override void VisitAssembly(AssemblyReference assembly)
                => _InvokeCallback(assembly);

            protected override void VisitType(TypeReference type)
                => _InvokeCallback(type);

            protected override void VisitArray(ArrayTypeReference array)
                => _InvokeCallback(array);

            protected override void VisitByRef(ByRefTypeReference byRef)
                => _InvokeCallback(byRef);

            protected override void VisitPointer(PointerTypeReference pointer)
                => _InvokeCallback(pointer);

            protected override void VisitGenericMethodParameter(GenericMethodParameterReference genericMethodParameter)
                => _InvokeCallback(genericMethodParameter);

            protected override void VisitGenericTypeParameter(GenericTypeParameterReference genericTypeParameter)
                => _InvokeCallback(genericTypeParameter);

            protected override void VisitConstant(ConstantReference constant)
                => _InvokeCallback(constant);

            protected override void VisitField(FieldReference field)
                => _InvokeCallback(field);

            protected override void VisitConstructor(ConstructorReference constructor)
                => _InvokeCallback(constructor);

            protected override void VisitEvent(EventReference @event)
                => _InvokeCallback(@event);

            protected override void VisitProperty(PropertyReference property)
                => _InvokeCallback(property);

            protected override void VisitMethod(MethodReference method)
                => _InvokeCallback(method);

            private void _InvokeCallback<TVisitedMemberReference>(TVisitedMemberReference actualMemberReference)
                where TVisitedMemberReference : MemberReference
            {
                if (!typeof(TVisitedMemberReference).IsAssignableFrom(typeof(TMemberReference)))
                    throw new NotImplementedException();

                VisitCount++;
                Assert.Same(_expectedMemberReference, actualMemberReference);
            }
        }
    }
}