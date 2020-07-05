using System.Collections;

namespace CodeMap.Documentation.Helpers
{
    public class HasAny : HandlebarsBooleanHelper<IEnumerable>
    {
        public override string Name
            => nameof(HasAny);

        public override bool Apply(PageContext context, IEnumerable enumerable)
            => enumerable != null && enumerable.GetEnumerator().MoveNext();
    }
}