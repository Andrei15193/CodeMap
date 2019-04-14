using CodeMap.Elements;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace CodeMap.Tests
{
    public class WriterDocumentationVisitorTests
    {
#if DEBUG
        private const string _jsonTestDataFileName = "testData.debug";
#else
        private const string _jsonTestDataFileName = "testData.release";
#endif

        private readonly AssemblyDocumentationElement _testDataDocumentation;

        public WriterDocumentationVisitorTests()
        {
            var testDataAssembly = typeof(WriterDocumentationVisitorTests)
                .Assembly
                .GetReferencedAssemblies()
                .Where(dependency => dependency.Name == "CodeMap.Tests.Data")
                .Select(Assembly.Load)
                .Single();
            _testDataDocumentation = DocumentationElement.Create(testDataAssembly);
        }

        [Fact]
        public async Task TestJsonWriter()
        {
            var expectedJson = _NormalizeJson(await _ReadTestDataFileAsync(_jsonTestDataFileName));

            object actualJsonSync, actualJsonAsync;
            using (var stringWriter = new StringWriter())
            {
                using (var jsonWriterVisitor = new JsonWriterDocumentationVisitor(new JsonTextWriter(stringWriter)))
                    _testDataDocumentation.Accept(jsonWriterVisitor);
                actualJsonSync = _NormalizeJson(stringWriter.ToString());
            }

            using (var stringWriter = new StringWriter())
            {
                using (var jsonWriterVisitor = new JsonWriterDocumentationVisitor(new JsonTextWriter(stringWriter)))
                    await _testDataDocumentation.AcceptAsync(jsonWriterVisitor);
                actualJsonAsync = _NormalizeJson(stringWriter.ToString());
            }

            Assert.True(expectedJson.Equals(actualJsonSync), "Actual (sync) JSON is different from expected.");
            Assert.True(expectedJson.Equals(actualJsonAsync), "Actual (async) JSON is different from expected.");
        }

        private static async Task<string> _ReadTestDataFileAsync(string testDataFileName)
        {
            using (var testDataFile = typeof(WriterDocumentationVisitorTests).Assembly.GetManifestResourceStream(typeof(WriterDocumentationVisitorTests), $"{testDataFileName}.json"))
            using (var testDataFileReader = new StreamReader(testDataFile))
                return await testDataFileReader.ReadToEndAsync();
        }

        private static string _NormalizeJson(string json)
            => JsonConvert.SerializeObject(JsonConvert.DeserializeObject(json));
    }
}