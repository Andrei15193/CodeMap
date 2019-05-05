﻿using CodeMap.ReferenceData;
using CodeMap.Tests.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
                ""kind"": ""genericParameter"",
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
            ""kind"": ""genericParameter"",
            ""name"": ""TParam2""
        },
        {
            ""kind"": ""genericParameter"",
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

        private static async Task _AssertAsync(MemberInfo memberInfo, string expectedJson)
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

                        var memberReference = memberReferenceFactory.Create(memberInfo);
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