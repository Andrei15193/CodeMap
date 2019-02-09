using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

            foreach (var genericParameterPosition in Enumerable.Range(0, 1))
            {
                var genericParameterName = $"TParam{genericParameterPosition + 1}";
                Assert.Contains(
                    interfaceDocumentationElement.GenericParameters,
                    genericParameter => genericParameter.Name == genericParameterName
                );

                memberDocumentation.AssertSameItems(
                    () => memberDocumentation.GenericParameters[genericParameterName],
                    interfaceDocumentationElement.GenericParameters[genericParameterPosition].Description
                );
            }

            return interfaceDocumentationElement;
        }

        public static ClassDocumentationElement AssertDocumentation(this ClassDocumentationElement classDocumentationElement, MemberDocumentation memberDocumentation)
        {
            _AssertDocumentation(classDocumentationElement, memberDocumentation);

            foreach (var genericParameterPosition in Enumerable.Range(0, 1))
            {
                var genericParameterName = $"TParam{genericParameterPosition + 1}";
                Assert.Contains(
                    classDocumentationElement.GenericParameters,
                    genericParameter => genericParameter.Name == genericParameterName
                );

                memberDocumentation.AssertSameItems(
                    () => memberDocumentation.GenericParameters[genericParameterName],
                    classDocumentationElement.GenericParameters[genericParameterPosition].Description
                );
            }

            return classDocumentationElement;
        }

        public static StructDocumentationElement AssertDocumentation(this StructDocumentationElement structDocumentationElement, MemberDocumentation memberDocumentation)
        {
            _AssertDocumentation(structDocumentationElement, memberDocumentation);

            foreach (var genericParameterPosition in Enumerable.Range(0, 1))
            {
                var genericParameterName = $"TParam{genericParameterPosition + 1}";
                Assert.Contains(
                    structDocumentationElement.GenericParameters,
                    genericParameter => genericParameter.Name == genericParameterName
                );

                memberDocumentation.AssertSameItems(
                    () => memberDocumentation.GenericParameters[genericParameterName],
                    structDocumentationElement.GenericParameters[genericParameterPosition].Description
                );
            }

            return structDocumentationElement;
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
            foreach (var genericParameter in interfaceDocumentationElement.GenericParameters)
                Assert.Empty(genericParameter.Description);
            foreach (var @event in interfaceDocumentationElement.Events)
                AssertNoDocumentation(@event);
            foreach (var property in interfaceDocumentationElement.Properties)
                AssertNoDocumentation(property);
            foreach (var method in interfaceDocumentationElement.Methods)
                AssertNoDocumentation(method);
            return interfaceDocumentationElement;
        }

        public static ClassDocumentationElement AssertNoDocumentation(this ClassDocumentationElement classDocumentationElement)
        {
            _AssertNoDocumentation(classDocumentationElement);
            foreach (var genericParameter in classDocumentationElement.GenericParameters)
                Assert.Empty(genericParameter.Description);
            foreach (var constant in classDocumentationElement.Constants)
                AssertNoDocumentation(constant);
            foreach (var field in classDocumentationElement.Fields)
                AssertNoDocumentation(field);
            foreach (var constructor in classDocumentationElement.Constructors)
                AssertNoDocumentation(constructor);
            foreach (var @event in classDocumentationElement.Events)
                AssertNoDocumentation(@event);
            foreach (var property in classDocumentationElement.Properties)
                AssertNoDocumentation(property);
            foreach (var method in classDocumentationElement.Methods)
                AssertNoDocumentation(method);
            return classDocumentationElement;
        }

        public static StructDocumentationElement AssertNoDocumentation(this StructDocumentationElement structDocumentationElement)
        {
            _AssertNoDocumentation(structDocumentationElement);
            foreach (var genericParameter in structDocumentationElement.GenericParameters)
                Assert.Empty(genericParameter.Description);
            foreach (var constant in structDocumentationElement.Constants)
                AssertNoDocumentation(constant);
            foreach (var field in structDocumentationElement.Fields)
                AssertNoDocumentation(field);
            foreach (var constructor in structDocumentationElement.Constructors)
                AssertNoDocumentation(constructor);
            foreach (var @event in structDocumentationElement.Events)
                AssertNoDocumentation(@event);
            foreach (var property in structDocumentationElement.Properties)
                AssertNoDocumentation(property);
            foreach (var method in structDocumentationElement.Methods)
                AssertNoDocumentation(method);
            return structDocumentationElement;
        }

        private static void _AssertNoDocumentation(EventDocumentationElement @event)
        {
            Assert.Empty(@event.Summary.Content);
            Assert.Empty(@event.Remarks.Content);
            Assert.Empty(@event.Examples);
            Assert.Empty(@event.RelatedMembers);
            Assert.Empty(@event.Exceptions);
        }

        private static void _AssertNoDocumentation(PropertyDocumentationElement property)
        {
            Assert.Empty(property.Summary.Content);
            foreach (var parameter in property.Parameters)
                Assert.Empty(parameter.Description);
            Assert.Empty(property.Value.Content);
            Assert.Empty(property.Exceptions);
            Assert.Empty(property.Remarks.Content);
            Assert.Empty(property.Examples);
            Assert.Empty(property.RelatedMembers);
        }

        private static void _AssertNoDocumentation(MethodDocumentationElement method)
        {
            Assert.Empty(method.Summary.Content);
            foreach (var genericParameter in method.GenericParameters)
                Assert.Empty(genericParameter.Description);
            foreach (var parameter in method.Parameters)
                Assert.Empty(parameter.Description);
            Assert.Empty(method.Exceptions);
            Assert.Empty(method.Remarks.Content);
            Assert.Empty(method.Examples);
            Assert.Empty(method.RelatedMembers);
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

        public static FieldDocumentationElement AssertDocumentation(this FieldDocumentationElement fieldDocumentationElement, MemberDocumentation memberDocumentation)
        {
            _AssertDocumentation(fieldDocumentationElement, memberDocumentation);
            return fieldDocumentationElement;
        }

        public static ConstructorDocumentationElement AssertDocumentation(this ConstructorDocumentationElement constructorDocumentationElement, MemberDocumentation memberDocumentation)
        {
            _AssertDocumentation(constructorDocumentationElement, memberDocumentation);
            foreach (var parameter in constructorDocumentationElement.Parameters)
            {
                Assert.True(memberDocumentation.Parameters.Contains(parameter.Name));
                memberDocumentation
                    .AssertSameItems(
                        () => memberDocumentation.Parameters[parameter.Name],
                        parameter.Description
                    );
            }
            memberDocumentation
                .AssertSameItems(
                    () => memberDocumentation.Exceptions["T:System.ArgumentException"],
                    constructorDocumentationElement
                        .Exceptions
                        .Single(exception => exception.Type == typeof(ArgumentException))
                        .Description
                )
                .AssertSameItems(
                    () => memberDocumentation.Exceptions["T:System.ArgumentNullException"],
                    constructorDocumentationElement
                        .Exceptions
                        .Single(exception => exception.Type == typeof(ArgumentNullException))
                        .Description
                );
            return constructorDocumentationElement;
        }

        public static EventDocumentationElement AssertDocumentation(this EventDocumentationElement eventDocumentationElement, MemberDocumentation memberDocumentation)
        {
            _AssertDocumentation(eventDocumentationElement, memberDocumentation);
            memberDocumentation
                .AssertSameItems(
                    () => memberDocumentation.Exceptions["T:System.ArgumentException"],
                    eventDocumentationElement
                        .Exceptions
                        .Single(exception => exception.Type == typeof(ArgumentException))
                        .Description
                )
                .AssertSameItems(
                    () => memberDocumentation.Exceptions["T:System.ArgumentNullException"],
                    eventDocumentationElement
                        .Exceptions
                        .Single(exception => exception.Type == typeof(ArgumentNullException))
                        .Description
                );
            return eventDocumentationElement;
        }

        public static PropertyDocumentationElement AssertDocumentation(this PropertyDocumentationElement propertyDocumentationElement, MemberDocumentation memberDocumentation)
        {
            _AssertDocumentation(propertyDocumentationElement, memberDocumentation);
            foreach (var parameter in propertyDocumentationElement.Parameters)
            {
                Assert.True(memberDocumentation.Parameters.Contains(parameter.Name));
                memberDocumentation
                    .AssertSameItems(
                        () => memberDocumentation.Parameters[parameter.Name],
                        parameter.Description
                    );
            }
            Assert.Same(memberDocumentation.Value, propertyDocumentationElement.Value);
            memberDocumentation
                .AssertSameItems(
                    () => memberDocumentation.Exceptions["T:System.ArgumentException"],
                    propertyDocumentationElement
                        .Exceptions
                        .Single(exception => exception.Type == typeof(ArgumentException))
                        .Description
                )
                .AssertSameItems(
                    () => memberDocumentation.Exceptions["T:System.ArgumentNullException"],
                    propertyDocumentationElement
                        .Exceptions
                        .Single(exception => exception.Type == typeof(ArgumentNullException))
                        .Description
                );
            return propertyDocumentationElement;
        }

        public static MethodDocumentationElement AssertDocumentation(this MethodDocumentationElement methodDocumentationElement, MemberDocumentation memberDocumentation)
        {
            _AssertDocumentation(methodDocumentationElement, memberDocumentation);
            foreach (var genericParameter in methodDocumentationElement.GenericParameters)
            {
                Assert.True(memberDocumentation.GenericParameters.Contains(genericParameter.Name));
                memberDocumentation
                    .AssertSameItems(
                        () => memberDocumentation.GenericParameters[genericParameter.Name],
                        genericParameter.Description
                    );
            }
            foreach (var parameter in methodDocumentationElement.Parameters)
            {
                Assert.True(memberDocumentation.Parameters.Contains(parameter.Name));
                memberDocumentation
                    .AssertSameItems(
                        () => memberDocumentation.Parameters[parameter.Name],
                        parameter.Description
                    );
            }
            memberDocumentation
                .AssertSameItems(
                    () => memberDocumentation.Exceptions["T:System.ArgumentException"],
                    methodDocumentationElement
                        .Exceptions
                        .Single(exception => exception.Type == typeof(ArgumentException))
                        .Description
                )
                .AssertSameItems(
                    () => memberDocumentation.Exceptions["T:System.ArgumentNullException"],
                    methodDocumentationElement
                        .Exceptions
                        .Single(exception => exception.Type == typeof(ArgumentNullException))
                        .Description
                );
            return methodDocumentationElement;
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

        public static FieldDocumentationElement AssertNoDocumentation(this FieldDocumentationElement fieldDocumentationElement)
        {
            _AssertNoDocumentation(fieldDocumentationElement);
            return fieldDocumentationElement;
        }

        public static ConstructorDocumentationElement AssertNoDocumentation(this ConstructorDocumentationElement constructorDocumentationElement)
        {
            _AssertNoDocumentation(constructorDocumentationElement);
            foreach (var parameter in constructorDocumentationElement.Parameters)
                Assert.Empty(parameter.Description);
            Assert.Empty(constructorDocumentationElement.Exceptions);
            return constructorDocumentationElement;
        }

        public static EventDocumentationElement AssertNoDocumentation(this EventDocumentationElement eventDocumentationElement)
        {
            _AssertNoDocumentation((MemberDocumentationElement)eventDocumentationElement);
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

        public static MethodDocumentationElement AssertNoDocumentation(this MethodDocumentationElement methodDocumentationElement)
        {
            _AssertNoDocumentation(methodDocumentationElement);
            foreach (var genericParameter in methodDocumentationElement.GenericParameters)
                Assert.Empty(genericParameter.Description);
            foreach (var parameter in methodDocumentationElement.Parameters)
                Assert.Empty(parameter.Description);
            Assert.Empty(methodDocumentationElement.Return.Description);
            Assert.Empty(methodDocumentationElement.Exceptions);
            return methodDocumentationElement;
        }

        private static MemberDocumentationElement _AssertNoDocumentation(this MemberDocumentationElement memberDocumentationElement)
        {
            Assert.Empty(memberDocumentationElement.Summary.Content);
            Assert.Empty(memberDocumentationElement.Remarks.Content);
            Assert.Empty(memberDocumentationElement.Examples);
            Assert.Empty(memberDocumentationElement.RelatedMembers);
            return memberDocumentationElement;
        }

        public static AttributeData AssertTestAttribute(this AttributeData attributeData, string attributeValuePrefix)
            => attributeData
                .AssertType(() => attributeData.Type, typeof(TestAttribute))
                .AssertCollectionMember(
                    () => attributeData.PositionalParameters,
                    positionalParameter => positionalParameter
                        .AssertEqual(() => positionalParameter.Name, "value1")
                        .AssertEqual(() => positionalParameter.Value, $"{attributeValuePrefix} test 1")
                        .AssertType(() => positionalParameter.Type, typeof(object))
                )
                .AssertCollectionMember(
                    () => attributeData.NamedParameters.OrderBy(namedParameter => namedParameter.Name),
                    namedParameter => namedParameter
                        .AssertEqual(() => namedParameter.Name, "Value2")
                        .AssertEqual(() => namedParameter.Value, $"{attributeValuePrefix} test 2")
                        .AssertType(() => namedParameter.Type, typeof(object)),
                    namedParameter => namedParameter
                        .AssertEqual(() => namedParameter.Name, "Value3")
                        .AssertEqual(() => namedParameter.Value, $"{attributeValuePrefix} test 3")
                        .AssertType(() => namedParameter.Type, typeof(object))
                );

        public static AttributeData AssertDefaultMemberAttribute(this AttributeData attributeData)
            => attributeData
                .AssertType(() => attributeData.Type, typeof(DefaultMemberAttribute))
                .AssertCollectionMember(
                    () => attributeData.PositionalParameters,
                    positionalParameter => positionalParameter
                        .AssertEqual(() => positionalParameter.Name, "memberName")
                        .AssertEqual(() => positionalParameter.Value, "Item")
                        .AssertType(() => positionalParameter.Type, typeof(string))
                )
                .AssertEmpty(() => attributeData.NamedParameters);

        public static TTypeReference AssertType<TTypeReference>(this TTypeReference typeReference, Type type)
            where TTypeReference : TypeReferenceDocumentationElement
        {
            Assert.True(typeReference == type);
            Assert.True(type == typeReference);
            Assert.False(typeReference != type);
            Assert.False(type != typeReference);

            var otherType = type == typeof(object) ? typeof(string) : typeof(object);
            Assert.True(typeReference != otherType);
            Assert.True(otherType != typeReference);
            return typeReference;
        }

        public static FieldDocumentationElement AssertTestField(this FieldDocumentationElement fieldDocumentationElement, TypeDocumentationElement declaringType, string attributeValuePrefix)
            => fieldDocumentationElement
                .AssertEqual(() => fieldDocumentationElement.Name, "TestField")
                .AssertCollectionMember(
                    () => fieldDocumentationElement.Attributes,
                    attribute => attribute.AssertTestAttribute(attributeValuePrefix)
                )
                .AssertEqual(() => fieldDocumentationElement.AccessModifier, AccessModifier.Private)
                .AssertSame(() => fieldDocumentationElement.DeclaringType, declaringType)
                .AssertType(() => fieldDocumentationElement.Type, typeof(byte))
                .AssertFalse(() => fieldDocumentationElement.IsReadOnly)
                .AssertFalse(() => fieldDocumentationElement.IsStatic)
                .AssertFalse(() => fieldDocumentationElement.IsShadowing);

        public static FieldDocumentationElement AssertReadOnlyField(this FieldDocumentationElement field, TypeDocumentationElement declaringType)
            => field
                .AssertEqual(() => field.Name, "ReadonlyTestField")
                .AssertEmpty(() => field.Attributes)
                .AssertEqual(() => field.AccessModifier, AccessModifier.Private)
                .AssertSame(() => field.DeclaringType, declaringType)
                .AssertType(() => field.Type, typeof(char))
                .AssertTrue(() => field.IsReadOnly)
                .AssertFalse(() => field.IsStatic)
                .AssertFalse(() => field.IsShadowing);

        public static FieldDocumentationElement AssertShadowingField(this FieldDocumentationElement field, TypeDocumentationElement declaringType)
            => field
                .AssertEqual(() => field.Name, "ShadowedTestField")
                .AssertEmpty(() => field.Attributes)
                .AssertEqual(() => field.AccessModifier, AccessModifier.Family)
                .AssertSame(() => field.DeclaringType, declaringType)
                .AssertType(() => field.Type, typeof(int))
                .AssertFalse(() => field.IsReadOnly)
                .AssertFalse(() => field.IsStatic)
                .AssertTrue(() => field.IsShadowing);

        public static FieldDocumentationElement AssertStaticField(this FieldDocumentationElement field, TypeDocumentationElement declaringType)
            => field
                .AssertEqual(() => field.Name, "StaticTestField")
                .AssertEmpty(() => field.Attributes)
                .AssertEqual(() => field.AccessModifier, AccessModifier.Private)
                .AssertSame(() => field.DeclaringType, declaringType)
                .AssertType(() => field.Type, typeof(string))
                .AssertFalse(() => field.IsReadOnly)
                .AssertTrue(() => field.IsStatic)
                .AssertFalse(() => field.IsShadowing);

        public static TTypeDocumentationElement AssertTestEvent<TTypeDocumentationElement>(this TTypeDocumentationElement typeDocumentationElement, Func<IEnumerable<EventDocumentationElement>> selector, string eventName, string attributeValuePrefix, bool checkAccessors = false)
            where TTypeDocumentationElement : TypeDocumentationElement
        {
            typeDocumentationElement
                .AssertCollectionMember(
                    () => selector().Where(@event => @event.Name == eventName),
                    eventDocumentationElement =>
                    {
                        eventDocumentationElement
                            .AssertType(() => eventDocumentationElement.Type, typeof(EventHandler<EventArgs>))
                            .AssertEqual(() => eventDocumentationElement.AccessModifier, AccessModifier.Public)
                            .AssertSame(() => eventDocumentationElement.DeclaringType, typeDocumentationElement)
                            .AssertFalse(() => eventDocumentationElement.IsStatic)
                            .AssertFalse(() => eventDocumentationElement.IsVirtual)
                            .AssertFalse(() => eventDocumentationElement.IsAbstract)
                            .AssertFalse(() => eventDocumentationElement.IsOverride)
                            .AssertFalse(() => eventDocumentationElement.IsSealed)
                            .AssertFalse(() => eventDocumentationElement.IsShadowing);

                        if (checkAccessors)
                            eventDocumentationElement
                                .AssertCollectionMember(
                                    () => eventDocumentationElement.Adder.Attributes,
                                    attribute => attribute.AssertTestAttribute($"{attributeValuePrefix} adder")
                                )
                                .AssertCollectionMember(
                                    () => eventDocumentationElement.Adder.ReturnAttributes,
                                    attribute => attribute.AssertTestAttribute($"{attributeValuePrefix} adder return")
                                )
                                .AssertCollectionMember(
                                    () => eventDocumentationElement.Adder.Attributes,
                                    attribute => attribute.AssertTestAttribute($"{attributeValuePrefix} remover")
                                )
                                .AssertCollectionMember(
                                    () => eventDocumentationElement.Adder.ReturnAttributes,
                                    attribute => attribute.AssertTestAttribute($"{attributeValuePrefix} remover return")
                                );
                    }
                );

            return typeDocumentationElement;
        }

        public static TTypeDocumentationElement AssertShadowingEvent<TTypeDocumentationElement>(this TTypeDocumentationElement typeDocumentationElement, Func<IEnumerable<EventDocumentationElement>> selector, string eventName)
            where TTypeDocumentationElement : TypeDocumentationElement
        {
            typeDocumentationElement
                .AssertCollectionMember(
                    () => selector().Where(@event => @event.Name == eventName),
                    eventDocumentationElement =>
                    {
                        eventDocumentationElement
                            .AssertType(() => eventDocumentationElement.Type, typeof(EventHandler))
                            .AssertEqual(() => eventDocumentationElement.AccessModifier, AccessModifier.Public)
                            .AssertSame(() => eventDocumentationElement.DeclaringType, typeDocumentationElement)
                            .AssertFalse(() => eventDocumentationElement.IsStatic)
                            .AssertFalse(() => eventDocumentationElement.IsVirtual)
                            .AssertFalse(() => eventDocumentationElement.IsAbstract)
                            .AssertFalse(() => eventDocumentationElement.IsOverride)
                            .AssertFalse(() => eventDocumentationElement.IsSealed)
                            .AssertTrue(() => eventDocumentationElement.IsShadowing);
                    }
                );

            return typeDocumentationElement;
        }

        public static TTypeDocumentationElement AssertAbstractEvent<TTypeDocumentationElement>(this TTypeDocumentationElement typeDocumentationElement, Func<IEnumerable<EventDocumentationElement>> selector, string eventName)
            where TTypeDocumentationElement : TypeDocumentationElement
        {
            typeDocumentationElement
                .AssertCollectionMember(
                    () => selector().Where(@event => @event.Name == eventName),
                    eventDocumentationElement =>
                    {
                        eventDocumentationElement
                            .AssertType(() => eventDocumentationElement.Type, typeof(EventHandler))
                            .AssertEqual(() => eventDocumentationElement.AccessModifier, AccessModifier.Public)
                            .AssertSame(() => eventDocumentationElement.DeclaringType, typeDocumentationElement)
                            .AssertFalse(() => eventDocumentationElement.IsStatic)
                            .AssertFalse(() => eventDocumentationElement.IsVirtual)
                            .AssertTrue(() => eventDocumentationElement.IsAbstract)
                            .AssertFalse(() => eventDocumentationElement.IsOverride)
                            .AssertFalse(() => eventDocumentationElement.IsSealed)
                            .AssertFalse(() => eventDocumentationElement.IsShadowing);
                    }
                );

            return typeDocumentationElement;
        }

        public static TTypeDocumentationElement AssertVirtualEvent<TTypeDocumentationElement>(this TTypeDocumentationElement typeDocumentationElement, Func<IEnumerable<EventDocumentationElement>> selector, string eventName)
            where TTypeDocumentationElement : TypeDocumentationElement
        {
            typeDocumentationElement
                .AssertCollectionMember(
                    () => selector().Where(@event => @event.Name == eventName),
                    eventDocumentationElement =>
                    {
                        eventDocumentationElement
                            .AssertType(() => eventDocumentationElement.Type, typeof(EventHandler))
                            .AssertEqual(() => eventDocumentationElement.AccessModifier, AccessModifier.Public)
                            .AssertSame(() => eventDocumentationElement.DeclaringType, typeDocumentationElement)
                            .AssertFalse(() => eventDocumentationElement.IsStatic)
                            .AssertTrue(() => eventDocumentationElement.IsVirtual)
                            .AssertFalse(() => eventDocumentationElement.IsAbstract)
                            .AssertFalse(() => eventDocumentationElement.IsOverride)
                            .AssertFalse(() => eventDocumentationElement.IsSealed)
                            .AssertFalse(() => eventDocumentationElement.IsShadowing);
                    }
                );

            return typeDocumentationElement;
        }

        public static TTypeDocumentationElement AssertOverrideEvent<TTypeDocumentationElement>(this TTypeDocumentationElement typeDocumentationElement, Func<IEnumerable<EventDocumentationElement>> selector, string eventName)
            where TTypeDocumentationElement : TypeDocumentationElement
        {
            typeDocumentationElement
                .AssertCollectionMember(
                    () => selector().Where(@event => @event.Name == eventName),
                    eventDocumentationElement =>
                    {
                        eventDocumentationElement
                            .AssertType(() => eventDocumentationElement.Type, typeof(EventHandler))
                            .AssertEqual(() => eventDocumentationElement.AccessModifier, AccessModifier.Public)
                            .AssertSame(() => eventDocumentationElement.DeclaringType, typeDocumentationElement)
                            .AssertFalse(() => eventDocumentationElement.IsStatic)
                            .AssertFalse(() => eventDocumentationElement.IsVirtual)
                            .AssertFalse(() => eventDocumentationElement.IsAbstract)
                            .AssertTrue(() => eventDocumentationElement.IsOverride)
                            .AssertFalse(() => eventDocumentationElement.IsSealed)
                            .AssertFalse(() => eventDocumentationElement.IsShadowing);
                    }
                );

            return typeDocumentationElement;
        }

        public static TTypeDocumentationElement AssertStaticEvent<TTypeDocumentationElement>(this TTypeDocumentationElement typeDocumentationElement, Func<IEnumerable<EventDocumentationElement>> selector, string eventName)
            where TTypeDocumentationElement : TypeDocumentationElement
        {
            typeDocumentationElement
                .AssertCollectionMember(
                    () => selector().Where(@event => @event.Name == eventName),
                    eventDocumentationElement =>
                    {
                        eventDocumentationElement
                            .AssertType(() => eventDocumentationElement.Type, typeof(EventHandler))
                            .AssertEqual(() => eventDocumentationElement.AccessModifier, AccessModifier.Public)
                            .AssertSame(() => eventDocumentationElement.DeclaringType, typeDocumentationElement)
                            .AssertTrue(() => eventDocumentationElement.IsStatic)
                            .AssertFalse(() => eventDocumentationElement.IsVirtual)
                            .AssertFalse(() => eventDocumentationElement.IsAbstract)
                            .AssertFalse(() => eventDocumentationElement.IsOverride)
                            .AssertFalse(() => eventDocumentationElement.IsSealed)
                            .AssertFalse(() => eventDocumentationElement.IsShadowing);
                    }
                );

            return typeDocumentationElement;
        }

        public static TTypeDocumentationElement AssertSealedEvent<TTypeDocumentationElement>(this TTypeDocumentationElement typeDocumentationElement, Func<IEnumerable<EventDocumentationElement>> selector, string eventName)
            where TTypeDocumentationElement : TypeDocumentationElement
        {
            typeDocumentationElement
                .AssertCollectionMember(
                    () => selector().Where(@event => @event.Name == eventName),
                    eventDocumentationElement =>
                    {
                        eventDocumentationElement
                            .AssertType(() => eventDocumentationElement.Type, typeof(EventHandler))
                            .AssertEqual(() => eventDocumentationElement.AccessModifier, AccessModifier.Public)
                            .AssertSame(() => eventDocumentationElement.DeclaringType, typeDocumentationElement)
                            .AssertFalse(() => eventDocumentationElement.IsStatic)
                            .AssertFalse(() => eventDocumentationElement.IsVirtual)
                            .AssertFalse(() => eventDocumentationElement.IsAbstract)
                            .AssertTrue(() => eventDocumentationElement.IsOverride)
                            .AssertTrue(() => eventDocumentationElement.IsSealed)
                            .AssertFalse(() => eventDocumentationElement.IsShadowing);
                    }
                );

            return typeDocumentationElement;
        }

        public static TTypeDocumentationElement AssertTestProperty<TTypeDocumentationElement>(this TTypeDocumentationElement typeDocumentationElement, Func<IEnumerable<PropertyDocumentationElement>> selector, string propertyName, string attributeValuePrefix)
            where TTypeDocumentationElement : TypeDocumentationElement
        {
            typeDocumentationElement
                .AssertCollectionMember(
                    () => selector().Where(property => property.Name == propertyName),
                    property =>
                    {
                        property
                            .AssertType(() => property.Type, typeof(byte))
                            .AssertEqual(() => property.AccessModifier, AccessModifier.Public)
                            .AssertEqual(() => property.Getter.AccessModifier, AccessModifier.Public)
                            .AssertEqual(() => property.Setter.AccessModifier, AccessModifier.Public)
                            .AssertSame(() => property.DeclaringType, typeDocumentationElement)
                            .AssertFalse(() => property.IsStatic)
                            .AssertFalse(() => property.IsVirtual)
                            .AssertFalse(() => property.IsAbstract)
                            .AssertFalse(() => property.IsOverride)
                            .AssertFalse(() => property.IsSealed)
                            .AssertFalse(() => property.IsShadowing)
                            .AssertCollectionMember(
                                () => property.Getter.Attributes,
                                attribute => attribute.AssertTestAttribute($"{attributeValuePrefix} getter")
                            )
                            .AssertCollectionMember(
                                () => property.Getter.ReturnAttributes,
                                attribute => attribute.AssertTestAttribute($"{attributeValuePrefix} getter return")
                            )
                            .AssertCollectionMember(
                                () => property.Setter.Attributes,
                                attribute => attribute.AssertTestAttribute($"{attributeValuePrefix} setter")
                            )
                            .AssertCollectionMember(
                                () => property.Setter.ReturnAttributes,
                                attribute => attribute.AssertTestAttribute($"{attributeValuePrefix} setter return")
                            );
                    }
                );

            return typeDocumentationElement;
        }

        public static TTypeDocumentationElement AssertIndexProperty<TTypeDocumentationElement>(this TTypeDocumentationElement typeDocumentationElement, Func<IEnumerable<PropertyDocumentationElement>> selector, Type typeGenericParameter, string attributeValuePrefix)
            where TTypeDocumentationElement : TypeDocumentationElement
        {
            typeDocumentationElement
                .AssertCollectionMember(
                    () => selector().Where(property => property.Name == "Item"),
                    property =>
                        property
                            .AssertType(() => property.Type, typeof(int))
                            .AssertCollectionMember(
                                () => property.Attributes,
                                attribute => attribute.AssertTestAttribute(attributeValuePrefix)
                            )
                            .AssertEqual(() => property.AccessModifier, AccessModifier.Public)
                            .AssertEqual(() => property.Getter.AccessModifier, AccessModifier.Public)
                            .AssertEqual(() => property.Setter.AccessModifier, AccessModifier.Public)
                            .AssertSame(() => property.DeclaringType, typeDocumentationElement)
                            .AssertFalse(() => property.IsStatic)
                            .AssertFalse(() => property.IsVirtual)
                            .AssertFalse(() => property.IsAbstract)
                            .AssertFalse(() => property.IsOverride)
                            .AssertFalse(() => property.IsSealed)
                            .AssertFalse(() => property.IsShadowing)
                            .AssertCollectionMember(
                                () => property.Getter.Attributes,
                                attribute => attribute.AssertTestAttribute($"{attributeValuePrefix} getter")
                            )
                            .AssertCollectionMember(
                                () => property.Getter.ReturnAttributes,
                                attribute => attribute.AssertTestAttribute($"{attributeValuePrefix} getter return")
                            )
                            .AssertCollectionMember(
                                () => property.Setter.Attributes,
                                attribute => attribute.AssertTestAttribute($"{attributeValuePrefix} setter")
                            )
                            .AssertCollectionMember(
                                () => property.Setter.ReturnAttributes,
                                attribute => attribute.AssertTestAttribute($"{attributeValuePrefix} setter return")
                            )
                            .AssertCollectionMember(
                                () => property.Parameters,
                                parameter => parameter
                                    .AssertEqual(() => parameter.Name, "param1")
                                    .AssertType(() => parameter.Type, typeof(int))
                                    .AssertCollectionMember(
                                        () => parameter.Attributes,
                                        attribute => attribute.AssertTestAttribute($"{attributeValuePrefix} parameter")
                                    )
                                    .AssertFalse(() => parameter.IsInputByReference)
                                    .AssertFalse(() => parameter.IsInputOutputByReference)
                                    .AssertFalse(() => parameter.IsOutputByReference)
                                    .AssertFalse(() => parameter.HasDefaultValue)
                                    .AssertNull(() => parameter.DefaultValue),
                                parameter => parameter
                                    .AssertEqual(() => parameter.Name, "param2")
                                    .AssertType(() => parameter.Type, typeof(byte[]))
                                    .AssertEmpty(() => parameter.Attributes)
                                    .AssertFalse(() => parameter.IsInputByReference)
                                    .AssertFalse(() => parameter.IsInputOutputByReference)
                                    .AssertFalse(() => parameter.IsOutputByReference)
                                    .AssertFalse(() => parameter.HasDefaultValue)
                                    .AssertNull(() => parameter.DefaultValue),
                                parameter => parameter
                                    .AssertEqual(() => parameter.Name, "param3")
                                    .AssertType(() => parameter.Type, typeof(char[][]))
                                    .AssertEmpty(() => parameter.Attributes)
                                    .AssertFalse(() => parameter.IsInputByReference)
                                    .AssertFalse(() => parameter.IsInputOutputByReference)
                                    .AssertFalse(() => parameter.IsOutputByReference)
                                    .AssertFalse(() => parameter.HasDefaultValue)
                                    .AssertNull(() => parameter.DefaultValue),
                                parameter => parameter
                                    .AssertEqual(() => parameter.Name, "param4")
                                    .AssertType(() => parameter.Type, typeof(double[,]))
                                    .AssertEmpty(() => parameter.Attributes)
                                    .AssertFalse(() => parameter.IsInputByReference)
                                    .AssertFalse(() => parameter.IsInputOutputByReference)
                                    .AssertFalse(() => parameter.IsOutputByReference)
                                    .AssertFalse(() => parameter.HasDefaultValue)
                                    .AssertNull(() => parameter.DefaultValue),
                                parameter => parameter
                                    .AssertEqual(() => parameter.Name, "param5")
                                    .AssertType(() => parameter.Type, typeof(TestClass<int>.NestedTestClass<byte[], IEnumerable<string>>))
                                    .AssertEmpty(() => parameter.Attributes)
                                    .AssertFalse(() => parameter.IsInputByReference)
                                    .AssertFalse(() => parameter.IsInputOutputByReference)
                                    .AssertFalse(() => parameter.IsOutputByReference)
                                    .AssertFalse(() => parameter.HasDefaultValue)
                                    .AssertNull(() => parameter.DefaultValue),
                                parameter => parameter
                                    .AssertEqual(() => parameter.Name, "param6")
                                    .AssertType(() => parameter.Type, typeof(TestClass<int>.NestedTestClass<byte[], IEnumerable<string>>[]))
                                    .AssertEmpty(() => parameter.Attributes)
                                    .AssertFalse(() => parameter.IsInputByReference)
                                    .AssertFalse(() => parameter.IsInputOutputByReference)
                                    .AssertFalse(() => parameter.IsOutputByReference)
                                    .AssertFalse(() => parameter.HasDefaultValue)
                                    .AssertNull(() => parameter.DefaultValue),
                                parameter => parameter
                                    .AssertEqual(() => parameter.Name, "param7")
                                    .AssertDynamicType(() => parameter.Type)
                                    .AssertCollectionMember(
                                        () => parameter.Attributes,
                                        attribute => attribute.AssertDynamicTypeAttribute()
                                    )
                                    .AssertFalse(() => parameter.IsInputByReference)
                                    .AssertFalse(() => parameter.IsInputOutputByReference)
                                    .AssertFalse(() => parameter.IsOutputByReference)
                                    .AssertFalse(() => parameter.HasDefaultValue)
                                    .AssertNull(() => parameter.DefaultValue),
                                parameter => parameter
                                    .AssertEqual(() => parameter.Name, "param8")
                                    .AssertType(() => parameter.Type, typeof(int*))
                                    .AssertEmpty(() => parameter.Attributes)
                                    .AssertFalse(() => parameter.IsInputByReference)
                                    .AssertFalse(() => parameter.IsInputOutputByReference)
                                    .AssertFalse(() => parameter.IsOutputByReference)
                                    .AssertFalse(() => parameter.HasDefaultValue)
                                    .AssertNull(() => parameter.DefaultValue),
                                parameter => parameter
                                    .AssertEqual(() => parameter.Name, "param9")
                                    .AssertType(() => parameter.Type, typeof(byte*[]))
                                    .AssertEmpty(() => parameter.Attributes)
                                    .AssertFalse(() => parameter.IsInputByReference)
                                    .AssertFalse(() => parameter.IsInputOutputByReference)
                                    .AssertFalse(() => parameter.IsOutputByReference)
                                    .AssertFalse(() => parameter.HasDefaultValue)
                                    .AssertNull(() => parameter.DefaultValue),
                                parameter => parameter
                                    .AssertEqual(() => parameter.Name, "param10")
                                    .AssertType(() => parameter.Type, typeof(void*))
                                    .AssertEmpty(() => parameter.Attributes)
                                    .AssertFalse(() => parameter.IsInputByReference)
                                    .AssertFalse(() => parameter.IsInputOutputByReference)
                                    .AssertFalse(() => parameter.IsOutputByReference)
                                    .AssertFalse(() => parameter.HasDefaultValue)
                                    .AssertNull(() => parameter.DefaultValue),
                                parameter => parameter
                                    .AssertEqual(() => parameter.Name, "param11")
                                    .AssertType(() => parameter.Type, typeof(void**))
                                    .AssertEmpty(() => parameter.Attributes)
                                    .AssertFalse(() => parameter.IsInputByReference)
                                    .AssertFalse(() => parameter.IsInputOutputByReference)
                                    .AssertFalse(() => parameter.IsOutputByReference)
                                    .AssertFalse(() => parameter.HasDefaultValue)
                                    .AssertNull(() => parameter.DefaultValue),
                                parameter => parameter
                                    .AssertEqual(() => parameter.Name, "param12")
                                    .AssertType(() => parameter.Type, typeof(void**[]))
                                    .AssertEmpty(() => parameter.Attributes)
                                    .AssertFalse(() => parameter.IsInputByReference)
                                    .AssertFalse(() => parameter.IsInputOutputByReference)
                                    .AssertFalse(() => parameter.IsOutputByReference)
                                    .AssertFalse(() => parameter.HasDefaultValue)
                                    .AssertNull(() => parameter.DefaultValue),
                                parameter => parameter
                                    .AssertEqual(() => parameter.Name, "param13")
                                    .AssertType(() => parameter.Type, typeGenericParameter)
                                    .AssertEmpty(() => parameter.Attributes)
                                    .AssertFalse(() => parameter.IsInputByReference)
                                    .AssertFalse(() => parameter.IsInputOutputByReference)
                                    .AssertFalse(() => parameter.IsOutputByReference)
                                    .AssertFalse(() => parameter.HasDefaultValue)
                                    .AssertNull(() => parameter.DefaultValue),
                                parameter => parameter
                                    .AssertEqual(() => parameter.Name, "param14")
                                    .AssertType(() => parameter.Type, typeof(string))
                                    .AssertCollectionMember(
                                        () => parameter.Attributes,
                                        attribute => attribute.AssertOptionalParameterAttribute()
                                    )
                                    .AssertFalse(() => parameter.IsInputByReference)
                                    .AssertFalse(() => parameter.IsInputOutputByReference)
                                    .AssertFalse(() => parameter.IsOutputByReference)
                                    .AssertTrue(() => parameter.HasDefaultValue)
                                    .AssertEqual(() => parameter.DefaultValue, "test")
                            )
                );

            return typeDocumentationElement;
        }

        public static TTypeDocumentationElement AssertAbstractProperty<TTypeDocumentationElement>(this TTypeDocumentationElement typeDocumentationElement, Func<IEnumerable<PropertyDocumentationElement>> selector, string propertyName)
            where TTypeDocumentationElement : TypeDocumentationElement
        {
            typeDocumentationElement
                .AssertCollectionMember(
                    () => selector().Where(property => property.Name == propertyName),
                    property =>
                    {
                        property
                            .AssertType(() => property.Type, typeof(byte))
                            .AssertEqual(() => property.AccessModifier, AccessModifier.Public)
                            .AssertSame(() => property.DeclaringType, typeDocumentationElement)
                            .AssertFalse(() => property.IsStatic)
                            .AssertFalse(() => property.IsVirtual)
                            .AssertTrue(() => property.IsAbstract)
                            .AssertFalse(() => property.IsOverride)
                            .AssertFalse(() => property.IsSealed)
                            .AssertFalse(() => property.IsShadowing);
                    }
                );

            return typeDocumentationElement;
        }

        public static TTypeDocumentationElement AssertVirtualProperty<TTypeDocumentationElement>(this TTypeDocumentationElement typeDocumentationElement, Func<IEnumerable<PropertyDocumentationElement>> selector, string propertyName)
            where TTypeDocumentationElement : TypeDocumentationElement
        {
            typeDocumentationElement
                .AssertCollectionMember(
                    () => selector().Where(property => property.Name == propertyName),
                    property =>
                    {
                        property
                            .AssertType(() => property.Type, typeof(string))
                            .AssertEqual(() => property.AccessModifier, AccessModifier.Public)
                            .AssertSame(() => property.DeclaringType, typeDocumentationElement)
                            .AssertFalse(() => property.IsStatic)
                            .AssertTrue(() => property.IsVirtual)
                            .AssertFalse(() => property.IsAbstract)
                            .AssertFalse(() => property.IsOverride)
                            .AssertFalse(() => property.IsSealed)
                            .AssertFalse(() => property.IsShadowing);
                    }
                );

            return typeDocumentationElement;
        }

        public static TTypeDocumentationElement AssertOverrideProperty<TTypeDocumentationElement>(this TTypeDocumentationElement typeDocumentationElement, Func<IEnumerable<PropertyDocumentationElement>> selector, string propertyName)
            where TTypeDocumentationElement : TypeDocumentationElement
        {
            typeDocumentationElement
                .AssertCollectionMember(
                    () => selector().Where(property => property.Name == propertyName),
                    property =>
                    {
                        property
                            .AssertType(() => property.Type, typeof(byte))
                            .AssertEqual(() => property.AccessModifier, AccessModifier.Public)
                            .AssertSame(() => property.DeclaringType, typeDocumentationElement)
                            .AssertFalse(() => property.IsStatic)
                            .AssertFalse(() => property.IsVirtual)
                            .AssertFalse(() => property.IsAbstract)
                            .AssertTrue(() => property.IsOverride)
                            .AssertFalse(() => property.IsSealed)
                            .AssertFalse(() => property.IsShadowing);
                    }
                );

            return typeDocumentationElement;
        }

        public static TTypeDocumentationElement AssertSealedProperty<TTypeDocumentationElement>(this TTypeDocumentationElement typeDocumentationElement, Func<IEnumerable<PropertyDocumentationElement>> selector, string propertyName)
            where TTypeDocumentationElement : TypeDocumentationElement
        {
            typeDocumentationElement
                .AssertCollectionMember(
                    () => selector().Where(property => property.Name == propertyName),
                    property =>
                    {
                        property
                            .AssertType(() => property.Type, typeof(string))
                            .AssertEqual(() => property.AccessModifier, AccessModifier.Public)
                            .AssertSame(() => property.DeclaringType, typeDocumentationElement)
                            .AssertFalse(() => property.IsStatic)
                            .AssertFalse(() => property.IsVirtual)
                            .AssertFalse(() => property.IsAbstract)
                            .AssertTrue(() => property.IsOverride)
                            .AssertTrue(() => property.IsSealed)
                            .AssertFalse(() => property.IsShadowing);
                    }
                );

            return typeDocumentationElement;
        }

        public static TTypeDocumentationElement AssertShadowingProperty<TTypeDocumentationElement>(this TTypeDocumentationElement typeDocumentationElement, Func<IEnumerable<PropertyDocumentationElement>> selector, string propertyName)
            where TTypeDocumentationElement : TypeDocumentationElement
        {
            typeDocumentationElement
                .AssertCollectionMember(
                    () => selector().Where(property => property.Name == propertyName),
                    property =>
                    {
                        property
                            .AssertType(() => property.Type, typeof(int))
                            .AssertEqual(() => property.AccessModifier, AccessModifier.Public)
                            .AssertSame(() => property.DeclaringType, typeDocumentationElement)
                            .AssertFalse(() => property.IsStatic)
                            .AssertFalse(() => property.IsVirtual)
                            .AssertFalse(() => property.IsAbstract)
                            .AssertFalse(() => property.IsOverride)
                            .AssertFalse(() => property.IsSealed)
                            .AssertTrue(() => property.IsShadowing);
                    }
                );

            return typeDocumentationElement;
        }

        public static TTypeDocumentationElement AssertStaticProperty<TTypeDocumentationElement>(this TTypeDocumentationElement typeDocumentationElement, Func<IEnumerable<PropertyDocumentationElement>> selector, string propertyName)
            where TTypeDocumentationElement : TypeDocumentationElement
        {
            typeDocumentationElement
                .AssertCollectionMember(
                    () => selector().Where(property => property.Name == propertyName),
                    property =>
                    {
                        property
                            .AssertType(() => property.Type, typeof(int))
                            .AssertEqual(() => property.AccessModifier, AccessModifier.Public)
                            .AssertSame(() => property.DeclaringType, typeDocumentationElement)
                            .AssertTrue(() => property.IsStatic)
                            .AssertFalse(() => property.IsVirtual)
                            .AssertFalse(() => property.IsAbstract)
                            .AssertFalse(() => property.IsOverride)
                            .AssertFalse(() => property.IsSealed)
                            .AssertFalse(() => property.IsShadowing);
                    }
                );

            return typeDocumentationElement;
        }

        public static TInstance AssertType<TInstance>(this TInstance instance, Func<TypeReferenceDocumentationElement> selector, Type type)
        {
            selector().AssertType(type);
            return instance;
        }

        public static TInstance AssertDynamicType<TInstance>(this TInstance instance, Func<TypeReferenceDocumentationElement> selector)
            => instance
                .AssertType(selector, typeof(object))
                .AssertMember(selector, type => type.AssertIs<DynamicTypeReferenceDocumentationElement>());

        public static AttributeData AssertDynamicTypeAttribute(this AttributeData attribute, bool[] transformFlags = null)
            => attribute
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
                .AssertEmpty(() => attribute.NamedParameters);

        public static AttributeData AssertOutputParameterAttribute(this AttributeData attributeData)
            => attributeData
                .AssertType(() => attributeData.Type, typeof(OutAttribute))
                .AssertEmpty(() => attributeData.PositionalParameters)
                .AssertEmpty(() => attributeData.NamedParameters);

        public static AttributeData AssertOptionalParameterAttribute(this AttributeData attributeData)
            => attributeData
                .AssertType(() => attributeData.Type, typeof(OptionalAttribute))
                .AssertEmpty(() => attributeData.PositionalParameters)
                .AssertEmpty(() => attributeData.NamedParameters);

        public static TTypeDocumentationElement AssertTypeGenericParameters<TTypeDocumentationElement>(this TTypeDocumentationElement typeDocumentationElement, Func<IEnumerable<TypeGenericParameterDocumentationElement>> selector)
            where TTypeDocumentationElement : TypeDocumentationElement
            => typeDocumentationElement
                .AssertCollectionMember(
                    selector,
                    genericParameter => genericParameter
                        .AssertEqual(() => genericParameter.Name, "TParam1")
                        .AssertSame(() => genericParameter.DeclaringType, typeDocumentationElement)
                        .AssertEqual(() => genericParameter.Position, 0)
                        .AssertFalse(() => genericParameter.IsCovariant)
                        .AssertFalse(() => genericParameter.IsContravariant)
                        .AssertFalse(() => genericParameter.HasReferenceTypeConstraint)
                        .AssertFalse(() => genericParameter.HasNonNullableValueTypeConstraint)
                        .AssertFalse(() => genericParameter.HasDefaultConstructorConstraint)
                        .AssertEmpty(() => genericParameter.TypeConstraints)
                );

        public static DelegateDocumentationElement AssertDelegateParameters(this DelegateDocumentationElement delegateDocumentationElement, Type genericParameter)
            => delegateDocumentationElement
                .AssertCollectionMember(
                    () => delegateDocumentationElement.Parameters,
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param1")
                        .AssertCollectionMember(
                            () => delegateDocumentationElement.Attributes,
                            attribute => attribute.AssertTestAttribute($"delegate")
                        )
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
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertOutputParameterAttribute()
                        )
                        .AssertType(() => parameter.Type, typeof(int))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertTrue(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param10")
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertOutputParameterAttribute()
                        )
                        .AssertType(() => parameter.Type, typeof(byte[]))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertTrue(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param11")
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertOutputParameterAttribute()
                        )
                        .AssertType(() => parameter.Type, typeof(char[][]))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertTrue(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param12")
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertOutputParameterAttribute()
                        )
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
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertOutputParameterAttribute()
                        )
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
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertOutputParameterAttribute()
                        )
                        .AssertType(() => parameter.Type, typeof(TestClass<int>.NestedTestClass<byte, IEnumerable<string>>[]))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertTrue(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param19")
                        .AssertDynamicType(() => parameter.Type)
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertDynamicTypeAttribute()
                        )
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param20")
                        .AssertDynamicType(() => parameter.Type)
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertDynamicTypeAttribute(new[] { false, true })
                        )
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertTrue(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param21")
                        .AssertDynamicType(() => parameter.Type)
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertOutputParameterAttribute(),
                            attribute => attribute.AssertDynamicTypeAttribute(new[] { false, true })
                        )
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
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertOutputParameterAttribute()
                        )
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
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertOutputParameterAttribute()
                        )
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
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertOutputParameterAttribute()
                        )
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
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertOutputParameterAttribute()
                        )
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
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertOutputParameterAttribute()
                        )
                        .AssertType(() => parameter.Type, typeof(void**[]))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertTrue(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param38")
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertOptionalParameterAttribute()
                        )
                        .AssertType(() => parameter.Type, typeof(string))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertTrue(() => parameter.HasDefaultValue)
                        .AssertEqual(() => parameter.DefaultValue, "test")
                );

        public static ConstructorDocumentationElement AssertDefaultConstructor(this ConstructorDocumentationElement constructorDocumentationElement, TypeDocumentationElement declaringType)
            => constructorDocumentationElement
                .AssertEqual(() => constructorDocumentationElement.Name, declaringType.Name)
                .AssertEmpty(() => constructorDocumentationElement.Attributes)
                .AssertSame(() => constructorDocumentationElement.DeclaringType, declaringType)
                .AssertEmpty(() => constructorDocumentationElement.Parameters);

        public static ConstructorDocumentationElement AssertTestConstructor(this ConstructorDocumentationElement constructorDocumentationElement, TypeDocumentationElement declaringType, Type typeGenericParameter, string attributeValuePrefix)
            => constructorDocumentationElement
                .AssertEqual(() => constructorDocumentationElement.Name, declaringType.Name)
                .AssertCollectionMember(
                    () => constructorDocumentationElement.Attributes,
                    attribute => attribute.AssertTestAttribute(attributeValuePrefix)
                )
                .AssertSame(() => constructorDocumentationElement.DeclaringType, declaringType)
                .AssertCollectionMember(
                    () => constructorDocumentationElement.Parameters,
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param1")
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertTestAttribute($"{attributeValuePrefix} parameter")
                        )
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
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertOutputParameterAttribute()
                        )
                        .AssertType(() => parameter.Type, typeof(int))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertTrue(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param10")
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertOutputParameterAttribute()
                        )
                        .AssertType(() => parameter.Type, typeof(byte[]))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertTrue(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param11")
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertOutputParameterAttribute()
                        )
                        .AssertType(() => parameter.Type, typeof(char[][]))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertTrue(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param12")
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertOutputParameterAttribute()
                        )
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
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertOutputParameterAttribute()
                        )
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
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertOutputParameterAttribute()
                        )
                        .AssertType(() => parameter.Type, typeof(TestClass<int>.NestedTestClass<byte, IEnumerable<string>>[]))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertTrue(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param19")
                        .AssertDynamicType(() => parameter.Type)
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertDynamicTypeAttribute()
                        )
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param20")
                        .AssertDynamicType(() => parameter.Type)
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertDynamicTypeAttribute(new[] { false, true })
                        )
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertTrue(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param21")
                        .AssertDynamicType(() => parameter.Type)
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertOutputParameterAttribute(),
                            attribute => attribute.AssertDynamicTypeAttribute(new[] { false, true })
                        )
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertTrue(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param22")
                        .AssertEmpty(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeGenericParameter)
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param23")
                        .AssertEmpty(() => parameter.Attributes)
                        .AssertType(() => parameter.Type, typeGenericParameter)
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertTrue(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param24")
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertOutputParameterAttribute()
                        )
                        .AssertType(() => parameter.Type, typeGenericParameter)
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
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertOutputParameterAttribute()
                        )
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
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertOutputParameterAttribute()
                        )
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
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertOutputParameterAttribute()
                        )
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
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertOutputParameterAttribute()
                        )
                        .AssertType(() => parameter.Type, typeof(void**[]))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertTrue(() => parameter.IsOutputByReference)
                        .AssertFalse(() => parameter.HasDefaultValue)
                        .AssertNull(() => parameter.DefaultValue),
                    parameter => parameter
                        .AssertEqual(() => parameter.Name, "param38")
                        .AssertCollectionMember(
                            () => parameter.Attributes,
                            attribute => attribute.AssertOptionalParameterAttribute()
                        )
                        .AssertType(() => parameter.Type, typeof(string))
                        .AssertFalse(() => parameter.IsInputByReference)
                        .AssertFalse(() => parameter.IsInputOutputByReference)
                        .AssertFalse(() => parameter.IsOutputByReference)
                        .AssertTrue(() => parameter.HasDefaultValue)
                        .AssertEqual(() => parameter.DefaultValue, "test")
                );

        public static TTypeDocumentationElement AssertTestMethod<TTypeDocumentationElement>(this TTypeDocumentationElement typeDocumentationElement, Func<IEnumerable<MethodDocumentationElement>> selector, string methodName, Type typeGenericParameter, Type methodGenericParameter, string attributeValuePrefix)
            where TTypeDocumentationElement : TypeDocumentationElement
            => typeDocumentationElement
                .AssertCollectionMember(
                    () => selector().Where(method => method.Name == methodName),
                    method => method
                        .AssertCollectionMember(
                            () => method.Attributes,
                            attribute => attribute.AssertTestAttribute(attributeValuePrefix)
                        )
                        .AssertCollectionMember(
                            () => method.Return.Attributes,
                            attribute => attribute.AssertTestAttribute($"{attributeValuePrefix} return")
                        )
                        .AssertMember(
                            () => method.Return.Type,
                            returnType => returnType
                                .AssertType(typeof(void))
                                .AssertIs<VoidTypeReferenceDocumentationElement>()
                        )
                        .AssertCollectionMember(
                            () => method.GenericParameters,
                            genericParameter =>
                                genericParameter
                                    .AssertEqual(() => genericParameter.Name, "TMethodParam1")
                                    .AssertEqual(() => genericParameter.Position, 0)
                                    .AssertFalse(() => genericParameter.IsCovariant)
                                    .AssertFalse(() => genericParameter.IsContravariant)
                                    .AssertFalse(() => genericParameter.HasReferenceTypeConstraint)
                                    .AssertFalse(() => genericParameter.HasNonNullableValueTypeConstraint)
                                    .AssertFalse(() => genericParameter.HasDefaultConstructorConstraint)
                                    .AssertEmpty(() => genericParameter.TypeConstraints)
                        )
                        .AssertSame(() => method.DeclaringType, typeDocumentationElement)
                        .AssertCollectionMember(
                            () => method.Parameters,
                            parameter => parameter
                                .AssertEqual(() => parameter.Name, "param1")
                                .AssertCollectionMember(
                                    () => parameter.Attributes,
                                    attribute => attribute.AssertTestAttribute($"{attributeValuePrefix} parameter")
                                )
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
                                .AssertCollectionMember(
                                    () => parameter.Attributes,
                                    attribute => attribute.AssertOutputParameterAttribute()
                                )
                                .AssertType(() => parameter.Type, typeof(int))
                                .AssertFalse(() => parameter.IsInputByReference)
                                .AssertFalse(() => parameter.IsInputOutputByReference)
                                .AssertTrue(() => parameter.IsOutputByReference)
                                .AssertFalse(() => parameter.HasDefaultValue)
                                .AssertNull(() => parameter.DefaultValue),
                            parameter => parameter
                                .AssertEqual(() => parameter.Name, "param10")
                                .AssertCollectionMember(
                                    () => parameter.Attributes,
                                    attribute => attribute.AssertOutputParameterAttribute()
                                )
                                .AssertType(() => parameter.Type, typeof(byte[]))
                                .AssertFalse(() => parameter.IsInputByReference)
                                .AssertFalse(() => parameter.IsInputOutputByReference)
                                .AssertTrue(() => parameter.IsOutputByReference)
                                .AssertFalse(() => parameter.HasDefaultValue)
                                .AssertNull(() => parameter.DefaultValue),
                            parameter => parameter
                                .AssertEqual(() => parameter.Name, "param11")
                                .AssertCollectionMember(
                                    () => parameter.Attributes,
                                    attribute => attribute.AssertOutputParameterAttribute()
                                )
                                .AssertType(() => parameter.Type, typeof(char[][]))
                                .AssertFalse(() => parameter.IsInputByReference)
                                .AssertFalse(() => parameter.IsInputOutputByReference)
                                .AssertTrue(() => parameter.IsOutputByReference)
                                .AssertFalse(() => parameter.HasDefaultValue)
                                .AssertNull(() => parameter.DefaultValue),
                            parameter => parameter
                                .AssertEqual(() => parameter.Name, "param12")
                                .AssertCollectionMember(
                                    () => parameter.Attributes,
                                    attribute => attribute.AssertOutputParameterAttribute()
                                )
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
                                .AssertCollectionMember(
                                    () => parameter.Attributes,
                                    attribute => attribute.AssertOutputParameterAttribute()
                                )
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
                                .AssertCollectionMember(
                                    () => parameter.Attributes,
                                    attribute => attribute.AssertOutputParameterAttribute()
                                )
                                .AssertType(() => parameter.Type, typeof(TestClass<int>.NestedTestClass<byte, IEnumerable<string>>[]))
                                .AssertFalse(() => parameter.IsInputByReference)
                                .AssertFalse(() => parameter.IsInputOutputByReference)
                                .AssertTrue(() => parameter.IsOutputByReference)
                                .AssertFalse(() => parameter.HasDefaultValue)
                                .AssertNull(() => parameter.DefaultValue),
                            parameter => parameter
                                .AssertEqual(() => parameter.Name, "param19")
                                .AssertDynamicType(() => parameter.Type)
                                .AssertCollectionMember(
                                    () => parameter.Attributes,
                                    attribute => attribute.AssertDynamicTypeAttribute()
                                )
                                .AssertFalse(() => parameter.IsInputByReference)
                                .AssertFalse(() => parameter.IsInputOutputByReference)
                                .AssertFalse(() => parameter.IsOutputByReference)
                                .AssertFalse(() => parameter.HasDefaultValue)
                                .AssertNull(() => parameter.DefaultValue),
                            parameter => parameter
                                .AssertEqual(() => parameter.Name, "param20")
                                .AssertDynamicType(() => parameter.Type)
                                .AssertCollectionMember(
                                    () => parameter.Attributes,
                                    attribute => attribute.AssertDynamicTypeAttribute(new[] { false, true })
                                )
                                .AssertFalse(() => parameter.IsInputByReference)
                                .AssertTrue(() => parameter.IsInputOutputByReference)
                                .AssertFalse(() => parameter.IsOutputByReference)
                                .AssertFalse(() => parameter.HasDefaultValue)
                                .AssertNull(() => parameter.DefaultValue),
                            parameter => parameter
                                .AssertEqual(() => parameter.Name, "param21")
                                .AssertDynamicType(() => parameter.Type)
                                .AssertCollectionMember(
                                    () => parameter.Attributes,
                                    attribute => attribute.AssertOutputParameterAttribute(),
                                    attribute => attribute.AssertDynamicTypeAttribute(new[] { false, true })
                                )
                                .AssertFalse(() => parameter.IsInputByReference)
                                .AssertFalse(() => parameter.IsInputOutputByReference)
                                .AssertTrue(() => parameter.IsOutputByReference)
                                .AssertFalse(() => parameter.HasDefaultValue)
                                .AssertNull(() => parameter.DefaultValue),
                            parameter => parameter
                                .AssertEqual(() => parameter.Name, "param22")
                                .AssertEmpty(() => parameter.Attributes)
                                .AssertType(() => parameter.Type, typeGenericParameter)
                                .AssertFalse(() => parameter.IsInputByReference)
                                .AssertFalse(() => parameter.IsInputOutputByReference)
                                .AssertFalse(() => parameter.IsOutputByReference)
                                .AssertFalse(() => parameter.HasDefaultValue)
                                .AssertNull(() => parameter.DefaultValue),
                            parameter => parameter
                                .AssertEqual(() => parameter.Name, "param23")
                                .AssertEmpty(() => parameter.Attributes)
                                .AssertType(() => parameter.Type, typeGenericParameter)
                                .AssertFalse(() => parameter.IsInputByReference)
                                .AssertTrue(() => parameter.IsInputOutputByReference)
                                .AssertFalse(() => parameter.IsOutputByReference)
                                .AssertFalse(() => parameter.HasDefaultValue)
                                .AssertNull(() => parameter.DefaultValue),
                            parameter => parameter
                                .AssertEqual(() => parameter.Name, "param24")
                                .AssertCollectionMember(
                                    () => parameter.Attributes,
                                    attribute => attribute.AssertOutputParameterAttribute()
                                )
                                .AssertType(() => parameter.Type, typeGenericParameter)
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
                                .AssertCollectionMember(
                                    () => parameter.Attributes,
                                    attribute => attribute.AssertOutputParameterAttribute()
                                )
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
                                .AssertCollectionMember(
                                    () => parameter.Attributes,
                                    attribute => attribute.AssertOutputParameterAttribute()
                                )
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
                                .AssertCollectionMember(
                                    () => parameter.Attributes,
                                    attribute => attribute.AssertOutputParameterAttribute()
                                )
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
                                .AssertCollectionMember(
                                    () => parameter.Attributes,
                                    attribute => attribute.AssertOutputParameterAttribute()
                                )
                                .AssertType(() => parameter.Type, typeof(void**[]))
                                .AssertFalse(() => parameter.IsInputByReference)
                                .AssertFalse(() => parameter.IsInputOutputByReference)
                                .AssertTrue(() => parameter.IsOutputByReference)
                                .AssertFalse(() => parameter.HasDefaultValue)
                                .AssertNull(() => parameter.DefaultValue),
                            parameter => parameter
                                .AssertEqual(() => parameter.Name, "param38")
                                .AssertEmpty(() => parameter.Attributes)
                                .AssertType(() => parameter.Type, methodGenericParameter)
                                .AssertFalse(() => parameter.IsInputByReference)
                                .AssertFalse(() => parameter.IsInputOutputByReference)
                                .AssertFalse(() => parameter.IsOutputByReference)
                                .AssertFalse(() => parameter.HasDefaultValue)
                                .AssertNull(() => parameter.DefaultValue),
                            parameter => parameter
                                .AssertEqual(() => parameter.Name, "param39")
                                .AssertEmpty(() => parameter.Attributes)
                                .AssertType(() => parameter.Type, methodGenericParameter)
                                .AssertFalse(() => parameter.IsInputByReference)
                                .AssertTrue(() => parameter.IsInputOutputByReference)
                                .AssertFalse(() => parameter.IsOutputByReference)
                                .AssertFalse(() => parameter.HasDefaultValue)
                                .AssertNull(() => parameter.DefaultValue),
                            parameter => parameter
                                .AssertEqual(() => parameter.Name, "param40")
                                .AssertCollectionMember(
                                    () => parameter.Attributes,
                                    attribute => attribute.AssertOutputParameterAttribute()
                                )
                                .AssertType(() => parameter.Type, methodGenericParameter)
                                .AssertFalse(() => parameter.IsInputByReference)
                                .AssertFalse(() => parameter.IsInputOutputByReference)
                                .AssertTrue(() => parameter.IsOutputByReference)
                                .AssertFalse(() => parameter.HasDefaultValue)
                                .AssertNull(() => parameter.DefaultValue),
                            parameter => parameter
                                .AssertEqual(() => parameter.Name, "param41")
                                .AssertCollectionMember(
                                    () => parameter.Attributes,
                                    attribute => attribute.AssertOptionalParameterAttribute()
                                )
                                .AssertType(() => parameter.Type, typeof(string))
                                .AssertFalse(() => parameter.IsInputByReference)
                                .AssertFalse(() => parameter.IsInputOutputByReference)
                                .AssertFalse(() => parameter.IsOutputByReference)
                                .AssertTrue(() => parameter.HasDefaultValue)
                                .AssertEqual(() => parameter.DefaultValue, "test")
                        )
                );

        public static TTypeDocumentationElement AssertAbstractMethod<TTypeDocumentationElement>(this TTypeDocumentationElement typeDocumentationElement, Func<IEnumerable<MethodDocumentationElement>> selector, string methodName)
            where TTypeDocumentationElement : TypeDocumentationElement
        {
            typeDocumentationElement
                .AssertCollectionMember(
                    () => selector().Where(method => method.Name == methodName),
                    method =>
                    {
                        method
                            .AssertEmpty(() => method.GenericParameters)
                            .AssertEmpty(() => method.Parameters)
                            .AssertType(() => method.Return.Type, typeof(string))
                            .AssertEqual(() => method.AccessModifier, AccessModifier.Public)
                            .AssertSame(() => method.DeclaringType, typeDocumentationElement)
                            .AssertFalse(() => method.IsStatic)
                            .AssertFalse(() => method.IsVirtual)
                            .AssertTrue(() => method.IsAbstract)
                            .AssertFalse(() => method.IsOverride)
                            .AssertFalse(() => method.IsSealed)
                            .AssertFalse(() => method.IsShadowing);
                    }
                );

            return typeDocumentationElement;
        }

        public static TTypeDocumentationElement AssertVirtualMethod<TTypeDocumentationElement>(this TTypeDocumentationElement typeDocumentationElement, Func<IEnumerable<MethodDocumentationElement>> selector, string methodName)
            where TTypeDocumentationElement : TypeDocumentationElement
        {
            typeDocumentationElement
                .AssertCollectionMember(
                    () => selector().Where(method => method.Name == methodName),
                    method =>
                    {
                        method
                            .AssertEmpty(() => method.GenericParameters)
                            .AssertEmpty(() => method.Parameters)
                            .AssertType(() => method.Return.Type, typeof(bool))
                            .AssertEqual(() => method.AccessModifier, AccessModifier.Public)
                            .AssertSame(() => method.DeclaringType, typeDocumentationElement)
                            .AssertFalse(() => method.IsStatic)
                            .AssertTrue(() => method.IsVirtual)
                            .AssertFalse(() => method.IsAbstract)
                            .AssertFalse(() => method.IsOverride)
                            .AssertFalse(() => method.IsSealed)
                            .AssertFalse(() => method.IsShadowing);
                    }
                );

            return typeDocumentationElement;
        }

        public static TTypeDocumentationElement AssertOverrideMethod<TTypeDocumentationElement>(this TTypeDocumentationElement typeDocumentationElement, Func<IEnumerable<MethodDocumentationElement>> selector, string methodName)
            where TTypeDocumentationElement : TypeDocumentationElement
        {
            typeDocumentationElement
                .AssertCollectionMember(
                    () => selector().Where(method => method.Name == methodName),
                    method =>
                    {
                        method
                            .AssertEmpty(() => method.GenericParameters)
                            .AssertEmpty(() => method.Parameters)
                            .AssertType(() => method.Return.Type, typeof(string))
                            .AssertEqual(() => method.AccessModifier, AccessModifier.Public)
                            .AssertSame(() => method.DeclaringType, typeDocumentationElement)
                            .AssertFalse(() => method.IsStatic)
                            .AssertFalse(() => method.IsVirtual)
                            .AssertFalse(() => method.IsAbstract)
                            .AssertTrue(() => method.IsOverride)
                            .AssertFalse(() => method.IsSealed)
                            .AssertFalse(() => method.IsShadowing);
                    }
                );

            return typeDocumentationElement;
        }

        public static TTypeDocumentationElement AssertSealedMethod<TTypeDocumentationElement>(this TTypeDocumentationElement typeDocumentationElement, Func<IEnumerable<MethodDocumentationElement>> selector, string methodName)
            where TTypeDocumentationElement : TypeDocumentationElement
        {
            typeDocumentationElement
                .AssertCollectionMember(
                    () => selector().Where(method => method.Name == methodName),
                    method =>
                    {
                        method
                            .AssertEmpty(() => method.GenericParameters)
                            .AssertEmpty(() => method.Parameters)
                            .AssertType(() => method.Return.Type, typeof(bool))
                            .AssertEqual(() => method.AccessModifier, AccessModifier.Public)
                            .AssertSame(() => method.DeclaringType, typeDocumentationElement)
                            .AssertFalse(() => method.IsStatic)
                            .AssertFalse(() => method.IsVirtual)
                            .AssertFalse(() => method.IsAbstract)
                            .AssertTrue(() => method.IsOverride)
                            .AssertTrue(() => method.IsSealed)
                            .AssertFalse(() => method.IsShadowing);
                    }
                );

            return typeDocumentationElement;
        }

        public static TTypeDocumentationElement AssertShadowingMethod<TTypeDocumentationElement>(this TTypeDocumentationElement typeDocumentationElement, Func<IEnumerable<MethodDocumentationElement>> selector, string methodName)
            where TTypeDocumentationElement : TypeDocumentationElement
        {
            typeDocumentationElement
                .AssertCollectionMember(
                    () => selector().Where(method => method.Name == methodName),
                    method =>
                    {
                        method
                            .AssertEmpty(() => method.GenericParameters)
                            .AssertEmpty(() => method.Parameters)
                            .AssertType(() => method.Return.Type, typeof(int))
                            .AssertEqual(() => method.AccessModifier, AccessModifier.Public)
                            .AssertSame(() => method.DeclaringType, typeDocumentationElement)
                            .AssertFalse(() => method.IsStatic)
                            .AssertFalse(() => method.IsVirtual)
                            .AssertFalse(() => method.IsAbstract)
                            .AssertFalse(() => method.IsOverride)
                            .AssertFalse(() => method.IsSealed)
                            .AssertTrue(() => method.IsShadowing);
                    }
                );

            return typeDocumentationElement;
        }

        public static TTypeDocumentationElement AssertStaticMethod<TTypeDocumentationElement>(this TTypeDocumentationElement typeDocumentationElement, Func<IEnumerable<MethodDocumentationElement>> selector, string methodName)
            where TTypeDocumentationElement : TypeDocumentationElement
        {
            typeDocumentationElement
                .AssertCollectionMember(
                    () => selector().Where(method => method.Name == methodName),
                    method =>
                    {
                        method
                            .AssertEmpty(() => method.GenericParameters)
                            .AssertEmpty(() => method.Parameters)
                            .AssertType(() => method.Return.Type, typeof(void))
                            .AssertEqual(() => method.AccessModifier, AccessModifier.Public)
                            .AssertSame(() => method.DeclaringType, typeDocumentationElement)
                            .AssertTrue(() => method.IsStatic)
                            .AssertFalse(() => method.IsVirtual)
                            .AssertFalse(() => method.IsAbstract)
                            .AssertFalse(() => method.IsOverride)
                            .AssertFalse(() => method.IsSealed)
                            .AssertFalse(() => method.IsShadowing);
                    }
                );

            return typeDocumentationElement;
        }

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