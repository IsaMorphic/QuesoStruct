using DeltaStruct.Types;
using DeltaStruct.Types.Pointers;
using System;
using System.IO;
using System.Text;

namespace DeltaStruct.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TestStruct1.Init();
            TestStruct2.Init();
            Primitives.NullTerminatingString.Init();
            Absolute.UInt16Pointer<Primitives.NullTerminatingString>.Init();

            using var file = File.OpenRead("test.bin");
            var context = new Context(file, Context.SystemEndianess, Encoding.ASCII);

            var serializer = Serializers.Get<TestStruct1>();
            var test1 = serializer.Read(context);
        }
    }
}
