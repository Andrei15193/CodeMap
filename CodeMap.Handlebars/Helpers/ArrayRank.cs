using System.IO;
using System.Linq;
using CodeMap.ReferenceData;

namespace CodeMap.Handlebars.Helpers
{
    public class ArrayRank : IHandlebarsHelper
    {
        public string Name
            => nameof(ArrayRank);

        public void Apply(TextWriter writer, object context, params object[] parameters)
        {
            if (parameters.DefaultIfEmpty(context).First() is ArrayTypeReference arrayTypeReference)
            {
                writer.Write('[');
                writer.Write(new string(',', arrayTypeReference.Rank - 1));
                writer.Write(']');
            }
        }
    }
}