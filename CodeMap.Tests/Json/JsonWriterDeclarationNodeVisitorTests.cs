using CodeMap.DeclarationNodes;
using CodeMap.Json;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace CodeMap.Tests.Json
{
    public class JsonWriterDeclarationNodeVisitorTests
    {
#if DEBUG
        private const string _jsonTestDataFileName = "testData.debug";
#else
        private const string _jsonTestDataFileName = "testData.release";
#endif

        private readonly AssemblyDeclaration _testDataDocumentation;

        public JsonWriterDeclarationNodeVisitorTests()
        {
            var testDataAssembly = typeof(JsonWriterDeclarationNodeVisitorTests)
                .Assembly
                .GetReferencedAssemblies()
                .Where(dependency => dependency.Name == "CodeMap.Tests.Data")
                .Select(Assembly.Load)
                .Single();
            _testDataDocumentation = DeclarationNode.Create(testDataAssembly);
        }

        [Fact]
        public void TestJsonWriter()
        {
            var expectedJson = _NormalizeJson(_ReadTestDataFile(_jsonTestDataFileName));

            using (var stringWriter = new StringWriter())
            {
                using (var jsonWriter = new JsonTextWriter(stringWriter))
                {
                    var jsonWriterVisitor = new JsonWriterDeclarationNodeVisitor(jsonWriter);
                    _testDataDocumentation.Accept(jsonWriterVisitor);
                }
                var actualJson = _NormalizeJson(stringWriter.ToString());
                Assert.True(expectedJson.Equals(actualJson), "Actual JSON is different from expected.");
            }

        }

        private static string _ReadTestDataFile(string testDataFileName)
        {
            using (var testDataFile = typeof(JsonWriterDeclarationNodeVisitorTests).Assembly.GetManifestResourceStream(typeof(JsonWriterDeclarationNodeVisitorTests), $"{testDataFileName}.json"))
            using (var testDataFileReader = new StreamReader(testDataFile))
                return testDataFileReader.ReadToEnd();
        }

        private static string _NormalizeJson(string json)
            => JsonConvert.SerializeObject(JsonConvert.DeserializeObject(json));
    }
}