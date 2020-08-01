using CodeMap.DeclarationNodes;
using CodeMap.DocumentationElements;
using CodeMap.Tests.Data;
using CodeMap.Tests.DeclarationNodes.Mocks;
using System;
using System.Linq;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes
{
    public class CodeMapTestsDataNamespaceDeclarationTests : DeclarationNodeTests<NamespaceDeclaration>
    {
        protected override bool DeclarationNodePredicate(NamespaceDeclaration namespaceDeclaration)
            => !(namespaceDeclaration is GlobalNamespaceDeclaration);

        [Fact]
        public void HasNameSet()
            => Assert.Equal("CodeMap.Tests.Data", DeclarationNode.Name);

        [Fact]
        public void HasAssemblySet()
            => Assert.Same(TestDataAssemblyDeclaration, DeclarationNode.Assembly);

        [Fact]
        public void HasDeclaredMembersSet()
            => Assert.Equal(
                DeclarationNode.Enums.AsEnumerable<TypeDeclaration>().Concat(DeclarationNode.Delegates).Concat(DeclarationNode.Interfaces).Concat(DeclarationNode.Classes).Concat(DeclarationNode.Structs),
                DeclarationNode.DeclaredTypes
            );

        [Fact]
        public void HasEnumsSet()
            => Assert.Single(DeclarationNode.Enums);

        [Fact]
        public void HasTestEnum()
            => Assert.Single(DeclarationNode.Enums, @enum => @enum == typeof(TestEnum));

        [Fact]
        public void HasDelegatesSet()
            => Assert.Single(DeclarationNode.Delegates);

        [Fact]
        public void HasTestDelegate()
            => Assert.Single(DeclarationNode.Delegates, @delegate => @delegate == typeof(TestDelegate<>));

        [Fact]
        public void HasInterfacesSet()
            => Assert.Equal(5, DeclarationNode.Interfaces.Count);

        [Fact]
        public void HasITestBaseInterface()
            => Assert.Single(DeclarationNode.Interfaces, @interface => @interface == typeof(ITestBaseInterface));

        [Fact]
        public void HasITestExplicitInterface()
            => Assert.Single(DeclarationNode.Interfaces, @interface => @interface == typeof(ITestExplicitInterface));

        [Fact]
        public void HasITestExtendedBaseInterface()
            => Assert.Single(DeclarationNode.Interfaces, @interface => @interface == typeof(ITestExtendedBaseInterface));

        [Fact]
        public void HasITestGenericParameterInterface()
            => Assert.Single(DeclarationNode.Interfaces, @interface => @interface == typeof(ITestGenericParameter<,,,,,>));

        [Fact]
        public void HasITestInterface()
            => Assert.Single(DeclarationNode.Interfaces, @interface => @interface == typeof(ITestInterface<>));

        [Fact]
        public void HasClassesSet()
            => Assert.Equal(8, DeclarationNode.Classes.Count);

        [Fact]
        public void HasTestAbstractClass()
            => Assert.Single(DeclarationNode.Classes, @class => @class == typeof(TestAbstractClass));

        [Fact]
        public void HasTestAttributeClass()
            => Assert.Single(DeclarationNode.Classes, @class => @class == typeof(TestAttribute));

        [Fact]
        public void HasTestBaseClass()
            => Assert.Single(DeclarationNode.Classes, @class => @class == typeof(TestBaseClass));

        [Fact]
        public void HasTestClass()
            => Assert.Single(DeclarationNode.Classes, @class => @class == typeof(TestClass<>));

        [Fact]
        public void HasTestDocumentation()
            => Assert.Single(DeclarationNode.Classes, @class => @class == typeof(TestDocumentation));

        [Fact]
        public void HasTestExplicitClass()
            => Assert.Single(DeclarationNode.Classes, @class => @class == typeof(TestExplicitClass));

        [Fact]
        public void HasTestSealedClass()
            => Assert.Single(DeclarationNode.Classes, @class => @class == typeof(TestSealedClass));

        [Fact]
        public void HasTestStaticClass()
            => Assert.Single(DeclarationNode.Classes, @class => @class == typeof(TestStaticClass));

        [Fact]
        public void HasStructsSet()
            => Assert.Single(DeclarationNode.Structs);

        [Fact]
        public void HasTestStruct()
            => Assert.Single(DeclarationNode.Structs, @struct => @struct == typeof(TestStruct<>));

        [Fact]
        public void HasCircularReferenceSet()
            => Assert.Single(DeclarationNode.Assembly.Namespaces, @namespace => ReferenceEquals(@namespace, DeclarationNode));

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
                new NamespaceDocumentationAdditionMock { Skip = true, Summary = DocumentationElement.Summary() },
                new NamespaceDocumentationAdditionMock { Summary = summary }
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
                new NamespaceDocumentationAdditionMock { Skip = true, Remarks = DocumentationElement.Remarks() },
                new NamespaceDocumentationAdditionMock { Remarks = remarks }
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
                new NamespaceDocumentationAdditionMock { Skip = true, Examples = new[] { DocumentationElement.Example() } },
                new NamespaceDocumentationAdditionMock { Examples = examples }
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
                new NamespaceDocumentationAdditionMock { Skip = true, RelatedMembers = new[] { DocumentationElement.MemberReference(typeof(object)) } },
                new NamespaceDocumentationAdditionMock { RelatedMembers = relatedMembers }
            );

            Assert.Same(relatedMembers, DeclarationNode.RelatedMembers);
            Assert.Empty(DeclarationNode.Summary.Content);
            Assert.Empty(DeclarationNode.Remarks.Content);
            Assert.Empty(DeclarationNode.Examples);
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