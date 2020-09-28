using System.IO;
using HandlebarsDotNet;

namespace CodeMap.Handlebars.Helpers
{
    public interface IHandlebarsBlockHelper
    {
        string Name { get; }

        void Apply(TextWriter writer, HelperOptions options, object context, params object[] parameters);
    }
}