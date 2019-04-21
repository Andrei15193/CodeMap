using CodeMap.Elements;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap
{
    /// <summary>Represents a visitor for traversing documentation trees.</summary>
    public abstract class DocumentationVisitor
    {
        /// <summary>Initializes a new instance of the <see cref="DocumentationVisitor"/> class.</summary>
        protected DocumentationVisitor()
        {
        }

        /// <summary>Visits an <see cref="AssemblyDocumentationElement"/>.</summary>
        /// <param name="assembly">The <see cref="AssemblyDocumentationElement"/> to visit.</param>
        protected internal virtual void VisitAssembly(AssemblyDocumentationElement assembly)
        {
        }

        /// <summary>Visits an <see cref="AssemblyDocumentationElement"/>.</summary>
        /// <param name="assembly">The <see cref="AssemblyDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitAssemblyAsync(AssemblyDocumentationElement assembly, CancellationToken cancellationToken)
        {
            try
            {
                VisitAssembly(assembly);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits a <see cref="NamespaceDocumentationElement"/>.</summary>
        /// <param name="namespace">The <see cref="NamespaceDocumentationElement"/> to visit.</param>
        protected internal virtual void VisitNamespace(NamespaceDocumentationElement @namespace)
        {
        }

        /// <summary>Visits a <see cref="NamespaceDocumentationElement"/>.</summary>
        /// <param name="namespace">The <see cref="NamespaceDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitNamespaceAsync(NamespaceDocumentationElement @namespace, CancellationToken cancellationToken)
        {
            try
            {
                VisitNamespace(@namespace);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits an <see cref="EnumDocumentationElement"/>.</summary>
        /// <param name="enum">The <see cref="EnumDocumentationElement"/> to visit.</param>
        protected internal virtual void VisitEnum(EnumDocumentationElement @enum)
        {
        }

        /// <summary>Visits an <see cref="EnumDocumentationElement"/>.</summary>
        /// <param name="enum">The <see cref="EnumDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitEnumAsync(EnumDocumentationElement @enum, CancellationToken cancellationToken)
        {
            try
            {
                VisitEnum(@enum);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits a <see cref="DelegateDocumentationElement"/>.</summary>
        /// <param name="delegate">The <see cref="DelegateDocumentationElement"/> to visit.</param>
        protected internal virtual void VisitDelegate(DelegateDocumentationElement @delegate)
        {
        }

        /// <summary>Visits a <see cref="DelegateDocumentationElement"/>.</summary>
        /// <param name="delegate">The <see cref="DelegateDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitDelegateAsync(DelegateDocumentationElement @delegate, CancellationToken cancellationToken)
        {
            try
            {
                VisitDelegate(@delegate);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits an <see cref="InterfaceDocumentationElement"/>.</summary>
        /// <param name="interface">The <see cref="InterfaceDocumentationElement"/> to visit.</param>
        protected internal virtual void VisitInterface(InterfaceDocumentationElement @interface)
        {
        }

        /// <summary>Visits an <see cref="InterfaceDocumentationElement"/>.</summary>
        /// <param name="interface">The <see cref="InterfaceDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitInterfaceAsync(InterfaceDocumentationElement @interface, CancellationToken cancellationToken)
        {
            try
            {
                VisitInterface(@interface);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits a <see cref="ClassDocumentationElement"/>.</summary>
        /// <param name="class">The <see cref="ClassDocumentationElement"/> to visit.</param>
        protected internal virtual void VisitClass(ClassDocumentationElement @class)
        {
        }

        /// <summary>Visits a <see cref="ClassDocumentationElement"/>.</summary>
        /// <param name="class">The <see cref="ClassDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitClassAsync(ClassDocumentationElement @class, CancellationToken cancellationToken)
        {
            try
            {
                VisitClass(@class);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits a <see cref="StructDocumentationElement"/>.</summary>
        /// <param name="struct">The <see cref="StructDocumentationElement"/> to visit.</param>
        protected internal virtual void VisitStruct(StructDocumentationElement @struct)
        {
        }

        /// <summary>Visits a <see cref="StructDocumentationElement"/>.</summary>
        /// <param name="struct">The <see cref="StructDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitStructAsync(StructDocumentationElement @struct, CancellationToken cancellationToken)
        {
            try
            {
                VisitStruct(@struct);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits a <see cref="ConstantDocumentationElement"/>.</summary>
        /// <param name="constant">The <see cref="ConstantDocumentationElement"/> to visit.</param>
        protected internal virtual void VisitConstant(ConstantDocumentationElement constant)
        {
        }

        /// <summary>Visits a <see cref="ConstantDocumentationElement"/>.</summary>
        /// <param name="constant">The <see cref="ConstantDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitConstantAsync(ConstantDocumentationElement constant, CancellationToken cancellationToken)
        {
            try
            {
                VisitConstant(constant);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits a <see cref="FieldDocumentationElement"/>.</summary>
        /// <param name="field">The <see cref="FieldDocumentationElement"/> to visit.</param>
        protected internal virtual void VisitField(FieldDocumentationElement field)
        {
        }

        /// <summary>Visits a <see cref="FieldDocumentationElement"/>.</summary>
        /// <param name="field">The <see cref="FieldDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitFieldAsync(FieldDocumentationElement field, CancellationToken cancellationToken)
        {
            try
            {
                VisitField(field);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits a <see cref="ConstructorDocumentationElement"/>.</summary>
        /// <param name="constructor">The <see cref="ConstructorDocumentationElement"/> to visit.</param>
        protected internal virtual void VisitConstructor(ConstructorDocumentationElement constructor)
        {
        }

        /// <summary>Visits a <see cref="ConstructorDocumentationElement"/>.</summary>
        /// <param name="constructor">The <see cref="ConstructorDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitConstructorAsync(ConstructorDocumentationElement constructor, CancellationToken cancellationToken)
        {
            try
            {
                VisitConstructor(constructor);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits a <see cref="EventDocumentationElement"/>.</summary>
        /// <param name="event">The <see cref="EventDocumentationElement"/> to visit.</param>
        protected internal virtual void VisitEvent(EventDocumentationElement @event)
        {
        }

        /// <summary>Visits a <see cref="EventDocumentationElement"/>.</summary>
        /// <param name="event">The <see cref="EventDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitEventAsync(EventDocumentationElement @event, CancellationToken cancellationToken)
        {
            try
            {
                VisitEvent(@event);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits a <see cref="PropertyDocumentationElement"/>.</summary>
        /// <param name="property">The <see cref="PropertyDocumentationElement"/> to visit.</param>
        protected internal virtual void VisitProperty(PropertyDocumentationElement property)
        {
        }

        /// <summary>Visits a <see cref="PropertyDocumentationElement"/>.</summary>
        /// <param name="property">The <see cref="PropertyDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitPropertyAsync(PropertyDocumentationElement property, CancellationToken cancellationToken)
        {
            try
            {
                VisitProperty(property);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits a <see cref="MethodDocumentationElement"/>.</summary>
        /// <param name="method">The <see cref="MethodDocumentationElement"/> to visit.</param>
        protected internal virtual void VisitMethod(MethodDocumentationElement method)
        {
        }

        /// <summary>Visits a <see cref="MethodDocumentationElement"/>.</summary>
        /// <param name="method">The <see cref="MethodDocumentationElement"/> to visit.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitMethodAsync(MethodDocumentationElement method, CancellationToken cancellationToken)
        {
            try
            {
                VisitMethod(method);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a summary element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>summary</c> element.</param>
        protected internal virtual void VisitSummaryBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of a summary element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>summary</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitSummaryBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitSummaryBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a summary element.</summary>
        protected internal virtual void VisitSummaryEnding()
        {
        }

        /// <summary>Visits the ending of a summary element.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitSummaryEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitSummaryEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a remarks element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>remarks</c> element.</param>
        protected internal virtual void VisitRemarksBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of a remarks element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>remarks</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitRemarksBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitRemarksBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a remarks element.</summary>
        protected internal virtual void VisitRemarksEnding()
        {
        }

        /// <summary>Visits the ending of a remarks element.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitRemarksEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitRemarksEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of an example element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>example</c> element.</param>
        protected internal virtual void VisitExampleBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }


        /// <summary>Visits the beginning of an example element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>example</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitExampleBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitExampleBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of an example element.</summary>
        protected internal virtual void VisitExampleEnding()
        {
        }

        /// <summary>Visits the ending of an example element.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitExampleEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitExampleEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a value element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>value</c> element.</param>
        protected internal virtual void VisitValueBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of a value element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>value</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitValueBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitValueBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a value element.</summary>
        protected internal virtual void VisitValueEnding()
        {
        }

        /// <summary>Visits the ending of a value element.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitValueEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitValueEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a paragraph element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>para</c> element.</param>
        protected internal virtual void VisitParagraphBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of a paragraph element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>para</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitParagraphBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitParagraphBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a paragraph element.</summary>
        protected internal virtual void VisitParagraphEnding()
        {
        }

        /// <summary>Visits the ending of a paragraph element.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitParagraphEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitParagraphEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits a code block element.</summary>
        /// <param name="code">The text inside the code block.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>code</c> element.</param>
        protected internal virtual void VisitCodeBlock(string code, IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits a code block element.</summary>
        /// <param name="code">The text inside the code block.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>code</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitCodeBlockAsync(string code, IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitCodeBlock(code, xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a related members list.</summary>
        protected internal virtual void VisitRelatedMembersListBeginning()
        {
        }

        /// <summary>Visits the beginning of a related members list.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitRelatedMembersListBeginningAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitRelatedMembersListBeginning();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a related members list.</summary>
        protected internal virtual void VisitRelatedMembersListEnding()
        {
        }

        /// <summary>Visits the ending of a related members list.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitRelatedMembersListEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitRelatedMembersListEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits a related member element.</summary>
        /// <param name="relatedMember">The related member reference.</param>
        protected internal virtual void VisitRelatedMember(MemberReferenceDocumentationElement relatedMember)
        {
            relatedMember.Accept(this);
        }

        /// <summary>Visits a related member element.</summary>
        /// <param name="relatedMember">The related member reference.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitRelatedMemberAsync(MemberReferenceDocumentationElement relatedMember, CancellationToken cancellationToken)
        {
            try
            {
                VisitRelatedMember(relatedMember);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of an unordered list element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>list</c> element.</param>
        protected internal virtual void VisitUnorderedListBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of an unordered list element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>list</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitUnorderedListBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitUnorderedListBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of an unordered list element.</summary>
        protected internal virtual void VisitUnorderedListEnding()
        {
        }

        /// <summary>Visits the ending of an unordered list element.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitUnorderedListEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitUnorderedListEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of an ordered list element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>list</c> element.</param>
        protected internal virtual void VisitOrderedListBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of an ordered list element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>list</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitOrderedListBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitOrderedListBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of an ordered list element.</summary>
        protected internal virtual void VisitOrderedListEnding()
        {
        }

        /// <summary>Visits the ending of an ordered list element.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitOrderedListEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitOrderedListEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a list item element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>item</c> or <c>description</c> element.</param>
        protected internal virtual void VisitListItemBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of a list item element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>item</c> or <c>description</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitListItemBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitListItemBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a list item element.</summary>
        protected internal virtual void VisitListItemEnding()
        {
        }

        /// <summary>Visits the ending of a list item element.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitListItemEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitListItemEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a definition list element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>list</c> element.</param>
        protected internal virtual void VisitDefinitionListBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of a definition list element.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>list</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitDefinitionListBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitDefinitionListBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a definition list element.</summary>
        protected internal virtual void VisitDefinitionListEnding()
        {
        }

        /// <summary>Visits the ending of a definition list element.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitDefinitionListEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitDefinitionListEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a definition list title.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>listheader</c> element.</param>
        protected internal virtual void VisitDefinitionListTitleBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of a definition list title.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>listheader</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitDefinitionListTitleBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitDefinitionListTitleBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a definition list title.</summary>
        protected internal virtual void VisitDefinitionListTitleEnding()
        {
        }

        /// <summary>Visits the ending of a definition list title.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitDefinitionListTitleEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitDefinitionListTitleEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a definition list item.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>item</c> element.</param>
        protected internal virtual void VisitDefinitionListItemBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of a definition list item.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>item</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitDefinitionListItemBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitDefinitionListItemBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a definition list item.</summary>
        protected internal virtual void VisitDefinitionListItemEnding()
        {
        }

        /// <summary>Visits the ending of a definition list item.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitDefinitionListItemEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitDefinitionListItemEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a definition list term.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>term</c> element.</param>
        protected internal virtual void VisitDefinitionTermBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of a definition list term.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>term</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitDefinitionTermBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitDefinitionTermBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a definition list term.</summary>
        protected internal virtual void VisitDefinitionTermEnding()
        {
        }

        /// <summary>Visits the ending of a definition list term.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitDefinitionTermEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitDefinitionTermEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a definition list term description.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>description</c> element.</param>
        protected internal virtual void VisitDefinitionTermDescriptionBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of a definition list term description.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>description</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitDefinitionTermDescriptionBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitDefinitionTermDescriptionBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a definition list term description.</summary>
        protected internal virtual void VisitDefinitionTermDescriptionEnding()
        {
        }

        /// <summary>Visits the ending of a definition list term description.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitDefinitionTermDescriptionEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitDefinitionTermDescriptionEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a table.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>list</c> element.</param>
        protected internal virtual void VisitTableBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of a table.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>list</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitTableBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitTableBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a table.</summary>
        protected internal virtual void VisitTableEnding()
        {
        }

        /// <summary>Visits the ending of a table.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitTableEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitTableEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a table heading.</summary>
        protected internal virtual void VisitTableHeadingBeginning()
        {
        }

        /// <summary>Visits the beginning of a table heading.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitTableHeadingBeginningAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitTableHeadingBeginning();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a table heading.</summary>
        protected internal virtual void VisitTableHeadingEnding()
        {
        }

        /// <summary>Visits the ending of a table heading.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitTableHeadingEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitTableHeadingEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a table body.</summary>
        protected internal virtual void VisitTableBodyBeginning()
        {
        }

        /// <summary>Visits the beginning of a table body.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitTableBodyBeginningAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitTableBodyBeginning();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a table body.</summary>
        protected internal virtual void VisitTableBodyEnding()
        {
        }

        /// <summary>Visits the ending of a table body.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitTableBodyEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitTableBodyEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a table column.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>term</c> element.</param>
        protected internal virtual void VisitTableColumnBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of a table column.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>term</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitTableColumnBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitTableColumnBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a table column.</summary>
        protected internal virtual void VisitTableColumnEnding()
        {
        }

        /// <summary>Visits the ending of a table column.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitTableColumnEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitTableColumnEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a table row.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>item</c> element.</param>
        protected internal virtual void VisitTableRowBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of a table row.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>item</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitTableRowBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitTableRowBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a table row.</summary>
        protected internal virtual void VisitTableRowEnding()
        {
        }

        /// <summary>Visits the ending of a table row.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitTableRowEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitTableRowEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the beginning of a table cell.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>description</c> element.</param>
        protected internal virtual void VisitTableCellBeginning(IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits the beginning of a table cell.</summary>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>description</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitTableCellBeginningAsync(IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitTableCellBeginning(xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits the ending of a table cell.</summary>
        protected internal virtual void VisitTableCellEnding()
        {
        }

        /// <summary>Visits the ending of a table cell.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitTableCellEndingAsync(CancellationToken cancellationToken)
        {
            try
            {
                VisitTableCellEnding();
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits plain text.</summary>
        /// <param name="text">The plain text inside a block element.</param>
        protected internal virtual void VisitText(string text)
        {
        }

        /// <summary>Visits plain text.</summary>
        /// <param name="text">The plain text inside a block element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitTextAsync(string text, CancellationToken cancellationToken)
        {
            try
            {
                VisitText(text);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits an inline member reference.</summary>
        /// <param name="canonicalName">The canonical name of the referred member.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>see</c> or <c>seealso</c> element.</param>
        protected internal virtual void VisitInlineReference(string canonicalName, IReadOnlyDictionary<string, string> xmlAttributes)
        {
            throw new InvalidOperationException($"Could not find member from '{canonicalName}' canonical name. Override VisitInlineReference(string) or VisitInlineReferenceAsync(string, CancellationToken) to ignore this error.");
        }

        /// <summary>Visits an inline member reference.</summary>
        /// <param name="canonicalName">The canonical name of the referred member.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>see</c> or <c>seealso</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitInlineReferenceAsync(string canonicalName, IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitInlineReference(canonicalName, xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits an inline member reference.</summary>
        /// <param name="referredMember">The referred member.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>see</c> or <c>seealso</c> element.</param>
        protected internal virtual void VisitInlineReference(MemberInfo referredMember, IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits an inline member reference.</summary>
        /// <param name="referredMember">The referred member.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>see</c> or <c>seealso</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitInlineReferenceAsync(MemberInfo referredMember, IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitInlineReference(referredMember, xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits an inline code snippet.</summary>
        /// <param name="code">The text inside the inline code.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>c</c> element.</param>
        protected internal virtual void VisitInlineCode(string code, IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits an inline code snippet.</summary>
        /// <param name="code">The text inside the inline code.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>c</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitInlineCodeAsync(string code, IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitInlineCode(code, xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits an inline parameter reference.</summary>
        /// <param name="parameterName">The name of the referred parameter.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>paramref</c> element.</param>
        protected internal virtual void VisitParameterReference(string parameterName, IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits an inline parameter reference.</summary>
        /// <param name="parameterName">The name of the referred parameter.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>paramref</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitParameterReferenceAsync(string parameterName, IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitParameterReference(parameterName, xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

        /// <summary>Visits an inline generic parameter reference.</summary>
        /// <param name="genericParameterName">The name of the referred generic parameter.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>typeparamref</c> element.</param>
        protected internal virtual void VisitGenericParameterReference(string genericParameterName, IReadOnlyDictionary<string, string> xmlAttributes)
        {
        }

        /// <summary>Visits an inline generic parameter reference.</summary>
        /// <param name="genericParameterName">The name of the referred generic parameter.</param>
        /// <param name="xmlAttributes">The XML attributes specified on the <c>typeparamref</c> element.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        protected internal virtual Task VisitGenericParameterReferenceAsync(string genericParameterName, IReadOnlyDictionary<string, string> xmlAttributes, CancellationToken cancellationToken)
        {
            try
            {
                VisitGenericParameterReference(genericParameterName, xmlAttributes);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }
    }
}