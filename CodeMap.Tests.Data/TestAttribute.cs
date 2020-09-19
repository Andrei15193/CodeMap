using System;

namespace CodeMap.Tests.Data
{
    /// <summary>A test attribute that is used all over the place in the test data.</summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class TestAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="TestAttribute"/> class.</summary>
        /// <param name="value1">A positional parameter.</param>
        public TestAttribute(object value1)
        {
            Value1 = value1;
        }

        /// <summary>Gets the value of the provided positional parameter.</summary>
        public object Value1 { get; }

        /// <summary>A named parameter, declared as a property.</summary>
        public object Value2 { get; set; }

        /// <summary>A named parameter, declared as a field.</summary>
        public object Value3;
    }
}