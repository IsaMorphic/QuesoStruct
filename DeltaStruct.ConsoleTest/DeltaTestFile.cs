using DeltaStruct.Types.Collections;

namespace DeltaStruct.ConsoleTest
{
    [StructType]
    public partial class DeltaTestFile
    {
        [StructMember]
        public LinkedList.Double<DeltaTestStruct> Items { get; set; }
    }
}
