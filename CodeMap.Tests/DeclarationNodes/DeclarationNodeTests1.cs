using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeMap.DeclarationNodes;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes
{
    public abstract class DeclarationNodeTests<TDeclarationNode>
        where TDeclarationNode : DeclarationNode
    {
        private readonly Lazy<Assembly> _testDataAssembly;
        private readonly Lazy<AssemblyDeclaration> _testDataAssemblyDeclaration;
        private readonly Lazy<TDeclarationNode> _declarationNode;

        protected DeclarationNodeTests()
        {
            _testDataAssembly = new Lazy<Assembly>(
                () => Assert.Single(
                    typeof(DeclarationNodeTests)
                        .Assembly
                        .GetReferencedAssemblies()
                        .Where(dependency => dependency.Name == "CodeMap.Tests.Data")
                        .Select(Assembly.Load)
                )
            );
            _testDataAssemblyDeclaration = new Lazy<AssemblyDeclaration>(() => CodeMap.DeclarationNodes.DeclarationNode.Create(TestDataAssembly));
            _declarationNode = new Lazy<TDeclarationNode>(() => Assert.Single(_GetDeclarationNodes().OfType<TDeclarationNode>(), DeclarationNodePredicate));
        }

        private IEnumerable<DeclarationNode> _GetDeclarationNodes()
        {
            return _GetAssemblyDeclarations()
                .AsEnumerable<DeclarationNode>()
                .Concat(_GetNamespaceDeclarations());

            IEnumerable<AssemblyDeclaration> _GetAssemblyDeclarations()
                => Enumerable.Repeat(TestDataAssemblyDeclaration, 1);

            IEnumerable<NamespaceDeclaration> _GetNamespaceDeclarations()
                => _GetAssemblyDeclarations().SelectMany(assemblyDeclaration => assemblyDeclaration.Namespaces);
        }

        protected Assembly TestDataAssembly
            => _testDataAssembly.Value;

        protected AssemblyDeclaration TestDataAssemblyDeclaration
            => _testDataAssemblyDeclaration.Value;

        protected TDeclarationNode DeclarationNode
            => _declarationNode.Value;

        protected abstract bool DeclarationNodePredicate(TDeclarationNode declarationNode);

        protected static void AssertAttribute<TAttribute>(IEnumerable<AttributeData> attributes, IEnumerable<(string Name, object Value, Type Type)> positionalParameters, IEnumerable<(string Name, object Value, Type Type)> namedParameters)
            where TAttribute : Attribute
        {
            var attributeData = Assert.Single(attributes, attribute => attribute.Type == typeof(TAttribute));

            var expectedPositionalParameters = positionalParameters.ToList();
            var actualPositionalParameters = attributeData.PositionalParameters.Select(parameter => (parameter.Name, parameter.Value, parameter.Type)).ToList();

            Assert.Equal(expectedPositionalParameters.Count, actualPositionalParameters.Count);
            foreach (var (expectedParameter, actualParameter) in expectedPositionalParameters.Zip(actualPositionalParameters, (expectedParameter, actualParameter) => (expectedParameter, actualParameter)))
            {
                Assert.Equal(expectedParameter.Name, actualParameter.Name);
                Assert.Equal(expectedParameter.Value, actualParameter.Value);
                Assert.True(expectedParameter.Type == actualParameter.Type, $"Expected {expectedParameter.Type}.");
            }

            var expectedNamedParameters = namedParameters.OrderBy(parameter => parameter.Name, StringComparer.Ordinal).ToList();
            var actualNamedParameters = attributeData.NamedParameters.OrderBy(parameter => parameter.Name, StringComparer.Ordinal).Select(parameter => (parameter.Name, parameter.Value, parameter.Type)).ToList();

            Assert.Equal(expectedNamedParameters.Count, actualNamedParameters.Count);
            foreach (var (expectedParameter, actualParameter) in expectedNamedParameters.Zip(actualNamedParameters, (expectedParameter, actualParameter) => (expectedParameter, actualParameter)))
            {
                Assert.Equal(expectedParameter.Name, actualParameter.Name);
                Assert.Equal(expectedParameter.Value, actualParameter.Value);
                Assert.True(expectedParameter.Type == actualParameter.Type, $"Expected {expectedParameter.Type}.");
            }
        }
    }
}