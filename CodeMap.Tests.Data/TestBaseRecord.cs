using System;

namespace CodeMap.Tests.Data
{
    /// <summary>A base record declaring members of all kinds that are inherited, overridden and shadowed.</summary>
    /// <param name = "param" > A constructor parameter, with attributes.</param>
    public abstract record TestBaseRecord([Test("record constructor parameter test 1", Value2 = "record constructor parameter test 2", Value3 = "record constructor parameter test 3")] int param)
    {
        /// <summary>A public constant that is shadowed in <see cref="TestRecord{TParam}"/>.</summary>
        /// <seealso cref="TestRecord{TParam}.RecordShadowedTestConstant"/>
        public const float RecordShadowedTestConstant = 1;

        /// <summary>A protected field that is shadowed in <see cref="TestRecord{TParam}"/>.</summary>
        /// <seealso cref="TestRecord{TParam}.ShadowedTestField"/>
        protected int ShadowedTestField;

        /// <summary>An event that is shadowed in <see cref="TestRecord{TParam}"/>.</summary>
        /// <seealso cref="TestRecord{TParam}.RecordShadowedTestEvent"/>
        public event EventHandler RecordShadowedTestEvent;

        /// <summary>An abstract event implemented by <see cref="TestRecord{TParam}"/>.</summary>
        public abstract event EventHandler AbstractTestEvent;

        /// <summary>A virtual event overridden by <see cref="TestRecord{TParam}"/>.</summary>
        public virtual event EventHandler VirtualTestEvent;

        /// <summary>An abstract property implemented by <see cref="TestRecord{TParam}"/></summary>
        public abstract byte AbstractTestProperty { get; set; }

        /// <summary>A virtual property overridden by <see cref="TestRecord{TParam}"/>.</summary>
        public virtual string VirtualTestProperty { get; set; }

        /// <summary>A property shadowed in <see cref="TestRecord{TParam}"/>.</summary>
        /// <seealso cref="TestRecord{TParam}.RecordShadowedTestProperty"/>
        public char RecordShadowedTestProperty { get; set; }

        /// <summary>A static property.</summary>
        public static int StaticTestProperty { get; set; }

        /// <summary>A static method.</summary>
        public static void StaticTestMethod()
        {
        }

        /// <summary>A method shadowed in <see cref="TestRecord{TParam}"/>.</summary>
        /// <see cref="TestRecord{TParam}.RecordShadowedTestMethod"/>
        public void RecordShadowedTestMethod()
        {
        }

        /// <summary>An abstract method implemented by <see cref="TestRecord{TParam}"/>.</summary>
        /// <returns>Returns the state of the object as a serialized JSON, or just an empty string.</returns>
        public abstract string AbstractTestMethod();

        /// <summary>A virtual method overridden by <see cref="TestRecord{TParam}"/>.</summary>
        /// <returns>Returns <c>false</c>, always <c>false</c>.</returns>
        protected internal virtual bool VirtualTestMethod()
            => default;
    }
}