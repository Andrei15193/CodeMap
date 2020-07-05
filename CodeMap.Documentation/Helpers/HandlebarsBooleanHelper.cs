using System.IO;
using System.Linq;
using HandlebarsDotNet;

namespace CodeMap.Documentation.Helpers
{
    public abstract class HandlebarsBooleanHelper : IHandlebarsHelper
    {
        public abstract string Name { get; }

        public abstract bool Apply(PageContext context);

        void IHandlebarsHelper.Apply(TextWriter writer, dynamic context, params object[] parameters)
        {
            if (Apply((PageContext)parameters[0]))
                writer.Write(true);
        }
    }

    public abstract class HandlebarsBooleanHelper<TParameter> : IHandlebarsHelper
    {
        public abstract string Name { get; }

        public abstract bool Apply(PageContext context, TParameter parameter);

        void IHandlebarsHelper.Apply(TextWriter writer, dynamic context, params object[] parameters)
        {
            if (Apply(
                    (PageContext)parameters[0],
                    HandlebarsUtils.IsUndefinedBindingResult(parameters.ElementAtOrDefault(1)) ? default : (TParameter)parameters.ElementAtOrDefault(1)))
                writer.Write(true);
        }
    }

    public abstract class HandlebarsBooleanHelper<TParameter1, TParameter2> : IHandlebarsHelper
    {
        public abstract string Name { get; }

        public abstract bool Apply(PageContext context, TParameter1 parameter1, TParameter2 parameter2);

        void IHandlebarsHelper.Apply(TextWriter writer, dynamic context, params object[] parameters)
        {
            if (Apply(
                    (PageContext)parameters[0],
                    HandlebarsUtils.IsUndefinedBindingResult(parameters.ElementAtOrDefault(1)) ? default : (TParameter1)parameters.ElementAtOrDefault(1),
                    HandlebarsUtils.IsUndefinedBindingResult(parameters.ElementAtOrDefault(2)) ? default : (TParameter2)parameters.ElementAtOrDefault(2)))
                writer.Write(true);
        }
    }

    public abstract class HandlebarsBooleanHelper<TParameter1, TParameter2, TParameter3> : IHandlebarsHelper
    {
        public abstract string Name { get; }

        public abstract bool Apply(PageContext context, TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3);

        void IHandlebarsHelper.Apply(TextWriter writer, dynamic context, params object[] parameters)
        {
            if (Apply(
                    (PageContext)parameters[0],
                    HandlebarsUtils.IsUndefinedBindingResult(parameters.ElementAtOrDefault(1)) ? default : (TParameter1)parameters.ElementAtOrDefault(1),
                    HandlebarsUtils.IsUndefinedBindingResult(parameters.ElementAtOrDefault(2)) ? default : (TParameter2)parameters.ElementAtOrDefault(2),
                    HandlebarsUtils.IsUndefinedBindingResult(parameters.ElementAtOrDefault(3)) ? default : (TParameter3)parameters.ElementAtOrDefault(3)))
                writer.Write(true);
        }
    }
}