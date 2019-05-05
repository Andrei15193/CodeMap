﻿using CodeMap.ReferenceData;
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
            await jsonWriter.WritePropertyNameAsync(propertyName, cancellationToken);
            if (memberReference != null)
                await memberReference.AcceptAsync(visitor, cancellationToken);
            else
                await jsonWriter.WriteNullAsync(cancellationToken);
        }

        public static void WriteProperty(this JsonWriter jsonWriter, string propertyName, MemberReferenceVisitor visitor, IEnumerable<MemberReference> memberReferences)
        {
            jsonWriter.WritePropertyName(propertyName);
            jsonWriter.WriteStartArray();
            foreach (var memberReference in memberReferences)
                memberReference.Accept(visitor);
            jsonWriter.WriteEndArray();
        }

        public static async Task WritePropertyCollectionAsync<T>(this JsonWriter jsonWriter, string propertyName, MemberReferenceVisitor visitor, IEnumerable<MemberReference> memberReferences, CancellationToken cancellationToken)
        {
            await jsonWriter.WritePropertyNameAsync(propertyName, cancellationToken);
            await jsonWriter.WriteStartArrayAsync(cancellationToken);
            foreach (var item in memberReferences)
                await item.AcceptAsync(visitor, cancellationToken);
            await jsonWriter.WriteEndArrayAsync(cancellationToken);
        }
    }
}