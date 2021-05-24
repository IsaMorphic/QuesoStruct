using DeltaStruct.Types;
using DeltaStruct.Types.Pointers;

namespace DeltaStruct.ConsoleTest
{
    [StructType]
    public partial class TestStruct1
    {
        [StructMember]
        public int num1 { get; set; }

        [StructMember]
        public long num2 { get; set; }

        [StructMember]
        Absolute.UInt16Pointer<Primitives.NullTerminatingString> string1 { get; set; }

        [StructMember]
        public TestStruct2 test2 { get; set; }

        public void OnAfterRead() { }
        public void OnBeforeWrite(Context context) { }
    }
}
