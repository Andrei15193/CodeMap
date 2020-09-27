using System;
using System.IO;
using System.Linq;
using CodeMap.ReferenceData;

namespace CodeMap.Documentation.Helpers
{
    public class SimpleMemberName : IHandlebarsHelper
    {
        public string Name
            => nameof(SimpleMemberName);

        public void Apply(TextWriter writer, object context, params object[] parameters)
        {
            var baseTypeReference = parameters.DefaultIfEmpty(context).First() as BaseTypeReference;
            if (baseTypeReference is null)
                throw new ArgumentException("Expected a " + nameof(BaseTypeReference) + " provided as the first parameter or context.");

            if (baseTypeReference is GenericParameterReference genericParameterReference)
                writer.Write(genericParameterReference.Name);
            else
            {
                var typeReference = (TypeReference)(baseTypeReference is TypeReference ? baseTypeReference : ((ArrayTypeReference)baseTypeReference).ItemType);
                if (typeReference == typeof(void))
                    writer.Write("void");
                else if (typeReference == typeof(object))
                    writer.Write("object");
                else if (typeReference == typeof(bool))
                    writer.Write("bool");
                else if (typeReference == typeof(byte))
                    writer.Write("byte");
                else if (typeReference == typeof(sbyte))
                    writer.Write("sbyte");
                else if (typeReference == typeof(short))
                    writer.Write("short");
                else if (typeReference == typeof(ushort))
                    writer.Write("ushort");
                else if (typeReference == typeof(int))
                    writer.Write("int");
                else if (typeReference == typeof(uint))
                    writer.Write("uint");
                else if (typeReference == typeof(long))
                    writer.Write("long");
                else if (typeReference == typeof(float))
                    writer.Write("float");
                else if (typeReference == typeof(double))
                    writer.Write("double");
                else if (typeReference == typeof(decimal))
                    writer.Write("decimal");
                else if (typeReference == typeof(char))
                    writer.Write("char");
                else if (typeReference == typeof(string))
                    writer.Write("string");
                else if (typeReference is DynamicTypeReference)
                    writer.Write("dynamic");
                else
                    writer.Write(typeReference.Name);
            }
        }
    }
}