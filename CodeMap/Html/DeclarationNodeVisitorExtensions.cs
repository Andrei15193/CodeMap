using CodeMap.DeclarationNodes;

namespace CodeMap.Html
{
    /// <summary/>
    public static class DeclarationNodeVisitorExtensions
    {
        /// <summary/>
        public static string GetSimpleNameReference(this DeclarationNode declarationNode)
        {
            var declarationNodeVisitor = new SimpleNameDeclarationNodeVisitor();
            declarationNode.Accept(declarationNodeVisitor);
            return declarationNodeVisitor.StringBuilder.ToString();
        }

        /// <summary/>
        public static string GetFullNameReference(this DeclarationNode declarationNode)
        {
            var declarationNodeVisitor = new FullNameDeclarationNodeVisitor();
            declarationNode.Accept(declarationNodeVisitor);
            return declarationNodeVisitor.StringBuilder.ToString();
        }
    }
}