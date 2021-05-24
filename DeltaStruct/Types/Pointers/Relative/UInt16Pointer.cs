namespace DeltaStruct.Types.Pointers
{
    public partial class Relative
    {
        [StructType]
        public partial class UInt16Pointer<TInst> : IStructReference<TInst>
        where TInst : IStructInstance
        {
            public long? OffsetValue => PointerValue + RelativeTo.Offset;

            public void Update()
            {
                PointerValue = (ushort)(Instance.Offset - RelativeTo.Offset);
            }

            [StructMember]
            public ushort PointerValue { get; set; }

            public IStructInstance RelativeTo { get; set; }

            public void OnAfterRead() { }
            public void OnBeforeWrite(Context context) { }
        }
    }
}
