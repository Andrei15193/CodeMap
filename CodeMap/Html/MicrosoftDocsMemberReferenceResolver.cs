using System.Linq;
using System.Text;
using CodeMap.ReferenceData;

namespace CodeMap.Html
{
    /// <summary>A default implementation for resolving member references.</summary>
    public class MicrosoftDocsMemberReferenceResolver : IMemberReferenceResolver
    {
        private const string _baseUrl = "https://learn.microsoft.com/";
        private readonly string _microsoftDocsView;
        private readonly string _locale;

        /// <summary>Initializes a new instance of the <see cref="MicrosoftDocsMemberReferenceResolver"/> class.</summary>
        public MicrosoftDocsMemberReferenceResolver()
            : this(null, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="MicrosoftDocsMemberReferenceResolver"/> class.</summary>
        /// <param name="microsoftDocsView">The view query string parameter when generating MS docs links, this corresponds to the target version.</param>
        public MicrosoftDocsMemberReferenceResolver(string microsoftDocsView)
            : this(microsoftDocsView, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="MicrosoftDocsMemberReferenceResolver"/> class.</summary>
        /// <param name="microsoftDocsView">The view query string parameter when generating MS docs links, this corresponds to the target version.</param>
        /// <param name="locale">The locale to use when generating MS docs links.</param>
        public MicrosoftDocsMemberReferenceResolver(string microsoftDocsView, string locale)
        {
            _microsoftDocsView = microsoftDocsView;
            _locale = locale;
        }

        /// <summary>Gets the URL for the provided <paramref name="memberReference"/>.</summary>
        /// <param name="memberReference">The <see cref="MemberReference"/> for which to generate the URL.</param>
        /// <returns>Returns the URL for the provided <see cref="MemberReference"/>. If it points to a member of the documented library then an URL for that page is returned; otherwise an MS doc reference is created.</returns>
        public string GetUrl(MemberReference memberReference)
        {
            var memberReferenceMicrosoftLinkVisitor = new MemberReferenceMicrosoftLinkVisitor(_microsoftDocsView, _locale);
            memberReference.Accept(memberReferenceMicrosoftLinkVisitor);
            return memberReferenceMicrosoftLinkVisitor.Result;
        }

        private class MemberReferenceMicrosoftLinkVisitor : MemberReferenceVisitor
        {
            private readonly StringBuilder _linkBuilder;
            private readonly string _view;
            private string _result;

            public MemberReferenceMicrosoftLinkVisitor(string view, string locale)
            {
                _linkBuilder = new StringBuilder(_baseUrl);
                if (!string.IsNullOrWhiteSpace(locale))
                    _linkBuilder.Append(locale).Append('/');
                _linkBuilder.Append("dotnet/api/");
                _view = view;
            }

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

            protected internal override void VisitAssembly(AssemblyReference assembly)
            {
            }

            protected internal override void VisitNamespace(NamespaceReference @namespace)
            {
            }

            protected internal override void VisitType(TypeReference type)
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

            protected internal override void VisitArray(ArrayTypeReference array)
                => array.ItemType.Accept(this);

            protected internal override void VisitByRef(ByRefTypeReference byRef)
                => byRef.ReferentType.Accept(this);

            protected internal override void VisitPointer(PointerTypeReference pointer)
                => pointer.ReferentType.Accept(this);

            protected internal override void VisitConstant(ConstantReference constant)
            {
                constant.DeclaringType.Accept(this);
                _linkBuilder.Append('.').Append(constant.Name);
            }

            protected internal override void VisitField(FieldReference field)
            {
                field.DeclaringType.Accept(this);
                _linkBuilder.Append('.').Append(field.Name);
            }

            protected internal override void VisitConstructor(ConstructorReference constructor)
            {
                constructor.DeclaringType.Accept(this);
                _linkBuilder.Append(".-ctor");
            }

            protected internal override void VisitEvent(EventReference @event)
            {
                @event.DeclaringType.Accept(this);
                _linkBuilder.Append('.').Append(@event.Name);
            }

            protected internal override void VisitGenericMethodParameter(GenericMethodParameterReference genericMethodParameter)
            {
            }

            protected internal override void VisitGenericTypeParameter(GenericTypeParameterReference genericTypeParameter)
            {
            }

            protected internal override void VisitProperty(PropertyReference property)
            {
                property.DeclaringType.Accept(this);
                _linkBuilder.Append('.').Append(property.Name);
            }

            protected internal override void VisitMethod(MethodReference method)
            {
                method.DeclaringType.Accept(this);
                _linkBuilder.Append('.').Append(method.Name);
                if (method.GenericArguments.Any())
                    _linkBuilder.Append('-').Append(method.GenericArguments.Count);
            }
        }
    }
}