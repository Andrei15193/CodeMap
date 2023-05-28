using CodeMap.DeclarationNodes;
using CodeMap.Html;
using Xunit;

namespace CodeMap.Tests.Html
{
    public class AssemblyDeclarationExtensionsTests
    {
        [Fact]
        public void GettingInformalVersionRetrievesValue()
        {
            var assemblyDeclaration = DeclarationNode.Create(typeof(GlobalTestClass).Assembly);

            var informalVersion = assemblyDeclaration.GetInformalVersion();

            Assert.Equal("test-data", informalVersion);
        }
    }
}