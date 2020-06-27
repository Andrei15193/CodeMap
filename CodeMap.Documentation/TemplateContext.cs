using CodeMap.DocumentationElements;

namespace CodeMap.Documentation
{
    public class TemplateContext
    {
        public TemplateContext(PageContext pageContext, DocumentationElement documentationElement)
            => (PageContext, DocumentationElement) = (pageContext, documentationElement);

        public PageContext PageContext { get; }

        public DocumentationElement DocumentationElement { get; }
    }

    public class TemplateContext<TDocumentationElement> : TemplateContext
        where TDocumentationElement : DocumentationElement
    {
        public TemplateContext(PageContext pageContext, TDocumentationElement documentationElement)
            : base(pageContext, documentationElement)
        {
        }

        public new TDocumentationElement DocumentationElement
            => (TDocumentationElement)base.DocumentationElement;
    }
}