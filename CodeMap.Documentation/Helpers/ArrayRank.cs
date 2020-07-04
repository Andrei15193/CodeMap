using System.IO;
using CodeMap.ReferenceData;

namespace CodeMap.Documentation.Helpers
{
    public class ArrayRank : IHandlebarsHelper
    {
        public string Name
            => nameof(ArrayRank);

        public void Apply(TextWriter writer, dynamic context, params object[] parameters)
        {
            if (parameters[0] is ArrayTypeReference arrayTypeReference)
            {
                writer.Write('[');
                writer.Write(new string(',', arrayTypeReference.Rank - 1));
                writer.Write(']');
            }
        }
    }
}