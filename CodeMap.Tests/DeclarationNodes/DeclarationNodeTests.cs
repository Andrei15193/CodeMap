using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeMap.DeclarationNodes;
using CodeMap.Tests.Data;
using Xunit;

namespace CodeMap.Tests.DeclarationNodes
{
    public abstract class DeclarationNodeTests<TDeclarationNode>
        where TDeclarationNode : DeclarationNode
    {
        private readonly Lazy<AssemblyDeclaration> _testDataAssemblyDeclaration;
        private readonly Lazy<TDeclarationNode> _declarationNode;

        protected DeclarationNodeTests()
        {
            _testDataAssemblyDeclaration = new Lazy<AssemblyDeclaration>(() => CodeMap.DeclarationNodes.DeclarationNode.Create(TestDataAssembly));
            _declarationNode = new Lazy<TDeclarationNode>(() => Assert.Single(_GetDeclarationNodes().OfType<TDeclarationNode>(), DeclarationNodePredicate));
        }

        private IEnumerable<DeclarationNode> _GetDeclarationNodes()
        {
            return _GetAssemblyDeclarations()
                .AsEnumerable<DeclarationNode>()
                .Concat(_GetNamespaceDeclarations())
                .Concat(_GetTypeDeclarations())
                .Concat(_GetEnumMemberDeclarations())
                .Concat(_GetInterfaceMemberDeclarations());

            IEnumerable<AssemblyDeclaration> _GetAssemblyDeclarations()
                => Enumerable.Repeat(TestDataAssemblyDeclaration, 1);

            IEnumerable<NamespaceDeclaration> _GetNamespaceDeclarations()
                => _GetAssemblyDeclarations().SelectMany(assemblyDeclaration => assemblyDeclaration.Namespaces);

            IEnumerable<TypeDeclaration> _GetTypeDeclarations()
                => _GetNamespaceDeclarations().SelectMany(namespaceDeclaration => namespaceDeclaration.DeclaredTypes);

            IEnumerable<MemberDeclaration> _GetEnumMemberDeclarations()
                => _GetNamespaceDeclarations().SelectMany(namespaceDeclaration => namespaceDeclaration.Enums.SelectMany(enumDeclaration => enumDeclaration.Members));

            IEnumerable<MemberDeclaration> _GetInterfaceMemberDeclarations()
                => _GetNamespaceDeclarations().SelectMany(namespaceDeclaration => namespaceDeclaration.Interfaces.SelectMany(interfaceDeclaration => interfaceDeclaration.Members));
        }

        protected Assembly TestDataAssembly
            => typeof(TestClass<>).Assembly;

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

        protected sealed class DeclarationNodeVisitorMock : DeclarationNodeVisitor
        {
            private readonly TDeclarationNode _expectedDeclarationNode;

            public DeclarationNodeVisitorMock(TDeclarationNode declarationNode)
                => _expectedDeclarationNode = declarationNode;

            public int VisitCount { get; private set; }

            protected override void VisitAssembly(AssemblyDeclaration assembly)
                => _InvokeCallback(assembly);

            protected override void VisitNamespace(NamespaceDeclaration @namespace)
                => _InvokeCallback(@namespace);

            protected override void VisitEnum(EnumDeclaration @enum)
                => _InvokeCallback(@enum);

            protected override void VisitDelegate(DelegateDeclaration @delegate)
                => _InvokeCallback(@delegate);

            protected override void VisitInterface(InterfaceDeclaration @interface)
                => _InvokeCallback(@interface);

            protected override void VisitClass(ClassDeclaration @class)
                => _InvokeCallback(@class);

            protected override void VisitStruct(StructDeclaration @struct)
                => _InvokeCallback(@struct);

            protected override void VisitConstant(ConstantDeclaration constant)
                => _InvokeCallback(constant);

            protected override void VisitField(FieldDeclaration field)
                => _InvokeCallback(field);

            protected override void VisitConstructor(ConstructorDeclaration constructor)
                => _InvokeCallback(constructor);

            protected override void VisitEvent(EventDeclaration @event)
                => _InvokeCallback(@event);

            protected override void VisitProperty(PropertyDeclaration property)
                => _InvokeCallback(property);

            protected override void VisitMethod(MethodDeclaration method)
                => _InvokeCallback(method);

            private void _InvokeCallback<TVisitedDeclarationNode>(TVisitedDeclarationNode actualDeclarationNode)
                where TVisitedDeclarationNode : DeclarationNode
            {
                if (!typeof(TVisitedDeclarationNode).IsAssignableFrom(typeof(TDeclarationNode)))
                    throw new NotImplementedException();

                VisitCount++;
                Assert.Same(_expectedDeclarationNode, actualDeclarationNode);
            }
        }
    }
}