using System;
using System.Collections.Generic;
using System.Linq;
using CodeMap.Elements;
using Xunit;

namespace CodeMap.Tests
{
    public class ReflectionDocumentationElementFactoryTests
    {
        private readonly SummaryDocumentationElement _summaryDocumentationElement = DocumentationElement.Summary();
        private readonly RemarksDocumentationElement _remarksDocumentationElement = DocumentationElement.Remarks();
        private readonly IReadOnlyCollection<ExampleDocumentationElement> _exampleDocumentationElements = new[] { DocumentationElement.Example() };
        private readonly RelatedMembersList _relatedMembersList = DocumentationElement.RelatedMembersList(Enumerable.Empty<MemberReferenceDocumentationElement>());

        [Fact]
        public void CreateEnumDocumentationElementCheckBasicInformation()
        {
            var factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = factory.Create(typeof(TestEnum));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestEnum")
                .AssertEqual(() => typeDocumentationElement.AccessModifier, AccessModifier.Assembly)
                .AssertNull(() => typeDocumentationElement.DeclaringType)
                .AssertEmpty(() => typeDocumentationElement.Summary.Content)
                .AssertEmpty(() => typeDocumentationElement.Remarks.Content)
                .AssertEmpty(() => typeDocumentationElement.Examples)
                .AssertEmpty(() => typeDocumentationElement.RelatedMembers)
                .AssertIs<EnumDocumentationElement>(
                    enumDocumentationElement => enumDocumentationElement
                        .AssertTypeReference(() => enumDocumentationElement.UnderlyingType, "System", "Byte")
                        .AssertTypeReferenceAssembly(() => enumDocumentationElement.UnderlyingType, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                );
        }

        [Fact]
        public void CreateEnumDocumentationElementCheckDocumentation()
        {
            var enumDocumentationMember = _CreateMemberDocumentationMock("T:CodeMap.Tests.TestEnum");
            var factory = new ReflectionDocumentationElementFactory(
                new MemberDocumentationCollection(new[] { enumDocumentationMember })
            );

            var typeDocumentationElement = factory.Create(typeof(TestEnum));

            typeDocumentationElement
                .AssertSame(() => typeDocumentationElement.Summary, enumDocumentationMember.Summary)
                .AssertSame(() => typeDocumentationElement.Remarks, enumDocumentationMember.Remarks)
                .AssertSame(() => typeDocumentationElement.Examples, enumDocumentationMember.Examples)
                .AssertSame(() => typeDocumentationElement.RelatedMembers, enumDocumentationMember.RelatedMembers);
        }

        [Fact]
        public void CreateEnumDocumentationElementCheckAttributes()
        {
            var factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = factory.Create(typeof(TestEnum));

            typeDocumentationElement
                .AssertCollectionMember(
                    () => typeDocumentationElement.Attributes,
                    attribute => attribute
                        .AssertTypeReference(() => attribute.Type, "CodeMap.Tests", "TestAttribute")
                        .AssertTypeReferenceAssembly(() => attribute.Type, "CodeMap.Tests", new Version(1, 2, 3, 4))
                        .AssertCollectionMember(
                            () => attribute.PositionalParameters,
                            positionalParameter => positionalParameter
                                .AssertEqual(() => positionalParameter.Name, "value1")
                                .AssertEqual(() => positionalParameter.Value, "test 1")
                                .AssertTypeReference(() => positionalParameter.Type, "System", "Object")
                                .AssertTypeReferenceAssembly(() => positionalParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                        )
                        .AssertCollectionMember(
                            () => attribute.NamedParameters.OrderBy(namedParameter => namedParameter.Name),
                            namedParameter => namedParameter
                                .AssertEqual(() => namedParameter.Name, "Value2")
                                .AssertEqual(() => namedParameter.Value, "test 2")
                                .AssertTypeReference(() => namedParameter.Type, "System", "Object")
                                .AssertTypeReferenceAssembly(() => namedParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0)),
                            namedParameter => namedParameter
                                .AssertEqual(() => namedParameter.Name, "Value3")
                                .AssertEqual(() => namedParameter.Value, "test 3")
                                .AssertTypeReference(() => namedParameter.Type, "System", "Object")
                                .AssertTypeReferenceAssembly(() => namedParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                        )
                );
        }

        [Fact]
        public void CreateEnumDocumentationElementCheckMembers()
        {
            var factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = factory.Create(typeof(TestEnum));

            typeDocumentationElement
                .AssertIs<EnumDocumentationElement>(
                    enumDocumentationElement => enumDocumentationElement
                        .AssertCollectionMember(
                            () => enumDocumentationElement.Members,
                            enumMember => enumMember
                                .AssertEqual(() => enumMember.Name, "TestMember1")
                                .AssertEqual(() => enumMember.AccessModifier, AccessModifier.Public)
                                .AssertSame(() => enumMember.DeclaringType, enumDocumentationElement)
                                .AssertTypeReference(() => enumMember.Type, "CodeMap.Tests", "TestEnum")
                                .AssertTypeReferenceAssembly(() => enumMember.Type, "CodeMap.Tests", new Version(1, 2, 3, 4))
                                .AssertEqual(() => enumMember.Value, TestEnum.TestMember1)
                                .AssertEmpty(() => enumMember.Summary.Content)
                                .AssertEmpty(() => enumMember.Remarks.Content)
                                .AssertEmpty(() => enumMember.Examples)
                                .AssertEmpty(() => enumMember.RelatedMembers),
                            enumMember => enumMember
                                .AssertEqual(() => enumMember.Name, "TestMember2")
                                .AssertEqual(() => enumMember.AccessModifier, AccessModifier.Public)
                                .AssertSame(() => enumMember.DeclaringType, enumDocumentationElement)
                                .AssertTypeReference(() => enumMember.Type, "CodeMap.Tests", "TestEnum")
                                .AssertTypeReferenceAssembly(() => enumMember.Type, "CodeMap.Tests", new Version(1, 2, 3, 4))
                                .AssertEqual(() => enumMember.Value, TestEnum.TestMember2)
                                .AssertEmpty(() => enumMember.Summary.Content)
                                .AssertEmpty(() => enumMember.Remarks.Content)
                                .AssertEmpty(() => enumMember.Examples)
                                .AssertEmpty(() => enumMember.RelatedMembers),
                            enumMember => enumMember
                                .AssertEqual(() => enumMember.Name, "TestMember3")
                                .AssertEqual(() => enumMember.AccessModifier, AccessModifier.Public)
                                .AssertSame(() => enumMember.DeclaringType, enumDocumentationElement)
                                .AssertTypeReference(() => enumMember.Type, "CodeMap.Tests", "TestEnum")
                                .AssertTypeReferenceAssembly(() => enumMember.Type, "CodeMap.Tests", new Version(1, 2, 3, 4))
                                .AssertEqual(() => enumMember.Value, TestEnum.TestMember3)
                                .AssertEmpty(() => enumMember.Summary.Content)
                                .AssertEmpty(() => enumMember.Remarks.Content)
                                .AssertEmpty(() => enumMember.Examples)
                                .AssertEmpty(() => enumMember.RelatedMembers)
                        )
                );
        }

        [Fact]
        public void CreateEnumDocumentationElementCheckMemberAttributes()
        {
            var factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = factory.Create(typeof(TestEnum));

            typeDocumentationElement
                .AssertIs<EnumDocumentationElement>(
                    enumDocumentationElement => enumDocumentationElement
                        .AssertCollectionMember(
                            () => enumDocumentationElement.Members,
                            enumMember => enumMember
                                .AssertEqual(() => enumMember.Name, "TestMember1")
                                .AssertCollectionMember(
                                    () => enumMember.Attributes,
                                    attribute => attribute
                                        .AssertTypeReference(() => attribute.Type, "CodeMap.Tests", "TestAttribute")
                                        .AssertTypeReferenceAssembly(() => attribute.Type, "CodeMap.Tests", new Version(1, 2, 3, 4))
                                        .AssertCollectionMember(
                                            () => attribute.PositionalParameters,
                                            positionalParameter => positionalParameter
                                                .AssertEqual(() => positionalParameter.Name, "value1")
                                                .AssertEqual(() => positionalParameter.Value, "member test 1")
                                                .AssertTypeReference(() => positionalParameter.Type, "System", "Object")
                                                .AssertTypeReferenceAssembly(() => positionalParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                        )
                                        .AssertCollectionMember(
                                            () => attribute.NamedParameters.OrderBy(namedParameter => namedParameter.Name),
                                            namedParameter => namedParameter
                                                .AssertEqual(() => namedParameter.Name, "Value2")
                                                .AssertEqual(() => namedParameter.Value, "member test 2")
                                                .AssertTypeReference(() => namedParameter.Type, "System", "Object")
                                                .AssertTypeReferenceAssembly(() => namedParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0)),
                                            namedParameter => namedParameter
                                                .AssertEqual(() => namedParameter.Name, "Value3")
                                                .AssertEqual(() => namedParameter.Value, "member test 3")
                                                .AssertTypeReference(() => namedParameter.Type, "System", "Object")
                                                .AssertTypeReferenceAssembly(() => namedParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                        )
                                ),
                            enumMember => enumMember
                                .AssertEqual(() => enumMember.Name, "TestMember2")
                                .AssertEmpty(() => enumMember.Attributes),
                            enumMember => enumMember
                                .AssertEqual(() => enumMember.Name, "TestMember3")
                                .AssertEmpty(() => enumMember.Attributes)
                        )
                );
        }

        [Fact]
        public void CreateEnumDocumentationElementCheckMemberDocumentation()
        {
            var enumMemberDocumentationMember = _CreateMemberDocumentationMock("F:CodeMap.Tests.TestEnum.TestMember1");
            var factory = new ReflectionDocumentationElementFactory(
                new MemberDocumentationCollection(new[] { enumMemberDocumentationMember })
            );

            var typeDocumentationElement = factory.Create(typeof(TestEnum));

            typeDocumentationElement
                .AssertIs<EnumDocumentationElement>(
                    enumDocumentationElement => enumDocumentationElement
                        .AssertTypeReference(() => enumDocumentationElement.UnderlyingType, "System", "Byte")
                        .AssertTypeReferenceAssembly(() => enumDocumentationElement.UnderlyingType, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                        .AssertCollectionMember(
                            () => enumDocumentationElement.Members,
                            enumMember => enumMember
                                .AssertEqual(() => enumMember.Name, "TestMember1")
                                .AssertSame(() => enumMember.Summary, enumMemberDocumentationMember.Summary)
                                .AssertSame(() => enumMember.Remarks, enumMemberDocumentationMember.Remarks)
                                .AssertSame(() => enumMember.Examples, enumMemberDocumentationMember.Examples)
                                .AssertSame(() => enumMember.RelatedMembers, enumMemberDocumentationMember.RelatedMembers),
                            enumMember => enumMember
                                .AssertEqual(() => enumMember.Name, "TestMember2")
                                .AssertEmpty(() => enumMember.Summary.Content)
                                .AssertEmpty(() => enumMember.Remarks.Content)
                                .AssertEmpty(() => enumMember.Examples)
                                .AssertEmpty(() => enumMember.RelatedMembers),
                            enumMember => enumMember
                                .AssertEqual(() => enumMember.Name, "TestMember3")
                                .AssertEmpty(() => enumMember.Summary.Content)
                                .AssertEmpty(() => enumMember.Remarks.Content)
                                .AssertEmpty(() => enumMember.Examples)
                                .AssertEmpty(() => enumMember.RelatedMembers)
                        )
                );
        }

        [Fact]
        public void CreateDelegateDocumentationElementCheckBasicInformation()
        {
            var _factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = _factory.Create(typeof(TestDelegate<,,>));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestDelegate")
                .AssertEqual(() => typeDocumentationElement.AccessModifier, AccessModifier.Assembly)
                .AssertNull(() => typeDocumentationElement.DeclaringType)
                .AssertEmpty(() => typeDocumentationElement.Summary.Content)
                .AssertEmpty(() => typeDocumentationElement.Remarks.Content)
                .AssertEmpty(() => typeDocumentationElement.Examples)
                .AssertEmpty(() => typeDocumentationElement.RelatedMembers)
                .AssertIs<DelegateDocumentationElement>(delegateDocumentationElement => delegateDocumentationElement
                    .AssertEmpty(() => delegateDocumentationElement.Exceptions)
                );
        }

        [Fact]
        public void CreateDelegateDocumentationElementCheckDocumentation()
        {
            var memberDocumentation = _CreateMemberDocumentationMock(
                "T:CodeMap.Tests.TestDelegate`3",
                exceptions: new[] { "T:System.ArgumentException" }
            );
            var _factory = new ReflectionDocumentationElementFactory(new MemberDocumentationCollection(new[] { memberDocumentation }));

            var typeDocumentationElement = _factory.Create(typeof(TestDelegate<,,>));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestDelegate")
                .AssertSame(() => typeDocumentationElement.Summary, memberDocumentation.Summary)
                .AssertSame(() => typeDocumentationElement.Remarks, memberDocumentation.Remarks)
                .AssertSame(() => typeDocumentationElement.Examples, memberDocumentation.Examples)
                .AssertSame(() => typeDocumentationElement.RelatedMembers, memberDocumentation.RelatedMembers)
                .AssertIs<DelegateDocumentationElement>(delegateDocumentationElement => delegateDocumentationElement
                    .AssertCollectionMember(
                        () => delegateDocumentationElement.Exceptions,
                        exception => exception
                            .AssertTypeReference(() => exception.Type, "System", "ArgumentException")
                            .AssertTypeReferenceAssembly(() => exception.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                            .AssertSameItems(() => exception.Description, memberDocumentation.Exceptions["T:System.ArgumentException"])
                    )
                );
        }

        [Fact]
        public void CreateDelegateDocumentationElementCheckAttributes()
        {
            var _factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = _factory.Create(typeof(TestDelegate<,,>));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestDelegate")
                .AssertCollectionMember(
                    () => typeDocumentationElement.Attributes,
                    attribute =>
                        attribute
                            .AssertTypeReference(() => attribute.Type, "CodeMap.Tests", "TestAttribute")
                            .AssertTypeReferenceAssembly(() => attribute.Type, "CodeMap.Tests", new Version(1, 2, 3, 4))
                            .AssertCollectionMember(
                                () => attribute.PositionalParameters,
                                parameter => parameter
                                    .AssertEqual(() => parameter.Name, "value1")
                                    .AssertEqual(() => parameter.Value, "test 1")
                                    .AssertTypeReference(() => parameter.Type, "System", "Object")
                                    .AssertTypeReferenceAssembly(() => parameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                            )
                            .AssertCollectionMember(
                                () => attribute.NamedParameters.OrderBy(namedParameter => namedParameter.Name),
                                parameter => parameter
                                    .AssertEqual(() => parameter.Name, "Value2")
                                    .AssertEqual(() => parameter.Value, "test 2")
                                    .AssertTypeReference(() => parameter.Type, "System", "Object")
                                    .AssertTypeReferenceAssembly(() => parameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0)),
                                parameter => parameter
                                    .AssertEqual(() => parameter.Name, "Value3")
                                    .AssertEqual(() => parameter.Value, "test 3")
                                    .AssertTypeReference(() => parameter.Type, "System", "Object")
                                    .AssertTypeReferenceAssembly(() => parameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                            )
                );
        }

        [Fact]
        public void CreateDelegateDocumentationElementCheckReturnType()
        {
            var _factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = _factory.Create(typeof(TestDelegate<,,>));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestDelegate")
                .AssertIs<DelegateDocumentationElement>(
                    delegateDocumentationElement => delegateDocumentationElement.Return.Type.AssertIs<VoidTypeReferenceDocumentationElement>()
                );
        }

        [Fact]
        public void CreateDelegateDocumentationElementCheckReturnDocumentation()
        {
            var memberDocumentation = _CreateMemberDocumentationMock("T:CodeMap.Tests.TestDelegate`3");
            var _factory = new ReflectionDocumentationElementFactory(new MemberDocumentationCollection(new[] { memberDocumentation }));

            var typeDocumentationElement = _factory.Create(typeof(TestDelegate<,,>));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestDelegate")
                .AssertIs<DelegateDocumentationElement>(
                    delegateDocumentationElement => delegateDocumentationElement
                        .AssertSameItems(() => delegateDocumentationElement.Return.Description, memberDocumentation.Returns)
                );
        }

        [Fact]
        public void CreateDelegateDocumentationElementCheckReturnAttributes()
        {
            var _factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = _factory.Create(typeof(TestDelegate<,,>));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestDelegate")
                .AssertIs<DelegateDocumentationElement>(
                    delegateDocumentationElement => delegateDocumentationElement
                        .AssertCollectionMember(
                            () => delegateDocumentationElement.Return.Attributes,
                            attribute =>
                                attribute
                                    .AssertTypeReference(() => attribute.Type, "CodeMap.Tests", "TestAttribute")
                                    .AssertTypeReferenceAssembly(() => attribute.Type, "CodeMap.Tests", new Version(1, 2, 3, 4))
                                    .AssertCollectionMember(
                                        () => attribute.PositionalParameters,
                                        parameter => parameter
                                            .AssertEqual(() => parameter.Name, "value1")
                                            .AssertEqual(() => parameter.Value, "return test 1")
                                            .AssertTypeReference(() => parameter.Type, "System", "Object")
                                            .AssertTypeReferenceAssembly(() => parameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                    )
                                    .AssertCollectionMember(
                                        () => attribute.NamedParameters.OrderBy(namedParameter => namedParameter.Name),
                                        parameter => parameter
                                            .AssertEqual(() => parameter.Name, "Value2")
                                            .AssertEqual(() => parameter.Value, "return test 2")
                                            .AssertTypeReference(() => parameter.Type, "System", "Object")
                                            .AssertTypeReferenceAssembly(() => parameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0)),
                                        parameter => parameter
                                            .AssertEqual(() => parameter.Name, "Value3")
                                            .AssertEqual(() => parameter.Value, "return test 3")
                                            .AssertTypeReference(() => parameter.Type, "System", "Object")
                                            .AssertTypeReferenceAssembly(() => parameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                    )
                    )
                );
        }

        [Fact]
        public void CreateDelegateDocumentationElementCheckGenericParameters()
        {
            var _factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = _factory.Create(typeof(TestDelegate<,,>));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestDelegate")
                .AssertIs<DelegateDocumentationElement>(
                    delegateDocumentationElement => delegateDocumentationElement
                        .AssertCollectionMember(
                            () => delegateDocumentationElement.GenericParameters,
                            genericParameter =>
                                genericParameter
                                    .AssertEqual(() => genericParameter.Name, "TParam1")
                                    .AssertTrue(() => genericParameter.IsCovariant)
                                    .AssertFalse(() => genericParameter.IsContravariant)
                                    .AssertTrue(() => genericParameter.HasReferenceTypeConstraint)
                                    .AssertFalse(() => genericParameter.HasNonNullableValueTypeConstraint)
                                    .AssertTrue(() => genericParameter.HasDefaultConstructorConstraint)
                                    .AssertCollectionMember(
                                        () => genericParameter.TypeConstraints,
                                        typeConstraint => typeConstraint
                                            .AssertSame(delegateDocumentationElement.GenericParameters.ElementAt(1)),
                                        typeConstraint => typeConstraint
                                            .AssertTypeReference("System", "IComparable")
                                            .AssertGenericArguments(
                                                genericArgument => genericArgument.AssertSame(delegateDocumentationElement.GenericParameters.ElementAt(0))
                                            )
                                            .AssertTypeReferenceAssembly("System.Private.CoreLib", new Version(4, 0, 0, 0))
                                    )
                                    .AssertEmpty(() => genericParameter.Description),
                            genericParameter =>
                                genericParameter
                                    .AssertEqual(() => genericParameter.Name, "TParam2")
                                    .AssertFalse(() => genericParameter.IsCovariant)
                                    .AssertTrue(() => genericParameter.IsContravariant)
                                    .AssertFalse(() => genericParameter.HasReferenceTypeConstraint)
                                    .AssertFalse(() => genericParameter.HasNonNullableValueTypeConstraint)
                                    .AssertFalse(() => genericParameter.HasDefaultConstructorConstraint)
                                    .AssertEmpty(() => genericParameter.TypeConstraints)
                                    .AssertEmpty(() => genericParameter.Description),
                            genericParameter =>
                                genericParameter
                                    .AssertEqual(() => genericParameter.Name, "TParam3")
                                    .AssertFalse(() => genericParameter.IsCovariant)
                                    .AssertFalse(() => genericParameter.IsContravariant)
                                    .AssertFalse(() => genericParameter.HasReferenceTypeConstraint)
                                    .AssertTrue(() => genericParameter.HasNonNullableValueTypeConstraint)
                                    .AssertFalse(() => genericParameter.HasDefaultConstructorConstraint)
                                    .AssertEmpty(() => genericParameter.TypeConstraints)
                                    .AssertEmpty(() => genericParameter.Description)
                    )
                );
        }

        [Fact]
        public void CreateDelegateDocumentationElementCheckGenericParametersDocumentation()
        {
            var memberDocumentation = _CreateMemberDocumentationMock("T:CodeMap.Tests.TestDelegate`3", genericParameters: new[] { "TParam1" });
            var _factory = new ReflectionDocumentationElementFactory(new MemberDocumentationCollection(new[] { memberDocumentation }));

            var typeDocumentationElement = _factory.Create(typeof(TestDelegate<,,>));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestDelegate")
                .AssertIs<DelegateDocumentationElement>(
                    delegateDocumentationElement => delegateDocumentationElement
                        .AssertCollectionMember(
                            () => delegateDocumentationElement.GenericParameters,
                            genericParameter =>
                                genericParameter
                                    .AssertEqual(() => genericParameter.Name, "TParam1")
                                    .AssertSameItems(() => genericParameter.Description, memberDocumentation.GenericParameters["TParam1"]),
                            genericParameter =>
                                genericParameter
                                    .AssertEqual(() => genericParameter.Name, "TParam2")
                                    .AssertEmpty(() => genericParameter.Description),
                            genericParameter =>
                                genericParameter
                                    .AssertEqual(() => genericParameter.Name, "TParam3")
                                    .AssertEmpty(() => genericParameter.Description)
                    )
                );
        }

        [Fact]
        public void CreateDelegateDocumentationElementCheckParameters()
        {
            var _factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = _factory.Create(typeof(TestDelegate<,,>));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestDelegate")
                .AssertIs<DelegateDocumentationElement>(
                    delegateDocumentationElement => delegateDocumentationElement
                        .AssertCollectionMember(
                            () => delegateDocumentationElement.Parameters,
                            parameter =>
                                parameter
                                    .AssertEqual(() => parameter.Name, "param1")
                                    .AssertTypeReference(() => parameter.Type, "System", "Int32")
                                    .AssertTypeReferenceAssembly(() => parameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                    .AssertFalse(() => parameter.IsInputByReference)
                                    .AssertFalse(() => parameter.IsInputOutputByReference)
                                    .AssertFalse(() => parameter.IsOutputByReference)
                                    .AssertFalse(() => parameter.HasDefaultValue)
                                    .AssertNull(() => parameter.DefaultValue)
                                    .AssertEmpty(() => parameter.Description),
                            parameter =>
                                parameter
                                    .AssertEqual(() => parameter.Name, "param2")
                                    .AssertSame(() => parameter.Type, delegateDocumentationElement.GenericParameters.ElementAt(1))
                                    .AssertFalse(() => parameter.IsInputByReference)
                                    .AssertFalse(() => parameter.IsInputOutputByReference)
                                    .AssertFalse(() => parameter.IsOutputByReference)
                                    .AssertFalse(() => parameter.HasDefaultValue)
                                    .AssertNull(() => parameter.DefaultValue)
                                    .AssertEmpty(() => parameter.Description),
                            parameter =>
                                parameter
                                    .AssertEqual(() => parameter.Name, "param3")
                                    .AssertTypeReference(() => parameter.Type, "System", "Char")
                                    .AssertTypeReferenceAssembly(() => parameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                    .AssertFalse(() => parameter.IsInputByReference)
                                    .AssertTrue(() => parameter.IsInputOutputByReference)
                                    .AssertFalse(() => parameter.IsOutputByReference)
                                    .AssertFalse(() => parameter.HasDefaultValue)
                                    .AssertNull(() => parameter.DefaultValue)
                                    .AssertEmpty(() => parameter.Description),
                            parameter =>
                                parameter
                                    .AssertEqual(() => parameter.Name, "param4")
                                    .AssertTypeReference(() => parameter.Type, "System", "Decimal")
                                    .AssertTypeReferenceAssembly(() => parameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                    .AssertFalse(() => parameter.IsInputByReference)
                                    .AssertFalse(() => parameter.IsInputOutputByReference)
                                    .AssertTrue(() => parameter.IsOutputByReference)
                                    .AssertFalse(() => parameter.HasDefaultValue)
                                    .AssertNull(() => parameter.DefaultValue)
                                    .AssertEmpty(() => parameter.Description),
                            parameter =>
                                parameter
                                    .AssertEqual(() => parameter.Name, "param5")
                                    .AssertTypeReference(() => parameter.Type, "System", "String")
                                    .AssertTypeReferenceAssembly(() => parameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                    .AssertFalse(() => parameter.IsInputByReference)
                                    .AssertFalse(() => parameter.IsInputOutputByReference)
                                    .AssertFalse(() => parameter.IsOutputByReference)
                                    .AssertTrue(() => parameter.HasDefaultValue)
                                    .AssertEqual(() => parameter.DefaultValue, "test")
                                    .AssertEmpty(() => parameter.Description)
                    )
                );
        }

        [Fact]
        public void CreateDelegateDocumentationElementCheckParameterAttributes()
        {
            var _factory = new ReflectionDocumentationElementFactory();

            var typeDocumentationElement = _factory.Create(typeof(TestDelegate<,,>));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestDelegate")
                .AssertIs<DelegateDocumentationElement>(
                    delegateDocumentationElement => delegateDocumentationElement
                        .AssertCollectionMember(
                            () => delegateDocumentationElement.Parameters,
                            parameter =>
                                parameter
                                    .AssertEqual(() => parameter.Name, "param1")
                                    .AssertCollectionMember(
                                        () => parameter.Attributes,
                                        attribute => attribute
                                            .AssertTypeReference(() => attribute.Type, "CodeMap.Tests", "TestAttribute")
                                            .AssertTypeReferenceAssembly(() => attribute.Type, "CodeMap.Tests", new Version(1, 2, 3, 4))
                                            .AssertCollectionMember(
                                                () => attribute.PositionalParameters,
                                                attributeParameter => attributeParameter
                                                    .AssertEqual(() => attributeParameter.Name, "value1")
                                                    .AssertEqual(() => attributeParameter.Value, "param test 1")
                                                    .AssertTypeReference(() => attributeParameter.Type, "System", "Object")
                                                    .AssertTypeReferenceAssembly(() => attributeParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                            )
                                            .AssertCollectionMember(
                                                () => attribute.NamedParameters.OrderBy(attributeParameter => attributeParameter.Name),
                                                attributeParameter => attributeParameter
                                                    .AssertEqual(() => attributeParameter.Name, "Value2")
                                                    .AssertEqual(() => attributeParameter.Value, "param test 2")
                                                    .AssertTypeReference(() => attributeParameter.Type, "System", "Object")
                                                    .AssertTypeReferenceAssembly(() => attributeParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0)),
                                                attributeParameter => attributeParameter
                                                    .AssertEqual(() => attributeParameter.Name, "Value3")
                                                    .AssertEqual(() => attributeParameter.Value, "param test 3")
                                                    .AssertTypeReference(() => attributeParameter.Type, "System", "Object")
                                                    .AssertTypeReferenceAssembly(() => attributeParameter.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                            )
                                    ),
                            parameter =>
                                parameter
                                    .AssertEqual(() => parameter.Name, "param2")
                                    .AssertEmpty(() => parameter.Attributes),
                            parameter =>
                                parameter
                                    .AssertEqual(() => parameter.Name, "param3")
                                    .AssertEmpty(() => parameter.Attributes),
                            parameter =>
                                parameter
                                    .AssertEqual(() => parameter.Name, "param4")
                                    .AssertCollectionMember(
                                        () => parameter.Attributes,
                                        attribute => attribute
                                            .AssertTypeReference(() => attribute.Type, "System.Runtime.InteropServices", "OutAttribute")
                                            .AssertTypeReferenceAssembly(() => attribute.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                            .AssertEmpty(() => attribute.PositionalParameters)
                                            .AssertEmpty(() => attribute.NamedParameters)
                                    ),
                            parameter =>
                                parameter
                                    .AssertEqual(() => parameter.Name, "param5")
                                    .AssertCollectionMember(
                                        () => parameter.Attributes,
                                        attribute => attribute
                                            .AssertTypeReference(() => attribute.Type, "System.Runtime.InteropServices", "OptionalAttribute")
                                            .AssertTypeReferenceAssembly(() => attribute.Type, "System.Private.CoreLib", new Version(4, 0, 0, 0))
                                            .AssertEmpty(() => attribute.PositionalParameters)
                                            .AssertEmpty(() => attribute.NamedParameters)
                                    )
                    )
                );
        }

        [Fact]
        public void CreateDelegateDocumentationElementCheckParametersDocumentation()
        {
            var memberDocumentation = _CreateMemberDocumentationMock("T:CodeMap.Tests.TestDelegate`3", parameters: new[] { "param1" });
            var _factory = new ReflectionDocumentationElementFactory(new MemberDocumentationCollection(new[] { memberDocumentation }));

            var typeDocumentationElement = _factory.Create(typeof(TestDelegate<,,>));

            typeDocumentationElement
                .AssertEqual(() => typeDocumentationElement.Name, "TestDelegate")
                .AssertIs<DelegateDocumentationElement>(
                    delegateDocumentationElement => delegateDocumentationElement
                        .AssertCollectionMember(
                            () => delegateDocumentationElement.Parameters,
                            parameter =>
                                parameter
                                    .AssertEqual(() => parameter.Name, "param1")
                                    .AssertSameItems(() => parameter.Description, memberDocumentation.Parameters["param1"]),
                            parameter =>
                                parameter
                                    .AssertEqual(() => parameter.Name, "param2")
                                    .AssertEmpty(() => parameter.Description),
                            parameter =>
                                parameter
                                    .AssertEqual(() => parameter.Name, "param3")
                                    .AssertEmpty(() => parameter.Description),
                            parameter =>
                                parameter
                                    .AssertEqual(() => parameter.Name, "param4")
                                    .AssertEmpty(() => parameter.Description),
                            parameter =>
                                parameter
                                    .AssertEqual(() => parameter.Name, "param5")
                                    .AssertEmpty(() => parameter.Description)
                    )
                );
        }

        [Fact]
        public void ConstructorWithNullMembersDocumentationCollectionThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new ReflectionDocumentationElementFactory(null));
            Assert.Equal(new ArgumentNullException("membersDocumentation").Message, exception.Message);
        }

        private static MemberDocumentation _CreateMemberDocumentationMock(string canonicalName, IEnumerable<string> genericParameters = null, IEnumerable<string> parameters = null, IEnumerable<string> exceptions = null)
            => new MemberDocumentation(
                canonicalName,
                DocumentationElement.Summary(DocumentationElement.Paragraph()),
                genericParameters?.ToLookup(
                    genericParameter => genericParameter,
                    genericParameter => DocumentationElement.Paragraph() as BlockDocumentationElement
                ),
                parameters?.ToLookup(
                    parameter => parameter,
                    parameter => DocumentationElement.Paragraph() as BlockDocumentationElement
                ),
                new[] { DocumentationElement.Paragraph() },
                exceptions?.ToLookup(
                    exception => exception,
                    exception => DocumentationElement.Paragraph() as BlockDocumentationElement
                ),
                DocumentationElement.Remarks(DocumentationElement.Paragraph()),
                new[] { DocumentationElement.Example(DocumentationElement.Paragraph()) },
                DocumentationElement.Value(DocumentationElement.Paragraph()),
                DocumentationElement.RelatedMembersList(DocumentationElement.MemberReference(typeof(object)))
            );
    }
}