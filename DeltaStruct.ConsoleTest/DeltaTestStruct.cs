using DeltaStruct.Types.Collections;
using DeltaStruct.Types.Pointers;

namespace DeltaStruct.ConsoleTest
{
    [StructType]
    public partial class DeltaTestStruct : IPointerOwner, IDoublyLinkedItem<DeltaTestStruct>
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
        public SInt16Pointer<DeltaTestStruct> Prev { get; set; }

        [StructMember]
        [AutoInitialize]
        public SInt16Pointer<DeltaTestStruct> Next { get; set; }

        DeltaTestStruct IDoublyLinkedItem<DeltaTestStruct>.Prev { get => Prev.Instance; set => Prev.Instance = value; }
        DeltaTestStruct ISinglyLinkedItem<DeltaTestStruct>.Next { get => Next.Instance; set => Next.Instance = value; }

        public IStructInstance RelativeOffsetBase => Parent;
        public long AddedOffsetFromBase => 0;

        public bool IsNullPointer(IStructReference refr)
        {
            return refr is SInt16Pointer<DeltaTestStruct> ptr && ptr.PointerValue == -1;
        }

        public void SetNullPointer(IStructReference refr)
        {
            (refr as SInt16Pointer<DeltaTestStruct>).PointerValue = -1;
        }
    }
}
