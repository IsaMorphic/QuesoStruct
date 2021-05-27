using DeltaStruct.Types.Collections;
using System.IO;
using System.Text;

namespace DeltaStruct.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var serializer = Serializers.Get<DeltaTestFile>();

            DeltaTestFile test;
            using (var fileIn = File.OpenRead("test.bin"))
            {
                var context = new Context(fileIn, Context.SystemEndianess, Encoding.ASCII);

                test = serializer.Read(context);
            }

            using (var fileOut = File.Create("test_output1.bin"))
            {
                var context = new Context(fileOut, Context.SystemEndianess, Encoding.ASCII);

                serializer.Write(test, context);
            }

            using (var fileOut = File.Create("test_output2.bin"))
            {
                var context = new Context(fileOut, Context.SystemEndianess, Encoding.ASCII);

                test = new DeltaTestFile();
                test.Items = new LinkedList.Single<DeltaTestStruct>(test);
                test.Items.Add(new DeltaTestStruct(test.Items) { X = 1, Y = 1, Z = 1, W = 1 });
                test.Items.Add(new DeltaTestStruct(test.Items) { X = 2, Y = 2, Z = 2, W = 2 });
                test.Items.Add(new DeltaTestStruct(test.Items) { X = 3, Y = 3, Z = 3, W = 3 });

                serializer.Write(test, context);
                serializer.Write(test, context);
            }
        }
    }
}
