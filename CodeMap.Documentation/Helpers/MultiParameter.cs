using System;
using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet;

namespace CodeMap.Documentation.Helpers
{
    public class MultiParameter
    {
        public static TParameter TryGet<TParameter>(object value)
        {
            if (HandlebarsUtils.IsUndefinedBindingResult(value))
                return default;
            else if (typeof(TParameter).IsGenericType
                    && (typeof(TParameter).GetGenericTypeDefinition() == typeof(MultiParameter<,>)
                        || typeof(TParameter).GetGenericTypeDefinition() == typeof(MultiParameter<,,>)))
                return (TParameter)typeof(TParameter).GetConstructor(new[] { typeof(object) }).Invoke(new[] { value });
            else
                return (TParameter)value;
        }

        private readonly object _parameter;
        private readonly IEnumerable<Type> _types;

        public MultiParameter(object parameter, IEnumerable<Type> types)
        {
            _parameter = parameter;
            _types = types;

            if (_types.All(type => !type.IsAssignableFrom(parameter.GetType())))
                throw new ArgumentException($"Unhandled parameter type: '{_parameter.GetType().Name}'");
        }

        public MultiParameter(object parameter, params Type[] types)
            : this(parameter, types.AsEnumerable())
        {
        }


        public MultiParameter Handle<TParameter>(Action<TParameter> callback)
        {
            if (_types.All(type => !type.IsAssignableFrom(typeof(TParameter))))
                throw new ArgumentException($"Unknown parameter type: '{typeof(TParameter).Name}'");
            else if (typeof(TParameter).IsAssignableFrom(_parameter.GetType()))
                callback((TParameter)_parameter);

            return this;
        }
    }

    public class MultiParameter<TParameter1, TParameter2> : MultiParameter
    {
        public MultiParameter(object parameter)
            : base(parameter, typeof(TParameter1), typeof(TParameter2))
        {
        }
    }

    public class MultiParameter<TParameter1, TParameter2, TParameter3> : MultiParameter
    {
        public MultiParameter(object parameter)
            : base(parameter, typeof(TParameter1), typeof(TParameter2), typeof(TParameter3))
        {
        }
    }
}