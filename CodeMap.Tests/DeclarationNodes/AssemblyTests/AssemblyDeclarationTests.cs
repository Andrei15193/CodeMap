using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using CodeMap.DeclarationNodes;
using CodeMap.DocumentationElements;
using CodeMap.Tests.DeclarationNodes.Mocks;
using Xunit;
using static System.Diagnostics.DebuggableAttribute;

namespace CodeMap.Tests.DeclarationNodes.AssemblyTests
{
    public class AssemblyDeclarationTests : DeclarationNodeTests<AssemblyDeclaration>
    {
        protected override bool DeclarationNodePredicate(AssemblyDeclaration assemblyDeclaration)
            => true;

        [Fact]
        public void AssemblyEqualityComparison()
        {
            Assert.True(DeclarationNode.Equals(TestDataAssembly));
            Assert.True(DeclarationNode.Equals(TestDataAssembly as object));
            Assert.True(TestDataAssembly == DeclarationNode);
            Assert.True(DeclarationNode == TestDataAssembly);

            var systemAssembly = typeof(object).Assembly;
            Assert.False(DeclarationNode.Equals(systemAssembly));
            Assert.False(DeclarationNode.Equals(systemAssembly as object));
            Assert.True(systemAssembly != DeclarationNode);
            Assert.True(DeclarationNode != systemAssembly);
        }

        [Fact]
        public void AssemblyNameEqualityComparison()
        {
            var testDataAssemblyName = TestDataAssembly.GetName();
            Assert.True(DeclarationNode.Equals(testDataAssemblyName));
            Assert.True(DeclarationNode.Equals(testDataAssemblyName as object));
            Assert.True(testDataAssemblyName == DeclarationNode);
            Assert.True(DeclarationNode == testDataAssemblyName);

            var systemAssemblyName = typeof(object).Assembly.GetName();
            Assert.False(DeclarationNode.Equals(systemAssemblyName));
            Assert.False(DeclarationNode.Equals(systemAssemblyName as object));
            Assert.True(systemAssemblyName != DeclarationNode);
            Assert.True(DeclarationNode != systemAssemblyName);
        }

        [Fact]
        public void HasNameSet()
            => Assert.Equal("CodeMap.Tests.Data", DeclarationNode.Name);

        [Fact]
        public void HasCultureSetToEmptyString()
            => Assert.Empty(DeclarationNode.Culture);

#if DEBUG
        [Fact]
        public void HasPublicKeyTokenSetToEmptyString()
            => Assert.Empty(DeclarationNode.PublicKeyToken);
#else
        [Fact]
        public void HasPublicKeyTokenSet()
            => Assert.Equal("4919ac5af74d53e8", DeclarationNode.PublicKeyToken);
#endif

        [Fact]
        public void HasVersionSet()
            => Assert.Equal(new Version(1, 2, 3, 4), DeclarationNode.Version);

        [Fact]
        public void HasAttributesSet()
            => Assert.Equal(4, DeclarationNode.Attributes.Count);

#if DEBUG
        [Fact]
        public void HasDebuggableAttribute()
            => AssertAttribute<DebuggableAttribute>(
                DeclarationNode.Attributes,
                new (string, object, Type)[] { ("modes", DebuggingModes.Default | DebuggingModes.IgnoreSymbolStoreSequencePoints | DebuggingModes.EnableEditAndContinue | DebuggingModes.DisableOptimizations, typeof(DebuggingModes)) },
                Enumerable.Empty<(string, object, Type)>()
            );
#else
        [Fact]
        public void HasDebuggableAttribute()
            => AssertAttribute<DebuggableAttribute>(
                DeclarationNode.Attributes,
                new (string, object, Type)[] { ("modes", DebuggingModes.IgnoreSymbolStoreSequencePoints, typeof(DebuggingModes)) },
                Enumerable.Empty<(string, object, Type)>()
            );
#endif

        [Fact]
        public void HasCompilationRelaxationsAttribute()
            => AssertAttribute<CompilationRelaxationsAttribute>(
                DeclarationNode.Attributes,
                new (string, object, Type)[] { ("relaxations", 8, typeof(int)) },
                Enumerable.Empty<(string, object, Type)>()
            );
        [Fact]
        public void HasRuntimeCompatibilityAttribute()
            => AssertAttribute<RuntimeCompatibilityAttribute>(
                DeclarationNode.Attributes,
                Enumerable.Empty<(string, object, Type)>(),
                new (string, object, Type)[] { ("WrapNonExceptionThrows", true, typeof(bool)) }
            );

        [Fact]
        public void HasTargetFrameworkAttribute()
            => AssertAttribute<TargetFrameworkAttribute>(
                DeclarationNode.Attributes,
                new (string, object, Type)[] { ("frameworkName", ".NETCoreApp,Version=v3.1", typeof(string)) },
                new (string, object, Type)[] { ("FrameworkDisplayName", "", typeof(string)) }
            );

        [Fact]
        public void HasNamespacesSet()
            => Assert.Equal(2, DeclarationNode.Namespaces.Count);

#if DEBUG
        [Fact]
        public void HasDependenciesSet()
            => Assert.Equal(5, DeclarationNode.Dependencies.Count);
#else
        [Fact]
        public void HasDependenciesSet()
            => Assert.Equal(4, DeclarationNode.Dependencies.Count);
#endif

#if DEBUG
        [Fact]
        public void HasSystemDiagnosticsDependencySet()
            => Assert.Single(DeclarationNode.Dependencies, dependency =>
                dependency.Name == "System.Diagnostics.Debug"
                && dependency.Version == new Version(4, 1, 2, 0)
                && dependency.Culture == string.Empty
                && dependency.PublicKeyToken == "b03f5f7f11d50a3a");
#endif

        [Fact]
        public void HasSystemLinqExpressionsDependencySet()
            => Assert.Single(DeclarationNode.Dependencies, dependency =>
                dependency.Name == "System.Linq.Expressions"
                && dependency.Version == new Version(4, 2, 2, 0)
                && dependency.Culture == string.Empty
                && dependency.PublicKeyToken == "b03f5f7f11d50a3a");

        [Fact]
        public void HasSystemRuntimeDependencySet()
            => Assert.Single(DeclarationNode.Dependencies, dependency =>
                dependency.Name == "System.Runtime"
                && dependency.Version == new Version(4, 2, 2, 0)
                && dependency.Culture == string.Empty
                && dependency.PublicKeyToken == "b03f5f7f11d50a3a");

        [Fact]
        public void HasSystemRuntimeExtensionsSet()
            => Assert.Single(DeclarationNode.Dependencies, dependency =>
                dependency.Name == "System.Runtime.Extensions"
                && dependency.Version == new Version(4, 2, 2, 0)
                && dependency.Culture == string.Empty
                && dependency.PublicKeyToken == "b03f5f7f11d50a3a");

        [Fact]
        public void HasSystemThreadingSet()
            => Assert.Single(DeclarationNode.Dependencies, dependency =>
                dependency.Name == "System.Threading"
                && dependency.Version == new Version(4, 1, 2, 0)
                && dependency.Culture == string.Empty
                && dependency.PublicKeyToken == "b03f5f7f11d50a3a");

        [Fact]
        public void HasEmptySummary()
            => Assert.Empty(DeclarationNode.Summary.Content);

        [Fact]
        public void HasEmptyRemarks()
            => Assert.Empty(DeclarationNode.Remarks.Content);

        [Fact]
        public void HasEmptyExamples()
            => Assert.Empty(DeclarationNode.Examples);

        [Fact]
        public void HasEmptyRelatedMembers()
            => Assert.Empty(DeclarationNode.RelatedMembers);

        [Fact]
        public void ApplyNullAdditionsThrowsException()
            => Assert.Throws<ArgumentNullException>("additions", () => DeclarationNode.Apply(null));

        [Fact]
        public void ApplySummaryDocumentationAddition()
        {
            var summary = DocumentationElement.Summary();

            DeclarationNode.Apply(
                new AssemblyDocumentationAdditionMock { Skip = true, Summary = DocumentationElement.Summary() },
                new AssemblyDocumentationAdditionMock { Summary = summary }
            );

            Assert.Same(summary, DeclarationNode.Summary);
            Assert.Empty(DeclarationNode.Remarks.Content);
            Assert.Empty(DeclarationNode.Examples);
            Assert.Empty(DeclarationNode.RelatedMembers);
        }

        [Fact]
        public void ApplyRemarksDocumentationAddition()
        {
            var remarks = DocumentationElement.Remarks();

            DeclarationNode.Apply(
                new AssemblyDocumentationAdditionMock { Skip = true, Remarks = DocumentationElement.Remarks() },
                new AssemblyDocumentationAdditionMock { Remarks = remarks }
            );

            Assert.Same(remarks, DeclarationNode.Remarks);
            Assert.Empty(DeclarationNode.Summary.Content);
            Assert.Empty(DeclarationNode.Examples);
            Assert.Empty(DeclarationNode.RelatedMembers);
        }

        [Fact]
        public void ApplyExamplesDocumentationAddition()
        {
            var examples = new[] { DocumentationElement.Example() };

            DeclarationNode.Apply(
                new AssemblyDocumentationAdditionMock { Skip = true, Examples = new[] { DocumentationElement.Example() } },
                new AssemblyDocumentationAdditionMock { Examples = examples }
            );

            Assert.Same(examples, DeclarationNode.Examples);
            Assert.Empty(DeclarationNode.Summary.Content);
            Assert.Empty(DeclarationNode.Remarks.Content);
            Assert.Empty(DeclarationNode.RelatedMembers);
        }

        [Fact]
        public void ApplyRelatedMembersDocumenationAddition()
        {
            var relatedMembers = new[] { DocumentationElement.MemberReference(typeof(object)) };

            DeclarationNode.Apply(
                new AssemblyDocumentationAdditionMock { Skip = true, RelatedMembers = new[] { DocumentationElement.MemberReference(typeof(object)) } },
                new AssemblyDocumentationAdditionMock { RelatedMembers = relatedMembers }
            );

            Assert.Same(relatedMembers, DeclarationNode.RelatedMembers);
            Assert.Empty(DeclarationNode.Summary.Content);
            Assert.Empty(DeclarationNode.Remarks.Content);
            Assert.Empty(DeclarationNode.Examples);
        }

        [Fact]
        public void ApplyNamespaceDocumentationAddition()
        {
            var summary = DocumentationElement.Summary();

            DeclarationNode.Apply(
                new AssemblyDocumentationAdditionMock
                {
                    Skip = true,
                    NamespaceAdditions = new[]
                    {
                        new NamespaceDocumentationAdditionMock
                        {
                            Summary = DocumentationElement.Summary()
                        }
                    }
                },
                new AssemblyDocumentationAdditionMock
                {
                    NamespaceAdditions = new[]
                    {
                        new NamespaceDocumentationAdditionMock
                        {
                            Skip = true,
                            Summary = DocumentationElement.Summary()
                        },
                        new NamespaceDocumentationAdditionMock
                        {
                            Summary = summary
                        }
                    }
                }
            );

            foreach (var @namespace in DeclarationNode.Namespaces)
                Assert.Same(summary, @namespace.Summary);
        }

        [Fact]
        public void AcceptVisitor()
        {
            var visitor = new DeclarationNodeVisitorMock(DeclarationNode);

            DeclarationNode.Accept(visitor);

            Assert.Equal(1, visitor.VisitCount);
        }
    }
}