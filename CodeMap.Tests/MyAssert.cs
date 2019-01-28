using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeMap.Elements;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests
{
    internal static class MyAssert
    {
        public static EnumDocumentationElement AssertDocumentation(this EnumDocumentationElement enumDocumentationElement, MemberDocumentation memberDocumentation)
        {
            _AssertDocumentation(enumDocumentationElement, memberDocumentation);
            return enumDocumentationElement;
        }

        public static DelegateDocumentationElement AssertDocumentation(this DelegateDocumentationElement delegateDocumentationElement, MemberDocumentation memberDocumentation)
        {
            _AssertDocumentation(delegateDocumentationElement, memberDocumentation);
            foreach (var genericParameterPosition in Enumerable.Range(0, 1))
            {
                var genericParameterName = $"TParam{genericParameterPosition + 1}";
                Assert.Contains(
                    delegateDocumentationElement.GenericParameters,
                    genericParameter => genericParameter.Name == genericParameterName
                );

                memberDocumentation.AssertSameItems(
                    () => memberDocumentation.GenericParameters[genericParameterName],
                    delegateDocumentationElement.GenericParameters[genericParameterPosition].Description
                );
            }
            foreach (var parameterPosition in Enumerable.Range(0, 38))
            {
                var parameterName = $"param{parameterPosition + 1}";
                Assert.Contains(
                    delegateDocumentationElement.Parameters,
                    parameter => parameter.Name == parameterName
                );
                memberDocumentation.AssertSameItems(
                    () => memberDocumentation.Parameters[parameterName],
                    delegateDocumentationElement.Parameters[parameterPosition].Description
                );
            }
            Assert.Same(memberDocumentation.Returns, delegateDocumentationElement.Return.Description);

            memberDocumentation
                .AssertSameItems(
                    () => memberDocumentation.Exceptions["T:System.ArgumentException"],
                    delegateDocumentationElement
                        .Exceptions
                        .Single(exception => exception.Type == typeof(ArgumentException))
                        .Description
                )
                .AssertSameItems(
                    () => memberDocumentation.Exceptions["T:System.ArgumentNullException"],
                    delegateDocumentationElement
                        .Exceptions
                        .Single(exception => exception.Type == typeof(ArgumentNullException))
                        .Description
                );

            return delegateDocumentationElement;
        }

        public static InterfaceDocumentationElement AssertDocumentation(this InterfaceDocumentationElement interfaceDocumentationElement, MemberDocumentation memberDocumentation)
        {
            _AssertDocumentation(interfaceDocumentationElement, memberDocumentation);
            return interfaceDocumentationElement;
        }

        private static TypeDocumentationElement _AssertDocumentation(TypeDocumentationElement typeDocumentationElement, MemberDocumentation memberDocumentation)
        {
            Assert.Same(memberDocumentation.Summary, typeDocumentationElement.Summary);
            Assert.Same(memberDocumentation.Remarks, typeDocumentationElement.Remarks);
            Assert.Same(memberDocumentation.Examples, typeDocumentationElement.Examples);
            Assert.Same(memberDocumentation.RelatedMembers, typeDocumentationElement.RelatedMembers);
            return typeDocumentationElement;
        }

        public static EnumDocumentationElement AssertNoDocumentation(this EnumDocumentationElement enumDocumentationElement)
        {
            _AssertNoDocumentation(enumDocumentationElement);
            return enumDocumentationElement;
        }

        public static DelegateDocumentationElement AssertNoDocumentation(this DelegateDocumentationElement delegateDocumentationElement)
        {
            _AssertNoDocumentation(delegateDocumentationElement);
            foreach (var genericParameter in delegateDocumentationElement.GenericParameters)
                Assert.Empty(genericParameter.Description);
            foreach (var parameter in delegateDocumentationElement.Parameters)
                Assert.Empty(parameter.Description);
            Assert.Empty(delegateDocumentationElement.Return.Description);
            Assert.Empty(delegateDocumentationElement.Exceptions);
            return delegateDocumentationElement;
        }

        public static InterfaceDocumentationElement AssertNoDocumentation(this InterfaceDocumentationElement interfaceDocumentationElement)
        {
            _AssertNoDocumentation(interfaceDocumentationElement);
            return interfaceDocumentationElement;
        }

        private static TypeDocumentationElement _AssertNoDocumentation(TypeDocumentationElement typeDocumentationElement)
        {
            Assert.Empty(typeDocumentationElement.Summary.Content);
            Assert.Empty(typeDocumentationElement.Remarks.Content);
            Assert.Empty(typeDocumentationElement.Examples);
            Assert.Empty(typeDocumentationElement.RelatedMembers);
            return typeDocumentationElement;
        }

        public static ConstantDocumentationElement AssertDocumentation(this ConstantDocumentationElement constantDocumentationElement, MemberDocumentation memberDocumentation)
        {
            _AssertDocumentation(constantDocumentationElement, memberDocumentation);
            return constantDocumentationElement;
        }

        public static EventDocumentationElement AssertDocumentation(this EventDocumentationElement eventDocumentationElement, MemberDocumentation memberDocumentation)
        {
            _AssertDocumentation(eventDocumentationElement, memberDocumentation);
            Assert.Same(memberDocumentation.Exceptions, eventDocumentationElement.Exceptions);
            return eventDocumentationElement;
        }

        public static PropertyDocumentationElement AssertDocumentation(this PropertyDocumentationElement propertyDocumentationElement, MemberDocumentation memberDocumentation)
        {
            _AssertDocumentation(propertyDocumentationElement, memberDocumentation);
            foreach (var parameter in propertyDocumentationElement.Parameters)
            {
                Assert.True(memberDocumentation.Parameters.Contains(parameter.Name));
                Assert.Same(memberDocumentation.Parameters[parameter.Name], parameter.Description);
            }
            Assert.Same(memberDocumentation.Value, propertyDocumentationElement.Value);
            Assert.Same(memberDocumentation.Exceptions, propertyDocumentationElement.Exceptions);
            return propertyDocumentationElement;
        }

        private static MemberDocumentationElement _AssertDocumentation(this MemberDocumentationElement typeDocumentationElement, MemberDocumentation memberDocumentation)
        {
            Assert.Same(memberDocumentation.Summary, typeDocumentationElement.Summary);
            Assert.Same(memberDocumentation.Remarks, typeDocumentationElement.Remarks);
            Assert.Same(memberDocumentation.Examples, typeDocumentationElement.Examples);
            Assert.Same(memberDocumentation.RelatedMembers, typeDocumentationElement.RelatedMembers);
            return typeDocumentationElement;
        }

        public static ConstantDocumentationElement AssertNoDocumentation(this ConstantDocumentationElement constantDocumentationElement)
        {
            _AssertNoDocumentation(constantDocumentationElement);
            return constantDocumentationElement;
        }

        public static EventDocumentationElement AssertNoDocumentation(this EventDocumentationElement eventDocumentationElement)
        {
            _AssertNoDocumentation(eventDocumentationElement);
            Assert.Empty(eventDocumentationElement.Exceptions);
            return eventDocumentationElement;
        }

        public static PropertyDocumentationElement AssertNoDocumentation(this PropertyDocumentationElement propertyDocumentationElement)
        {
            _AssertNoDocumentation(propertyDocumentationElement);
            foreach (var parameter in propertyDocumentationElement.Parameters)
                Assert.Empty(parameter.Description);
            Assert.Empty(propertyDocumentationElement.Value.Content);
            Assert.Empty(propertyDocumentationElement.Exceptions);
            return propertyDocumentationElement;
        }

        private static MemberDocumentationElement _AssertNoDocumentation(this MemberDocumentationElement memberDocumentationElement)
        {
            Assert.Empty(memberDocumentationElement.Summary.Content);
            Assert.Empty(memberDocumentationElement.Remarks.Content);
            Assert.Empty(memberDocumentationElement.Examples);
            Assert.Empty(memberDocumentationElement.RelatedMembers);
            return memberDocumentationElement;
        }

        public static TInstance AssertAttributes<TInstance>(this TInstance instance, Func<IEnumerable<AttributeData>> selector, string valuePrefix)
            => instance
                .AssertCollectionMember(
                    selector,
                    attribute => attribute
                        .AssertType(() => attribute.Type, typeof(TestAttribute))
                        .AssertCollectionMember(
                            () => attribute.PositionalParameters,
                            positionalParameter => positionalParameter
                                .AssertEqual(() => positionalParameter.Name, "value1")
                                .AssertEqual(() => positionalParameter.Value, $"{valuePrefix} test 1")
                                .AssertType(() => positionalParameter.Type, typeof(object))
                        )
                        .AssertCollectionMember(
                            () => attribute.NamedParameters.OrderBy(namedParameter => namedParameter.Name),
                            namedParameter => namedParameter
                                .AssertEqual(() => namedParameter.Name, "Value2")
                                .AssertEqual(() => namedParameter.Value, $"{valuePrefix} test 2")
                                .AssertType(() => namedParameter.Type, typeof(object)),
                            namedParameter => namedParameter
                                .AssertEqual(() => namedParameter.Name, "Value3")
                                .AssertEqual(() => namedParameter.Value, $"{valuePrefix} test 3")
                                .AssertType(() => namedParameter.Type, typeof(object))
                        )
                );

        public static TInstance AssertType<TInstance>(this TInstance instance, Func<TypeReferenceDocumentationElement> selector, Type type)
        {
            var typeReference = selector();
            Assert.True(typeReference == type);
            Assert.True(type == typeReference);
            Assert.False(typeReference != type);
            Assert.False(type != typeReference);

            var otherType = type == typeof(object) ? typeof(string) : typeof(object);
            Assert.True(typeReference != otherType);
            Assert.True(otherType != typeReference);
            return instance;
        }

        public static TInstance AssertDynamicType<TInstance>(this TInstance instance, Func<TypeReferenceDocumentationElement> typeSelector, Func<IEnumerable<AttributeData>> attributesSelector, bool[] transformFlags = null)
            => instance
                .AssertType(typeSelector, typeof(object))
                .AssertMember(typeSelector, type => type.AssertIs<DynamicTypeReferenceDocumentationElement>())
                .AssertCollectionMemberContains(
                    attributesSelector,
                    attribute => attribute
                        .AssertType(() => attribute.Type, typeof(DynamicAttribute))
                        .AssertMember(
                            () => attribute.PositionalParameters,
                            positionalParameters =>
                            {
                                if (transformFlags == null)
                                    positionalParameters.AssertEmpty(() => positionalParameters);
                                else
                                    positionalParameters.AssertCollectionMember(
                                        () => positionalParameters,
                                        positionalParameter =>
                                            positionalParameter
                                                .AssertEqual(() => positionalParameter.Name, "transformFlags")
                                                .AssertType(() => positionalParameter.Type, typeof(bool[]))
                                                .AssertEqual(() => positionalParameter.Value, transformFlags)
                                    );
                            }
                        )
                        .AssertEmpty(() => attribute.NamedParameters)
                );

        public static TInstance AssertOutputParameterAttribute<TInstance>(this TInstance instance, Func<IEnumerable<AttributeData>> attributesSelector)
            => instance
                .AssertCollectionMemberContains(
                    attributesSelector,
                    attribute => attribute
                        .AssertType(() => attribute.Type, typeof(OutAttribute))
                        .AssertEmpty(() => attribute.PositionalParameters)
                        .AssertEmpty(() => attribute.NamedParameters)
                );

        public static TInstance AssertOptionalParameterAttribute<TInstance>(this TInstance instance, Func<IEnumerable<AttributeData>> attributesSelector)
            => instance
                .AssertCollectionMemberContains(
                    attributesSelector,
                    attribute => attribute
                        .AssertType(() => attribute.Type, typeof(OptionalAttribute))
                        .AssertEmpty(() => attribute.PositionalParameters)
                        .AssertEmpty(() => attribute.NamedParameters)
                );

        public static TTypeDocumentationElement AssertTypeGenericParameters<TTypeDocumentationElement>(this TTypeDocumentationElement typeDcouemntationElement, Func<IEnumerable<GenericParameterDocumentationElement>> selector)
            where TTypeDocumentationElement : TypeDocumentationElement
            => typeDcouemntationElement
                .AssertCollectionMember(
                    selector,
                    genericParameter => genericParameter
                        .AssertEqual(() => genericParameter.Name, "TParam1")
                        .AssertEqual(() => genericParameter.Position, 0)
                        .AssertFalse(() => genericParameter.IsCovariant)
                        .AssertFalse(() => genericParameter.IsContravariant)
                        .AssertFalse(() => genericParameter.HasReferenceTypeConstraint)
                        .AssertFalse(() => genericParameter.HasNonNullableValueTypeConstraint)
                        .AssertFalse(() => genericParameter.HasDefaultConstructorConstraint)
                        .AssertEmpty(() => genericParameter.TypeConstraints)
                        .AssertIs<TypeGenericParameterDocumentationElement>(
                            typeGenericParameter => typeGenericParameter.AssertSame(() => typeGenericParameter.DeclaringType, typeDcouemntationElement)
                        )
                );

        public static DelegateDocumentationElement AssertDelegateParameters(this DelegateDocumentationElement delegateDocumentationElement, Type genericParameter)
            => delegateDocumentationElement
                .AssertCollectionMember(
                    () => delegateDocumentationElement.Parameters,
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param1")
                        .AssertAttributes(() => parameter.Attributes, "delegate parameter")
                        .AssertType(() => parameter.Type, typeof(int))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param2")
                        .AssertEmpty(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(byte[]))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param3")
                        .AssertEmpty(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(char[][]))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param4")
                        .AssertEmpty(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(double[,]))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param5")
                        .AssertEmpty(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(int))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertTrue(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param6")
                        .AssertEmpty(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(byte[]))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertTrue(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param7")
                        .AssertEmpty(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(char[][]))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertTrue(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param8")
                        .AssertEmpty(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(double[,]))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertTrue(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param9")
                        .AssertEqual(() => parameter.Attributes.Count, 1)
                        .AssertOutputParameterAttribute(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(int))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertTrue(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param10")
                        .AssertEqual(() => parameter.Attributes.Count, 1)
                        .AssertOutputParameterAttribute(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(byte[]))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertTrue(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param11")
                        .AssertEqual(() => parameter.Attributes.Count, 1)
                        .AssertOutputParameterAttribute(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(char[][]))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertTrue(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param12")
                        .AssertEqual(() => parameter.Attributes.Count, 1)
                        .AssertOutputParameterAttribute(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(double[,]))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertTrue(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param13")
                        .AssertEmpty(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(TestClass<int>.NestedTestClass<byte, IEnumerable<string>>))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param14")
                        .AssertEmpty(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(TestClass<int>.NestedTestClass<byte, IEnumerable<string>>[]))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param15")
                        .AssertEmpty(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(TestClass<int>.NestedTestClass<byte, IEnumerable<string>>))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertTrue(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param16")
                        .AssertEqual(() => parameter.Attributes.Count, 1)
                        .AssertOutputParameterAttribute(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(TestClass<int>.NestedTestClass<byte, IEnumerable<string>>))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertTrue(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param17")
                        .AssertEmpty(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(TestClass<int>.NestedTestClass<byte, IEnumerable<string>>[]))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertTrue(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param18")
                        .AssertEqual(() => parameter.Attributes.Count, 1)
                        .AssertOutputParameterAttribute(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(TestClass<int>.NestedTestClass<byte, IEnumerable<string>>[]))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertTrue(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param19")
                        .AssertEqual(() => parameter.Attributes.Count, 1)
                        .AssertDynamicType(() => parameter.Type, () => parameter.Attributes)
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param20")
                        .AssertEqual(() => parameter.Attributes.Count, 1)
                        .AssertDynamicType(() => parameter.Type, () => parameter.Attributes, new[] { false, true })
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertTrue(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param21")
                        .AssertEqual(() => parameter.Attributes.Count, 2)
                        .AssertDynamicType(() => parameter.Type, () => parameter.Attributes, new[] { false, true })
                        .AssertOutputParameterAttribute(() => parameter.Attributes)
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertTrue(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param22")
                        .AssertEmpty(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, genericParameter)
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param23")
                        .AssertEmpty(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, genericParameter)
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertTrue(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param24")
                        .AssertEqual(() => parameter.Attributes.Count, 1)
                        .AssertOutputParameterAttribute(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, genericParameter)
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertTrue(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param25")
                        .AssertEmpty(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(int*))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param26")
                        .AssertEmpty(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(byte*[]))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param27")
                        .AssertEmpty(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(char*))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertTrue(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param28")
                        .AssertEqual(() => parameter.Attributes.Count, 1)
                        .AssertOutputParameterAttribute(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(double*))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertTrue(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param29")
                        .AssertEmpty(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(decimal*[]))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertTrue(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param30")
                        .AssertEqual(() => parameter.Attributes.Count, 1)
                        .AssertOutputParameterAttribute(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(short*[]))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertTrue(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param31")
                        .AssertEmpty(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(void*))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param32")
                        .AssertEmpty(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(void**))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param33")
                        .AssertEmpty(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(void**))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertTrue(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param34")
                        .AssertEqual(() => parameter.Attributes.Count, 1)
                        .AssertOutputParameterAttribute(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(void**))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertTrue(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param35")
                        .AssertEmpty(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(void**[]))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param36")
                        .AssertEmpty(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(void**[]))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertTrue(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param37")
                        .AssertEqual(() => parameter.Attributes.Count, 1)
                        .AssertOutputParameterAttribute(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(void**[]))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertTrue(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param38")
                        .AssertEqual(() => parameter.Attributes.Count, 1)
                        .AssertOptionalParameterAttribute(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeof(string))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertTrue(() => parameter.HasDefaultValue)
                        .AssertEqual(() => parameter.DefaultValue, "test")
                );

        public static TConcrete AssertIs<TConcrete>(this object instance)
        {
            Assert.IsType<TConcrete>(instance);
            return (TConcrete)instance;
        }

        public static TConcrete AssertIs<TConcrete>(this object instance, Action<TConcrete> callback)
        {
            Assert.IsType<TConcrete>(instance);
            var concrete = (TConcrete)instance;
            callback(concrete);
            return concrete;
        }

        [Obsolete("Equality comparisons have been implemented for instances that resemble .NET types, use them instead")]
        public static TInstance AssertTypeReference<TInstance>(this TInstance instance, string @namespace, string name)
            where TInstance : TypeReferenceDocumentationElement
        {
            instance
                .AssertIs<InstanceTypeDocumentationElement>(
                    instanceTypeReference =>
                        instanceTypeReference
                            .AssertEqual(() => instanceTypeReference.Name, name)
                            .AssertEqual(() => instanceTypeReference.Namespace, @namespace)
                );
            return instance;
        }

        [Obsolete("Equality comparisons have been implemented for instances that resemble .NET types, use them instead")]
        public static TInstance AssertTypeReference<TInstance>(this TInstance instance, Func<TypeReferenceDocumentationElement> selector, string @namespace, string name)
        {
            selector().AssertTypeReference(@namespace, name);
            return instance;
        }

        public static TInstance AssertGenericArguments<TInstance>(this TInstance instance, params Action<TypeReferenceDocumentationElement>[] callbacks)
            where TInstance : TypeReferenceDocumentationElement
        {
            instance.AssertIs<InstanceTypeDocumentationElement>(
                instanceTypeReference => instanceTypeReference.AssertCollectionMember(() => instanceTypeReference.GenericArguments, callbacks)
            );
            return instance;
        }

        public static TInstance AssertGenericArguments<TInstance>(this TInstance instance, Func<TypeReferenceDocumentationElement> selector, params Action<TypeReferenceDocumentationElement>[] callbacks)
        {
            selector().AssertGenericArguments(callbacks);
            return instance;
        }

        [Obsolete("Equality comparisons have been implemented for instances that resemble .NET types, use them instead")]
        public static TInstance AssertTypeReferenceAssembly<TInstance>(this TInstance instance, string name, Version version)
            where TInstance : TypeReferenceDocumentationElement
        {
            instance
                .AssertIs<InstanceTypeDocumentationElement>(
                    instanceTypeReference =>
                        instanceTypeReference.AssertMember(
                            () => instanceTypeReference.Assembly,
                            assembly => assembly
                                .AssertEqual(() => assembly.Name, name)
                                .AssertEqual(() => assembly.Version, version)
                        )
                );
            return instance;
        }

        [Obsolete("Equality comparisons have been implemented for instances that resemble .NET types, use them instead")]
        public static TInstance AssertTypeReferenceAssembly<TInstance>(this TInstance instance, string name, Version version, string culture, string publicKeyToken)
            where TInstance : TypeReferenceDocumentationElement
        {
            instance
                .AssertIs<InstanceTypeDocumentationElement>(
                    instanceTypeReference =>
                        instanceTypeReference.AssertMember(
                            () => instanceTypeReference.Assembly,
                            assembly => assembly
                                .AssertEqual(() => assembly.Name, name)
                                .AssertEqual(() => assembly.Version, version)
                                .AssertEqual(() => assembly.Culture, culture)
                                .AssertEqual(() => assembly.PublicKeyToken, publicKeyToken)
                        )
                );
            return instance;
        }

        [Obsolete("Equality comparisons have been implemented for instances that resemble .NET types, use them instead")]
        public static TInstance AssertTypeReferenceAssembly<TInstance>(this TInstance instance, Func<TypeReferenceDocumentationElement> selector, string name, Version version)
        {
            selector().AssertTypeReferenceAssembly(name, version);
            return instance;
        }

        [Obsolete("Equality comparisons have been implemented for instances that resemble .NET types, use them instead")]
        public static TInstance AssertTypeReferenceAssembly<TInstance>(this TInstance instance, Func<TypeReferenceDocumentationElement> selector, string name, Version version, string culture, string publicKeyToken)
        {
            selector().AssertTypeReferenceAssembly(name, version, culture, publicKeyToken);
            return instance;
        }

        public static TInstance AssertMember<TInstance, TMember>(this TInstance instance, Func<TMember> selector, Action<TMember> callback)
        {
            callback(selector());
            return instance;
        }

        public static TInstance AssertCollectionMember<TInstance, TItem>(this TInstance instance, Func<IEnumerable<TItem>> selector, params Action<TItem>[] callbacks)
        {
            var items = selector();
            var itemsList = items as IReadOnlyCollection<TItem> ?? items.ToList();
            Assert.Equal(callbacks.Length, itemsList.Count);
            foreach (var pair in callbacks.Zip(itemsList, (callback, item) => new { Callback = callback, Item = item }))
                pair.Callback(pair.Item);
            return instance;
        }

        public static TInstance AssertCollectionMemberContains<TInstance, TItem>(this TInstance instance, Func<IEnumerable<TItem>> selector, Action<TItem> callback)
        {
            Assert.Contains(
                selector(),
                item =>
                {
                    try
                    {
                        callback(item);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            );
            return instance;
        }

        public static TInstance AssertEmpty<TInstance, TItem>(this TInstance instance, Func<IEnumerable<TItem>> selector)
        {
            Assert.Empty(selector());
            return instance;
        }

        public static TInstance AssertEqual<TInstance, TValue>(this TInstance instance, Func<TValue> selector, TValue expected)
        {
            var actual = selector();

            if (expected is string && actual is string)
                Assert.Equal(expected, actual);
            if (expected is IEnumerable expectedCollection && actual is IEnumerable actualCollection)
                Assert.True(expectedCollection.Cast<object>().SequenceEqual(actualCollection.Cast<object>()));
            else
                Assert.Equal(expected, actual);

            return instance;
        }

        public static TInstance AssertTrue<TInstance>(this TInstance instance, Func<bool> selector)
        {
            Assert.True(selector());
            return instance;
        }

        public static TInstance AssertFalse<TInstance>(this TInstance instance, Func<bool> selector)
        {
            Assert.False(selector());
            return instance;
        }

        public static TInstance AssertSame<TInstance>(this TInstance instance, TInstance expected)
            where TInstance : class
        {
            Assert.Same(expected, instance);
            return instance;
        }

        public static TInstance AssertSame<TInstance, TValue>(this TInstance instance, Func<TValue> selector, TValue expected) where TValue : class
        {
            Assert.Same(expected, selector());
            return instance;
        }

        public static TInstance AssertSameItems<TInstance, TItem>(this TInstance instance, Func<IEnumerable<TItem>> selector, IEnumerable<TItem> expected) where TItem : class
        {
            var actual = selector();
            var expectedItems = expected as IReadOnlyCollection<TItem> ?? expected.ToList();
            var actualItems = actual as IReadOnlyCollection<TItem> ?? actual.ToList();
            Assert.Equal(expectedItems.Count, actualItems.Count);
            foreach (var pair in expectedItems.Zip(actualItems, (expectedItem, actualItem) => new { ExpectedItem = expectedItem, ActualItem = actualItem }))
                Assert.Same(pair.ExpectedItem, pair.ActualItem);

            return instance;
        }

        public static TInstance AssertNull<TInstance, TValue>(this TInstance instance, Func<TValue> selector) where TValue : class
        {
            Assert.Null(selector());
            return instance;
        }
    }
}