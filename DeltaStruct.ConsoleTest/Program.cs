using System;
using System.IO;

namespace DeltaStruct.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TestStruct1.Init();

            var s = Serializers.Get<TestStruct1>(new Context(null, 0, Endianess.Little));
            var inst = s.ReadFromStream(Stream.Null);

            Console.WriteLine($"inst.num1 = {inst.num1}");
            Console.WriteLine($"inst.num2 = {inst.num2}");
            Console.WriteLine($"inst.test1.num1 = {inst.test1.num1}");
            Console.WriteLine($"inst.test1.num2 = {inst.test1.num2}");
            Console.WriteLine($"inst.test1.num3 = {inst.test1.num3}");
        }
    }
}
