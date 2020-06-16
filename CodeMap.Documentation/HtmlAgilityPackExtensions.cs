using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeMap.Documentation
{
    public static class HtmlAgilityPackExtensions
    {
        public static HtmlNode Apply(this HtmlNode htmlNode, Action<HtmlNode> callback)
        {
            callback(htmlNode);
            return htmlNode;
        }

        public static HtmlNode Aggregate<T>(this HtmlNode htmlNode, IEnumerable<T> items, Action<T, HtmlNode> reducer)
            => items.Aggregate(htmlNode, (result, item) => { reducer(item, result); return result; });

        public static HtmlNode AddDoctype(this HtmlNode htmlNode)
            => htmlNode.AddChild(htmlNode.OwnerDocument.CreateComment("<!DOCTYPE html>")).ParentNode;

        public static HtmlNode AddChild(this HtmlNode htmlNode, string elementName)
            => htmlNode.AddChild(htmlNode.OwnerDocument.CreateElement(elementName));

        public static HtmlNode AddChild(this HtmlNode htmlNode, HtmlNode childHtmlNode)
            => htmlNode.AppendChild(childHtmlNode);

        public static HtmlNode AppendText(this HtmlNode htmlNode, string text)
            => htmlNode.AppendChild(htmlNode.OwnerDocument.CreateTextNode(text)).ParentNode;

        public static HtmlNode SetClass(this HtmlNode htmlNode, string classes)
            => htmlNode.SetAttribute("class", classes);

        public static HtmlNode SetAttribute(this HtmlNode htmlNode, string name, string value)
            => htmlNode.SetAttributeValue(name, value).OwnerNode;
    }
}