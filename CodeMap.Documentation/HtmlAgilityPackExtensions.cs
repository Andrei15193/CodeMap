using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace CodeMap.Documentation
{
    public static class HtmlAgilityPackExtensions
    {
        public static HtmlNode Aggregate<T>(this HtmlNode htmlNode, IEnumerable<T> items, Action<HtmlNode, T> reducer)
        {
            foreach (var item in items)
                reducer(htmlNode, item);
            return htmlNode;
        }

        public static HtmlNode AddChild(this HtmlNode htmlNode, string elementName)
            => htmlNode.AddChild(htmlNode.OwnerDocument.CreateElement(elementName));

        public static HtmlNode AddChild(this HtmlNode htmlNode, HtmlNode childHtmlNode)
            => htmlNode.AppendChild(childHtmlNode);

        public static HtmlNode AppendText(this HtmlNode htmlNode, string text)
            => htmlNode.AppendChild(htmlNode.OwnerDocument.CreateTextNode(HtmlDocument.HtmlEncode(text))).ParentNode;

        public static HtmlNode SetClass(this HtmlNode htmlNode, string classes)
            => htmlNode.SetAttribute("class", classes);

        public static HtmlNode SetAttribute(this HtmlNode htmlNode, string name, string value)
            => htmlNode.SetAttributeValue(name, value).OwnerNode;
    }
}