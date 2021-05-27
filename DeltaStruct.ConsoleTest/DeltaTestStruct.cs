using DeltaStruct.Types.Collections;
using DeltaStruct.Types.Pointers;

namespace DeltaStruct.ConsoleTest
{
    [StructType]
    public partial class DeltaTestStruct : IPointerOwner, ISinglyLinkedItem<DeltaTestStruct>
    {
        [StructMember]
        public byte X { get; set; }

        [StructMember]
        public byte Y { get; set; }

        [StructMember]
        public byte Z { get; set; }

        [StructMember]
        public byte W { get; set; }

        [StructMember]
        [AutoInitialize]
        public SInt16Pointer<DeltaTestStruct> Next { get; set; }

        DeltaTestStruct ISinglyLinkedItem<DeltaTestStruct>.Next { get => Next.Instance; set => Next.Instance = value; }

        public IStructInstance RelativeOffsetBase => Parent.Parent;
        public long AddedOffsetFromBase => 0;
        public long NullOffsetValue => -1;
    }
}
