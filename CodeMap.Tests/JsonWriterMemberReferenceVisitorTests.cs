﻿using CodeMap.ReferenceData;
using CodeMap.Tests.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace CodeMap.Tests
{
    public class JsonWriterMemberReferenceVisitorTests
    {
        private const string _testDataPublicKeyToken = "";

        [Fact]
        public async Task SerializeSimpleType()
        {
            await _AssertAsync(
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
        }

        [Fact]
        public async Task SerializeVoidType()
        {
            await _AssertAsync(
                typeof(void),
                @"{
    ""kind"": ""specific/void""
}"
            );
        }

        [Fact]
        public async Task SerializeDynamicType()
        {
            await _AssertAsync(
                memberReferenceFactory => memberReferenceFactory.CreateDynamic(),
                @"{
    ""kind"": ""specific/dynamic""
}"
            );
        }

        [Fact]
        public async Task SerializeArrayType()
        {
            await _AssertAsync(
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
        }

        [Fact]
        public async Task SerializePointerType()
        {
            await _AssertAsync(
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
        }

        [Fact]
        public async Task SerializeByRefType()
        {
            await _AssertAsync(
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
        }

        [Fact]
        public async Task SerializeConstructedGenericType()
        {
            await _AssertAsync(
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
        }

        [Fact]
        public async Task SerializeGenericTypeDefinition()
        {
            await _AssertAsync(
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
        }

        [Fact]
        public async Task SerializeConstant()
        {
            await _AssertAsync(
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
        }

        [Fact]
        public async Task SerializeField()
        {
            await _AssertAsync(
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
        }

        [Fact]
        public async Task SerializeConstructor()
        {
            await _AssertAsync(
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
        }

        [Fact]
        public async Task SerializeEvent()
        {
            await _AssertAsync(
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
            ""version"": ""4.1.1.0"",
            ""culture"": """",
            ""publicKeyToken"": ""b03f5f7f11d50a3a""
        }
    }
}"
            );
        }

        [Fact]
        public async Task SerializeProperty()
        {
            await _AssertAsync(
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
        }

        [Fact]
        public async Task SerializeMethod()
        {
            await _AssertAsync(
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
        }

        [Fact]
        public async Task SerializeConstructedGenericMethod()
        {
            await _AssertAsync(
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
        }

        [Fact]
        public async Task SerializeGenericMethodDefinition()
        {
            await _AssertAsync(
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
        }

        private static Task _AssertAsync(MemberInfo memberInfo, string expectedJson)
            => _AssertAsync(memberReferenceFactory => memberReferenceFactory.Create(memberInfo), expectedJson);

        private static async Task _AssertAsync(Func<MemberReferenceFactory, MemberReference> memberReferenceProvider, string expectedJson)
        {
            var expected = NormalizeJson(expectedJson);
            await CompareJson(
                (memberReference, memberReferenceVisitor) =>
                {
                    memberReference.Accept(memberReferenceVisitor);
                    return Task.CompletedTask;
                }
            );

            await CompareJson(
                (memberReference, memberReferenceVisitor) => memberReference.AcceptAsync(memberReferenceVisitor)
            );

            async Task CompareJson(Func<MemberReference, MemberReferenceVisitor, Task> visitCallback)
            {
                string actualJson;
                using (var stringWriter = new StringWriter())
                {
                    using (var jsonWriter = new JsonTextWriter(stringWriter))
                    {
                        var memberReferenceFactory = new MemberReferenceFactory();
                        var jsonWriterVisitor = new JsonWriterMemberReferenceVisitor(jsonWriter);

                        var memberReference = memberReferenceProvider(memberReferenceFactory);
                        await visitCallback(memberReference, jsonWriterVisitor);

                    }
                    actualJson = stringWriter.ToString();
                }

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