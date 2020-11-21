using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeMap.ReferenceData;
using Xunit;

namespace CodeMap.Tests.ReferenceData
{
    public class MethodReferenceTests : MemberReferenceTests
    {
        [Fact]
        public void CreateReferenceFromSimpleMethodInfo()
        {
            var methodReference = (MethodReference)Factory.Create(_GetToStringMethodInfo());
            var visitor = new MemberReferenceVisitorMock<MethodReference>(methodReference);

            Assert.Equal("ToString", methodReference.Name);
            Assert.Empty(methodReference.GenericArguments);
            Assert.True(methodReference.DeclaringType == typeof(object));
            Assert.Empty(methodReference.ParameterTypes);
            Assert.True(methodReference == _GetToStringMethodInfo());
            Assert.True(methodReference != _GetGetHashCodeMethodInfo());

            methodReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);

            MethodInfo _GetToStringMethodInfo()
                => typeof(object).GetMethods().Single(methodInfo => methodInfo.Name == nameof(object.ToString));

            MethodInfo _GetGetHashCodeMethodInfo()
                => typeof(object).GetMethods().Single(methodInfo => methodInfo.Name == nameof(object.GetHashCode));
        }

        [Fact]
        public void CreateReferenceFromConstructedGenericMethodInfo()
        {
            var methodReference = (MethodReference)Factory.Create(_GetMethodInfo());
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
                        methodInfo => methodInfo.Name == nameof(string.Join)
                            && methodInfo.IsGenericMethod
                            && methodInfo.GetParameters().First().ParameterType == typeof(string)
                    )
                    .MakeGenericMethod(typeof(int));
        }
        [Fact]
        public void CreateReferenceFromGenericDefinitionMethodInfo()
        {
            var methodReference = (MethodReference)Factory.Create(_GetMethodInfo());
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
                        methodInfo => methodInfo.Name == nameof(string.Join)
                            && methodInfo.IsGenericMethod
                            && methodInfo.GetParameters().First().ParameterType == typeof(string)
                    );

            Type _GetGenericArgument()
                => _GetMethodInfo().GetGenericArguments().Single();
        }
    }
}