using HandlebarsDotNet;
using System.IO;

namespace CodeMap.Documentation.Helpers
{
    public interface IHandlebarsBlockHelper
    {
        string Name { get; }

        void Apply(TextWriter writer, HelperOptions options, dynamic context, params object[] parameters);
    }
}