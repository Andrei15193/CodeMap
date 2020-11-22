using System;
using System.Linq;
using System.Reflection;
using CodeMap.ReferenceData;
using Xunit;

namespace CodeMap.Tests.ReferenceData
{
    public class GenericMethodParameterReferenceTests : MemberReferenceTests
    {
        [Fact]
        public void CreateReferenceFromGenericMethodParameterType()
        {
            var genericParameterReference = (GenericMethodParameterReference)Factory.Create(_GetGenericMethodParameter());
            var visitor = new MemberReferenceVisitorMock<GenericMethodParameterReference>(genericParameterReference);

            Assert.Equal("T", genericParameterReference.Name);
            Assert.Equal(0, genericParameterReference.Position);
            Assert.True(genericParameterReference.DeclaringMethod == _GetMethodInfo());
            Assert.True(genericParameterReference == _GetGenericMethodParameter());
            Assert.True(genericParameterReference.Assembly == typeof(string).Assembly);

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
    }
}