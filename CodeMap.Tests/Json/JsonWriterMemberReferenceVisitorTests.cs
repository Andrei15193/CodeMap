using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using CodeMap.Json;
using CodeMap.ReferenceData;
using CodeMap.Tests.Data;
using Newtonsoft.Json;
using Xunit;

namespace CodeMap.Tests.Json
{
    public class JsonWriterMemberReferenceVisitorTests
    {
        private const string _testDataPublicKeyToken = "";

        [Fact]
        public void SerializeSimpleType()
            => _Assert(
                typeof(int),
                @"{
    ""kind"": ""specific"",
    ""name"": ""Int32"",
    ""namespace"": ""System"",
    ""declaringType"": null,
    ""genericArguments"": [],
    ""assembly"": {
        ""name"": ""System.Private.CoreLib"",
        ""version"": ""4.0.0.0"",
        ""culture"": """",
        ""publicKeyToken"": ""7cec85d7bea7798e""
    }
}"
            );

        [Fact]
        public void SerializeVoidType()
            => _Assert(
                typeof(void),
                @"{
    ""kind"": ""specific/void""
}"
            );

        [Fact]
        public void SerializeDynamicType()
            => _Assert(
                memberReferenceFactory => memberReferenceFactory.CreateDynamic(),
                @"{
    ""kind"": ""specific/dynamic""
}"
            );

        [Fact]
        public void SerializeArrayType()
            => _Assert(
                typeof(int[][,]),
                @"{
    ""kind"": ""array"",
    ""rank"": 1,
    ""itemType"": {
        ""kind"": ""array"",
        ""rank"": 2,
        ""itemType"": {
            ""kind"": ""specific"",
            ""name"": ""Int32"",
            ""namespace"": ""System"",
            ""declaringType"": null,
            ""genericArguments"": [],
            ""assembly"": {
                ""name"": ""System.Private.CoreLib"",
                ""version"": ""4.0.0.0"",
                ""culture"": """",
                ""publicKeyToken"": ""7cec85d7bea7798e""
            }
        }
    }
}"
            );

        [Fact]
        public void SerializePointerType()
            => _Assert(
                typeof(long**),
                @"{
    ""kind"": ""pointer"",
    ""referentType"": {
        ""kind"": ""pointer"",
        ""referentType"": {
            ""kind"": ""specific"",
            ""name"": ""Int64"",
            ""namespace"": ""System"",
            ""declaringType"": null,
            ""genericArguments"": [],
            ""assembly"": {
                ""name"": ""System.Private.CoreLib"",
                ""version"": ""4.0.0.0"",
                ""culture"": """",
                ""publicKeyToken"": ""7cec85d7bea7798e""
            }
        }
    }
}"
            );

        [Fact]
        public void SerializeByRefType()
            => _Assert(
                typeof(short).MakeByRefType(),
                @"{
    ""kind"": ""byRef"",
    ""referentType"": {
        ""kind"": ""specific"",
        ""name"": ""Int16"",
        ""namespace"": ""System"",
        ""declaringType"": null,
        ""genericArguments"": [],
        ""assembly"": {
            ""name"": ""System.Private.CoreLib"",
            ""version"": ""4.0.0.0"",
            ""culture"": """",
            ""publicKeyToken"": ""7cec85d7bea7798e""
        }
    }
}"
            );

        [Fact]
        public void SerializeConstructedGenericType()
            => _Assert(
                typeof(TestClass<int>.NestedTestClass<IEnumerable<string>, decimal>),
                @"{
    ""kind"": ""specific"",
    ""name"": ""NestedTestClass"",
    ""namespace"": ""CodeMap.Tests.Data"",
    ""declaringType"": {
        ""kind"": ""specific"",
        ""name"": ""TestClass"",
        ""namespace"": ""CodeMap.Tests.Data"",
        ""declaringType"": null,
        ""genericArguments"": [
            {
                ""kind"": ""specific"",
                ""name"": ""Int32"",
                ""namespace"": ""System"",
                ""declaringType"": null,
                ""genericArguments"": [],
                ""assembly"": {
                    ""name"": ""System.Private.CoreLib"",
                    ""version"": ""4.0.0.0"",
                    ""culture"": """",
                    ""publicKeyToken"": ""7cec85d7bea7798e""
                }
            }
        ],
        ""assembly"": {
            ""name"": ""CodeMap.Tests.Data"",
            ""version"": ""1.2.3.4"",
            ""culture"": """",
            ""publicKeyToken"": """ + _testDataPublicKeyToken + @"""
        }
    },
    ""genericArguments"": [
        {
            ""kind"": ""specific"",
            ""name"": ""IEnumerable"",
            ""namespace"": ""System.Collections.Generic"",
            ""declaringType"": null,
            ""genericArguments"": [
                {
                    ""kind"": ""specific"",
                    ""name"": ""String"",
                    ""namespace"": ""System"",
                    ""declaringType"": null,
                    ""genericArguments"": [],
                    ""assembly"": {
                        ""name"": ""System.Private.CoreLib"",
                        ""version"": ""4.0.0.0"",
                        ""culture"": """",
                        ""publicKeyToken"": ""7cec85d7bea7798e""
                    }
                }
            ],
            ""assembly"": {
                ""name"": ""System.Private.CoreLib"",
                ""version"": ""4.0.0.0"",
                ""culture"": """",
                ""publicKeyToken"": ""7cec85d7bea7798e""
            }
        },
        {
            ""kind"": ""specific"",
            ""name"": ""Decimal"",
            ""namespace"": ""System"",
            ""declaringType"": null,
            ""genericArguments"": [],
            ""assembly"": {
                ""name"": ""System.Private.CoreLib"",
                ""version"": ""4.0.0.0"",
                ""culture"": """",
                ""publicKeyToken"": ""7cec85d7bea7798e""
            }
        }
    ],
    ""assembly"": {
        ""name"": ""CodeMap.Tests.Data"",
        ""version"": ""1.2.3.4"",
        ""culture"": """",
        ""publicKeyToken"": """ + _testDataPublicKeyToken + @"""
    }
}"
            );

        [Fact]
        public void SerializeGenericTypeDefinition()
            => _Assert(
                typeof(TestClass<int>.NestedTestClass<IEnumerable<string>, decimal>).GetGenericTypeDefinition(),
                @"{
    ""kind"": ""specific"",
    ""name"": ""NestedTestClass"",
    ""namespace"": ""CodeMap.Tests.Data"",
    ""declaringType"": {
        ""kind"": ""specific"",
        ""name"": ""TestClass"",
        ""namespace"": ""CodeMap.Tests.Data"",
        ""declaringType"": null,
        ""genericArguments"": [
            {
                ""kind"": ""genericTypeParameter"",
                ""name"": ""TParam1""
            }
        ],
        ""assembly"": {
            ""name"": ""CodeMap.Tests.Data"",
            ""version"": ""1.2.3.4"",
            ""culture"": """",
            ""publicKeyToken"": """ + _testDataPublicKeyToken + @"""
        }
    },
    ""genericArguments"": [
        {
            ""kind"": ""genericTypeParameter"",
            ""name"": ""TParam2""
        },
        {
            ""kind"": ""genericTypeParameter"",
            ""name"": ""TParam3""
        }
    ],
    ""assembly"": {
        ""name"": ""CodeMap.Tests.Data"",
        ""version"": ""1.2.3.4"",
        ""culture"": """",
        ""publicKeyToken"": """ + _testDataPublicKeyToken + @"""
    }
}"
            );

        [Fact]
        public void SerializeConstant()
            => _Assert(
                typeof(TestClass<>).GetField("TestConstant", BindingFlags.Static | BindingFlags.NonPublic),
                @"{
    ""kind"": ""constant"",
    ""name"": ""TestConstant"",
    ""value"": 1.0,
    ""declaringType"": {
        ""kind"": ""specific"",
        ""name"": ""TestClass"",
        ""namespace"": ""CodeMap.Tests.Data"",
        ""declaringType"": null,
        ""genericArguments"": [
            {
                ""kind"": ""genericTypeParameter"",
                ""name"": ""TParam1""
            }
        ],
        ""assembly"": {
            ""name"": ""CodeMap.Tests.Data"",
            ""version"": ""1.2.3.4"",
            ""culture"": """",
            ""publicKeyToken"": """ + _testDataPublicKeyToken + @"""
        }
    }
}"
            );

        [Fact]
        public void SerializeField()
            => _Assert(
                typeof(TestClass<>).GetField("TestField", BindingFlags.Instance | BindingFlags.NonPublic),
                @"{
    ""kind"": ""field"",
    ""name"": ""TestField"",
    ""declaringType"": {
        ""kind"": ""specific"",
        ""name"": ""TestClass"",
        ""namespace"": ""CodeMap.Tests.Data"",
        ""declaringType"": null,
        ""genericArguments"": [
            {
                ""kind"": ""genericTypeParameter"",
                ""name"": ""TParam1""
            }
        ],
        ""assembly"": {
            ""name"": ""CodeMap.Tests.Data"",
            ""version"": ""1.2.3.4"",
            ""culture"": """",
            ""publicKeyToken"": """ + _testDataPublicKeyToken + @"""
        }
    }
}"
            );

        [Fact]
        public void SerializeConstructor()
            => _Assert(
                typeof(string).GetConstructor(new[] { typeof(char), typeof(int) }),
                @"{
    ""kind"": ""constructor"",
    ""declaringType"": {
        ""kind"": ""specific"",
        ""name"": ""String"",
        ""namespace"": ""System"",
        ""declaringType"": null,
        ""genericArguments"": [],
        ""assembly"": {
            ""name"": ""System.Private.CoreLib"",
            ""version"": ""4.0.0.0"",
            ""culture"": """",
            ""publicKeyToken"": ""7cec85d7bea7798e""
        }
    },
    ""parameterTypes"": [
        {
            ""kind"": ""specific"",
            ""name"": ""Char"",
            ""namespace"": ""System"",
            ""declaringType"": null,
            ""genericArguments"": [],
            ""assembly"": {
                ""name"": ""System.Private.CoreLib"",
                ""version"": ""4.0.0.0"",
                ""culture"": """",
                ""publicKeyToken"": ""7cec85d7bea7798e""
            }
        },
        {
            ""kind"": ""specific"",
            ""name"": ""Int32"",
            ""namespace"": ""System"",
            ""declaringType"": null,
            ""genericArguments"": [],
            ""assembly"": {
                ""name"": ""System.Private.CoreLib"",
                ""version"": ""4.0.0.0"",
                ""culture"": """",
                ""publicKeyToken"": ""7cec85d7bea7798e""
            }
        }
    ]
}"
            );

        [Fact]
        public void SerializeEvent()
            => _Assert(
                typeof(INotifyPropertyChanged).GetEvent("PropertyChanged", BindingFlags.Instance | BindingFlags.Public),
                @"{
    ""kind"": ""event"",
    ""name"": ""PropertyChanged"",
    ""declaringType"": {
        ""kind"": ""specific"",
        ""name"": ""INotifyPropertyChanged"",
        ""namespace"": ""System.ComponentModel"",
        ""declaringType"": null,
        ""genericArguments"": [],
        ""assembly"": {
            ""name"": ""System.ObjectModel"",
            ""version"": ""4.1.2.0"",
            ""culture"": """",
            ""publicKeyToken"": ""b03f5f7f11d50a3a""
        }
    }
}"
            );

        [Fact]
        public void SerializeProperty()
            => _Assert(
                typeof(IList<string>).GetDefaultMembers().OfType<PropertyInfo>().Single(),
                @"{
    ""kind"": ""property"",
    ""name"": ""Item"",
    ""declaringType"": {
        ""kind"": ""specific"",
        ""name"": ""IList"",
        ""namespace"": ""System.Collections.Generic"",
        ""declaringType"": null,
        ""genericArguments"": [
            {
                ""kind"": ""specific"",
                ""name"": ""String"",
                ""namespace"": ""System"",
                ""declaringType"": null,
                ""genericArguments"": [],
                ""assembly"": {
                    ""name"": ""System.Private.CoreLib"",
                    ""version"": ""4.0.0.0"",
                    ""culture"": """",
                    ""publicKeyToken"": ""7cec85d7bea7798e""
                }
            }
        ],
        ""assembly"": {
            ""name"": ""System.Private.CoreLib"",
            ""version"": ""4.0.0.0"",
            ""culture"": """",
            ""publicKeyToken"": ""7cec85d7bea7798e""
        }
    },
    ""parameterTypes"": [
        {
            ""kind"": ""specific"",
            ""name"": ""Int32"",
            ""namespace"": ""System"",
            ""declaringType"": null,
            ""genericArguments"": [],
            ""assembly"": {
                ""name"": ""System.Private.CoreLib"",
                ""version"": ""4.0.0.0"",
                ""culture"": """",
                ""publicKeyToken"": ""7cec85d7bea7798e""
            }
        }
    ]
}"
            );

        [Fact]
        public void SerializeMethod()
            => _Assert(
                typeof(int).GetMethod("Parse", new[] { typeof(string) }),
                @"{
    ""kind"": ""method"",
    ""name"": ""Parse"",
    ""declaringType"": {
        ""kind"": ""specific"",
        ""name"": ""Int32"",
        ""namespace"": ""System"",
        ""declaringType"": null,
        ""genericArguments"": [],
        ""assembly"": {
            ""name"": ""System.Private.CoreLib"",
            ""version"": ""4.0.0.0"",
            ""culture"": """",
            ""publicKeyToken"": ""7cec85d7bea7798e""
        }
    },
    ""genericArguments"": [],
    ""parameterTypes"": [
        {
            ""kind"": ""specific"",
            ""name"": ""String"",
            ""namespace"": ""System"",
            ""declaringType"": null,
            ""genericArguments"": [],
            ""assembly"": {
                ""name"": ""System.Private.CoreLib"",
                ""version"": ""4.0.0.0"",
                ""culture"": """",
                ""publicKeyToken"": ""7cec85d7bea7798e""
            }
        }
    ]
}"
            );

        [Fact]
        public void SerializeConstructedGenericMethod()
            => _Assert(
                typeof(string)
                    .GetMethod("Join", new[] { typeof(string), typeof(IEnumerable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)) })
                    .MakeGenericMethod(typeof(int)),
                @"{
    ""kind"": ""method"",
    ""name"": ""Join"",
    ""declaringType"": {
        ""kind"": ""specific"",
        ""name"": ""String"",
        ""namespace"": ""System"",
        ""declaringType"": null,
        ""genericArguments"": [],
        ""assembly"": {
            ""name"": ""System.Private.CoreLib"",
            ""version"": ""4.0.0.0"",
            ""culture"": """",
            ""publicKeyToken"": ""7cec85d7bea7798e""
        }
    },
    ""genericArguments"": [
        {
            ""kind"": ""specific"",
            ""name"": ""Int32"",
            ""namespace"": ""System"",
            ""declaringType"": null,
            ""genericArguments"": [],
            ""assembly"": {
                ""name"": ""System.Private.CoreLib"",
                ""version"": ""4.0.0.0"",
                ""culture"": """",
                ""publicKeyToken"": ""7cec85d7bea7798e""
            }
        }
    ],
    ""parameterTypes"": [
        {
            ""kind"": ""specific"",
            ""name"": ""String"",
            ""namespace"": ""System"",
            ""declaringType"": null,
            ""genericArguments"": [],
            ""assembly"": {
                ""name"": ""System.Private.CoreLib"",
                ""version"": ""4.0.0.0"",
                ""culture"": """",
                ""publicKeyToken"": ""7cec85d7bea7798e""
            }
        },
        {
            ""kind"": ""specific"",
            ""name"": ""IEnumerable"",
            ""namespace"": ""System.Collections.Generic"",
            ""declaringType"": null,
            ""genericArguments"": [
                {
                    ""kind"": ""specific"",
                    ""name"": ""Int32"",
                    ""namespace"": ""System"",
                    ""declaringType"": null,
                    ""genericArguments"": [],
                    ""assembly"": {
                        ""name"": ""System.Private.CoreLib"",
                        ""version"": ""4.0.0.0"",
                        ""culture"": """",
                        ""publicKeyToken"": ""7cec85d7bea7798e""
                    }
                }
            ],
            ""assembly"": {
                ""name"": ""System.Private.CoreLib"",
                ""version"": ""4.0.0.0"",
                ""culture"": """",
                ""publicKeyToken"": ""7cec85d7bea7798e""
            }
        }
    ]
}"
            );

        [Fact]
        public void SerializeGenericMethodDefinition()
            => _Assert(
                typeof(string).GetMethod("Join", new[] { typeof(string), typeof(IEnumerable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)) }),
                @"{
    ""kind"": ""method"",
    ""name"": ""Join"",
    ""declaringType"": {
        ""kind"": ""specific"",
        ""name"": ""String"",
        ""namespace"": ""System"",
        ""declaringType"": null,
        ""genericArguments"": [],
        ""assembly"": {
            ""name"": ""System.Private.CoreLib"",
            ""version"": ""4.0.0.0"",
            ""culture"": """",
            ""publicKeyToken"": ""7cec85d7bea7798e""
        }
    },
    ""genericArguments"": [
        {
            ""kind"": ""genericMethodParameter"",
            ""name"": ""T""
        }
    ],
    ""parameterTypes"": [
        {
            ""kind"": ""specific"",
            ""name"": ""String"",
            ""namespace"": ""System"",
            ""declaringType"": null,
            ""genericArguments"": [],
            ""assembly"": {
                ""name"": ""System.Private.CoreLib"",
                ""version"": ""4.0.0.0"",
                ""culture"": """",
                ""publicKeyToken"": ""7cec85d7bea7798e""
            }
        },
        {
            ""kind"": ""specific"",
            ""name"": ""IEnumerable"",
            ""namespace"": ""System.Collections.Generic"",
            ""declaringType"": null,
            ""genericArguments"": [
                {
                    ""kind"": ""genericMethodParameter"",
                    ""name"": ""T""
                }
            ],
            ""assembly"": {
                ""name"": ""System.Private.CoreLib"",
                ""version"": ""4.0.0.0"",
                ""culture"": """",
                ""publicKeyToken"": ""7cec85d7bea7798e""
            }
        }
    ]
}"
            );

        private static void _Assert(MemberInfo memberInfo, string expectedJson)
            => _Assert(memberReferenceFactory => memberReferenceFactory.Create(memberInfo), expectedJson);

        private static void _Assert(Func<MemberReferenceFactory, MemberReference> memberReferenceProvider, string expectedJson)
        {
            var expected = NormalizeJson(expectedJson);

            using (var stringWriter = new StringWriter())
            {
                using (var jsonWriter = new JsonTextWriter(stringWriter))
                {
                    var memberReferenceFactory = new MemberReferenceFactory();
                    var jsonWriterVisitor = new JsonWriterMemberReferenceVisitor(jsonWriter);

                    var memberReference = memberReferenceProvider(memberReferenceFactory);
                    memberReference.Accept(jsonWriterVisitor);

                }
                var actualJson = stringWriter.ToString();
                Assert.Equal(expected, NormalizeJson(actualJson));
            }

            string NormalizeJson(string json)
                => JsonConvert.SerializeObject(JsonConvert.DeserializeObject(json));
        }

        [Fact]
        public void CreatingJsonWriterVisitorWithNull_ThrowsExceptions()
        {
            var exception = Assert.Throws<ArgumentNullException>("jsonWriter", () => new JsonWriterMemberReferenceVisitor(null));
            Assert.Equal(new ArgumentNullException("jsonWriter").Message, exception.Message);
        }
    }
}