using DeltaStruct.Types.Collections;

namespace DeltaStruct.ConsoleTest
{
    [StructType]
    public partial class DeltaTestFile
    {
        [StructMember]
        public LinkedList.Single<DeltaTestStruct> Items { get; set; }
    }
}
