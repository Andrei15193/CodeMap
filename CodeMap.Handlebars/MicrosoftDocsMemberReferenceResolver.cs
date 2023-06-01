using System.Linq;
using System.Text;
using CodeMap.ReferenceData;

namespace CodeMap.Handlebars
{
    /// <summary>A default implementation for resolving member references.</summary>
    public class MicrosoftDocsMemberReferenceResolver : IMemberReferenceResolver
    {
        private readonly string _microsoftDocsView;

        /// <summary>Initializes a new instance of the <see cref="MicrosoftDocsMemberReferenceResolver"/> class.</summary>
        /// <param name="microsoftDocsView">The view query string parameter when generating MS docs links, this corresponds to the target version.</param>
        public MicrosoftDocsMemberReferenceResolver(string microsoftDocsView)
            => _microsoftDocsView = microsoftDocsView;

        /// <summary>Gets the URL for the provided <paramref name="memberReference"/>.</summary>
        /// <param name="memberReference">The <see cref="MemberReference"/> for which to generate the URL.</param>
        /// <returns>Returns the URL for the provided <see cref="MemberReference"/>. If it points to a member of the documented library then an URL for that page is returned; otherwise an MS doc reference is created.</returns>
        public string GetUrl(MemberReference memberReference)
        {
            var memberReferenceMicrosoftLinkVisitor = new MemberReferenceMicrosoftLinkVisitor(_microsoftDocsView);
            memberReference.Accept(memberReferenceMicrosoftLinkVisitor);
            return memberReferenceMicrosoftLinkVisitor.Result;
        }

        private class MemberReferenceMicrosoftLinkVisitor : MemberReferenceVisitor
        {
            private readonly StringBuilder _linkBuilder = new StringBuilder("https://learn.microsoft.com/dotnet/api/");
            private readonly string _view;
            private string _result;

            public MemberReferenceMicrosoftLinkVisitor(string view)
                => _view = view;

            public string Result
            {
                get
                {
                    if (_result is null)
                    {
                        if (!string.IsNullOrWhiteSpace(_view))
                            _linkBuilder.Append("?view=").Append(_view);
                        _result = _linkBuilder.ToString();
                    }
                    return _result;
                }
            }

            protected override void VisitAssembly(AssemblyReference assembly)
            {
            }

            protected override void VisitNamespace(NamespaceReference @namespace)
            {
            }

            protected override void VisitType(TypeReference type)
            {
                if (type.DeclaringType is object)
                {
                    type.DeclaringType.Accept(this);
                    _linkBuilder.Append('.');
                }
                else
                    _linkBuilder.Append(type.Namespace.Name).Append('.');

                _linkBuilder.Append(type.Name);
                if (type.GenericArguments.Any())
                {
                    _linkBuilder.Append('-');
                    _linkBuilder.Append(type.GenericArguments.Count);
                }
            }

            protected override void VisitArray(ArrayTypeReference array)
                => array.ItemType.Accept(this);

            protected override void VisitByRef(ByRefTypeReference byRef)
                => byRef.ReferentType.Accept(this);

            protected override void VisitPointer(PointerTypeReference pointer)
                => pointer.ReferentType.Accept(this);

            protected override void VisitConstant(ConstantReference constant)
            {
                constant.DeclaringType.Accept(this);
                _linkBuilder.Append('.').Append(constant.Name);
            }

            protected override void VisitField(FieldReference field)
            {
                field.DeclaringType.Accept(this);
                _linkBuilder.Append('.').Append(field.Name);
            }

            protected override void VisitConstructor(ConstructorReference constructor)
            {
                constructor.DeclaringType.Accept(this);
                _linkBuilder.Append(".-ctor");
            }

            protected override void VisitEvent(EventReference @event)
            {
                @event.DeclaringType.Accept(this);
                _linkBuilder.Append('.').Append(@event.Name);
            }

            protected override void VisitGenericMethodParameter(GenericMethodParameterReference genericMethodParameter)
            {
            }

            protected override void VisitGenericTypeParameter(GenericTypeParameterReference genericTypeParameter)
            {
            }

            protected override void VisitProperty(PropertyReference property)
            {
                property.DeclaringType.Accept(this);
                _linkBuilder.Append('.').Append(property.Name);
            }

            protected override void VisitMethod(MethodReference method)
            {
                method.DeclaringType.Accept(this);
                _linkBuilder.Append('.').Append(method.Name);
                if (method.GenericArguments.Any())
                    _linkBuilder.Append('-').Append(method.GenericArguments.Count);
            }
        }
    }
}