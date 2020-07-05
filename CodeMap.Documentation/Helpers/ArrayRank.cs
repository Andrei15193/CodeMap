using System.IO;
using CodeMap.ReferenceData;

namespace CodeMap.Documentation.Helpers
{
    public class ArrayRank : HandlebarsContextualHelper<object>
    {
        public override string Name
            => nameof(ArrayRank);

        public override void Apply(TextWriter writer, PageContext context, object parameter)
        {
            if (parameter is ArrayTypeReference arrayTypeReference)
            {
                writer.Write('[');
                writer.Write(new string(',', arrayTypeReference.Rank - 1));
                writer.Write(']');
            }
        }
    }
}