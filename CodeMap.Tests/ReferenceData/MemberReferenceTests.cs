using System;
using CodeMap.ReferenceData;
using Xunit;

namespace CodeMap.Tests.ReferenceData
{
    public abstract class MemberReferenceTests
    {
        protected MemberReferenceFactory Factory { get; } = new MemberReferenceFactory();

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