using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeMap.ReferenceData
{
    internal sealed class MemberInfoEqualityComparerVisitor : MemberReferenceVisitor
    {
        private MemberInfo _current;
        private readonly IDictionary<MemberReference, MemberInfo> _checked = new Dictionary<MemberReference, MemberInfo>();

        public MemberInfoEqualityComparerVisitor(MemberInfo memberInfo)
        {
            _current = memberInfo;
            _checked = new Dictionary<MemberReference, MemberInfo>();
            AreEqual = true;
        }

        public bool AreEqual { get; private set; }

        protected internal override void VisitType(TypeReference typeReference)
        {
            if (_ShouldStopComparing(typeReference))
                return;

            switch (_current)
            {
                case Type type when !type.IsArray
                                    && !type.IsByRef
                                    && !type.IsPointer
                                    && !type.IsGenericParameter
                                    && typeReference.Name.AsSpan().Equals(type.GetTypeName(), StringComparison.OrdinalIgnoreCase)
                                    && string.Equals(typeReference.Namespace, type.Namespace, StringComparison.OrdinalIgnoreCase)
                                    && typeReference.Assembly == type.Assembly.GetName():
                    _checked.Add(typeReference, type);
                    CompareGenericArguments(type);
                    if (AreEqual)
                        if (type.DeclaringType != null && typeReference.DeclaringType != null)
                        {
                            _current = type.GetDeclaringType();
                            typeReference.DeclaringType.Accept(this);
                        }
                        else if (type.DeclaringType != null || typeReference.DeclaringType != null)
                            _SetNotEqual();
                    break;

                default:
                    _SetNotEqual();
                    break;
            }

            void CompareGenericArguments(Type type)
            {
                var genericArguments = type.GetGenericArguments().Skip(type.DeclaringType?.GetGenericArguments().Length ?? 0).AsReadOnlyList();
                if (typeReference.GenericArguments.Count != genericArguments.Count)
                    _SetNotEqual();

                if (typeReference.GenericArguments.OfType<GenericTypeParameterReference>().Any(genericParameter => genericParameter.DeclaringType == typeReference))
                {
                    if (!type.IsGenericTypeDefinition)
                        _SetNotEqual();
                }
                else
                    foreach (var (genericArgumentReference, genericArgument) in typeReference
                        .GenericArguments
                        .Zip(genericArguments, (genericArgumentReference, genericArgument) => (genericArgumentReference, genericArgument))
                        .TakeWhile(pair => AreEqual))
                    {
                        _current = genericArgument;
                        genericArgumentReference.Accept(this);
                    }
            }
        }

        protected internal override void VisitPointer(PointerTypeReference pointerReference)
        {
            if (_ShouldStopComparing(pointerReference))
                return;

            switch (_current)
            {
                case Type type when type.IsPointer:
                    _checked.Add(pointerReference, type);
                    _current = type.GetElementType();
                    pointerReference.ReferentType.Accept(this);
                    break;

                default:
                    _SetNotEqual();
                    break;
            }
        }

        protected internal override void VisitArray(ArrayTypeReference arrayReference)
        {
            if (_ShouldStopComparing(arrayReference))
                return;

            switch (_current)
            {
                case Type type when type.IsArray
                                    && type.GetArrayRank() == arrayReference.Rank:
                    _checked.Add(arrayReference, type);
                    _current = type.GetElementType();
                    arrayReference.ItemType.Accept(this);
                    break;

                default:
                    _SetNotEqual();
                    break;
            }
        }

        protected internal override void VisitByRef(ByRefTypeReference byRefTypeReference)
        {
            if (_ShouldStopComparing(byRefTypeReference))
                return;

            switch (_current)
            {
                case Type type when type.IsByRef:
                    _checked.Add(byRefTypeReference, type);
                    _current = type.GetElementType();
                    byRefTypeReference.ReferentType.Accept(this);
                    break;

                default:
                    _SetNotEqual();
                    break;
            }
        }

        protected internal override void VisitGenericTypeParameter(GenericTypeParameterReference genericTypeParameterReference)
        {
            if (_ShouldStopComparing(genericTypeParameterReference))
                return;

            switch (_current)
            {
                case Type type when type.IsGenericTypeParameter
                                    && string.Equals(genericTypeParameterReference.Name, type.Name, StringComparison.OrdinalIgnoreCase):
                    _checked.Add(genericTypeParameterReference, type);
                    _current = type.DeclaringType;
                    genericTypeParameterReference.DeclaringType.Accept(this);
                    break;

                default:
                    _SetNotEqual();
                    break;
            }
        }

        protected internal override void VisitGenericMethodParameter(GenericMethodParameterReference genericMethodParameterReference)
        {
            if (_ShouldStopComparing(genericMethodParameterReference))
                return;

            switch (_current)
            {
                case Type type when type.IsGenericMethodParameter
                                    && string.Equals(genericMethodParameterReference.Name, type.Name, StringComparison.OrdinalIgnoreCase):
                    _checked.Add(genericMethodParameterReference, type);
                    _current = type.DeclaringMethod;
                    genericMethodParameterReference.DeclaringMethod.Accept(this);
                    break;

                default:
                    _SetNotEqual();
                    break;
            }
        }

        protected internal override void VisitConstant(ConstantReference constantReference)
        {
            if (_ShouldStopComparing(constantReference))
                return;

            switch (_current)
            {
                case FieldInfo fieldInfo when string.Equals(constantReference.Name, fieldInfo.Name, StringComparison.OrdinalIgnoreCase):
                    _checked.Add(constantReference, fieldInfo);
                    _current = fieldInfo.DeclaringType;
                    constantReference.DeclaringType.Accept(this);
                    break;

                default:
                    _SetNotEqual();
                    break;
            }
        }

        protected internal override void VisitField(FieldReference fieldReference)
        {
            if (_ShouldStopComparing(fieldReference))
                return;

            switch (_current)
            {
                case FieldInfo fieldInfo when string.Equals(fieldReference.Name, fieldInfo.Name, StringComparison.OrdinalIgnoreCase):
                    _checked.Add(fieldReference, fieldInfo);
                    _current = fieldInfo.DeclaringType;
                    fieldReference.DeclaringType.Accept(this);
                    break;

                default:
                    _SetNotEqual();
                    break;
            }
        }

        protected internal override void VisitConstructor(ConstructorReference constructorReference)
        {
            if (_ShouldStopComparing(constructorReference))
                return;

            switch (_current)
            {
                case ConstructorInfo constructorInfo:
                    _checked.Add(constructorReference, constructorInfo);
                    var parameters = constructorInfo.GetParameters();
                    if (constructorReference.ParameterTypes.Count != parameters.Length)
                        _SetNotEqual();
                    else
                    {
                        _current = constructorInfo.DeclaringType;
                        constructorReference.DeclaringType.Accept(this);
                        if (AreEqual)
                            foreach (var (paramterTypeReference, parameterInfo) in constructorReference
                                .ParameterTypes
                                .Zip(parameters, (parameterType, parameter) => (parameterType, parameter))
                                .TakeWhile(pair => AreEqual))
                            {
                                _current = parameterInfo.ParameterType;
                                paramterTypeReference.Accept(this);
                            }
                    }
                    break;

                default:
                    _SetNotEqual();
                    break;
            }
        }

        protected internal override void VisitEvent(EventReference eventReference)
        {
            if (_ShouldStopComparing(eventReference))
                return;

            switch (_current)
            {
                case EventInfo eventInfo when string.Equals(eventReference.Name, eventInfo.Name, StringComparison.OrdinalIgnoreCase):
                    _checked.Add(eventReference, eventInfo);
                    _current = eventInfo.DeclaringType;
                    eventReference.DeclaringType.Accept(this);
                    break;

                default:
                    _SetNotEqual();
                    break;
            }
        }

        protected internal override void VisitProperty(PropertyReference propertyReference)
        {
            if (_ShouldStopComparing(propertyReference))
                return;

            switch (_current)
            {
                case PropertyInfo propertyInfo:
                    _checked.Add(propertyReference, propertyInfo);
                    var parameters = propertyInfo.GetIndexParameters();
                    if (propertyReference.ParameterTypes.Count != parameters.Length)
                        _SetNotEqual();
                    else
                    {
                        _current = propertyInfo.DeclaringType;
                        propertyReference.DeclaringType.Accept(this);
                        if (AreEqual)
                            foreach (var (paramterTypeReference, parameterInfo) in propertyReference
                                .ParameterTypes
                                .Zip(parameters, (parameterType, parameter) => (parameterType, parameter))
                                .TakeWhile(pair => AreEqual))
                            {
                                _current = parameterInfo.ParameterType;
                                paramterTypeReference.Accept(this);
                            }
                    }
                    break;

                default:
                    _SetNotEqual();
                    break;
            }
        }

        protected internal override void VisitMethod(MethodReference methodReference)
        {
            if (_ShouldStopComparing(methodReference))
                return;

            switch (_current)
            {
                case MethodInfo methodInfo when string.Equals(methodReference.Name, methodInfo.Name, StringComparison.OrdinalIgnoreCase):
                    _checked.Add(methodReference, methodInfo);
                    var parameters = methodInfo.GetParameters();
                    if (methodReference.ParameterTypes.Count != parameters.Length)
                        _SetNotEqual();
                    else
                    {
                        _current = methodInfo.DeclaringType;
                        methodReference.DeclaringType.Accept(this);
                        if (AreEqual)
                            CompareGenericArguments(methodInfo);
                        if (AreEqual)
                            foreach (var (paramterTypeReference, parameterInfo) in methodReference
                                .ParameterTypes
                                .Zip(parameters, (parameterType, parameter) => (parameterType, parameter))
                                .TakeWhile(pair => AreEqual))
                            {
                                _current = parameterInfo.ParameterType;
                                paramterTypeReference.Accept(this);
                            }
                    }
                    break;

                default:
                    _SetNotEqual();
                    break;
            }

            void CompareGenericArguments(MethodInfo methodInfo)
            {
                var genericArguments = methodInfo.GetGenericArguments();
                if (methodReference.GenericArguments.Count != genericArguments.Length)
                    _SetNotEqual();

                if (methodReference.GenericArguments.OfType<GenericMethodParameterReference>().Any(genericParameter => genericParameter.DeclaringMethod == methodReference))
                {
                    if (!methodInfo.IsGenericMethodDefinition)
                        _SetNotEqual();
                }
                else
                    foreach (var (genericArgumentReference, genericArgument) in methodReference
                        .GenericArguments
                        .Zip(genericArguments, (genericArgumentReference, genericArgument) => (genericArgumentReference, genericArgument))
                        .TakeWhile(pair => AreEqual))
                    {
                        _current = genericArgument;
                        genericArgumentReference.Accept(this);
                    }
            }
        }

        protected internal override void VisitAssembly(AssemblyReference assembly)
        {
            AreEqual = false;
        }

        private bool _ShouldStopComparing(MemberReference memberReference)
        {
            if (!AreEqual)
                return true;
            else if (_current == null)
            {
                _SetNotEqual();
                return true;
            }
            else if (_checked.TryGetValue(memberReference, out var checkedMemberInfo))
            {
                if (checkedMemberInfo != _current)
                    _SetNotEqual();
                return true;
            }
            else
                return false;
        }

        private void _SetNotEqual()
        {
            _current = null;
            AreEqual = false;
        }
    }
}