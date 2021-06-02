using QuesoStruct.Types.Collections;
using System.IO;
using System.Text;

namespace QuesoStruct.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var serializer = Serializers.Get<QuesoTestFile>();

            QuesoTestFile test;
            using (var fileOut = File.Create("test_input.bin"))
            {
                var context = new Context(fileOut, Context.SystemEndianess, Encoding.ASCII);

                test = new QuesoTestFile();
                test.Items = new LinkedList.Double<QuesoTestStruct>
                {
                    new QuesoTestStruct(test) { X = 1, Y = 1, Z = 1, W = 1 },
                    new QuesoTestStruct(test) { X = 2, Y = 2, Z = 2, W = 2 },
                    new QuesoTestStruct(test) { X = 3, Y = 3, Z = 3, W = 3 },
                    new QuesoTestStruct(test) { X = 4, Y = 4, Z = 4, W = 4 },
                    new QuesoTestStruct(test) { X = 5, Y = 5, Z = 5, W = 5 },
                };

                serializer.Write(test, context);
                context.RewriteUnresolvedReferences();
            }

            using (var fileIn = File.OpenRead("test_input.bin"))
            {
                var context = new Context(fileIn, Context.SystemEndianess, Encoding.ASCII);

                test = serializer.Read(context);
            }

            using (var fileOut = File.Create("test_output.bin"))
            {
                var context = new Context(fileOut, Context.SystemEndianess, Encoding.ASCII);

                serializer.Write(test, context);
            }
        }
    }
}
