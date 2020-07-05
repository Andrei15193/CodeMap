namespace CodeMap.Documentation
{
    public class PageContextWithData<TData> : PageContext
    {
        public PageContextWithData(PageContext pageContext, TData data)
            : base(pageContext)
            => Data = data;

        public TData Data { get; }
    }
}