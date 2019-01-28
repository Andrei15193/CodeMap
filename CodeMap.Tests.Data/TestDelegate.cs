using System.Collections.Generic;

namespace CodeMap.Tests.Data
{
    /// <summary/>
    [Test("delegate test 1", Value2 = "delegate test 2", Value3 = "delegate test 3")]
    [return: Test("delegate return test 1", Value2 = "delegate return test 2", Value3 = "delegate return test 3")]
    public unsafe delegate void TestDelegate<TParam1>(
        [Test("delegate parameter test 1", Value2 = "delegate parameter test 2", Value3 = "delegate parameter test 3")]
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
        TParam1 param22,
        ref TParam1 param23,
        out TParam1 param24,
        int* param25,
        byte*[] param26,
        ref char* param27,
        out double* param28,
        ref decimal*[] param29,
        out short*[] param30,
        void* param31,
        void** param32,
        ref void** param33,
        out void** param34,
        void**[] param35,
        ref void**[] param36,
        out void**[] param37,
        string param38 = "test"
    );
}