using System;
using System.Linq;
using CodeMap.DocumentationElements;
using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.PathStructure;

namespace CodeMap.Handlebars.Helpers
{
    /// <summary>A helper used to get the partial name for a <see cref="DocumentationElement"/>.</summary>
    /// <example>
    /// The following template will generate a paragraph containing the partial name for a <see cref="DocumentationElement"/>.
    /// <code language="html">
    /// &lt;p&gt;{{GetDocumentationPartialName}}&lt;/p&gt;
    /// </code>
    /// If the current context is an <see cref="SummaryDocumentationElement"/>, the output would be equal to <see cref="DocumentationPartialNames.Summary"/> (<c>Summary</c>):
    /// <code language="html">
    /// &lt;p&gt;Summary&lt;/p&gt;
    /// </code>
    /// While it may not be useful to display this information like this, it is useful to use this helper as a selector for a specific partial.
    /// For instance, the <c>Documentation</c> partial is defined like this:
    /// <code language="html">
    /// {{#if title}}<h3>{{title}}</h3>{{/if}}
    /// {{#if asList}}
    ///     {{#node}}
    ///         {{#if @first}}<ul>{{/if}}
    ///             <li>{{> (GetDocumentationPartialName)}}</li>
    ///         {{#if @last}}</ul>{{/if}}
    ///     {{/node}}
    /// {{else}}
    ///     {{#node}}
    ///         {{> (GetDocumentationPartialName)}}
    ///     {{/node}}
    /// {{/if}}
    /// </code>
    /// As you can see, the specific <see cref="DocumentationElement"/> partial name is selected using this helper, it's like determining the name
    /// of the function you want to call dynamically.
    /// </example>
    public class GetDocumentationPartialName : IHelperDescriptor<HelperOptions>
    {
        /// <summary>Gets the name of the helper.</summary>
        /// <value>The value of this property is <c>GetDocumentationPartialName</c>.</value>
        public PathInfo Name
            => nameof(GetDocumentationPartialName);

        /// <summary>Gets the documentation partial name of the first argument or current context.</summary>
        /// <param name="options">The helper options.</param>
        /// <param name="context">The context in which this helper is called.</param>
        /// <param name="arguments">The arguments with which this helper has been called.</param>
        /// <returns>Returns the documentation partial name of the first argument or current context.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the first argument is not an <see cref="DocumentationElement"/> or when not provided and the given <paramref name="context"/> is not an <see cref="DocumentationElement"/>.
        /// </exception>
        public object Invoke(in HelperOptions options, in Context context, in Arguments arguments)
        {
            var documentationElement = arguments.DefaultIfEmpty(context.Value).First() as DocumentationElement ?? throw new ArgumentException("Expected a " + nameof(DocumentationElement) + " provided as the first argument or context.");

            var visitor = new PartialNameSelector();
            documentationElement.Accept(visitor);
            return visitor.PartialName;
        }

        /// <summary>Writes the documentation partial name of the first argument or current context to the provided <paramref name="output"/>.</summary>
        /// <param name="output">The <see cref="EncodedTextWriter"/> to write the result to.</param>
        /// <param name="options">The helper options.</param>
        /// <param name="context">The context in which this helper is called.</param>
        /// <param name="arguments">The arguments with which this helper has been called.</param>
        /// <returns>Returns the documentation partial name of the first argument or current context.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the first argument is not an <see cref="DocumentationElement"/> or when not provided and the given <paramref name="context"/> is not an <see cref="DocumentationElement"/>.
        /// </exception>
        public void Invoke(in EncodedTextWriter output, in HelperOptions options, in Context context, in Arguments arguments)
            => output.Write(Convert.ToString(Invoke(options, context, arguments)), encode: false);

        private sealed class PartialNameSelector : DocumentationVisitor
        {
            public string PartialName { get; private set; }

            protected override void VisitSummary(SummaryDocumentationElement summary)
                => PartialName = DocumentationPartialNames.Summary;

            protected override void VisitRemarks(RemarksDocumentationElement remarks)
                => PartialName = DocumentationPartialNames.Remarks;

            protected override void VisitExample(ExampleDocumentationElement example)
                => PartialName = DocumentationPartialNames.Example;

            protected override void VisitException(ExceptionDocumentationElement exception)
                => PartialName = DocumentationPartialNames.Exception;

            protected override void VisitValue(ValueDocumentationElement value)
                => PartialName = DocumentationPartialNames.Value;

            protected override void VisitParagraph(ParagraphDocumentationElement paragraph)
                => PartialName = DocumentationPartialNames.Paragraph;

            protected override void VisitCodeBlock(CodeBlockDocumentationElement codeBlock)
                => PartialName = DocumentationPartialNames.CodeBlock;

            protected override void VisitUnorderedList(UnorderedListDocumentationElement unorderedList)
                => PartialName = DocumentationPartialNames.UnorderedList;

            protected override void VisitOrderedList(OrderedListDocumentationElement orderedList)
                => PartialName = DocumentationPartialNames.OrderedList;

            protected override void VisitListItem(ListItemDocumentationElement listItem)
                => PartialName = DocumentationPartialNames.ListItem;

            protected override void VisitDefinitionList(DefinitionListDocumentationElement definitionList)
                => PartialName = DocumentationPartialNames.DefinitionList;

            protected override void VisitDefinitionListTitle(DefinitionListTitleDocumentationElement definitionListTitle)
                => PartialName = DocumentationPartialNames.DefinitionListTitle;

            protected override void VisitDefinitionListItem(DefinitionListItemDocumentationElement definitionListItem)
                => PartialName = DocumentationPartialNames.DefinitionListItem;

            protected override void VisitDefinitionListItemTerm(DefinitionListItemTermDocumentationElement definitionListItemTerm)
                => PartialName = DocumentationPartialNames.DefinitionListItemTerm;

            protected override void VisitDefinitionListItemDescription(DefinitionListItemDescriptionDocumentationElement definitionListItemDescription)
                => PartialName = DocumentationPartialNames.DefinitionListItemDescription;

            protected override void VisitTable(TableDocumentationElement table)
                => PartialName = DocumentationPartialNames.Table;

            protected override void VisitTableColumn(TableColumnDocumentationElement tableColumn)
                => PartialName = DocumentationPartialNames.TableColumn;

            protected override void VisitTableRow(TableRowDocumentationElement tableRow)
                => PartialName = DocumentationPartialNames.TableRow;

            protected override void VisitTableCell(TableCellDocumentationElement tableCell)
                => PartialName = DocumentationPartialNames.TableCell;

            protected override void VisitText(TextDocumentationElement text)
                => PartialName = DocumentationPartialNames.Text;

            protected override void VisitHyperlink(HyperlinkDocumentationElement hyperlink)
                => PartialName = DocumentationPartialNames.Hyperlink;

            protected override void VisitInlineCode(InlineCodeDocumentationElement inlineCode)
                => PartialName = DocumentationPartialNames.Code;

            protected override void VisitInlineReference(ReferenceDataDocumentationElement memberInfoReference)
                => PartialName = DocumentationPartialNames.MemberReference;

            protected override void VisitGenericParameterReference(GenericParameterReferenceDocumentationElement genericParameterReference)
                => PartialName = DocumentationPartialNames.GenericParameterReference;

            protected override void VisitParameterReference(ParameterReferenceDocumentationElement parameterReference)
                => PartialName = DocumentationPartialNames.ParameterReference;
        }
    }
}