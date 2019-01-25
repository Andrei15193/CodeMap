﻿using System;
using System.Collections.Generic;

namespace CodeMap.Tests.TestData
{
    internal unsafe interface ITestInterface<TParam1> : ITestExtendedBaseInterface
    {
        [Test("interface event test 1", Value2 = "interface event test 2", Value3 = "interface event test 3")]
        event EventHandler<EventArgs> TestEvent;

        [Test("interface indexer test 1", Value2 = "interface indexer test 2", Value3 = "interface indexer test 3")]
        int this[
            int param1,
            byte[] param2,
            char[][] param3,
            double[,] param4,
            TestClass<int>.NestedTestClass<byte, IEnumerable<string>> param5,
            TestClass<int>.NestedTestClass<byte, IEnumerable<string>>[] param6,
            dynamic param7,
            int* param8,
            byte*[] param9,
            void* param10,
            void** param11,
            void**[] param12,
            TParam1 param13,
            string param14 = "test"]
        {
            [Test("interface indexer getter test 1", Value2 = "interface indexer getter test 2", Value3 = "interface indexer getter test 3")]
            [return: Test("interface indexer getter return test 1", Value2 = "interface indexer getter return test 2", Value3 = "interface indexer getter return test 3")]
            get;
            [Test("interface indexer setter test 1", Value2 = "interface indexer setter test 2", Value3 = "interface indexer setter test 3")]
            [return: Test("interface indexer setter return test 1", Value2 = "interface indexer setter return test 2", Value3 = "interface indexer setter return test 3")]
            set;
        }

        [Test("interface property test 1", Value2 = "interface property test 2", Value3 = "interface property test 3")]
        byte TestProperty
        {
            [Test("interface property getter test 1", Value2 = "interface property getter test 2", Value3 = "interface property getter test 3")]
            [return: Test("interface property getter return test 1", Value2 = "interface property getter return test 2", Value3 = "interface property getter return test 3")]
            get;
            [Test("interface property setter test 1", Value2 = "interface property setter test 2", Value3 = "interface property setter test 3")]
            [return: Test("interface property setter return test 1", Value2 = "interface property setter return test 2", Value3 = "interface property setter return test 3")]
            set;
        }

        new void InterfaceShadowedTestMethod();

        new int InterfaceShadowedTestProperty { get; set; }

        [Test("interface method test 1", Value2 = "interface method test 2", Value3 = "interface method test 3")]
        [return: Test("interface method return test 1", Value2 = "interface method return test 2", Value3 = "interface method return test 3")]
        void TestMethod<TMethodParam1>(
            [Test("interface method parameter test 1", Value2 = "interface method parameter test 2", Value3 = "interface method parameter test 3")]
            int param1,
            byte[] param2,
            char[][] param3,
            double[,] param4,
            ref int param5,
            ref byte[] param6,
            ref char[][] param7,
            ref double[,] param8,
            out int param9,
            out byte[] param10,
            out char[][] param11,
            out double[,] param12,
            TestClass<int>.NestedTestClass<byte, IEnumerable<string>> param13,
            TestClass<int>.NestedTestClass<byte, IEnumerable<string>>[] param14,
            ref TestClass<int>.NestedTestClass<byte, IEnumerable<string>> param15,
            out TestClass<int>.NestedTestClass<byte, IEnumerable<string>> param16,
            ref TestClass<int>.NestedTestClass<byte, IEnumerable<string>>[] param17,
            out TestClass<int>.NestedTestClass<byte, IEnumerable<string>>[] param18,
            dynamic param19,
            ref dynamic param20,
            out dynamic param21,
            TMethodParam1 param22,
            ref TMethodParam1 param23,
            out TMethodParam1 pram24,
            int* param25,
            byte*[] param26,
            ref char* param27,
            out double* param29,
            ref decimal*[] param30,
            out short*[] param31,
            void* param32,
            void** param33,
            ref void** param34,
            out void** param35,
            void**[] param36,
            ref void**[] param37,
            out void**[] param38,
            TParam1 param39,
            ref TParam1 param40,
            out TParam1 param41,
            string param42 = "test");
    }
}