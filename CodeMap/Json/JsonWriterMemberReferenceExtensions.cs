using System.Collections.Generic;
using CodeMap.ReferenceData;
using Newtonsoft.Json;

namespace CodeMap.Json
{
    internal static class JsonWriterMemberReferenceExtensions
    {
        public static void WriteProperty(this JsonWriter jsonWriter, string propertyName, object value)
        {
            jsonWriter.WritePropertyName(propertyName);
            jsonWriter.WriteValue(value);
        }

        public static void WriteProperty(this JsonWriter jsonWriter, string propertyName, MemberReferenceVisitor visitor, MemberReference memberReference)
        {
            jsonWriter.WritePropertyName(propertyName);
            if (memberReference != null)
                memberReference.Accept(visitor);
            else
                jsonWriter.WriteNull();
        }

        public static void WriteProperty(this JsonWriter jsonWriter, string propertyName, MemberReferenceVisitor visitor, AssemblyReference assemblyReference)
        {
            jsonWriter.WritePropertyName(propertyName);
            if (!(assemblyReference is null))
                assemblyReference.Accept(visitor);
            else
                jsonWriter.WriteNull();
        }

        public static void WriteProperty(this JsonWriter jsonWriter, string propertyName, MemberReferenceVisitor visitor, IEnumerable<MemberReference> memberReferences)
        {
            jsonWriter.WritePropertyName(propertyName);
            jsonWriter.WriteStartArray();
            foreach (var memberReference in memberReferences)
                memberReference.Accept(visitor);
            jsonWriter.WriteEndArray();
        }
    }
}