using System;

namespace CodeMap.Tests.TestData
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    internal class TestAttribute : Attribute
    {
        public TestAttribute(object value1)
        {
            Value1 = value1;
        }

        public object Value1 { get; }

        public object Value2 { get; set; }

        public object Value3;
    }
}