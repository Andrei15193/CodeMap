using System;

namespace CodeMap.Tests.Data
{
    /// <summary/>
    public abstract class TestBaseClass
    {
        /// <summary/>
        protected int ShadowedTestField;

        /// <summary/>
        public event EventHandler ClassShadowedTestEvent;

        /// <summary/>
        public abstract event EventHandler AbstractTestEvent;

        /// <summary/>
        public virtual event EventHandler VirtualTestEvent;

        /// <summary/>
        public abstract byte AbstractTestProperty { get; set; }

        /// <summary/>
        public virtual string VirtualTestProperty { get; set; }

        /// <summary/>
        public char ClassShadowedTestProperty { get; set; }

        /// <summary/>
        public static int StaticTestProperty { get; set; }

        /// <summary/>
        public static void StaticTestMethod()
        {
        }

        /// <summary/>
        public void ClassShadowedTestMethod()
        {
        }

        /// <summary/>
        public abstract string AbstractTestMethod();

        /// <summary/>
        protected internal virtual bool VirtualTestMethod()
            => default;
    }
}