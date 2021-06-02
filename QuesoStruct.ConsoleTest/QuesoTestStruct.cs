using QuesoStruct.Types.Collections;
using QuesoStruct.Types.Pointers;

namespace QuesoStruct.ConsoleTest
{
    [StructType]
    public partial class QuesoTestStruct : IPointerOwner, IDoublyLinkedItem<QuesoTestStruct>
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
        public SInt16Pointer<QuesoTestStruct> Prev { get; set; }

        [StructMember]
        [AutoInitialize]
        public SInt16Pointer<QuesoTestStruct> Next { get; set; }

        QuesoTestStruct IDoublyLinkedItem<QuesoTestStruct>.Prev { get => Prev.Instance; set => Prev.Instance = value; }
        QuesoTestStruct IDoublyLinkedItem<QuesoTestStruct>.Next { get => Next.Instance; set => Next.Instance = value; }

        public IStructInstance RelativeOffsetBase => Parent;
        public long AddedOffsetFromBase => 0;

        public bool IsNullPointer(IStructReference refr)
        {
            return refr is SInt16Pointer<QuesoTestStruct> ptr && ptr.PointerValue == -1;
        }

        public void SetNullPointer(IStructReference refr)
        {
            (refr as SInt16Pointer<QuesoTestStruct>).PointerValue = -1;
        }
    }
}
