using CodeMap.ReferenceData;

namespace CodeMap.Elements
{
    /// <summary>Represents a documented exception that might be thrown by a method.</summary>
    public sealed class ExceptionData
    {
        internal ExceptionData()
        {
        }

        /// <summary>The type of the exception.</summary>
        public TypeReference Type { get; internal set; }

        /// <summary>The description of the cases where the exception is thrown.</summary>
        public BlockDescriptionDocumentationElement Description { get; internal set; }
    }
}