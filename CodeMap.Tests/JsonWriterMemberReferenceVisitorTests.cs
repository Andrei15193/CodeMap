using CodeMap.ReferenceData;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace CodeMap.Tests
{
    public class JsonWriterMemberReferenceVisitorTests
    {
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