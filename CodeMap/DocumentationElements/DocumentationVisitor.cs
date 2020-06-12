using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodeMap.DocumentationElements
{
    /// <summary>Represents a visitor for traversing documentation trees.</summary>
    public abstract class DocumentationVisitor
    {
        /// <summary>Initializes a new instance of the <see cref="DocumentationVisitor"/> class.</summary>
        protected DocumentationVisitor()
        {
        }

        /// <summary>Visits the beginning of a summary element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>summary</c> element.</param>
        protected internal abstract void VisitSummaryBeginning(IReadOnlyDictionary<string, string> xmlAttributes);

        /// <summary>Visits the ending of a summary element.</summary>
        protected internal abstract void VisitSummaryEnding();

        /// <summary>Visits the beginning of a remarks element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>remarks</c> element.</param>
        protected internal abstract void VisitRemarksBeginning(IReadOnlyDictionary<string, string> xmlAttributes);

        /// <summary>Visits the ending of a remarks element.</summary>
        protected internal abstract void VisitRemarksEnding();

        /// <summary>Visits the beginning of an example element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>example</c> element.</param>
        protected internal abstract void VisitExampleBeginning(IReadOnlyDictionary<string, string> xmlAttributes);


        /// <summary>Visits the ending of an example element.</summary>
        protected internal abstract void VisitExampleEnding();

        /// <summary>Visits the beginning of a value element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>value</c> element.</param>
        protected internal abstract void VisitValueBeginning(IReadOnlyDictionary<string, string> xmlAttributes);

        /// <summary>Visits the ending of a value element.</summary>
        protected internal abstract void VisitValueEnding();

        /// <summary>Visits the beginning of a paragraph element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>para</c> element.</param>
        protected internal abstract void VisitParagraphBeginning(IReadOnlyDictionary<string, string> xmlAttributes);

        /// <summary>Visits the ending of a paragraph element.</summary>
        protected internal abstract void VisitParagraphEnding();

        /// <summary>Visits a code block element.</summary>
        /// <param name="code">The text inside the code block.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>code</c> element.</param>
        protected internal abstract void VisitCodeBlock(string code, IReadOnlyDictionary<string, string> xmlAttributes);

        /// <summary>Visits the beginning of an unordered list element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>list</c> element.</param>
        protected internal abstract void VisitUnorderedListBeginning(IReadOnlyDictionary<string, string> xmlAttributes);

        /// <summary>Visits the ending of an unordered list element.</summary>
        protected internal abstract void VisitUnorderedListEnding();

        /// <summary>Visits the beginning of an ordered list element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>list</c> element.</param>
        protected internal abstract void VisitOrderedListBeginning(IReadOnlyDictionary<string, string> xmlAttributes);

        /// <summary>Visits the ending of an ordered list element.</summary>
        protected internal abstract void VisitOrderedListEnding();

        /// <summary>Visits the beginning of a list item element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>item</c> or <c>description</c> element.</param>
        protected internal abstract void VisitListItemBeginning(IReadOnlyDictionary<string, string> xmlAttributes);

        /// <summary>Visits the ending of a list item element.</summary>
        protected internal abstract void VisitListItemEnding();

        /// <summary>Visits the beginning of a definition list element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>list</c> element.</param>
        protected internal abstract void VisitDefinitionListBeginning(IReadOnlyDictionary<string, string> xmlAttributes);

        /// <summary>Visits the ending of a definition list element.</summary>
        protected internal abstract void VisitDefinitionListEnding();

        /// <summary>Visits the beginning of a definition list title.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>listheader</c> element.</param>
        protected internal abstract void VisitDefinitionListTitleBeginning(IReadOnlyDictionary<string, string> xmlAttributes);

        /// <summary>Visits the ending of a definition list title.</summary>
        protected internal abstract void VisitDefinitionListTitleEnding();

        /// <summary>Visits the beginning of a definition list item.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>item</c> element.</param>
        protected internal abstract void VisitDefinitionListItemBeginning(IReadOnlyDictionary<string, string> xmlAttributes);

        /// <summary>Visits the ending of a definition list item.</summary>
        protected internal abstract void VisitDefinitionListItemEnding();

        /// <summary>Visits the beginning of a definition list term.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>term</c> element.</param>
        protected internal abstract void VisitDefinitionTermBeginning(IReadOnlyDictionary<string, string> xmlAttributes);

        /// <summary>Visits the ending of a definition list term.</summary>
        protected internal abstract void VisitDefinitionTermEnding();

        /// <summary>Visits the beginning of a definition list term description.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>description</c> element.</param>
        protected internal abstract void VisitDefinitionTermDescriptionBeginning(IReadOnlyDictionary<string, string> xmlAttributes);

        /// <summary>Visits the ending of a definition list term description.</summary>
        protected internal abstract void VisitDefinitionTermDescriptionEnding();

        /// <summary>Visits the beginning of a table.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>list</c> element.</param>
        protected internal abstract void VisitTableBeginning(IReadOnlyDictionary<string, string> xmlAttributes);

        /// <summary>Visits the ending of a table.</summary>
        protected internal abstract void VisitTableEnding();

        /// <summary>Visits the beginning of a table heading.</summary>
        protected internal abstract void VisitTableHeadingBeginning();

        /// <summary>Visits the ending of a table heading.</summary>
        protected internal abstract void VisitTableHeadingEnding();

        /// <summary>Visits the beginning of a table body.</summary>
        protected internal abstract void VisitTableBodyBeginning();

        /// <summary>Visits the ending of a table body.</summary>
        protected internal abstract void VisitTableBodyEnding();

        /// <summary>Visits the beginning of a table column.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>term</c> element.</param>
        protected internal abstract void VisitTableColumnBeginning(IReadOnlyDictionary<string, string> xmlAttributes);

        /// <summary>Visits the ending of a table column.</summary>
        protected internal abstract void VisitTableColumnEnding();

        /// <summary>Visits the beginning of a table row.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>item</c> element.</param>
        protected internal abstract void VisitTableRowBeginning(IReadOnlyDictionary<string, string> xmlAttributes);

        /// <summary>Visits the ending of a table row.</summary>
        protected internal abstract void VisitTableRowEnding();

        /// <summary>Visits the beginning of a table cell.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>description</c> element.</param>
        protected internal abstract void VisitTableCellBeginning(IReadOnlyDictionary<string, string> xmlAttributes);

        /// <summary>Visits the ending of a table cell.</summary>
        protected internal abstract void VisitTableCellEnding();

        /// <summary>Visits plain text.</summary>
        /// <param name="text">The plain text inside a block element.</param>
        protected internal abstract void VisitText(string text);

        /// <summary>Visits an inline member reference.</summary>
        /// <param name="canonicalName">The canonical name of the referred member.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>see</c> or <c>seealso</c> element.</param>
        protected internal virtual void VisitInlineReference(string canonicalName, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            throw new InvalidOperationException($"Could not find member from '{canonicalName}' canonical name. Override VisitInlineReference(string) to ignore this error.");
        }

        /// <summary>Visits an inline member reference.</summary>
        /// <param name="referredMember">The referred member.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>see</c> or <c>seealso</c> element.</param>
        protected internal abstract void VisitInlineReference(MemberInfo referredMember, IReadOnlyDictionary<string, string> xmlAttributes);

        /// <summary>Visits an inline code snippet.</summary>
        /// <param name="code">The text inside the inline code.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>c</c> element.</param>
        protected internal abstract void VisitInlineCode(string code, IReadOnlyDictionary<string, string> xmlAttributes);

        /// <summary>Visits an inline parameter reference.</summary>
        /// <param name="parameterName">The name of the referred parameter.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>paramref</c> element.</param>
        protected internal abstract void VisitParameterReference(string parameterName, IReadOnlyDictionary<string, string> xmlAttributes);

        /// <summary>Visits an inline generic parameter reference.</summary>
        /// <param name="genericParameterName">The name of the referred generic parameter.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>typeparamref</c> element.</param>
        protected internal abstract void VisitGenericParameterReference(string genericParameterName, IReadOnlyDictionary<string, string> xmlAttributes);
    }
}