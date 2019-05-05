using CodeMap.ReferenceData;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMap
{
    internal static class JsonWriterMemberReferenceExtensions
    {
        public static void WriteProperty(this JsonWriter jsonWriter, string propertyName, object value)
        {
            jsonWriter.WritePropertyName(propertyName);
            jsonWriter.WriteValue(value);
        }

        public static async Task WritePropertyAsync(this JsonWriter jsonWriter, string propertyName, object value, CancellationToken cancellationToken)
        {
            await jsonWriter.WritePropertyNameAsync(propertyName, cancellationToken).ConfigureAwait(false);
            await jsonWriter.WriteValueAsync(value, cancellationToken).ConfigureAwait(false);
        }

        public static void WriteProperty(this JsonWriter jsonWriter, string propertyName, MemberReferenceVisitor visitor, MemberReference memberReference)
        {
            jsonWriter.WritePropertyName(propertyName);
            if (memberReference != null)
                memberReference.Accept(visitor);
            else
                jsonWriter.WriteNull();
        }

        public static async Task WritePropertyAsync(this JsonWriter jsonWriter, string propertyName, MemberReferenceVisitor visitor, MemberReference memberReference, CancellationToken cancellationToken)
        {
            await jsonWriter.WritePropertyNameAsync(propertyName, cancellationToken).ConfigureAwait(false);
            if (memberReference != null)
                await memberReference.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
            else
                await jsonWriter.WriteNullAsync(cancellationToken).ConfigureAwait(false);
        }

        public static void WriteProperty(this JsonWriter jsonWriter, string propertyName, MemberReferenceVisitor visitor, IEnumerable<MemberReference> memberReferences)
        {
            jsonWriter.WritePropertyName(propertyName);
            jsonWriter.WriteStartArray();
            foreach (var memberReference in memberReferences)
                memberReference.Accept(visitor);
            jsonWriter.WriteEndArray();
        }

        public static async Task WritePropertyAsync(this JsonWriter jsonWriter, string propertyName, MemberReferenceVisitor visitor, IEnumerable<MemberReference> memberReferences, CancellationToken cancellationToken)
        {
            await jsonWriter.WritePropertyNameAsync(propertyName, cancellationToken).ConfigureAwait(false);
            await jsonWriter.WriteStartArrayAsync(cancellationToken).ConfigureAwait(false);
            foreach (var item in memberReferences)
                await item.AcceptAsync(visitor, cancellationToken).ConfigureAwait(false);
            await jsonWriter.WriteEndArrayAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}