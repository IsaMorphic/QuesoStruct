using QuesoStruct.Types.Collections;

namespace QuesoStruct.ConsoleTest
{
    [StructType]
    public partial class QuesoTestFile
    {
        [StructMember]
        public LinkedList.Double<QuesoTestStruct> Items { get; set; }
    }
}
