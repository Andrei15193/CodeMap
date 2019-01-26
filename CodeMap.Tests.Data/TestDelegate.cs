using System.Collections.Generic;

namespace CodeMap.Tests.Data
{
    /// <summary/>
    public unsafe delegate void TestDelegate<TMethodParam1>(
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
        string param39 = "test"
    );
}