using System;

namespace CodeMap.Tests.Data
{
    /// <summary/>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class TestAttribute : Attribute
    {
        /// <summary/>
        public TestAttribute(object value1)
        {
            Value1 = value1;
        }

        /// <summary/>
        public object Value1 { get; }

        /// <summary/>
        public object Value2 { get; set; }

        /// <summary/>
        public object Value3;
    }
}