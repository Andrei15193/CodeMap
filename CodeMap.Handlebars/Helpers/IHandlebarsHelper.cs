using System.IO;

namespace CodeMap.Handlebars.Helpers
{
    public interface IHandlebarsHelper
    {
        string Name { get; }

        void Apply(TextWriter writer, object context, params object[] parameters);
    }
}