namespace CodeMap.Elements
{
    /// <summary>
    /// Represents inline documentation elements that form a <see cref="BlockDocumentationElement"/>.
    /// These include plain text, inline code, parameter references, generic parameter references and member references.
    /// </summary>
    public abstract class InlineDocumentationElement : DocumentationElement
    {
        internal InlineDocumentationElement()
        {
        }
    }
}