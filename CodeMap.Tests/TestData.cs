using System;
using System.Reflection;

[assembly: AssemblyVersion("1.2.3.4")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyDescription("This is a test")]

namespace CodeMap.Tests
{
    /// <summary>TestEnum summary.</summary>
    internal enum TestEnum
    {
        /// <summary>Enum test member 1.</summary>
        TestMember1,
        /// <summary>Enum test member 2.</summary>
        TestMember2,
        /// <summary>Enum test member 3.</summary>
        TestMember3,
    }

    /// <summary>TestDelegate summary.</summary>
    internal delegate void TestDelegate();

    /// <summary>TestDelegate summary.</summary>
    internal delegate void TestDelegate<TParam1>();

    /// <summary>ITestInterface summary.</summary>
    internal interface ITestInterface
    {
        /// <summary>TestEvent summary.</summary>
        event EventHandler TestEvent;

        /// <summary>TestProperty summary.</summary>
        string TestProperty { get; set; }

        /// <summary>Indexer summary.</summary>
        int this[string param1] { get; set; }

        /// <summary>Indexer summary.</summary>
        int this[string param1, int param2] { get; set; }

        /// <summary>TestMethod summary 1.</summary>
        object TestMethod();

        /// <summary>TestMethod summary 2.</summary>
        object TestMethod(int param1);

        /// <summary>TestMethod summary 3.</summary>
        object TestMethod(int param1, string param2);
    }

    /// <summary>TestClass summary.</summary>
    internal class TestClass
    {
        /// <summary>_staticTestField summary.</summary>
        private static readonly double _staticTestField;

        /// <summary>StaticTestClass constructor summary.</summary>
        static TestClass()
        {
            _staticTestField = 1;
        }

        /// <summary>StaticTestEvent summary.</summary>
        private static event EventHandler StaticTestEvent;

        /// <summary>StaticTestProperty summary.</summary>
        private static double StaticTestProperty
        {
            get => _staticTestField;
            set => throw new NotImplementedException();
        }

        /// <summary>StaticTestMethod summary 1.</summary>
        private static object StaticTestMethod()
        {
            StaticTestEvent(null, EventArgs.Empty);
            throw new NotImplementedException();
        }

        /// <summary>StaticTestMethod summary 2.</summary>
        private static object StaticTestMethod(int param1)
        {
            StaticTestEvent(null, EventArgs.Empty);
            throw new NotImplementedException();
        }

        /// <summary>StaticTestMethod summary 3.</summary>
        private static object StaticTestMethod(int param1, string param2)
        {
            StaticTestEvent(null, EventArgs.Empty);
            throw new NotImplementedException();
        }

        /// <summary>StaticTestMethod summary 4.</summary>
        private static object StaticTestMethod<TParam1>(int param1, string param2)
        {
            StaticTestEvent(null, EventArgs.Empty);
            throw new NotImplementedException();
        }

        /// <summary>StaticTestMethod summary 5.</summary>
        private static object StaticTestMethod<TParam1, TParam2>()
        {
            StaticTestEvent(null, EventArgs.Empty);
            throw new NotImplementedException();
        }

        /// <summary>TestEnum summary.</summary>
        private enum NestedTestEnum
        {
            /// <summary>Enum test member 1.</summary>
            TestMember1,
            /// <summary>Enum test member 2.</summary>
            TestMember2,
            /// <summary>Enum test member 3.</summary>
            TestMember3,
        }

        /// <summary>TestDelegate summary.</summary>
        private delegate void NestedTestDelegate();

        /// <summary>TestDelegate summary.</summary>
        private delegate void NestedTestDelegate<TParam1>();

        /// <summary>ITestInterface summary.</summary>
        private interface INestedTestInterface
        {
            /// <summary>TestEvent summary.</summary>
            event EventHandler TestEvent;

            /// <summary>TestProperty summary.</summary>
            string TestProperty { get; set; }

            /// <summary>Indexer summary.</summary>
            int this[string param1] { get; set; }

            /// <summary>Indexer summary.</summary>
            int this[string param1, int param2] { get; set; }

            /// <summary>TestMethod summary 1.</summary>
            object TestMethod();

            /// <summary>TestMethod summary 2.</summary>
            object TestMethod(int param1);

            /// <summary>TestMethod summary 3.</summary>
            object TestMethod(int param1, string param2);
        }

        /// <summary>TestClass summary.</summary>
        private class NestedTestClass
        {
            /// <summary>_staticTestField summary.</summary>
            private static readonly double _staticTestField;

            /// <summary>Static NestedTestClass constructor summary.</summary>
            static NestedTestClass()
            {
                _staticTestField = 1;
            }

            /// <summary>StaticTestEvent summary.</summary>
            private static event EventHandler StaticTestEvent;

            /// <summary>StaticTestProperty summary.</summary>
            private static double StaticTestProperty
            {
                get => _staticTestField;
                set => throw new InvalidOperationException();
            }

            /// <summary>StaticTestMethod summary 1.</summary>
            private static object StaticTestMethod()
            {
                StaticTestEvent(null, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>StaticTestMethod summary 2.</summary>
            private static object StaticTestMethod(int param1)
            {
                StaticTestEvent(null, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>StaticTestMethod summary 3.</summary>
            private static object StaticTestMethod(int param1, string param2)
            {
                StaticTestEvent(null, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>StaticTestMethod summary 4.</summary>
            private static object StaticTestMethod<TParam1>(int param1, string param2)
            {
                StaticTestEvent(null, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>StaticTestMethod summary 5.</summary>
            private static object StaticTestMethod<TParam1, TParam2>()
            {
                StaticTestEvent(null, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>_testField summary.</summary>
            private readonly double _testField = 1;

            /// <summary>NestedTestClass constructor summary.</summary>
            private NestedTestClass()
            {
            }

            /// <summary>NestedTestClass constructor summary.</summary>
            private NestedTestClass(double param1)
            {
            }

            /// <summary>NestedTestClass constructor summary.</summary>
            private NestedTestClass(double param1, int param2)
            {
            }

            /// <summary>TestEvent summary.</summary>
            private event EventHandler TestEvent;

            /// <summary>TestProperty summary.</summary>
            private double TestProperty
            {
                get => _testField;
                set => throw new InvalidOperationException();
            }

            /// <summary>Indexer summary.</summary>
            private int this[string param1]
            {
                get => throw new NotImplementedException();
                set
                {
                    TestEvent(this, EventArgs.Empty);
                    throw new NotImplementedException();
                }
            }

            /// <summary>Indexer summary.</summary>
            private int this[string param1, int param2]
            {
                get => throw new NotImplementedException();
                set
                {
                    TestEvent(this, EventArgs.Empty);
                    throw new NotImplementedException();
                }
            }

            /// <summary>TestMethod summary 1.</summary>
            private object TestMethod()
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>TestMethod summary 2.</summary>
            private object TestMethod(int param1)
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>TestMethod summary 3.</summary>
            private object TestMethod(int param1, string param2)
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>TestMethod summary 4.</summary>
            private object TestMethod<TParam1>(int param1, string param2)
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>TestMethod summary 5.</summary>
            private object TestMethod<TParam1, TParam2>()
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }
        }

        /// <summary>TestClass summary.</summary>
        private class NestedTestClass<TParam1>
        {
            /// <summary>_staticTestField summary.</summary>
            private static readonly double _staticTestField;

            /// <summary>Static NestedTestClass constructor summary.</summary>
            static NestedTestClass()
            {
                _staticTestField = 1;
            }

            /// <summary>StaticTestEvent summary.</summary>
            private static event EventHandler StaticTestEvent;

            /// <summary>StaticTestProperty summary.</summary>
            private static double StaticTestProperty
            {
                get => _staticTestField;
                set => throw new InvalidOperationException();
            }

            /// <summary>StaticTestMethod summary 1.</summary>
            private static object StaticTestMethod()
            {
                StaticTestEvent(null, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>StaticTestMethod summary 2.</summary>
            private static object StaticTestMethod(int param1)
            {
                StaticTestEvent(null, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>StaticTestMethod summary 3.</summary>
            private static object StaticTestMethod(int param1, string param2)
            {
                StaticTestEvent(null, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>StaticTestMethod summary 4.</summary>
            private static object StaticTestMethod<TParam2>(int param1, string param2)
            {
                StaticTestEvent(null, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>StaticTestMethod summary 5.</summary>
            private static object StaticTestMethod<TParam2, TParam3>()
            {
                StaticTestEvent(null, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>_testField summary.</summary>
            private readonly double _testField = 1;

            /// <summary>NestedTestClass constructor summary.</summary>
            private NestedTestClass()
            {
            }

            /// <summary>NestedTestClass constructor summary.</summary>
            private NestedTestClass(double param1)
            {
            }

            /// <summary>NestedTestClass constructor summary.</summary>
            private NestedTestClass(double param1, int param2)
            {
            }

            /// <summary>TestEvent summary.</summary>
            private event EventHandler TestEvent;

            /// <summary>TestProperty summary.</summary>
            private double TestProperty
            {
                get => _testField;
                set => throw new InvalidOperationException();
            }

            /// <summary>Indexer summary.</summary>
            private int this[string param1]
            {
                get => throw new NotImplementedException();
                set
                {
                    TestEvent(this, EventArgs.Empty);
                    throw new NotImplementedException();
                }
            }

            /// <summary>Indexer summary.</summary>
            private int this[string param1, int param2]
            {
                get => throw new NotImplementedException();
                set
                {
                    TestEvent(this, EventArgs.Empty);
                    throw new NotImplementedException();
                }
            }

            /// <summary>TestMethod summary 1.</summary>
            private object TestMethod()
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>TestMethod summary 2.</summary>
            private object TestMethod(int param1)
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>TestMethod summary 3.</summary>
            private object TestMethod(int param1, string param2)
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>TestMethod summary 4.</summary>
            private object TestMethod<TParam2>(int param1, string param2)
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>TestMethod summary 5.</summary>
            private object TestMethod<TParam2, TParam3>()
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }
        }

        /// <summary>TestStruct summary.</summary>
        private class NestedTestStruct
        {
            /// <summary>_staticTestField summary.</summary>
            private static readonly double _staticTestField;

            /// <summary>Static NestedTestStruct constructor summary.</summary>
            static NestedTestStruct()
            {
                _staticTestField = 1;
            }

            /// <summary>StaticTestEvent summary.</summary>
            private static event EventHandler StaticTestEvent;

            /// <summary>Static estProperty summary.</summary>
            private static double StaticTestProperty
            {
                get => _staticTestField;
                set => throw new InvalidOperationException();
            }

            /// <summary>StaticTestMethod summary 1.</summary>
            private static object StaticTestMethod()
            {
                StaticTestEvent(null, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>StaticTestMethod summary 2.</summary>
            private static object StaticTestMethod(int param1)
            {
                StaticTestEvent(null, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>StaticTestMethod summary 3.</summary>
            private static object StaticTestMethod(int param1, string param2)
            {
                StaticTestEvent(null, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>TestField summary.</summary>
            private readonly double _testField = 1;

            /// <summary>NestedTestStruct constructor summary.</summary>
            private NestedTestStruct(double param1)
            {
            }

            /// <summary>NestedTestStruct constructor summary.</summary>
            private NestedTestStruct(double param1, int param2)
            {
            }

            /// <summary>TestEvent summary.</summary>
            private event EventHandler TestEvent;

            /// <summary>TestProperty summary.</summary>
            private double TestProperty
            {
                get => _testField;
                set => throw new InvalidOperationException();
            }

            /// <summary>Indexer summary.</summary>
            private int this[string param1]
            {
                get => throw new NotImplementedException();
                set
                {
                    TestEvent(this, EventArgs.Empty);
                    throw new NotImplementedException();
                }
            }

            /// <summary>Indexer summary.</summary>
            private int this[string param1, int param2]
            {
                get => throw new NotImplementedException();
                set
                {
                    TestEvent(this, EventArgs.Empty);
                    throw new NotImplementedException();
                }
            }

            /// <summary>TestMethod summary 1.</summary>
            private object TestMethod()
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>TestMethod summary 2.</summary>
            private object TestMethod(int param1)
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>TestMethod summary 3.</summary>
            private object TestMethod(int param1, string param2)
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }
        }

        /// <summary>TestField summary.</summary>
        private readonly double _testField = 1;

        /// <summary>TestClass constructor summary.</summary>
        private TestClass()
        {
        }

        /// <summary>TestClass constructor summary.</summary>
        private TestClass(double param1)
        {
        }

        /// <summary>TestClass constructor summary.</summary>
        private TestClass(double param1, int param2)
        {
        }

        /// <summary>TestEvent summary.</summary>
        private event EventHandler TestEvent;

        /// <summary>TestProperty summary.</summary>
        private double TestProperty
        {
            get => _testField;
            set => throw new InvalidOperationException();
        }

        /// <summary>Indexer summary.</summary>
        private int this[string param1]
        {
            get => throw new NotImplementedException();
            set
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }
        }

        /// <summary>Indexer summary.</summary>
        private int this[string param1, int param2]
        {
            get => throw new NotImplementedException();
            set
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }
        }

        /// <summary>TestMethod summary 1.</summary>
        private object TestMethod()
        {
            TestEvent(this, EventArgs.Empty);
            throw new NotImplementedException();
        }

        /// <summary>TestMethod summary 2.</summary>
        private object TestMethod(int param1)
        {
            TestEvent(this, EventArgs.Empty);
            throw new NotImplementedException();
        }

        /// <summary>TestMethod summary 3.</summary>
        private object TestMethod(int param1, string param2)
        {
            TestEvent(this, EventArgs.Empty);
            throw new NotImplementedException();
        }

        /// <summary>TestMethod summary 4.</summary>
        private object TestMethod<TParam1>(int param1, string param2)
        {
            TestEvent(this, EventArgs.Empty);
            throw new NotImplementedException();
        }

        /// <summary>TestMethod summary 5.</summary>
        private object TestMethod<TParam1, TParam2>()
        {
            TestEvent(this, EventArgs.Empty);
            throw new NotImplementedException();
        }

        /// <summary>TestMethod summary 6.</summary>
        private object TestMethod<TParam1>(TParam1 param1)
        {
            TestEvent(this, EventArgs.Empty);
            throw new NotImplementedException();
        }
    }

    /// <summary>TestClass summary.</summary>
    internal class TestClass<TParam1>
    {
        /// <summary>_staticTestField summary.</summary>
        private static readonly double _staticTestField;

        /// <summary>StaticTestClass constructor summary.</summary>
        static TestClass()
        {
            _staticTestField = 1;
        }

        /// <summary>StaticTestEvent summary.</summary>
        private static event EventHandler StaticTestEvent;

        /// <summary>StaticTestProperty summary.</summary>
        private static double StaticTestProperty
        {
            get => _staticTestField;
            set => throw new NotImplementedException();
        }

        /// <summary>StaticTestMethod summary 1.</summary>
        private static object StaticTestMethod()
        {
            StaticTestEvent(null, EventArgs.Empty);
            throw new NotImplementedException();
        }

        /// <summary>StaticTestMethod summary 2.</summary>
        private static object StaticTestMethod(int param1)
        {
            StaticTestEvent(null, EventArgs.Empty);
            throw new NotImplementedException();
        }

        /// <summary>StaticTestMethod summary 3.</summary>
        private static object StaticTestMethod(int param1, string param2)
        {
            StaticTestEvent(null, EventArgs.Empty);
            throw new NotImplementedException();
        }

        /// <summary>StaticTestMethod summary 4.</summary>
        private static object StaticTestMethod<TParam2>(int param1, string param2)
        {
            StaticTestEvent(null, EventArgs.Empty);
            throw new NotImplementedException();
        }

        /// <summary>StaticTestMethod summary 5.</summary>
        private static object StaticTestMethod<TParam2, TParam3>()
        {
            StaticTestEvent(null, EventArgs.Empty);
            throw new NotImplementedException();
        }

        /// <summary>TestEnum summary.</summary>
        private enum NestedTestEnum
        {
            /// <summary>Enum test member 1.</summary>
            TestMember1,
            /// <summary>Enum test member 2.</summary>
            TestMember2,
            /// <summary>Enum test member 3.</summary>
            TestMember3,
        }

        /// <summary>TestDelegate summary.</summary>
        private delegate void NestedTestDelegate();

        /// <summary>TestDelegate summary.</summary>
        private delegate void NestedTestDelegate<TParam2>();

        /// <summary>ITestInterface summary.</summary>
        private interface INestedTestInterface
        {
            /// <summary>TestEvent summary.</summary>
            event EventHandler TestEvent;

            /// <summary>TestProperty summary.</summary>
            string TestProperty { get; set; }

            /// <summary>Indexer summary.</summary>
            int this[string param1] { get; set; }

            /// <summary>Indexer summary.</summary>
            int this[string param1, int param2] { get; set; }

            /// <summary>TestMethod summary 1.</summary>
            object TestMethod();

            /// <summary>TestMethod summary 2.</summary>
            object TestMethod(int param1);

            /// <summary>TestMethod summary 3.</summary>
            object TestMethod(int param1, string param2);
        }

        /// <summary>TestClass summary.</summary>
        private class NestedTestClass
        {
            /// <summary>_staticTestField summary.</summary>
            private static readonly double _staticTestField;

            /// <summary>Static NestedTestClass constructor summary.</summary>
            static NestedTestClass()
            {
                _staticTestField = 1;
            }

            /// <summary>StaticTestEvent summary.</summary>
            private static event EventHandler StaticTestEvent;

            /// <summary>StaticTestProperty summary.</summary>
            private static double StaticTestProperty
            {
                get => _staticTestField;
                set => throw new InvalidOperationException();
            }

            /// <summary>StaticTestMethod summary 1.</summary>
            private static object StaticTestMethod()
            {
                StaticTestEvent(null, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>StaticTestMethod summary 2.</summary>
            private static object StaticTestMethod(int param1)
            {
                StaticTestEvent(null, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>StaticTestMethod summary 3.</summary>
            private static object StaticTestMethod(int param1, string param2)
            {
                StaticTestEvent(null, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>_testField summary.</summary>
            private readonly double _testField = 1;

            /// <summary>NestedTestClass constructor summary.</summary>
            private NestedTestClass()
            {
            }

            /// <summary>NestedTestClass constructor summary.</summary>
            private NestedTestClass(double param1)
            {
            }

            /// <summary>NestedTestClass constructor summary.</summary>
            private NestedTestClass(double param1, int param2)
            {
            }

            /// <summary>TestEvent summary.</summary>
            private event EventHandler TestEvent;

            /// <summary>TestProperty summary.</summary>
            private double TestProperty
            {
                get => _testField;
                set => throw new InvalidOperationException();
            }

            /// <summary>Indexer summary.</summary>
            private int this[string param1]
            {
                get => throw new NotImplementedException();
                set
                {
                    TestEvent(this, EventArgs.Empty);
                    throw new NotImplementedException();
                }
            }

            /// <summary>Indexer summary.</summary>
            private int this[string param1, int param2]
            {
                get => throw new NotImplementedException();
                set
                {
                    TestEvent(this, EventArgs.Empty);
                    throw new NotImplementedException();
                }
            }

            /// <summary>TestMethod summary 1.</summary>
            private object TestMethod()
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>TestMethod summary 2.</summary>
            private object TestMethod(int param1)
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>TestMethod summary 3.</summary>
            private object TestMethod(int param1, string param2)
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }
        }

        /// <summary>TestClass summary.</summary>
        private class NestedTestClass<TParam2>
        {
            /// <summary>_staticTestField summary.</summary>
            private static readonly double _staticTestField;

            /// <summary>Static NestedTestClass constructor summary.</summary>
            static NestedTestClass()
            {
                _staticTestField = 1;
            }

            /// <summary>StaticTestEvent summary.</summary>
            private static event EventHandler StaticTestEvent;

            /// <summary>StaticTestProperty summary.</summary>
            private static double StaticTestProperty
            {
                get => _staticTestField;
                set => throw new InvalidOperationException();
            }

            /// <summary>StaticTestMethod summary 1.</summary>
            private static object StaticTestMethod()
            {
                StaticTestEvent(null, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>StaticTestMethod summary 2.</summary>
            private static object StaticTestMethod(int param1)
            {
                StaticTestEvent(null, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>StaticTestMethod summary 3.</summary>
            private static object StaticTestMethod(int param1, string param2)
            {
                StaticTestEvent(null, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>StaticTestMethod summary 4.</summary>
            private static object StaticTestMethod<TParam3>(int param1, string param2)
            {
                StaticTestEvent(null, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>StaticTestMethod summary 5.</summary>
            private static object StaticTestMethod<TParam3, TParam4>()
            {
                StaticTestEvent(null, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>_testField summary.</summary>
            private readonly double _testField = 1;

            /// <summary>NestedTestClass constructor summary.</summary>
            private NestedTestClass()
            {
            }

            /// <summary>NestedTestClass constructor summary.</summary>
            private NestedTestClass(double param1)
            {
            }

            /// <summary>NestedTestClass constructor summary.</summary>
            private NestedTestClass(double param1, int param2)
            {
            }

            /// <summary>TestEvent summary.</summary>
            private event EventHandler TestEvent;

            /// <summary>TestProperty summary.</summary>
            private double TestProperty
            {
                get => _testField;
                set => throw new InvalidOperationException();
            }

            /// <summary>Indexer summary.</summary>
            private int this[string param1]
            {
                get => throw new NotImplementedException();
                set
                {
                    TestEvent(this, EventArgs.Empty);
                    throw new NotImplementedException();
                }
            }

            /// <summary>Indexer summary.</summary>
            private int this[string param1, int param2]
            {
                get => throw new NotImplementedException();
                set
                {
                    TestEvent(this, EventArgs.Empty);
                    throw new NotImplementedException();
                }
            }

            /// <summary>TestMethod summary 1.</summary>
            private object TestMethod()
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>TestMethod summary 2.</summary>
            private object TestMethod(int param1)
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>TestMethod summary 3.</summary>
            private object TestMethod(int param1, string param2)
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>TestMethod summary 4.</summary>
            private object TestMethod<TParam3>(int param1, string param2)
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>TestMethod summary 5.</summary>
            private object TestMethod<TParam3, TParam4>()
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>TestMethod summary 6.</summary>
            private object TestMethod<TParam3>(TParam1 param1, TParam2 param2, TParam3 param3)
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }
        }

        /// <summary>TestStruct summary.</summary>
        private class NestedTestStruct
        {
            /// <summary>_staticTestField summary.</summary>
            private static readonly double _staticTestField;

            /// <summary>Static NestedTestStruct constructor summary.</summary>
            static NestedTestStruct()
            {
                _staticTestField = 1;
            }

            /// <summary>StaticTestEvent summary.</summary>
            private static event EventHandler StaticTestEvent;

            /// <summary>Static estProperty summary.</summary>
            private static double StaticTestProperty
            {
                get => _staticTestField;
                set => throw new InvalidOperationException();
            }

            /// <summary>StaticTestMethod summary 1.</summary>
            private static object StaticTestMethod()
            {
                StaticTestEvent(null, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>StaticTestMethod summary 2.</summary>
            private static object StaticTestMethod(int param1)
            {
                StaticTestEvent(null, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>StaticTestMethod summary 3.</summary>
            private static object StaticTestMethod(int param1, string param2)
            {
                StaticTestEvent(null, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>TestField summary.</summary>
            private readonly double _testField = 1;

            /// <summary>NestedTestStruct constructor summary.</summary>
            private NestedTestStruct(double param1)
            {
            }

            /// <summary>NestedTestStruct constructor summary.</summary>
            private NestedTestStruct(double param1, int param2)
            {
            }

            /// <summary>TestEvent summary.</summary>
            private event EventHandler TestEvent;

            /// <summary>TestProperty summary.</summary>
            private double TestProperty
            {
                get => _testField;
                set => throw new InvalidOperationException();
            }

            /// <summary>Indexer summary.</summary>
            private int this[string param1]
            {
                get => throw new NotImplementedException();
                set
                {
                    TestEvent(this, EventArgs.Empty);
                    throw new NotImplementedException();
                }
            }

            /// <summary>Indexer summary.</summary>
            private int this[string param1, int param2]
            {
                get => throw new NotImplementedException();
                set
                {
                    TestEvent(this, EventArgs.Empty);
                    throw new NotImplementedException();
                }
            }

            /// <summary>TestMethod summary 1.</summary>
            private object TestMethod()
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>TestMethod summary 2.</summary>
            private object TestMethod(int param1)
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }

            /// <summary>TestMethod summary 3.</summary>
            private object TestMethod(int param1, string param2)
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }
        }

        /// <summary>TestField summary.</summary>
        private readonly double _testField = 1;

        /// <summary>TestClass constructor summary.</summary>
        private TestClass()
        {
        }

        /// <summary>TestClass constructor summary.</summary>
        private TestClass(double param1)
        {
        }

        /// <summary>TestClass constructor summary.</summary>
        private TestClass(double param1, int param2)
        {
        }

        /// <summary>TestEvent summary.</summary>
        private event EventHandler TestEvent;

        /// <summary>TestProperty summary.</summary>
        private double TestProperty
        {
            get => _testField;
            set => throw new InvalidOperationException();
        }

        /// <summary>Indexer summary.</summary>
        private int this[string param1]
        {
            get => throw new NotImplementedException();
            set
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }
        }

        /// <summary>Indexer summary.</summary>
        private int this[string param1, int param2]
        {
            get => throw new NotImplementedException();
            set
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }
        }

        /// <summary>Indexer summary.</summary>
        private int this[TParam1 param1]
        {
            get => throw new NotImplementedException();
            set
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }
        }

        /// <summary>TestMethod summary 1.</summary>
        private object TestMethod()
        {
            TestEvent(this, EventArgs.Empty);
            throw new NotImplementedException();
        }

        /// <summary>TestMethod summary 2.</summary>
        private object TestMethod(int param1)
        {
            TestEvent(this, EventArgs.Empty);
            throw new NotImplementedException();
        }

        /// <summary>TestMethod summary 3.</summary>
        private object TestMethod(int param1, string param2)
        {
            TestEvent(this, EventArgs.Empty);
            throw new NotImplementedException();
        }

        /// <summary>TestMethod summary 4.</summary>
        private object TestMethod<TParam2>(int param1, string param2)
        {
            TestEvent(this, EventArgs.Empty);
            throw new NotImplementedException();
        }

        /// <summary>TestMethod summary 5.</summary>
        private object TestMethod<TParam2, TParam3>()
        {
            TestEvent(this, EventArgs.Empty);
            throw new NotImplementedException();
        }

        /// <summary>TestMethod summary 6.</summary>
        private object TestMethod<TParam2>(TParam1 param1, TParam2 param2)
        {
            TestEvent(this, EventArgs.Empty);
            throw new NotImplementedException();
        }
    }

    /// <summary>TestStruct summary.</summary>
    internal class TestStruct
    {
        /// <summary>_staticTestField summary.</summary>
        private static readonly double _staticTestField;

        /// <summary>Static TestStruct constructor summary.</summary>
        static TestStruct()
        {
            _staticTestField = 1;
        }

        /// <summary>StaticTestEvent summary.</summary>
        private static event EventHandler StaticTestEvent;

        /// <summary>StaticTestProperty summary.</summary>
        private static double StaticTestProperty
        {
            get => _staticTestField;
            set => throw new InvalidOperationException();
        }

        /// <summary>StaticTestMethod summary 1.</summary>
        private static object StaticTestMethod()
        {
            StaticTestEvent(null, EventArgs.Empty);
            throw new NotImplementedException();
        }

        /// <summary>StaticTestMethod summary 2.</summary>
        private static object StaticTestMethod(int param1)
        {
            StaticTestEvent(null, EventArgs.Empty);
            throw new NotImplementedException();
        }

        /// <summary>StaticTestMethod summary 3.</summary>
        private static object StaticTestMethod(int param1, string param2)
        {
            StaticTestEvent(null, EventArgs.Empty);
            throw new NotImplementedException();
        }

        /// <summary>_testField summary.</summary>
        private readonly double _testField = 1;

        /// <summary>TestStruct constructor summary.</summary>
        private TestStruct(double param1)
        {
        }

        /// <summary>TestStruct constructor summary.</summary>
        private TestStruct(double param1, int param2)
        {
        }

        /// <summary>TestEvent summary.</summary>
        private event EventHandler TestEvent;

        /// <summary>TestProperty summary.</summary>
        private double TestProperty
        {
            get => _testField;
            set => throw new NotImplementedException();
        }

        /// <summary>Indexer summary.</summary>
        private int this[string param1]
        {
            get => throw new NotImplementedException();
            set
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }
        }

        /// <summary>Indexer summary.</summary>
        private int this[string param1, int param2]
        {
            get => throw new NotImplementedException();
            set
            {
                TestEvent(this, EventArgs.Empty);
                throw new NotImplementedException();
            }
        }

        /// <summary>TestMethod summary 1.</summary>
        private object TestMethod()
        {
            TestEvent(this, EventArgs.Empty);
            throw new NotImplementedException();
        }

        /// <summary>TestMethod summary 2.</summary>
        private object TestMethod(int param1)
        {
            TestEvent(this, EventArgs.Empty);
            throw new NotImplementedException();
        }

        /// <summary>TestMethod summary 3.</summary>
        private object TestMethod(int param1, string param2)
        {
            TestEvent(this, EventArgs.Empty);
            throw new NotImplementedException();
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    internal class TestAttribute : Attribute
    {
        internal TestAttribute(object value)
        {
            Value = value;
        }

        internal object Value { get; }
    }

    [Test("string")]
    [Test(new object[] { "string", 1, true, typeof(string), StringComparison.Ordinal })]
    [return: Test(1)]
    internal delegate void TestDelegate2(string parameter1, int parameter2);

    namespace TestNamespace
    {
        internal class TestClass
        {
        }
    }
}

/// <summary>GlobalTestClass summary.</summary>
internal class GlobalTestClass
{
    /// <summary>NestedTestClass summary.</summary>
    private class NestedTestClass
    {
    }
}