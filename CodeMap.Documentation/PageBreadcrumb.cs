namespace CodeMap.Documentation
{
    public class PageBreadcrumb
    {
        public PageBreadcrumb(string text)
            => Text = text;

        public PageBreadcrumb(string text, string url)
            => (Text, Url) = (text, url);

        public string Text { get; }

        public string Url { get; }
    }
}