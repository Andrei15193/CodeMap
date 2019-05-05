using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap
{
    internal static class JsonWriterExtensions
    {
        public static void WriteProperty(this JsonWriter jsonWriter, string propertyName, object value)
        {
            jsonWriter.WritePropertyName(propertyName);
            jsonWriter.WriteValue(value);
        }

        public static async Task WritePropertyAsync(this JsonWriter jsonWriter, string propertyName, object value, CancellationToken cancellationToken)
        {
            await jsonWriter.WritePropertyNameAsync(propertyName, cancellationToken);
            await jsonWriter.WriteValueAsync(value, cancellationToken);
        }

        public static void WritePropertyIfNotNull<T>(this JsonWriter jsonWriter, string propertyName, T value, Action<T> valueWriterCallback)
        {
            jsonWriter.WritePropertyName(propertyName);
            if (value != null)
                valueWriterCallback(value);
            else
                jsonWriter.WriteNull();
        }

        public static async Task WritePropertyIfNotNullAsync<T>(this JsonWriter jsonWriter, string propertyName, T value, Func<T, Task> valueWriterCallback, CancellationToken cancellationToken)
        {
            await jsonWriter.WritePropertyNameAsync(propertyName, cancellationToken);
            if (value != null)
                await valueWriterCallback(value);
            else
                await jsonWriter.WriteNullAsync(cancellationToken);
        }

        public static void WritePropertyCollection<T>(this JsonWriter jsonWriter, string propertyName, IEnumerable<T> collection, Action<T> itemWriterCallback)
        {
            jsonWriter.WritePropertyName(propertyName);
            jsonWriter.WriteStartArray();
            foreach (var item in collection)
                itemWriterCallback(item);
            jsonWriter.WriteEndArray();
        }

        public static async Task WritePropertyCollectionAsync<T>(this JsonWriter jsonWriter, string propertyName, IEnumerable<T> collection, Func<T, Task> itemWriterCallback, CancellationToken cancellationToken)
        {
            await jsonWriter.WritePropertyNameAsync(propertyName, cancellationToken);
            await jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var item in collection)
                await itemWriterCallback(item);
            await jsonWriter.WriteEndArrayAsync(cancellationToken);
        }
    }
}