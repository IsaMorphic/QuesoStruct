using System;
using System.IO;

namespace DeltaStruct.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TestStruct1.Init();

            using var file = File.Create("test.bin");
            var context = new Context(file, Context.SystemEndianess);

            var serializer = Serializers.Get<TestStruct1>();
            var testStruct = new TestStruct1
            {
                num1 = 1,
                num2 = 2,
                test1 = new TestStruct2
                {
                    num1 = -1,
                    num2 = 3,
                    num3 = 7
                }
            };

            serializer.WriteToStream(testStruct, file);
        }
    }
}
