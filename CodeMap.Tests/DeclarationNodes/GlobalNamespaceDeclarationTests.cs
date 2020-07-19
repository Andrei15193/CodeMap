using System;
using System.Linq;
using CodeMap.DeclarationNodes;
using CodeMap.DocumentationElements;
using CodeMap.Tests.DeclarationNodes.Mocks;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes
{
    public class GlobalNamespaceDeclarationTests : DeclarationNodeTests<GlobalNamespaceDeclaration>
    {
        protected override bool DeclarationNodePredicate(GlobalNamespaceDeclaration declarationNode)
            => true;

        [Fact]
        public void HasNameSet()
            => Assert.Empty(DeclarationNode.Name);

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
            => Assert.Empty(DeclarationNode.Enums);

        [Fact]
        public void HasDelegatesSet()
            => Assert.Empty(DeclarationNode.Delegates);

        [Fact]
        public void HasInterfacesSet()
            => Assert.Empty(DeclarationNode.Interfaces);

        [Fact]
        public void HasClassesSet()
            => Assert.Single(DeclarationNode.Classes);

        [Fact]
        public void HasGlobalTestClass()
            => Assert.Single(DeclarationNode.Classes, @class => @class == typeof(GlobalTestClass));

        [Fact]
        public void HasStructsSet()
            => Assert.Empty(DeclarationNode.Structs);

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
            var invocationCount = 0;
            var visitor = new DeclarationNodeVisitorMock
            {
                VisitCallback = namespaceDeclaration =>
                {
                    Assert.Same(DeclarationNode, namespaceDeclaration);
                    invocationCount++;
                }
            };

            DeclarationNode.Accept(visitor);

            Assert.Equal(1, invocationCount);
        }

        private sealed class DeclarationNodeVisitorMock : DeclarationNodeVisitor
        {
            public Action<NamespaceDeclaration> VisitCallback { get; set; }

            protected override void VisitNamespace(NamespaceDeclaration @namespace)
                => VisitCallback(@namespace);

            protected override void VisitAssembly(AssemblyDeclaration assembly)
                => throw new NotImplementedException();

            protected override void VisitClass(ClassDeclaration @class)
                => throw new NotImplementedException();

            protected override void VisitConstant(ConstantDeclaration constant)
                => throw new NotImplementedException();

            protected override void VisitConstructor(ConstructorDeclaration constructor)
                => throw new NotImplementedException();

            protected override void VisitDelegate(DelegateDeclaration @delegate)
                => throw new NotImplementedException();

            protected override void VisitEnum(EnumDeclaration @enum)
                => throw new NotImplementedException();

            protected override void VisitEvent(EventDeclaration @event)
                => throw new NotImplementedException();

            protected override void VisitField(FieldDeclaration field)
                => throw new NotImplementedException();

            protected override void VisitInterface(InterfaceDeclaration @interface)
                => throw new NotImplementedException();

            protected override void VisitMethod(MethodDeclaration method)
                => throw new NotImplementedException();

            protected override void VisitProperty(PropertyDeclaration property)
                => throw new NotImplementedException();

            protected override void VisitStruct(StructDeclaration @struct)
                => throw new NotImplementedException();
        }
    }
}