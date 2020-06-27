using System.IO;

namespace CodeMap.Documentation.Helpers
{
    public interface IHandlebarsHelper
    {
        string Name { get; }

        void Apply(TextWriter writer, dynamic context, params object[] parameters);
    }
}