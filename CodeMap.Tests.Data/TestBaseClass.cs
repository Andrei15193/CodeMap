using System;

namespace CodeMap.Tests.Data
{
    /// <summary>A base class declaring members of all kinds that are inherited, overridden and shadowed.</summary>
    public abstract class TestBaseClass
    {
        /// <summary>A protected field that is shadowed in <see cref="TestClass{TParam}"/>.</summary>
        /// <seealso cref="TestClass{TParam}.ShadowedTestField"/>
        protected int ShadowedTestField;

        /// <summary>An event that is shadowed in <see cref="TestClass{TParam}"/>.</summary>
        /// <seealso cref="TestClass{TParam}.ClassShadowedTestEvent"/>
        public event EventHandler ClassShadowedTestEvent;

        /// <summary>An abstract event implemented by <see cref="TestClass{TParam}"/>.</summary>
        public abstract event EventHandler AbstractTestEvent;

        /// <summary>A virtual event overridden by <see cref="TestClass{TParam}"/>.</summary>
        public virtual event EventHandler VirtualTestEvent;

        /// <summary>An abstract property implemented by <see cref="TestClass{TParam}"/></summary>
        public abstract byte AbstractTestProperty { get; set; }

        /// <summary>A virtual property overridden by <see cref="TestClass{TParam}"/>.</summary>
        public virtual string VirtualTestProperty { get; set; }

        /// <summary>A property shadowed in <see cref="TestClass{TParam}"/>.</summary>
        /// <seealso cref="TestClass{TParam}.ClassShadowedTestProperty"/>
        public char ClassShadowedTestProperty { get; set; }

        /// <summary>A static property.</summary>
        public static int StaticTestProperty { get; set; }

        /// <summary>A static method.</summary>
        public static void StaticTestMethod()
        {
        }

        /// <summary>A method shadowed in <see cref="TestClass{TParam}"/>.</summary>
        /// <see cref="TestClass{TParam}.ClassShadowedTestMethod"/>
        public void ClassShadowedTestMethod()
        {
        }

        /// <summary>An abstract method implemented by <see cref="TestClass{TParam}"/>.</summary>
        /// <returns>Returns the state of the object as a serialized JSON, or just an empty string.</returns>
        public abstract string AbstractTestMethod();

        /// <summary>A virtual method overridden by <see cref="TestClass{TParam}"/>.</summary>
        /// <returns>Returns <c>false</c>, always <c>false</c>.</returns>
        protected internal virtual bool VirtualTestMethod()
            => default;
    }
}